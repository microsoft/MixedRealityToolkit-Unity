// -----------------------------------------------------------------------
// <copyright file="LoadBalancingClient.cs" company="Exit Games GmbH">
//   Loadbalancing Framework for Photon - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Provides the operations and a state for games using the
//   Photon LoadBalancing server.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

#if UNITY_4_7 || UNITY_5 || UNITY_5_0 || UNITY_5_1 || UNITY_6_0
#define UNITY
#endif

namespace ExitGames.Client.Photon.LoadBalancing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    //#if UNITY_EDITOR || NETFX_CORE
    using Hashtable = ExitGames.Client.Photon.Hashtable;
    using SupportClass = ExitGames.Client.Photon.SupportClass;
    //#endif


    #region Enums

    /// <summary>Possible states for a LoadBalancingClient.</summary>
    public enum ClientState
    {
        /// <summary>Peer is created but not used yet.</summary>
        Uninitialized,

        /// <summary>Currently not used.</summary>
        Queued,

        /// <summary>Usually when Authenticated, the client will join a game or the lobby (if AutoJoinLobby is true).</summary>
        Authenticated,

        /// <summary>Connected to master and joined lobby. Display room list and join/create rooms at will.</summary>
        JoinedLobby,

        /// <summary>Transition from master to game server.</summary>
        DisconnectingFromMasterserver,

        /// <summary>Transition to gameserver (client will authenticate and join/create game).</summary>
        ConnectingToGameserver,

        /// <summary>Connected to gameserver (going to auth and join game).</summary>
        ConnectedToGameserver,

        /// <summary>Joining game on gameserver.</summary>
        Joining,

        /// <summary>The client arrived inside a room. CurrentRoom and Players are known. Send events with OpRaiseEvent.</summary>
        Joined,

        /// <summary>Currently not used. Instead of OpLeave, the client disconnects from a server (which also triggers a leave immediately).</summary>
        Leaving,

        /// <summary>Transition from gameserver to master (after leaving a room/game).</summary>
        DisconnectingFromGameserver,

        /// <summary>Connecting to master (includes connect, authenticate and joining the lobby)</summary>
        ConnectingToMasterserver,

        /// <summary>Currently not used.</summary>
        QueuedComingFromGameserver,

        /// <summary>The client disconnects (from any server).</summary>
        Disconnecting,

        /// <summary>The client is no longer connected (to any server). Connect to master to go on.</summary>
        Disconnected,

        /// <summary>Connected to master server.</summary>
        ConnectedToMaster,

        /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
        ConnectingToNameServer,

        /// <summary>Client is connected to the NameServer and established enctryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
        ConnectedToNameServer,

        /// <summary>Clients disconnects (specifically) from the NameServer to reconnect to the master server.</summary>
        DisconnectingFromNameServer,

        /// <summary>Client authenticates itself with the server. On the Photon Cloud this sends the AppId of your game. Used with Master Server and Game Server.</summary>
        Authenticating
    }

    /// <summary>Ways a room can be created or joined.</summary>
    public enum JoinType
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

    /// <summary>Enumaration of causes for Disconnects (used in LoadBalancingClient.DisconnectedCause).</summary>
    /// <remarks>Read the individual descriptions to find out what to do about this type of disconnect.</remarks>
    public enum DisconnectCause
    {
        /// <summary>No error was tracked.</summary>
        None,
        /// <summary>OnStatusChanged: The CCUs count of your Photon Server License is exausted (temporarily).</summary>
        DisconnectByServerUserLimit,
        /// <summary>OnStatusChanged: The server is not available or the address is wrong. Make sure the port is provided and the server is up.</summary>
        ExceptionOnConnect,
        /// <summary>OnStatusChanged: The server disconnected this client. Most likely the server's send buffer is full (receiving too much from other clients).</summary>
        DisconnectByServer,
        /// <summary>OnStatusChanged: This client detected that the server's responses are not received in due time. Maybe you send / receive too much?</summary>
        TimeoutDisconnect,
        /// <summary>OnStatusChanged: Some internal exception caused the socket code to fail. Contact Exit Games.</summary>
        Exception,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid AppId. Update your subscription or contact Exit Games.</summary>
        InvalidAuthentication,
        /// <summary>OnOperationResponse: Authenticate (temporarily) failed when using a Photon Cloud subscription without CCU Burst. Update your subscription.</summary>
        MaxCcuReached,
        /// <summary>OnOperationResponse: Authenticate when the app's Photon Cloud subscription is locked to some (other) region(s). Update your subscription or master server address.</summary>
        InvalidRegion,
        /// <summary>OnOperationResponse: Operation that's (currently) not available for this client (not authorized usually). Only tracked for op Authenticate.</summary>
        OperationNotAllowedInCurrentState,
        /// <summary>OnOperationResponse: Authenticate in the Photon Cloud with invalid client values or custom authentication setup in Cloud Dashboard.</summary>
        CustomAuthenticationFailed,
        /// <summary>OnStatusChanged: The server disconnected this client from within the room's logic (the C# code).</summary>
        DisconnectByServerLogic,
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
        /// With this encryption mode for UDP, the connection gets setup and all further datagrams get encrypted almost entirely. On-demand message encryption (like in PayloadEncryption) is skipped.
        /// </summary>
        DatagramEncryption = 10,
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
        public LoadBalancingPeer loadBalancingPeer;

        /// <summary>The version of your client. A new version also creates a new "virtual app" to separate players from older client versions.</summary>
        public string AppVersion { get; set; }

        /// <summary>The AppID as assigned from the Photon Cloud. If you host yourself, this is the "regular" Photon Server Application Name (most likely: "LoadBalancing").</summary>
        public string AppId { get; set; }

        /// <summary>A user's authentication values for authentication in Photon.</summary>
        /// <remarks>Set this property or pass AuthenticationValues by Connect(..., authValues).</remarks>
        public AuthenticationValues AuthValues { get; set; }


        /// <summary>Enables the new Authentication workflow.</summary>
        public AuthModeOption AuthMode = AuthModeOption.Auth;

        /// <summary>Defines how messages get encrypted.</summary>
        public EncryptionMode EncryptionMode = EncryptionMode.PayloadEncryption;

        /// <summary>The protocol which will be used on Master- and Gameserver.</summary>
        /// <remarks>
        /// When using AuthOnceWss, the client uses a wss-connection on the Nameserver but another protocol on the other servers.
        /// As the Nameserver sends an address, which is different per protocol, it needs to know the expected protocol.
        /// </remarks>
        public ConnectionProtocol ExpectedProtocol = ConnectionProtocol.Udp;


        ///<summary>Simplifies getting the token for connect/init requests, if this feature is enabled.</summary>
        private string TokenForInit
        {
            get
            {
                if (this.AuthMode == AuthModeOption.Auth)
                {
                    return null;
                }
                return (this.AuthValues != null) ? this.AuthValues.Token : null;
            }
        }


        /// <summary>True if this client uses a NameServer to get the Master Server address.</summary>
        public bool IsUsingNameServer { get; private set; }

        /// <summary>Name Server Host Name for Photon Cloud. Without port and without any prefix.</summary>
        public string NameServerHost = "ns.exitgames.com";

        /// <summary>Name Server for HTTP connections to the Photon Cloud. Includes prefix and port.</summary>
        public string NameServerHttp = "http://ns.exitgames.com:80/photon/n";

        /// <summary>Name Server port per protocol (the UDP port is different than TCP, etc).</summary>
        private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { { ConnectionProtocol.Udp, 5058 }, { ConnectionProtocol.Tcp, 4533 }, { ConnectionProtocol.WebSocket, 9093 }, { ConnectionProtocol.WebSocketSecure, 19093 } }; //, { ConnectionProtocol.RHttp, 6063 } };

        /// <summary>Name Server Address for Photon Cloud (based on current protocol). You can use the default values and usually won't have to set this value.</summary>
        public string NameServerAddress { get { return this.GetNameServerAddress(); } }

        /// <summary>The currently used server address (if any). The type of server is define by Server property.</summary>
        public string CurrentServerAddress { get { return this.loadBalancingPeer.ServerAddress; } }


        /// <summary>Your Master Server address. In PhotonCloud, call ConnectToRegionMaster() to find your Master Server.</summary>
        /// <remarks>
        /// In the Photon Cloud, explicit definition of a Master Server Address is not best practice.
        /// The Photon Cloud has a "Name Server" which redirects clients to a specific Master Server (per Region and AppId).
        /// </remarks>
        public string MasterServerAddress { get; protected internal set; }

        /// <summary>The game server's address for a particular room. In use temporarily, as assigned by master.</summary>
        public string GameServerAddress { get; protected internal set; }


        /// <summary>The server this client is currently connected or connecting to.</summary>
        /// <remarks>
        /// Each server (NameServer, MasterServer, GameServer) allow some operations and reject others.
        /// </remarks>
        public ServerConnection Server { get; private set; }

        /// <summary>Backing field for property.</summary>
        private ClientState state = ClientState.Uninitialized;

        /// <summary>Current state this client is in. Careful: several states are "transitions" that lead to other states.</summary>
        public ClientState State
        {
            get
            {
                return this.state;
            }

            protected internal set
            {
                this.state = value;
                if (OnStateChangeAction != null) OnStateChangeAction(this.state);
            }
        }

        /// <summary>Returns if this client is currently connected or connecting to some type of server.</summary>
        /// <remarks>This is even true while switching servers. Use IsConnectedAndReady to check only for those states that enable you to send Operations.</remarks>
        public bool IsConnected { get { return this.loadBalancingPeer != null && this.State != ClientState.Uninitialized && this.State != ClientState.Disconnected; } }


        /// <summary>
        /// A refined version of IsConnected which is true only if your connection to the server is ready to accept operations.
        /// </summary>
        /// <remarks>
        /// Which operations are available, depends on the Server. For example, the NameServer allows OpGetRegions which is not available anywhere else.
        /// The MasterServer does not allow you to send events (OpRaiseEvent) and on the GameServer you are unable to join a lobby (OpJoinLobby).
        /// Check which server you are on with PhotonNetwork.Server.
        /// </remarks>
        public bool IsConnectedAndReady
        {
            get
            {
                if (this.loadBalancingPeer == null)
                {
                    return false;
                }

                switch (this.State)
                {
                    case ClientState.Uninitialized:
                    case ClientState.Disconnected:
                    case ClientState.Disconnecting:
                    case ClientState.Authenticating:
                    case ClientState.ConnectingToGameserver:
                    case ClientState.ConnectingToMasterserver:
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
        public event Action<ClientState> OnStateChangeAction;

        /// <summary>Register a method to be called when an event got dispatched. Gets called at the end of OnEvent().</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnEvent().
        ///
        /// Note that OnEvent is executing before your Action is called.
        /// That means for example: Joining players will already be in the player list but leaving
        /// players will already be removed from the room.
        /// </remarks>
        public event Action<EventData> OnEventAction;

        /// <summary>Register a method to be called when this client's ClientState gets set.</summary>
        /// <remarks>
        /// This is an alternative to extending LoadBalancingClient to override OnOperationResponse().
        ///
        /// Note that OnOperationResponse gets executed before your Action is called.
        /// That means for example: The OpJoinLobby response already set the state to "JoinedLobby"
        /// and the response to OpLeave already triggered the Disconnect before this is called.
        /// </remarks>
        public event Action<OperationResponse> OnOpResponseAction;


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
        private bool inLobby;

        /// <summary>The lobby this client currently uses.</summary>
        public TypedLobby CurrentLobby { get; protected internal set; }

        /// <summary>Backing field for property.</summary>
        private bool autoJoinLobby = true;

        /// <summary>If your client should join random games, you can skip joining the lobby. Call OpJoinRandomRoom and create a room if that fails.</summary>
        public bool AutoJoinLobby
        {
            get
            {
                return this.autoJoinLobby;
            }

            set
            {
                this.autoJoinLobby = value;
            }
        }

        /// <summary>
        /// If set to true, the Master Server will report the list of used lobbies to the client. This sets and updates LobbyStatistics.
        /// </summary>
        /// <remarks>
        /// Lobby Statistics can be useful if a game uses multiple lobbies and you want
        /// to show activity of each to players.
        ///
        /// LobbyStatistics are updated when you connect to the Master Server.
        /// </remarks>
        public bool EnableLobbyStatistics;

        /// <summary>Internal lobby stats cache, used by LobbyStatistics.</summary>
        private List<TypedLobbyInfo> lobbyStatistics = new List<TypedLobbyInfo>();

        /// <summary>
        /// If RequestLobbyStatistics is true, this provides a list of used lobbies (their name, type, room- and player-count) of this application, while on the Master Server.
        /// </summary>
        /// <remarks>
        /// If turned on, the Master Server will provide information about active lobbies for this application.
        ///
        /// Lobby Statistics can be useful if a game uses multiple lobbies and you want
        /// to show activity of each to players. Per lobby, you get: name, type, room- and player-count.
        ///
        /// Lobby Statistics are not turned on by default.
        /// Enable them by setting RequestLobbyStatistics to true before you connect.
        ///
        /// LobbyStatistics are updated when you connect to the Master Server.
        /// You can check in OnEvent if EventCode.LobbyStats arrived. This the updates.
        /// </remarks>
        public List<TypedLobbyInfo> LobbyStatistics
        {
            get { return this.lobbyStatistics; }
            private set { this.lobbyStatistics = value; }
        }


        /// <summary>
        /// The nickname of the player (synced with others). Same as client.LocalPlayer.NickName.
        /// </summary>
        public string NickName
        {
            get
            {
                return this.LocalPlayer.NickName;
            }

            set
            {
                if (this.LocalPlayer == null)
                {
                    return;
                }

                this.LocalPlayer.NickName = value;
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
        public string UserId {
            get
            {
                if (this.AuthValues != null)
                {
                    return this.AuthValues.UserId;
                }
                return null;
            }
            set
            {
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                }
                this.AuthValues.UserId = value;
            }
        }

        /// <summary>This "list" is populated while being in the lobby of the Master. It contains RoomInfo per roomName (keys).</summary>
        public Dictionary<string, RoomInfo> RoomInfoList = new Dictionary<string, RoomInfo>();

        /// <summary>The current room this client is connected to (null if none available).</summary>
        public Room CurrentRoom;

        /// <summary>Private field for LocalPlayer.</summary>
        private Player localPlayer;

        /// <summary>The local player is never null but not valid unless the client is in a room, too. The ID will be -1 outside of rooms.</summary>
        public Player LocalPlayer
        {
            get
            {
                if (localPlayer == null)
                {
                    this.localPlayer = this.CreatePlayer(string.Empty, -1, true, null);
                }

                return this.localPlayer;
            }

            set
            {
                this.localPlayer = value;
            }
        }

        /// <summary>Statistic value available on master server: Players on master (looking for games).</summary>
        public int PlayersOnMasterCount { get; internal set; }

        /// <summary>Statistic value available on master server: Players in rooms (playing).</summary>
        public int PlayersInRoomsCount { get; internal set; }

        /// <summary>Statistic value available on master server: Rooms currently created.</summary>
        public int RoomsCount { get; internal set; }

        /// <summary>Internally used to decide if a room must be created or joined on game server.</summary>
        private JoinType lastJoinType;

        private EnterRoomParams enterRoomParamsCache;

        /// <summary>Internally used to trigger OpAuthenticate when encryption was established after a connect.</summary>
        private bool didAuthenticate;

        /// <summary>
        /// List of friends, their online status and the room they are in. Null until initialized by OpFindFriends response.
        /// </summary>
        /// <remarks>
        /// Do not modify this list! It's internally handled by OpFindFriends and meant as read-only.
        /// The value of FriendListAge gives you a hint how old the data is. Don't get this list more often than useful (> 10 seconds).
        /// In best case, keep the list you fetch really short. You could (e.g.) get the full list only once, then request a few updates
        /// only for friends who are online. After a while (e.g. 1 minute), you can get the full list again.
        /// </remarks>
        public List<FriendInfo> FriendList { get; private set; }

        /// <summary>Contains the list of names of friends to look up their state on the server.</summary>
        private string[] friendListRequested;

        /// <summary>
        /// Age of friend list info (in milliseconds). It's 0 until a friend list is fetched.
        /// </summary>
        public int FriendListAge { get { return (this.isFetchingFriendList || this.friendListTimestamp == 0) ? 0 : Environment.TickCount - this.friendListTimestamp; } }

        /// <summary>Private timestamp (in ms) of the last friendlist update.</summary>
        private int friendListTimestamp;

        /// <summary>Internal flag to know if the client currently fetches a friend list.</summary>
        private bool isFetchingFriendList;

        /// <summary>Internally used to check if a "Secret" is available to use. Sent by Photon Cloud servers, it simplifies authentication when switching servers.</summary>
        protected bool IsAuthorizeSecretAvailable
        {
            get
            {
                return this.AuthValues != null && !string.IsNullOrEmpty(this.AuthValues.Token);
            }
        }

        /// <summary>A list of region names for the Photon Cloud. Set by the result of OpGetRegions().</summary>
        /// <remarks>Put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.</remarks>
        public string[] AvailableRegions { get; private set; }

        /// <summary>A list of region server (IP addresses with port) for the Photon Cloud. Set by the result of OpGetRegions().</summary>
        /// <remarks>Put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.</remarks>
        public string[] AvailableRegionsServers { get; private set; }

        /// <summary>The cloud region this client connects to. Set by ConnectToRegionMaster(). Not set if you don't use a NameServer!</summary>
        public string CloudRegion { get; private set; }


        /// <summary>Creates a LoadBalancingClient with UDP protocol or the one specified.</summary>
        /// <param name="protocol">Specifies the network protocol to use for connections.</param>
        public LoadBalancingClient(ConnectionProtocol protocol = ConnectionProtocol.Udp)
        {
            this.loadBalancingPeer = new LoadBalancingPeer(this, protocol);
        }


        /// <summary>Creates a LoadBalancingClient, setting various values needed before connecting.</summary>
        /// <param name="masterAddress">The Master Server's address to connect to. Used in Connect.</param>
        /// <param name="appId">The AppId of this title. Needed for the Photon Cloud. Find it in the Dashboard.</param>
        /// <param name="gameVersion">A version for this client/build. In the Photon Cloud, players are separated by AppId, GameVersion and Region.</param>
        /// <param name="protocol">Specifies the network protocol to use for connections.</param>
        public LoadBalancingClient(string masterAddress, string appId, string gameVersion, ConnectionProtocol protocol = ConnectionProtocol.Udp) : this(protocol)
        {
            this.MasterServerAddress = masterAddress;
            this.AppId = appId;
            this.AppVersion = gameVersion;
        }

        /// <summary>
        /// Gets the NameServer Address (with prefix and port), based on the set protocol (this.loadBalancingPeer.UsedProtocol).
        /// </summary>
        /// <returns>NameServer Address (with prefix and port).</returns>
        private string GetNameServerAddress()
        {
            var protocolPort = 0;
            ProtocolToNameServerPort.TryGetValue(this.loadBalancingPeer.TransportProtocol, out protocolPort);

            switch (this.loadBalancingPeer.TransportProtocol)
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
        /// Starts the "process" to connect to the master server. Relevant connection-values parameters can be set via parameters.
        /// </summary>
        /// <remarks>
        /// The process to connect includes several steps: the actual connecting, establishing encryption, authentification
        /// (of app and optionally the user) and joining a lobby (if AutoJoinLobby is true).
        ///
        /// Instead of providing all these parameters, you can also set the individual properties of a client before calling Connect().
        ///
        /// Users can connect either anonymously or use "Custom Authentication" to verify each individual player's login.
        /// Custom Authentication in Photon uses external services and communities to verify users. While the client provides a user's info,
        /// the service setup is done in the Photon Cloud Dashboard.
        /// The parameter authValues will set this.AuthValues and use them in the connect process.
        ///
        /// To connect to the Photon Cloud, a valid AppId must be provided. This is shown in the Photon Cloud Dashboard.
        /// https://cloud.photonengine.com/dashboard
        /// Connecting to the Photon Cloud might fail due to:
        /// - Network issues (OnStatusChanged() StatusCode.ExceptionOnConnect)
        /// - Region not available (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.InvalidRegion)
        /// - Subscription CCU limit reached (OnOperationResponse() for OpAuthenticate with ReturnCode == ErrorCode.MaxCcuReached)
        /// More about the connection limitations:
        /// http://doc.photonengine.com/photon-cloud/SubscriptionErrorCases/#cat-references
        /// </remarks>
        /// <param name="masterServerAddress">Set a master server address instead of using the default. Uses default if null or empty.</param>
        /// <param name="appId">Your application's name or the AppID assigned by Photon Cloud (as listed in Dashboard). Uses default if null or empty.</param>
        /// <param name="appVersion">Can be used to separate users by their client's version (useful to add features without breaking older clients). Uses default if null or empty.</param>
        /// <param name="nickName">Optional name for this player.</param>
        /// <param name="authValues">Authentication values for this user. Optional. If you provide a unique userID it is used for FindFriends.</param>
        /// <returns>If the operation could be send (can be false for bad server urls).</returns>
        public bool Connect(string masterServerAddress, string appId, string appVersion, string nickName, AuthenticationValues authValues)
        {
            if (!string.IsNullOrEmpty(masterServerAddress))
            {
                this.MasterServerAddress = masterServerAddress;
            }

            if (!string.IsNullOrEmpty(appId))
            {
                this.AppId = appId;
            }

            if (!string.IsNullOrEmpty(appVersion))
            {
                this.AppVersion = appVersion;
            }

            if (!string.IsNullOrEmpty(nickName))
            {
                this.NickName = nickName;
            }

            this.AuthValues = authValues;

            return this.Connect();
        }


        /// <summary>
        /// Starts the "process" to connect to a Master Server, using MasterServerAddress and AppId properties.
        /// </summary>
        /// <remarks>
        /// To connect to the Photon Cloud, use ConnectToRegionMaster().
        ///
        /// The process to connect includes several steps: the actual connecting, establishing encryption, authentification
        /// (of app and optionally the user) and joining a lobby (if AutoJoinLobby is true).
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
        /// More about the connection limitations:
        /// http://doc.photonengine.com/photon-cloud/SubscriptionErrorCases/#cat-references
        /// </remarks>
        public virtual bool Connect()
        {
            this.DisconnectedCause = DisconnectCause.None;


            if (this.loadBalancingPeer.Connect(this.MasterServerAddress, this.AppId, this.TokenForInit))
            {
                this.State = ClientState.ConnectingToMasterserver;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Connects to the NameServer for Photon Cloud, where a region and server list can be obtained.
        /// </summary>
        /// <see cref="OpGetRegions"/>
        /// <returns>If the workflow was started or failed right away.</returns>
        public bool ConnectToNameServer()
        {
            this.IsUsingNameServer = true;
            this.CloudRegion = null;

            if (this.AuthMode == AuthModeOption.AuthOnceWss)
            {
                this.ExpectedProtocol = this.loadBalancingPeer.TransportProtocol;
                this.loadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }

            if (!this.loadBalancingPeer.Connect(NameServerAddress, "NameServer", this.TokenForInit))
            {
                return false;
            }

            this.State = ClientState.ConnectingToNameServer;
            return true;
        }


        /// <summary>
        /// Connects you to a specific region's Master Server, using the Name Server to find the IP.
        /// </summary>
        /// <returns>If the operation could be sent. If false, no operation was sent.</returns>
        public bool ConnectToRegionMaster(string region)
        {
            this.IsUsingNameServer = true;

            if (this.State == ClientState.ConnectedToNameServer)
            {
                this.CloudRegion = region;
                return this.CallAuthenticate();
            }

            this.loadBalancingPeer.Disconnect();
            this.CloudRegion = region;

            if (this.AuthMode == AuthModeOption.AuthOnceWss)
            {
                this.ExpectedProtocol = this.loadBalancingPeer.TransportProtocol;
                this.loadBalancingPeer.TransportProtocol = ConnectionProtocol.WebSocketSecure;
            }

            if (!this.loadBalancingPeer.Connect(this.NameServerAddress, "NameServer", null))
            {
                return false;
            }

            this.State = ClientState.ConnectingToNameServer;
            return true;
        }


        /// <summary>Disconnects this client from any server and sets this.State if the connection is successfuly closed.</summary>
        public void Disconnect()
        {
            if (this.State != ClientState.Disconnected)
            {
                this.State = ClientState.Disconnecting;
                this.loadBalancingPeer.Disconnect();

                //// we can set this high-level state if the low-level (connection)state is "disconnected"
                //if (this.loadBalancingPeer.PeerState == PeerStateValue.Disconnected || this.loadBalancingPeer.PeerState == PeerStateValue.InitializingApplication)
                //{
                //    this.State = ClientState.Disconnected;
                //}
            }
        }


        private bool CallAuthenticate()
        {
            if (this.AuthMode == AuthModeOption.Auth)
            {
                return this.loadBalancingPeer.OpAuthenticate(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, (this.EnableLobbyStatistics && this.Server == ServerConnection.MasterServer));
            }
            else
            {
                return this.loadBalancingPeer.OpAuthenticateOnce(this.AppId, this.AppVersion, this.AuthValues, this.CloudRegion, this.EncryptionMode, this.ExpectedProtocol);
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
        /// Call this method regularly (2..20 times a second).
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
            if (this.loadBalancingPeer != null)
            {
                this.loadBalancingPeer.Service();
            }
        }


        /// <summary>
        /// Private Disconnect variant that sets the state, too.
        /// </summary>
        private void DisconnectToReconnect()
        {
            switch (this.Server)
            {
                case ServerConnection.NameServer:
                    this.State = ClientState.DisconnectingFromNameServer;
                    break;
                case ServerConnection.MasterServer:
                    this.State = ClientState.DisconnectingFromMasterserver;
                    break;
                case ServerConnection.GameServer:
                    this.State = ClientState.DisconnectingFromGameserver;
                    break;
            }

            this.loadBalancingPeer.Disconnect();
        }


        /// <summary>
        /// Privately used only.
        /// Starts the "process" to connect to the game server (connect before a game is joined).
        /// </summary>
        private bool ConnectToGameServer()
        {
            if (this.loadBalancingPeer.Connect(this.GameServerAddress, this.AppId, this.TokenForInit))
            {
                this.State = ClientState.ConnectingToGameserver;
                return true;
            }

            // TODO: handle error "cant connect to GS"
            return false;
        }

        /// <summary>
        /// While on the NameServer, this gets you the list of regional servers (short names and their IPs to ping them).
        /// </summary>
        /// <returns>If the operation could be sent. If false, no operation was sent (e.g. while not connected to the NameServer).</returns>
        public bool OpGetRegions()
        {
            if (this.Server != ServerConnection.NameServer)
            {
                return false;
            }

            bool sent = this.loadBalancingPeer.OpGetRegions(this.AppId);
            if (sent)
            {
                this.AvailableRegions = null;
            }

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
        /// Internal:
        /// The server response includes 2 arrays of info (each index matching a friend from the request):
        /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
        /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
        /// </remarks>
        /// <param name="friendsToFind">Array of friend's names (make sure they are unique).</param>
        /// <returns>If the operation could be sent (requires connection).</returns>
        public bool OpFindFriends(string[] friendsToFind)
        {
            if (this.loadBalancingPeer == null)
            {
                return false;
            }

            if (this.isFetchingFriendList || this.Server == ServerConnection.GameServer)
            {
                return false;   // fetching friends currently, so don't do it again (avoid changing the list while fetching friends)
            }

            this.isFetchingFriendList = true;
            this.friendListRequested = friendsToFind;
            return this.loadBalancingPeer.OpFindFriends(friendsToFind);
        }

        /// <summary>
        /// Joins the lobby on the Master Server, where you get a list of RoomInfos of currently open rooms.
        /// This is an async request which triggers a OnOperationResponse() call.
        /// </summary>
        /// <param name="lobby">The lobby join to. Use null for default lobby.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpJoinLobby(TypedLobby lobby)
        {
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }
            bool sent = this.loadBalancingPeer.OpJoinLobby(lobby);
            if (sent)
            {
                this.CurrentLobby = lobby;
            }

            return sent;
        }


        /// <summary>Opposite of joining a lobby. You don't have to explicitly leave a lobby to join another (client can be in one max, at any time).</summary>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public bool OpLeaveLobby()
        {
            return this.loadBalancingPeer.OpLeaveLobby();
        }


        /// <summary>Operation to join a random room if available. You can use room properties to filter accepted rooms.</summary>
        /// <remarks>
        /// You can use expectedCustomRoomProperties and expectedMaxPlayers as filters for accepting rooms.
        /// If you set expectedCustomRoomProperties, a room must have the exact same key values set at Custom Properties.
        /// You need to define which Custom Room Properties will be available for matchmaking when you create a room.
        /// See: OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby)
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed or not visible).
        /// Override this class and implement OnOperationResponse(OperationResponse operationResponse).
        ///
        /// OpJoinRandomRoom can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before calling this method.
        /// Alternatively, check the returned bool value.
        ///
        /// While the server is looking for a game, the State will be Joining. It's set immediately when this method sent the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and join the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When joining a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// More about matchmaking:
        /// http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">Optional. A room will only be joined, if it matches these custom properties (with string keys).</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, string[] expectedUsers = null)
        {
            return OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, TypedLobby.Default, null, expectedUsers);
        }


        /// <summary>Operation to join a random room if available. You can use room properties to filter accepted rooms.</summary>
        /// <remarks>
        /// You can use expectedCustomRoomProperties and expectedMaxPlayers as filters for accepting rooms.
        /// If you set expectedCustomRoomProperties, a room must have the exact same key values set at Custom Properties.
        /// You need to define which Custom Room Properties will be available for matchmaking when you create a room.
        /// See: OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby)
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed or not visible).
        /// Override this class and implement OnOperationResponse(OperationResponse operationResponse).
        ///
        /// OpJoinRandomRoom can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before calling this method.
        /// Alternatively, check the returned bool value.
        ///
        /// While the server is looking for a game, the State will be Joining. It's set immediately when this method sent the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and join the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When joining a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// More about matchmaking:
        /// http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">Optional. A room will only be joined, if it matches these custom properties (with string keys).</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="matchmakingMode">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode)
        {
            return this.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, matchmakingMode, TypedLobby.Default, null);
        }


        /// <summary>Operation to join a random room if available. You can use room properties to filter accepted rooms.</summary>
        /// <remarks>
        /// You can use expectedCustomRoomProperties and expectedMaxPlayers as filters for accepting rooms.
        /// If you set expectedCustomRoomProperties, a room must have the exact same key values set at Custom Properties.
        /// You need to define which Custom Room Properties will be available for matchmaking when you create a room.
        /// See: OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby)
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed or not visible).
        /// Override this class and implement OnOperationResponse(OperationResponse operationResponse).
        ///
        /// OpJoinRandomRoom can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before calling this method.
        /// Alternatively, check the returned bool value.
        ///
        /// While the server is looking for a game, the State will be Joining.
        /// It's set immediately when this method sent the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and join the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When joining a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// The parameter lobby can be null (using the defaul lobby) or a typed lobby you make up.
        /// Lobbies are created on the fly, as required by the clients. If you organize matchmaking with lobbies,
        /// keep in mind that they also fragment your matchmaking. Using more lobbies will put less rooms in each.
        ///
        /// The parameter sqlLobbyFilter can only be combined with the LobbyType.SqlLobby. In that case, it's used
        /// to define a sql-like "WHERE" clause for filtering rooms. This is useful for skill-based matchmaking e.g..
        ///
        /// More about matchmaking:
        /// http://doc.photonengine.com/en/realtime/current/reference/matchmaking-and-lobby
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">Optional. A room will only be joined, if it matches these custom properties (with string keys).</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <param name="matchmakingMode">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
        /// <param name="lobby">The lobby in which to find a room. Use null for default lobby.</param>
        /// <param name="sqlLobbyFilter">Can be used with LobbyType.SqlLobby only. This is a "where" clause of a sql statement. Use null for random game.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchmakingMode, TypedLobby lobby, string sqlLobbyFilter, string[] expectedUsers = null)
        {
            if (lobby == null)
            {
                lobby = TypedLobby.Default;
            }

            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinRandomRoom;
            this.CurrentLobby = lobby;

            this.enterRoomParamsCache = new EnterRoomParams();
            this.enterRoomParamsCache.Lobby = lobby;
            this.enterRoomParamsCache.ExpectedUsers = expectedUsers;

            OpJoinRandomRoomParams opParams = new OpJoinRandomRoomParams();
            opParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
            opParams.ExpectedMaxPlayers = expectedMaxPlayers;
            opParams.MatchingType = matchmakingMode;
            opParams.TypedLobby = lobby;
            opParams.SqlLobbyFilter = sqlLobbyFilter;
            opParams.ExpectedUsers = expectedUsers;
            return this.loadBalancingPeer.OpJoinRandomRoom(opParams);

            //return this.loadBalancingPeer.OpJoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, playerPropsToSend, matchmakingMode, lobby, sqlLobbyFilter);
        }


        /// <summary>
        /// Joins a room by roomName. Useful when using room lists in lobbies or when you know the name otherwise.
        /// </summary>
        /// <remarks>
        /// This method is useful when you are using a lobby to list rooms and know their names.
        /// A room's name has to be unique (per region and game version), so it does not matter which lobby it's in.
        ///
        /// If the room is full, closed or not existing, this will fail. Override this class and implement
        /// OnOperationResponse(OperationResponse operationResponse) to get the errors.
        ///
        /// OpJoinRoom can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before calling this method.
        /// Alternatively, check the returned bool value.
        ///
        /// While the server is joining the game, the State will be ClientState.Joining.
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and join the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When joining a room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// It's usually better to use OpJoinOrCreateRoom for invitations.
        /// Then it does not matter if the room is already setup.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="roomName">The name of the room to join. Must be existing already, open and non-full or can't be joined.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinRoom(string roomName, string[] expectedUsers = null)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinRoom;
            bool onGameServer = this.Server == ServerConnection.GameServer;


            EnterRoomParams opParams = new EnterRoomParams();
            this.enterRoomParamsCache = opParams;
            opParams.RoomName = roomName;
            opParams.OnGameServer = onGameServer;
            opParams.ExpectedUsers = expectedUsers;

            return this.loadBalancingPeer.OpJoinRoom(opParams);
        }

        /// <summary>
        /// Rejoins a room by roomName (using the userID internally to return). Useful to return to a persisted room or after temporarily losing connection.
        /// </summary>
        public bool OpReJoinRoom(string roomName)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinRoom;
            bool onGameServer = this.Server == ServerConnection.GameServer;


            EnterRoomParams opParams = new EnterRoomParams();
            this.enterRoomParamsCache = opParams;
            opParams.RoomName = roomName;
            opParams.OnGameServer = onGameServer;
            opParams.RejoinOnly = true;

            return this.loadBalancingPeer.OpJoinRoom(opParams);
        }


        /// <summary>
        /// Joins a specific room by name. If the room does not exist (yet), it will be created implicitly.
        /// </summary>
        /// <remarks>
        /// Unlike OpJoinRoom, this operation does not fail if the room does not exist.
        /// This can be useful when you send invitations to a room before actually creating it:
        /// Any invited player (whoever is first) can call this and on demand, the room gets created implicitly.
        ///
        /// This operation does not allow you to re-join a game. To return to a room, use OpJoinRoom with
        /// the actorNumber which was assigned previously.
        ///
        /// If you set room properties in RoomOptions, they get ignored when the room is existing already.
        /// This avoids changing the room properties by late joining players. Only when the room gets created,
        /// the RoomOptions are set in this case.
        ///
        /// If the room is full or closed, this will fail. Override this class and implement
        /// OnOperationResponse(OperationResponse operationResponse) to get the errors.
        ///
        /// This method can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before
        /// calling this method. Alternatively, check the returned bool value.
        ///
        /// While the server is joining the game, the State will be ClientState.Joining.
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and join the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When entering the room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="roomName">The name of the room to join (might be created implicitly).</param>
        /// <param name="roomOptions">Contains the parameters and properties of the new room. See RoomOptions class for a description of each.</param>
        /// <param name="lobby">Typed lobby to be used if the roomname is not in use (and room gets created). If != null, it will also set CurrentLobby.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpJoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.JoinOrCreateRoom;
            this.CurrentLobby = lobby;
            bool onGameServer = this.Server == ServerConnection.GameServer;

            EnterRoomParams opParams = new EnterRoomParams();
            this.enterRoomParamsCache = opParams;
            opParams.RoomName = roomName;
            opParams.RoomOptions = roomOptions;
            opParams.Lobby = lobby;
            opParams.CreateIfNotExists = true;
            opParams.OnGameServer = onGameServer;
            opParams.ExpectedUsers = expectedUsers;

            return this.loadBalancingPeer.OpJoinRoom(opParams);
        }


        /// <summary>
        /// Creates a new room on the server (or fails if the name is already in use).
        /// </summary>
        /// <remarks>
        /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a
        /// roomName (a GUID as string). Room names are unique.
        ///
        /// A room will be attached to the specified lobby. Use null as lobby to attach the
        /// room to the lobby you are now in. If you are in no lobby, the default lobby is used.
        ///
        /// Multiple lobbies can help separate players by map or skill or game type. Each room can only be found
        /// in one lobby (no matter if defined by name and type or as default).
        ///
        /// This method can only be called while the client is connected to a Master Server.
        /// You should check LoadBalancingClient.Server and LoadBalancingClient.IsConnectedAndReady before calling this method.
        /// Alternatively, check the returned bool value.
        ///
        /// Even when sent, the Operation will fail (on the server) if the roomName is in use.
        /// Override this class and implement  OnOperationResponse(OperationResponse operationResponse) to get the errors.
        ///
        ///
        /// While the server is creating the game, the State will be ClientState.Joining.
        /// The state Joining is used because the client is on the way to enter a room (no matter if joining or creating).
        /// It's set immediately when this method sends the Operation.
        ///
        /// If successful, the LoadBalancingClient will get a Game Server Address and use it automatically
        /// to switch servers and enter the room. When you're in the room, this client's State will become
        /// ClientState.Joined (both, for joining or creating it).
        /// Set a OnStateChangeAction method to check for states.
        ///
        /// When entering the room, this client's Player Custom Properties will be sent to the room.
        /// Use LocalPlayer.SetCustomProperties to set them, even while not yet in the room.
        /// Note that the player properties will be cached locally and sent to any next room you would join, too.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        /// </remarks>
        /// <param name="roomName">The name to create a room with. Must be unique and not in use or can't be created. If null, the server will assign a GUID as name.</param>
        /// <param name="roomOptions">Contains the parameters and properties of the new room. See RoomOptions class for a description of each.</param>
        /// <param name="lobby">The lobby (name and type) in which to create the room. Null uses the current lobby or the default lobby (if not in a lobby).</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation could be sent currently (requires connection to Master Server).</returns>
        public bool OpCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby lobby, string[] expectedUsers = null)
        {
            this.State = ClientState.Joining;
            this.lastJoinType = JoinType.CreateRoom;
            this.CurrentLobby = lobby;
            bool onGameServer = this.Server == ServerConnection.GameServer;

            EnterRoomParams opParams = new EnterRoomParams();
            this.enterRoomParamsCache = opParams;
            opParams.RoomName = roomName;
            opParams.RoomOptions = roomOptions;
            opParams.Lobby = lobby;
            opParams.OnGameServer = onGameServer;
            opParams.ExpectedUsers = expectedUsers;

            return this.loadBalancingPeer.OpCreateRoom(opParams);
            //return this.loadBalancingPeer.OpCreateRoom(roomName, roomOptions, lobby, playerPropsToSend, onGameServer);
        }


        /// <summary>
        /// Leaves the CurrentRoom and returns to the Master server (back to the lobby).
        /// OpLeaveRoom skips execution when the room is null or the server is not GameServer or the client is disconnecting from GS already.
        /// OpLeaveRoom returns false in those cases and won't change the state, so check return of this method.
        /// </summary>
        /// <remarks>
        /// This method actually is not an operation per se. It sets a state and calls Disconnect().
        /// This is is quicker than calling OpLeave and then disconnect (which also triggers a leave).
        /// </remarks>
        /// <returns>If the current room could be left (impossible while not in a room).</returns>
        public bool OpLeaveRoom()
        {
            return OpLeaveRoom(false);  //TURNBASED
        }


        /// <summary>
        /// Leaves the current room, optionally telling the server that the user is just becoming inactive.
        /// </summary>
        ///
        /// <param name="becomeInactive">If true, this player becomes inactive in the game and can return later (if PlayerTTL of the room is > 0).</param>
        /// <remarks>
        /// OpLeaveRoom skips execution when the room is null or the server is not GameServer or the client is disconnecting from GS already.
        /// OpLeaveRoom returns false in those cases and won't change the state, so check return of this method.
        ///
        /// In some cases, this method will skip the OpLeave call and just call Disconnect(),
        /// which not only leaves the room but also the server. Disconnect also triggers a leave and so that workflow is is quicker.
        /// </remarks>
        /// <returns>If the current room could be left (impossible while not in a room).</returns>
        public bool OpLeaveRoom(bool becomeInactive)
        {
            if (this.CurrentRoom == null || this.Server != ServerConnection.GameServer || this.State == ClientState.DisconnectingFromGameserver)
            {
                return false;
            }

            if (becomeInactive)
            {
                this.State = ClientState.DisconnectingFromGameserver;
                this.loadBalancingPeer.Disconnect();
            }
            else
            {
                this.State = ClientState.Leaving;
                this.loadBalancingPeer.OpLeaveRoom(false);    //TURNBASED users can leave a room forever or return later
            }

            return true;
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

            if (this.CurrentRoom == null)
            {
                // if you attempt to set this player's values without conditions, then fine:
                if (expectedProperties == null && webFlags == null && this.LocalPlayer != null && this.LocalPlayer.ID == actorNr)
                {
                    this.LocalPlayer.SetCustomProperties(propertiesToSet);
                    return true;
                }

                if (this.loadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    this.DebugReturn(DebugLevel.ERROR, "OpSetCustomPropertiesOfActor() failed. To use expectedProperties or webForward, you have to be in a room. State: " + this.State);
                }
                return false;
            }

            Hashtable customActorProperties = new Hashtable();
            customActorProperties.MergeStringKeys(propertiesToSet);

            return this.OpSetPropertiesOfActor(actorNr, customActorProperties, expectedProperties, webFlags);
        }

        /// <summary>Replaced by a newer version with WebFlags.</summary>
        [Obsolete("Use the overload with WebFlags.")]
        public bool OpSetCustomPropertiesOfActor(int actorNr, Hashtable propertiesToSet, Hashtable expectedProperties, bool webForward)
        {
            return this.OpSetCustomPropertiesOfActor(actorNr, propertiesToSet, expectedProperties, (webForward ? new WebFlags(WebFlags.HttpForwardConst) : null));
        }


        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        protected internal bool OpSetPropertiesOfActor(int actorNr, Hashtable actorProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (this.CurrentRoom == null)
            {
                if (this.loadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfActor() failed because this client is not in a room currently. State: " + this.State);
                }
                return false;
            }

            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                Player target = this.CurrentRoom.GetPlayer(actorNr);
                if (target != null)
                {
                    target.CacheProperties(actorProperties);
                }
            }

            return this.loadBalancingPeer.OpSetPropertiesOfActor(actorNr, actorProperties, expectedProperties, webFlags);
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
        /// <param name="webForward">Defines if the set properties should be forwarded to a WebHook.</param>
        public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            Hashtable customGameProps = new Hashtable();
            customGameProps.MergeStringKeys(propertiesToSet);

            return this.OpSetPropertiesOfRoom(customGameProps, expectedProperties, webFlags);
        }

        /// <summary>Replaced by a newer version with WebFlags.</summary>
        [Obsolete("Use the overload with WebFlags.")]
        public bool OpSetCustomPropertiesOfRoom(Hashtable propertiesToSet, Hashtable expectedProperties, bool webForward)
        {
            return this.OpSetCustomPropertiesOfRoom(propertiesToSet, expectedProperties, (webForward ? new WebFlags(WebFlags.HttpForwardConst) : null));
        }


        /// <summary>Internally used to cache and set properties (including well known properties).</summary>
        /// <remarks>Requires being in a room (because this attempts to send an operation which will fail otherwise).</remarks>
        protected internal bool OpSetPropertiesOfRoom(Hashtable gameProperties, Hashtable expectedProperties = null, WebFlags webFlags = null)
        {
            if (this.CurrentRoom == null)
            {
                if (this.loadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                {
                    this.DebugReturn(DebugLevel.ERROR, "OpSetPropertiesOfRoom() failed because this client is not in a room currently. State: " + this.State);
                }
                return false;
            }

            if (expectedProperties == null || expectedProperties.Count == 0)
            {
                this.CurrentRoom.CacheProperties(gameProperties);
            }
            return this.loadBalancingPeer.OpSetPropertiesOfRoom(gameProperties, expectedProperties, webFlags);
        }


        /// <summary>
        /// Send an event with custom code/type and any content to the other players in the same room.
        /// </summary>
        /// <remarks>This override explicitly uses another parameter order to not mix it up with the implementation for Hashtable only.</remarks>
        /// <param name="eventCode">Identifies this type of event (and the content). Your game's event codes can start with 0.</param>
        /// <param name="customEventContent">Any serializable datatype (including Hashtable like the other OpRaiseEvent overloads).</param>
        /// <param name="sendReliable">If this event has to arrive reliably (potentially repeated if it's lost).</param>
        /// <param name="raiseEventOptions">Contains (slightly) less often used options. If you pass null, the default options will be used.</param>
        /// <returns>If operation could be enqueued for sending. Sent when calling: Service or SendOutgoingCommands.</returns>
        public virtual bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
        {
            if (this.loadBalancingPeer == null)
            {
                return false;
            }

            return this.loadBalancingPeer.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
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
            if (this.loadBalancingPeer == null)
            {
                return false;
            }

            return this.loadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
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
            Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
            opParameters.Add(ParameterCode.UriPath, uriPath);
            opParameters.Add(ParameterCode.WebRpcParameters, parameters);
            if (sendAuthCookie)
            {
                opParameters.Add(ParameterCode.EventForward, WebFlags.SendAuthCookieConst);
            }
            return this.loadBalancingPeer.OpCustom(OperationCode.WebRpc, opParameters, true);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Privately used to read-out properties coming from the server in events and operation responses (which might be a bit tricky).
        /// </summary>
        private void ReadoutProperties(Hashtable gameProperties, Hashtable actorProperties, int targetActorNr)
        {
            // Debug.LogWarning("ReadoutProperties game=" + gameProperties + " actors(" + actorProperties + ")=" + actorProperties + " " + targetActorNr);
            // read game properties and cache them locally
            if (this.CurrentRoom != null && gameProperties != null)
            {
                this.CurrentRoom.CacheProperties(gameProperties);
            }

            if (actorProperties != null && actorProperties.Count > 0)
            {
                if (targetActorNr > 0)
                {
                    // we have a single entry in the actorProperties with one user's name
                    // targets MUST exist before you set properties
                    Player target = this.CurrentRoom.GetPlayer(targetActorNr);
                    if (target != null)
                    {
                        target.CacheProperties(this.ReadoutPropertiesForActorNr(actorProperties, targetActorNr));
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

                        target = this.CurrentRoom.GetPlayer(actorNr);
                        if (target == null)
                        {
                            target = this.CreatePlayer(newName, actorNr, false, props);
                            this.CurrentRoom.StorePlayer(target);
                        }
                        else
                        {
                            target.CacheProperties(props);
                        }
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
        protected internal void ChangeLocalID(int newID)
        {
            if (this.LocalPlayer == null)
            {
                this.DebugReturn(DebugLevel.WARNING, string.Format("Local actor is null or not in mActors! mLocalActor: {0} mActors==null: {1} newID: {2}", this.LocalPlayer, this.CurrentRoom.Players == null, newID));
            }

            if (this.CurrentRoom == null)
            {
                // change to new actor/player ID and make sure the player does not have a room reference left
                this.LocalPlayer.ChangeLocalID(newID);
                this.LocalPlayer.RoomReference = null;
            }
            else
            {
                // remove old actorId from actor list
                this.CurrentRoom.RemovePlayer(this.LocalPlayer);

                // change to new actor/player ID
                this.LocalPlayer.ChangeLocalID(newID);

                // update the room's list with the new reference
                this.CurrentRoom.StorePlayer(this.LocalPlayer);
            }
        }

        /// <summary>
        /// Internally used to clean up local instances of players and room.
        /// </summary>
        private void CleanCachedValues()
        {
            this.ChangeLocalID(-1);
            this.isFetchingFriendList = false;

            // if this is called on the gameserver, we clean the room we were in. on the master, we keep the room to get into it
            if (this.Server == ServerConnection.GameServer || this.State == ClientState.Disconnecting || this.State == ClientState.Uninitialized)
            {
                this.CurrentRoom = null;    // players get cleaned up inside this, too, except LocalPlayer (which we keep)
            }

            // when we leave the master, we clean up the rooms list (which might be updated by the lobby when we join again)
            if (this.Server == ServerConnection.MasterServer || this.State == ClientState.Disconnecting || this.State == ClientState.Uninitialized)
            {
                this.RoomInfoList.Clear();
            }
        }

        /// <summary>
        /// Called internally, when a game was joined or created on the game server.
        /// This reads the response, finds out the local player's actorNumber (a.k.a. Player.ID) and applies properties of the room and players.
        /// </summary>
        /// <param name="operationResponse">Contains the server's response for an operation called by this peer.</param>
        private void GameEnteredOnGameServer(OperationResponse operationResponse)
        {
            if (operationResponse.ReturnCode != 0)
            {
                switch (operationResponse.OperationCode)
                {
                    case OperationCode.CreateGame:
                        this.DebugReturn(DebugLevel.ERROR, "Create failed on GameServer. Changing back to MasterServer. ReturnCode: " + operationResponse.ReturnCode);
                        break;
                    case OperationCode.JoinGame:
                    case OperationCode.JoinRandomGame:
                        this.DebugReturn(DebugLevel.ERROR, "Join failed on GameServer. Changing back to MasterServer.");

                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            this.DebugReturn(DebugLevel.INFO, "Most likely the game became empty during the switch to GameServer.");
                        }

                        // TODO: add callback to join failed
                        break;
                }

                this.DisconnectToReconnect();
                return;
            }

            this.CurrentRoom = this.CreateRoom(this.enterRoomParamsCache.RoomName, this.enterRoomParamsCache.RoomOptions);
            this.CurrentRoom.LoadBalancingClient = this;
            this.CurrentRoom.IsLocalClientInside = true;
            this.State = ClientState.Joined;    // TODO: maybe this set should be done later (when the room is fully setup with props and all)

            if (operationResponse.Parameters.ContainsKey(ParameterCode.ActorList))
            {
                int[] actorsInRoom = (int[])operationResponse.Parameters[ParameterCode.ActorList];
                this.UpdatedActorList(actorsInRoom);
            }

            // the local player's actor-properties are not returned in join-result. add this player to the list
            int localActorNr = (int)operationResponse[ParameterCode.ActorNr];
            this.ChangeLocalID(localActorNr);


            Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
            Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
            this.ReadoutProperties(gameProperties, actorProperties, 0);

            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    // TODO: add callback "game created"
                    break;
                case OperationCode.JoinGame:
                case OperationCode.JoinRandomGame:
                    // TODO: add callback "game joined"
                    break;
            }
        }

        private void UpdatedActorList(int[] actorsInGame)
        {
            if (actorsInGame != null)
            {
                foreach (int userId in actorsInGame)
                {
                    Player target = this.CurrentRoom.GetPlayer(userId);
                    if (target == null)
                    {
                        this.CurrentRoom.StorePlayer(this.CreatePlayer(string.Empty, userId, false, null));
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
            if (opt == null)
            {
                opt = new RoomOptions();
            }
            Room r = new Room(roomName, opt);
            return r;
        }

        private void SetupEncryption(Dictionary<byte, object> encryptionData)
        {
            var mode = (EncryptionMode)(byte)encryptionData[EncryptionDataParameters.Mode];
            switch (mode)
            {
                case EncryptionMode.PayloadEncryption:
                    byte[] encryptionSecret = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                    this.loadBalancingPeer.InitPayloadEncryption(encryptionSecret);
                    break;
                case EncryptionMode.DatagramEncryption:
                    {
                        byte[] secret1 = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                        byte[] secret2 = (byte[])encryptionData[EncryptionDataParameters.Secret2];
                        this.loadBalancingPeer.InitDatagramEncryption(secret1, secret2);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private byte[] encryptionSecret;

        #endregion

        #region IPhotonPeerListener

        /// <summary>Debug output of low level api (and this client).</summary>
        /// <remarks>This method is not responsible to keep up the state of a LoadBalancingClient. Calling base.DebugReturn on overrides is optional.</remarks>
        public virtual void DebugReturn(DebugLevel level, string message)
        {
            Debug.WriteLine(message);
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
                if (this.AuthValues == null)
                {
                    this.AuthValues = new AuthenticationValues();
                    //this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created AuthValues.");
                }

                this.AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
            }

            switch (operationResponse.OperationCode)
            {
                case OperationCode.AuthenticateOnce:
                case OperationCode.Authenticate:
                    {
                        if (operationResponse.ReturnCode != 0)
                        {
                            this.DebugReturn(DebugLevel.ERROR, operationResponse.ToStringFull() + " Server: " + this.Server + " Address: " + this.loadBalancingPeer.ServerAddress);

                            switch (operationResponse.ReturnCode)
                            {
                                case ErrorCode.InvalidAuthentication:
                                    this.DisconnectedCause = DisconnectCause.InvalidAuthentication;
                                    break;
                                case ErrorCode.CustomAuthenticationFailed:
                                    this.DisconnectedCause = DisconnectCause.CustomAuthenticationFailed;
                                    break;
                                case ErrorCode.InvalidRegion:
                                    this.DisconnectedCause = DisconnectCause.InvalidRegion;
                                    break;
                                case ErrorCode.MaxCcuReached:
                                    this.DisconnectedCause = DisconnectCause.MaxCcuReached;
                                    break;
                                case ErrorCode.OperationNotAllowedInCurrentState:
                                    this.DisconnectedCause = DisconnectCause.OperationNotAllowedInCurrentState;
                                    break;
                            }
                            this.State = ClientState.Disconnecting;
                            this.Disconnect();
                            break;  // if auth didn't succeed, we disconnect (above) and exit this operation's handling
                        }

                        if (this.Server == ServerConnection.NameServer || this.Server == ServerConnection.MasterServer)
                        {
                            if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                            {
                                string incomingId = (string)operationResponse.Parameters[ParameterCode.UserId];
                                if (!string.IsNullOrEmpty(incomingId))
                                {
                                    this.UserId = incomingId;
                                    this.DebugReturn(DebugLevel.INFO, string.Format("Setting UserId sent by Server: {0}", this.UserId));
                                }
                            }
                            if (operationResponse.Parameters.ContainsKey(ParameterCode.NickName))
                            {
                                this.NickName = (string)operationResponse.Parameters[ParameterCode.NickName];
                                this.DebugReturn(DebugLevel.INFO, string.Format("Setting Nickname sent by Server:{0}", this.NickName));
                            }

                            if (operationResponse.Parameters.ContainsKey(ParameterCode.EncryptionData))
                            {
                                this.SetupEncryption((Dictionary<byte, object>)operationResponse.Parameters[ParameterCode.EncryptionData]);
                            }
                        }

                        if (this.Server == ServerConnection.NameServer)
                        {
                            // on the NameServer, authenticate returns the MasterServer address for a region and we hop off to there
                            this.MasterServerAddress = operationResponse[ParameterCode.Address] as string;
                            if (this.AuthMode == AuthModeOption.AuthOnceWss)
                            {
                                this.DebugReturn(DebugLevel.INFO, string.Format("Due to AuthOnceWss, switching TransportProtocol to ExpectedProtocol: {0}.", this.ExpectedProtocol));
                                this.loadBalancingPeer.TransportProtocol = this.ExpectedProtocol;
                            }
                            this.DisconnectToReconnect();
                        }
                        else if (this.Server == ServerConnection.MasterServer)
                        {
                            this.State = ClientState.ConnectedToMaster;

                            if (this.AuthMode != AuthModeOption.Auth)
                            {
                                this.loadBalancingPeer.OpSettings(this.EnableLobbyStatistics);
                            }
                            if (this.AutoJoinLobby)
                            {
                                this.loadBalancingPeer.OpJoinLobby(this.CurrentLobby);
                            }
                        }
                        else if (this.Server == ServerConnection.GameServer)
                        {
                            this.State = ClientState.Joining;
                            this.enterRoomParamsCache.PlayerProperties = this.LocalPlayer.AllProperties;
                            this.enterRoomParamsCache.OnGameServer = true;

                            if (this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom || this.lastJoinType == JoinType.JoinOrCreateRoom)
                            {
                                this.loadBalancingPeer.OpJoinRoom(this.enterRoomParamsCache);
                            }
                            else if (this.lastJoinType == JoinType.CreateRoom)
                            {
                                this.loadBalancingPeer.OpCreateRoom(this.enterRoomParamsCache);
                            }
                            break;
                        }
                        break;
                    }

                case OperationCode.GetRegions:
                    this.AvailableRegions = operationResponse[ParameterCode.Region] as string[];
                    this.AvailableRegionsServers = operationResponse[ParameterCode.Address] as string[];
                    break;

                case OperationCode.Leave:
                    //this.CleanCachedValues(); // this is done in status change on "disconnect"
                    this.DisconnectToReconnect();
                    break;

                case OperationCode.JoinLobby:
                    this.State = ClientState.JoinedLobby;
                    this.inLobby = true;
                    break;

                case OperationCode.LeaveLobby:
                    this.State = ClientState.ConnectedToMaster;
                    this.inLobby = false;
                    break;

                case OperationCode.JoinRandomGame:  // this happens only on the master server. on gameserver this is a "regular" join
                case OperationCode.CreateGame:
                case OperationCode.JoinGame:
                    {
                        if (this.Server == ServerConnection.GameServer)
                        {
                            this.GameEnteredOnGameServer(operationResponse);
                        }
                        else
                        {
                            // TODO: handle more error cases
                            if (operationResponse.ReturnCode != 0)
                            {
                                this.State = (this.inLobby) ? ClientState.JoinedLobby : ClientState.ConnectedToMaster;

                                // TODO: error callbacks?! at the moment, developers have to implement this.OnOpResponseAction to get error cases of this.
                                //if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                                //{
                                //    // this happens only for JoinRandomRoom
                                //    break;
                                //}

                                if (this.loadBalancingPeer.DebugOut >= DebugLevel.ERROR)
                                {
                                    this.DebugReturn(DebugLevel.ERROR, string.Format("Getting into game failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));
                                }
                                break;
                            }

                            this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                            string roomName = operationResponse[ParameterCode.RoomName] as string;
                            if (!string.IsNullOrEmpty(roomName))
                            {
                                this.enterRoomParamsCache.RoomName = roomName;
                            }

                            this.DisconnectToReconnect();
                        }

                        break;
                    }

                case OperationCode.FindFriends:
                    if (operationResponse.ReturnCode != 0)
                    {
                        this.DebugReturn(DebugLevel.ERROR, "OpFindFriends failed: " + operationResponse.ToStringFull());
                        this.isFetchingFriendList = false;
                        break;
                    }

                    bool[] onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                    string[] roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                    this.FriendList = new List<FriendInfo>(this.friendListRequested.Length);
                    for (int index = 0; index < this.friendListRequested.Length; index++)
                    {
                        FriendInfo friend = new FriendInfo();
                        friend.Name = this.friendListRequested[index];
                        friend.Room = roomList[index];
                        friend.IsOnline = onlineList[index];
                        this.FriendList.Insert(index, friend);
                    }

                    this.friendListRequested = null;
                    this.isFetchingFriendList = false;
                    this.friendListTimestamp = Environment.TickCount;
                    if (this.friendListTimestamp == 0)
                    {
                        this.friendListTimestamp = 1;   // makes sure the timestamp is not accidentally 0
                    }
                    break;
            }

            if (this.OnOpResponseAction != null) this.OnOpResponseAction(operationResponse);
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
                    this.inLobby = false;

                    if (this.State == ClientState.ConnectingToNameServer)
                    {
                        if (this.loadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            this.DebugReturn(DebugLevel.ALL, "Connected to nameserver.");
                        }

                        this.Server = ServerConnection.NameServer;
                        if (this.AuthValues != null)
                        {
                            this.AuthValues.Token = null; // when connecting to NameServer, invalidate the secret (only)
                        }
                    }

                    if (this.State == ClientState.ConnectingToGameserver)
                    {
                        if (this.loadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            this.DebugReturn(DebugLevel.ALL, "Connected to gameserver.");
                        }

                        this.Server = ServerConnection.GameServer;
                    }

                    if (this.State == ClientState.ConnectingToMasterserver)
                    {
                        if (this.loadBalancingPeer.DebugOut >= DebugLevel.ALL)
                        {
                            this.DebugReturn(DebugLevel.ALL, "Connected to masterserver.");
                        }

                        this.Server = ServerConnection.MasterServer;
                    }


                    if (this.loadBalancingPeer.TransportProtocol != ConnectionProtocol.WebSocketSecure)
                    {
                        if (this.Server == ServerConnection.NameServer || this.AuthMode == AuthModeOption.Auth)
                        {
                            this.loadBalancingPeer.EstablishEncryption();
                        }
                    }
                    else
                    {
                        goto case StatusCode.EncryptionEstablished;
                    }

                    break;

                case StatusCode.EncryptionEstablished:
                    // on nameserver, the "process" is stopped here, so the developer/game can either get regions or authenticate with a specific region
                    if (this.Server == ServerConnection.NameServer)
                    {
                        this.State = ClientState.ConnectedToNameServer;

                        // TODO: should we automatically get the regions?!
                    }

                    if (this.Server != ServerConnection.NameServer && (this.AuthMode == AuthModeOption.AuthOnce || this.AuthMode == AuthModeOption.AuthOnceWss))
                    {
                        // AuthMode "Once" means we only authenticate on the NameServer
                        break;
                    }


                    // on any other server we might now have to authenticate still, so the client can do anything at all
                    if (!this.didAuthenticate && (!this.IsUsingNameServer || this.CloudRegion != null))
                    {
                        // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
                        this.didAuthenticate = this.CallAuthenticate();

                        if (this.didAuthenticate)
                        {
                            this.State = ClientState.Authenticating;
                        }
                        else
                        {
                            this.DebugReturn(DebugLevel.ERROR, "Error calling OpAuthenticate! Did not work. Check log output, AuthValues and if you're connected. State: " + this.State);
                        }
                    }
                    break;

                case StatusCode.Disconnect:
                    // disconnect due to connection exception is handled below (don't connect to GS or master in that case)

                    this.CleanCachedValues();
                    this.didAuthenticate = false;   // on connect, we know that we didn't
                    this.inLobby = false;

                    switch (this.State)
                    {
                        case ClientState.Uninitialized:
                        case ClientState.Disconnecting:
                            if (this.AuthValues != null)
                            {
                                this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                            }
                            this.State = ClientState.Disconnected;
                            break;

                        case ClientState.DisconnectingFromGameserver:
                        case ClientState.DisconnectingFromNameServer:
                            this.Connect();                 // this gets the client back to the Master Server
                            break;

                        case ClientState.DisconnectingFromMasterserver:
                            this.ConnectToGameServer();     // this connects the client with the Game Server (when joining/creating a room)
                            break;

                        default:
                            string stacktrace = "";
                            #if DEBUG && !NETFX_CORE
                            stacktrace = new System.Diagnostics.StackTrace(true).ToString();
                            #endif
                            this.DebugReturn(DebugLevel.WARNING, "Got a unexpected Disconnect in LoadBalancingClient State: " + this.State + ". " + stacktrace);

                            if (this.AuthValues != null)
                            {
                                this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                            }
                            this.State = ClientState.Disconnected;

                            break;
                    }
                    break;

                case StatusCode.DisconnectByServerUserLimit:
                    this.DebugReturn(DebugLevel.ERROR, "The Photon license's CCU Limit was reached. Server rejected this connection. Wait and re-try.");
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.DisconnectByServerUserLimit;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.ExceptionOnConnect:
                case StatusCode.SecurityExceptionOnConnect:
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.ExceptionOnConnect;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.DisconnectByServer:
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.DisconnectByServer;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.DisconnectByServerLogic:
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.DisconnectByServerLogic;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.TimeoutDisconnect:
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.TimeoutDisconnect;
                    this.State = ClientState.Disconnected;
                    break;
                case StatusCode.Exception:
                case StatusCode.ExceptionOnReceive:
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null; // when leaving the server, invalidate the secret (but not the auth values)
                    }
                    this.DisconnectedCause = DisconnectCause.Exception;
                    this.State = ClientState.Disconnected;
                    break;
            }
        }

        /// <summary>
        /// Uses the photonEvent's provided by the server to advance the internal state and call ops as needed.
        /// </summary>
        /// <remarks>This method is essential to update the internal state of a LoadBalancingClient. Overriding methods must call base.OnEvent.</remarks>
        public virtual void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case EventCode.GameList:
                case EventCode.GameListUpdate:
                    if (photonEvent.Code == EventCode.GameList)
                    {
                        this.RoomInfoList = new Dictionary<string, RoomInfo>();
                    }

                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (string gameName in games.Keys)
                    {
                        RoomInfo game = new RoomInfo(gameName, (Hashtable)games[gameName]);
                        if (game.removedFromList)
                        {
                            this.RoomInfoList.Remove(gameName);
                        }
                        else
                        {
                            this.RoomInfoList[gameName] = game;
                        }
                    }
                    break;

                case EventCode.Join:
                    int actorNr = (int)photonEvent[ParameterCode.ActorNr];  // actorNr (a.k.a. playerNumber / ID) of sending player
                    Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];

                    Player originatingPlayer = this.CurrentRoom.GetPlayer(actorNr);
                    if (originatingPlayer == null)
                    {
                        Player newPlayer = this.CreatePlayer(string.Empty, actorNr, false, actorProperties);
                        this.CurrentRoom.StorePlayer(newPlayer);
                    }
                    else
                    {
                        originatingPlayer.CacheProperties(actorProperties);
                        originatingPlayer.IsInactive = false;
                    }

                    if (this.LocalPlayer.ID == actorNr)
                    {
                        // in this player's own join event, we get a complete list of players in the room, so check if we know each of the
                        int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                        this.UpdatedActorList(actorsInRoom);
                    }
                    break;

                case EventCode.Leave:
                    {
                        int actorID = (int) photonEvent[ParameterCode.ActorNr];

                        bool isInactive = false;
                        if (photonEvent.Parameters.ContainsKey(ParameterCode.IsInactive))
                        {
                            isInactive = (bool)photonEvent.Parameters[ParameterCode.IsInactive];
                        }

                        if (isInactive)
                        {
                            this.CurrentRoom.MarkAsInactive(actorID);
                        }
                        else
                        {
                            this.CurrentRoom.RemovePlayer(actorID);
                        }

                        // having a new master before calling destroy for the leaving player is important!
                        // so we elect a new masterclient and ignore the leaving player (who is still in playerlists).
                        // note: there is/was a server-side-error which sent 0 as new master instead of skipping the key/value. below is a check for 0 due to that.
                        // note: if the MasterClientId is NOT set by the server, RoomInfo.serverSideMasterClient is false and RemovePlayer() sets the Master Client ID.
                        if (photonEvent.Parameters.ContainsKey(ParameterCode.MasterClientId))
                        {
                            int newMaster = (int)photonEvent[ParameterCode.MasterClientId];
                            if (newMaster != 0)
                            {
                                this.CurrentRoom.masterClientIdField = newMaster;
                            }
                        }
                    }
                    break;

                // EventCode.Disconnect was "replaced" by Leave which now has a "inactive" flag.
                //case EventCode.Disconnect:  //TURNBASED
                //    {
                //        int actorID = (int) photonEvent[ParameterCode.ActorNr];
                //        this.CurrentRoom.MarkAsInactive(actorID);
                //    }
                //    break;

                case EventCode.PropertiesChanged:
                    // whenever properties are sent in-room, they can be broadcasted as event (which we handle here)
                    // we get PLAYERproperties if actorNr > 0 or ROOMproperties if actorNumber is not set or 0
                    int targetActorNr = 0;
                    if (photonEvent.Parameters.ContainsKey(ParameterCode.TargetActorNr))
                    {
                        targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
                    }
                    Hashtable props = (Hashtable)photonEvent[ParameterCode.Properties];

                    if (targetActorNr > 0)
                    {
                        this.ReadoutProperties(null, props, targetActorNr);
                    }
                    else
                    {
                        this.ReadoutProperties(props, null, 0);
                    }

                    break;

                case EventCode.AppStats:
                    // only the master server sends these in (1 minute) intervals
                    this.PlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                    this.RoomsCount = (int)photonEvent[ParameterCode.GameCount];
                    this.PlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                    break;

                case EventCode.LobbyStats:
                    string[] names = photonEvent[ParameterCode.LobbyName] as string[];
                    byte[] types = photonEvent[ParameterCode.LobbyType] as byte[];
                    int[] peers = photonEvent[ParameterCode.PeerCount] as int[];
                    int[] rooms = photonEvent[ParameterCode.GameCount] as int[];

                    this.lobbyStatistics.Clear();
                    for (int i = 0; i < names.Length; i++)
                    {
                        TypedLobbyInfo info = new TypedLobbyInfo();
                        info.Name = names[i];
                        info.Type = (LobbyType)types[i];
                        info.PlayerCount = peers[i];
                        info.RoomCount = rooms[i];

                        this.lobbyStatistics.Add(info);
                    }
                    break;

                case EventCode.ErrorInfo:
                    if (this.OnEventAction != null)
                    {
                        this.OnEventAction(photonEvent);
                    }
                    break;

                case EventCode.AuthEvent:
                    if (this.AuthValues == null)
                    {
                        this.AuthValues = new AuthenticationValues();
                    }

                    this.AuthValues.Token = photonEvent[ParameterCode.Secret] as string;
                    break;

            }

            if (this.OnEventAction != null) this.OnEventAction(photonEvent);
        }

        /// <summary>In Photon 4, "raw messages" will get their own callback method in the interface. Not used yet.</summary>
        public virtual void OnMessage(object message)
        {
            this.DebugReturn(DebugLevel.ALL, string.Format("got OnMessage {0}", message));
        }

        #endregion
    }
}
