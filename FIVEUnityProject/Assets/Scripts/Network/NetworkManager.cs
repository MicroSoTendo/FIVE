using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace FIVE.Network
{
    [RequireComponent(typeof(SyncCenter))]
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
        private byte[] hashedPassword;
        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerInfoPort;
        [SerializeField] private ushort listServerUpdatePort;
        [SerializeField] private int updateRate = 30;
        private TcpClient lobbyInfoClient;
        private TcpClient hostInfoClient;
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        public NetworkState State { get; private set; } = NetworkState.Idle;

        //For hosting game
        private TcpListener Server;

        //For join other game
        private TcpClient Client;
        private int AssignedClientID;

        private bool IsReceivingLobbyInfo { get; set; }
        public void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            random = new Random((int)Time.time);
            lobbyInfoClient = new TcpClient();
            lobbyInfoClient.Connect(listServer, listServerInfoPort);
            hostInfoClient = new TcpClient();
            hostInfoClient.Connect(listServer, listServerUpdatePort);
            hostRoomInfo = new RoomInfo();
            IsReceivingLobbyInfo = true;
            StartCoroutine(ReceiveLobbyInfo());
        }

        private IEnumerator ReceiveLobbyInfo()
        {
            while (IsReceivingLobbyInfo)
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
                yield return new WaitForSeconds(15f / updateRate);
            }
        }

        public static ICollection<RoomInfo> GetRoomInfos => instance.roomInfos.Values;

        public static bool TryJoinRoom(Guid roomGuid, string password = "")
        {
            if (instance.roomInfos.TryGetValue(roomGuid, out RoomInfo roomInfo))
            {
                instance.Client = new TcpClient();
                instance.Client.Connect(roomInfo.Host.ToString(), roomInfo.Port);
                return instance.HandShakeToServer(instance.Client, roomInfo.HasPassword, password);
            }
            return false;
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
                instance.Server = new TcpListener(IPAddress.Loopback, instance.hostRoomInfo.Port);
                instance.Server.Start();
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
                instance.Server.Stop();
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

        public static void ClientCreateObject(string name)
        {

        }





        private static readonly Dictionary<int, TcpClient> ConnectedClients = new Dictionary<int, TcpClient>();

        private IEnumerator Host()
        {
            while (true)
            {
                TcpClient playerClient = instance.Server.AcceptTcpClient();
                HandShakeToClient(playerClient);
                yield return null;
            }
        }

        private Random random;

        private int GenerateClientID()
        {
            return random.Next();
        }

        private bool HandShakeToServer(TcpClient client, bool hasPassword, string password)
        {
            NetworkStream stream = instance.Client.GetStream();
            stream.Write((int)OpCode.JoinRequest);
            if (hasPassword)
            {
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
                stream.Write(hash);
            }
            if ((OpCode)stream.Read(4).ToI32() == OpCode.AcceptJoin)
            {
                instance.State = NetworkState.Client;
                AssignedClientID = stream.Read(4).ToI32();
                StartCoroutine(HostHandler(client));
                return true;
            }

            return false;
        }

        private void HandShakeToClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var code = (OpCode)stream.Read(4).ToI32();
            if (code == OpCode.JoinRequest)
            {
                if (hostRoomInfo.HasPassword)
                {
                    byte[] hashedBytes = stream.Read(16);
                    if (!hashedBytes.Equals(hashedPassword))
                    {
                        stream.Write((int)OpCode.RefuseJoin);
                        return;
                    }
                }
                stream.Write((int)OpCode.AcceptJoin);
                int id = GenerateClientID();
                ConnectedClients.Add(id, client);
                stream.Write(id.ToBytes());
                StartCoroutine(ClientHandler(id, client));
            }
            else
            {
                client.Dispose();
            }
        }
        
        public enum ComponentType
        {
            Transform = 0,
            Animator = 1,
        }

        private IEnumerator ClientHandler(int id, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            //Phase 1: send all existed objects
            GameObject[] networkedGameObjects = SyncCenter.NetworkedGameObjects.ToArray();
            stream.Write(networkedGameObjects.Length);
            foreach (GameObject networkedGameObject in networkedGameObjects)
            {
                stream.Write(SyncCenter.GetResourceID(networkedGameObject));
                List<object> list = SyncCenter.GetSynchronizedComponent(networkedGameObject);
                stream.Write(list.Count);
                foreach (object o in list)
                {
                    switch (o)
                    {
                        case Transform goTransform:
                            stream.Write((int)ComponentType.Transform);
                            stream.Write(TransformSerializer.Serialize(goTransform));
                            break;
                        case Animator animator:
                            break;
                    }
                }
            }
            //Phase 2: set up player

            //Phase 3: do sync
            while (true)
            {
                
            }
        }


        private IEnumerator HostHandler(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            //Phase 1: fetch all existed objects
            int count = stream.ReadI32();
            for (int i = 0; i < count; i++)
            {
                int resourceID = stream.ReadI32();
                GameObject go = Instantiate(SyncCenter.GetPrefab(resourceID));
                int componentCount = stream.ReadI32();
                for (int j = 0; j < componentCount; j++)
                {
                    ComponentType componentType = (ComponentType)stream.ReadI32();
                    switch (componentType)
                    {
                        case ComponentType.Transform:
                            byte[] transformBuffer = stream.Read(3 * 4 * 2);
                            TransformSerializer.Deserialize(transformBuffer, go.transform);
                            break;
                        case ComponentType.Animator:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            //Phase 2: set up player

            //Phase 3: do sync
            while (true)
            {

            }
        }

    }
}
