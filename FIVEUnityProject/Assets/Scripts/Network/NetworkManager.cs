using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

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

        /// <summary>
        /// Used by Host only
        /// </summary>
        private static readonly Dictionary<int, TcpClient> ConnectedClients = new Dictionary<int, TcpClient>();

        /// <summary>
        /// Stores all room infos fetched from list server.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, RoomInfo> roomInfos = new ConcurrentDictionary<Guid, RoomInfo>();


        //Run by host
        private readonly ConcurrentBag<(int id, List<GameObject> objects)> toBeSynced =
            new ConcurrentBag<(int id, List<GameObject> objects)>();

        /// <summary>
        /// Client ID fetched from host after connected successfully.
        /// </summary>
        private int assignedClientID;

        /// <summary>
        /// Tcp client used for connecting hosted game and syncing to server.
        /// </summary>
        private TcpClient gameClient;

        /// <summary>
        /// Used by <b>Host only</b>.<br/>
        /// TCP server used for hosting game and syncing with clients.
        /// </summary>
        private TcpListener gameServer;

        private byte[] hashedPassword;

        /// <summary>
        /// Used by <b>Host only</b>.<br/>
        /// Used for updating room info to list server.
        /// </summary>
        private TcpClient hostInfoClient;

        private RoomInfo hostRoomInfo;

        [SerializeField] private string listServer;
        [SerializeField] private ushort listServerInfoPort;
        [SerializeField] private ushort listServerUpdatePort;

        /// <summary>
        /// Used for fetching room info from list server.
        /// </summary>
        private TcpClient lobbyInfoClient;

        private Random random;
        private string roomPassword;
        [SerializeField] private int updateRate = 30;
        public NetworkState State { get; private set; } = NetworkState.Idle;

        /// <summary>
        /// Flag to control whether it is fectching room info from list server.
        /// </summary>
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
            //StartCoroutine(ReceiveLobbyInfo());
        }


        /// <summary>
        /// Used by <b>Host only</b>.<br/>
        /// Handles incoming clients.
        /// </summary>
        /// <returns>Coroutine runs by Unity.</returns>
        private IEnumerator Host()
        {
            while (true)
            {
                TcpClient playerClient = instance.gameServer.AcceptTcpClient();
                HandShakeToClient(playerClient);
                yield return null;
            }
        }

        private int GenerateClientID()
        {
            return random.Next();
        }

        private bool HandShakeToServer(TcpClient client, bool hasPassword, string password)
        {
            NetworkStream stream = gameClient.GetStream();
            stream.Write(OpCode.JoinRequest);
            if (hasPassword)
            {
                byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
                stream.Write(hash);
            }

            if (stream.Read<OpCode>() == OpCode.AcceptJoin)
            {
                State = NetworkState.Client;
                assignedClientID = stream.ReadI32();
                //StartCoroutine(HostHandler(client));
                return true;
            }

            return false;
        }

        private void HandShakeToClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            OpCode code = stream.Read<OpCode>();
            if (code == OpCode.JoinRequest)
            {
                if (hostRoomInfo.HasPassword)
                {
                    byte[] hashedBytes = stream.Read(16);
                    if (!hashedBytes.Equals(hashedPassword))
                    {
                        stream.Write(OpCode.RefuseJoin);
                        return;
                    }
                }

                stream.Write(OpCode.AcceptJoin);
                int id = GenerateClientID();
                ConnectedClients.Add(id, client);
                stream.Write(id.ToBytes());
                //StartCoroutine(InGameClientHandler(id, client));
            }
            else
            {
                client.Dispose();
            }
        }


        private void HostResolveComponentSync(int id, NetworkStream stream)
        {
        }

        private void HostResolveNetworkCall(int id, NetworkStream stream)
        {
            //Get next network call
            switch (stream.Read<PrimitiveCall>())
            {
                case PrimitiveCall.CreateObject:
                    int resourceID = stream.ReadI32();
                    int syncComponentCount = stream.ReadI32();

                    break;
                case PrimitiveCall.RemoveObject:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            //Do the call at host first

            //Broadcast to other clients
            foreach (KeyValuePair<int, TcpClient> kvp in ConnectedClients.Where(kvp => kvp.Key != id))
            {
            }
        }
    }
}