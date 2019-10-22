using Apathy;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
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
        private Client lobbyInfoClient;
        private Client hostInfoClient;
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        private bool isUpdatingLobby;
        public NetworkState State { get; private set; } = NetworkState.Idle;
        private Server server;
        private Client client;

        public bool IsUpdatingLobby
        {
            get => isUpdatingLobby;
            set
            {
                if (isUpdatingLobby == value)
                {
                    return;
                }
                isUpdatingLobby = value;
                if (value)
                {
                    StartCoroutine(ReceiveLobbyInfo());
                }
            }
        }

        public void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            lobbyInfoClient = new Client();
            lobbyInfoClient.Connect(listServer, listServerInfoPort);
            hostInfoClient = new Client();
            hostInfoClient.Connect(listServer, listServerUpdatePort);
            hostRoomInfo = new RoomInfo();
            StartCoroutine(ReceiveLobbyInfo());
        }

        private IEnumerator ReceiveLobbyInfo()
        {
            while (isUpdatingLobby)
            {
                while (!lobbyInfoClient.Connected)
                {
                    yield return new WaitForSeconds(1f / updateRate);
                }

                lobbyInfoClient.Send(new byte[4]);
                lobbyInfoClient.GetNextMessage(out Message size);
                int count = size.data.Array.ToI32();
                roomInfos.Clear();
                for (int i = 0; i < count; i++)
                {
                    lobbyInfoClient.GetNextMessage(out Message message);
                    byte[] rawData = message.data.Array;
                    if (rawData == null)
                    {
                        continue;
                    }
                    var roomInfo = rawData.ToRoomInfo();
                    roomInfos.TryAdd(roomInfo.Guid, roomInfo);
                    yield return null;
                }
            }
        }

        public static void JoinRoom(Guid roomGuid)
        {
            if (instance.roomInfos.TryGetValue(roomGuid, out RoomInfo roomInfo))
            {
                instance.client = new Client();
                instance.client.Connect(roomInfo.Host.ToString(), roomInfo.Port);
                instance.State = NetworkState.Client;
            }
        }

        public static void CreateRoom()
        {
            if (instance.hostInfoClient.Connected)
            {
                OpCode code = OpCode.CreateRoom;
                byte[] opCode = ((int)code).ToBytes();
                instance.hostInfoClient.Send(opCode);
                byte[] buffer = instance.hostRoomInfo.ToBytes();
                instance.hostInfoClient.Send(buffer.Length.ToBytes());
                instance.hostInfoClient.Send(buffer);
                instance.hostInfoClient.GetNextMessage(out Message guidMessage);
                instance.hostRoomInfo.Guid = guidMessage.data.Array.ToGuid();
                instance.server.Start(instance.hostRoomInfo.Port);
                instance.State = NetworkState.Host;
            }
        }        
        
        public static void RemoveRoom()
        {
            if (instance.hostInfoClient.Connected)
            {
                OpCode code = OpCode.RemoveRoom;
                byte[] opCode = ((int)code).ToBytes();
                instance.hostInfoClient.Send(opCode);
                instance.hostInfoClient.Send(instance.hostRoomInfo.Guid.ToBytes());
                instance.server.Stop();
                instance.State = NetworkState.Idle;
            }
        }

        private static void UpdateRoomInfo(OpCode code)
        {
            if (instance.lobbyInfoClient.Connected)
            {
                code |= OpCode.UpdateRoom;
                instance.hostInfoClient.Send(((int)code).ToBytes());
                instance.hostInfoClient.Send(instance.hostRoomInfo.Guid.ToBytes());
                switch (code)
                {
                    case OpCode.UpdateName:
                        byte[] nameBuffer = instance.hostRoomInfo.Name.ToBytes();
                        instance.hostInfoClient.Send(nameBuffer.Length.ToBytes());
                        instance.hostInfoClient.Send(nameBuffer);
                        break;
                    case OpCode.UpdateCurrentPlayer:
                        instance.hostInfoClient.Send(instance.hostRoomInfo.CurrentPlayers.ToBytes());
                        break;
                    case OpCode.UpdateMaxPlayer:
                        instance.hostInfoClient.Send(instance.hostRoomInfo.MaxPlayers.ToBytes());
                        break;
                    case OpCode.UpdatePassword:
                        instance.hostInfoClient.Send(instance.hostRoomInfo.HasPassword.ToBytes());
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
