using FIVE.Network.Core;
using FIVE.Network.Lobby;
using System;
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
            /// Connecting a game.
            /// </summary>
            Connecting,

            /// <summary>
            /// Connected to a game hosted.
            /// </summary>
            Client,
        }

        private static NetworkManager Instance { get; set; }

        [SerializeField] private bool hasLobby = true;
        [SerializeField] private string lobbyServerName;
        [SerializeField] private ushort lobbyServerPort;
        [SerializeField] private int lobbyUpdateRate = 5;

        [SerializeField] private ushort gameServerPort = 8889;
        [SerializeField] private int gameServerUpdateRate = 30;

        [SerializeField] private bool debugMode = false;
        public NetworkState State { get; internal set; }
        public int PlayerIndex { get; internal set; }
        private LobbyClient lobbyClient;
        private GameClient gameClient;
        private GameServer gameServer;

        private void Awake()
        {
            if (Instance != null)
            {
                throw new Exception($"{nameof(NetworkManager)} can only be instantiated once");
            }
            Instance = this;
            State = NetworkState.Idle;
            if (debugMode)
            {
                return;
            }

            if (hasLobby)
            {
                lobbyClient = gameObject.AddComponent<LobbyClient>();
                lobbyClient.LobbyServerName = lobbyServerName;
                lobbyClient.LobbyServerPort = lobbyServerPort;
                lobbyClient.UpdateRate = lobbyUpdateRate;
            }
        }


        private void Start()
        {
            if (hasLobby)
            {
                lobbyClient.enabled = true;
            }

            Debug.Log("NetworkManager Started");
        }

        #region Public APIs
        
        public void JoinRoom(Guid guid, string password)
        {
            RoomInfo roomInfo = lobbyClient.GetRoomInfo(guid);
            gameClient = gameObject.AddComponent<GameClient>();
            gameClient.GameServerAddress = roomInfo.Host;
            gameClient.GameServerPort = roomInfo.Port;
            gameClient.enabled = true;
            State = NetworkState.Connecting;
        }

        public void CreateRoom(string roomName, int maxPlayers, bool hasPassword, string password)
        {
            lobbyClient.CreateRoom(roomName, maxPlayers, hasPassword, password);
            gameServer = gameObject.AddComponent<GameServer>();
            gameServer.ListeningPort = gameServerPort;
            gameServer.UpdateRate = gameServerUpdateRate;
            gameServer.enabled = true;
            State = NetworkState.Host;
        }

        public GameObject Instantiate()
        {
            throw new NotImplementedException();
        }
        

        #endregion

    }
}