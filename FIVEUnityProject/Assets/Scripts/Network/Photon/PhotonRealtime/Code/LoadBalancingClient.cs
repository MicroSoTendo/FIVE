// -----------------------------------------------------------------------
// <copyright file="LoadBalancingClient.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Provides the operations and a state for games using the
//   Photon LoadBalancing server.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_3_OR_NEWER
#define SUPPORTED_UNITY
#endif


namespace Photon.Realtime
{
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

#if SUPPORTED_UNITY
    using Debug = UnityEngine.Debug;
#endif
#if SUPPORTED_UNITY || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
#endif


    #region Enums

    /// <summary>
    /// State values for a client, which handles switching Photon server types, some operations, etc.
    /// </summary>
    /// \ingroup publicApi
    public enum ClientState
    {
        /// <summary>Peer is created but not used yet.</summary>
        PeerCreated,

        /// <summary>Transition state while connecting to a server. On the Photon Cloud this sends the AppId and AuthenticationValues (UserID).</summary>
        Authenticating,

        /// <summary>Not Used.</summary>
        Authenticated,

        /// <summary>The client sent an OpJoinLobby and if this was done on the Master Server, it will result in. Depending on the lobby, it gets room listings.</summary>
        JoiningLobby,

        /// <summary>The client is in a lobby, connected to the MasterServer. Depending on the lobby, it gets room listings.</summary>
        JoinedLobby,

        /// <summary>Transition from MasterServer to GameServer.</summary>
        DisconnectingFromMasterServer,
        [Obsolete("Renamed to DisconnectingFromMasterServer")]
        DisconnectingFromMasterserver = DisconnectingFromMasterServer,

        /// <summary>Transition to GameServer (client authenticates and joins/creates a room).</summary>
        ConnectingToGameServer,
        [Obsolete("Renamed to ConnectingToGameServer")]
        ConnectingToGameserver = ConnectingToGameServer,

        /// <summary>Connected to GameServer (going to auth and join game).</summary>
        ConnectedToGameServer,
        [Obsolete("Renamed to ConnectedToGameServer")]
        ConnectedToGameserver = ConnectedToGameServer,

        /// <summary>Transition state while joining or creating a room on GameServer.</summary>
        Joining,

        /// <summary>The client entered a room. The CurrentRoom and Players are known and you can now raise events.</summary>
        Joined,

        /// <summary>Transition state when leaving a room.</summary>
        Leaving,

        /// <summary>Transition from GameServer to MasterServer (after leaving a room/game).</summary>
        DisconnectingFromGameServer,
        [Obsolete("Renamed to DisconnectingFromGameServer")]
        DisconnectingFromGameserver = DisconnectingFromGameServer,

        /// <summary>Connecting to MasterServer (includes sending authentication values).</summary>
        ConnectingToMasterServer,
        [Obsolete("Renamed to ConnectingToMasterServer.")]
        ConnectingToMasterserver = ConnectingToMasterServer,

        /// <summary>The client disconnects (from any server). This leads to state Disconnected.</summary>
        Disconnecting,

        /// <summary>The client is no longer connected (to any server). Connect to MasterServer to go on.</summary>
        Disconnected,

        /// <summary>Connected to MasterServer. You might use matchmaking or join a lobby now.</summary>
        ConnectedToMasterServer,
        [Obsolete("Renamed to ConnectedToMasterServer.")]
        ConnectedToMasterserver = ConnectedToMasterServer,
        [Obsolete("Renamed to ConnectedToMasterServer.")]
        ConnectedToMaster = ConnectedToMasterServer,

        /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
        ConnectingToNameServer,

        /// <summary>Client is connected to the NameServer and established encryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
        ConnectedToNameServer,

        /// <summary>Clients disconnects (specifically) from the NameServer (usually to connect to the MasterServer).</summary>
        DisconnectingFromNameServer
    }


    /// <summary>
    /// Internal state, how this peer gets into a particular room (joining it or creating it).
    /// </summary>
    internal enum JoinType
    {
        /// <summary>This client creates a room, gets into it (no need to join) and can set room properties.</summary>
        CreateRoom,
        /// <summary>The room existed already and we join into it (not setting room properties).</summary>
        JoinRoom,
        /// <summary>Done on Master Server and (if successful) followed by a Join on Game Server.</summary>
        JoinRandomRoom,
        /// <summary>Client is either joining or creating a room. On Master- and Game-Server.</summary>
        JoinOrCreateRoom
    }

    /// <summary>Enumeration of causes for Disconnects (used in LoadBalancingClient.DisconnectedCause).</summary>
    /// <remarks>Read the individual descriptions to find out what to do about this type of disconnect.</remarks>
    public enum DisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,
        /// <summary>OnStatusChanged: The server is not available or the address is wrong. Make sure the port is provided and the server is up.</summary>
        ExceptionOnConnect,
        /// <summary>OnStatusChanged: Some internal exception caused the socket code to fail. This may happen if you attempt to connect locally but the server is not available. In doubt: Contact Exit Games.</summary>
        Exception,

        /// <summary>OnStatusChanged: The server disconnected this client due to timing out (missing acknowledgement from the client).</summary>
        ServerTimeout,

        /// <summary>OnStatusChanged: This client detected that the server's responses are not received in due time.</summary>
        ClientTimeout,

        /// <summary>OnStatusChanged: The server disconnected this client from within the room's logic (the C# code).</summary>
        DisconnectByServerLogic,
        /// <summary>OnStatusChanged: The server disconnected this client for unknown reasons.</summary>
        DisconnectByServerReasonUnknown,

        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.</summary>
        InvalidAuthentication,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.</summary>
        CustomAuthenticationFailed,
        /// <summary>The authentication ticket should provide access to any Photon Cloud server without doing another authentication-service call. However, the ticket expired.</summary>
        AuthenticationTicketExpired,
        /// <summary>OnOperationResponse: Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.</summary>
        MaxCcuReached,

        /// <summary>OnOperationResponse: Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or master server address.</summary>
        InvalidRegion,

