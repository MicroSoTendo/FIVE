using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        public ICollection<RoomInfo> RoomInfos => lobbyHandler.GetRoomInfos;
        public int PlayerIndex { get; internal set; }
        internal int PrivateID { get; set; }
        public NetworkState State { get; internal set; } = NetworkState.Idle;
        private LobbyHandler lobbyHandler;
        private NetworkHandler networkHandler;
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
        public IEnumerator Start()
        {
            lobbyHandler.Start();
            yield return null;
        }

        public void JoinRoom(Guid guid, string password)
        {
            RoomInfo roomInfo = lobbyHandler[guid];
            roomInfo.SetRoomPassword(password);
            networkHandler = new ClientHandler(new TcpClient(), HandShaker.GetClientHandShaker(roomInfo));
            networkHandler.Start();
        }

        public void CreateRoom(string roomName, int maxPlayers, bool hasPassword, string password)
        {
            RoomInfo roomInfo = lobbyHandler.HostRoomInfo;
            roomInfo.Name = roomName;
            roomInfo.Port = gameServerPort;
            roomInfo.CurrentPlayers = 1; //Host self
            roomInfo.HasPassword = hasPassword;
            roomInfo.MaxPlayers = maxPlayers;
            if (hasPassword)
            {
                roomInfo.SetRoomPassword(password);
            }

            lobbyHandler.CreateRoom();
            networkHandler = new HostHandler(new TcpListener(IPAddress.Loopback, gameServerPort),
                HandShaker.GetHostHandShaker(lobbyHandler.HostRoomInfo));
            networkHandler.Start();
            State = NetworkState.Host;
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
                    request.Resolve();
                }
                yield return null;
            }
        }

        public void Disconnect()
        {
            lobbyHandler.RemoveRoom();
        }
    }
}