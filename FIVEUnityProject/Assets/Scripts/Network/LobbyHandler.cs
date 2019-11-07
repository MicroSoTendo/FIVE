using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FIVE.Network
{
    internal class LobbyHandler : NetworkHandler
    {
        public readonly RoomInfo HostRoomInfo;
        public ICollection<RoomInfo> GetRoomInfos => roomInfos.Values;
        public RoomInfo this[Guid guid] => roomInfos[guid];
        /// <summary>
        /// Used for fetching room info from list server.
        /// </summary>
        private readonly TcpClient listServerClient;
        private MD5 md5;

        /// <summary>
        /// Stores all room infos fetched from list server.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        private readonly string listServer;
        private readonly ushort listServerPort;
        private Action ticker;
        public LobbyHandler(string listServer, ushort listServerPort)
        {
            listServerClient = new TcpClient();
            HostRoomInfo = new RoomInfo();
            OnStart += LobbyHandlerOnStart;
            this.listServer = listServer;
            this.listServerPort = listServerPort;
            md5 = MD5.Create();
            ticker = RefreshRoomInfo;
        }

        private readonly ConcurrentQueue<Action> scheduledActions = new ConcurrentQueue<Action>();
        protected override async Task Handler()
        {
            while (true)
            {
                ticker();
                if (scheduledActions.TryDequeue(out Action action))
                {
                    action();
                }
                else
                {
                    await Task.Delay(1000 / 30);
                }
            }
        }

        private void RefreshRoomInfo()
        {
            NetworkStream stream = listServerClient.GetStream();
            stream.Write((ushort)ListServerCode.GetRoomInfos);
            int roomCount = stream.ReadI32();
            roomInfos.Clear();
            for (int i = 0; i < roomCount; i++)
            {
                byte[] roomInfoBuffer = new byte[stream.ReadI32()];
                stream.Read(roomInfoBuffer, 0, roomInfoBuffer.Length);
                var roomInfo = roomInfoBuffer.ToRoomInfo();
                roomInfos.TryAdd(roomInfo.Guid, roomInfo);
            }
        }

        private void LobbyHandlerOnStart()
        {
            listServerClient.Connect(listServer, listServerPort);
        }

        public void CreateRoom()
        {
            scheduledActions.Enqueue(CreateRoomInternal);
        }

        private unsafe void CreateRoomInternal()
        {
            if (!listServerClient.Connected)
            {
                return;
            }
            NetworkStream stream = listServerClient.GetStream();
            byte[] operation = new byte[sizeof(ushort) + sizeof(int)];
            byte[] roomInfoBuffer = HostRoomInfo.ToBytes();
            fixed (byte* pOperation = operation)
            {
                *(ushort*)pOperation = (ushort)ListServerCode.CreateRoom;
                *(int*)(pOperation + sizeof(ushort)) = roomInfoBuffer.Length;
            }
            stream.Write(operation);
            stream.Write(roomInfoBuffer);
            HostRoomInfo.Guid = stream.ReadGuid();
            ticker = AliveTicker;
        }

        private void AliveTicker()
        {
            byte[] buffer = ((ushort)ListServerCode.AliveTick).ToBytes();
            while (true)
            {
                listServerClient.GetStream().Write(buffer);
            }
        }

        public void RemoveRoom()
        {
            scheduledActions.Enqueue(RemoveRoomInternal);
        }

        private void RemoveRoomInternal()
        {
            if (!listServerClient.Connected)
            {
                return;
            }
            NetworkStream stream = listServerClient.GetStream();
            stream.Write(ListServerCode.RemoveRoom);
            stream.Write(HostRoomInfo.Guid);
            ticker = RefreshRoomInfo;
        }

        private void UpdateRoomInfo(ListServerCode code)
        {
            if (listServerClient.Connected)
            {
                NetworkStream stream = listServerClient.GetStream();
                code |= ListServerCode.UpdateRoom;
                stream.Write(code);
                stream.Write(HostRoomInfo.Guid);
                switch (code)
                {
                    case ListServerCode.UpdateName:
                        stream.Write(HostRoomInfo.Name);
                        break;
                    case ListServerCode.UpdateCurrentPlayer:
                        stream.Write(HostRoomInfo.CurrentPlayers);
                        break;
                    case ListServerCode.UpdateMaxPlayer:
                        stream.Write(HostRoomInfo.MaxPlayers);
                        break;
                    case ListServerCode.UpdatePassword:
                        stream.Write(HostRoomInfo.HasPassword);
                        break;
                    default:
                        break;
                }
            }
        }

        public void UpdateRoomName(string name)
        {
            HostRoomInfo.Name = name;
            UpdateRoomInfo(ListServerCode.UpdateName);
        }

        public void UpdateCurrentPlayers(int current)
        {
            HostRoomInfo.CurrentPlayers = current;
            UpdateRoomInfo(ListServerCode.UpdateCurrentPlayer);
        }

        public void UpdateMaxPlayers(int max)
        {
            HostRoomInfo.MaxPlayers = max;
            UpdateRoomInfo(ListServerCode.UpdateMaxPlayer);
        }

        public void UpdatePassword(bool hasPassword, string password)
        {
            HostRoomInfo.HasPassword = hasPassword;
            if (hasPassword)
            {
                HostRoomInfo.SetRoomPassword(password);
            }

            UpdateRoomInfo(ListServerCode.UpdatePassword);
        }

    }
}
