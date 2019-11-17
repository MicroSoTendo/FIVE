using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class LobbyHandler : IDisposable
    {   
        
        /// <summary>
        /// Used for communicating with list server.
        /// </summary>
        [Flags]
        private enum ListServerHeader : byte
        {
            AliveTick = 1,
            RoomInfos = 2,
            AssignGuid = 3,
            CreateRoom = 4,
            RemoveRoom = 5,
            UpdateName = 6,
            UpdateCurrentPlayer = 7,
            UpdateMaxPlayer = 8,
            UpdatePassword = 9
        }

        public RoomInfo HostRoomInfo { get; }
        public ICollection<RoomInfo> GetRoomInfos => roomInfos.Values;
        public RoomInfo this[Guid guid] => roomInfos[guid];
        /// <summary>
        /// Used for fetching room info from list server.
        /// </summary>
        private readonly TcpClient listServerClient;
        private readonly MD5 md5;

        /// <summary>
        /// Stores all room infos fetched from list server.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        private readonly string listServer;
        private readonly ushort listServerPort;
        private Task sendTask;
        private Task readTask;
        private Task timerTask;
        private CancellationTokenSource cancellationTokenSource;

        private int timeout = 0;
        private readonly Action<NetworkStream>[] handlers;

        private readonly ConcurrentQueue<ListServerHeader> sendQueue;

        public LobbyHandler(string listServer, ushort listServerPort)
        {
            listServerClient = new TcpClient();
            HostRoomInfo = new RoomInfo();
            this.listServer = listServer;
            this.listServerPort = listServerPort;
            md5 = MD5.Create();
            sendQueue = new ConcurrentQueue<ListServerHeader>();
            handlers = new Action<NetworkStream>[]
            {
                AliveTickHandler,
                RoomInfosHandler,
                AssignGuidHandler,
                CreateRoomHandler,
                RemoveRoomHandler,
                UpdateNameHandler,
                UpdateCurrentPlayer,  
                UpdateMaxPlayer,
                UpdatePassword
            };
        }

        private void AliveTickHandler(NetworkStream _)
        {
            Interlocked.Exchange(ref timeout, 0);
        }

        private void RoomInfosHandler(NetworkStream stream)
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
        
        private void CreateRoomHandler(NetworkStream stream)
        {
            stream.Write(HostRoomInfo.FullPacket());
        }

        private void AssignGuidHandler(NetworkStream stream)
        {
            HostRoomInfo.Guid = stream.ReadGuid();
        }

        private void RemoveRoomHandler(NetworkStream stream)
        {
            stream.WriteByte((byte)ListServerHeader.RemoveRoom);
        }

        private void UpdateNameHandler(NetworkStream stream)
        {
            stream.Write(HostRoomInfo.NamePacket());
        }  
        
        private void UpdateCurrentPlayer(NetworkStream stream)
        {
            stream.Write(HostRoomInfo.CurrentPlayerPacket());
        }
        
        private void UpdateMaxPlayer(NetworkStream stream)
        {
            stream.Write(HostRoomInfo.MaxPlayerPacket());
        }

        private void UpdatePassword(NetworkStream stream)
        {
            stream.Write(HostRoomInfo.PasswordPacket());
        }

        public void Start()
        {
            listServerClient.Connect(listServer, listServerPort);
            cancellationTokenSource = new CancellationTokenSource();
            sendTask = SendAsync(cancellationTokenSource.Token);
            readTask = ReadAsync(cancellationTokenSource.Token);
            timerTask = TimerAsync(cancellationTokenSource.Token);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            listServerClient.GetStream().Close();
            listServerClient.Close();
        }

        private async Task TimerAsync(CancellationToken ct)
        {
            while (true)
            {
                await Task.Delay(100, ct);
                if (ct.IsCancellationRequested)
                    return;
                Interlocked.Add(ref timeout, 100);
                if (Interlocked.CompareExchange(ref timeout, 0, 0) > 5000)
                {
                    Stop();
                }
            }
        }

        private async Task SendAsync(CancellationToken ct)
        {
            if (listServerClient.Connected)
            {
                return;
            }

            NetworkStream stream = listServerClient.GetStream();
            while (true)
            {
                while (sendQueue.TryDequeue(out ListServerHeader header))
                {
                    if (ct.IsCancellationRequested)
                        return;
                    handlers[(byte)header](stream);
                }
                await Task.Delay(30, ct);
            }
        }

        private async Task ReadAsync(CancellationToken ct)
        {
            if (listServerClient.Connected)
            {
                return;
            }
            NetworkStream stream = listServerClient.GetStream();
            while (true)
            {
                if (ct.IsCancellationRequested)
                    return;
                byte headerBuffer = stream.ReadAByte();
                handlers[headerBuffer](stream);
                await Task.Delay(500, ct);
            }
        }

        public void RemoveRoom()
        {
            sendQueue.Enqueue(ListServerHeader.RemoveRoom);
        }

        public void CreateRoom()
        {
            sendQueue.Enqueue(ListServerHeader.CreateRoom);
        }

        public void UpdateRoomName(string name)
        {
            HostRoomInfo.Name = name;
            sendQueue.Enqueue(ListServerHeader.UpdateName);
        }

        public void UpdateCurrentPlayers(int current)
        {
            HostRoomInfo.CurrentPlayers = current;
            sendQueue.Enqueue(ListServerHeader.UpdateCurrentPlayer);
        }

        public void UpdateMaxPlayers(int max)
        {
            HostRoomInfo.MaxPlayers = max;
            sendQueue.Enqueue(ListServerHeader.UpdateMaxPlayer);
        }

        public void UpdatePassword(bool hasPassword, string password)
        {
            if (hasPassword)
            {
                HostRoomInfo.SetRoomPassword(password);
            }
            if (HostRoomInfo.HasPassword != hasPassword)
            {
                sendQueue.Enqueue(ListServerHeader.UpdatePassword);
            }
        }

        public void Dispose()
        {
            listServerClient?.Dispose();
            md5?.Dispose();
            sendTask?.Dispose();
            readTask?.Dispose();
            timerTask?.Dispose();
        }
    }

    internal static class LobbyHandlerExtension
    {
        internal static unsafe byte[] FullPacket(this RoomInfo roomInfo)
        {
            const int fixedSize =
                sizeof(byte) + //header
                sizeof(int) + //size of roomInfo (exclude itself)
                sizeof(int) + //current player
                sizeof(int) + //max player
                sizeof(bool) + //password flag
                sizeof(ushort); //listening port
            string name = roomInfo.Name;
            byte[] buffer = new byte[fixedSize + Encoding.Unicode.GetByteCount(name)];
            buffer[0] = 4;
            fixed (byte* pBuffer = &buffer[1])
            {
                *(int*)pBuffer = buffer.Length - sizeof(int);
                *(int*)(pBuffer + sizeof(int)) = roomInfo.CurrentPlayers;
                *(int*)(pBuffer + sizeof(int) + sizeof(int)) = roomInfo.MaxPlayers;
                *(bool*)(pBuffer + sizeof(int) + sizeof(int) + sizeof(int)) = roomInfo.HasPassword;
                *(ushort*)(pBuffer + sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool)) = roomInfo.Port;
            }
            Encoding.Unicode.GetBytes(name, 0, name.Length, buffer, 4);
            return buffer;
        }

        internal static unsafe Guid ReadGuid(this NetworkStream stream)
        {
            byte[] buffer = new byte[16];
            stream.Read(buffer, 0, buffer.Length);
            fixed (byte* pBytes = buffer)
            {
                Guid* guid = (Guid*)pBytes;
                return *guid;
            }
        }

        internal static byte[] NamePacket(this RoomInfo roomInfo)
        {
            string str = roomInfo.Name;
            byte[] buffer = new byte[5 + Encoding.Unicode.GetByteCount(str)];
            buffer[0] = 6;
            Encoding.Unicode.GetBytes(str, 0, str.Length, buffer, 5);
            return buffer;
        }        
        
        internal static unsafe byte[] CurrentPlayerPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[5];
            buffer[0] = 7;
            fixed (byte* pBuffer = &buffer[1])
            {
                *(int*)pBuffer = roomInfo.CurrentPlayers;
            }
            return buffer;
        }        
        
        internal static unsafe byte[] MaxPlayerPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[5];
            buffer[0] = 8;
            fixed (byte* pBuffer = &buffer[1])
            {
                *(int*)pBuffer = roomInfo.MaxPlayers;
            }
            return buffer;
        }

        internal static byte[] PasswordPacket(this RoomInfo roomInfo)
        {
            byte[] buffer = new byte[2];
            buffer[0] = 9;
            buffer[1] = roomInfo.HasPassword ? (byte)1 : (byte)0;
            return buffer;
        }
    }
}
