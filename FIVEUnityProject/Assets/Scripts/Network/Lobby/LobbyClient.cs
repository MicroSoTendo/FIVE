using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.Network.Lobby
{
    internal sealed class LobbyClient : MonoBehaviour
    {
        public string LobbyServerName { get; set; }
        public int LobbyServerPort { get; set; }
        public int UpdateRate { get; set; } = 10;
        public RoomInfo CurrentRoomInfo { get; } = new RoomInfo();
        public IEnumerable<RoomInfo> RoomInfos => roomInfos.Values;

        private TcpClient client;
        private NetworkStream stream;
        private CancellationTokenSource cts;
        private Task sendTask;
        private Task readTask;
        private ConcurrentQueue<LobbySyncHeader> sendQueue;
        private ConcurrentDictionary<Guid, RoomInfo> roomInfos;
        private Dictionary<LobbySyncHeader, Action> syncHandlersByHeader;

        private void Awake()
        {
            client = new TcpClient();
            enabled = false;
        }

        private void OnEnable()
        {
            client.Connect(LobbyServerName, LobbyServerPort);
            client.BeginConnect(LobbyServerName, LobbyServerPort, OnConnected, null);
        }

        private void OnDisable()
        {
            cts.Cancel();
            client.Close();
        }

        private void OnConnected(IAsyncResult ar)
        {
            cts = new CancellationTokenSource();
            sendQueue = new ConcurrentQueue<LobbySyncHeader>();
            roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
            syncHandlersByHeader = new Dictionary<LobbySyncHeader, Action>
            {
                {LobbySyncHeader.AliveTick , AliveTickHandler},
                {LobbySyncHeader.RoomInfos, RoomInfosHandler},
                {LobbySyncHeader.AssignGuid, AssignGuidHandler},
                {LobbySyncHeader.CreateRoom, CreateRoomHandler},
                {LobbySyncHeader.RemoveRoom, RemoveRoomHandler},
                {LobbySyncHeader.UpdateName, UpdateNameHandler},
                {LobbySyncHeader.UpdateCurrentPlayer, UpdateCurrentPlayerHandler},
                {LobbySyncHeader.UpdateMaxPlayer, UpdateMaxPlayerHandler},
                {LobbySyncHeader.UpdatePassword, UpdatePasswordHandler}
            };
            stream = client.GetStream();
            sendTask = SendAsync(cts.Token);
            readTask = ReadAsync(cts.Token);
        }

        private async Task ReadAsync(CancellationToken ct)
        {

            while (!ct.IsCancellationRequested)
            {
                LobbySyncHeader header = stream.Read(1).As<LobbySyncHeader>();
                syncHandlersByHeader[header]();
                await Task.Delay(1000 / UpdateRate, ct);
            }
        }

        private async Task SendAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                if (sendQueue.TryDequeue(out LobbySyncHeader header))
                {
                    syncHandlersByHeader[header]();
                }
                else
                {
                    syncHandlersByHeader[LobbySyncHeader.AliveTick]();
                }
                await Task.Delay(1000 / UpdateRate, ct);
            }
        }
        
        #region Sync Handlers
        private void RoomInfosHandler()
        {

            roomInfos.Clear();
            int roomCount = stream.ReadI32();
            for (int i = 0; i < roomCount; i++)
            {
                byte[] roomInfoBuffer = new byte[stream.ReadI32()];
                stream.Read(roomInfoBuffer, 0, roomInfoBuffer.Length);
                var roomInfo = roomInfoBuffer.ToRoomInfo();
                roomInfos.TryAdd(roomInfo.Guid, roomInfo);
            }
        }

        private void AliveTickHandler()
        {
            client.GetStream().WriteByte((byte)LobbySyncHeader.AliveTick);
        }
        private void CreateRoomHandler()
        {
            client.GetStream().Write(CurrentRoomInfo.FullPacket());
        }

        private void AssignGuidHandler()
        {
            CurrentRoomInfo.Guid = client.GetStream().ReadGuid();
        }

        private void RemoveRoomHandler()
        {
            client.GetStream().WriteByte((byte)LobbySyncHeader.RemoveRoom);
        }

        private void UpdateNameHandler()
        {
            client.GetStream().Write(CurrentRoomInfo.NamePacket());
        }


        private void UpdateCurrentPlayerHandler()
        {
            client.GetStream().Write(CurrentRoomInfo.CurrentPlayerPacket());
        }

        private void UpdateMaxPlayerHandler()
        {
            client.GetStream().Write(CurrentRoomInfo.MaxPlayerPacket());
        }

        private void UpdatePasswordHandler()
        {
            client.GetStream().Write(CurrentRoomInfo.PasswordPacket());
        }

        #endregion

        #region Public APIs

        public RoomInfo GetRoomInfo(Guid guid)
        {
            return roomInfos[guid];
        }

        public void CreateRoom(string roomName, int maxPlayers, bool hasPassword, string password)
        {
            sendQueue.Enqueue(LobbySyncHeader.CreateRoom);
        }

        public void RemoveRoom()
        {
            sendQueue.Enqueue(LobbySyncHeader.RemoveRoom);
        }

        public void UpdateRoomName(string newName)
        {
            CurrentRoomInfo.Name = newName;
            sendQueue.Enqueue(LobbySyncHeader.UpdateName);
        }

        public void UpdateCurrentPlayers(int current)
        {
            CurrentRoomInfo.CurrentPlayers = current;
            sendQueue.Enqueue(LobbySyncHeader.UpdateCurrentPlayer);
        }

        public void UpdateMaxPlayers(int max)
        {
            CurrentRoomInfo.MaxPlayers = max;
            sendQueue.Enqueue(LobbySyncHeader.UpdateMaxPlayer);
        }

        public void UpdatePassword(bool hasPassword, string password)
        {
            if (hasPassword)
            {
                CurrentRoomInfo.SetRoomPassword(password);
            }
            if (CurrentRoomInfo.HasPassword != hasPassword)
            {
                sendQueue.Enqueue(LobbySyncHeader.UpdatePassword);
            }
        }
        #endregion

    }

    internal static class LobbyClientExtension
    {
        private static readonly MD5 Md5 = MD5.Create();
        internal static void SetRoomPassword(this RoomInfo roomInfo, string password)
        {
            roomInfo.HashedPassword = Md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        internal static unsafe byte[] FullPacket(this RoomInfo roomInfo)
        {
            const int headerSize =
                sizeof(byte) + //header
                sizeof(int); //size of following
            const int roomInfoFixedSize =
                sizeof(int) + //current player
                sizeof(int) + //max player
                sizeof(bool) + //password flag
                sizeof(ushort); //listening port
            string name = roomInfo.Name;
            int nameBytesCount = Encoding.Unicode.GetByteCount(name);
            byte[] buffer = new byte[headerSize + roomInfoFixedSize + nameBytesCount];
            buffer[0] = (byte)LobbySyncHeader.CreateRoom;
            fixed (byte* pBuffer = &buffer[1])
            {
                *(int*)pBuffer = roomInfoFixedSize + nameBytesCount;
                *(int*)(pBuffer + sizeof(int)) = roomInfo.CurrentPlayers;
                *(int*)(pBuffer + sizeof(int) + sizeof(int)) = roomInfo.MaxPlayers;
                *(bool*)(pBuffer + sizeof(int) + sizeof(int) + sizeof(int)) = roomInfo.HasPassword;
                *(ushort*)(pBuffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool)) = roomInfo.Port;
            }
            Encoding.Unicode.GetBytes(name, 0, name.Length, buffer, headerSize + roomInfoFixedSize);
            return buffer;
        }

        internal static unsafe Guid ReadGuid(this NetworkStream stream)
        {
            byte[] buffer = new byte[16];
            stream.Read(buffer, 0, buffer.Length);
            fixed (byte* pBytes = buffer)
            {
                var guid = (Guid*)pBytes;
                return *guid;
            }
        }

        internal static byte[] NamePacket(this RoomInfo roomInfo)
        {
            string str = roomInfo.Name;
            byte[] buffer = new byte[sizeof(LobbySyncHeader) + sizeof(int) + Encoding.Unicode.GetByteCount(str)];
            buffer[0] = (byte)LobbySyncHeader.UpdateName;
            Encoding.Unicode.GetBytes(str, 0, str.Length, buffer, 5);
            return buffer;
        }

        internal static unsafe byte[] CurrentPlayerPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[sizeof(LobbySyncHeader) + sizeof(int)];
            buffer[0] = (byte)LobbySyncHeader.UpdateCurrentPlayer;
            fixed (byte* pBuffer = &buffer[sizeof(LobbySyncHeader)])
            {
                *(int*)pBuffer = roomInfo.CurrentPlayers;
            }
            return buffer;
        }

        internal static unsafe byte[] MaxPlayerPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[sizeof(LobbySyncHeader) + sizeof(int)];
            buffer[0] = (byte)LobbySyncHeader.UpdateMaxPlayer;
            fixed (byte* pBuffer = &buffer[sizeof(LobbySyncHeader)])
            {
                *(int*)pBuffer = roomInfo.MaxPlayers;
            }
            return buffer;
        }

        internal static byte[] PasswordPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)LobbySyncHeader.UpdatePassword;
            buffer[1] = roomInfo.HasPassword ? (byte)1 : (byte)0;
            return buffer;
        }
    }
}
