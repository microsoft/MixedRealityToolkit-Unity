// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetworkingPeer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking (PUN)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using SupportClassPun = ExitGames.Client.Photon.SupportClass;



#region Enums

/// <summary>
/// Detailed connection / networking peer state.
/// PUN implements a loadbalancing and authentication workflow "behind the scenes", so
/// some states will automatically advance to some follow up state. Those states are
/// commented with "(will-change)".
/// </summary>
/// \ingroup publicApi
public enum ClientState
{
    /// <summary>Not running. Only set before initialization and first use.</summary>
    Uninitialized,

    /// <summary>Created and available to connect.</summary>
    PeerCreated,

    /// <summary>Not used at the moment.</summary>
    Queued,

    /// <summary>The application is authenticated. PUN usually joins the lobby now.</summary>
    /// <remarks>(will-change) Unless AutoJoinLobby is false.</remarks>
    Authenticated,

    /// <summary>Client is in the lobby of the Master Server and gets room listings.</summary>
    /// <remarks>Use Join, Create or JoinRandom to get into a room to play.</remarks>
    JoinedLobby,

    /// <summary>Disconnecting.</summary>
    /// <remarks>(will-change)</remarks>
    DisconnectingFromMasterserver,

    /// <summary>Connecting to game server (to join/create a room and play).</summary>
    /// <remarks>(will-change)</remarks>
    ConnectingToGameserver,

    /// <summary>Similar to Connected state but on game server. Still in process to join/create room.</summary>
    /// <remarks>(will-change)</remarks>
    ConnectedToGameserver,

    /// <summary>In process to join/create room (on game server).</summary>
    /// <remarks>(will-change)</remarks>
    Joining,

    /// <summary>Final state of a room join/create sequence. This client can now exchange events / call RPCs with other clients.</summary>
    Joined,

    /// <summary>Leaving a room.</summary>
    /// <remarks>(will-change)</remarks>
    Leaving,

    /// <summary>Workflow is leaving the game server and will re-connect to the master server.</summary>
    /// <remarks>(will-change)</remarks>
    DisconnectingFromGameserver,

    /// <summary>Workflow is connected to master server and will establish encryption and authenticate your app.</summary>
    /// <remarks>(will-change)</remarks>
    ConnectingToMasterserver,

    /// <summary>Same Queued but coming from game server.</summary>
    /// <remarks>(will-change)</remarks>
    QueuedComingFromGameserver,

    /// <summary>PUN is disconnecting. This leads to Disconnected.</summary>
    /// <remarks>(will-change)</remarks>
    Disconnecting,

    /// <summary>No connection is setup, ready to connect. Similar to PeerCreated.</summary>
    Disconnected,

    /// <summary>Final state for connecting to master without joining the lobby (AutoJoinLobby is false).</summary>
    ConnectedToMaster,

    /// <summary>Client connects to the NameServer. This process includes low level connecting and setting up encryption. When done, state becomes ConnectedToNameServer.</summary>
    ConnectingToNameServer,

    /// <summary>Client is connected to the NameServer and established enctryption already. You should call OpGetRegions or ConnectToRegionMaster.</summary>
    ConnectedToNameServer,

    /// <summary>When disconnecting from a Photon NameServer.</summary>
    /// <remarks>(will-change)</remarks>
    DisconnectingFromNameServer,

    /// <summary>When connecting to a Photon Server, this state is intermediate before you can call any operations.</summary>
    /// <remarks>(will-change)</remarks>
    Authenticating
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


/// <summary>
/// Summarizes the cause for a disconnect. Used in: OnConnectionFail and OnFailedToConnectToPhoton.
/// </summary>
/// <remarks>Extracted from the status codes from ExitGames.Client.Photon.StatusCode.</remarks>
/// <seealso cref="PhotonNetworkingMessage"/>
/// \ingroup publicApi
public enum DisconnectCause
{
    /// <summary>Server actively disconnected this client.
    /// Possible cause: The server's user limit was hit and client was forced to disconnect (on connect).</summary>
    DisconnectByServerUserLimit = StatusCode.DisconnectByServerUserLimit,

    /// <summary>Connection could not be established.
    /// Possible cause: Local server not running.</summary>
    ExceptionOnConnect = StatusCode.ExceptionOnConnect,

    /// <summary>Timeout disconnect by server (which decided an ACK was missing for too long).</summary>
    DisconnectByServerTimeout = StatusCode.DisconnectByServer,

    /// <summary>Server actively disconnected this client.
    /// Possible cause: Server's send buffer full (too much data for client).</summary>
    DisconnectByServerLogic = StatusCode.DisconnectByServerLogic,

    /// <summary>Some exception caused the connection to close.</summary>
    Exception = StatusCode.Exception,

    /// <summary>(32767) The Photon Cloud rejected the sent AppId. Check your Dashboard and make sure the AppId you use is complete and correct.</summary>
    InvalidAuthentication = ErrorCode.InvalidAuthentication,

    /// <summary>(32757) Authorization on the Photon Cloud failed because the concurrent users (CCU) limit of the app's subscription is reached.</summary>
    MaxCcuReached = ErrorCode.MaxCcuReached,

    /// <summary>(32756) Authorization on the Photon Cloud failed because the app's subscription does not allow to use a particular region's server.</summary>
    InvalidRegion = ErrorCode.InvalidRegion,

    /// <summary>The security settings for client or server don't allow a connection (see remarks).</summary>
    /// <remarks>
    /// A common cause for this is that browser clients read a "crossdomain" file from the server.
    /// If that file is unavailable or not configured to let the client connect, this exception is thrown.
    /// Photon usually provides this crossdomain file for Unity.
    /// If it fails, read:
    /// http://doc.exitgames.com/photon-server/PolicyApp
    /// </remarks>
    SecurityExceptionOnConnect = StatusCode.SecurityExceptionOnConnect,

    /// <summary>Timeout disconnect by client (which decided an ACK was missing for too long).</summary>
    DisconnectByClientTimeout = StatusCode.TimeoutDisconnect,

    /// <summary>Exception in the receive-loop.
    /// Possible cause: Socket failure.</summary>
    InternalReceiveException = StatusCode.ExceptionOnReceive,

    /// <summary>(32753) The Authentication ticket expired. Handle this by connecting again (which includes an authenticate to get a fresh ticket).</summary>
    AuthenticationTicketExpired = 32753,
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

#endregion

/// <summary>
/// Implements Photon LoadBalancing used in PUN.
/// This class is used internally by PhotonNetwork and not intended as public API.
/// </summary>
internal class NetworkingPeer : LoadBalancingPeer, IPhotonPeerListener
{
    /// <summary>Combination of GameVersion+"_"+PunVersion. Separates players per app by version.</summary>
    protected internal string AppVersion
    {
        get { return string.Format("{0}_{1}", PhotonNetwork.gameVersion, PhotonNetwork.versionPUN); }
    }

    /// <summary>Contains the AppId for the Photon Cloud (ignored by Photon Servers).</summary>
    protected internal string AppId;

    /// <summary>
    /// A user's authentication values used during connect for Custom Authentication with Photon (and a custom service/community).
    /// Set these before calling Connect if you want custom authentication.
    /// </summary>
    public AuthenticationValues AuthValues { get; set; }

	/// <summary>Internally used cache for the server's token. Identifies a user/session and can be used to rejoin.</summary>
    private string tokenCache;


    /// <summary>Enables the new Authentication workflow</summary>
    public AuthModeOption AuthMode = AuthModeOption.Auth;

    /// <summary>Defines how the communication gets encrypted.</summary>
    public EncryptionMode EncryptionMode = EncryptionMode.PayloadEncryption;


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
    public bool IsUsingNameServer { get; protected internal set; }

    /// <summary>Name Server Host Name for Photon Cloud. Without port and without any prefix.</summary>
    public const string NameServerHost = "ns.exitgames.com";

    /// <summary>Name Server for HTTP connections to the Photon Cloud. Includes prefix and port.</summary>
    public const string NameServerHttp = "http://ns.exitgamescloud.com:80/photon/n";

    /// <summary>Name Server port per protocol (the UDP port is different than TCP, etc).</summary>
    private static readonly Dictionary<ConnectionProtocol, int> ProtocolToNameServerPort = new Dictionary<ConnectionProtocol, int>() { { ConnectionProtocol.Udp, 5058 }, { ConnectionProtocol.Tcp, 4533 }, { ConnectionProtocol.WebSocket, 9093 }, { ConnectionProtocol.WebSocketSecure, 19093 } }; //, { ConnectionProtocol.RHttp, 6063 } };

    /// <summary>Name Server Address for Photon Cloud (based on current protocol). You can use the default values and usually won't have to set this value.</summary>
    public string NameServerAddress { get { return this.GetNameServerAddress(); } }

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
    protected internal ServerConnection Server { get; private set; }

    public ClientState State { get; internal set; }

    public bool IsInitialConnect = false;


    public bool insideLobby = false;
    public TypedLobby lobby { get; set; }


    private bool requestLobbyStatistics
    {
        get { return PhotonNetwork.EnableLobbyStatistics && this.Server == ServerConnection.MasterServer; }
    }

    protected internal List<TypedLobbyInfo> LobbyStatistics = new List<TypedLobbyInfo>();


    public Dictionary<string, RoomInfo> mGameList = new Dictionary<string, RoomInfo>();
    public RoomInfo[] mGameListCopy = new RoomInfo[0];

    private string playername = "";

    public string PlayerName
    {
        get
        {
            return this.playername;
        }

        set
        {
            if (string.IsNullOrEmpty(value) || value.Equals(this.playername))
            {
                return;
            }

            if (this.LocalPlayer != null)
            {
                this.LocalPlayer.name = value;
            }

            this.playername = value;
            if (this.CurrentRoom != null)
            {
                // Only when in a room
                this.SendPlayerName();
            }
        }
    }

    // "public" access to the current game - is null unless a room is joined on a gameserver
    // isLocalClientInside becomes true when op join result is positive on GameServer
    private bool mPlayernameHasToBeUpdated;


    public Room CurrentRoom
    {
        get
        {
            if (this.currentRoom != null && this.currentRoom.isLocalClientInside)
            {
                return this.currentRoom;
            }

            return null;
        }

        private set { this.currentRoom = value; }
    }

    private Room currentRoom;

    public PhotonPlayer LocalPlayer { get; internal set; }

    /// <summary>Stat value: Count of players on Master (looking for rooms)</summary>
    public int PlayersOnMasterCount { get; internal set; }

    /// <summary>Stat value: Count of Players in rooms</summary>
    public int PlayersInRoomsCount { get; internal set; }

    /// <summary>Stat value: Count of Rooms</summary>
    public int RoomsCount { get; internal set; }


    private JoinType lastJoinType;

    protected internal EnterRoomParams enterRoomParamsCache;


    /// <summary>Internally used to trigger OpAuthenticate when encryption was established after a connect.</summary>
    private bool didAuthenticate;



    /// <summary>Contains the list of names of friends to look up their state on the server.</summary>
    private string[] friendListRequested;

    /// <summary>
    /// Age of friend list info (in milliseconds). It's 0 until a friend list is fetched.
    /// </summary>
    protected internal int FriendListAge { get { return (this.isFetchingFriendList || this.friendListTimestamp == 0) ? 0 : Environment.TickCount - this.friendListTimestamp; } }

    private int friendListTimestamp;

    /// <summary>Internal flag to know if the client currently fetches a friend list.</summary>
    private bool isFetchingFriendList;

    /// <summary>Internally used to check if a "Secret" is available to use. Sent by Photon Cloud servers, it simplifies authentication when switching servers.</summary>
    public bool IsAuthorizeSecretAvailable
    {
        get
        {
            return this.AuthValues != null && !String.IsNullOrEmpty(this.AuthValues.Token);
        }
    }

    /// <summary>A list of region names for the Photon Cloud. Set by the result of OpGetRegions().</summary>
    /// <remarks>Put a "case OperationCode.GetRegions:" into your OnOperationResponse method to notice when the result is available.</remarks>
    public List<Region> AvailableRegions { get; protected internal set; }

    /// <summary>The cloud region this client connects to. Set by ConnectToRegionMaster().</summary>
    public CloudRegionCode CloudRegion { get; protected internal set; }



    public Dictionary<int, PhotonPlayer> mActors = new Dictionary<int, PhotonPlayer>();

    public PhotonPlayer[] mOtherPlayerListCopy = new PhotonPlayer[0];
    public PhotonPlayer[] mPlayerListCopy = new PhotonPlayer[0];


    public int mMasterClientId
    {
        get
        {
            if (PhotonNetwork.offlineMode) return this.LocalPlayer.ID;
            return (this.CurrentRoom == null) ? 0 : this.CurrentRoom.masterClientId;
        }
        private set
        {
            if (this.CurrentRoom != null)
            {
                this.CurrentRoom.masterClientId = value;
            }
        }
    }

    public bool hasSwitchedMC = false;

    private HashSet<int> allowedReceivingGroups = new HashSet<int>();

    private HashSet<int> blockSendingGroups = new HashSet<int>();

    protected internal Dictionary<int, PhotonView> photonViewList = new Dictionary<int, PhotonView>(); //TODO: make private again

    private readonly PhotonStream readStream = new PhotonStream(false, null);    // only used in OnSerializeRead()
    private readonly PhotonStream pStream = new PhotonStream(true, null);        // only used in OnSerializeWrite()
    private readonly Dictionary<int, Hashtable> dataPerGroupReliable = new Dictionary<int, Hashtable>();    // only used in RunViewUpdate()
    private readonly Dictionary<int, Hashtable> dataPerGroupUnreliable = new Dictionary<int, Hashtable>();  // only used in RunViewUpdate()

    protected internal short currentLevelPrefix = 0;

    /// <summary>Internally used to flag if the message queue was disabled by a "scene sync" situation (to re-enable it).</summary>
    protected internal bool loadingLevelAndPausedNetwork = false;

    /// <summary>For automatic scene syncing, the loaded scene is put into a room property. This is the name of said prop.</summary>
    protected internal const string CurrentSceneProperty = "curScn";

    public static bool UsePrefabCache = true;

    internal IPunPrefabPool ObjectPool;

    public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

    private Dictionary<Type, List<MethodInfo>> monoRPCMethodsCache = new Dictionary<Type, List<MethodInfo>>();

    private readonly Dictionary<string, int> rpcShortcuts;  // lookup "table" for the index (shortcut) of an RPC name

    /// <summary>Caches PhotonNetworkingMessage.OnPhotonInstantiate.ToString(), because DoInstantiate calls it often (and ToString() on the enum is astonishingly expensive).</summary>
    private static readonly string OnPhotonInstantiateString = PhotonNetworkingMessage.OnPhotonInstantiate.ToString();


    // TODO: CAS must be implemented for OfflineMode

    public NetworkingPeer(string playername, ConnectionProtocol connectionProtocol) : base(connectionProtocol)
    {
        this.Listener = this;
        this.LimitOfUnreliableCommands = 40;

        this.lobby = TypedLobby.Default;
        this.PlayerName = playername;
        this.LocalPlayer = new PhotonPlayer(true, -1, this.playername);
        this.AddNewPlayer(this.LocalPlayer.ID, this.LocalPlayer);

        // RPC shortcut lookup creation (from list of RPCs, which is updated by Editor scripts)
        rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
        for (int index = 0; index < PhotonNetwork.PhotonServerSettings.RpcList.Count; index++)
        {
            var name = PhotonNetwork.PhotonServerSettings.RpcList[index];
            rpcShortcuts[name] = index;
        }

        this.State = ClientState.PeerCreated;
    }

    /// <summary>
    /// Gets the NameServer Address (with prefix and port), based on the set protocol (this.UsedProtocol).
    /// </summary>
    /// <returns>NameServer Address (with prefix and port).</returns>
    private string GetNameServerAddress()
    {
        #if RHTTP
        if (currentProtocol == ConnectionProtocol.RHttp)
        {
            return NameServerHttp;
        }
        #endif

        ConnectionProtocol currentProtocol = this.TransportProtocol;
        int protocolPort = 0;
        ProtocolToNameServerPort.TryGetValue(currentProtocol, out protocolPort);

        string protocolPrefix = string.Empty;
        if (currentProtocol == ConnectionProtocol.WebSocket)
        {
            protocolPrefix = "ws://";
        }
        else if (currentProtocol == ConnectionProtocol.WebSocketSecure)
        {
            protocolPrefix = "wss://";
        }

        string result = string.Format("{0}{1}:{2}", protocolPrefix, NameServerHost, protocolPort);
        //Debug.Log("NameServer: " + result);
        return result;
    }

#region Operations and Connection Methods


    public override bool Connect(string serverAddress, string applicationName)
    {
        Debug.LogError("Avoid using this directly. Thanks.");
        return false;
    }

	/// <summary>Can be used to reconnect to the master server after a disconnect.</summary>
	/// <remarks>Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.</remarks>
    public bool ReconnectToMaster()
    {
        if (this.AuthValues == null)
        {
            Debug.LogWarning("ReconnectToMaster() with AuthValues == null is not correct!");
            this.AuthValues = new AuthenticationValues();
        }
        this.AuthValues.Token = this.tokenCache;

        return this.Connect(this.MasterServerAddress, ServerConnection.MasterServer);
    }

