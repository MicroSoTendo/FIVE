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
        private ConcurrentQueue<NetworkRequest>[] poolRequests;
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
            Assert.IsNull(Instance);
            Instance = this;
            poolRequests = new ConcurrentQueue<NetworkRequest>[PoolMask + 1];
            isPoolRunning = new bool[PoolMask + 1];
            for (int i = 0; i < PoolMask + 1; i++)
            {
                poolRequests[i] = new ConcurrentQueue<NetworkRequest>();
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

        public void Submit(NetworkRequest request)
        {
            poolRequests[NextPoolIndex].Enqueue(request);
        }

        private IEnumerator ResolveRequest(int i)
        {
            ConcurrentQueue<NetworkRequest> queue = poolRequests[i];
            while (isPoolRunning[i])
            {
                if (queue.TryDequeue(out NetworkRequest request))
                {
                    switch (request)
                    {
                        case CreateObject createObject:
                            break;
                        case RemoveObject removeObject:
                            break;
                    }
                }
                yield return null;
            }
        }
    }
}