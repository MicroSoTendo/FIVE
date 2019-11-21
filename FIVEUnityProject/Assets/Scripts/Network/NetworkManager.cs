using FIVE.FIVE.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static NetworkManager Instance { get; private set; }
        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerPort;
        [SerializeField] private int updateRate = 30;
        [SerializeField] private ushort gameServerPort = 8889;
        [SerializeField] private bool localMode = false;
        public NetworkState State { get; internal set; }

        public ICollection<RoomInfo> RoomInfos => lobbyHandler.GetRoomInfos;
        public RoomInfo CurrentRoomInfo { get; private set; } = new RoomInfo();
        private LobbyHandler lobbyHandler;

        public int PlayerIndex { get; internal set; }
        public BijectMap<int, MethodInfo> RpcInfos { get; internal set; }
        private NetworkGameHandler networkGameHandler;

        private float updateTimer;

        public void Awake()
        {
            if (Instance != null)
            {
                throw new Exception($"{nameof(NetworkManager)} can only be instantiated once");
            }
            Instance = this;
            State = NetworkState.Idle;
            if (!localMode)
            {
                lobbyHandler = new LobbyHandler(listServer, listServerPort);
                lobbyHandler.Start();
            }
        }

        private class MethodComparer : IComparer<MethodInfo>
        {
            public int Compare(MethodInfo x, MethodInfo y)
            {
                return x == null ? 1 : x.Name.CompareTo(y);
            }
        }

        public IEnumerator Start()
        {
            var methodsInfos = new SortedSet<MethodInfo>(new MethodComparer());
            foreach (Assembly assembly in RPCAttribute.ValidAssemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IRpcInvokeable).IsAssignableFrom(type))
                    {
                        foreach (MethodInfo methodInfo in type.GetMethods())
                        {
                            if (methodInfo.GetCustomAttributes(typeof(RPCAttribute), false).Length > 0)
                            {
                                methodsInfos.Add(methodInfo);
                            }
                        }
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
            if (methodsInfos.Count > 0)
            {
                RpcInfos = new BijectMap<int, MethodInfo>();
                int counter = 0;
                foreach (MethodInfo methodsInfo in methodsInfos)
                {
                    RpcInfos.Add(counter++, methodsInfo);
                    yield return new WaitForFixedUpdate();
                }
            }
            Debug.Log("NetworkManager Started");
        }

        public void JoinRoom(Guid guid, string password)
        {
            CurrentRoomInfo = lobbyHandler[guid];
            CurrentRoomInfo.SetRoomPassword(password);

            networkGameHandler = new ClientGameHandler();
            networkGameHandler.Start();
            State = NetworkState.Connecting;
        }

        public void CreateRoom(string roomName, int maxPlayers, bool hasPassword, string password)
        {
            CurrentRoomInfo.Name = roomName;
            CurrentRoomInfo.Port = gameServerPort;
            CurrentRoomInfo.CurrentPlayers = 1; //Host self
            CurrentRoomInfo.HasPassword = hasPassword;
            CurrentRoomInfo.MaxPlayers = maxPlayers;
            if (hasPassword)
            {
                CurrentRoomInfo.SetRoomPassword(password);
            }
            lobbyHandler.CreateRoom();

            networkGameHandler = new HostGameHandler();
            networkGameHandler.Start();
            State = NetworkState.Host;
        }

        public void Disconnect()
        {
            lobbyHandler.RemoveRoom();
        }

        public void LateUpdate()
        {
            if (!localMode && State != NetworkState.Idle)
            {
                updateTimer += Time.deltaTime;
                if (updateTimer > 1f / updateRate)
                {
                    networkGameHandler.LateUpdate();
                    updateTimer = 0;
                }
            }
        }
    }
}