    /// <summary>
    /// Can be used to return to a room quickly, by directly reconnecting to a game server to rejoin a room.
    /// </summary>
    /// <returns>False, if the conditions are not met. Then, this client does not attempt the ReconnectAndRejoin.</returns>
    public bool ReconnectAndRejoin()
    {
        if (this.AuthValues == null)
        {
            Debug.LogWarning("ReconnectAndRejoin() with AuthValues == null is not correct!");
            this.AuthValues = new AuthenticationValues();
        }
        this.AuthValues.Token = this.tokenCache;

        if (!string.IsNullOrEmpty(this.GameServerAddress) && this.enterRoomParamsCache != null)
        {
            this.lastJoinType = JoinType.JoinRoom;
            this.enterRoomParamsCache.RejoinOnly = true;
            return this.Connect(this.GameServerAddress, ServerConnection.GameServer);
        }

        return false;
    }


    public bool Connect(string serverAddress, ServerConnection type)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        if (this.State == ClientState.Disconnecting)
        {
            Debug.LogError("Connect() failed. Can't connect while disconnecting (still). Current state: " + PhotonNetwork.connectionStateDetailed);
            return false;
        }

        this.SetupProtocol(type);

        // connect might fail, if the DNS name can't be resolved or if no network connection is available
        bool connecting = base.Connect(serverAddress, "", this.TokenForInit);

        if (connecting)
        {
            switch (type)
            {
                case ServerConnection.NameServer:
                    State = ClientState.ConnectingToNameServer;
                    break;
                case ServerConnection.MasterServer:
                    State = ClientState.ConnectingToMasterserver;
                    break;
                case ServerConnection.GameServer:
                    State = ClientState.ConnectingToGameserver;
                    break;
            }
        }

        return connecting;
    }


    /// <summary>
    /// Connects to the NameServer for Photon Cloud, where a region and server list can be obtained.
    /// </summary>
    /// <see cref="OpGetRegions"/>
    /// <returns>If the workflow was started or failed right away.</returns>
    public bool ConnectToNameServer()
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        this.IsUsingNameServer = true;
        this.CloudRegion = CloudRegionCode.none;

        if (this.State == ClientState.ConnectedToNameServer)
        {
            return true;
        }

        this.SetupProtocol(ServerConnection.NameServer);
        if (!base.Connect(this.NameServerAddress, "ns", this.TokenForInit))
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
    public bool ConnectToRegionMaster(CloudRegionCode region)
    {
        if (PhotonHandler.AppQuits)
        {
            Debug.LogWarning("Ignoring Connect() because app gets closed. If this is an error, check PhotonHandler.AppQuits.");
            return false;
        }

        IsUsingNameServer = true;
        this.CloudRegion = region;

        if (this.State == ClientState.ConnectedToNameServer)
        {
            return this.CallAuthenticate();
        }

        this.SetupProtocol(ServerConnection.NameServer);
        if (!base.Connect(this.NameServerAddress, "ns", this.TokenForInit))
        {
            return false;
        }

        this.State = ClientState.ConnectingToNameServer;
        return true;
    }

    protected internal void SetupProtocol(ServerConnection serverType)
    {
        ConnectionProtocol protocolOverride = this.TransportProtocol;

        if (this.AuthMode == AuthModeOption.AuthOnceWss)
        {
            if (serverType != ServerConnection.NameServer)
            {
                if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
                {
                    Debug.LogWarning("Using PhotonServerSettings.Protocol when leaving the NameServer (AuthMode is AuthOnceWss): " + PhotonNetwork.PhotonServerSettings.Protocol);
                }
                protocolOverride = PhotonNetwork.PhotonServerSettings.Protocol;
            }
            else
            {
                if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
                {
                    Debug.LogWarning("Using WebSocket to connect NameServer (AuthMode is AuthOnceWss).");
                }
                protocolOverride = ConnectionProtocol.WebSocketSecure;
            }
        }

        // to support WebGL export in Unity, we find and assign the SocketWebTcp class (if it's in the project).
        // alternatively class SocketWebTcp might be in the Photon3Unity3D.dll
        Type socketTcp = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp", false);
        if (socketTcp == null)
        {
            socketTcp = Type.GetType("ExitGames.Client.Photon.SocketWebTcp, Assembly-CSharp-firstpass", false);
        }
        if (socketTcp != null)
        {
            this.SocketImplementationConfig[ConnectionProtocol.WebSocket] = socketTcp;
            this.SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = socketTcp;
        }


        #if UNITY_WEBGL
        if (this.TransportProtocol != ConnectionProtocol.WebSocket && this.TransportProtocol != ConnectionProtocol.WebSocketSecure)
        {
			Debug.Log("WebGL only supports WebSocket protocol. Overriding PhotonServerSettings.");
	        protocolOverride = ConnectionProtocol.WebSocketSecure;
		}
        #endif


        #if !UNITY_EDITOR && (UNITY_WINRT)
        // this automatically uses a separate assembly-file with Win8-style Socket usage (not possible in Editor)
        Debug.LogWarning("Using PingWindowsStore");
        PhotonHandler.PingImplementation = typeof(PingWindowsStore);    // but for ping, we have to set the implementation explicitly to Win 8 Store/Phone
        #endif


        #pragma warning disable 0162    // the library variant defines if we should use PUN's SocketUdp variant (at all)
        if (PhotonPeer.NoSocket)
        {
            if (this.TransportProtocol != ConnectionProtocol.Udp)
            {
                Debug.Log("This Photon3Unity3d.dll only allows UDP. TransportProtocol was: " + this.TransportProtocol + ". SocketImplementation: " + this.SocketImplementation);
            }
            protocolOverride = ConnectionProtocol.Udp;

            #if !UNITY_EDITOR && (UNITY_PS3 || UNITY_ANDROID)
            this.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpNativeDynamic);
            PhotonHandler.PingImplementation = typeof(PingNativeDynamic);
            #elif !UNITY_EDITOR && UNITY_IPHONE
            this.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdpNativeStatic);
            PhotonHandler.PingImplementation = typeof(PingNativeStatic);
            #elif !UNITY_EDITOR && UNITY_WINRT
            // this automatically uses a separate assembly-file with Win8-style Socket usage (not possible in Editor)
            #else
            this.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdp);
            PhotonHandler.PingImplementation = typeof(PingMonoEditor);
            #endif

