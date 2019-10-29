using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        private ConcurrentQueue<NetworkRequest> requests;
        public void Awake()
        {
            Assert.IsNull(Instance);
            Instance = this;
            requests = new ConcurrentQueue<NetworkRequest>();
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
                roomInfo.SetRoomPassword(password);
            lobbyHandler.CreateRoom();
            inGameHandler = new HostHandler(new TcpListener(IPAddress.Loopback, gameServerPort),
                HandShaker.GetHostHandShaker(lobbyHandler.HostRoomInfo));
            inGameHandler.Start();
        }

        public void Submit(NetworkRequest request)
        {
            requests.Enqueue(request);
        }

        private IEnumerator ResolveRequest(NetworkRequest request)
        {
            while (true)
            {
                switch (request)
                {
                    case CreateObject createObject:
                        
                        break;
                    case RemoveObject removeObject:
                        break;
                }
                yield return null;
            }
        }


        public void Update()
        {
            while (requests.Count > 0)
            {
                requests.TryDequeue(out NetworkRequest request);
                StartCoroutine(ResolveRequest(request));
            }
        }



    }
}