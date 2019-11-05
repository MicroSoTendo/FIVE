using FIVE.Network.Serializers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviour
    {
        /// <summary>
        /// Describes state of this instance.
        /// </summary>
        public enum NetworkState
        {
            /// <summary>
            /// Networked game is not running.
            /// </summary>
            Idle,

            /// <summary>
            /// Hosting a networked game.
            /// </summary>
            Host,

            /// <summary>
            /// Connected to a game hosted.
            /// </summary>
            Client,
        }

        public static NetworkManager Instance { get; private set; }
        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerPort;
        [SerializeField] private int updateRate = 30;
        [SerializeField] private ushort gameServerPort = 8889;
        public NetworkState State { get; private set; } = NetworkState.Idle;
        private LobbyHandler lobbyHandler;
        private NetworkHandler inGameHandler;
        private ConcurrentQueue<MainThreadRequest>[] poolRequests;
        private bool[] isPoolRunning;
        private const int PoolMask = 0b11;
        private int nextPoolIndex = 0;
        private int NextPoolIndex
        {
            get
            {
                nextPoolIndex &= PoolMask;
                return nextPoolIndex++;
            }
        }

        public void Awake()
        {
            if (Instance != null)
            {
                throw new Exception($"{nameof(NetworkManager)} can only be instantiated once");
            }

            Instance = this;
            poolRequests = new ConcurrentQueue<MainThreadRequest>[PoolMask + 1];
            isPoolRunning = new bool[PoolMask + 1];
            for (int i = 0; i < PoolMask + 1; i++)
            {
                poolRequests[i] = new ConcurrentQueue<MainThreadRequest>();
                isPoolRunning[i] = true;
                StartCoroutine(ResolveRequest(i));
            }
            lobbyHandler = new LobbyHandler(listServer, listServerPort);
        }

        public void Start()
        {
            lobbyHandler.Start();
        }

        public void JoinRoom(Guid guid, string password)
        {
            RoomInfo roomInfo = lobbyHandler[guid];
            roomInfo.SetRoomPassword(password);
            inGameHandler = new ClientHandler(new TcpClient(),
                HandShaker.GetClientHandShaker(roomInfo));
            inGameHandler.Start();
        }

        public void CreateRoom(string roomName, bool hasPassword, string password)
        {
            RoomInfo roomInfo = lobbyHandler.HostRoomInfo;
            roomInfo.Name = roomName;
            roomInfo.Port = gameServerPort;
            roomInfo.CurrentPlayers = 1; //Host self
            roomInfo.HasPassword = hasPassword;
            if (hasPassword)
            {
                roomInfo.SetRoomPassword(password);
            }

            lobbyHandler.CreateRoom();
            inGameHandler = new HostHandler(new TcpListener(IPAddress.Loopback, gameServerPort),
                HandShaker.GetHostHandShaker(lobbyHandler.HostRoomInfo));
            inGameHandler.Start();
        }

        public void Submit(MainThreadRequest request)
        {
            poolRequests[NextPoolIndex].Enqueue(request);
        }

        private IEnumerator ResolveRequest(int i)
        {
            ConcurrentQueue<MainThreadRequest> queue = poolRequests[i];
            while (isPoolRunning[i])
            {
                if (queue.TryDequeue(out MainThreadRequest request))
                {
                    switch (request)
                    {
                        case CreateObject createObject:
                            break;
                        case RemoveObject removeObject:
                            break;
                        case SyncComponent syncComponent:
                            ResolveSync(syncComponent);
                            break;
                    }
                }
                yield return null;
            }
        }

        private unsafe void ResolveSync(SyncComponent syncComponent)
        {
            byte[] rawBytes = syncComponent.RawBytes;
            fixed (byte* pBytes = rawBytes)
            {
                int gameObjectID = *(int*)pBytes;
                GameObject go = SyncCenter.Instance.NetworkedGameObjects[gameObjectID];
                int count = *((int*)pBytes + 1);
                int offset = 8;
                for (int i = 0; i < count; i++)
                {
                    var type = (ComponentType)(*(pBytes + offset));
                    offset += 1;
                    switch (type)
                    {
                        case ComponentType.Transform:
                            Serializer<Transform>.Instance.Deserialize(pBytes + offset, go.transform);
                            offset += 24;
                            break;
                        case ComponentType.Animator:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}