        /// <summary>OnOperationResponse: Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.</summary>
        OperationNotAllowedInCurrentState,
        /// <summary>OnStatusChanged: The client disconnected from within the logic (the C# code).</summary>
        DisconnectByClientLogic
    }

    /// <summary>Available server (types) for internally used field: server.</summary>
    /// <remarks>Photon uses 3 different roles of servers: Name Server, Master Server and Game Server.</remarks>
    public enum ServerConnection
    {
        /// <summary>This server is where matchmaking gets done and where clients can get lists of rooms in lobbies.</summary>
        MasterServer,
        /// <summary>This server handles a number of rooms to execute and relay the messages between players (in a room).</summary>
        GameServer,
        /// <summary>This server is used initially to get the address (IP) of a Master Server for a specific region. Not used for Photon OnPremise (self hosted).</summary>
        NameServer
    }

    /// <summary>
    /// Defines how the communication gets encrypted.
    /// </summary>
    public enum EncryptionMode
    {
        /// <summary>
        /// This is the default encryption mode: Messages get encrypted only on demand (when you send operations with the "encrypt" parameter set to true).
        /// </summary>
        PayloadEncryption,
        /// <summary>
        /// With this encryption mode for UDP, the connection gets setup and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is unavailable.
        /// </summary>
        DatagramEncryption = 10,
        /// <summary>
        /// With this encryption mode for UDP, the connection gets setup with random sequence numbers and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is unavailable.
        /// </summary>
        DatagramEncryptionRandomSequence = 11,
    }


    public static class EncryptionDataParameters
    {
        /// <summary>
        /// Key for encryption mode
        /// </summary>
        public const byte Mode = 0;
        /// <summary>
        /// Key for first secret
        /// </summary>
        public const byte Secret1 = 1;
        /// <summary>
        /// Key for second secret
        /// </summary>
        public const byte Secret2 = 2;
    }
    #endregion

    /// <summary>
    /// This class implements the Photon LoadBalancing workflow by using a LoadBalancingPeer.
    /// It keeps a state and will automatically execute transitions between the Master and Game Servers.
    /// </summary>
    /// <remarks>
    /// This class (and the Player class) should be extended to implement your own game logic.
    /// You can override CreatePlayer as "factory" method for Players and return your own Player instances.
    /// The State of this class is essential to know when a client is in a lobby (or just on the master)
    /// and when in a game where the actual gameplay should take place.
    /// Extension notes:
    /// An extension of this class should override the methods of the IPhotonPeerListener, as they
    /// are called when the state changes. Call base.method first, then pick the operation or state you
    /// want to react to and put it in a switch-case.
    /// We try to provide demo to each platform where this api can be used, so lookout for those.
    /// </remarks>
    public class LoadBalancingClient : IPhotonPeerListener
    {
        /// <summary>
        /// The client uses a LoadBalancingPeer as API to communicate with the server.
        /// This is public for ease-of-use: Some methods like OpRaiseEvent are not relevant for the connection state and don't need a override.
        /// </summary>
        public LoadBalancingPeer LoadBalancingPeer { get; private set; }

        /// <summary>The version of your client. A new version also creates a new "virtual app" to separate players from older client versions.</summary>
        public string AppVersion { get; set; }

        /// <summary>The AppID as assigned from the Photon Cloud. If you host yourself, this is the "regular" Photon Server Application Name (most likely: "LoadBalancing").</summary>
        public string AppId { get; set; }


        /// <summary>User authentication values to be sent to the Photon server right after connecting.</summary>
        /// <remarks>Set this property or pass AuthenticationValues by Connect(..., authValues).</remarks>
        public AuthenticationValues AuthValues { get; set; }

        /// <summary>Enables the new Authentication workflow.</summary>
        public AuthModeOption AuthMode = AuthModeOption.Auth;

        /// <summary>Defines how the communication gets encrypted.</summary>
        public EncryptionMode EncryptionMode = EncryptionMode.PayloadEncryption;

        /// <summary>The protocol which will be used on Master- and GameServer.</summary>
        /// <remarks>
        /// When using AuthMode = AuthModeOption.AuthOnceWss, the client uses a wss-connection on the NameServer but another protocol on the other servers.
        /// As the NameServer sends an address, which is different per protocol, it needs to know the expected protocol.
        /// </remarks>
        public ConnectionProtocol ExpectedProtocol = ConnectionProtocol.Udp;


        ///<summary>Simplifies getting the token for connect/init requests, if this feature is enabled.</summary>
        private string TokenForInit
        {
            get
            {
                if (AuthMode == AuthModeOption.Auth)
                {
                    return null;
                }
                return (AuthValues != null) ? AuthValues.Token : null;
            }
        }

        /// <summary>Internally used cache for the server's token. Identifies a user/session and can be used to rejoin.</summary>
        private string tokenCache;


        /// <summary>True if this client uses a NameServer to get the Master Server address.</summary>
        /// <remarks>This value is public, despite being an internal value, which should only be set by this client or PUN.</remarks>
        public bool IsUsingNameServer { get; set; }

        /// <summary>Name Server Host Name for Photon Cloud. Without port and without any prefix.</summary>
        public string NameServerHost = "ns.exitgames.com";

        /// <summary>Name Server for HTTP connections to the Photon Cloud. Includes prefix and port.</summary>
        public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

        /// <summary>Name Server Address for Photon Cloud (based on current protocol). You can use the default values and usually won't have to set this value.</summary>
        public string NameServerAddress => GetNameServerAddress();

        /// <summary>Name Server port per protocol (the UDP port is different than TCP, etc).</summary>
        private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { { ConnectionProtocol.Udp, 5058 }, { ConnectionProtocol.Tcp, 4533 }, { ConnectionProtocol.WebSocket, 9093 }, { ConnectionProtocol.WebSocketSecure, 19093 } }; //, { ConnectionProtocol.RHttp, 6063 } };

        /// <summary>Use the alternative ports for UDP connections in the Public Cloud (27000 to 27003).</summary>
        /// <remarks>
        /// This should be used when players have issues with connection stability.
        /// Some players reported better connectivity for Steam games.
        /// The effect might vary, which is why the alternative ports are not the new default.
        ///
        /// The alternative (server) ports are 27000 up to 27003.
        ///
        /// The values are appplied by replacing any incoming server-address string accordingly.
        /// You only need to set this to true though.
        ///
        /// This value does not affect TCP or WebSocket connections.
        /// </remarks>
        public bool UseAlternativeUdpPorts { get; set; }


        /// <summary>The currently used server address (if any). The type of server is define by Server property.</summary>
        public string CurrentServerAddress => LoadBalancingPeer.ServerAddress;

        /// <summary>Your Master Server address. In PhotonCloud, call ConnectToRegionMaster() to find your Master Server.</summary>
        /// <remarks>
        /// In the Photon Cloud, explicit definition of a Master Server Address is not best practice.
        /// The Photon Cloud has a "Name Server" which redirects clients to a specific Master Server (per Region and AppId).
        /// </remarks>
        public string MasterServerAddress { get; set; }

        /// <summary>The game server's address for a particular room. In use temporarily, as assigned by master.</summary>
        public string GameServerAddress { get; protected internal set; }

        /// <summary>The server this client is currently connected or connecting to.</summary>
        /// <remarks>
        /// Each server (NameServer, MasterServer, GameServer) allow some operations and reject others.
        /// </remarks>
        public ServerConnection Server { get; private set; }

        /// <summary>Backing field for property.</summary>
        private ClientState state = ClientState.PeerCreated;

        /// <summary>Current state this client is in. Careful: several states are "transitions" that lead to other states.</summary>
        public ClientState State
        {
            get => state;

            set
            {
                if (state == value)
                {
                    return;
                }
                ClientState previousState = state;
                state = value;
                if (StateChanged != null)
                {
                    StateChanged(previousState, state);
                }
            }
        }

        /// <summary>Returns if this client is currently connected or connecting to some type of server.</summary>
        /// <remarks>This is even true while switching servers. Use IsConnectedAndReady to check only for those states that enable you to send Operations.</remarks>
        public bool IsConnected => LoadBalancingPeer != null && State != ClientState.PeerCreated && State != ClientState.Disconnected;


        /// <summary>
        /// A refined version of IsConnected which is true only if your connection is ready to send operations.
        /// </summary>
        /// <remarks>
        /// Not all operations can be called on all types of servers. If an operation is unavailable on the currently connected server,
        /// this will result in a OperationResponse with ErrorCode != 0.
        ///
        /// Examples: The NameServer allows OpGetRegions which is not available anywhere else.
        /// The MasterServer does not allow you to send events (OpRaiseEvent) and on the GameServer you are unable to join a lobby (OpJoinLobby).
        ///
        /// To check which server you are on, use: PhotonNetwork.Server.
        /// </remarks>
        public bool IsConnectedAndReady
        {
            get
            {
                if (LoadBalancingPeer == null)
                {
                    return false;
                }

                switch (State)
                {
                    case ClientState.PeerCreated:
                    case ClientState.Disconnected:
                    case ClientState.Disconnecting:
                    case ClientState.DisconnectingFromGameServer:
                    case ClientState.DisconnectingFromMasterServer:
                    case ClientState.DisconnectingFromNameServer:
                    case ClientState.Authenticating:
                    case ClientState.ConnectingToGameServer:
                    case ClientState.ConnectingToMasterServer:
                    case ClientState.ConnectingToNameServer:
                    case ClientState.Joining:
                    case ClientState.Leaving:
                        return false;   // we are not ready to execute any operations
                }

                return true;
            }
        }


        /// <summary>Register a method to be called when this client's ClientState gets set.</summary>
        /// <remarks>This can be useful to react to being connected, joined into a room, etc.</remarks>
        public event Action<ClientState, ClientState> StateChanged;

        /// <summary>Register a method to be called when an event got dispatched. Gets called after the LoadBalancingClient handled the internal events first.</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnEvent().
        ///
        /// Note that OnEvent is calling EventReceived after it handled internal events first.
        /// That means for example: Joining players will already be in the player list but leaving
        /// players will already be removed from the room.
        /// </remarks>
        public event Action<EventData> EventReceived;

        /// <summary>Register a method to be called when an operation response is received.</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnOperationResponse().
        ///
        /// Note that OnOperationResponse gets executed before your Action is called.
        /// That means for example: The OpJoinLobby response already set the state to "JoinedLobby"
        /// and the response to OpLeave already triggered the Disconnect before this is called.
        /// </remarks>
        public event Action<OperationResponse> OpResponseReceived;


        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        public ConnectionCallbacksContainer ConnectionCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        public MatchMakingCallbacksContainer MatchMakingCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal InRoomCallbacksContainer InRoomCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal LobbyCallbacksContainer LobbyCallbackTargets;

        /// <summary>Wraps up the target objects for a group of callbacks, so they can be called conveniently.</summary>
        /// <remarks>By using Add or Remove, objects can "subscribe" or "unsubscribe" for this group  of callbacks.</remarks>
        internal WebRpcCallbacksContainer WebRpcCallbackTargets;


        /// <summary>Summarizes (aggregates) the different causes for disconnects of a client.</summary>
        /// <remarks>
        /// A disconnect can be caused by: errors in the network connection or some vital operation failing
        /// (which is considered "high level"). While operations always trigger a call to OnOperationResponse,
        /// connection related changes are treated in OnStatusChanged.
        /// The DisconnectCause is set in either case and summarizes the causes for any disconnect in a single
        /// state value which can be used to display (or debug) the cause for disconnection.
        /// </remarks>
        public DisconnectCause DisconnectedCause { get; protected set; }


        /// <summary>Internal value if the client is in a lobby.</summary>
        /// <remarks>This is used to re-set this.State, when joining/creating a room fails.</remarks>
        public bool InLobby => State == ClientState.JoinedLobby;

        /// <summary>The lobby this client currently uses. Defined when joining a lobby or creating rooms</summary>
        public TypedLobby CurrentLobby { get; internal set; }

        /// <summary>
        /// If enabled, the client will get a list of available lobbies from the Master Server.
        /// </summary>
        /// <remarks>
        /// Set this value before the client connects to the Master Server. While connected to the Master
        /// Server, a change has no effect.
        ///
        /// Implement OptionalInfoCallbacks.OnLobbyStatisticsUpdate, to get the list of used lobbies.
        ///
        /// The lobby statistics can be useful if your title dynamically uses lobbies, depending (e.g.)
        /// on current player activity or such.
        /// In this case, getting a list of available lobbies, their room-count and player-count can
        /// be useful info.
        ///
        /// ConnectUsingSettings sets this to the PhotonServerSettings value.
        /// </remarks>
        public bool EnableLobbyStatistics;

        /// <summary>Internal lobby stats cache, used by LobbyStatistics.</summary>
        private readonly List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();


        /// <summary>The local player is never null but not valid unless the client is in a room, too. The ID will be -1 outside of rooms.</summary>
        public Player LocalPlayer { get; internal set; }

        /// <summary>
        /// The nickname of the player (synced with others). Same as client.LocalPlayer.NickName.
        /// </summary>
        public string NickName
        {
            get => LocalPlayer.NickName;

            set
            {
                if (LocalPlayer == null)
                {
                    return;
                }

                LocalPlayer.NickName = value;
            }
        }


        /// <summary>An ID for this user. Sent in OpAuthenticate when you connect. If not set, the PlayerName is applied during connect.</summary>
        /// <remarks>
        /// On connect, if the UserId is null or empty, the client will copy the PlayName to UserId. If PlayerName is not set either
        /// (before connect), the server applies a temporary ID which stays unknown to this client and other clients.
        ///
        /// The UserId is what's used in FindFriends and for fetching data for your account (with WebHooks e.g.).
        ///
        /// By convention, set this ID before you connect, not while being connected.
        /// There is no error but the ID won't change while being connected.
        /// </remarks>
        public string UserId
        {
            get
            {
                if (AuthValues != null)
                {
                    return AuthValues.UserId;
                }
                return null;
            }
            set
            {
                if (AuthValues == null)
                {
                    AuthValues = new AuthenticationValues();
                }
                AuthValues.UserId = value;
            }
        }

        /// <summary>The current room this client is connected to (null if none available).</summary>
        public Room CurrentRoom { get; private set; }


        /// <summary>Is true while being in a room (this.state == ClientState.Joined).</summary>
        /// <remarks>
        /// Aside from polling this value, game logic should implement IMatchmakingCallbacks in some class
        /// and react when that gets called.<br/>
        /// OpRaiseEvent, OpLeave and some other operations can only be used (successfully) when the client is in a room.
        /// PUN's PhotonNetwork.InRoom will support being InRoom in it's offline mode (by providing it's own property).
        /// </remarks>
        public bool InRoom => state == ClientState.Joined && CurrentRoom != null;

        /// <summary>Statistic value available on master server: Players on master (looking for games).</summary>
        public int PlayersOnMasterCount { get; internal set; }

        /// <summary>Statistic value available on master server: Players in rooms (playing).</summary>
        public int PlayersInRoomsCount { get; internal set; }

        /// <summary>Statistic value available on master server: Rooms currently created.</summary>
        public int RoomsCount { get; internal set; }


        /// <summary>Internally used to decide if a room must be created or joined on game server.</summary>
        private JoinType lastJoinType;

        /// <summary>Used when the client arrives on the GS, to join the room with the correct values.</summary>
        private EnterRoomParams enterRoomParamsCache;

        /// <summary>Used to cache a failed "enter room" operation on the Game Server, to return to the Master Server before calling a fail-callback.</summary>
        private OperationResponse failedRoomEntryOperation;


        /// <summary>Maximum of userIDs that can be sent in one friend list request.</summary>
        private const int FriendRequestListMax = 512;

        /// <summary>Contains the list of names of friends to look up their state on the server.</summary>
        private string[] friendListRequested;

        /// <summary>Internal flag to know if the client currently fetches a friend list.</summary>
        public bool IsFetchingFriendList => friendListRequested != null;


        /// <summary>The cloud region this client connects to. Set by ConnectToRegionMaster(). Not set if you don't use a NameServer!</summary>
        public string CloudRegion { get; private set; }

        /// <summary>Contains the list if enabled regions this client may use. Null, unless the client got a response to OpGetRegions.</summary>
        public RegionHandler RegionHandler;


        private class CallbackTargetChange
        {
            public readonly object Target;
            /// <summary>Add if true, remove if false.</summary>
            public readonly bool AddTarget;

            public CallbackTargetChange(object target, bool addTarget)
            {
                Target = target;
                AddTarget = addTarget;
            }
        }

        private readonly Queue<CallbackTargetChange> callbackTargetChanges = new Queue<CallbackTargetChange>();
        private readonly HashSet<object> callbackTargets = new HashSet<object>();


        /// <summary>Creates a LoadBalancingClient with UDP protocol or the one specified.</summary>
        /// <param name="protocol">Specifies the network protocol to use for connections.</param>
        public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
        {
            ConnectionCallbackTargets = new ConnectionCallbacksContainer(this);
            MatchMakingCallbackTargets = new MatchMakingCallbacksContainer(this);
            InRoomCallbackTargets = new InRoomCallbacksContainer(this);
            LobbyCallbackTargets = new LobbyCallbacksContainer(this);
            WebRpcCallbackTargets = new WebRpcCallbacksContainer(this);

            LoadBalancingPeer = new LoadBalancingPeer(this, protocol)
            {
                SerializationProtocolType = SerializationProtocol.GpBinaryV18
            };
            LocalPlayer = CreatePlayer(string.Empty, -1, true, null); //TODO: Check if we can do this later

#if UNITY_WEBGL
            if (this.LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Tcp || this.LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Udp)
            {
                this.LoadBalancingPeer.Listener.DebugReturn(DebugLevel.WARNING, "WebGL requires WebSockets. Switching TransportProtocol to WebSocketSecure.");
                this.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
                //SocketWebTcp.SerializationProtocol = Enum.GetName(typeof(SerializationProtocol), this.LoadBalancingPeer.SerializationProtocolType);
            }
#endif

            State = ClientState.PeerCreated;
        }


        /// <summary>Creates a LoadBalancingClient, setting various values needed before connecting.</summary>
        /// <param name="masterAddress">The Master Server's address to connect to. Used in Connect.</param>
        /// <param name="appId">The AppId of this title. Needed for the Photon Cloud. Find it in the Dashboard.</param>
        /// <param name="gameVersion">A version for this client/build. In the Photon Cloud, players are separated by AppId, GameVersion and Region.</param>
        /// <param name="protocol">Specifies the network protocol to use for connections.</param>
        public LoadBalancingClient(string masterAddress, string appId, string gameVersion, ConnectionProtocol protocol = ConnectionProtocol.Udp) : this(protocol)
        {
            MasterServerAddress = masterAddress;
            AppId = appId;
            AppVersion = gameVersion;
        }

        /// <summary>
        /// Gets the NameServer Address (with prefix and port), based on the set protocol (this.LoadBalancingPeer.UsedProtocol).
        /// </summary>
        /// <returns>NameServer Address (with prefix and port).</returns>
        private string GetNameServerAddress()
        {
            ProtocolToNameServerPort.TryGetValue(LoadBalancingPeer.TransportProtocol, out int protocolPort);
            if (LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Udp && UseAlternativeUdpPorts)
            {
                protocolPort = 27000;
            }

            switch (LoadBalancingPeer.TransportProtocol)
            {
                case ConnectionProtocol.Udp:
                case ConnectionProtocol.Tcp:
                    return string.Format("{0}:{1}", NameServerHost, protocolPort);
#if RHTTP
                case ConnectionProtocol.RHttp:
                    return NameServerHttp;
#endif
                case ConnectionProtocol.WebSocket:
                    return string.Format("ws://{0}:{1}", NameServerHost, protocolPort);
                case ConnectionProtocol.WebSocketSecure:
                    return string.Format("wss://{0}:{1}", NameServerHost, protocolPort);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Operations and Commands



        /// <summary>
        /// Starts the "process" to connect to a Master Server, using MasterServerAddress and AppId properties.
        /// </summary>
        /// <remarks>
        /// To connect to the Photon Cloud, use ConnectToRegionMaster().
        ///
        /// The process to connect includes several steps: the actual connecting, establishing encryption, authentification
        /// (of app and optionally the user) and connecting to the MasterServer
        ///
        /// Users can connect either anonymously or use "Custom Authentication" to verify each individual player's login.
        /// Custom Authentication in Photon uses external services and communities to verify users. While the client provides a user's info,
        /// the service setup is done in the Photon Cloud Dashboard.
        /// The parameter authValues will set this.AuthValues and use them in the connect process.
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        /// - Region not available (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.InvalidRegion)
        /// - Subscription CCU limit reached (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached)
        /// </remarks>
        public virtual bool Connect()
        {
#if UNITY_WEBGL
            SocketWebTcp.SerializationProtocol = Enum.GetName(typeof(SerializationProtocol), this.LoadBalancingPeer.SerializationProtocolType);
#endif

            DisconnectedCause = DisconnectCause.None;
            if (LoadBalancingPeer.Connect(MasterServerAddress, AppId, TokenForInit))
            {
                State = ClientState.ConnectingToMasterServer;
                return true;
            }

#if UNITY_XBOXONE
            if (this.AuthValues == null || this.AuthValues.AuthType != CustomAuthenticationType.Xbox)
            {
                UnityEngine.Debug.LogError("UNITY_XBOXONE builds must set AuthValues.AuthType to \"CustomAuthenticationType.Xbox\". Set this before calling any Connect method. Connect failed!");
                return false;
            }
#endif

            return false;
        }


        /// <summary>
        /// Connects to the NameServer for Photon Cloud, where a region and server list can be obtained.
        /// </summary>
        /// <see cref="OpGetRegions"/>
        /// <returns>If the workflow was started or failed right away.</returns>
        public bool ConnectToNameServer()
        {
            IsUsingNameServer = true;
            CloudRegion = null;

#if UNITY_WEBGL
            SocketWebTcp.SerializationProtocol = Enum.GetName(typeof(SerializationProtocol), this.LoadBalancingPeer.SerializationProtocolType);
#endif

            if (AuthMode == AuthModeOption.AuthOnceWss)
            {
                ExpectedProtocol = LoadBalancingPeer.TransportProtocol;
                LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }

            DisconnectedCause = DisconnectCause.None;
            if (!LoadBalancingPeer.Connect(NameServerAddress, "NameServer", TokenForInit))
            {
                return false;
            }

            State = ClientState.ConnectingToNameServer;
            return true;
        }

        /// <summary>
        /// Connects you to a specific region's Master Server, using the Name Server to find the IP.
        /// </summary>
        /// <returns>If the operation could be sent. If false, no operation was sent.</returns>
        public bool ConnectToRegionMaster(string region)
        {
            IsUsingNameServer = true;

            if (State == ClientState.ConnectedToNameServer)
            {
                CloudRegion = region;
                return CallAuthenticate();
            }

            LoadBalancingPeer.Disconnect();
            CloudRegion = region;

#if UNITY_WEBGL
            SocketWebTcp.SerializationProtocol = Enum.GetName(typeof(SerializationProtocol), this.LoadBalancingPeer.SerializationProtocolType);
#endif

            if (AuthMode == AuthModeOption.AuthOnceWss)
            {
                ExpectedProtocol = LoadBalancingPeer.TransportProtocol;
                LoadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }

            DisconnectedCause = DisconnectCause.None;
            if (!LoadBalancingPeer.Connect(NameServerAddress, "NameServer", null))
            {
                return false;
            }

            State = ClientState.ConnectingToNameServer;
            return true;
        }


        /// <summary>
        /// Privately used only for reconnecting.
        /// </summary>
        private bool Connect(string serverAddress, ServerConnection type)
        {
            // TODO: Make sure app doesn't quit right now

            if (State == ClientState.Disconnecting)
            {
                DebugReturn(DebugLevel.ERROR, "Connect() failed. Can't connect while disconnecting (still). Current state: " + State);
                return false;
            }


            // connect might fail, if the DNS name can't be resolved or if no network connection is available
            DisconnectedCause = DisconnectCause.None;
            bool connecting = LoadBalancingPeer.Connect(serverAddress, "", TokenForInit);

            if (connecting)
            {
                switch (type)
                {
                    case ServerConnection.NameServer:
                        State = ClientState.ConnectingToNameServer;
                        break;
                    case ServerConnection.MasterServer:
                        State = ClientState.ConnectingToMasterServer;
                        break;
                    case ServerConnection.GameServer:
                        State = ClientState.ConnectingToGameServer;
                        break;
                }
            }

            return connecting;
        }


        /// <summary>
        /// Privately used only.
        /// Starts the "process" to connect to the game server (connect before a game is joined).
        /// </summary>
        private bool ConnectToGameServer()
        {
            if (LoadBalancingPeer.Connect(GameServerAddress, AppId, TokenForInit))
            {
                State = ClientState.ConnectingToGameServer;
                return true;
            }

            // TODO: handle error "cant connect to GS"
            return false;
        }


        /// <summary>Can be used to reconnect to the master server after a disconnect.</summary>
        /// <remarks>Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.</remarks>
        public bool ReconnectToMaster()
        {
            if (AuthValues == null)
            {
                DebugReturn(DebugLevel.WARNING, "ReconnectToMaster() with AuthValues == null is not correct!");
                AuthValues = new AuthenticationValues();
            }
            AuthValues.Token = tokenCache;

            return Connect(MasterServerAddress, ServerConnection.MasterServer);
        }

        /// <summary>
        /// Can be used to return to a room quickly, by directly reconnecting to a game server to rejoin a room.
        /// </summary>
        /// <remarks>
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        /// </remarks>
        /// <returns>False, if the conditions are not met. Then, this client does not attempt the ReconnectAndRejoin.</returns>
        public bool ReconnectAndRejoin()
        {
            if (string.IsNullOrEmpty(GameServerAddress))
            {
                DebugReturn(DebugLevel.ERROR, "ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
                return false;
            }
            if (enterRoomParamsCache == null)
            {
                DebugReturn(DebugLevel.ERROR, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
                return false;
            }
            if (tokenCache == null)
            {
                DebugReturn(DebugLevel.ERROR, "ReconnectAndRejoin() failed. It seems the client doesn't have any previous authentication token to re-connect.");
                return false;
            }

            if (AuthValues == null)
            {
                AuthValues = new AuthenticationValues();
            }
            AuthValues.Token = tokenCache;


            if (!string.IsNullOrEmpty(GameServerAddress) && enterRoomParamsCache != null)
            {
                lastJoinType = JoinType.JoinRoom;
                enterRoomParamsCache.RejoinOnly = true;
                return Connect(GameServerAddress, ServerConnection.GameServer);
            }

            return false;
        }


        /// <summary>Disconnects this client from any server and sets this.State if the connection is successfuly closed.</summary>
        public void Disconnect()
        {
            if (State != ClientState.Disconnected)
            {
                State = ClientState.Disconnecting;
                DisconnectedCause = DisconnectCause.DisconnectByClientLogic;
                LoadBalancingPeer.Disconnect();

                //// we can set this high-level state if the low-level (connection)state is "disconnected"
                //if (this.LoadBalancingPeer.PeerState == PeerStateValue.Disconnected || this.LoadBalancingPeer.PeerState == PeerStateValue.InitializingApplication)
                //{
                //    this.State = ClientState.Disconnected;
                //}
            }
        }


        /// <summary>
        /// Private Disconnect variant that sets the state, too.
        /// </summary>
        private void DisconnectToReconnect()
        {
            switch (Server)
            {
                case ServerConnection.NameServer:
                    State = ClientState.DisconnectingFromNameServer;
                    break;
                case ServerConnection.MasterServer:
                    State = ClientState.DisconnectingFromMasterServer;
                    break;
                case ServerConnection.GameServer:
                    State = ClientState.DisconnectingFromGameServer;
                    break;
            }

            LoadBalancingPeer.Disconnect();
        }

        /// <summary>
        /// Useful to test loss of connection which will end in a client timeout. This modifies LoadBalancingPeer.NetworkSimulationSettings. Read remarks.
        /// </summary>
        /// <remarks>
        /// Use with care as this sets LoadBalancingPeer.IsSimulationEnabled.<br/>
        /// Read LoadBalancingPeer.IsSimulationEnabled to check if this is on or off, if needed.<br/>
        ///
        /// If simulateTimeout is true, LoadBalancingPeer.NetworkSimulationSettings.IncomingLossPercentage and
        /// LoadBalancingPeer.NetworkSimulationSettings.OutgoingLossPercentage will be set to 100.<br/>
        /// Obviously, this overrides any network simulation settings done before.<br/>
        ///
        /// If you want fine-grained network simulation control, use the NetworkSimulationSettings.<br/>
        ///
        /// The timeout will lead to a call to <see cref="IConnectionCallbacks.OnDisconnected"/>, as usual in a client timeout.
        ///
        /// You could modify this method (or use NetworkSimulationSettings) to deliberately run into a server timeout by
        /// just setting the OutgoingLossPercentage = 100 and the IncomingLossPercentage = 0.
        /// </remarks>
        /// <param name="simulateTimeout">If true, a connection loss is simulated. If false, the simulation ends.</param>
        public void SimulateConnectionLoss(bool simulateTimeout)
        {
            DebugReturn(DebugLevel.WARNING, "SimulateConnectionLoss() set to: " + simulateTimeout);

            if (simulateTimeout)
            {
                LoadBalancingPeer.NetworkSimulationSettings.IncomingLossPercentage = 100;
                LoadBalancingPeer.NetworkSimulationSettings.OutgoingLossPercentage = 100;
            }

            LoadBalancingPeer.IsSimulationEnabled = simulateTimeout;
        }

        private bool CallAuthenticate()
        {
            if (AuthMode == AuthModeOption.Auth)
            {
                if (!CheckIfOpCanBeSent(OperationCode.Authenticate, Server, "Authenticate"))
                {
                    return false;
                }
                return LoadBalancingPeer.OpAuthenticate(AppId, AppVersion, AuthValues, CloudRegion, (EnableLobbyStatistics && Server == ServerConnection.MasterServer));
            }
            else
            {
                if (!CheckIfOpCanBeSent(OperationCode.AuthenticateOnce, Server, "AuthenticateOnce"))
                {
                    return false;
                }
                return LoadBalancingPeer.OpAuthenticateOnce(AppId, AppVersion, AuthValues, CloudRegion, EncryptionMode, ExpectedProtocol);
            }
        }


        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// It uses DispatchIncomingCommands and SendOutgoingCommands to do that.
        /// </summary>
        /// <remarks>
        /// The Photon client libraries are designed to fit easily into a game or application. The application
        /// is in control of the context (thread) in which incoming events and responses are executed and has
        /// full control of the creation of UDP/TCP packages.
        ///
        /// Sending packages and dispatching received messages are two separate tasks. Service combines them
        /// into one method at the cost of control. It calls DispatchIncomingCommands and SendOutgoingCommands.
        ///
        /// Call this method regularly (10..50 times a second).
        ///
        /// This will Dispatch ANY received commands (unless a reliable command in-order is still missing) and
        /// events AND will send queued outgoing commands. Fewer calls might be more effective if a device
        /// cannot send many packets per second, as multiple operations might be combined into one package.
        /// </remarks>
        /// <example>
        /// You could replace Service by:
        ///
        ///     while (DispatchIncomingCommands()); //Dispatch until everything is Dispatched...
        ///     SendOutgoingCommands(); //Send a UDP/TCP package with outgoing messages
        /// </example>
        /// <seealso cref="PhotonPeer.DispatchIncomingCommands"/>
        /// <seealso cref="PhotonPeer.SendOutgoingCommands"/>
        public void Service()
        {
            if (LoadBalancingPeer != null)
            {
                LoadBalancingPeer.Service();
            }
        }


        /// <summary>
        /// While on the NameServer, this gets you the list of regional servers (short names and their IPs to ping them).
        /// </summary>
        /// <returns>If the operation could be sent. If false, no operation was sent (e.g. while not connected to the NameServer).</returns>
        private bool OpGetRegions()
        {
            if (!CheckIfOpCanBeSent(OperationCode.GetRegions, Server, "GetRegions"))
            {
                return false;
            }

            bool sent = LoadBalancingPeer.OpGetRegions(AppId);
            return sent;
        }


        /// <summary>
        /// Request the rooms and online status for a list of friends. All clients should set a unique UserId before connecting. The result is available in this.FriendList.
        /// </summary>
        /// <remarks>
        /// Used on Master Server to find the rooms played by a selected list of users.
        /// The result will be stored in LoadBalancingClient.FriendList, which is null before the first server response.
        ///
        /// Users identify themselves by setting a UserId in the LoadBalancingClient instance.
        /// This will send the ID in OpAuthenticate during connect (to master and game servers).
        /// Note: Changing a player's name doesn't make sense when using a friend list.
        ///
        /// The list of usernames must be fetched from some other source (not provided by Photon).
        ///
        ///
        /// Internal:<br/>
        /// The server response includes 2 arrays of info (each index matching a friend from the request):<br/>
        /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states<br/>
        /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)<br/>
        /// <br/>
        /// The options may be used to define which state a room must match to be returned.
        /// </remarks>
        /// <param name="friendsToFind">Array of friend's names (make sure they are unique).</param>
        /// <param name="options">Options that affect the result of the FindFriends operation.</param>
        /// <returns>If the operation could be sent (requires connection).</returns>
        public bool OpFindFriends(string[] friendsToFind, FindFriendsOptions options = null)
        {
            if (!CheckIfOpCanBeSent(OperationCode.FindFriends, Server, "FindFriends"))
            {
                return false;
            }

            if (IsFetchingFriendList)
            {
                DebugReturn(DebugLevel.WARNING, "OpFindFriends skipped: already fetching friends list.");
                return false;   // fetching friends currently, so don't do it again (avoid changing the list while fetching friends)
            }

            if (friendsToFind == null || friendsToFind.Length == 0)
            {
                DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friendsToFind array is null or empty.");
                return false;
            }

            if (friendsToFind.Length > FriendRequestListMax)
            {
                DebugReturn(DebugLevel.ERROR, string.Format("OpFindFriends skipped: friendsToFind array exceeds allowed length of {0}.", FriendRequestListMax));
                return false;
            }

            List<string> friendsList = new List<string>(friendsToFind.Length);
            for (int i = 0; i < friendsToFind.Length; i++)
            {
                string friendUserId = friendsToFind[i];
                if (string.IsNullOrEmpty(friendUserId))
                {
                    DebugReturn(DebugLevel.WARNING,
                        string.Format(
                            "friendsToFind array contains a null or empty UserId, element at position {0} skipped.",
                            i));
                }
                else if (friendUserId.Equals(UserId))
                {
                    DebugReturn(DebugLevel.WARNING,
                        string.Format(
                            "friendsToFind array contains local player's UserId \"{0}\", element at position {1} skipped.",
                            friendUserId,
                            i));
                }
                else if (friendsList.Contains(friendUserId))
                {
                    DebugReturn(DebugLevel.WARNING,
                        string.Format(
                            "friendsToFind array contains duplicate UserId \"{0}\", element at position {1} skipped.",
                            friendUserId,
                            i));
                }
                else
                {
                    friendsList.Add(friendUserId);
                }
            }

            if (friendsList.Count == 0)
            {
                DebugReturn(DebugLevel.ERROR, "OpFindFriends skipped: friends list to find is empty.");
                return false;
            }

            string[] filteredArray = friendsList.ToArray();
            bool sent = LoadBalancingPeer.OpFindFriends(filteredArray, options);
            friendListRequested = sent ? filteredArray : null;

            return sent;
        }

        /// <summary>If already connected to a Master Server, this joins the specified lobby. This request triggers an OnOperationResponse() call and the callback OnJoinedLobby().</summary>
        /// <param name="lobby">The lobby to join. Use null for default lobby.</param>
        /// <returns>If the operation could be sent. False, if the client is not IsConnectedAndReady or when it's not connected to a Master Server.</returns>
        public bool OpJoinLobby(TypedLobby lobby)
        {
            if (!CheckIfOpCanBeSent(OperationCode.JoinLobby, Server, "JoinLobby"))
            {
                return false;
            }

            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            bool sent = LoadBalancingPeer.OpJoinLobby(lobby);
            if (sent)
            {
                CurrentLobby = lobby;
                State = ClientState.JoiningLobby;
            }

            return sent;
        }


        /// <summary>Opposite of joining a lobby. You don't have to explicitly leave a lobby to join another (client can be in one max, at any time).</summary>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpLeaveLobby()
        {
            if (!CheckIfOpCanBeSent(OperationCode.LeaveLobby, Server, "LeaveLobby"))
            {
                return false;
            }
            return LoadBalancingPeer.OpLeaveLobby();
        }


        /// <summary>
        /// Joins a random room that matches the filter. Will callback: OnJoinedRoom or OnJoinRandomFailed.
        /// </summary>
        /// <remarks>
        /// Used for random matchmaking. You can join any room or one with specific properties defined in opJoinRandomRoomParams.
        ///
        /// You can use expectedCustomRoomProperties and expectedMaxPlayers as filters for accepting rooms.
        /// If you set expectedCustomRoomProperties, a room must have the exact same key values set at Custom Properties.
        /// You need to define which Custom Room Properties will be available for matchmaking when you create a room.
        /// See: OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby)
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed or not visible).
        /// It may also fail when actually joining the room which was found. Rooms may close, become full or empty anytime.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        ///
        /// This client's State is set to ClientState.Joining immediately, when the operation could
        /// be called. In the background, the client will switch servers and call various related operations.
        ///
        /// When you're in the room, this client's State will become ClientState.Joined.
        ///
        ///
        /// When entering a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and are not wiped when leaving a room.
        ///
        /// More about matchmaking:
        /// https://doc.photonengine.com/en-us/realtime/current/reference/matchmaking-and-lobby
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="opJoinRandomRoomParams">Optional definition of properties to filter rooms in random matchmaking.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams = null)
        {
            if (!CheckIfOpCanBeSent(OperationCode.JoinRandomGame, Server, "JoinRandomGame"))
            {
                return false;
            }

            if (opJoinRandomRoomParams == null)
            {
                opJoinRandomRoomParams = new OpJoinRandomRoomParams();
            }

            enterRoomParamsCache = new EnterRoomParams
            {
                Lobby = opJoinRandomRoomParams.TypedLobby,
                ExpectedUsers = opJoinRandomRoomParams.ExpectedUsers
            };


            bool sending = LoadBalancingPeer.OpJoinRandomRoom(opJoinRandomRoomParams);
            if (sending)
            {
                lastJoinType = JoinType.JoinRandomRoom;
                State = ClientState.Joining;
            }
            return sending;
        }


        /// <summary>
        /// Creates a new room. Will callback: OnCreatedRoom and OnJoinedRoom or OnCreateRoomFailed.
        /// </summary>
        /// <remarks>
        /// When successful, the client will enter the specified room and callback both OnCreatedRoom and OnJoinedRoom.
        /// In all error cases, OnCreateRoomFailed gets called.
        ///
        /// Creating a room will fail if the room name is already in use or when the RoomOptions clashing
        /// with one another. Check the EnterRoomParams reference for the various room creation options.
        ///
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        ///
        /// When you're in the room, this client's State will become ClientState.Joined.
        ///
        ///
        /// When entering a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and are not wiped when leaving a room.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="enterRoomParams">Definition of properties for the room to create.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpCreateRoom(EnterRoomParams enterRoomParams)
        {
            if (!CheckIfOpCanBeSent(OperationCode.CreateGame, Server, "CreateGame"))
            {
                return false;
            }
            bool onGameServer = Server == ServerConnection.GameServer;
            enterRoomParams.OnGameServer = onGameServer;
            if (!onGameServer)
            {
                enterRoomParamsCache = enterRoomParams;
            }

            bool sending = LoadBalancingPeer.OpCreateRoom(enterRoomParams);
            if (sending)
            {
                lastJoinType = JoinType.CreateRoom;
                State = ClientState.Joining;
            }
            return sending;
        }


        /// <summary>
        /// Joins a specific room by name and creates it on demand. Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// Useful when players make up a room name to meet in:
        /// All involved clients call the same method and whoever is first, also creates the room.
        ///
        /// When successful, the client will enter the specified room.
        /// The client which creates the room, will callback both OnCreatedRoom and OnJoinedRoom.
        /// Clients that join an existing room will only callback OnJoinedRoom.
        /// In all error cases, OnJoinRoomFailed gets called.
        ///
        /// Joining a room will fail, if the room is full, closed or when the user
        /// already is present in the room (checked by userId).
        ///
        /// To return to a room, use OpRejoinRoom.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// This client's State is set to ClientState.Joining immediately, when the operation could
        /// be called. In the background, the client will switch servers and call various related operations.
        ///
        /// When you're in the room, this client's State will become ClientState.Joined.
        ///
        ///
        /// If you set room properties in roomOptions, they get ignored when the room is existing already.
        /// This avoids changing the room properties by late joining players.
        ///
        /// When entering a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and are not wiped when leaving a room.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="enterRoomParams">Definition of properties for the room to create or join.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinOrCreateRoom(EnterRoomParams enterRoomParams)
        {
            bool onGameServer = Server == ServerConnection.GameServer;
            enterRoomParams.CreateIfNotExists = true;
            enterRoomParams.OnGameServer = onGameServer;
            if (!onGameServer)
            {
                enterRoomParamsCache = enterRoomParams;
            }

            bool sending = LoadBalancingPeer.OpJoinRoom(enterRoomParams);
            if (sending)
            {
                lastJoinType = JoinType.JoinOrCreateRoom;
                State = ClientState.Joining;
            }
            return sending;
        }


        /// <summary>
        /// Joins a room by name. Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// Useful when using lobbies or when players follow friends or invite each other.
        ///
        /// When successful, the client will enter the specified room and callback via OnJoinedRoom.
        /// In all error cases, OnJoinRoomFailed gets called.
        ///
        /// Joining a room will fail if the room is full, closed, not existing or when the user
        /// already is present in the room (checked by userId).
        ///
        /// To return to a room, use OpRejoinRoom.
        /// When players invite each other and it's unclear who's first to respond, use OpJoinOrCreateRoom instead.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// A room's name has to be unique (per region, appid and gameversion).
        /// When your title uses a global matchmaking or invitations (e.g. an external solution),
        /// keep regions and the game versions in mind to join a room.
        ///
        ///
        /// This client's State is set to ClientState.Joining immediately, when the operation could
        /// be called. In the background, the client will switch servers and call various related operations.
        ///
        /// When you're in the room, this client's State will become ClientState.Joined.
        ///
        ///
        /// When entering a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and are not wiped when leaving a room.
        ///
        /// You can define an array of expectedUsers, to reserve player slots in the room for friends or party members.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="enterRoomParams">Definition of properties for the room to join.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRoom(EnterRoomParams enterRoomParams)
        {
            if (!CheckIfOpCanBeSent(OperationCode.JoinGame, Server, "JoinRoom"))
            {
                return false;
            }
            bool onGameServer = Server == ServerConnection.GameServer;
            enterRoomParams.OnGameServer = onGameServer;
            if (!onGameServer)
            {
                enterRoomParamsCache = enterRoomParams;
            }

            bool sending = LoadBalancingPeer.OpJoinRoom(enterRoomParams);
            if (sending)
            {
                lastJoinType = (enterRoomParams.CreateIfNotExists) ? JoinType.JoinOrCreateRoom : JoinType.JoinRoom;
                State = ClientState.Joining;
            }
            return sending;
        }


        /// <summary>
        /// Rejoins a room by roomName (using the userID internally to return).  Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// Used to return to a room, before this user was removed from the players list.
        /// Internally, the userID will be checked by the server, to make sure this user is in the room (active or inactice).
        ///
        /// In contrast to join, this operation never adds a players to a room. It will attempt to retake an existing
        /// spot in the playerlist or fail. This makes sure the client doean't accidentally join a room when the
        /// game logic meant to re-activate an existing actor in an existing room.
        ///
        /// This method will fail on the server, when the room does not exist, can't be loaded (persistent rooms) or
        /// when the userId is not in the player list of this room. This will lead to a callback OnJoinRoomFailed.
        ///
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        /// </remarks>
        public bool OpRejoinRoom(string roomName)
        {
            bool onGameServer = Server == ServerConnection.GameServer;

            EnterRoomParams opParams = new EnterRoomParams();
            enterRoomParamsCache = opParams;
            opParams.RoomName = roomName;
            opParams.OnGameServer = onGameServer;
            opParams.RejoinOnly = true;

            bool sending = LoadBalancingPeer.OpJoinRoom(opParams);
            if (sending)
            {
                lastJoinType = JoinType.JoinRoom;
                State = ClientState.Joining;
            }
            return sending;
        }


        /// <summary>
        /// Leaves the current room, optionally telling the server that the user is just becoming inactive. Will callback: OnLeftRoom.
        /// </summary>
        ///
        /// <remarks>
        /// OpLeaveRoom skips execution when the room is null or the server is not GameServer or the client is disconnecting from GS already.
        /// OpLeaveRoom returns false in those cases and won't change the state, so check return of this method.
        ///
        /// In some cases, this method will skip the OpLeave call and just call Disconnect(),
        /// which not only leaves the room but also the server. Disconnect also triggers a leave and so that workflow is is quicker.
        /// </remarks>
        /// <param name="becomeInactive">If true, this player becomes inactive in the game and can return later (if PlayerTTL of the room is != 0).</param>
        /// <param name="sendAuthCookie">WebFlag: Securely transmit the encrypted object AuthCookie to the web service in PathLeave webhook when available</param>
        /// <returns>If the current room could be left (impossible while not in a room).</returns>
        public bool OpLeaveRoom(bool becomeInactive, bool sendAuthCookie = false)
        {
            if (!CheckIfOpCanBeSent(OperationCode.Leave, Server, "LeaveRoom"))
            {
                return false;
            }

            State = ClientState.Leaving;
            return LoadBalancingPeer.OpLeaveRoom(becomeInactive, sendAuthCookie);
        }


        /// <summary>Gets a list of games matching a SQL-like where clause.</summary>
        /// <remarks>
        /// Operation is only available for lobbies of type SqlLobby.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/matchmaking-and-lobby#sql_lobby_type"/>
        /// <param name="typedLobby">The lobby to query. Has to be of type SqlLobby.</param>
        /// <param name="sqlLobbyFilter">The sql query statement.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpGetGameList(TypedLobby typedLobby, string sqlLobbyFilter)
        {
            if (!CheckIfOpCanBeSent(OperationCode.GetGameList, Server, "GetGameList"))
            {
                return false;
            }
            return LoadBalancingPeer.OpGetGameList(typedLobby, sqlLobbyFilter);
        }


        ///  <summary>
        ///  Updates and synchronizes a Player's Custom Properties. Optionally, expectedProperties can be provided as condition.
        ///  </summary>
        ///  <remarks>
        ///  Custom Properties are a set of string keys and arbitrary values which is synchronized
        ///  for the players in a Room. They are available when the client enters the room, as
        ///  they are in the response of OpJoin and OpCreate.
        ///
        ///  Custom Properties either relate to the (current) Room or a Player (in that Room).
        ///
        ///  Both classes locally cache the current key/values and make them available as
        ///  property: CustomProperties. This is provided only to read them.
        ///  You must use the method SetCustomProperties to set/modify them.
        ///
        ///  Any client can set any Custom Properties anytime (when in a room).
        ///  It's up to the game logic to organize how they are best used.
        ///
        ///  You should call SetCustomProperties only with key/values that are new or changed. This reduces
        ///  traffic and performance.
        ///
        ///  Unless you define some expectedProperties, setting key/values is always permitted.
        ///  In this case, the property-setting client will not receive the new values from the server but
        ///  instead update its local cache in SetCustomProperties.
        ///
        ///  If you define expectedProperties, the server will skip updates if the server property-cache
        ///  does not contain all expectedProperties with the same values.
        ///  In this case, the property-setting client will get an update from the server and update it's
        ///  cached key/values at about the same time as everyone else.
        ///
        ///  The benefit of using expectedProperties can be only one client successfully sets a key from
        ///  one known value to another.
        ///  As example: Store who owns an item in a Custom Property "ownedBy". It's 0 initally.
        ///  When multiple players reach the item, they all attempt to change "ownedBy" from 0 to their
        ///  actorNumber. If you use expectedProperties {"ownedBy", 0} as condition, the first player to
        ///  take the item will have it (and the others fail to set the ownership).
        ///
        ///  Properties get saved with the game state for Turnbased games (which use IsPersistent = true).
        ///  </remarks>
        /// <param name="actorNr">Defines which player the Custom Properties belong to. ActorID of a player.</param>
        /// <param name="propertiesToSet">Hashtable of Custom Properties that changes.</param>
        /// <param name="expectedProperties">Provide some keys/values to use as condition for setting the new values. Client must be in room.</param>
        /// <param name="webFlags">Defines if the set properties should be forwarded to a WebHook. Client must be in room.</param>
        public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {

            if (CurrentRoom == null)
            {
                // if you attempt to set this player's values without conditions, then fine:
                if (expectedProperties == null && webFlags == null && LocalPlayer != null && LocalPlayer.ActorNumber == actorNr)
                {
                    LocalPlayer.SetCustomProperties(propertiesToSet);
                    return true;
                }

                if (LoadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + State);
                }
                return false;
            }

            Hashtable customActorProperties = new Hashtable();
            customActorProperties.MergeStringKeys(propertiesToSet);

            return OpSetPropertiesOfActor(actorNr, customActorProperties, expectedProperties, webFlags);
        }


        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (!CheckIfOpCanBeSent(OperationCode.SetProperties, Server, "SetProperties"))
            {
                return false;
            }
            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                Player target = CurrentRoom.GetPlayer(actorNr);
                if (target != null)
                {
                    target.InternalCacheProperties(actorProperties);
                }
            }

            return LoadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
        }


        ///  <summary>
        ///  Updates and synchronizes this Room's Custom Properties. Optionally, expectedProperties can be provided as condition.
        ///  </summary>
        ///  <remarks>
        ///  Custom Properties are a set of string keys and arbitrary values which is synchronized
        ///  for the players in a Room. They are available when the client enters the room, as
        ///  they are in the response of OpJoin and OpCreate.
        ///
        ///  Custom Properties either relate to the (current) Room or a Player (in that Room).
        ///
        ///  Both classes locally cache the current key/values and make them available as
        ///  property: CustomProperties. This is provided only to read them.
        ///  You must use the method SetCustomProperties to set/modify them.
        ///
        ///  Any client can set any Custom Properties anytime (when in a room).
        ///  It's up to the game logic to organize how they are best used.
        ///
        ///  You should call SetCustomProperties only with key/values that are new or changed. This reduces
        ///  traffic and performance.
        ///
        ///  Unless you define some expectedProperties, setting key/values is always permitted.
        ///  In this case, the property-setting client will not receive the new values from the server but
        ///  instead update its local cache in SetCustomProperties.
        ///
        ///  If you define expectedProperties, the server will skip updates if the server property-cache
        ///  does not contain all expectedProperties with the same values.
        ///  In this case, the property-setting client will get an update from the server and update it's
        ///  cached key/values at about the same time as everyone else.
        ///
        ///  The benefit of using expectedProperties can be only one client successfully sets a key from
        ///  one known value to another.
        ///  As example: Store who owns an item in a Custom Property "ownedBy". It's 0 initally.
        ///  When multiple players reach the item, they all attempt to change "ownedBy" from 0 to their
        ///  actorNumber. If you use expectedProperties {"ownedBy", 0} as condition, the first player to
        ///  take the item will have it (and the others fail to set the ownership).
        ///
        ///  Properties get saved with the game state for Turnbased games (which use IsPersistent = true).
        ///  </remarks>
        /// <param name="propertiesToSet">Hashtable of Custom Properties that changes.</param>
        /// <param name="expectedProperties">Provide some keys/values to use as condition for setting the new values.</param>
        /// <param name="webFlags">Defines web flags for an optional PathProperties webhook.</param>
        public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            Hashtable customGameProps = new Hashtable();
            customGameProps.MergeStringKeys(propertiesToSet);

            return OpSetPropertiesOfRoom(customGameProps, expectedProperties, webFlags);
        }


        protected internal void OpSetPropertyOfRoom(byte propCode, object value)
        {
            Hashtable properties = new Hashtable
            {
                [propCode] = value
            };
            OpSetPropertiesOfRoom(properties);
        }

        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        public bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (!CheckIfOpCanBeSent(OperationCode.SetProperties, Server, "SetProperties"))
            {
                return false;
            }
            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                CurrentRoom.InternalCacheProperties(gameProperties);
            }
            return LoadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
        }


        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="raiseEventOptions">Contains used send options. If you pass null, the default options will be used.</param>
        /// <param name="sendOptions">Send options for reliable, encryption etc</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            if (LoadBalancingPeer == null)
            {
                return false;
            }
            if (!CheckIfOpCanBeSent(OperationCode.RaiseEvent, Server, "RaiseEvent"))
            {
                return false;
            }
            return LoadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, raiseEventOptions, sendOptions);
        }



        /// <summary>
        /// Operation to handle this client's interest groups (for events in room).
        /// </summary>
        /// <remarks>
        /// Note the difference between passing null and byte[0]:
        ///   null won't add/remove any groups.
        ///   byte[0] will add/remove all (existing) groups.
        /// First, removing groups is executed. This way, you could leave all groups and join only the ones provided.
        ///
        /// Changes become active not immediately but when the server executes this operation (approximately RTT/2).
        /// </remarks>
        /// <param name="groupsToRemove">Groups to remove from interest. Null will not remove any. A byte[0] will remove all.</param>
        /// <param name="groupsToAdd">Groups to add to interest. Null will not add any. A byte[0] will add all current.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpChangeGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            if (LoadBalancingPeer == null)
            {
                return false;
            }
            if (!CheckIfOpCanBeSent(OperationCode.ChangeGroups, Server, "ChangeGroups"))
            {
                return false;
            }
            return LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
        }


        #endregion

        #region Helpers

        /// <summary>
        /// Privately used to read-out properties coming from the server in events and operation responses (which might be a bit tricky).
        /// </summary>
        private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
        {
            // read game properties and cache them locally
            if (CurrentRoom != null && gameProperties != null)
            {
                CurrentRoom.InternalCacheProperties(gameProperties);
                if (InRoom)
                {
                    InRoomCallbackTargets.OnRoomPropertiesUpdate(gameProperties);
                }
            }

            if (actorProperties != null && actorProperties.Count > 0)
            {
                if (targetActorNr > 0)
                {
                    // we have a single entry in the actorProperties with one user's name
                    // targets MUST exist before you set properties
                    Player target = CurrentRoom.GetPlayer(targetActorNr);
                    if (target != null)
                    {
                        Hashtable props = ReadoutPropertiesForActorNr(actorProperties, targetActorNr);
                        target.InternalCacheProperties(props);
                        InRoomCallbackTargets.OnPlayerPropertiesUpdate(target, props);
                    }
                }
                else
                {
                    // in this case, we've got a key-value pair per actor (each
                    // value is a hashtable with the actor's properties then)
                    int actorNr;
                    Hashtable props;
                    string newName;
                    Player target;

                    foreach (object key in actorProperties.Keys)
                    {
                        actorNr = (int)key;
                        props = (Hashtable)actorProperties[key];
                        newName = (string)props[ActorProperties.PlayerName];

                        target = CurrentRoom.GetPlayer(actorNr);
                        if (target == null)
                        {
                            target = CreatePlayer(newName, actorNr, false, props);
                            CurrentRoom.StorePlayer(target);
                        }
                        target.InternalCacheProperties(props);
                    }
                }
            }
        }


        /// <summary>
        /// Privately used only to read properties for a distinct actor (which might be the hashtable OR a key-pair value IN the actorProperties).
        /// </summary>
        private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
        {
            if (actorProperties.ContainsKey(actorNr))
            {
                return (Hashtable)actorProperties[actorNr];
            }

            return actorProperties;
        }

        /// <summary>
        /// Internally used to set the LocalPlayer's ID (from -1 to the actual in-room ID).
        /// </summary>
        /// <param name="newID">New actor ID (a.k.a actorNr) assigned when joining a room.</param>
        public void ChangeLocalID(int newID)
        {
            if (LocalPlayer == null)
            {
                DebugReturn(DebugLevel.WARNING, string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", LocalPlayer, CurrentRoom.Players == null, newID));
            }

            if (CurrentRoom == null)
            {
                // change to new actor/player ID and make sure the player does not have a room reference left
                LocalPlayer.ChangeLocalID(newID);
                LocalPlayer.RoomReference = null;
            }
            else
            {
                // remove old actorId from actor list
                CurrentRoom.RemovePlayer(LocalPlayer);

                // change to new actor/player ID
                LocalPlayer.ChangeLocalID(newID);

                // update the room's list with the new reference
                CurrentRoom.StorePlayer(LocalPlayer);
            }
        }



        /// <summary>
        /// Called internally, when a game was joined or created on the game server successfully.
        /// </summary>
        /// <remarks>
        /// This reads the response, finds out the local player's actorNumber (a.k.a. Player.ID) and applies properties of the room and players.
        /// Errors for these operations are to be handled before this method is called.
        /// </remarks>
        /// <param name="operationResponse">Contains the server's response for an operation called by this peer.</param>
        private void GameEnteredOnGameServer(OperationResponse operationResponse)
        {
            CurrentRoom = CreateRoom(enterRoomParamsCache.RoomName, enterRoomParamsCache.RoomOptions);
            CurrentRoom.LoadBalancingClient = this;

            // first change the local id, instead of first updating the actorList since actorList uses ID to update itself

            // the local player's actor-properties are not returned in join-result. add this player to the list
            int localActorNr = (int)operationResponse[ParameterCode.ActorNr];
            ChangeLocalID(localActorNr);

            if (operationResponse.Parameters.ContainsKey(ParameterCode.ActorList))
            {
                int[] actorsInRoom = (int[])operationResponse.Parameters[ParameterCode.ActorList];
                UpdatedActorList(actorsInRoom);
            }


            Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
            Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
            ReadoutProperties(gameProperties, actorProperties, 0);

            State = ClientState.Joined;

            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    MatchMakingCallbackTargets.OnCreatedRoom();
                    break;
                case OperationCode.JoinGame:
                case OperationCode.JoinRandomGame:
                    // "game joined" should be called in another place (the join ev contains important info for the room).
                    break;
            }
        }

        private void UpdatedActorList(int[] actorsInGame)
        {
            if (actorsInGame != null)
            {
                foreach (int userId in actorsInGame)
                {
                    Player target = CurrentRoom.GetPlayer(userId);
                    if (target == null)
                    {
                        CurrentRoom.StorePlayer(CreatePlayer(string.Empty, userId, false, null));
                    }
                }
            }
        }

        /// <summary>
        /// Factory method to create a player instance - override to get your own player-type with custom features.
        /// </summary>
        /// <param name="actorName">The name of the player to be created. </param>
        /// <param name="actorNumber">The player ID (a.k.a. actorNumber) of the player to be created.</param>
        /// <param name="isLocal">Sets the distinction if the player to be created is your player or if its assigned to someone else.</param>
        /// <param name="actorProperties">The custom properties for this new player</param>
        /// <returns>The newly created player</returns>
        protected internal virtual Player CreatePlayer(string actorName, int actorNumber, bool isLocal, Hashtable actorProperties)
        {
            Player newPlayer = new Player(actorName, actorNumber, isLocal, actorProperties);
            return newPlayer;
        }

        /// <summary>Internal "factory" method to create a room-instance.</summary>
        protected internal virtual Room CreateRoom(string roomName, RoomOptions opt)
        {
            Room r = new Room(roomName, opt);
            return r;
        }

        private bool CheckIfOpAllowedOnServer(byte opCode, ServerConnection serverConnection)
        {
            switch (serverConnection)
            {
                case ServerConnection.MasterServer:
                    switch (opCode)
                    {
                        case OperationCode.CreateGame:
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.FindFriends:
                        case OperationCode.GetGameList:
                        case OperationCode.GetLobbyStats:
                        case OperationCode.JoinGame:
                        case OperationCode.JoinLobby:
                        case OperationCode.LeaveLobby:
                        case OperationCode.WebRpc:
                        case OperationCode.ServerSettings:
                        case OperationCode.JoinRandomGame:
                            return true;
                    }
                    break;
                case ServerConnection.GameServer:
                    switch (opCode)
                    {
                        case OperationCode.CreateGame:
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.ChangeGroups:
                        case OperationCode.GetProperties:
                        case OperationCode.JoinGame:
                        case OperationCode.Leave:
                        case OperationCode.WebRpc:
                        case OperationCode.ServerSettings:
                        case OperationCode.SetProperties:
                        case OperationCode.RaiseEvent:
                            return true;
                    }
                    break;
                case ServerConnection.NameServer:
                    switch (opCode)
                    {
                        case OperationCode.Authenticate:
                        case OperationCode.AuthenticateOnce:
                        case OperationCode.GetRegions:
                        case OperationCode.ServerSettings:
                            return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("serverConnection", serverConnection, null);
            }
            return false;
        }

        private bool CheckIfOpCanBeSent(byte opCode, ServerConnection serverConnection, string opName)
        {
            if (LoadBalancingPeer == null)
            {
                DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) can't be sent because peer is null", opName, opCode));
                return false;
            }
            if (!CheckIfOpAllowedOnServer(opCode, serverConnection))
            {
                if (LoadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not allowed on current server ({2})", opName, opCode, serverConnection));
                }
                return false;
            }
            if (!CheckIfClientIsReadyToCallOperation(opCode))
            {
                if (LoadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not called because client is not connected or not ready yet, client state: {2}", opName, opCode, Enum.GetName(typeof(ClientState), State)));
                }
                return false;
            }
            return true;
        }

        private bool CheckIfClientIsReadyToCallOperation(byte opCode)
        {
            switch (opCode)
            {
                //case OperationCode.ServerSettings: // ??
                //case OperationCode.WebRpc: // WebRPC works on MS and GS and I think it does not need the client to be ready

                case OperationCode.Authenticate:
                case OperationCode.AuthenticateOnce:
                    return IsConnectedAndReady ||
                         State == ClientState.ConnectingToNameServer || // this is required since we do not set state to ConnectedToNameServer before authentication
                        State == ClientState.ConnectingToMasterServer || // this is required since we do not set state to ConnectedToMasterServer before authentication
                        State == ClientState.ConnectingToGameServer; // this is required since we do not set state to ConnectedToGameServer before authentication

                case OperationCode.ChangeGroups:
                case OperationCode.GetProperties:
                case OperationCode.SetProperties:
                case OperationCode.RaiseEvent:
                case OperationCode.Leave:
                    return InRoom;

                case OperationCode.JoinGame:
                case OperationCode.CreateGame:
                    return State == ClientState.ConnectedToMasterServer || InLobby || State == ClientState.ConnectedToGameServer; // CurrentRoom can be not null in case of quick rejoin

                case OperationCode.LeaveLobby:
                    return InLobby;

                case OperationCode.JoinRandomGame:
                case OperationCode.FindFriends:
                case OperationCode.GetGameList:
                case OperationCode.GetLobbyStats: // do we need to be inside lobby to call this?
                case OperationCode.JoinLobby: // You don't have to explicitly leave a lobby to join another (client can be in one max, at any time)
                    return State == ClientState.ConnectedToMasterServer || InLobby;
                case OperationCode.GetRegions:
                    return State == ClientState.ConnectedToNameServer;
            }
            return IsConnected;
        }

        #endregion

        #region Implementation of IPhotonPeerListener

        /// <summary>Debug output of low level api (and this client).</summary>
        /// <remarks>This method is not responsible to keep up the state of a LoadBalancingClient. Calling base.DebugReturn on overrides is optional.</remarks>
        public virtual void DebugReturn(DebugLevel level, string message)
        {
            if (LoadBalancingPeer.DebugOut != DebugLevel.ALL && level > LoadBalancingPeer.DebugOut)
            {
                return;
            }
#if !SUPPORTED_UNITY
            Debug.WriteLine(message);
#else
            if (level == DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == DebugLevel.WARNING)
            {
                Debug.LogWarning(message);
            }
            else if (level == DebugLevel.INFO)
            {
                Debug.Log(message);
            }
            else if (level == DebugLevel.ALL)
            {
                Debug.Log(message);
            }
#endif
        }

        private void CallbackRoomEnterFailed(OperationResponse operationResponse)
        {
            if (operationResponse.ReturnCode != 0)
            {
                if (operationResponse.OperationCode == OperationCode.JoinGame)
                {
                    MatchMakingCallbackTargets.OnJoinRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
                }
                else if (operationResponse.OperationCode == OperationCode.CreateGame)
                {
                    MatchMakingCallbackTargets.OnCreateRoomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
                }
                else if (operationResponse.OperationCode == OperationCode.JoinRandomGame)
                {
                    MatchMakingCallbackTargets.OnJoinRandomFailed(operationResponse.ReturnCode, operationResponse.DebugMessage);
                }
            }
        }

        /// <summary>
        /// Uses the OperationResponses provided by the server to advance the internal state and call ops as needed.
        /// </summary>
        /// <remarks>
        /// When this method finishes, it will call your OnOpResponseAction (if any). This way, you can get any
        /// operation response without overriding this class.
        ///
        /// To implement a more complex game/app logic, you should implement your own class that inherits the
        /// LoadBalancingClient. Override this method to use your own operation-responses easily.
        ///
        /// This method is essential to update the internal state of a LoadBalancingClient, so overriding methods
        /// must call base.OnOperationResponse().
        /// </remarks>
        /// <param name="operationResponse">Contains the server's response for an operation called by this peer.</param>
        public virtual void OnOperationResponse(OperationResponse operationResponse)
        {
            // if (operationResponse.ReturnCode != 0) this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull());

            // use the "secret" or "token" whenever we get it. doesn't really matter if it's in AuthResponse.
            if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
            {
                if (AuthValues == null)
                {
                    AuthValues = new AuthenticationValues();
                    //this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created AuthValues.");
                }

                AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
                tokenCache = AuthValues.Token;
            }

            switch (operationResponse.OperationCode)
            {
                case OperationCode.Authenticate:
                case OperationCode.AuthenticateOnce:
                {
                    if (operationResponse.ReturnCode != 0)
                    {
                        DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull() + " Server: " + Server + " Address: " + LoadBalancingPeer.ServerAddress);

                        switch (operationResponse.ReturnCode)
                        {
                            case ErrorCode.InvalidAuthentication:
                                DisconnectedCause = DisconnectCause.InvalidAuthentication;
                                ConnectionCallbackTargets.OnDisconnected(DisconnectCause.InvalidAuthentication);
                                break;
                            case ErrorCode.CustomAuthenticationFailed:
                                DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
                                ConnectionCallbackTargets.OnCustomAuthenticationFailed(operationResponse.DebugMessage);
                                break;
                            case ErrorCode.InvalidRegion:
                                DisconnectedCause = DisconnectCause.InvalidRegion;
                                ConnectionCallbackTargets.OnDisconnected(DisconnectCause.InvalidRegion);
                                break;
                            case ErrorCode.MaxCcuReached:
                                DisconnectedCause = DisconnectCause.MaxCcuReached;
                                ConnectionCallbackTargets.OnDisconnected(DisconnectCause.MaxCcuReached);
                                break;
                            case ErrorCode.OperationNotAllowedInCurrentState:
                                DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
                                break;
                            case ErrorCode.AuthenticationTicketExpired:
                                DisconnectedCause = DisconnectCause.AuthenticationTicketExpired;
                                ConnectionCallbackTargets.OnDisconnected(DisconnectCause.AuthenticationTicketExpired);
                                break;
                        }

                        Disconnect();
                        break;  // if auth didn't succeed, we disconnect (above) and exit this operation's handling
                    }

                    if (Server == ServerConnection.NameServer || Server == ServerConnection.MasterServer)
                    {
                        if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                        {
                            string incomingId = (string)operationResponse.Parameters[ParameterCode.UserId];
                            if (!string.IsNullOrEmpty(incomingId))
                            {
                                UserId = incomingId;
                                LocalPlayer.UserId = incomingId;
                                DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", UserId));
                            }
                        }
                        if (operationResponse.Parameters.ContainsKey(ParameterCode.NickName))
                        {
                            NickName = (string)operationResponse.Parameters[ParameterCode.NickName];
                            DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", NickName));
                        }

                        if (operationResponse.Parameters.ContainsKey(ParameterCode.EncryptionData))
                        {
                            SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[ParameterCode.EncryptionData]);
                        }
                    }

                    if (Server == ServerConnection.NameServer)
                    {
                        // on the NameServer, authenticate returns the MasterServer address for a region and we hop off to there
                        MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                        if (LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Udp && UseAlternativeUdpPorts)
                        {
                            // TODO: Make this work with AuthOnceWss, which uses WSS on NameServer but "expects" to use UDP...
                            MasterServerAddress = MasterServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                        }

                        if (AuthMode == AuthModeOption.AuthOnceWss)
                        {
                            DebugReturn(DebugLevel.INFO, string.Format("Due to AuthOnceWss, switching TransportProtocol to ExpectedProtocol: {0}.", ExpectedProtocol));
                            LoadBalancingPeer.TransportProtocol = ExpectedProtocol;
                        }
                        DisconnectToReconnect();
                    }
                    else if (Server == ServerConnection.MasterServer)
                    {
                        State = ClientState.ConnectedToMasterServer;
                        if (failedRoomEntryOperation == null)
                        {
                            ConnectionCallbackTargets.OnConnectedToMaster();
                        }
                        else
                        {
                            CallbackRoomEnterFailed(failedRoomEntryOperation);
                            failedRoomEntryOperation = null;
                        }

                        if (AuthMode != AuthModeOption.Auth)
                        {
                            LoadBalancingPeer.OpSettings(EnableLobbyStatistics);
                        }
                    }
                    else if (Server == ServerConnection.GameServer)
                    {
                        State = ClientState.Joining;

                        if (enterRoomParamsCache.RejoinOnly)
                        {
                            enterRoomParamsCache.PlayerProperties = null;
                        }
                        else
                        {
                            Hashtable allProps = new Hashtable();
                            allProps.Merge(LocalPlayer.CustomProperties);
                            allProps[ActorProperties.PlayerName] = LocalPlayer.NickName;

                            enterRoomParamsCache.PlayerProperties = allProps;
                        }

                        enterRoomParamsCache.OnGameServer = true;

                        if (lastJoinType == JoinType.JoinRoom || lastJoinType == JoinType.JoinRandomRoom || lastJoinType == JoinType.JoinOrCreateRoom)
                        {
                            LoadBalancingPeer.OpJoinRoom(enterRoomParamsCache);
                        }
                        else if (lastJoinType == JoinType.CreateRoom)
                        {
                            LoadBalancingPeer.OpCreateRoom(enterRoomParamsCache);
                        }
                        break;
                    }

                    // optionally, OpAuth may return some data for the client to use. if it's available, call OnCustomAuthenticationResponse
                    Dictionary<string, object> data = (Dictionary<string, object>)operationResponse[ParameterCode.Data];
                    if (data != null)
                    {
                        ConnectionCallbackTargets.OnCustomAuthenticationResponse(data);
                    }
                    break;
                }

                case OperationCode.GetRegions:
                    // Debug.Log("GetRegions returned: " + operationResponse.ToStringFull());

                    if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                    {
                        DebugReturn(DebugLevel.ERROR, string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                        ConnectionCallbackTargets.OnCustomAuthenticationFailed("Invalid Authentication");

                        Disconnect();
                        break;
                    }
                    if (operationResponse.ReturnCode != ErrorCode.Ok)
                    {
                        DebugReturn(DebugLevel.ERROR, "GetRegions failed. Can't provide regions list. Error: " + operationResponse.ReturnCode + ": " + operationResponse.DebugMessage);
                        break;
                    }
                    if (RegionHandler == null)
                    {
                        RegionHandler = new RegionHandler();
                    }

                    if (RegionHandler.IsPinging)
                    {
                        DebugReturn(DebugLevel.WARNING, "Received an response for OpGetRegions while the RegionHandler is pinging regions already. Skipping this response in favor of completing the current region-pinging.");
                        return; // in this particular case, we suppress the duplicate GetRegion response. we don't want a callback for this, cause there is a warning already.
                    }

                    RegionHandler.SetRegions(operationResponse);
                    ConnectionCallbackTargets.OnRegionListReceived(RegionHandler);
                    break;

                case OperationCode.JoinRandomGame:  // this happens only on the master server. on gameserver this is a "regular" join
                case OperationCode.CreateGame:
                case OperationCode.JoinGame:

                    if (operationResponse.ReturnCode != 0)
                    {
                        if (Server == ServerConnection.GameServer)
                        {
                            failedRoomEntryOperation = operationResponse;
                            DisconnectToReconnect();
                        }
                        else
                        {
                            State = (InLobby) ? ClientState.JoinedLobby : ClientState.ConnectedToMasterServer;
                            CallbackRoomEnterFailed(operationResponse);
                        }
                    }
                    else
                    {
                        if (Server == ServerConnection.GameServer)
                        {
                            GameEnteredOnGameServer(operationResponse);
                        }
                        else
                        {
                            GameServerAddress = (string)operationResponse[ParameterCode.Address];
                            if (LoadBalancingPeer.TransportProtocol == ConnectionProtocol.Udp && UseAlternativeUdpPorts)
                            {
                                GameServerAddress = GameServerAddress.Replace("5058", "27000").Replace("5055", "27001").Replace("5056", "27002");
                            }

                            string roomName = operationResponse[ParameterCode.RoomName] as string;
                            if (!string.IsNullOrEmpty(roomName))
                            {
                                enterRoomParamsCache.RoomName = roomName;
                            }

                            DisconnectToReconnect();
                        }
                    }
                    break;

                case OperationCode.GetGameList:
                    if (operationResponse.ReturnCode != 0)
                    {
                        DebugReturn(DebugLevel.ERROR, "GetGameList failed: " + operationResponse.ToStringFull());
                        break;
                    }

                    List<RoomInfo> _RoomInfoList = new List<RoomInfo>();

                    Hashtable games = (Hashtable)operationResponse[ParameterCode.GameList];
                    foreach (string gameName in games.Keys)
                    {
                        _RoomInfoList.Add(new RoomInfo(gameName, (Hashtable)games[gameName]));
                    }

                    LobbyCallbackTargets.OnRoomListUpdate(_RoomInfoList);
                    break;

                case OperationCode.JoinLobby:
                    State = ClientState.JoinedLobby;
                    LobbyCallbackTargets.OnJoinedLobby();
                    break;

                case OperationCode.LeaveLobby:
                    State = ClientState.ConnectedToMasterServer;
                    // TODO: clean room list?!
                    LobbyCallbackTargets.OnLeftLobby();
                    break;

                case OperationCode.Leave:
                    DisconnectToReconnect();
                    break;

                case OperationCode.FindFriends:
                    if (operationResponse.ReturnCode != 0)
                    {
                        DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
                        friendListRequested = null;
                        break;
                    }

                    bool[] onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                    string[] roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                    //if (onlineList == null || roomList == null || this.friendListRequested == null || onlineList.Length != this.friendListRequested.Length)
                    //{
                    //    // TODO: Check if we should handle this case better / more extensively
                    //    this.DebugReturn(DebugLevel.ERROR, "OpFindFriends failed. Some list is not set. OpResponse: " + operationResponse.ToStringFull());
                    //    this.friendListRequested = null;
                    //    this.isFetchingFriendList = false;
                    //    break;
                    //}

                    List<FriendInfo> friendList = new List<FriendInfo>(friendListRequested.Length);
                    for (int index = 0; index < friendListRequested.Length; index++)
                    {
                        FriendInfo friend = new FriendInfo
                        {
                            UserId = friendListRequested[index],
                            Room = roomList[index],
                            IsOnline = onlineList[index]
                        };
                        friendList.Insert(index, friend);
                    }

                    friendListRequested = null;

                    MatchMakingCallbackTargets.OnFriendListUpdate(friendList);
                    break;

                case OperationCode.WebRpc:
                    WebRpcCallbackTargets.OnWebRpcResponse(operationResponse);
                    break;
            }

            if (OpResponseReceived != null)
            {
                OpResponseReceived(operationResponse);
            }
        }

        /// <summary>
        /// Uses the connection's statusCodes to advance the internal state and call operations as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnStatusChanged.</remarks>
        public virtual void OnStatusChanged(StatusCode statusCode)
        {
            switch (statusCode)
            {
                case StatusCode.Connect:
                    if (State == ClientState.ConnectingToNameServer)
                    {
                        if (LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
                        }

                        Server = ServerConnection.NameServer;
                        if (AuthValues != null)
                        {
                            AuthValues.Token = null; // when connecting to NameServer, invalidate the secret (only)
                        }
                    }

                    if (State == ClientState.ConnectingToGameServer)
                    {
                        if (LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
                        }

                        Server = ServerConnection.GameServer;
                    }

                    if (State == ClientState.ConnectingToMasterServer)
                    {
                        if (LoadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
                        }

                        Server = ServerConnection.MasterServer;
                        ConnectionCallbackTargets.OnConnected(); // if initial connect
                    }


                    if (LoadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure)
                    {
                        if (Server == ServerConnection.NameServer || AuthMode == AuthModeOption.Auth)
                        {
                            LoadBalancingPeer.EstablishEncryption();
                        }
                    }
                    else
                    {
                        goto case StatusCode.EncryptionEstablished;
                    }

                    break;

                case StatusCode.EncryptionEstablished:
                    if (Server == ServerConnection.NameServer)
                    {
                        State = ClientState.ConnectedToNameServer;

                        // if there is no specific region to connect to, get available regions from the Name Server. the result triggers next actions in workflow
                        if (string.IsNullOrEmpty(CloudRegion))
                        {
                            OpGetRegions();
                            break;
                        }
                    }
                    else
                    {
                        // auth AuthOnce, no explicit authentication is needed on Master Server and Game Server. this is done via token, so: break
                        if (AuthMode == AuthModeOption.AuthOnce || AuthMode == AuthModeOption.AuthOnceWss)
                        {
                            break;
                        }
                    }

                    // authenticate in all other cases
                    bool authenticating = CallAuthenticate();
                    if (authenticating)
                    {
                        State = ClientState.Authenticating;
                    }
                    else
                    {
                        DebugReturn(DebugLevel.ERROR, "OpAuthenticate failed. Check log output and AuthValues. State: " + State);
                    }
                    break;


                case StatusCode.Disconnect:
                    // disconnect due to connection exception is handled below (don't connect to GS or master in that case)

                    ChangeLocalID(-1);
                    friendListRequested = null;

                    bool wasInRoom = CurrentRoom != null;
                    CurrentRoom = null;    // players get cleaned up inside this, too, except LocalPlayer (which we keep)

                    if (Server == ServerConnection.GameServer && wasInRoom)
                    {
                        MatchMakingCallbackTargets.OnLeftRoom();
                    }

                    switch (State)
                    {
                        case ClientState.PeerCreated:
                        case ClientState.Disconnecting:
                            if (AuthValues != null)
                            {
                                AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                            }
                            State = ClientState.Disconnected;
                            ConnectionCallbackTargets.OnDisconnected(DisconnectedCause);
                            break;

                        case ClientState.DisconnectingFromGameServer:
                        case ClientState.DisconnectingFromNameServer:
                            Connect();                 // this gets the client back to the Master Server
                            break;

                        case ClientState.DisconnectingFromMasterServer:
                            ConnectToGameServer();     // this connects the client with the Game Server (when joining/creating a room)
                            break;

                        case ClientState.Disconnected:
                            // this client is already Disconnected, so no further action is needed.
                            // this.DebugReturn(DebugLevel.INFO, "LBC.OnStatusChanged(Disconnect) this.State: " + this.State + ". Server: " + this.Server);
                            break;

                        default:
                            string stacktrace = "";
#if DEBUG && !NETFX_CORE
                            stacktrace = new System.Diagnostics.StackTrace(true).ToString();
#endif
                            DebugReturn(DebugLevel.WARNING, "Got a unexpected Disconnect in LoadBalancingClient State: " + State + ". Server: " + Server + " Trace: " + stacktrace);

                            if (AuthValues != null)
                            {
                                AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                            }
                            State = ClientState.Disconnected;
                            ConnectionCallbackTargets.OnDisconnected(DisconnectCause.None);
                            break;
                    }
                    break;

                case StatusCode.DisconnectByServerUserLimit:
                    DebugReturn(DebugLevel.ERROR, "This connection was rejected due to the apps CCU limit.");
                    DisconnectedCause = DisconnectCause.MaxCcuReached;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.ExceptionOnConnect:
                case StatusCode.SecurityExceptionOnConnect:
                    DisconnectedCause = DisconnectCause.ExceptionOnConnect;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.Exception:
                case StatusCode.ExceptionOnReceive:
                    DisconnectedCause = DisconnectCause.Exception;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.DisconnectByServerTimeout:
                    DisconnectedCause = DisconnectCause.ServerTimeout;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.DisconnectByServerLogic:
                    DisconnectedCause = DisconnectCause.DisconnectByServerLogic;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.DisconnectByServerReasonUnknown:
                    DisconnectedCause = DisconnectCause.DisconnectByServerReasonUnknown;
                    State = ClientState.Disconnecting;
                    break;
                case StatusCode.TimeoutDisconnect:
                    DisconnectedCause = DisconnectCause.ClientTimeout;
                    State = ClientState.Disconnecting;
                    break;
            }
        }


        /// <summary>
        /// Uses the photonEvent's provided by the server to advance the internal state and call ops as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnEvent.</remarks>
        public virtual void OnEvent(EventData photonEvent)
        {
            int actorNr = photonEvent.Sender;
            Player originatingPlayer = (CurrentRoom != null) ? CurrentRoom.GetPlayer(actorNr) : null;

            switch (photonEvent.Code)
            {
                case EventCode.GameList:
                case EventCode.GameListUpdate:

                    List<RoomInfo> _RoomInfoList = new List<RoomInfo>();

                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (string gameName in games.Keys)
                    {
                        _RoomInfoList.Add(new RoomInfo(gameName, (Hashtable)games[gameName]));
                    }

                    LobbyCallbackTargets.OnRoomListUpdate(_RoomInfoList);

                    break;

                case EventCode.Join:
                    Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];

                    if (originatingPlayer == null)
                    {
                        originatingPlayer = CreatePlayer(string.Empty, actorNr, false, actorProperties);
                        CurrentRoom.StorePlayer(originatingPlayer);
                    }
                    else
                    {
                        originatingPlayer.InternalCacheProperties(actorProperties);
                        originatingPlayer.IsInactive = false;
                    }

                    if (actorNr == LocalPlayer.ActorNumber)
                    {
                        // in this player's own join event, we get a complete list of players in the room, so check if we know each of the
                        int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                        UpdatedActorList(actorsInRoom);
                        // joinWithCreateOnDemand can turn an OpJoin into creating the room. Then actorNumber is 1 and callback: OnCreatedRoom()
                        if (lastJoinType == JoinType.JoinOrCreateRoom && LocalPlayer.ActorNumber == 1)
                        {
                            MatchMakingCallbackTargets.OnCreatedRoom();
                        }

                        MatchMakingCallbackTargets.OnJoinedRoom();
                    }
                    else
                    {
                        InRoomCallbackTargets.OnPlayerEnteredRoom(originatingPlayer);
                    }
                    break;

                case EventCode.Leave:
                    bool isInactive = false;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.IsInactive))
                    {
                        isInactive = (bool)photonEvent.Parameters[ParameterCode.IsInactive];
                    }

                    if (isInactive)
                    {
                        originatingPlayer.IsInactive = true;
                    }
                    else
                    {
                        CurrentRoom.RemovePlayer(actorNr);
                    }

                    if (photonEvent.Parameters.ContainsKey(ParameterCode.MasterClientId))
                    {
                        int newMaster = (int)photonEvent[ParameterCode.MasterClientId];
                        if (newMaster != 0)
                        {
                            CurrentRoom.masterClientId = newMaster;
                            InRoomCallbackTargets.OnMasterClientSwitched(CurrentRoom.GetPlayer(newMaster));
                        }
                    }
                    // finally, send notification that a player left
                    InRoomCallbackTargets.OnPlayerLeftRoom(originatingPlayer); // TODO: make sure it's clear how to separate "inactive"- from "abandon"-leave, as well as kicked out
                    break;

                case EventCode.PropertiesChanged:
                    // whenever properties are sent in-room, they can be broadcasted as event (which we handle here)
                    // we get PLAYERproperties if actorNr > 0 or ROOMproperties if actorNumber is not set or 0
                    int targetActorNr = 0;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.TargetActorNr))
                    {
                        targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
                    }

                    Hashtable gameProperties = null;
                    Hashtable actorProps = null;
                    if (targetActorNr == 0)
                    {
                        gameProperties = (Hashtable)photonEvent[ParameterCode.Properties];
                    }
                    else
                    {
                        actorProps = (Hashtable)photonEvent[ParameterCode.Properties];
                    }

                    ReadoutProperties(gameProperties, actorProps, targetActorNr);
                    break;

                case EventCode.AppStats:
                    // only the master server sends these in (1 minute) intervals
                    PlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                    RoomsCount = (int)photonEvent[ParameterCode.GameCount];
                    PlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                    break;

                case EventCode.LobbyStats:
                    string[] names = photonEvent[ParameterCode.LobbyName] as string[];
                    byte[] types = photonEvent[ParameterCode.LobbyType] as byte[];
                    int[] peers = photonEvent[ParameterCode.PeerCount] as int[];
                    int[] rooms = photonEvent[ParameterCode.GameCount] as int[];

                    lobbyStatistics.Clear();
                    for (int i = 0; i < names.Length; i++)
                    {
                        TypedLobbyInfo info = new TypedLobbyInfo
                        {
                            Name = names[i],
                            Type = (LobbyType)types[i],
                            PlayerCount = peers[i],
                            RoomCount = rooms[i]
                        };

                        lobbyStatistics.Add(info);
                    }

                    LobbyCallbackTargets.OnLobbyStatisticsUpdate(lobbyStatistics);
                    break;

                case EventCode.ErrorInfo:
                    // if (this.OnEventAction != null) this.OnEventAction(photonEvent); // this gets called below for all events!
                    break;

                case EventCode.AuthEvent:
                    if (AuthValues == null)
                    {
                        AuthValues = new AuthenticationValues();
                    }

                    AuthValues.Token = photonEvent[ParameterCode.Secret] as string;
                    tokenCache = AuthValues.Token;
                    break;

            }

            UpdateCallbackTargets();
            if (EventReceived != null)
            {
                EventReceived(photonEvent);
            }
        }

        /// <summary>In Photon 4, "raw messages" will get their own callback method in the interface. Not used yet.</summary>
        public virtual void OnMessage(object message)
        {
            DebugReturn(DebugLevel.ALL, string.Format("got OnMessage {0}", message));
        }

        #endregion

        private void SetupEncryption(Dictionary<byte, object> encryptionData)
        {
            EncryptionMode mode = (EncryptionMode)(byte)encryptionData[EncryptionDataParameters.Mode];
            switch (mode)
            {
                case EncryptionMode.PayloadEncryption:
                    byte[] encryptionSecret = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                    LoadBalancingPeer.InitPayloadEncryption(encryptionSecret);
                    break;
                case EncryptionMode.DatagramEncryption:
                case EncryptionMode.DatagramEncryptionRandomSequence:
                {
                    byte[] secret1 = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                    byte[] secret2 = (byte[])encryptionData[EncryptionDataParameters.Secret2];
                    LoadBalancingPeer.InitDatagramEncryption(secret1, secret2, mode == EncryptionMode.DatagramEncryptionRandomSequence);
                }
                break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        /// <summary>
        /// This operation makes Photon call your custom web-service by path/name with the given parameters (converted into Json).
        /// </summary>
        /// <remarks>
        /// A WebRPC calls a custom, http-based function on a server you provide. The uriPath is relative to a "base path"
        /// which is configured server-side. The sent parameters get converted from C# types to Json. Vice versa, the response
        /// of the web-service will be converted to C# types and sent back as normal operation response.
        ///
        ///
        /// To use this feature, you have to setup your server:
        ///
        /// For a Photon Cloud application, <a href="https://doc.photonengine.com/en-us/realtime/current/reference/webhooks">
        /// visit the Dashboard </a> and setup "WebHooks". The BaseUrl is used for WebRPCs as well.
        ///
        ///
        /// The response by Photon will call OnOperationResponse() with Code: OperationCode.WebRpc.
        /// To get this response, you can derive the LoadBalancingClient, or (much easier) you set a suitable
        /// OnOpResponseAction to be called.
        ///
        ///
        /// It's important to understand that the OperationResponse tells you if the WebRPC could be called or not
        /// but the content of the response will contain the values the web-service sent (if any).
        /// If the web-service could not execute the request, it might return another error and a message. This is
        /// inside the OperationResponse.
        ///
        /// The class WebRpcResponse is a helper-class that extracts the most valuable content from the WebRPC
        /// response.
        /// </remarks>
        /// <example>
        /// To get a WebRPC response, set a OnOpResponseAction:
        ///
        ///     this.OnOpResponseAction = this.OpResponseHandler;
        ///
        /// It could look like this:
        ///
        ///     public void OpResponseHandler(OperationResponse operationResponse)
        ///     {
        ///         if (operationResponse.OperationCode == OperationCode.WebRpc)
        ///         {
        ///             if (operationResponse.ReturnCode != 0)
        ///             {
        ///                 Console.WriteLine("WebRpc failed. Response: " + operationResponse.ToStringFull());
        ///             }
        ///             else
        ///             {
        ///                 WebRpcResponse webResponse = new WebRpcResponse(operationResponse);
        ///                 Console.WriteLine(webResponse.DebugMessage);    // message from the webserver
        ///
        ///                 // do something with the response...
        ///             }
        ///         }
        ///     }
        /// </example>
        /// <param name="uriPath">The url path to call, relative to the baseUrl configured on Photon's server-side.</param>
        /// <param name="parameters">The parameters to send to the web-service method.</param>
        /// <param name="sendAuthCookie">Defines if the authentication cookie gets sent to a WebHook (if setup).</param>
        public bool OpWebRpc(string uriPath, object parameters, bool sendAuthCookie = false)
        {
            if (!CheckIfOpAllowedOnServer(OperationCode.WebRpc, Server))
            {
                DebugReturn(DebugLevel.ERROR, string.Format("Operation {0} ({1}) not allowed on current server ({2})", "WebRPC", OperationCode.WebRpc, Server));
                return false;
            }
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>
            {
                { ParameterCode.UriPath, uriPath },
                { ParameterCode.WebRpcParameters, parameters }
            };
            if (sendAuthCookie)
            {
                opParameters.Add(ParameterCode.EventForward, WebFlags.SendAuthCookieConst);
            }

            //return this.LoadBalancingPeer.OpCustom(OperationCode.WebRpc, opParameters, true);
            return LoadBalancingPeer.SendOperation(OperationCode.WebRpc, opParameters, SendOptions.SendReliable);
        }


        /// <summary>
        /// Registers an object for callbacks for the implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// Adding and removing callback targets is queued to not mess with callbacks in execution.
        /// Internally, this means that the addition/removal is done before the LoadBalancingClient
        /// calls the next callbacks. This detail should not affect a game's workflow.
        /// 
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks"/>
        /// </remarks>
        /// <param name="target">The object that registers to get callbacks from this client.</param>
        public void AddCallbackTarget(object target)
        {
            callbackTargetChanges.Enqueue(new CallbackTargetChange(target, true));
        }

        /// <summary>
        /// Unregisters an object from callbacks for the implemented callback-interfaces.
        /// </summary>
        /// <remarks>
        /// Adding and removing callback targets is queued to not mess with callbacks in execution.
        /// Internally, this means that the addition/removal is done before the LoadBalancingClient
        /// calls the next callbacks. This detail should not affect a game's workflow.
        /// 
        /// The covered callback interfaces are: IConnectionCallbacks, IMatchmakingCallbacks,
        /// ILobbyCallbacks, IInRoomCallbacks, IOnEventCallback and IWebRpcCallback.
        ///
        /// See: <a href="https://doc.photonengine.com/en-us/pun/v2/getting-started/dotnet-callbacks">.Net Callbacks</a>
        /// </remarks>
        /// <param name="target">The object that unregisters from getting callbacks.</param>
        public void RemoveCallbackTarget(object target)
        {
            callbackTargetChanges.Enqueue(new CallbackTargetChange(target, false));
        }


        /// <summary>
        /// Applies queued callback cahnges from a queue to the actual containers. Will cause exceptions if used while callbacks execute.
        /// </summary>
        /// <remarks>
        /// There is no explicit check that this is not called during callbacks, however the implemented, private logic takes care of this.
        /// </remarks>
        protected internal void UpdateCallbackTargets()
        {
            while (callbackTargetChanges.Count > 0)
            {
                CallbackTargetChange change = callbackTargetChanges.Dequeue();

                if (change.AddTarget)
                {
                    if (callbackTargets.Contains(change.Target))
                    {
                        //Debug.Log("UpdateCallbackTargets skipped adding a target, as the object is already registered. Target: " + change.Target);
                        continue;
                    }

                    callbackTargets.Add(change.Target);
                }
                else
                {
                    if (!callbackTargets.Contains(change.Target))
                    {
                        //Debug.Log("UpdateCallbackTargets skipped removing a target, as the object is not registered. Target: " + change.Target);
                        continue;
                    }

                    callbackTargets.Remove(change.Target);
                }

                UpdateCallbackTarget<IInRoomCallbacks>(change, InRoomCallbackTargets);
                UpdateCallbackTarget<IConnectionCallbacks>(change, ConnectionCallbackTargets);
                UpdateCallbackTarget<IMatchmakingCallbacks>(change, MatchMakingCallbackTargets);
                UpdateCallbackTarget<ILobbyCallbacks>(change, LobbyCallbackTargets);
                UpdateCallbackTarget<IWebRpcCallback>(change, WebRpcCallbackTargets);

                IOnEventCallback onEventCallback = change.Target as IOnEventCallback;
                if (onEventCallback != null)
                {
                    if (change.AddTarget)
                    {
                        EventReceived += onEventCallback.OnEvent;
                    }
                    else
                    {
                        EventReceived -= onEventCallback.OnEvent;
                    }
                }
            }
        }

        /// <summary>Helper method to cast and apply a target per (interface) type.</summary>
        /// <typeparam name="T">Either of the interfaces for callbacks.</typeparam>
        /// <param name="change">The queued change to apply (add or remove) some target.</param>
        /// <param name="container">The container that calls callbacks on it's list of targets.</param>
        private void UpdateCallbackTarget<T>(CallbackTargetChange change, List<T> container) where T : class
        {
            T target = change.Target as T;
            if (target != null)
            {
                if (change.AddTarget)
                {
                    container.Add(target);
                }
                else
                {
                    container.Remove(target);
                }
            }
        }
    }


    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover: Connection and Regions.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, PhotonNetwork.AddCallbackTarget(&lt;Your Component implementing this interface&gt;);
    /// To stop getting callbacks, PhotonNetwork.RemoveCallbackTarget(&lt;Your Component implementing this interface&gt;);
    ///
    /// You can also simply override MonoBehaviourPunCallbacks which will provide you with Magic Callbacks ( like Unity would call Start(), Update() on a MonoBehaviour)
    /// </remarks>
    /// \ingroup callbacks
    public interface IConnectionCallbacks
    {
        /// <summary>
        /// Called to signal that the "low level connection" got established but before the client can call operation on the server.
        /// </summary>
        /// <remarks>
        /// After the (low level transport) connection is established, the client will automatically send
        /// the Authentication operation, which needs to get a response before the client can call other operations.
        ///
        /// Your logic should wait for either: OnRegionListReceived or OnConnectedToMaster.
        ///
        /// This callback is useful to detect if the server can be reached at all (technically).
        /// Most often, it's enough to implement OnDisconnected(DisconnectCause cause) and check for the cause.
        ///
        /// This is not called for transitions from the masterserver to game servers.
        /// </remarks>
        void OnConnected();

        /// <summary>
        /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
        /// </summary>
        /// <remarks>
        /// The list of available rooms won't become available unless you join a lobby via LoadBalancingClient.OpJoinLobby.
        /// You can join rooms and create them even without being in a lobby. The default lobby is used in that case.
        /// </remarks>
        void OnConnectedToMaster();

        /// <summary>
        /// Called after disconnecting from the Photon server. It could be a failure or an explicit disconnect call
        /// </summary>
        /// <remarks>
        ///  The reason for this disconnect is provided as DisconnectCause.
        /// </remarks>
        void OnDisconnected(DisconnectCause cause);

        /// <summary>
        /// Called when the Name Server provided a list of regions for your title.
        /// </summary>
        /// <remarks>Check the RegionHandler class description, to make use of the provided values.</remarks>
        /// <param name="regionHandler">The currently used RegionHandler.</param>
        void OnRegionListReceived(RegionHandler regionHandler);


        /// <summary>
        /// Called when your Custom Authentication service responds with additional data.
        /// </summary>
        /// <remarks>
        /// Custom Authentication services can include some custom data in their response.
        /// When present, that data is made available in this callback as Dictionary.
        /// While the keys of your data have to be strings, the values can be either string or a number (in Json).
        /// You need to make extra sure, that the value type is the one you expect. Numbers become (currently) int64.
        ///
        /// Example: void OnCustomAuthenticationResponse(Dictionary&lt;string, object&gt; data) { ... }
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/realtime/current/reference/custom-authentication"/>
        void OnCustomAuthenticationResponse(Dictionary<string, object> data);

        /// <summary>
        /// Called when the custom authentication failed. Followed by disconnect!
        /// </summary>
        /// <remarks>
        /// Custom Authentication can fail due to user-input, bad tokens/secrets.
        /// If authentication is successful, this method is not called. Implement OnJoinedLobby() or OnConnectedToMaster() (as usual).
        ///
        /// During development of a game, it might also fail due to wrong configuration on the server side.
        /// In those cases, logging the debugMessage is very important.
        ///
        /// Unless you setup a custom authentication service for your app (in the [Dashboard](https://dashboard.photonengine.com)),
        /// this won't be called!
        /// </remarks>
        /// <param name="debugMessage">Contains a debug message why authentication failed. This has to be fixed during development.</param>
        void OnCustomAuthenticationFailed(string debugMessage);

    }


    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover the Lobby.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, PhotonNetwork.AddCallbackTarget(&lt;Your Component implementing this interface&gt;);
    /// To stop getting callbacks, PhotonNetwork.RemoveCallbackTarget(&lt;Your Component implementing this interface&gt;);
    ///
    /// You can also simply override MonoBehaviourPunCallbacks which will provide you with Magic Callbacks ( like Unity would call Start(), Update() on a MonoBehaviour)
    /// </remarks>
    /// \ingroup callbacks
    public interface ILobbyCallbacks
    {

        /// <summary>
        /// Called on entering a lobby on the Master Server. The actual room-list updates will call OnRoomListUpdate.
        /// </summary>
        /// <remarks>
        /// While in the lobby, the roomlist is automatically updated in fixed intervals (which you can't modify in the public cloud).
        /// The room list gets available via OnRoomListUpdate.
        /// </remarks>
        void OnJoinedLobby();

        /// <summary>
        /// Called after leaving a lobby.
        /// </summary>
        /// <remarks>
        /// When you leave a lobby, [OpCreateRoom](@ref OpCreateRoom) and [OpJoinRandomRoom](@ref OpJoinRandomRoom)
        /// automatically refer to the default lobby.
        /// </remarks>
        void OnLeftLobby();

        /// <summary>
        /// Called for any update of the room-listing while in a lobby (InLobby) on the Master Server.
        /// </summary>
        /// <remarks>
        /// Each item is a RoomInfo which might include custom properties (provided you defined those as lobby-listed when creating a room).
        /// Not all types of lobbies provide a listing of rooms to the client. Some are silent and specialized for server-side matchmaking.
        /// </remarks>
        void OnRoomListUpdate(List<RoomInfo> roomList);

        /// <summary>
        /// Called when the Master Server sent an update for the Lobby Statistics, updating PhotonNetwork.LobbyStatistics.
        /// </summary>
        /// <remarks>
        /// This callback has two preconditions:
        /// EnableLobbyStatistics must be set to true, before this client connects.
        /// And the client has to be connected to the Master Server, which is providing the info about lobbies.
        /// </remarks>
        void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics);
    }


    /// <summary>
    /// Collection of "organizational" callbacks for the Realtime Api to cover Matchmaking.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, PhotonNetwork.AddCallbackTarget(&lt;Your Component implementing this interface&gt;);
    /// To stop getting callbacks, PhotonNetwork.RemoveCallbackTarget(&lt;Your Component implementing this interface&gt;);
    ///
    /// You can also simply override MonoBehaviourPunCallbacks which will provide you with Magic Callbacks ( like Unity would call Start(), Update() on a MonoBehaviour)
    /// </remarks>
    /// \ingroup callbacks
    public interface IMatchmakingCallbacks
    {

        /// <summary>
        /// Called when the server sent the response to a FindFriends request.
        /// </summary>
        /// <remarks>
        /// After calling OpFindFriends, the Master Server will cache the friend list and send updates to the friend
        /// list. The friends includes the name, userId, online state and the room (if any) for each requested user/friend.
        ///
        /// Use the friendList to update your UI and store it, if the UI should highlight changes.
        /// </remarks>
        void OnFriendListUpdate(List<FriendInfo> friendList);

        /// <summary>
        /// Called when this client created a room and entered it. OnJoinedRoom() will be called as well.
        /// </summary>
        /// <remarks>
        /// This callback is only called on the client which created a room (see OpCreateRoom).
        ///
        /// As any client might close (or drop connection) anytime, there is a chance that the
        /// creator of a room does not execute OnCreatedRoom.
        ///
        /// If you need specific room properties or a "start signal", implement OnMasterClientSwitched()
        /// and make each new MasterClient check the room's state.
        /// </remarks>
        void OnCreatedRoom();

        /// <summary>
        /// Called when the server couldn't create a room (OpCreateRoom failed).
        /// </summary>
        /// <remarks>
        /// Creating a room may fail for various reasons. Most often, the room already exists (roomname in use) or
        /// the RoomOptions clash and it's impossible to create the room.
        ///
        /// When creating a room fails on a Game Server:
        /// The client will cache the failure internally and returns to the Master Server before it calls the fail-callback.
        /// This way, the client is ready to find/create a room at the moment of the callback.
        /// In this case, the client skips calling OnConnectedToMaster but returning to the Master Server will still call OnConnected.
        /// Treat callbacks of OnConnected as pure information that the client could connect.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnCreateRoomFailed(short returnCode, string message);

        /// <summary>
        /// Called when the LoadBalancingClient entered a room, no matter if this client created it or simply joined.
        /// </summary>
        /// <remarks>
        /// When this is called, you can access the existing players in Room.Players, their custom properties and Room.CustomProperties.
        ///
        /// In this callback, you could create player objects. For example in Unity, instantiate a prefab for the player.
        ///
        /// If you want a match to be started "actively", enable the user to signal "ready" (using OpRaiseEvent or a Custom Property).
        /// </remarks>
        void OnJoinedRoom();

        /// <summary>
        /// Called when a previous OpJoinRoom call failed on the server.
        /// </summary>
        /// <remarks>
        /// Joining a room may fail for various reasons. Most often, the room is full or does not exist anymore
        /// (due to someone else being faster or closing the room).
        ///
        /// When joining a room fails on a Game Server:
        /// The client will cache the failure internally and returns to the Master Server before it calls the fail-callback.
        /// This way, the client is ready to find/create a room at the moment of the callback.
        /// In this case, the client skips calling OnConnectedToMaster but returning to the Master Server will still call OnConnected.
        /// Treat callbacks of OnConnected as pure information that the client could connect.
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnJoinRoomFailed(short returnCode, string message);

        /// <summary>
        /// Called when a previous OpJoinRandom call failed on the server.
        /// </summary>
        /// <remarks>
        /// The most common causes are that a room is full or does not exist (due to someone else being faster or closing the room).
        ///
        /// This operation is only ever sent to the Master Server. Once a room is found by the Master Server, the client will
        /// head off to the designated Game Server and use the operation Join on the Game Server.
        ///
        /// When using multiple lobbies (via OpJoinLobby or a TypedLobby parameter), another lobby might have more/fitting rooms.<br/>
        /// </remarks>
        /// <param name="returnCode">Operation ReturnCode from the server.</param>
        /// <param name="message">Debug message for the error.</param>
        void OnJoinRandomFailed(short returnCode, string message);

        /// <summary>
        /// Called when the local user/client left a room, so the game's logic can clean up it's internal state.
        /// </summary>
        /// <remarks>
        /// When leaving a room, the LoadBalancingClient will disconnect the Game Server and connect to the Master Server.
        /// This wraps up multiple internal actions.
        ///
        /// Wait for the callback OnConnectedToMaster, before you use lobbies and join or create rooms.
        /// </remarks>
        void OnLeftRoom();
    }

    /// <summary>
    /// Collection of "in room" callbacks for the Realtime Api to cover: Players entering or leaving, property updates and Master Client switching.
    /// </summary>
    /// <remarks>
    /// The callback to get events is in a separate interface: IOnEventCallback.
    ///
    /// To register for callbacks, PhotonNetwork.AddCallbackTarget(&lt;Your Component implementing this interface&gt;);
    /// To stop getting callbacks, PhotonNetwork.RemoveCallbackTarget(&lt;Your Component implementing this interface&gt;);
    ///
    /// You can also simply override MonoBehaviourPunCallbacks which will provide you with Magic Callbacks ( like Unity would call Start(), Update() on a MonoBehaviour)
    /// </remarks>
    /// \ingroup callbacks
    public interface IInRoomCallbacks
    {
        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <remarks>
        /// If your game starts with a certain number of players, this callback can be useful to check the
        /// Room.playerCount and find out if you can start.
        /// </remarks>
        void OnPlayerEnteredRoom(Player newPlayer);

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <remarks>
        /// If another player leaves the room or if the server detects a lost connection, this callback will
        /// be used to notify your game logic.
        ///
        /// Depending on the room's setup, players may become inactive, which means they may return and retake
        /// their spot in the room. In such cases, the Player stays in the Room.Players dictionary.
        ///
        /// If the player is not just inactive, it gets removed from the Room.Players dictionary, before
        /// the callback is called.
        /// </remarks>
        void OnPlayerLeftRoom(Player otherPlayer);


        /// <summary>
        /// Called when a room's custom properties changed. The propertiesThatChanged contains all that was set via Room.SetCustomProperties.
        /// </summary>
        /// <remarks>
        /// Since v1.25 this method has one parameter: Hashtable propertiesThatChanged.<br/>
        /// Changing properties must be done by Room.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="propertiesThatChanged"></param>
        void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged);

        /// <summary>
        /// Called when custom player-properties are changed. Player and the changed properties are passed as object[].
        /// </summary>
        /// <remarks>
        /// Changing properties must be done by Player.SetCustomProperties, which causes this callback locally, too.
        /// </remarks>
        /// <param name="targetPlayer">Contains Player that changed.</param>
        /// <param name="changedProps">Contains the properties that changed.</param>
        void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps);

        /// <summary>
        /// Called after switching to a new MasterClient when the current one leaves.
        /// </summary>
        /// <remarks>
        /// This is not called when this client enters a room.
        /// The former MasterClient is still in the player list when this method get called.
        /// </remarks>
        void OnMasterClientSwitched(Player newMasterClient);
    }


    /// <summary>
    /// Event callback for the Realtime Api. Covers events from the server and those sent by clients via OpRaiseEvent.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, register the instance via: LoadBalancingClient.EventReceived += instance.
    /// To stop getting callbacks, remove the instance via: -=.
    /// </remarks>
    /// \ingroup callbacks
    public interface IOnEventCallback
    {
        /// <summary>Called for any incoming events.</summary>
        /// <remarks>
        /// To receive events, implement IOnEventCallback in any class and register it via AddCallbackTarget
        /// (either in LoadBalancingClient or PhotonNetwork).
        ///
        /// With the EventData.Sender you can look up the Player who sent the event.
        ///
        /// It is best practice to assign an eventCode for each different type of content and action, so the Code
        /// will be essential to read the incoming events.
        /// </remarks>
        void OnEvent(EventData photonEvent);
    }

    /// <summary>
    /// Interface for "WebRpc" callbacks for the Realtime Api. Currently includes only responses for Web RPCs.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface must be registered to get callbacks for various situations.
    ///
    /// To register for callbacks, use the LoadBalancingClient.WebRpcCallbackTargets and Add() the instance.
    /// To stop getting callbacks, Remove() the instance.
    /// </remarks>
    /// \ingroup callbacks
    public interface IWebRpcCallback
    {
        /// <summary>
        /// Called by PUN when the response to a WebRPC is available. See PhotonNetwork.WebRPC.
        /// </summary>
        /// <remarks>
        /// Important: The response.ReturnCode is 0 if Photon was able to reach your web-service.<br/>
        /// The content of the response is what your web-service sent. You can create a WebRpcResponse from it.<br/>
        /// Example: WebRpcResponse webResponse = new WebRpcResponse(operationResponse);<br/>
        ///
        /// Please note: Class OperationResponse is in a namespace which needs to be "used":<br/>
        /// using ExitGames.Client.Photon;  // includes OperationResponse (and other classes)
        /// </remarks>
        void OnWebRpcResponse(OperationResponse response);
    }


    /// <summary>
    /// Container type for callbacks defined by IConnectionCallbacks. See LoadBalancingCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    public class ConnectionCallbacksContainer : List<IConnectionCallbacks>, IConnectionCallbacks
    {
        private readonly LoadBalancingClient client;

        public ConnectionCallbacksContainer(LoadBalancingClient client)
        {
            this.client = client;
        }

        public void OnConnected()
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnConnected();
            }
        }

        public void OnConnectedToMaster()
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnConnectedToMaster();
            }
        }

        public void OnRegionListReceived(RegionHandler regionHandler)
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnRegionListReceived(regionHandler);
            }
        }

        public void OnDisconnected(DisconnectCause cause)
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnDisconnected(cause);
            }
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnCustomAuthenticationResponse(data);
            }
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
            client.UpdateCallbackTargets();

            foreach (IConnectionCallbacks target in this)
            {
                target.OnCustomAuthenticationFailed(debugMessage);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by IMatchmakingCallbacks. See MatchMakingCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    public class MatchMakingCallbacksContainer : List<IMatchmakingCallbacks>, IMatchmakingCallbacks
    {
        private readonly LoadBalancingClient client;

        public MatchMakingCallbacksContainer(LoadBalancingClient client)
        {
            this.client = client;
        }

        public void OnCreatedRoom()
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnCreatedRoom();
            }
        }

        public void OnJoinedRoom()
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinedRoom();
            }
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnCreateRoomFailed(returnCode, message);
            }
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinRandomFailed(returnCode, message);
            }
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnJoinRoomFailed(returnCode, message);
            }
        }

        public void OnLeftRoom()
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnLeftRoom();
            }
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            client.UpdateCallbackTargets();

            foreach (IMatchmakingCallbacks target in this)
            {
                target.OnFriendListUpdate(friendList);
            }
        }
    }


    /// <summary>
    /// Container type for callbacks defined by IInRoomCallbacks. See InRoomCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class InRoomCallbacksContainer : List<IInRoomCallbacks>, IInRoomCallbacks
    {
        private readonly LoadBalancingClient client;

        public InRoomCallbacksContainer(LoadBalancingClient client)
        {
            this.client = client;
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerEnteredRoom(newPlayer);
            }
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerLeftRoom(otherPlayer);
            }
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnRoomPropertiesUpdate(propertiesThatChanged);
            }
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProp)
        {
            client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnPlayerPropertiesUpdate(targetPlayer, changedProp);
            }
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            client.UpdateCallbackTargets();

            foreach (IInRoomCallbacks target in this)
            {
                target.OnMasterClientSwitched(newMasterClient);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by ILobbyCallbacks. See LobbyCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class LobbyCallbacksContainer : List<ILobbyCallbacks>, ILobbyCallbacks
    {
        private readonly LoadBalancingClient client;

        public LobbyCallbacksContainer(LoadBalancingClient client)
        {
            this.client = client;
        }

        public void OnJoinedLobby()
        {
            client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnJoinedLobby();
            }
        }

        public void OnLeftLobby()
        {
            client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnLeftLobby();
            }
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnRoomListUpdate(roomList);
            }
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            client.UpdateCallbackTargets();

            foreach (ILobbyCallbacks target in this)
            {
                target.OnLobbyStatisticsUpdate(lobbyStatistics);
            }
        }
    }

    /// <summary>
    /// Container type for callbacks defined by IWebRpcCallback. See WebRpcCallbackTargets.
    /// </summary>
    /// <remarks>
    /// While the interfaces of callbacks wrap up the methods that will be called,
    /// the container classes implement a simple way to call a method on all registered objects.
    /// </remarks>
    internal class WebRpcCallbacksContainer : List<IWebRpcCallback>, IWebRpcCallback
    {
        private readonly LoadBalancingClient client;

        public WebRpcCallbacksContainer(LoadBalancingClient client)
        {
            this.client = client;
        }

        public void OnWebRpcResponse(OperationResponse response)
        {
            client.UpdateCallbackTargets();

            foreach (IWebRpcCallback target in this)
            {
                target.OnWebRpcResponse(response);
            }
        }
    }
}