            if (this.SocketImplementationConfig[ConnectionProtocol.Udp] == null)
            {
                Debug.Log("No socket implementation set for 'NoSocket' assembly. Please check your settings.");
            }
        }
        #pragma warning restore 0162

        if (PhotonHandler.PingImplementation == null)
        {
            PhotonHandler.PingImplementation = typeof(PingMono);
        }


        if (this.TransportProtocol == protocolOverride)
        {
            return;
        }


        if (PhotonNetwork.logLevel >= PhotonLogLevel.ErrorsOnly)
        {
            Debug.LogWarning("Protocol switch from: " + this.TransportProtocol + " to: " + protocolOverride + ".");
        }

        this.TransportProtocol = protocolOverride;
    }

    /// <summary>
    /// Complete disconnect from photon (and the open master OR game server)
    /// </summary>
    public override void Disconnect()
    {
        if (this.PeerState == PeerStateValue.Disconnected)
        {
            if (!PhotonHandler.AppQuits)
            {
                Debug.LogWarning(string.Format("Can't execute Disconnect() while not connected. Nothing changed. State: {0}", this.State));
            }
            return;
        }

        this.State = ClientState.Disconnecting;
        base.Disconnect();

        //this.LeftRoomCleanup();
        //this.LeftLobbyCleanup();
    }

    private bool CallAuthenticate()
    {
        // once encryption is availble, the client should send one (secure) authenticate. it includes the AppId (which identifies your app on the Photon Cloud)
        AuthenticationValues auth = this.AuthValues ?? new AuthenticationValues() { UserId = this.PlayerName };
        if (this.AuthMode == AuthModeOption.Auth)
        {
            return this.OpAuthenticate(this.AppId, this.AppVersion, auth, this.CloudRegion.ToString(), this.requestLobbyStatistics);
        }
        else
        {
            return this.OpAuthenticateOnce(this.AppId, this.AppVersion, auth, this.CloudRegion.ToString(), this.EncryptionMode, PhotonNetwork.PhotonServerSettings.Protocol);
        }
    }



    /// <summary>
    /// Internally used only. Triggers OnStateChange with "Disconnect" in next dispatch which is the signal to re-connect (if at all).
    /// </summary>
    private void DisconnectToReconnect()
    {
        switch (this.Server)
        {
            case ServerConnection.NameServer:
                this.State = ClientState.DisconnectingFromNameServer;
                base.Disconnect();
                break;
            case ServerConnection.MasterServer:
                this.State = ClientState.DisconnectingFromMasterserver;
                base.Disconnect();
                //LeftLobbyCleanup();
                break;
            case ServerConnection.GameServer:
                this.State = ClientState.DisconnectingFromGameserver;
                base.Disconnect();
                //this.LeftRoomCleanup();
                break;
        }
    }

    /// <summary>
    /// While on the NameServer, this gets you the list of regional servers (short names and their IPs to ping them).
    /// </summary>
    /// <returns>If the operation could be sent. If false, no operation was sent (e.g. while not connected to the NameServer).</returns>
    public bool GetRegions()
    {
        if (this.Server != ServerConnection.NameServer)
        {
            return false;
        }

        bool sent = this.OpGetRegions(this.AppId);
        if (sent)
        {
            this.AvailableRegions = null;
        }

        return sent;
    }

    /// <summary>
    /// Request the rooms and online status for a list of friends. All client must set a unique username via PlayerName property. The result is available in this.Friends.
    /// </summary>
    /// <remarks>
    /// Used on Master Server to find the rooms played by a selected list of users.
    /// The result will be mapped to LoadBalancingClient.Friends when available.
    /// The list is initialized by OpFindFriends on first use (before that, it is null).
    ///
    /// Users identify themselves by setting a PlayerName in the LoadBalancingClient instance.
    /// This in turn will send the name in OpAuthenticate after each connect (to master and game servers).
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
    /// <returns>If the operation could be sent (requires connection, only one request is allowed at any time). Always false in offline mode.</returns>
    public override bool OpFindFriends(string[] friendsToFind)
    {
        if (this.isFetchingFriendList)
        {
            return false;   // fetching friends currently, so don't do it again (avoid changing the list while fetching friends)
        }

        this.friendListRequested = friendsToFind;
        this.isFetchingFriendList = true;

        return base.OpFindFriends(friendsToFind);
    }

    /// <summary>NetworkingPeer.OpCreateGame</summary>
    public bool OpCreateGame(EnterRoomParams enterRoomParams)
    {
        bool onGameServer = this.Server == ServerConnection.GameServer;
        enterRoomParams.OnGameServer = onGameServer;
        enterRoomParams.PlayerProperties = GetLocalActorProperties();
        if (!onGameServer)
        {
            enterRoomParamsCache = enterRoomParams;
        }

        this.lastJoinType = JoinType.CreateRoom;
        return base.OpCreateRoom(enterRoomParams);
    }

    /// <summary>NetworkingPeer.OpJoinRoom</summary>
    public override bool OpJoinRoom(EnterRoomParams opParams)
    {
        bool onGameServer = this.Server == ServerConnection.GameServer;
        opParams.OnGameServer = onGameServer;
        if (!onGameServer)
        {
            this.enterRoomParamsCache = opParams;
        }

        this.lastJoinType = (opParams.CreateIfNotExists) ? JoinType.JoinOrCreateRoom : JoinType.JoinRoom;
        return base.OpJoinRoom(opParams);
    }

    /// <summary>NetworkingPeer.OpJoinRandomRoom</summary>
    /// <remarks>this override just makes sure we have a mRoomToGetInto, even if it's blank (the properties provided in this method are filters. they are not set when we join the game)</remarks>
    public override bool OpJoinRandomRoom(OpJoinRandomRoomParams opJoinRandomRoomParams)
    {
        enterRoomParamsCache = new EnterRoomParams();   // this is used when the client arrives on the GS and joins the room
        enterRoomParamsCache.Lobby = opJoinRandomRoomParams.TypedLobby;

        this.lastJoinType = JoinType.JoinRandomRoom;
        return base.OpJoinRandomRoom(opJoinRandomRoomParams);
    }

    /// <summary>
    /// Operation Leave will exit any current room.
    /// </summary>
    /// <remarks>
    /// This also happens when you disconnect from the server.
    /// Disconnect might be a step less if you don't want to create a new room on the same server.
    /// </remarks>
    /// <returns></returns>
    public virtual bool OpLeave()
    {
        if (this.State != ClientState.Joined)
        {
            Debug.LogWarning("Not sending leave operation. State is not 'Joined': " + this.State);
            return false;
        }

        return this.OpCustom((byte)OperationCode.Leave, null, true, 0);
    }

    public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable, RaiseEventOptions raiseEventOptions)
    {
        if (PhotonNetwork.offlineMode)
        {
            return false;
        }

        return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, raiseEventOptions);
    }

    #endregion

    #region Helpers

    private void ReadoutProperties(Hashtable gameProperties, Hashtable pActorProperties, int targetActorNr)
    {
        // Debug.LogWarning("ReadoutProperties gameProperties: " + gameProperties.ToStringFull() + " pActorProperties: " + pActorProperties.ToStringFull() + " targetActorNr: " + targetActorNr);

        // read per-player properties (or those of one target player) and cache those locally
        if (pActorProperties != null && pActorProperties.Count > 0)
        {
            if (targetActorNr > 0)
            {
                // we have a single entry in the pActorProperties with one
                // user's name
                // targets MUST exist before you set properties
                PhotonPlayer target = this.GetPlayerWithId(targetActorNr);
                if (target != null)
                {
                    Hashtable props = this.ReadoutPropertiesForActorNr(pActorProperties, targetActorNr);
                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
            else
            {
                // in this case, we've got a key-value pair per actor (each
                // value is a hashtable with the actor's properties then)
                int actorNr;
                Hashtable props;
                string newName;
                PhotonPlayer target;

                foreach (object key in pActorProperties.Keys)
                {
                    actorNr = (int)key;
                    props = (Hashtable)pActorProperties[key];
                    newName = (string)props[ActorProperties.PlayerName];

                    target = this.GetPlayerWithId(actorNr);
                    if (target == null)
                    {
                        target = new PhotonPlayer(false, actorNr, newName);
                        this.AddNewPlayer(actorNr, target);
                    }

                    target.InternalCacheProperties(props);
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerPropertiesChanged, target, props);
                }
            }
        }

        // read game properties and cache them locally
        if (this.CurrentRoom != null && gameProperties != null)
        {
            this.CurrentRoom.InternalCacheProperties(gameProperties);
            SendMonoMessage(PhotonNetworkingMessage.OnPhotonCustomRoomPropertiesChanged, gameProperties);
            if (PhotonNetwork.automaticallySyncScene)
            {
                this.LoadLevelIfSynced();   // will load new scene if sceneName was changed
            }
        }
    }

    private Hashtable ReadoutPropertiesForActorNr(Hashtable actorProperties, int actorNr)
    {
        if (actorProperties.ContainsKey(actorNr))
        {
            return (Hashtable)actorProperties[actorNr];
        }

        return actorProperties;
    }

    public void ChangeLocalID(int newID)
    {
        if (this.LocalPlayer == null)
        {
            Debug.LogWarning(
                string.Format(
                    "LocalPlayer is null or not in mActors! LocalPlayer: {0} mActors==null: {1} newID: {2}",
                    this.LocalPlayer,
                    this.mActors == null,
                    newID));
        }

        if (this.mActors.ContainsKey(this.LocalPlayer.ID))
        {
            this.mActors.Remove(this.LocalPlayer.ID);
        }

        this.LocalPlayer.InternalChangeLocalID(newID);
        this.mActors[this.LocalPlayer.ID] = this.LocalPlayer;
        this.RebuildPlayerListCopies();
    }

    /// <summary>
    /// Called at disconnect/leavelobby etc. This CAN also be called when we are not in a lobby (e.g. disconnect from room)
    /// </summary>
    /// <remarks>Calls callback method OnLeftLobby if this client was in a lobby initially. Clears the lobby's game lists.</remarks>
    private void LeftLobbyCleanup()
    {
        this.mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];

        if (this.insideLobby)
        {
            this.insideLobby = false;
            SendMonoMessage(PhotonNetworkingMessage.OnLeftLobby);
        }
    }

    /// <summary>
    /// Called when "this client" left a room to clean up.
    /// </summary>
    private void LeftRoomCleanup()
    {
        bool wasInRoom = this.CurrentRoom != null;
        // when leaving a room, we clean up depending on that room's settings.
        bool autoCleanupSettingOfRoom = (this.CurrentRoom != null) ? this.CurrentRoom.autoCleanUp : PhotonNetwork.autoCleanUpPlayerObjects;

        this.hasSwitchedMC = false;
        this.CurrentRoom = null;
        this.mActors = new Dictionary<int, PhotonPlayer>();
        this.mPlayerListCopy = new PhotonPlayer[0];
        this.mOtherPlayerListCopy = new PhotonPlayer[0];
        this.allowedReceivingGroups = new HashSet<int>();
        this.blockSendingGroups = new HashSet<int>();
        this.mGameList = new Dictionary<string, RoomInfo>();
        this.mGameListCopy = new RoomInfo[0];
        this.isFetchingFriendList = false;

        this.ChangeLocalID(-1);

        // Cleanup all network objects (all spawned PhotonViews, local and remote)
        if (autoCleanupSettingOfRoom)
        {
            this.LocalCleanupAnythingInstantiated(true);
            PhotonNetwork.manuallyAllocatedViewIds = new List<int>();       // filled and easier to replace completely
        }

        if (wasInRoom)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
        }
    }

    /// <summary>
    /// Cleans up anything that was instantiated in-game (not loaded with the scene).
    /// </summary>
    protected internal void LocalCleanupAnythingInstantiated(bool destroyInstantiatedGameObjects)
    {
        if (this.tempInstantiationData.Count > 0)
        {
            Debug.LogWarning("It seems some instantiation is not completed, as instantiation data is used. You should make sure instantiations are paused when calling this method. Cleaning now, despite this.");
        }

        // Destroy GO's (if we should)
        if (destroyInstantiatedGameObjects)
        {
            // Fill list with Instantiated objects
            HashSet<GameObject> instantiatedGos = new HashSet<GameObject>();
            foreach (PhotonView view in this.photonViewList.Values)
            {
                if (view.isRuntimeInstantiated)
                {
                    instantiatedGos.Add(view.gameObject); // HashSet keeps each object only once
                }
            }

            foreach (GameObject go in instantiatedGos)
            {
                this.RemoveInstantiatedGO(go, true);
            }
        }

        // photonViewList is cleared of anything instantiated (so scene items are left inside)
        // any other lists can be
        this.tempInstantiationData.Clear(); // should be empty but to be safe we clear (no new list needed)
        PhotonNetwork.lastUsedViewSubId = 0;
        PhotonNetwork.lastUsedViewSubIdStatic = 0;
    }


    private void GameEnteredOnGameServer(OperationResponse operationResponse)
    {
        if (operationResponse.ReturnCode != 0)
        {
            switch (operationResponse.OperationCode)
            {
                case OperationCode.CreateGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Create failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                case OperationCode.JoinRandomGame:
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.Log("Join failed on GameServer. Changing back to MasterServer. Msg: " + operationResponse.DebugMessage);
                        if (operationResponse.ReturnCode == ErrorCode.GameDoesNotExist)
                        {
                            Debug.Log("Most likely the game became empty during the switch to GameServer.");
                        }
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
            }

            this.DisconnectToReconnect();
            return;
        }

        Room current = new Room(this.enterRoomParamsCache.RoomName, null);
        current.isLocalClientInside = true;
        this.CurrentRoom = current;

        this.State = ClientState.Joined;

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

        if (!this.CurrentRoom.serverSideMasterClient) this.CheckMasterClient(-1);

        if (this.mPlayernameHasToBeUpdated)
        {
            this.SendPlayerName();
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.CreateGame:
                SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                break;
            case OperationCode.JoinGame:
            case OperationCode.JoinRandomGame:
                // the mono message for this is sent at another place
                break;
        }
    }

    private void AddNewPlayer(int ID, PhotonPlayer player)
    {
        if (!this.mActors.ContainsKey(ID))
        {
            this.mActors[ID] = player;
            RebuildPlayerListCopies();
        }
        else
        {
            Debug.LogError("Adding player twice: " + ID);
        }
    }

    void RemovePlayer(int ID, PhotonPlayer player)
    {
        this.mActors.Remove(ID);
        if (!player.isLocal)
        {
            RebuildPlayerListCopies();
        }
    }

    void RebuildPlayerListCopies()
    {
        this.mPlayerListCopy = new PhotonPlayer[this.mActors.Count];
        this.mActors.Values.CopyTo(this.mPlayerListCopy, 0);

        List<PhotonPlayer> otherP = new List<PhotonPlayer>();
        for (int i = 0; i < this.mPlayerListCopy.Length; i++)
        {
            PhotonPlayer player = this.mPlayerListCopy[i];
            if (!player.isLocal)
            {
                otherP.Add(player);
            }
        }

        this.mOtherPlayerListCopy = otherP.ToArray();
    }

    /// <summary>
    /// Resets the PhotonView "lastOnSerializeDataSent" so that "OnReliable" synched PhotonViews send a complete state to new clients (if the state doesnt change, no messages would be send otherwise!).
    /// Note that due to this reset, ALL other players will receive the full OnSerialize.
    /// </summary>
    private void ResetPhotonViewsOnSerialize()
    {
        foreach (PhotonView photonView in this.photonViewList.Values)
        {
            photonView.lastOnSerializeDataSent = null;
        }
    }

    /// <summary>
    /// Called when the event Leave (of some other player) arrived.
    /// Cleans game objects, views locally. The master will also clean the
    /// </summary>
    /// <param name="actorID">ID of player who left.</param>
    private void HandleEventLeave(int actorID, EventData evLeave)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log("HandleEventLeave for player ID: " + actorID + " evLeave: " + evLeave.ToStringFull());


        // actorNr is fetched out of event
        PhotonPlayer player = this.GetPlayerWithId(actorID);
        if (player == null)
        {
            Debug.LogError(String.Format("Received event Leave for unknown player ID: {0}", actorID));
            return;
        }

		bool _isAlreadyInactive =  player.isInactive;

        if (evLeave.Parameters.ContainsKey(ParameterCode.IsInactive))
        {
            // player becomes inactive (but might return / is not gone for good)
            player.isInactive = (bool)evLeave.Parameters[ParameterCode.IsInactive];
			if (player.isInactive && _isAlreadyInactive)
            {
                Debug.LogWarning("HandleEventLeave for player ID: " + actorID + " isInactive: " + player.isInactive + ". Stopping handling if inactive.");
                return;
            }
        }

		// having a new master before calling destroy for the leaving player is important!
		// so we elect a new masterclient and ignore the leaving player (who is still in playerlists).
		// note: there is/was a server-side-error which sent 0 as new master instead of skipping the key/value. below is a check for 0 due to that
		if (evLeave.Parameters.ContainsKey(ParameterCode.MasterClientId))
		{
			int newMaster = (int) evLeave[ParameterCode.MasterClientId];
			if (newMaster != 0)
			{
				this.mMasterClientId = (int)evLeave[ParameterCode.MasterClientId];
				this.UpdateMasterClient();
			}
		}
		else if (!this.CurrentRoom.serverSideMasterClient)
		{
			this.CheckMasterClient(actorID);
		}


		// we let the player up if inactive but if we were already inactive, then we have to actually remove the player properly.
		if (player.isInactive && !_isAlreadyInactive)
		{
			return;
		}

        // destroy objects & buffered messages
        if (this.CurrentRoom != null && this.CurrentRoom.autoCleanUp)
        {
            this.DestroyPlayerObjects(actorID, true);
        }

        RemovePlayer(actorID, player);

        // finally, send notification (the playerList and masterclient are now updated)
        SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerDisconnected, player);
    }

    /// <summary>Picks the new master client from player list, if the current Master is leaving (leavingPlayerId) or if no master was assigned so far.</summary>
    /// <param name="leavingPlayerId">
    /// The ignored player is the one who's leaving and should not become master (again). Pass -1 to select any player from the list.
    /// </param>
    private void CheckMasterClient(int leavingPlayerId)
    {
        bool currentMasterIsLeaving = this.mMasterClientId == leavingPlayerId;
        bool someoneIsLeaving = leavingPlayerId > 0;

        // return early if SOME player (leavingId > 0) is leaving AND it's NOT the current master
        if (someoneIsLeaving && !currentMasterIsLeaving)
        {
            return;
        }

        // picking the player with lowest ID (longest in game).
        int lowestActorNumber;
        if (this.mActors.Count <= 1)
        {
            lowestActorNumber = this.LocalPlayer.ID;
        }
        else
        {
            // keys in mActors are their actorNumbers
            lowestActorNumber = Int32.MaxValue;
            foreach (int key in this.mActors.Keys)
            {
                if (key < lowestActorNumber && key != leavingPlayerId)
                {
                    lowestActorNumber = key;
                }
            }
        }
        this.mMasterClientId = lowestActorNumber;

        // callback ONLY when the current master left
        if (someoneIsLeaving)
        {
            SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.GetPlayerWithId(lowestActorNumber));
        }
    }

    /// <summary>Call when the server provides a MasterClientId (due to joining or the current MC leaving, etc).</summary>
    internal protected void UpdateMasterClient()
    {
        SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, PhotonNetwork.masterClient);
    }

    private static int ReturnLowestPlayerId(PhotonPlayer[] players, int playerIdToIgnore)
    {
        if (players == null || players.Length == 0)
        {
            return -1;
        }

        int lowestActorNumber = Int32.MaxValue;
        for (int i = 0; i < players.Length; i++)
        {
            PhotonPlayer photonPlayer = players[i];
            if (photonPlayer.ID == playerIdToIgnore)
            {
                continue;
            }

            if (photonPlayer.ID < lowestActorNumber)
            {
                lowestActorNumber = photonPlayer.ID;
            }
        }

        return lowestActorNumber;
    }

    /// <summary>Fake-sets a new Master Client for this room via RaiseEvent.</summary>
    /// <remarks>Does not affect RaiseEvent with target MasterClient but RPC().</remarks>
    internal protected bool SetMasterClient(int playerId, bool sync)
    {
        bool masterReplaced = this.mMasterClientId != playerId;
        if (!masterReplaced || !this.mActors.ContainsKey(playerId))
        {
            return false;
        }

        if (sync)
        {
            bool sent = this.OpRaiseEvent(PunEvent.AssignMaster, new Hashtable() { { (byte)1, playerId } }, true, null);
            if (!sent)
            {
                return false;
            }
        }

        this.hasSwitchedMC = true;
        this.CurrentRoom.masterClientId = playerId;
        SendMonoMessage(PhotonNetworkingMessage.OnMasterClientSwitched, this.GetPlayerWithId(playerId));    // we only callback when an actual change is done
        return true;
    }

    /// <summary>Uses a well-known property to set someone new as Master Client in room (requires "Server Side Master Client" feature).</summary>
    public bool SetMasterClient(int nextMasterId)
    {
        Hashtable newProps = new Hashtable() { { GamePropertyKey.MasterClientId, nextMasterId } };
        Hashtable prevProps = new Hashtable() { { GamePropertyKey.MasterClientId, this.mMasterClientId } };
        return this.OpSetPropertiesOfRoom(newProps, expectedProperties: prevProps, webForward: false);
    }

    protected internal PhotonPlayer GetPlayerWithId(int number)
    {
        if (this.mActors == null) return null;

        PhotonPlayer player = null;
        this.mActors.TryGetValue(number, out player);
        return player;
    }

    private void SendPlayerName()
    {
        if (this.State == ClientState.Joining)
        {
            // this means, the join on the gameServer is sent (with an outdated name). send the new when in game
            this.mPlayernameHasToBeUpdated = true;
            return;
        }

        if (this.LocalPlayer != null)
        {
            this.LocalPlayer.name = this.PlayerName;
            Hashtable properties = new Hashtable();
            properties[ActorProperties.PlayerName] = this.PlayerName;
            if (this.LocalPlayer.ID > 0)
            {
                this.OpSetPropertiesOfActor(this.LocalPlayer.ID, properties, null);
                this.mPlayernameHasToBeUpdated = false;
            }
        }
    }

    private Hashtable GetLocalActorProperties()
    {
        if (PhotonNetwork.player != null)
        {
            return PhotonNetwork.player.allProperties;
        }

        Hashtable actorProperties = new Hashtable();
        actorProperties[ActorProperties.PlayerName] = this.PlayerName;
        return actorProperties;
    }

    #endregion

    #region Implementation of IPhotonPeerListener

    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else if (level == DebugLevel.INFO && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
        {
            Debug.Log(message);
        }
        else if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
        {
            Debug.Log(message);
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        if (PhotonNetwork.networkingPeer.State == ClientState.Disconnecting)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            {
                Debug.Log("OperationResponse ignored while disconnecting. Code: " + operationResponse.OperationCode);
            }
            return;
        }

        // extra logging for error debugging (helping developers with a bit of automated analysis)
        if (operationResponse.ReturnCode == 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log(operationResponse.ToString());
        }
        else
        {
            if (operationResponse.ReturnCode == ErrorCode.OperationNotAllowedInCurrentState)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " could not be executed (yet). Wait for state JoinedLobby or ConnectedToMaster and their callbacks before calling operations. WebRPCs need a server-side configuration. Enum OperationCode helps identify the operation.");
            }
            else if (operationResponse.ReturnCode == ErrorCode.PluginReportedError)
            {
                Debug.LogError("Operation " + operationResponse.OperationCode + " failed in a server-side plugin. Check the configuration in the Dashboard. Message from server-plugin: " + operationResponse.DebugMessage);
            }
            else if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
            {
                Debug.LogWarning("Operation failed: " + operationResponse.ToStringFull());
            }
            else
            {
                Debug.LogError("Operation failed: " + operationResponse.ToStringFull() + " Server: " + this.Server);
            }
        }

        // use the "secret" or "token" whenever we get it. doesn't really matter if it's in AuthResponse.
        if (operationResponse.Parameters.ContainsKey(ParameterCode.Secret))
        {
            if (this.AuthValues == null)
            {
                this.AuthValues = new AuthenticationValues();
                // this.DebugReturn(DebugLevel.ERROR, "Server returned secret. Created AuthValues.");
            }

            this.AuthValues.Token = operationResponse[ParameterCode.Secret] as string;
            this.tokenCache = this.AuthValues.Token;
        }

        switch (operationResponse.OperationCode)
        {
            case OperationCode.Authenticate:
            case OperationCode.AuthenticateOnce:
            {
                // ClientState oldState = this.State;

                if (operationResponse.ReturnCode != 0)
                {
                    if (operationResponse.ReturnCode == ErrorCode.InvalidOperation)
                    {
                        Debug.LogError(string.Format("If you host Photon yourself, make sure to start the 'Instance LoadBalancing' "+ this.ServerAddress));
                    }
                    else if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                    {
                        Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                        SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);
                    }
                    else if (operationResponse.ReturnCode == ErrorCode.CustomAuthenticationFailed)
                    {
                        Debug.LogError(string.Format("Custom Authentication failed (either due to user-input or configuration or AuthParameter string format). Calling: OnCustomAuthenticationFailed()"));
                        SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationFailed, operationResponse.DebugMessage);
                    }
                    else
                    {
                        Debug.LogError(string.Format("Authentication failed: '{0}' Code: {1}", operationResponse.DebugMessage, operationResponse.ReturnCode));
                    }

                    this.State = ClientState.Disconnecting;
                    this.Disconnect();

                    if (operationResponse.ReturnCode == ErrorCode.MaxCcuReached)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            Debug.LogWarning(string.Format("Currently, the limit of users is reached for this title. Try again later. Disconnecting"));
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonMaxCccuReached);
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.MaxCcuReached);
                    }
                    else if (operationResponse.ReturnCode == ErrorCode.InvalidRegion)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            Debug.LogError(string.Format("The used master server address is not available with the subscription currently used. Got to Photon Cloud Dashboard or change URL. Disconnecting."));
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.InvalidRegion);
                    }
                    else if (operationResponse.ReturnCode == ErrorCode.AuthenticationTicketExpired)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            Debug.LogError(string.Format("The authentication ticket expired. You need to connect (and authenticate) again. Disconnecting."));
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, DisconnectCause.AuthenticationTicketExpired);
                    }
                    break;
                }
                else
                {
                    // successful connect/auth. depending on the used server, do next steps:

                    if (this.Server == ServerConnection.NameServer || this.Server == ServerConnection.MasterServer)
                    {
                        if (operationResponse.Parameters.ContainsKey(ParameterCode.UserId))
                        {
                            string incomingId = (string)operationResponse.Parameters[ParameterCode.UserId];
                            if (!string.IsNullOrEmpty(incomingId))
                            {
                                if (this.AuthValues == null)
                                {
                                    this.AuthValues = new AuthenticationValues();
                                }
                                this.AuthValues.UserId = incomingId;
                                PhotonNetwork.player.userId = incomingId;

                                if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                                {
                                    this.DebugReturn(DebugLevel.INFO, string.Format("Received your UserID from server. Updating local value to: {0}", incomingId));
                                }
                            }
                        }
                        if (operationResponse.Parameters.ContainsKey(ParameterCode.NickName))
                        {
                            this.playername = (string)operationResponse.Parameters[ParameterCode.NickName];
                            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            {
                                this.DebugReturn(DebugLevel.INFO, string.Format("Received your NickName from server. Updating local value to: {0}", this.playername));
                            }
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
                        this.DisconnectToReconnect();
                    }
                    else if (this.Server == ServerConnection.MasterServer)
                    {
                        if (this.AuthMode != AuthModeOption.Auth)
                        {
                            this.OpSettings(this.requestLobbyStatistics);
                        }
                        if (PhotonNetwork.autoJoinLobby)
                        {
                            this.State = ClientState.Authenticated;
                            this.OpJoinLobby(this.lobby);
                        }
                        else
                        {
                            this.State = ClientState.ConnectedToMaster;
                            SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
                        }
                    }
                    else if (this.Server == ServerConnection.GameServer)
                    {
                        this.State = ClientState.Joining;
                        this.enterRoomParamsCache.PlayerProperties = GetLocalActorProperties();
                        this.enterRoomParamsCache.OnGameServer = true;

                        if (this.lastJoinType == JoinType.JoinRoom || this.lastJoinType == JoinType.JoinRandomRoom || this.lastJoinType == JoinType.JoinOrCreateRoom)
                        {
                            // if we just "join" the game, do so. if we wanted to "create the room on demand", we have to send this to the game server as well.
                            this.OpJoinRoom(this.enterRoomParamsCache);
                        }
                        else if (this.lastJoinType == JoinType.CreateRoom)
                        {
                            this.OpCreateGame(this.enterRoomParamsCache);
                        }
                    }

                    if (operationResponse.Parameters.ContainsKey(ParameterCode.Data))
                    {
                        // optionally, OpAuth may return some data for the client to use. if it's available, call OnCustomAuthenticationResponse
                        Dictionary<string, object> data = (Dictionary<string, object>)operationResponse.Parameters[ParameterCode.Data];
                        if (data != null)
                        {
                            SendMonoMessage(PhotonNetworkingMessage.OnCustomAuthenticationResponse, data);
                        }
                    }
                }
                break;
            }

            case OperationCode.GetRegions:
                // Debug.Log("GetRegions returned: " + operationResponse.ToStringFull());

                if (operationResponse.ReturnCode == ErrorCode.InvalidAuthentication)
                {
                    Debug.LogError(string.Format("The appId this client sent is unknown on the server (Cloud). Check settings. If using the Cloud, check account."));
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, DisconnectCause.InvalidAuthentication);

                    this.State = ClientState.Disconnecting;
                    this.Disconnect();
                    break;
                }
                if (operationResponse.ReturnCode != ErrorCode.Ok)
                {
                    Debug.LogError("GetRegions failed. Can't provide regions list. Error: " + operationResponse.ReturnCode + ": " + operationResponse.DebugMessage);
                    break;
                }

                string[] regions = operationResponse[ParameterCode.Region] as string[];
                string[] servers = operationResponse[ParameterCode.Address] as string[];
                if (regions == null || servers == null || regions.Length != servers.Length)
                {
                    Debug.LogError("The region arrays from Name Server are not ok. Must be non-null and same length. " + (regions ==null)+ " " + (servers==null) + "\n"+operationResponse.ToStringFull());
                    break;
                }

                this.AvailableRegions = new List<Region>(regions.Length);
                for (int i = 0; i < regions.Length; i++)
                {
                    string regionCodeString = regions[i];
                    if (string.IsNullOrEmpty(regionCodeString))
                    {
                        continue;
                    }
                    regionCodeString = regionCodeString.ToLower();
                    CloudRegionCode code = Region.Parse(regionCodeString);

                    // check if enabled (or ignored by PhotonServerSettings.EnabledRegions)
                    bool enabledRegion = true;
                    if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion && PhotonNetwork.PhotonServerSettings.EnabledRegions != 0)
                    {
                        CloudRegionFlag flag = Region.ParseFlag(regionCodeString);
                        enabledRegion = ((PhotonNetwork.PhotonServerSettings.EnabledRegions & flag) != 0);
                        if (!enabledRegion && PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                        {
                            Debug.Log("Skipping region because it's not in PhotonServerSettings.EnabledRegions: " + code);
                        }
                    }
                    if (enabledRegion) this.AvailableRegions.Add(new Region() { Code = code, HostAndPort = servers[i] });
                }

                // PUN assumes you fetch the name-server's list of regions to ping them
                if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
                {
                    PhotonHandler.PingAvailableRegionsAndConnectToBest();
                }
                break;

            case OperationCode.CreateGame:
            {
                if (this.Server == ServerConnection.GameServer)
                {
                    this.GameEnteredOnGameServer(operationResponse);
                }
                else
                {
                    if (operationResponse.ReturnCode != 0)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            Debug.LogWarning(string.Format("CreateRoom failed, client stays on masterserver: {0}.", operationResponse.ToStringFull()));

                        this.State = (this.insideLobby) ? ClientState.JoinedLobby : ClientState.ConnectedToMaster;
                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonCreateRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                        break;
                    }

                    string gameID = (string) operationResponse[ParameterCode.RoomName];
                    if (!string.IsNullOrEmpty(gameID))
                    {
                        // is only sent by the server's response, if it has not been
                        // sent with the client's request before!
                        this.enterRoomParamsCache.RoomName = gameID;
                    }

                    this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                    this.DisconnectToReconnect();
                }

                break;
            }

            case OperationCode.JoinGame:
            {
                if (this.Server != ServerConnection.GameServer)
                {
                    if (operationResponse.ReturnCode != 0)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                            Debug.Log(string.Format("JoinRoom failed (room maybe closed by now). Client stays on masterserver: {0}. State: {1}", operationResponse.ToStringFull(), this.State));

                        SendMonoMessage(PhotonNetworkingMessage.OnPhotonJoinRoomFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                        break;
                    }

                    this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                    this.DisconnectToReconnect();
                }
                else
                {
                    this.GameEnteredOnGameServer(operationResponse);
                }

                break;
            }

            case OperationCode.JoinRandomGame:
            {
                // happens only on master. on gameserver, this is a regular join (we don't need to find a random game again)
                // the operation OpJoinRandom either fails (with returncode 8) or returns game-to-join information
                if (operationResponse.ReturnCode != 0)
                {
                    if (operationResponse.ReturnCode == ErrorCode.NoRandomMatchFound)
                    {
                        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                            Debug.Log("JoinRandom failed: No open game. Calling: OnPhotonRandomJoinFailed() and staying on master server.");
                    }
                    else if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                    {
                        Debug.LogWarning(string.Format("JoinRandom failed: {0}.", operationResponse.ToStringFull()));
                    }

                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonRandomJoinFailed, operationResponse.ReturnCode, operationResponse.DebugMessage);
                    break;
                }

                string roomName = (string)operationResponse[ParameterCode.RoomName];
                this.enterRoomParamsCache.RoomName = roomName;
                this.GameServerAddress = (string)operationResponse[ParameterCode.Address];
                this.DisconnectToReconnect();
                break;
            }

            case OperationCode.JoinLobby:
                this.State = ClientState.JoinedLobby;
                this.insideLobby = true;
                SendMonoMessage(PhotonNetworkingMessage.OnJoinedLobby);

                // this.mListener.joinLobbyReturn();
                break;
            case OperationCode.LeaveLobby:
                this.State = ClientState.Authenticated;
                this.LeftLobbyCleanup();    // will set insideLobby = false
                break;

            case OperationCode.Leave:
                this.DisconnectToReconnect();
                break;

            case OperationCode.SetProperties:
                // this.mListener.setPropertiesReturn(returnCode, debugMsg);
                break;

            case OperationCode.GetProperties:
            {
                Hashtable actorProperties = (Hashtable)operationResponse[ParameterCode.PlayerProperties];
                Hashtable gameProperties = (Hashtable)operationResponse[ParameterCode.GameProperties];
                this.ReadoutProperties(gameProperties, actorProperties, 0);

                // RemoveByteTypedPropertyKeys(actorProperties, false);
                // RemoveByteTypedPropertyKeys(gameProperties, false);
                // this.mListener.getPropertiesReturn(gameProperties, actorProperties, returnCode, debugMsg);
                break;
            }

            case OperationCode.RaiseEvent:
                // this usually doesn't give us a result. only if the caching is affected the server will send one.
                break;

            case OperationCode.FindFriends:
                bool[] onlineList = operationResponse[ParameterCode.FindFriendsResponseOnlineList] as bool[];
                string[] roomList = operationResponse[ParameterCode.FindFriendsResponseRoomIdList] as string[];

                if (onlineList != null && roomList != null && this.friendListRequested != null && onlineList.Length == this.friendListRequested.Length)
                {
                    List<FriendInfo> friendList = new List<FriendInfo>(this.friendListRequested.Length);
                    for (int index = 0; index < this.friendListRequested.Length; index++)
                    {
                        FriendInfo friend = new FriendInfo();
                        friend.Name = this.friendListRequested[index];
                        friend.Room = roomList[index];
                        friend.IsOnline = onlineList[index];
                        friendList.Insert(index, friend);
                    }
                    PhotonNetwork.Friends = friendList;
                }
                else
                {
                    // any of the lists is null and shouldn't. print a error
                    Debug.LogError("FindFriends failed to apply the result, as a required value wasn't provided or the friend list length differed from result.");
                }

                this.friendListRequested = null;
                this.isFetchingFriendList = false;
                this.friendListTimestamp = Environment.TickCount;
                if (this.friendListTimestamp == 0)
                {
                    this.friendListTimestamp = 1;   // makes sure the timestamp is not accidentally 0
                }

                SendMonoMessage(PhotonNetworkingMessage.OnUpdatedFriendList);
                break;

            case OperationCode.WebRpc:
                SendMonoMessage(PhotonNetworkingMessage.OnWebRpcResponse, operationResponse);
                break;

            default:
                Debug.LogWarning(string.Format("OperationResponse unhandled: {0}", operationResponse.ToString()));
                break;
        }

        //this.externalListener.OnOperationResponse(operationResponse);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnStatusChanged: {0} current State: {1}", statusCode.ToString(), this.State));

        switch (statusCode)
        {
            case StatusCode.Connect:
                if (this.State == ClientState.ConnectingToNameServer)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to NameServer.");

                    this.Server = ServerConnection.NameServer;
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null;     // when connecting to NameServer, invalidate any auth values
                    }
                }

                if (this.State == ClientState.ConnectingToGameserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to gameserver.");

                    this.Server = ServerConnection.GameServer;
                    this.State = ClientState.ConnectedToGameserver;
                }

                if (this.State == ClientState.ConnectingToMasterserver)
                {
                    if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
                        Debug.Log("Connected to masterserver.");

                    this.Server = ServerConnection.MasterServer;
                    this.State = ClientState.Authenticating;  // photon v4 always requires OpAuthenticate. even self-hosted Photon Server

                    if (this.IsInitialConnect)
                    {
                        this.IsInitialConnect = false;  // after handling potential initial-connect issues with special messages, we are now sure we can reach a server
                        SendMonoMessage(PhotonNetworkingMessage.OnConnectedToPhoton);
                    }
                }


                if (this.TransportProtocol != ConnectionProtocol.WebSocketSecure)
                {
                    if (this.Server == ServerConnection.NameServer || this.AuthMode == AuthModeOption.Auth)
                    {
                        this.EstablishEncryption();
                    }
                }
                else
                {
                    if (DebugOut == DebugLevel.INFO)
                    {
                        Debug.Log("Skipping EstablishEncryption. Protocol is secure.");
                    }

                    goto case StatusCode.EncryptionEstablished;
                }
                break;

            case StatusCode.EncryptionEstablished:
                // on nameserver, the "process" is stopped here, so the developer/game can either get regions or authenticate with a specific region
                if (this.Server == ServerConnection.NameServer)
                {
                    this.State = ClientState.ConnectedToNameServer;

                    if (!this.didAuthenticate && this.CloudRegion == CloudRegionCode.none)
                    {
                        // this client is not setup to connect to a default region. find out which regions there are!
                        this.OpGetRegions(this.AppId);
                    }
                }

                if (this.Server != ServerConnection.NameServer && (this.AuthMode == AuthModeOption.AuthOnce || this.AuthMode == AuthModeOption.AuthOnceWss))
                {
                    // AuthMode "Once" means we only authenticate on the NameServer
                    break;
                }


                // we might need to authenticate automatically now, so the client can do anything at all
                if (!this.didAuthenticate && (!this.IsUsingNameServer || this.CloudRegion !=  CloudRegionCode.none))
                {
                    this.didAuthenticate = this.CallAuthenticate();

                    if (this.didAuthenticate)
                    {
                        this.State = ClientState.Authenticating;
                    }
                }
                break;

            case StatusCode.EncryptionFailedToEstablish:
                Debug.LogError("Encryption wasn't established: " + statusCode + ". Going to authenticate anyways.");
                AuthenticationValues authV = this.AuthValues ?? new AuthenticationValues() { UserId = this.PlayerName };
                this.OpAuthenticate(this.AppId, this.AppVersion, authV, this.CloudRegion.ToString(), this.requestLobbyStatistics);     // TODO: check if there are alternatives
                break;

            case StatusCode.Disconnect:
                this.didAuthenticate = false;
                this.isFetchingFriendList = false;
                if (this.Server == ServerConnection.GameServer) this.LeftRoomCleanup();
                if (this.Server == ServerConnection.MasterServer) this.LeftLobbyCleanup();

                if (this.State == ClientState.DisconnectingFromMasterserver)
                {
                    if (this.Connect(this.GameServerAddress, ServerConnection.GameServer))
                    {
                        this.State = ClientState.ConnectingToGameserver;
                    }
                }
                else if (this.State == ClientState.DisconnectingFromGameserver || this.State == ClientState.DisconnectingFromNameServer)
                {
                    this.SetupProtocol(ServerConnection.MasterServer);
                    if (this.Connect(this.MasterServerAddress, ServerConnection.MasterServer))
                    {
                        this.State = ClientState.ConnectingToMasterserver;
                    }
                }
                else
                {
                    if (this.AuthValues != null)
                    {
                        this.AuthValues.Token = null;  // invalidate any custom auth secrets
                    }

                    this.State = ClientState.PeerCreated; // if we set another state here, we could keep clients from connecting in OnDisconnectedFromPhoton right here.
                    SendMonoMessage(PhotonNetworkingMessage.OnDisconnectedFromPhoton);
                }
                break;

            case StatusCode.SecurityExceptionOnConnect:
            case StatusCode.ExceptionOnConnect:
                this.State = ClientState.PeerCreated;
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

                DisconnectCause cause = (DisconnectCause)statusCode;
                SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                break;

            case StatusCode.Exception:
                if (this.IsInitialConnect)
                {
                    Debug.LogError("Exception while connecting to: " + this.ServerAddress + ". Check if the server is available.");
                    if (this.ServerAddress == null || this.ServerAddress.StartsWith("127.0.0.1"))
                    {
                        Debug.LogWarning("The server address is 127.0.0.1 (localhost): Make sure the server is running on this machine. Android and iOS emulators have their own localhost.");
                        if (this.ServerAddress == this.GameServerAddress)
                        {
                            Debug.LogWarning("This might be a misconfiguration in the game server config. You need to edit it to a (public) address.");
                        }
                    }

                    this.State = ClientState.PeerCreated;
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    this.State = ClientState.PeerCreated;

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }

                this.Disconnect();
                break;

            case StatusCode.TimeoutDisconnect:
                if (this.IsInitialConnect)
                {
                    Debug.LogWarning(statusCode + " while connecting to: " + this.ServerAddress + ". Check if the server is available.");

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

				/* JF: we need this when reconnecting and joining.
                if (this.ServerAddress.Equals(this.GameServerAddress))
                {
                    this.GameServerAddress = null;
                }
                if (this.ServerAddress.Equals(this.MasterServerAddress))
                {
                    this.ServerAddress = null;
                }
                */

                this.Disconnect();
                break;

            case StatusCode.ExceptionOnReceive:
            case StatusCode.DisconnectByServer:
            case StatusCode.DisconnectByServerLogic:
            case StatusCode.DisconnectByServerUserLimit:
                if (this.IsInitialConnect)
                {
                    Debug.LogWarning(statusCode + " while connecting to: " + this.ServerAddress + ". Check if the server is available.");

                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnFailedToConnectToPhoton, cause);
                }
                else
                {
                    cause = (DisconnectCause)statusCode;
                    SendMonoMessage(PhotonNetworkingMessage.OnConnectionFail, cause);
                }
                if (this.AuthValues != null)
                {
                    this.AuthValues.Token = null;  // invalidate any custom auth secrets
                }

                this.Disconnect();
                break;

            case StatusCode.SendError:
                // this.mListener.clientErrorReturn(statusCode);
                break;

            case StatusCode.QueueOutgoingReliableWarning:
            case StatusCode.QueueOutgoingUnreliableWarning:
            case StatusCode.QueueOutgoingAcksWarning:
            case StatusCode.QueueSentWarning:
                // this.mListener.warningReturn(statusCode);
                break;

            case StatusCode.QueueIncomingReliableWarning:
            case StatusCode.QueueIncomingUnreliableWarning:
                Debug.Log(statusCode + ". This client buffers many incoming messages. This is OK temporarily. With lots of these warnings, check if you send too much or execute messages too slow. " + (PhotonNetwork.isMessageQueueRunning? "":"Your isMessageQueueRunning is false. This can cause the issue temporarily.") );
                break;

                // // TCP "routing" is an option of Photon that's not currently needed (or supported) by PUN
                //case StatusCode.TcpRouterResponseOk:
                //    break;
                //case StatusCode.TcpRouterResponseEndpointUnknown:
                //case StatusCode.TcpRouterResponseNodeIdUnknown:
                //case StatusCode.TcpRouterResponseNodeNotReady:

                //    this.DebugReturn(DebugLevel.ERROR, "Unexpected router response: " + statusCode);
                //    break;

            default:

                // this.mListener.serverErrorReturn(statusCode.value());
                Debug.LogError("Received unknown status code: " + statusCode);
                break;
        }

        //this.externalListener.OnStatusChanged(statusCode);
    }


    public void OnEvent(EventData photonEvent)
    {
        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            Debug.Log(string.Format("OnEvent: {0}", photonEvent.ToString()));

        int actorNr = -1;
        PhotonPlayer originatingPlayer = null;

        if (photonEvent.Parameters.ContainsKey(ParameterCode.ActorNr))
        {
            actorNr = (int)photonEvent[ParameterCode.ActorNr];
            originatingPlayer = this.GetPlayerWithId(actorNr);

            //else
            //{
            //    // the actor sending this event is not in actorlist. this is usually no problem
            //    if (photonEvent.Code != (byte)LiteOpCode.Join)
            //    {
            //        Debug.LogWarning("Received event, but we do not have this actor:  " + actorNr);
            //    }
            //}
        }

        switch (photonEvent.Code)
        {
            case PunEvent.OwnershipRequest:
            {
                int[] requestValues = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                int requestedViewId = requestValues[0];
                int currentOwner = requestValues[1];
                Debug.Log("Ev OwnershipRequest: " + photonEvent.Parameters.ToStringFull() + " ViewID: " + requestedViewId + " from: " + currentOwner + " Time: " + Environment.TickCount%1000);

                PhotonView requestedView = PhotonView.Find(requestedViewId);
                if (requestedView == null)
                {
                    Debug.LogWarning("Can't find PhotonView of incoming OwnershipRequest. ViewId not found: " + requestedViewId);
                    break;
                }

                Debug.Log("Ev OwnershipRequest PhotonView.ownershipTransfer: " + requestedView.ownershipTransfer + " .ownerId: " + requestedView.ownerId + " isOwnerActive: " + requestedView.isOwnerActive + ". This client's player: " + PhotonNetwork.player.ToStringFull());

                switch (requestedView.ownershipTransfer)
                {
                    case OwnershipOption.Fixed:
                        Debug.LogWarning("Ownership mode == fixed. Ignoring request.");
                        break;
                    case OwnershipOption.Takeover:
                        if (currentOwner == requestedView.ownerId)
                        {
                            // a takeover is successful automatically, if taken from current owner
                            requestedView.ownerId = actorNr;
                        }
                        break;
                    case OwnershipOption.Request:
                        if (currentOwner == PhotonNetwork.player.ID || PhotonNetwork.player.isMasterClient)
                        {
                            if ((requestedView.ownerId == PhotonNetwork.player.ID) || (PhotonNetwork.player.isMasterClient && !requestedView.isOwnerActive))
                            {
                                SendMonoMessage(PhotonNetworkingMessage.OnOwnershipRequest, new object[] {requestedView, originatingPlayer});
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
                break;

            case PunEvent.OwnershipTransfer:
                {
                    int[] transferViewToUserID = (int[]) photonEvent.Parameters[ParameterCode.CustomEventContent];
                    Debug.Log("Ev OwnershipTransfer. ViewID " + transferViewToUserID[0] + " to: " + transferViewToUserID[1] + " Time: " + Environment.TickCount%1000);

                    int requestedViewId = transferViewToUserID[0];
                    int newOwnerId = transferViewToUserID[1];

                    PhotonView pv = PhotonView.Find(requestedViewId);
                    if (pv != null)
                    {
                        pv.ownerId = newOwnerId;
                    }

                    break;
                }
            case EventCode.GameList:
                {
                    this.mGameList = new Dictionary<string, RoomInfo>();
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (var gameKey in games.Keys)
                    {
                        string gameName = (string)gameKey;
                        this.mGameList[gameName] = new RoomInfo(gameName, (Hashtable)games[gameKey]);
                    }
                    mGameListCopy = new RoomInfo[mGameList.Count];
                    mGameList.Values.CopyTo(mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.GameListUpdate:
                {
                    Hashtable games = (Hashtable)photonEvent[ParameterCode.GameList];
                    foreach (var roomKey in games.Keys)
                    {
                        string gameName = (string)roomKey;
                        RoomInfo game = new RoomInfo(gameName, (Hashtable)games[roomKey]);
                        if (game.removedFromList)
                        {
                            this.mGameList.Remove(gameName);
                        }
                        else
                        {
                            this.mGameList[gameName] = game;
                        }
                    }
                    this.mGameListCopy = new RoomInfo[this.mGameList.Count];
                    this.mGameList.Values.CopyTo(this.mGameListCopy, 0);
                    SendMonoMessage(PhotonNetworkingMessage.OnReceivedRoomListUpdate);
                    break;
                }

            case EventCode.QueueState:
                // not used anymore
                break;

            case EventCode.AppStats:
                // Debug.LogInfo("Received stats!");
                this.PlayersInRoomsCount = (int)photonEvent[ParameterCode.PeerCount];
                this.PlayersOnMasterCount = (int)photonEvent[ParameterCode.MasterPeerCount];
                this.RoomsCount = (int)photonEvent[ParameterCode.GameCount];
                break;

            case EventCode.Join:
                // actorNr is fetched out of event above
                Hashtable actorProperties = (Hashtable)photonEvent[ParameterCode.PlayerProperties];
                if (originatingPlayer == null)
                {
                    bool isLocal = this.LocalPlayer.ID == actorNr;
                    this.AddNewPlayer(actorNr, new PhotonPlayer(isLocal, actorNr, actorProperties));
                    this.ResetPhotonViewsOnSerialize(); // This sets the correct OnSerializeState for Reliable OnSerialize
                }
                else
                {
                    originatingPlayer.InternalCacheProperties(actorProperties);
                    originatingPlayer.isInactive = false;
                }

                if (actorNr == this.LocalPlayer.ID)
                {
                    // in this player's 'own' join event, we get a complete list of players in the room, so check if we know all players
                    int[] actorsInRoom = (int[])photonEvent[ParameterCode.ActorList];
                    this.UpdatedActorList(actorsInRoom);

                    // joinWithCreateOnDemand can turn an OpJoin into creating the room. Then actorNumber is 1 and callback: OnCreatedRoom()
                    if (this.lastJoinType == JoinType.JoinOrCreateRoom && this.LocalPlayer.ID == 1)
                    {
                        SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
                    }
                    SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom); //Always send OnJoinedRoom

                }
                else
                {
                    SendMonoMessage(PhotonNetworkingMessage.OnPhotonPlayerConnected, this.mActors[actorNr]);
                }
                break;

            case EventCode.Leave:
                this.HandleEventLeave(actorNr, photonEvent);
                break;

            case EventCode.PropertiesChanged:
                int targetActorNr = (int)photonEvent[ParameterCode.TargetActorNr];
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

                this.ReadoutProperties(gameProperties, actorProps, targetActorNr);
                break;

            case PunEvent.RPC:
                //ts: each event now contains a single RPC. execute this
                // Debug.Log("Ev RPC from: " + originatingPlayer);
                this.ExecuteRpc(photonEvent[ParameterCode.Data] as Hashtable, originatingPlayer);
                break;

            case PunEvent.SendSerialize:
            case PunEvent.SendSerializeReliable:
                Hashtable serializeData = (Hashtable)photonEvent[ParameterCode.Data];
                //Debug.Log(serializeData.ToStringFull());

                int remoteUpdateServerTimestamp = (int)serializeData[(byte)0];
                short remoteLevelPrefix = -1;
                short initialDataIndex = 10;
                int headerLength = 1;
                if (serializeData.ContainsKey((byte)1))
                {
                    remoteLevelPrefix = (short)serializeData[(byte)1];
                    headerLength = 2;
                }

                for (short s = initialDataIndex; s - initialDataIndex < serializeData.Count - headerLength; s++)
                {
                    this.OnSerializeRead(serializeData[s] as object[], originatingPlayer, remoteUpdateServerTimestamp, remoteLevelPrefix);
                }
                break;

            case PunEvent.Instantiation:
                this.DoInstantiate((Hashtable)photonEvent[ParameterCode.Data], originatingPlayer, null);
                break;

            case PunEvent.CloseConnection:
                // MasterClient "requests" a disconnection from us
                if (originatingPlayer == null || !originatingPlayer.isMasterClient)
                {
                    Debug.LogError("Error: Someone else(" + originatingPlayer + ") then the masterserver requests a disconnect!");
                }
                else
                {
                    PhotonNetwork.LeaveRoom();
                }

                break;

            case PunEvent.DestroyPlayer:
                Hashtable evData = (Hashtable)photonEvent[ParameterCode.Data];
                int targetPlayerId = (int)evData[(byte)0];
                if (targetPlayerId >= 0)
                {
                    this.DestroyPlayerObjects(targetPlayerId, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.INFO) Debug.Log("Ev DestroyAll! By PlayerId: " + actorNr);
                    this.DestroyAll(true);
                }
                break;

            case PunEvent.Destroy:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int instantiationId = (int)evData[(byte)0];
                // Debug.Log("Ev Destroy for viewId: " + instantiationId + " sent by owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == actorNr) + " this client is owner: " + (instantiationId / PhotonNetwork.MAX_VIEW_IDS == this.LocalPlayer.ID));


                PhotonView pvToDestroy = null;
                if (this.photonViewList.TryGetValue(instantiationId, out pvToDestroy))
                {
                    this.RemoveInstantiatedGO(pvToDestroy.gameObject, true);
                }
                else
                {
                    if (this.DebugOut >= DebugLevel.ERROR) Debug.LogError("Ev Destroy Failed. Could not find PhotonView with instantiationId " + instantiationId + ". Sent by actorNr: " + actorNr);
                }

                break;

            case PunEvent.AssignMaster:
                evData = (Hashtable)photonEvent[ParameterCode.Data];
                int newMaster = (int)evData[(byte)1];
                this.SetMasterClient(newMaster, false);
                break;

            case EventCode.LobbyStats:
                //Debug.Log("LobbyStats EV: " + photonEvent.ToStringFull());

                string[] names = photonEvent[ParameterCode.LobbyName] as string[];
                byte[] types = photonEvent[ParameterCode.LobbyType] as byte[];
                int[] peers = photonEvent[ParameterCode.PeerCount] as int[];
                int[] rooms = photonEvent[ParameterCode.GameCount] as int[];

                this.LobbyStatistics.Clear();
                for (int i = 0; i < names.Length; i++)
                {
                    TypedLobbyInfo info = new TypedLobbyInfo();
                    info.Name = names[i];
                    info.Type = (LobbyType)types[i];
                    info.PlayerCount = peers[i];
                    info.RoomCount = rooms[i];

                    this.LobbyStatistics.Add(info);
                }

                SendMonoMessage(PhotonNetworkingMessage.OnLobbyStatisticsUpdate);
                break;

            case EventCode.ErrorInfo:
                if (PhotonNetwork.OnEventCall != null)
                {
                    object content = photonEvent[ParameterCode.Data];
                    PhotonNetwork.OnEventCall(photonEvent.Code, content, actorNr);
                }
                else
                {
                    Debug.LogWarning("Warning: Unhandled Event ErrorInfo (251). Set PhotonNetwork.OnEventCall to the method PUN should call for this event.");
                }
                break;

            default:
                if (photonEvent.Code < 200)
                {
                    if (PhotonNetwork.OnEventCall != null)
                    {
                        object content = photonEvent[ParameterCode.Data];
                        PhotonNetwork.OnEventCall(photonEvent.Code, content, actorNr);
                    }
                    else
                    {
                        Debug.LogWarning("Warning: Unhandled event " + photonEvent + ". Set PhotonNetwork.OnEventCall.");
                    }
                }
                break;
        }

        //this.externalListener.OnEvent(photonEvent);
    }

    public void OnMessage(object messages)
    {
        // not used here
    }

    #endregion


    private void SetupEncryption(Dictionary<byte, object> encryptionData)
    {
        // this should not be called when authentication is done per server. this mode does not support the required "key-exchange via token"
        if (this.AuthMode == AuthModeOption.Auth)
        {
            if (DebugOut == DebugLevel.ERROR)
            {
                UnityEngine.Debug.LogWarning("SetupEncryption() called but ignored. Not XB1 compiled. EncryptionData: " + encryptionData.ToStringFull());
                return;
            }
        }


        // for AuthOnce and AuthOnceWss, we can keep the same secret across machines (for the session, basically)
        if (DebugOut == DebugLevel.INFO)
        {
            UnityEngine.Debug.Log("SetupEncryption() got called. "+encryptionData.ToStringFull());
        }

        var mode = (EncryptionMode)(byte)encryptionData[EncryptionDataParameters.Mode];
        switch (mode)
        {
            case EncryptionMode.PayloadEncryption:
                byte[] secret = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                this.InitPayloadEncryption(secret);
                break;
            case EncryptionMode.DatagramEncryption:
                {
                    byte[] secret1 = (byte[])encryptionData[EncryptionDataParameters.Secret1];
                    byte[] secret2 = (byte[])encryptionData[EncryptionDataParameters.Secret2];
                    this.InitDatagramEncryption(secret1, secret2);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    protected internal void UpdatedActorList(int[] actorsInRoom)
    {
        for (int i = 0; i < actorsInRoom.Length; i++)
        {
            int actorNrToCheck = actorsInRoom[i];
            if (this.LocalPlayer.ID != actorNrToCheck && !this.mActors.ContainsKey(actorNrToCheck))
            {
                this.AddNewPlayer(actorNrToCheck, new PhotonPlayer(false, actorNrToCheck, string.Empty));
            }
        }
    }

    private void SendVacantViewIds()
    {
        Debug.Log("SendVacantViewIds()");
        List<int> vacantViews = new List<int>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (!view.isOwnerActive)
            {
                vacantViews.Add(view.viewID);
            }
        }

        Debug.Log("Sending vacant view IDs. Length: " + vacantViews.Count);
        //this.OpRaiseEvent(PunEvent.VacantViewIds, true, vacantViews.ToArray());
        this.OpRaiseEvent(PunEvent.VacantViewIds, vacantViews.ToArray(), true, null);
    }

    public static void SendMonoMessage(PhotonNetworkingMessage methodString, params object[] parameters)
    {
        HashSet<GameObject> objectsToCall;
        if (PhotonNetwork.SendMonoMessageTargets != null)
        {
            objectsToCall = PhotonNetwork.SendMonoMessageTargets;
        }
        else
        {
            objectsToCall = PhotonNetwork.FindGameObjectsWithComponent(PhotonNetwork.SendMonoMessageTargetType);
        }

        string methodName = methodString.ToString();
        object callParameter = (parameters != null && parameters.Length == 1) ? parameters[0] : parameters;
        foreach (GameObject gameObject in objectsToCall)
        {
			if (gameObject!=null)
			{
            	gameObject.SendMessage(methodName, callParameter, SendMessageOptions.DontRequireReceiver);
			}
        }
    }

    // PHOTONVIEW/RPC related

    /// <summary>
    /// Executes a received RPC event
    /// </summary>
    protected internal void ExecuteRpc(Hashtable rpcData, PhotonPlayer sender)
    {
        if (rpcData == null || !rpcData.ContainsKey((byte)0))
        {
            Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
            return;
        }

        // ts: updated with "flat" event data
        int netViewID = (int)rpcData[(byte)0]; // LIMITS PHOTONVIEWS&PLAYERS
        int otherSidePrefix = 0;    // by default, the prefix is 0 (and this is not being sent)
        if (rpcData.ContainsKey((byte)1))
        {
            otherSidePrefix = (short)rpcData[(byte)1];
        }


        string inMethodName;
        if (rpcData.ContainsKey((byte)5))
        {
            int rpcIndex = (byte)rpcData[(byte)5];  // LIMITS RPC COUNT
            if (rpcIndex > PhotonNetwork.PhotonServerSettings.RpcList.Count - 1)
            {
                Debug.LogError("Could not find RPC with index: " + rpcIndex + ". Going to ignore! Check PhotonServerSettings.RpcList");
                return;
            }
            else
            {
                inMethodName = PhotonNetwork.PhotonServerSettings.RpcList[rpcIndex];
            }
        }
        else
        {
            inMethodName = (string)rpcData[(byte)3];
        }

        object[] inMethodParameters = null;
        if (rpcData.ContainsKey((byte)4))
        {
            inMethodParameters = (object[])rpcData[(byte)4];
        }

        if (inMethodParameters == null)
        {
            inMethodParameters = new object[0];
        }

        PhotonView photonNetview = this.GetPhotonView(netViewID);
        if (photonNetview == null)
        {
            int viewOwnerId = netViewID/PhotonNetwork.MAX_VIEW_IDS;
            bool owningPv = (viewOwnerId == this.LocalPlayer.ID);
            bool ownerSent = (viewOwnerId == sender.ID);

            if (owningPv)
            {
                Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! View was/is ours." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ID);
            }
            else
            {
                Debug.LogWarning("Received RPC \"" + inMethodName + "\" for viewID " + netViewID + " but this PhotonView does not exist! Was remote PV." + (ownerSent ? " Owner called." : " Remote called.") + " By: " + sender.ID + " Maybe GO was destroyed but RPC not cleaned up.");
            }
            return;
        }

        if (photonNetview.prefix != otherSidePrefix)
        {
            Debug.LogError("Received RPC \"" + inMethodName + "\" on viewID " + netViewID + " with a prefix of " + otherSidePrefix + ", our prefix is " + photonNetview.prefix + ". The RPC has been ignored.");
            return;
        }

        // Get method name
        if (string.IsNullOrEmpty(inMethodName))
        {
            Debug.LogError("Malformed RPC; this should never occur. Content: " + SupportClassPun.DictionaryToString(rpcData));
            return;
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
            Debug.Log("Received RPC: " + inMethodName);


        // SetReceiving filtering
        if (photonNetview.group != 0 && !allowedReceivingGroups.Contains(photonNetview.group))
        {
            return; // Ignore group
        }

        Type[] argTypes = new Type[0];
        if (inMethodParameters.Length > 0)
        {
            argTypes = new Type[inMethodParameters.Length];
            int i = 0;
            for (int index = 0; index < inMethodParameters.Length; index++)
            {
                object objX = inMethodParameters[index];
                if (objX == null)
                {
                    argTypes[i] = null;
                }
                else
                {
                    argTypes[i] = objX.GetType();
                }

                i++;
            }
        }

        int receivers = 0;
        int foundMethods = 0;
        if (!PhotonNetwork.UseRpcMonoBehaviourCache || photonNetview.RpcMonoBehaviours == null || photonNetview.RpcMonoBehaviours.Length == 0)
        {
            photonNetview.RefreshRpcMonoBehaviourCache();
        }

        for (int componentsIndex = 0; componentsIndex < photonNetview.RpcMonoBehaviours.Length; componentsIndex++)
        {
            MonoBehaviour monob = photonNetview.RpcMonoBehaviours[componentsIndex];
            if (monob == null)
            {
                Debug.LogError("ERROR You have missing MonoBehaviours on your gameobjects!");
                continue;
            }

            Type type = monob.GetType();

            // Get [PunRPC] methods from cache
            List<MethodInfo> cachedRPCMethods = null;
            bool methodsOfTypeInCache = this.monoRPCMethodsCache.TryGetValue(type, out cachedRPCMethods);

            if (!methodsOfTypeInCache)
            {
                List<MethodInfo> entries = SupportClassPun.GetMethods(type, typeof(PunRPC));

                this.monoRPCMethodsCache[type] = entries;
                cachedRPCMethods = entries;
            }

            if (cachedRPCMethods == null)
            {
                continue;
            }

            // Check cache for valid methodname+arguments
            for (int index = 0; index < cachedRPCMethods.Count; index++)
            {
                MethodInfo mInfo = cachedRPCMethods[index];
                if (mInfo.Name.Equals(inMethodName))
                {
                    foundMethods++;
                    ParameterInfo[] pArray = mInfo.GetCachedParemeters();

                    if (pArray.Length == argTypes.Length)
                    {
                        // Normal, PhotonNetworkMessage left out
                        if (this.CheckTypeMatch(pArray, argTypes))
                        {
                            receivers++;
                            object result = mInfo.Invoke((object)monob, inMethodParameters);
                            if (PhotonNetwork.StartRpcsAsCoroutine && mInfo.ReturnType == typeof(IEnumerator))
                            {
                                monob.StartCoroutine((IEnumerator)result);
                            }
                        }
                    }
                    else if ((pArray.Length - 1) == argTypes.Length)
                    {
                        // Check for PhotonNetworkMessage being the last
                        if (this.CheckTypeMatch(pArray, argTypes))
                        {
                            if (pArray[pArray.Length - 1].ParameterType == typeof(PhotonMessageInfo))
                            {
                                receivers++;

                                int sendTime = (int)rpcData[(byte)2];
                                object[] deParamsWithInfo = new object[inMethodParameters.Length + 1];
                                inMethodParameters.CopyTo(deParamsWithInfo, 0);
                                deParamsWithInfo[deParamsWithInfo.Length - 1] = new PhotonMessageInfo(sender, sendTime, photonNetview);

                                object result = mInfo.Invoke((object)monob, deParamsWithInfo);
                                if (PhotonNetwork.StartRpcsAsCoroutine && mInfo.ReturnType == typeof(IEnumerator))
                                {
                                    monob.StartCoroutine((IEnumerator)result);
                                }
                            }
                        }
                    }
                    else if (pArray.Length == 1 && pArray[0].ParameterType.IsArray)
                    {
                        receivers++;
                        object result = mInfo.Invoke((object)monob, new object[] { inMethodParameters });
                        if (PhotonNetwork.StartRpcsAsCoroutine && mInfo.ReturnType == typeof(IEnumerator))
                        {
                            monob.StartCoroutine((IEnumerator)result);
                        }
                    }
                }
            }
        }

        // Error handling
        if (receivers != 1)
        {
            string argsString = string.Empty;
            for (int index = 0; index < argTypes.Length; index++)
            {
                Type ty = argTypes[index];
                if (argsString != string.Empty)
                {
                    argsString += ", ";
                }

                if (ty == null)
                {
                    argsString += "null";
                }
                else
                {
                    argsString += ty.Name;
                }
            }

            if (receivers == 0)
            {
                if (foundMethods == 0)
                {
                    Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" marked with the [PunRPC](C#) or @PunRPC(JS) property! Args: " + argsString);
                }
                else
                {
                    Debug.LogError("PhotonView with ID " + netViewID + " has no method \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString);
                }
            }
            else
            {
                Debug.LogError("PhotonView with ID " + netViewID + " has " + receivers + " methods \"" + inMethodName + "\" that takes " + argTypes.Length + " argument(s): " + argsString + ". Should be just one?");
            }
        }
    }

    /// <summary>
    /// Check if all types match with parameters. We can have more paramters then types (allow last RPC type to be different).
    /// </summary>
    /// <param name="methodParameters"></param>
    /// <param name="callParameterTypes"></param>
    /// <returns>If the types-array has matching parameters (of method) in the parameters array (which may be longer).</returns>
    private bool CheckTypeMatch(ParameterInfo[] methodParameters, Type[] callParameterTypes)
    {
        if (methodParameters.Length < callParameterTypes.Length)
        {
            return false;
        }

        for (int index = 0; index < callParameterTypes.Length; index++)
        {
            #if NETFX_CORE
            TypeInfo methodParamTI = methodParameters[index].ParameterType.GetTypeInfo();
            TypeInfo callParamTI = callParameterTypes[index].GetTypeInfo();

            if (callParameterTypes[index] != null && !methodParamTI.IsAssignableFrom(callParamTI) && !(callParamTI.IsEnum && System.Enum.GetUnderlyingType(methodParamTI.AsType()).GetTypeInfo().IsAssignableFrom(callParamTI)))
            {
                return false;
            }
            #else
            Type type = methodParameters[index].ParameterType;
            if (callParameterTypes[index] != null && !type.IsAssignableFrom(callParameterTypes[index]) && !(type.IsEnum && System.Enum.GetUnderlyingType(type).IsAssignableFrom(callParameterTypes[index])))
            {
                return false;
            }
            #endif
        }

        return true;
    }

    internal Hashtable SendInstantiate(string prefabName, Vector3 position, Quaternion rotation, int group, int[] viewIDs, object[] data, bool isGlobalObject)
    {
        // first viewID is now also the gameobject's instantiateId
        int instantiateId = viewIDs[0];   // LIMITS PHOTONVIEWS&PLAYERS

        //TODO: reduce hashtable key usage by using a parameter array for the various values
        Hashtable instantiateEvent = new Hashtable(); // This players info is sent via ActorID
        instantiateEvent[(byte)0] = prefabName;

        if (position != Vector3.zero)
        {
            instantiateEvent[(byte)1] = position;
        }

        if (rotation != Quaternion.identity)
        {
            instantiateEvent[(byte)2] = rotation;
        }

        if (group != 0)
        {
            instantiateEvent[(byte)3] = group;
        }

        // send the list of viewIDs only if there are more than one. else the instantiateId is the viewID
        if (viewIDs.Length > 1)
        {
            instantiateEvent[(byte)4] = viewIDs; // LIMITS PHOTONVIEWS&PLAYERS
        }

        if (data != null)
        {
            instantiateEvent[(byte)5] = data;
        }

        if (this.currentLevelPrefix > 0)
        {
            instantiateEvent[(byte)8] = this.currentLevelPrefix;    // photonview's / object's level prefix
        }

        instantiateEvent[(byte)6] = PhotonNetwork.ServerTimestamp;
        instantiateEvent[(byte)7] = instantiateId;


        RaiseEventOptions options = new RaiseEventOptions();
        options.CachingOption = (isGlobalObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

        this.OpRaiseEvent(PunEvent.Instantiation, instantiateEvent, true, options);
        return instantiateEvent;
    }

    internal GameObject DoInstantiate(Hashtable evData, PhotonPlayer photonPlayer, GameObject resourceGameObject)
    {
        // some values always present:
        string prefabName = (string)evData[(byte)0];
        int serverTime = (int)evData[(byte)6];
        int instantiationId = (int)evData[(byte)7];

        Vector3 position;
        if (evData.ContainsKey((byte)1))
        {
            position = (Vector3)evData[(byte)1];
        }
        else
        {
            position = Vector3.zero;
        }

        Quaternion rotation = Quaternion.identity;
        if (evData.ContainsKey((byte)2))
        {
            rotation = (Quaternion)evData[(byte)2];
        }

        int group = 0;
        if (evData.ContainsKey((byte)3))
        {
            group = (int)evData[(byte)3];
        }

        short objLevelPrefix = 0;
        if (evData.ContainsKey((byte)8))
        {
            objLevelPrefix = (short)evData[(byte)8];
        }

        int[] viewsIDs;
        if (evData.ContainsKey((byte)4))
        {
            viewsIDs = (int[])evData[(byte)4];
        }
        else
        {
            viewsIDs = new int[1] { instantiationId };
        }

        object[] incomingInstantiationData;
        if (evData.ContainsKey((byte)5))
        {
            incomingInstantiationData = (object[])evData[(byte)5];
        }
        else
        {
            incomingInstantiationData = null;
        }

        // SetReceiving filtering
        if (group != 0 && !this.allowedReceivingGroups.Contains(group))
        {
            return null; // Ignore group
        }

        if (ObjectPool != null)
        {
            GameObject go = ObjectPool.Instantiate(prefabName, position, rotation);

            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            if (photonViews.Length != viewsIDs.Length)
            {
                throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
            }
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].didAwake = false;
                photonViews[i].viewID = 0;

                photonViews[i].prefix = objLevelPrefix;
                photonViews[i].instantiationId = instantiationId;
                photonViews[i].isRuntimeInstantiated = true;
                photonViews[i].instantiationDataField = incomingInstantiationData;

                photonViews[i].didAwake = true;
                photonViews[i].viewID = viewsIDs[i];    // with didAwake true and viewID == 0, this will also register the view
            }

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(OnPhotonInstantiateString, new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
            return go;
        }
        else
        {
            // load prefab, if it wasn't loaded before (calling methods might do this)
            if (resourceGameObject == null)
            {
                if (!NetworkingPeer.UsePrefabCache || !NetworkingPeer.PrefabCache.TryGetValue(prefabName, out resourceGameObject))
                {
                    resourceGameObject = (GameObject)Resources.Load(prefabName, typeof (GameObject));
                    if (NetworkingPeer.UsePrefabCache)
                    {
                        NetworkingPeer.PrefabCache.Add(prefabName, resourceGameObject);
                    }
                }

                if (resourceGameObject == null)
                {
                    Debug.LogError("PhotonNetwork error: Could not Instantiate the prefab [" + prefabName + "]. Please verify you have this gameobject in a Resources folder.");
                    return null;
                }
            }

            // now modify the loaded "blueprint" object before it becomes a part of the scene (by instantiating it)
            PhotonView[] resourcePVs = resourceGameObject.GetPhotonViewsInChildren();
            if (resourcePVs.Length != viewsIDs.Length)
            {
                throw new Exception("Error in Instantiation! The resource's PhotonView count is not the same as in incoming data.");
            }

            for (int i = 0; i < viewsIDs.Length; i++)
            {
                // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
                // so we only set the viewID and instantiationId now. the instantiationData can be fetched
                resourcePVs[i].viewID = viewsIDs[i];
                resourcePVs[i].prefix = objLevelPrefix;
                resourcePVs[i].instantiationId = instantiationId;
                resourcePVs[i].isRuntimeInstantiated = true;
            }

            this.StoreInstantiationData(instantiationId, incomingInstantiationData);

            // load the resource and set it's values before instantiating it:
            GameObject go = (GameObject)GameObject.Instantiate(resourceGameObject, position, rotation);

            for (int i = 0; i < viewsIDs.Length; i++)
            {
                // NOTE instantiating the loaded resource will keep the viewID but would not copy instantiation data, so it's set below
                // so we only set the viewID and instantiationId now. the instantiationData can be fetched
                resourcePVs[i].viewID = 0;
                resourcePVs[i].prefix = -1;
                resourcePVs[i].prefixBackup = -1;
                resourcePVs[i].instantiationId = -1;
                resourcePVs[i].isRuntimeInstantiated = false;
            }

            this.RemoveInstantiationData(instantiationId);

            // Send OnPhotonInstantiate callback to newly created GO.
            // GO will be enabled when instantiated from Prefab and it does not matter if the script is enabled or disabled.
            go.SendMessage(OnPhotonInstantiateString, new PhotonMessageInfo(photonPlayer, serverTime, null), SendMessageOptions.DontRequireReceiver);
            return go;
        }
    }

    private Dictionary<int, object[]> tempInstantiationData = new Dictionary<int, object[]>();

    private void StoreInstantiationData(int instantiationId, object[] instantiationData)
    {
        // Debug.Log("StoreInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        tempInstantiationData[instantiationId] = instantiationData;
    }

    public object[] FetchInstantiationData(int instantiationId)
    {
        object[] data = null;
        if (instantiationId == 0)
        {
            return null;
        }

        tempInstantiationData.TryGetValue(instantiationId, out data);
        // Debug.Log("FetchInstantiationData() instantiationId: " + instantiationId + " tempInstantiationData.Count: " + tempInstantiationData.Count);
        return data;
    }

    private void RemoveInstantiationData(int instantiationId)
    {
        tempInstantiationData.Remove(instantiationId);
    }


    /// <summary>
    /// Destroys all Instantiates and RPCs locally and (if not localOnly) sends EvDestroy(player) and clears related events in the server buffer.
    /// </summary>
    public void DestroyPlayerObjects(int playerId, bool localOnly)
    {
        if (playerId <= 0)
        {
            Debug.LogError("Failed to Destroy objects of playerId: " + playerId);
            return;
        }

        if (!localOnly)
        {
            // clean server's Instantiate and RPC buffers
            this.OpRemoveFromServerInstantiationsOfPlayer(playerId);
            this.OpCleanRpcBuffer(playerId);

            // send Destroy(player) to anyone else
            this.SendDestroyOfPlayer(playerId);
        }

        // locally cleaning up that player's objects
        HashSet<GameObject> playersGameObjects = new HashSet<GameObject>();
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view!=null && view.CreatorActorNr == playerId)
            {
                playersGameObjects.Add(view.gameObject);
            }
        }

        // any non-local work is already done, so with the list of that player's objects, we can clean up (locally only)
        foreach (GameObject gameObject in playersGameObjects)
        {
            this.RemoveInstantiatedGO(gameObject, true);
        }

        // with ownership transfer, some objects might lose their owner.
        // in that case, the creator becomes the owner again. every client can apply this. done below.
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.ownerId == playerId)
            {
                view.ownerId = view.CreatorActorNr;
                //Debug.Log("Creator is: " + view.ownerId);
            }
        }
    }

    public void DestroyAll(bool localOnly)
    {
        if (!localOnly)
        {
            this.OpRemoveCompleteCache();
            this.SendDestroyOfAll();
        }

        this.LocalCleanupAnythingInstantiated(true);
    }

    /// <summary>Removes GameObject and the PhotonViews on it from local lists and optionally updates remotes. GameObject gets destroyed at end.</summary>
    /// <remarks>
    /// This method might fail and quit early due to several tests.
    /// </remarks>
    /// <param name="go">GameObject to cleanup.</param>
    /// <param name="localOnly">For localOnly, tests of control are skipped and the server is not updated.</param>
    protected internal void RemoveInstantiatedGO(GameObject go, bool localOnly)
    {
        if (go == null)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because it's null.");
            return;
        }

        // Don't remove the GO if it doesn't have any PhotonView
        PhotonView[] views = go.GetComponentsInChildren<PhotonView>(true);
        if (views == null || views.Length <= 0)
        {
            Debug.LogError("Failed to 'network-remove' GameObject because has no PhotonView components: " + go);
            return;
        }

        PhotonView viewZero = views[0];
        int creatorId = viewZero.CreatorActorNr;            // creatorId of obj is needed to delete EvInstantiate (only if it's from that user)
        int instantiationId = viewZero.instantiationId;     // actual, live InstantiationIds start with 1 and go up

        // Don't remove GOs that are owned by others (unless this is the master and the remote player left)
        if (!localOnly)
        {
            if (!viewZero.isMine)
            {
                Debug.LogError("Failed to 'network-remove' GameObject. Client is neither owner nor masterClient taking over for owner who left: " + viewZero);
                return;
            }

            // Don't remove the Instantiation from the server, if it doesn't have a proper ID
            if (instantiationId < 1)
            {
                Debug.LogError("Failed to 'network-remove' GameObject because it is missing a valid InstantiationId on view: " + viewZero + ". Not Destroying GameObject or PhotonViews!");
                return;
            }
        }


        // cleanup instantiation (event and local list)
        if (!localOnly)
        {
            this.ServerCleanInstantiateAndDestroy(instantiationId, creatorId, viewZero.isRuntimeInstantiated);   // server cleaning
        }


        // cleanup PhotonViews and their RPCs events (if not localOnly)
        for (int j = views.Length - 1; j >= 0; j--)
        {
            PhotonView view = views[j];
            if (view == null)
            {
                continue;
            }

            // we only destroy/clean PhotonViews that were created by PhotonNetwork.Instantiate (and those have an instantiationId!)
            if (view.instantiationId >= 1)
            {
                this.LocalCleanPhotonView(view);
            }
            if (!localOnly)
            {
                this.OpCleanRpcBuffer(view);
            }
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Network destroy Instantiated GO: " + go.name);
        }


        if (this.ObjectPool != null)
        {
            PhotonView[] photonViews = go.GetPhotonViewsInChildren();
            for (int i = 0; i < photonViews.Length; i++)
            {
                photonViews[i].viewID = 0;  // marks the PV as not being in use currently.
            }
            this.ObjectPool.Destroy(go);
        }
        else
        {
            GameObject.Destroy(go);
        }
    }

    /// <summary>
    /// This returns -1 if the GO could not be found in list of instantiatedObjects.
    /// </summary>
    public int GetInstantiatedObjectsId(GameObject go)
    {
        int id = -1;
        if (go == null)
        {
            Debug.LogError("GetInstantiatedObjectsId() for GO == null.");
            return id;
        }

        PhotonView[] pvs = go.GetPhotonViewsInChildren();
        if (pvs != null && pvs.Length > 0 && pvs[0] != null)
        {
            return pvs[0].instantiationId;
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
            UnityEngine.Debug.Log("GetInstantiatedObjectsId failed for GO: " + go);


        return id;
    }

    /// <summary>
    /// Removes an instantiation event from the server's cache. Needs id and actorNr of player who instantiated.
    /// </summary>
    private void ServerCleanInstantiateAndDestroy(int instantiateId, int creatorId, bool isRuntimeInstantiated)
    {
        Hashtable removeFilter = new Hashtable();
        removeFilter[(byte)7] = instantiateId;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { creatorId } };
        this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, removeFilter, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);

        Hashtable evData = new Hashtable();
        evData[(byte)0] = instantiateId;
        options = null;
        if (!isRuntimeInstantiated)
        {
            // if the view got loaded with the scene, the EvDestroy must be cached (there is no Instantiate-msg which we can remove)
            // reason: joining players will load the obj and have to destroy it (too)
            options = new RaiseEventOptions();
            options.CachingOption = EventCaching.AddToRoomCacheGlobal;
            Debug.Log("Destroying GO as global. ID: " + instantiateId);
        }
        this.OpRaiseEvent(PunEvent.Destroy, evData, true, options);
    }

    private void SendDestroyOfPlayer(int actorNr)
    {
        Hashtable evData = new Hashtable();
        evData[(byte)0] = actorNr;

        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    private void SendDestroyOfAll()
    {
        Hashtable evData = new Hashtable();
        evData[(byte)0] = -1;


        this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, null);
        //this.OpRaiseEvent(PunEvent.DestroyPlayer, evData, true, 0, EventCaching.DoNotCache, ReceiverGroup.Others);
    }

    private void OpRemoveFromServerInstantiationsOfPlayer(int actorNr)
    {
        // removes all "Instantiation" events of player actorNr. this is not an event for anyone else
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNr } };
        this.OpRaiseEvent(PunEvent.Instantiation, null, true, options);
        //this.OpRaiseEvent(PunEvent.Instantiation, null, true, 0, new int[] { actorNr }, EventCaching.RemoveFromRoomCache);
    }

    internal protected void RequestOwnership(int viewID, int fromOwner)
    {
        Debug.Log("RequestOwnership(): " + viewID + " from: " + fromOwner + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipRequest, true, new int[] { viewID, fromOwner }, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipRequest, new int[] {viewID, fromOwner}, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

    internal protected void TransferOwnership(int viewID, int playerID)
    {
        Debug.Log("TransferOwnership() view " + viewID + " to: " + playerID + " Time: " + Environment.TickCount % 1000);
        //PhotonNetwork.networkingPeer.OpRaiseEvent(PunEvent.OwnershipTransfer, true, new int[] {viewID, playerID}, 0, EventCaching.DoNotCache, null, ReceiverGroup.All, 0);
        this.OpRaiseEvent(PunEvent.OwnershipTransfer, new int[] { viewID, playerID }, true, new RaiseEventOptions() { Receivers = ReceiverGroup.All });   // All sends to all via server (including self)
    }

    public bool LocalCleanPhotonView(PhotonView view)
    {
        view.removedFromLocalViewList = true;
        return this.photonViewList.Remove(view.viewID);
    }

    public PhotonView GetPhotonView(int viewID)
    {
        PhotonView result = null;
        this.photonViewList.TryGetValue(viewID, out result);

        if (result == null)
        {
            PhotonView[] views = GameObject.FindObjectsOfType(typeof(PhotonView)) as PhotonView[];

            for (int i = 0; i < views.Length; i++)
            {
                PhotonView view = views[i];
                if (view.viewID == viewID)
                {
                    if (view.didAwake)
                    {
                        Debug.LogWarning("Had to lookup view that wasn't in photonViewList: " + view);
                    }
                    return view;
                }
            }
        }

        return result;
    }

    public void RegisterPhotonView(PhotonView netView)
    {
        if (!Application.isPlaying)
        {
            this.photonViewList = new Dictionary<int, PhotonView>();
            return;
        }

        if (netView.viewID == 0)
        {
            // don't register views with ID 0 (not initialized). they register when a ID is assigned later on
            Debug.Log("PhotonView register is ignored, because viewID is 0. No id assigned yet to: " + netView);
            return;
        }

        PhotonView listedView = null;
        bool isViewListed = this.photonViewList.TryGetValue(netView.viewID, out listedView);
        if (isViewListed)
        {
            // if some other view is in the list already, we got a problem. it might be undestructible. print out error
            if (netView != listedView)
            {
                Debug.LogError(string.Format("PhotonView ID duplicate found: {0}. New: {1} old: {2}. Maybe one wasn't destroyed on scene load?! Check for 'DontDestroyOnLoad'. Destroying old entry, adding new.", netView.viewID, netView, listedView));
            }
            else
            {
                return;
            }

            this.RemoveInstantiatedGO(listedView.gameObject, true);
        }

        // Debug.Log("adding view to known list: " + netView);
        this.photonViewList.Add(netView.viewID, netView);
        //Debug.LogError("view being added. " + netView);	// Exit Games internal log

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Registered PhotonView: " + netView.viewID);
        }
    }

    ///// <summary>
    ///// Will remove the view from list of views (by its ID).
    ///// </summary>
    //public void RemovePhotonView(PhotonView netView)
    //{
    //    if (!Application.isPlaying)
    //    {
    //        this.photonViewList = new Dictionary<int, PhotonView>();
    //        return;
    //    }

    //    //PhotonView removedView = null;
    //    //this.photonViewList.TryGetValue(netView.viewID, out removedView);
    //    //if (removedView != netView)
    //    //{
    //    //    Debug.LogError("Detected two differing PhotonViews with same viewID: " + netView.viewID);
    //    //}

    //    this.photonViewList.Remove(netView.viewID);

    //    //if (this.DebugOut >= DebugLevel.ALL)
    //    //{
    //    //    this.DebugReturn(DebugLevel.ALL, "Removed PhotonView: " + netView.viewID);
    //    //}
    //}

    /// <summary>
    /// Removes the RPCs of someone else (to be used as master).
    /// This won't clean any local caches. It just tells the server to forget a player's RPCs and instantiates.
    /// </summary>
    /// <param name="actorNumber"></param>
    public void OpCleanRpcBuffer(int actorNumber)
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(PunEvent.RPC, null, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }

    /// <summary>
    /// Instead removing RPCs or Instantiates, this removed everything cached by the actor.
    /// </summary>
    /// <param name="actorNumber"></param>
    public void OpRemoveCompleteCacheOfPlayer(int actorNumber)
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, TargetActors = new int[] { actorNumber } };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, new int[] { actorNumber }, EventCaching.RemoveFromRoomCache);
    }


    public void OpRemoveCompleteCache()
    {
        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache, Receivers = ReceiverGroup.MasterClient };
        this.OpRaiseEvent(0, null, true, options);
        //this.OpRaiseEvent(0, null, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.MasterClient);  // TODO: check who gets this event?
    }

    /// This clears the cache of any player/actor who's no longer in the room (making it a simple clean-up option for a new master)
    private void RemoveCacheOfLeftPlayers()
    {
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters[ParameterCode.Code] = (byte)0;		// any event
        opParameters[ParameterCode.Cache] = (byte)EventCaching.RemoveFromRoomCacheForActorsLeft;    // option to clear the room cache of all events of players who left

        this.OpCustom((byte)OperationCode.RaiseEvent, opParameters, true, 0);
    }

    // Remove RPCs of view (if they are local player's RPCs)
    public void CleanRpcBufferIfMine(PhotonView view)
    {
        if (view.ownerId != this.LocalPlayer.ID && !LocalPlayer.isMasterClient)
        {
            Debug.LogError("Cannot remove cached RPCs on a PhotonView thats not ours! " + view.owner + " scene: " + view.isSceneView);
            return;
        }

        this.OpCleanRpcBuffer(view);
    }

    /// <summary>Cleans server RPCs for PhotonView (without any further checks).</summary>
    public void OpCleanRpcBuffer(PhotonView view)
    {
        Hashtable rpcFilterByViewId = new Hashtable();
        rpcFilterByViewId[(byte)0] = view.viewID;

        RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.RemoveFromRoomCache };
        this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, options);
        //this.OpRaiseEvent(PunEvent.RPC, rpcFilterByViewId, true, 0, EventCaching.RemoveFromRoomCache, ReceiverGroup.Others);
    }

    public void RemoveRPCsInGroup(int group)
    {
        foreach (PhotonView view in this.photonViewList.Values)
        {
            if (view.group == group)
            {
                this.CleanRpcBufferIfMine(view);
            }
        }
    }

    public void SetLevelPrefix(short prefix)
    {
        this.currentLevelPrefix = prefix;
        // TODO: should we really change the prefix for existing PVs?! better keep it!
        //foreach (PhotonView view in this.photonViewList.Values)
        //{
        //    view.prefix = prefix;
        //}
    }


    /// RPC Hashtable Structure
    /// (byte)0 -> (int) ViewId (combined from actorNr and actor-unique-id)
    /// (byte)1 -> (short) prefix (level)
    /// (byte)2 -> (int) server timestamp
    /// (byte)3 -> (string) methodname
    /// (byte)4 -> (object[]) parameters
    /// (byte)5 -> (byte) method shortcut (alternative to name)
    ///
    /// This is sent as event (code: 200) which will contain a sender (origin of this RPC).

    internal void RPC(PhotonView view, string methodName, PhotonTargets target, PhotonPlayer player, bool encrypt, params object[] parameters)
    {
        if (this.blockSendingGroups.Contains(view.group))
        {
            return; // Block sending on this group
        }

        if (view.viewID < 1)
        {
            Debug.LogError("Illegal view ID:" + view.viewID + " method: " + methodName + " GO:" + view.gameObject.name);
        }

        if (PhotonNetwork.logLevel >= PhotonLogLevel.Full)
        {
            Debug.Log("Sending RPC \"" + methodName + "\" to target: " + target + " or player:" + player + ".");
        }


        //ts: changed RPCs to a one-level hashtable as described in internal.txt
        Hashtable rpcEvent = new Hashtable();
        rpcEvent[(byte)0] = (int)view.viewID; // LIMITS NETWORKVIEWS&PLAYERS
        if (view.prefix > 0)
        {
            rpcEvent[(byte)1] = (short)view.prefix;
        }
        rpcEvent[(byte)2] = PhotonNetwork.ServerTimestamp;


        // send name or shortcut (if available)
        int shortcut = 0;
        if (rpcShortcuts.TryGetValue(methodName, out shortcut))
        {
            rpcEvent[(byte)5] = (byte)shortcut; // LIMITS RPC COUNT
        }
        else
        {
            rpcEvent[(byte)3] = methodName;
        }

        if (parameters != null && parameters.Length > 0)
        {
            rpcEvent[(byte)4] = (object[])parameters;
        }


        // if sent to target player, this overrides the target
        if (player != null)
        {
            if (this.LocalPlayer.ID == player.ID)
            {
                this.ExecuteRpc(rpcEvent, player);
            }
            else
            {
                RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { player.ID }, Encrypt = encrypt };
                this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            }

            return;
        }

        // send to a specific set of players
        if (target == PhotonTargets.All)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);

            // Execute local
            this.ExecuteRpc(rpcEvent, this.LocalPlayer);
        }
        else if (target == PhotonTargets.Others)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
        }
        else if (target == PhotonTargets.AllBuffered)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);

            // Execute local
            this.ExecuteRpc(rpcEvent, this.LocalPlayer);
        }
        else if (target == PhotonTargets.OthersBuffered)
        {
            RaiseEventOptions options = new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
        }
        else if (target == PhotonTargets.MasterClient)
        {
            if (this.mMasterClientId == this.LocalPlayer.ID)
            {
                this.ExecuteRpc(rpcEvent, this.LocalPlayer);
            }
            else
            {
                RaiseEventOptions options = new RaiseEventOptions() { Receivers = ReceiverGroup.MasterClient, Encrypt = encrypt };
                this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            }
        }
        else if (target == PhotonTargets.AllViaServer)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Receivers = ReceiverGroup.All, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            if (PhotonNetwork.offlineMode)
            {
                this.ExecuteRpc(rpcEvent, this.LocalPlayer);
            }
        }
        else if (target == PhotonTargets.AllBufferedViaServer)
        {
            RaiseEventOptions options = new RaiseEventOptions() { InterestGroup = (byte)view.group, Receivers = ReceiverGroup.All, CachingOption = EventCaching.AddToRoomCache, Encrypt = encrypt };
            this.OpRaiseEvent(PunEvent.RPC, rpcEvent, true, options);
            if (PhotonNetwork.offlineMode)
            {
                this.ExecuteRpc(rpcEvent, this.LocalPlayer);
            }
        }
        else
        {
            Debug.LogError("Unsupported target enum: " + target);
        }
    }

    // SetReceiving
    public void SetReceivingEnabled(int group, bool enabled)
    {
        if (group <= 0)
        {
            Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + group + ". The group number should be at least 1.");
            return;
        }

        if (enabled)
        {
            if (!this.allowedReceivingGroups.Contains(group))
            {
                this.allowedReceivingGroups.Add(group);
                byte[] groups = new byte[1] { (byte)group };
                this.OpChangeGroups(null, groups);
            }
        }
        else
        {
            if (this.allowedReceivingGroups.Contains(group))
            {
                this.allowedReceivingGroups.Remove(group);
                byte[] groups = new byte[1] { (byte)group };
                this.OpChangeGroups(groups, null);
            }
        }
    }


    public void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        List<byte> enableList = new List<byte>();
        List<byte> disableList = new List<byte>();

        if (enableGroups != null)
        {
            for (int index = 0; index < enableGroups.Length; index++)
            {
                int i = enableGroups[index];
                if (i <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + i + ". The group number should be at least 1.");
                    continue;
                }
                if (!this.allowedReceivingGroups.Contains(i))
                {
                    this.allowedReceivingGroups.Add(i);
                    enableList.Add((byte)i);
                }
            }
        }
        if (disableGroups != null)
        {
            for (int index = 0; index < disableGroups.Length; index++)
            {
                int i = disableGroups[index];
                if (i <= 0)
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled was called with an illegal group number: " + i + ". The group number should be at least 1.");
                    continue;
                }
                if (enableList.Contains((byte)i))
                {
                    Debug.LogError("Error: PhotonNetwork.SetReceivingEnabled disableGroups contains a group that is also in the enableGroups: " + i + ".");
                    continue;
                }
                if (this.allowedReceivingGroups.Contains(i))
                {
                    this.allowedReceivingGroups.Remove(i);
                    disableList.Add((byte)i);
                }
            }
        }

        this.OpChangeGroups(disableList.Count > 0 ? disableList.ToArray() : null, enableList.Count > 0 ? enableList.ToArray() : null); //Passing a 0 sized array != passing null
    }

    // SetSending
    public void SetSendingEnabled(int group, bool enabled)
    {
        if (!enabled)
        {
            this.blockSendingGroups.Add(group); // can be added to HashSet no matter if already in it
        }
        else
        {
            this.blockSendingGroups.Remove(group);
        }
    }


    public void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if(enableGroups!=null){
            foreach(int i in enableGroups){
                if(this.blockSendingGroups.Contains(i))
                    this.blockSendingGroups.Remove(i);
            }
        }
        if(disableGroups!=null){
            foreach(int i in disableGroups){
                if(!this.blockSendingGroups.Contains(i))
                    this.blockSendingGroups.Add(i);
            }
        }
    }


    public void NewSceneLoaded()
    {
        if (this.loadingLevelAndPausedNetwork)
        {
            this.loadingLevelAndPausedNetwork = false;
            PhotonNetwork.isMessageQueueRunning = true;
        }
        // Debug.Log("OnLevelWasLoaded photonViewList.Count: " + photonViewList.Count); // Exit Games internal log

        List<int> removeKeys = new List<int>();
        foreach (KeyValuePair<int, PhotonView> kvp in this.photonViewList)
        {
            PhotonView view = kvp.Value;
            if (view == null)
            {
                removeKeys.Add(kvp.Key);
            }
        }

        for (int index = 0; index < removeKeys.Count; index++)
        {
            int key = removeKeys[index];
            this.photonViewList.Remove(key);
        }

        if (removeKeys.Count > 0)
        {
            if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                Debug.Log("New level loaded. Removed " + removeKeys.Count + " scene view IDs from last level.");
        }
    }


    // this is called by Update() and in Unity that means it's single threaded.
    public void RunViewUpdate()
    {
        if (!PhotonNetwork.connected || PhotonNetwork.offlineMode || this.mActors == null)
        {
            return;
        }

        // no need to send OnSerialize messages while being alone (these are not buffered anyway)
        if (this.mActors.Count <= 1)
        {
            #if !PHOTON_DEVELOP
            return;
            #endif
        }


        /* Format of the data hashtable:
         * Hasthable dataPergroup*
         *  [(byte)0] = PhotonNetwork.ServerTimestamp;
         *  [(byte)1] = currentLevelPrefix;  OPTIONAL!
         *
         *  [(short)10] = data 1
         *  [(short)11] = data 2 ...
         */

        int countOfUpdatesToSend = 0;
        foreach (PhotonView view in this.photonViewList.Values)
        {
            // a client only sends updates for active, synchronized PhotonViews that are under it's control (isMine)
            if (view.synchronization == ViewSynchronization.Off || view.isMine == false || view.gameObject.activeInHierarchy == false)
            {
                continue;
            }

            if (this.blockSendingGroups.Contains(view.group))
            {
                continue; // Block sending on this group
            }


            // call the PhotonView's serialize method(s)
            object[] evData = this.OnSerializeWrite(view);
            if (evData == null)
            {
                continue;
            }

            if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed || view.mixedModeIsReliable)
            {
                Hashtable groupHashtable = null;
                bool found = this.dataPerGroupReliable.TryGetValue(view.group, out groupHashtable);
                if (!found)
                {
                    groupHashtable = new Hashtable(10);
                    this.dataPerGroupReliable[view.group] = groupHashtable;
                }

                groupHashtable.Add((short)(groupHashtable.Count+10), evData);
                countOfUpdatesToSend++;
            }
            else
            {
                Hashtable groupHashtable = null;
                bool found = this.dataPerGroupUnreliable.TryGetValue(view.group, out groupHashtable);
                if (!found)
                {
                    groupHashtable = new Hashtable(10);
                    this.dataPerGroupUnreliable[view.group] = groupHashtable;
                }

                groupHashtable.Add((short)(groupHashtable.Count+10), evData);
                countOfUpdatesToSend++;
            }
        }   // all views serialized


        // if we didn't produce anything to send, don't do it
        if (countOfUpdatesToSend == 0)
        {
            return;
        }


        // we got updates to send. every group is send it's own message and unreliable and reliable are split as well
        RaiseEventOptions options = new RaiseEventOptions();
        #if PHOTON_DEVELOP
        options.Receivers = ReceiverGroup.All;
        #endif

        foreach (int groupId in this.dataPerGroupReliable.Keys)
        {
            options.InterestGroup = (byte)groupId;
            Hashtable groupHashtable = this.dataPerGroupReliable[groupId];
            if (groupHashtable.Count == 0)
            {
                continue;
            }

            groupHashtable[(byte)0] = PhotonNetwork.ServerTimestamp;
            if (this.currentLevelPrefix >= 0)
            {
                groupHashtable[(byte)1] = this.currentLevelPrefix;
            }

            this.OpRaiseEvent(PunEvent.SendSerializeReliable, groupHashtable, true, options);
            groupHashtable.Clear();
        }
        foreach (int groupId in this.dataPerGroupUnreliable.Keys)
        {
            options.InterestGroup = (byte)groupId;
            Hashtable groupHashtable = this.dataPerGroupUnreliable[groupId];
            if (groupHashtable.Count == 0)
            {
                continue;
            }

            groupHashtable[(byte)0] = PhotonNetwork.ServerTimestamp;
            if (this.currentLevelPrefix >= 0)
            {
                groupHashtable[(byte)1] = this.currentLevelPrefix;
            }

            this.OpRaiseEvent(PunEvent.SendSerialize, groupHashtable, false, options);
            groupHashtable.Clear();
        }
    }


    // calls OnPhotonSerializeView (through ExecuteOnSerialize)
    // the content created here is consumed by receivers in: ReadOnSerialize
    private object[] OnSerializeWrite(PhotonView view)
    {
        if (view.synchronization == ViewSynchronization.Off)
        {
            return null;
        }


        // each view creates a list of values that should be sent
        PhotonMessageInfo info = new PhotonMessageInfo(this.LocalPlayer, PhotonNetwork.ServerTimestamp, view);
        this.pStream.ResetWriteStream();
        this.pStream.SendNext((int)view.viewID);
        this.pStream.SendNext(false);
        this.pStream.SendNext(null);
        view.SerializeView(this.pStream, info);


        // check if there are actual values to be sent (after the "header" of viewId, (bool)compressed and (int[])nullValues)
        if (this.pStream.Count <= SyncFirstValue)
        {
            return null;
        }
        if (view.synchronization == ViewSynchronization.Unreliable)
        {
            return this.pStream.ToArray();
        }


        // ViewSynchronization: Off, Unreliable, UnreliableOnChange, ReliableDeltaCompressed


        object[] currentValues = this.pStream.ToArray();
        if (view.synchronization == ViewSynchronization.UnreliableOnChange)
        {
            if (AlmostEquals(currentValues, view.lastOnSerializeDataSent))
            {
                if (view.mixedModeIsReliable)
                {
                    return null;
                }

                view.mixedModeIsReliable = true;
                view.lastOnSerializeDataSent = currentValues;
            }
            else
            {
                view.mixedModeIsReliable = false;
                view.lastOnSerializeDataSent = currentValues;
            }

            return currentValues;
        }

        if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
        {
            // compress content of data set (by comparing to view.lastOnSerializeDataSent)
            // the "original" dataArray is NOT modified by DeltaCompressionWrite
            object[] dataToSend = this.DeltaCompressionWrite(view.lastOnSerializeDataSent, currentValues);

            // cache the values that were written this time (not the compressed values)
            view.lastOnSerializeDataSent = currentValues;

            return dataToSend;
        }

        return null;
    }


	string LogObjectArray(object[] data)
	{
	    if (data == null) return "null";
        string[] sb = new string[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            object o = data[i];
            sb[i] = (o != null) ? o.ToString() : "null";
        }

        return string.Join(", ",sb);
    }


    /// <summary>
    /// Reads updates created by OnSerializeWrite
    /// </summary>
    private void OnSerializeRead(object[] data, PhotonPlayer sender, int networkTime, short correctPrefix)
    {
        // read view ID from key (byte)0: a int-array (PUN 1.17++)
        int viewID = (int)data[SyncViewId];


        // debug:
        //LogObjectArray(data);

        PhotonView view = this.GetPhotonView(viewID);
        if (view == null)
        {
            Debug.LogWarning("Received OnSerialization for view ID " + viewID + ". We have no such PhotonView! Ignored this if you're leaving a room. State: " + this.State);
            return;
        }

        if (view.prefix > 0 && correctPrefix != view.prefix)
        {
            Debug.LogError("Received OnSerialization for view ID " + viewID + " with prefix " + correctPrefix + ". Our prefix is " + view.prefix);
            return;
        }

        // SetReceiving filtering
        if (view.group != 0 && !this.allowedReceivingGroups.Contains(view.group))
        {
            return; // Ignore group
        }


        if (view.synchronization == ViewSynchronization.ReliableDeltaCompressed)
        {
            object[] uncompressed = this.DeltaCompressionRead(view.lastOnSerializeDataReceived, data);
            //LogObjectArray(uncompressed,"uncompressed ");
            if (uncompressed == null)
            {
                // Skip this packet as we haven't got received complete-copy of this view yet.
                if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
                {
                    Debug.Log("Skipping packet for " + view.name + " [" + view.viewID + "] as we haven't received a full packet for delta compression yet. This is OK if it happens for the first few frames after joining a game.");
                }
                return;
            }

            // store last received values (uncompressed) for delta-compression usage
            view.lastOnSerializeDataReceived = uncompressed;
            data = uncompressed;
        }

        //// TODO: check if we really want to set the owner of a GO, based on who sends something about it.
        //// this has nothing to do with reading the actual synchronization update.
        //if (sender.ID != view.ownerId)
        //{
        //    if (!view.isSceneView || !sender.isMasterClient)
        //    {
        //        // obviously the owner changed and we didn't yet notice.
        //        Debug.Log("Adjusting owner to sender of updates. From: " + view.ownerId + " to: " + sender.ID);
        //        view.ownerId = sender.ID;
        //    }
        //}

        this.readStream.SetReadStream(data, 3);
        PhotonMessageInfo info = new PhotonMessageInfo(sender, networkTime, view);

        view.DeserializeView(this.readStream, info);
    }


    private bool AlmostEquals(object[] lastData, object[] currentContent)
    {
        if (lastData == null && currentContent == null)
        {
            return true;
        }

        if (lastData == null || currentContent == null || (lastData.Length != currentContent.Length))
        {
            return false;
        }

        for (int index = 0; index < currentContent.Length; index++)
        {
            object newObj = currentContent[index];
            object oldObj = lastData[index];
            if (!this.ObjectIsSameWithInprecision(newObj, oldObj))
            {
                return false;
            }
        }

        return true;
    }


    // compresses currentContent by using NULL as value if currentContent equals previousContent
    // skips initial indexes, as defined by SyncFirstValue
    // to conserve memory, the previousContent is re-used as buffer for the result! duplicate the values before using this, if needed
    // returns null, if nothing must be sent (current content might be null, which also returns null)
    // SyncFirstValue should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
    private object[] DeltaCompressionWrite(object[] previousContent, object[] currentContent)
    {
        if (currentContent == null || previousContent == null || previousContent.Length != currentContent.Length)
        {
            return currentContent;  // the current data needs to be sent (which might be null)
        }

        if (currentContent.Length <= SyncFirstValue)
        {
            return null;  // this send doesn't contain values (except the "headers"), so it's not being sent
        }


        object[] compressedContent = previousContent;   // the previous content is no longer needed, once we compared the values!
        compressedContent[SyncCompressed] = false;
        int compressedValues = 0;

        Queue<int> valuesThatAreChangedToNull = null;
        for (int index = SyncFirstValue; index < currentContent.Length; index++)
        {
            object newObj = currentContent[index];
            object oldObj = previousContent[index];
            if (this.ObjectIsSameWithInprecision(newObj, oldObj))
            {
                // compress (by using null, instead of value, which is same as before)
                compressedValues++;
                compressedContent[index] = null;
            }
            else
            {
                compressedContent[index] = newObj;

                // value changed, we don't replace it with null
                // new value is null (like a compressed value): we have to mark it so it STAYS null instead of being replaced with previous value
                if (newObj == null)
                {
                    if (valuesThatAreChangedToNull == null)
                    {
                        valuesThatAreChangedToNull = new Queue<int>(currentContent.Length);
                    }
                    valuesThatAreChangedToNull.Enqueue(index);
                }
            }
        }

        // Only send the list of compressed fields if we actually compressed 1 or more fields.
        if (compressedValues > 0)
        {
            if (compressedValues == currentContent.Length - SyncFirstValue)
            {
                // all values are compressed to null, we have nothing to send
                return null;
            }

            compressedContent[SyncCompressed] = true;
            if (valuesThatAreChangedToNull != null)
            {
                compressedContent[SyncNullValues] = valuesThatAreChangedToNull.ToArray(); // data that is actually null (not just cause we didn't want to send it)
            }
        }

        compressedContent[SyncViewId] = currentContent[SyncViewId];
        return compressedContent;    // some data was compressed but we need to send something
    }

    public const int SyncViewId     = 0;
    public const int SyncCompressed = 1;
    public const int SyncNullValues = 2;
    public const int SyncFirstValue = 3;


    // startIndex should be the index of the first actual data-value (3 in PUN's case, as 0=viewId, 1=(bool)compressed, 2=(int[])values that are now null)
    // returns the incomingData with modified content. any object being null (means: value unchanged) gets replaced with a previously sent value. incomingData is being modified
    private object[] DeltaCompressionRead(object[] lastOnSerializeDataReceived, object[] incomingData)
    {
        if ((bool)incomingData[SyncCompressed] == false)
        {
            // index 1 marks "compressed" as being true.
            return incomingData;
        }

        // Compression was applied (as data[1] == true)
        // we need a previous "full" list of values to restore values that are null in this msg. else, ignore this
        if (lastOnSerializeDataReceived == null)
        {
            return null;
        }


        int[] indexesThatAreChangedToNull = incomingData[(byte)2] as int[];
        for (int index = SyncFirstValue; index < incomingData.Length; index++)
        {
            if (indexesThatAreChangedToNull != null && indexesThatAreChangedToNull.Contains(index))
            {
                continue;   // if a value was set to null in this update, we don't need to fetch it from an earlier update
            }
            if (incomingData[index] == null)
            {
                // we replace null values in this received msg unless a index is in the "changed to null" list
                object lastValue = lastOnSerializeDataReceived[index];
                incomingData[index] = lastValue;
            }
        }

        return incomingData;
    }


    /// <summary>
    /// Returns true if both objects are almost identical.
    /// Used to check whether two objects are similar enough to skip an update.
    /// </summary>
    bool ObjectIsSameWithInprecision(object one, object two)
    {
        if (one == null || two == null)
        {
            return one == null && two == null;
        }

        if (!one.Equals(two))
        {
            // if A is not B, lets check if A is almost B
            if (one is Vector3)
            {
                Vector3 a = (Vector3)one;
                Vector3 b = (Vector3)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForVectorSynchronization))
                {
                    return true;
                }
            }
            else if (one is Vector2)
            {
                Vector2 a = (Vector2)one;
                Vector2 b = (Vector2)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForVectorSynchronization))
                {
                    return true;
                }
            }
            else if (one is Quaternion)
            {
                Quaternion a = (Quaternion)one;
                Quaternion b = (Quaternion)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForQuaternionSynchronization))
                {
                    return true;
                }
            }
            else if (one is float)
            {
                float a = (float)one;
                float b = (float)two;
                if (a.AlmostEquals(b, PhotonNetwork.precisionForFloatSynchronization))
                {
                    return true;
                }
            }

            // one does not equal two
            return false;
        }

        return true;
    }

    internal protected static bool GetMethod(MonoBehaviour monob, string methodType, out MethodInfo mi)
    {
        mi = null;

        if (monob == null || string.IsNullOrEmpty(methodType))
        {
            return false;
        }

        List<MethodInfo> methods = SupportClassPun.GetMethods(monob.GetType(), null);
        for (int index = 0; index < methods.Count; index++)
        {
            MethodInfo methodInfo = methods[index];
            if (methodInfo.Name.Equals(methodType))
            {
                mi = methodInfo;
                return true;
            }
        }

        return false;
    }

    /// <summary>Internally used to detect the current scene and load it if PhotonNetwork.automaticallySyncScene is enabled.</summary>
    internal protected void LoadLevelIfSynced()
    {
        if (!PhotonNetwork.automaticallySyncScene || PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
        {
            return;
        }

        // check if "current level" is set in props
        if (!PhotonNetwork.room.customProperties.ContainsKey(NetworkingPeer.CurrentSceneProperty))
        {
            return;
        }

        // if loaded level is not the one defined my master in props, load that level
        object sceneId = PhotonNetwork.room.customProperties[NetworkingPeer.CurrentSceneProperty];
        if (sceneId is int)
        {
            if (SceneManagerHelper.ActiveSceneBuildIndex != (int)sceneId)
                PhotonNetwork.LoadLevel((int)sceneId);
        }
        else if (sceneId is string)
        {
            if (SceneManagerHelper.ActiveSceneName != (string)sceneId)
                PhotonNetwork.LoadLevel((string)sceneId);
        }
    }

    protected internal void SetLevelInPropsIfSynced(object levelId)
    {
        if (!PhotonNetwork.automaticallySyncScene || !PhotonNetwork.isMasterClient || PhotonNetwork.room == null)
        {
            return;
        }
        if (levelId == null)
        {
            Debug.LogError("Parameter levelId can't be null!");
            return;
        }

        // check if "current level" is already set in props
        if (PhotonNetwork.room.customProperties.ContainsKey(NetworkingPeer.CurrentSceneProperty))
        {
            object levelIdInProps = PhotonNetwork.room.customProperties[NetworkingPeer.CurrentSceneProperty];
            if (levelIdInProps is int && SceneManagerHelper.ActiveSceneBuildIndex == (int)levelIdInProps)
            {
                return;
            }
            if (levelIdInProps is string && SceneManagerHelper.ActiveSceneName != null && SceneManagerHelper.ActiveSceneName.Equals((string)levelIdInProps))
            {
                return;
            }
        }

        // current level is not yet in props, so this client has to set it
        Hashtable setScene = new Hashtable();
        if (levelId is int) setScene[NetworkingPeer.CurrentSceneProperty] = (int)levelId;
        else if (levelId is string) setScene[NetworkingPeer.CurrentSceneProperty] = (string)levelId;
        else Debug.LogError("Parameter levelId must be int or string!");

        PhotonNetwork.room.SetCustomProperties(setScene);
        this.SendOutgoingCommands();    // send immediately! because: in most cases the client will begin to load and not send for a while
    }

    public void SetApp(string appId, string gameVersion)
    {
        this.AppId = appId.Trim();

        if (!string.IsNullOrEmpty(gameVersion))
        {
            PhotonNetwork.gameVersion = gameVersion.Trim();
        }
    }


    public bool WebRpc(string uriPath, object parameters)
    {
        Dictionary<byte, object> opParameters = new Dictionary<byte, object>();
        opParameters.Add(ParameterCode.UriPath, uriPath);
        opParameters.Add(ParameterCode.WebRpcParameters, parameters);

        return this.OpCustom(OperationCode.WebRpc, opParameters, true);

    }
}
