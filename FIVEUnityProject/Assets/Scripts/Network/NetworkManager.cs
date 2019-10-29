using System;
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

        private static NetworkManager instance;
        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerPort;
        [SerializeField] private int updateRate = 30;

        [SerializeField] private ushort gameServerPort = 8889;
        public NetworkState State { get; private set; } = NetworkState.Idle;
        private LobbyHandler lobbyHandler;
        private NetworkHandler inGameHandler;
        public void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
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
    }
}