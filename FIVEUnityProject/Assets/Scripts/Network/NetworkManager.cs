using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Assertions;
namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviour
    {
        public enum NetworkState
        {
            Idle,
            Host,
            Client,
        }
        private static NetworkManager instance;
        private RoomInfo hostRoomInfo;
        private string roomPassword;

        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerInfoPort;
        [SerializeField] private ushort listServerUpdatePort;
        [SerializeField] private int updateRate = 30;
        private TcpClient lobbyInfoClient;
        private TcpClient hostInfoClient;
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        public NetworkState State { get; private set; } = NetworkState.Idle;

        private TcpListener server;
        private TcpClient client;

        public void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            lobbyInfoClient = new TcpClient();
            lobbyInfoClient.Connect(listServer, listServerInfoPort);
            hostInfoClient = new TcpClient();
            hostInfoClient.Connect(listServer, listServerUpdatePort);
            hostRoomInfo = new RoomInfo();
            StartCoroutine(ReceiveLobbyInfo());
        }

        private IEnumerator ReceiveLobbyInfo()
        {
            while (true)
            {
                while (!lobbyInfoClient.Connected)
                {
                    yield return new WaitForSeconds(1f / updateRate);
                }

                NetworkStream stream = lobbyInfoClient.GetStream();
                stream.Write(new byte[4], 0, 4);
                byte[] sizeBuffer = new byte[4];
                stream.Read(sizeBuffer, 0, 4);
                int count = sizeBuffer.ToI32();
                roomInfos.Clear();
                for (int i = 0; i < count; i++)
                {
                    byte[] roomInfoBufferSize = new byte[4];
                    stream.Read(roomInfoBufferSize, 0, 4);
                    byte[] roomInfoBuffer = new byte[roomInfoBufferSize.ToI32()];
                    stream.Read(roomInfoBuffer, 0, roomInfoBuffer.Length);
                    var roomInfo = roomInfoBuffer.ToRoomInfo();
                    roomInfos.TryAdd(roomInfo.Guid, roomInfo);
                    yield return null;
                }
                yield return new WaitForSeconds(15f/updateRate);
            }
        }

        public static void JoinRoom(Guid roomGuid)
        {
            if (instance.roomInfos.TryGetValue(roomGuid, out RoomInfo roomInfo))
            {
                instance.client = new TcpClient();
                instance.client.Connect(roomInfo.Host.ToString(), roomInfo.Port);
                instance.State = NetworkState.Client;
            }
        }

        public static void CreateRoom(string name, int maxPlayers, bool hasPassword, string password)
        {
            instance.hostRoomInfo.Name = name;
            instance.hostRoomInfo.MaxPlayers = maxPlayers;
            instance.hostRoomInfo.CurrentPlayers = 1;
            instance.hostRoomInfo.Port = 8889;
            instance.hostRoomInfo.HasPassword = hasPassword;
            instance.roomPassword = password;
            if (instance.hostInfoClient.Connected)
            {
                NetworkStream stream = instance.hostInfoClient.GetStream();
                OpCode code = OpCode.CreateRoom;
                stream.Write((int)code);
                stream.Write(instance.hostRoomInfo);
                instance.hostRoomInfo.Guid = stream.Read(16).ToGuid();
                instance.server = new TcpListener(IPAddress.Loopback, instance.hostRoomInfo.Port);
                instance.server.Start();
                instance.State = NetworkState.Host;
            }
        }        
        
        public static void RemoveRoom()
        {
            if (instance.hostInfoClient.Connected)
            {
                NetworkStream stream = instance.hostInfoClient.GetStream();
                OpCode code = OpCode.RemoveRoom;
                stream.Write((int)code);
                stream.Write(instance.hostRoomInfo.Guid);
                instance.server.Stop();
                instance.State = NetworkState.Idle;
            }
        }

        private static void UpdateRoomInfo(OpCode code)
        {
            if (instance.lobbyInfoClient.Connected)
            {
                NetworkStream stream = instance.hostInfoClient.GetStream();
                code |= OpCode.UpdateRoom;
                stream.Write((int)code);
                stream.Write(instance.hostRoomInfo.Guid);
                switch (code)
                {
                    case OpCode.UpdateName:
                        stream.Write(instance.hostRoomInfo.Name);
                        break;
                    case OpCode.UpdateCurrentPlayer:
                        stream.Write(instance.hostRoomInfo.CurrentPlayers);
                        break;
                    case OpCode.UpdateMaxPlayer:
                        stream.Write(instance.hostRoomInfo.MaxPlayers);
                        break;
                    case OpCode.UpdatePassword:
                        stream.Write(instance.hostRoomInfo.HasPassword);
                        break;
                    default:
                        break;
                }
            }
        }

        public static void UpdateRoomName(string name)
        {
            instance.hostRoomInfo.Name = name;
            UpdateRoomInfo(OpCode.UpdateName | OpCode.UpdateRoom);
        }
        public static void UpdateCurrentPlayers(int current)
        {
            instance.hostRoomInfo.CurrentPlayers = current;
            UpdateRoomInfo(OpCode.UpdateCurrentPlayer | OpCode.UpdateRoom);
        }
        public static void UpdateMaxPlayers(int max)
        {
            instance.hostRoomInfo.MaxPlayers = max;
            UpdateRoomInfo(OpCode.UpdateMaxPlayer | OpCode.UpdateRoom);
        }
        public static void UpdatePassword(bool hasPassword, string password)
        {
            instance.hostRoomInfo.HasPassword = hasPassword;
            UpdateRoomInfo(OpCode.UpdatePassword | OpCode.UpdateRoom);
        }
    }
}
