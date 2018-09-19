// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonNetwork.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using System.Diagnostics;
using UnityEngine;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Hashtable = ExitGames.Client.Photon.Hashtable;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif


/// <summary>
/// The main class to use the PhotonNetwork plugin.
/// This class is static.
/// </summary>
/// \ingroup publicApi
public static class PhotonNetwork
{
    /// <summary>Version number of PUN. Also used in GameVersion to separate client version from each other.</summary>
    public const string versionPUN = "1.76";

    /// <summary>Version string for your this build. Can be used to separate incompatible clients. Sent during connect.</summary>
    /// <remarks>This is only sent when you connect so that is also the place you set it usually (e.g. in ConnectUsingSettings).</remarks>
    public static string gameVersion { get; set; }

    /// <summary>
    /// This Monobehaviour allows Photon to run an Update loop.
    /// </summary>
    internal static readonly PhotonHandler photonMono;

    /// <summary>
    /// Photon peer class that implements LoadBalancing in PUN.
    /// Primary use is internal (by PUN itself).
    /// </summary>
    internal static NetworkingPeer networkingPeer;

    /// <summary>
    /// The maximum number of assigned PhotonViews <i>per player</i> (or scene). See the [General Documentation](@ref general) topic "Limitations" on how to raise this limitation.
    /// </summary>
    public static readonly int MAX_VIEW_IDS = 1000; // VIEW & PLAYER LIMIT CAN BE EASILY CHANGED, SEE DOCS


    /// <summary>Name of the PhotonServerSettings file (used to load and by PhotonEditor to save new files).</summary>
    internal const string serverSettingsAssetFile = "PhotonServerSettings";

    /// <summary>Path to the PhotonServerSettings file (used by PhotonEditor).</summary>
    internal const string serverSettingsAssetPath = "Assets/Photon Unity Networking/Resources/" + PhotonNetwork.serverSettingsAssetFile + ".asset";


    /// <summary>Serialized server settings, written by the Setup Wizard for use in ConnectUsingSettings.</summary>
    public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load(PhotonNetwork.serverSettingsAssetFile, typeof(ServerSettings));

    /// <summary>Currently used server address (no matter if master or game server).</summary>
    public static string ServerAddress { get { return (networkingPeer != null) ? networkingPeer.ServerAddress : "<not connected>"; } }

    /// <summary>
    /// False until you connected to Photon initially. True in offline mode, while connected to any server and even while switching servers.
    /// </summary>
    public static bool connected
    {
        get
        {
            if (offlineMode)
            {
                return true;
            }

            if (networkingPeer == null)
            {
                return false;
            }

            return !networkingPeer.IsInitialConnect && networkingPeer.State != ClientState.PeerCreated && networkingPeer.State != ClientState.Disconnected && networkingPeer.State != ClientState.Disconnecting && networkingPeer.State != ClientState.ConnectingToNameServer;
        }
    }

    /// <summary>
    /// True when you called ConnectUsingSettings (or similar) until the low level connection to Photon gets established.
    /// </summary>
    public static bool connecting
    {
        get { return networkingPeer.IsInitialConnect && !offlineMode; }
    }

    /// <summary>
    /// A refined version of connected which is true only if your connection to the server is ready to accept operations like join, leave, etc.
    /// </summary>
    public static bool connectedAndReady
    {
        get
        {
            // connected property will check offlineMode and networkingPeer being null
            if (!connected)
            {
                return false;
            }

            if (offlineMode)
            {
                return true;
            }

            switch (connectionStateDetailed)
            {
                case ClientState.PeerCreated:
                case ClientState.Disconnected:
                case ClientState.Disconnecting:
                case ClientState.Authenticating:
                case ClientState.ConnectingToGameserver:
                case ClientState.ConnectingToMasterserver:
                case ClientState.ConnectingToNameServer:
                case ClientState.Joining:
                    return false;   // we are not ready to execute any operations
            }

            return true;
        }
    }

    /// <summary>
    /// Simplified connection state
    /// </summary>
    public static ConnectionState connectionState
    {
        get
        {
            if (offlineMode)
            {
                return ConnectionState.Connected;
            }

            if (networkingPeer == null)
            {
                return ConnectionState.Disconnected;
            }

            switch (networkingPeer.PeerState)
            {
                case PeerStateValue.Disconnected:
                    return ConnectionState.Disconnected;
                case PeerStateValue.Connecting:
                    return ConnectionState.Connecting;
                case PeerStateValue.Connected:
                    return ConnectionState.Connected;
                case PeerStateValue.Disconnecting:
                    return ConnectionState.Disconnecting;
                case PeerStateValue.InitializingApplication:
                    return ConnectionState.InitializingApplication;
            }

            return ConnectionState.Disconnected;
        }
    }

    /// <summary>
    /// Detailed connection state (ignorant of PUN, so it can be "disconnected" while switching servers).
    /// </summary>
    /// <remarks>
    /// In OfflineMode, this is ClientState.Joined (after create/join) or it is ConnectedToMaster in all other cases.
    /// </remarks>
    public static ClientState connectionStateDetailed
    {
        get
        {
            if (offlineMode)
            {
                return (offlineModeRoom != null) ? ClientState.Joined : ClientState.ConnectedToMaster;
            }

            if (networkingPeer == null)
            {
                return ClientState.Disconnected;
            }

            return networkingPeer.State;
        }
    }

    /// <summary>The server (type) this client is currently connected or connecting to.</summary>
    /// <remarks>Photon uses 3 different roles of servers: Name Server, Master Server and Game Server.</remarks>
    public static ServerConnection Server { get { return (PhotonNetwork.networkingPeer != null) ? PhotonNetwork.networkingPeer.Server : ServerConnection.NameServer; } }

    /// <summary>
    /// A user's authentication values used during connect for Custom Authentication with Photon (and a custom service/community).
    /// Set these before calling Connect if you want custom authentication.
    /// </summary>
    /// <remarks>
    /// If authentication fails for any values, PUN will call your implementation of OnCustomAuthenticationFailed(string debugMsg).
    /// See: PhotonNetworkingMessage.OnCustomAuthenticationFailed
    /// </remarks>
    public static AuthenticationValues AuthValues
    {
        get { return (networkingPeer != null) ? networkingPeer.AuthValues : null; }
        set { if (networkingPeer != null) networkingPeer.AuthValues = value; }
    }

    /// <summary>
    /// Get the room we're currently in. Null if we aren't in any room.
    /// </summary>
    public static Room room
    {
        get
        {
            if (isOfflineMode)
            {
                return offlineModeRoom;
            }

            return networkingPeer.CurrentRoom;
        }
    }

    /// <summary>If true, Instantiate methods will check if you are in a room and fail if you are not.</summary>
    /// <remarks>
    /// Instantiating anything outside of a specific room is very likely to break things.
    /// Turn this off only if you know what you do.</remarks>
    public static bool InstantiateInRoomOnly = true;

    /// <summary>
    /// Network log level. Controls how verbose PUN is.
    /// </summary>
    public static PhotonLogLevel logLevel = PhotonLogLevel.ErrorsOnly;

    /// <summary>
    /// The local PhotonPlayer. Always available and represents this player.
    /// CustomProperties can be set before entering a room and will be synced as well.
    /// </summary>
    public static PhotonPlayer player
    {
        get
        {
            if (networkingPeer == null)
            {
                return null; // Surpress ExitApplication errors
            }

            return networkingPeer.LocalPlayer;
        }
    }

    /// <summary>
    /// The Master Client of the current room or null (outside of rooms).
    /// </summary>
    /// <remarks>
    /// Can be used as "authoritative" client/player to make descisions, run AI or other.
    ///
    /// If the current Master Client leaves the room (leave/disconnect), the server will quickly assign someone else.
    /// If the current Master Client times out (closed app, lost connection, etc), messages sent to this client are
    /// effectively lost for the others! A timeout can take 10 seconds in which no Master Client is active.
    ///
    /// Implement the method IPunCallbacks.OnMasterClientSwitched to be called when the Master Client switched.
    ///
    /// Use PhotonNetwork.SetMasterClient, to switch manually to some other player / client.
    ///
    /// With offlineMode == true, this always returns the PhotonNetwork.player.
    /// </remarks>
    public static PhotonPlayer masterClient
    {
        get
        {
            if (offlineMode)
            {
                return PhotonNetwork.player;
            }

            if (networkingPeer == null)
            {
                return null;
            }

            return networkingPeer.GetPlayerWithId(networkingPeer.mMasterClientId);
        }
    }

    /// <summary>
    /// Set to synchronize the player's nickname with everyone in the room(s) you enter. This sets PhotonPlayer.name.
    /// </summary>
    /// <remarks>
    /// The playerName is just a nickname and does not have to be unique or backed up with some account.<br/>
    /// Set the value any time (e.g. before you connect) and it will be available to everyone you play with.<br/>
    /// Access the names of players by: PhotonPlayer.name. <br/>
    /// PhotonNetwork.otherPlayers is a list of other players - each contains the playerName the remote player set.
    /// </remarks>
    public static string playerName
    {
        get
        {
            return networkingPeer.PlayerName;
        }

        set
        {
            networkingPeer.PlayerName = value;
        }
    }

    /// <summary>The list of players in the current room, including the local player.</summary>
    /// <remarks>
    /// This list is only valid, while the client is in a room.
    /// It automatically gets updated when someone joins or leaves.
    ///
    /// This can be used to list all players in a room.
    /// Each player's PhotonPlayer.customProperties are accessible (set and synchronized via
    /// PhotonPlayer.SetCustomProperties).
    ///
    /// You can use a PhotonPlayer.TagObject to store an arbitrary object for reference.
    /// That is not synchronized via the network.
    /// </remarks>
    public static PhotonPlayer[] playerList
    {
        get
        {
            if (networkingPeer == null)
                return new PhotonPlayer[0];

            return networkingPeer.mPlayerListCopy;
        }
    }

    /// <summary>The list of players in the current room, excluding the local player.</summary>
    /// <remarks>
    /// This list is only valid, while the client is in a room.
    /// It automatically gets updated when someone joins or leaves.
    ///
    /// This can be used to list all other players in a room.
    /// Each player's PhotonPlayer.customProperties are accessible (set and synchronized via
    /// PhotonPlayer.SetCustomProperties).
    ///
    /// You can use a PhotonPlayer.TagObject to store an arbitrary object for reference.
    /// That is not synchronized via the network.
    /// </remarks>
    public static PhotonPlayer[] otherPlayers
    {
        get
        {
            if (networkingPeer == null)
                return new PhotonPlayer[0];

            return networkingPeer.mOtherPlayerListCopy;
        }
    }

    /// <summary>
    /// Read-only list of friends, their online status and the room they are in. Null until initialized by a FindFriends call.
    /// </summary>
    /// <remarks>
    /// Do not modify this list!
    /// It is internally handled by FindFriends and only available to read the values.
    /// The value of FriendListAge tells you how old the data is in milliseconds.
    ///
    /// Don't get this list more often than useful (> 10 seconds). In best case, keep the list you fetch really short.
    /// You could (e.g.) get the full list only once, then request a few updates only for friends who are online.
    /// After a while (e.g. 1 minute), you can get the full list again (to update online states).
    /// </remarks>
    public static List<FriendInfo> Friends { get; internal set; }

    /// <summary>
    /// Age of friend list info (in milliseconds). It's 0 until a friend list is fetched.
    /// </summary>
    public static int FriendsListAge
    {
        get { return (networkingPeer != null) ? networkingPeer.FriendListAge : 0; }
    }

    /// <summary>
    /// The minimum difference that a Vector2 or Vector3(e.g. a transforms rotation) needs to change before we send it via a PhotonView's OnSerialize/ObservingComponent.
    /// </summary>
    /// <remarks>
    /// Note that this is the sqrMagnitude. E.g. to send only after a 0.01 change on the Y-axix, we use 0.01f*0.01f=0.0001f. As a remedy against float inaccuracy we use 0.000099f instead of 0.0001f.
    /// </remarks>
    public static float precisionForVectorSynchronization = 0.000099f;

    /// <summary>
    /// The minimum angle that a rotation needs to change before we send it via a PhotonView's OnSerialize/ObservingComponent.
    /// </summary>
    public static float precisionForQuaternionSynchronization = 1.0f;

    /// <summary>
    /// The minimum difference between floats before we send it via a PhotonView's OnSerialize/ObservingComponent.
    /// </summary>
    public static float precisionForFloatSynchronization = 0.01f;

    /// <summary>
    /// While enabled, the MonoBehaviours on which we call RPCs are cached, avoiding costly GetComponents<MonoBehaviour>() calls.
    /// </summary>
    /// <remarks>
    /// RPCs are called on the MonoBehaviours of a target PhotonView. Those have to be found via GetComponents.
    ///
    /// When set this to true, the list of MonoBehaviours gets cached in each PhotonView.
    /// You can use photonView.RefreshRpcMonoBehaviourCache() to manually refresh a PhotonView's
    /// list of MonoBehaviours on demand (when a new MonoBehaviour gets added to a networked GameObject, e.g.).
    /// </remarks>
    public static bool UseRpcMonoBehaviourCache;

    /// <summary>
    /// While enabled (true), Instantiate uses PhotonNetwork.PrefabCache to keep game objects in memory (improving instantiation of the same prefab).
    /// </summary>
    /// <remarks>
    /// Setting UsePrefabCache to false during runtime will not clear PrefabCache but will ignore it right away.
    /// You could clean and modify the cache yourself. Read its comments.
    /// </remarks>
    public static bool UsePrefabCache = true;

    /// <summary>
    /// An Object Pool can be used to keep and reuse instantiated object instances. It replaced Unity's default Instantiate and Destroy methods.
    /// </summary>
    /// <remarks>
    /// To use a GameObject pool, implement IPunPrefabPool and assign it here.
    /// Prefabs are identified by name.
    /// </remarks>
    public static IPunPrefabPool PrefabPool { get { return networkingPeer.ObjectPool; } set { networkingPeer.ObjectPool = value; }}

    /// <summary>
    /// Keeps references to GameObjects for frequent instantiation (out of memory instead of loading the Resources).
    /// </summary>
    /// <remarks>
    /// You should be able to modify the cache anytime you like, except while Instantiate is used. Best do it only in the main-Thread.
    /// </remarks>
    public static Dictionary<string, GameObject> PrefabCache = new Dictionary<string, GameObject>();

    /// <summary>
    /// If not null, this is the (exclusive) list of GameObjects that get called by PUN SendMonoMessage().
    /// </summary>
    /// <remarks>
    /// For all callbacks defined in PhotonNetworkingMessage, PUN will use SendMonoMessage and
    /// call FindObjectsOfType() to find all scripts and GameObjects that might want a callback by PUN.
    ///
    /// PUN callbacks are not very frequent (in-game, property updates are most frequent) but
    /// FindObjectsOfType is time consuming and with a large number of GameObjects, performance might
    /// suffer.
    ///
    /// Optionally, SendMonoMessageTargets can be used to supply a list of target GameObjects. This
    /// skips the FindObjectsOfType() but any GameObject that needs callbacks will have to Add itself
    /// to this list.
    ///
    /// If null, the default behaviour is to do a SendMessage on each GameObject with a MonoBehaviour.
    /// </remarks>
    public static HashSet<GameObject> SendMonoMessageTargets;


    /// <summary>
    /// Defines which classes can contain PUN Callback implementations.
    /// </summary>
    /// <remarks>
    /// This provides the option to optimize your runtime for speed.<br/>
    /// The more specific this Type is, the fewer classes will be checked with reflection for callback methods.
    /// </remarks>
    public static Type SendMonoMessageTargetType = typeof(MonoBehaviour);

    /// <summary>
    /// Can be used to skip starting RPCs as Coroutine, which can be a performance issue.
    /// </summary>
    public static bool StartRpcsAsCoroutine = true;

    /// <summary>
    /// Offline mode can be set to re-use your multiplayer code in singleplayer game modes.
    /// When this is on PhotonNetwork will not create any connections and there is near to
    /// no overhead. Mostly usefull for reusing RPC's and PhotonNetwork.Instantiate
    /// </summary>
    public static bool offlineMode
    {
        get
        {
            return isOfflineMode;
        }

        set
        {
            if (value == isOfflineMode)
            {
                return;
            }

            if (value && connected)
            {
                Debug.LogError("Can't start OFFLINE mode while connected!");
                return;
            }

            if (networkingPeer.PeerState != PeerStateValue.Disconnected)
            {
                networkingPeer.Disconnect(); // Cleanup (also calls OnLeftRoom to reset stuff)
            }
            isOfflineMode = value;
            if (isOfflineMode)
            {
                networkingPeer.ChangeLocalID(-1);
                NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
            }
            else
            {
                offlineModeRoom = null;
                networkingPeer.ChangeLocalID(-1);
            }
        }
    }

    private static bool isOfflineMode = false;
    private static Room offlineModeRoom = null;


    /// <summary>Only used in Unity Networking. In PUN, set the number of players in PhotonNetwork.CreateRoom.</summary>
    [Obsolete("Used for compatibility with Unity networking only.")]
    public static int maxConnections;

    /// <summary>Defines if all clients in a room should load the same level as the Master Client (if that used PhotonNetwork.LoadLevel).</summary>
    /// <remarks>
    /// To synchronize the loaded level, the Master Client should use PhotonNetwork.LoadLevel.
    /// All clients will load the new scene when they get the update or when they join.
    ///
    /// Internally, a Custom Room Property is set for the loaded scene. When a client reads that
    /// and is not in the same scene yet, it will immediately pause the Message Queue
    /// (PhotonNetwork.isMessageQueueRunning = false) and load. When the scene finished loading,
    /// PUN will automatically re-enable the Message Queue.
    /// </remarks>
    public static bool automaticallySyncScene
    {
        get
        {
            return _mAutomaticallySyncScene;
        }
        set
        {
            _mAutomaticallySyncScene = value;
            if (_mAutomaticallySyncScene && room != null)
            {
                networkingPeer.LoadLevelIfSynced();
            }
        }
    }

    private static bool _mAutomaticallySyncScene = false;

    /// <summary>
    /// This setting defines per room, if network-instantiated GameObjects (with PhotonView) get cleaned up when the creator of it leaves.
    /// </summary>
    /// <remarks>
    /// This setting is done per room. It can't be changed in the room and it will override the settings of individual clients.
    ///
    /// If room.AutoCleanUp is enabled in a room, the PUN clients will destroy a player's GameObjects on leave.
    /// This includes GameObjects manually instantiated (via RPCs, e.g.).
    /// When enabled, the server will clean RPCs, instantiated GameObjects and PhotonViews of the leaving player, too. and
    /// Players who join after someone left, won't get the events of that player anymore.
    ///
    /// Under the hood, this setting is stored as a Custom Room Property.
    /// Enabled by default.
    /// </remarks>
    public static bool autoCleanUpPlayerObjects
    {
        get
        {
            return m_autoCleanUpPlayerObjects;
        }
        set
        {
            if (room != null)
                Debug.LogError("Setting autoCleanUpPlayerObjects while in a room is not supported.");
            else m_autoCleanUpPlayerObjects = value;
        }
    }

    private static bool m_autoCleanUpPlayerObjects = true;

    /// <summary>
    /// Set in PhotonServerSettings asset. Defines if the PhotonNetwork should join the "lobby" when connected to the Master server.
    /// </summary>
    /// <remarks>
    /// If this is false, OnConnectedToMaster() will be called when connection to the Master is available.
    /// OnJoinedLobby() will NOT be called if this is false.
    ///
    /// Enabled by default.
    ///
    /// The room listing will not become available.
    /// Rooms can be created and joined (randomly) without joining the lobby (and getting sent the room list).
    /// </remarks>
    public static bool autoJoinLobby
    {
        get
        {
            return PhotonNetwork.PhotonServerSettings.JoinLobby;
        }
        set
        {
            PhotonNetwork.PhotonServerSettings.JoinLobby = value;
        }
    }


    /// <summary>
    /// Set in PhotonServerSettings asset. Enable to get a list of active lobbies from the Master Server.
    /// </summary>
    /// <remarks>
    /// Lobby Statistics can be useful if a game uses multiple lobbies and you want
    /// to show activity of each to players.
    ///
    /// This value is stored in PhotonServerSettings.
    ///
    /// PhotonNetwork.LobbyStatistics is updated when you connect to the Master Server.
    /// There is also a callback PunBehaviour.
    /// </remarks>
    public static bool EnableLobbyStatistics
    {
        get
        {
            return PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics;
        }
        set
        {
            PhotonNetwork.PhotonServerSettings.EnableLobbyStatistics = value;
        }
    }

    /// <summary>
    /// If turned on, the Master Server will provide information about active lobbies for this application.
    /// </summary>
    /// <remarks>
    /// Lobby Statistics can be useful if a game uses multiple lobbies and you want
    /// to show activity of each to players. Per lobby, you get: name, type, room- and player-count.
    ///
    /// PhotonNetwork.LobbyStatistics is updated when you connect to the Master Server.
    /// There is also a callback PunBehaviour.OnLobbyStatisticsUpdate, which you should implement
    /// to update your UI (e.g.).
    ///
    /// Lobby Statistics are not turned on by default.
    /// Enable them in the PhotonServerSettings file of the project.
    /// </remarks>
    public static List<TypedLobbyInfo> LobbyStatistics
    {
        get { return PhotonNetwork.networkingPeer.LobbyStatistics; }
        // only available to reset the state conveniently. done by state updates of PUN
        private set { PhotonNetwork.networkingPeer.LobbyStatistics = value; }
    }


    /// <summary>True while this client is in a lobby.</summary>
    /// <remarks>
    /// Implement IPunCallbacks.OnReceivedRoomListUpdate() for a notification when the list of rooms
    /// becomes available or updated.
    ///
    /// You are automatically leaving any lobby when you join a room!
    /// Lobbies only exist on the Master Server (whereas rooms are handled by Game Servers).
    /// </remarks>
    public static bool insideLobby
    {
        get
        {
            return networkingPeer.insideLobby;
        }
    }

    /// <summary>
    /// The lobby that will be used when PUN joins a lobby or creates a game.
    /// </summary>
    /// <remarks>
    /// The default lobby uses an empty string as name.
    /// PUN will enter a lobby on the Master Server if autoJoinLobby is set to true.
    /// So when you connect or leave a room, PUN automatically gets you into a lobby again.
    ///
    /// Check PhotonNetwork.insideLobby if the client is in a lobby.
    /// (@ref masterServerAndLobby)
    /// </remarks>
    public static TypedLobby lobby
    {
        get { return networkingPeer.lobby; }
        set { networkingPeer.lobby = value; }
    }

    /// <summary>
    /// Defines how many times per second PhotonNetwork should send a package. If you change
    /// this, do not forget to also change 'sendRateOnSerialize'.
    /// </summary>
    /// <remarks>
    /// Less packages are less overhead but more delay.
    /// Setting the sendRate to 50 will create up to 50 packages per second (which is a lot!).
    /// Keep your target platform in mind: mobile networks are slower and less reliable.
    /// </remarks>
    public static int sendRate
    {
        get
        {
            return 1000 / sendInterval;
        }

        set
        {
            sendInterval = 1000 / value;
            if (photonMono != null)
            {
                photonMono.updateInterval = sendInterval;
            }

            if (value < sendRateOnSerialize)
            {
                // sendRateOnSerialize needs to be <= sendRate
                sendRateOnSerialize = value;
            }
        }
    }

    /// <summary>
    /// Defines how many times per second OnPhotonSerialize should be called on PhotonViews.
    /// </summary>
    /// <remarks>
    /// Choose this value in relation to PhotonNetwork.sendRate. OnPhotonSerialize will create updates and messages to be sent.<br/>
    /// A lower rate takes up less performance but will cause more lag.
    /// </remarks>
    public static int sendRateOnSerialize
    {
        get
        {
            return 1000 / sendIntervalOnSerialize;
        }

        set
        {
            if (value > sendRate)
            {
                Debug.LogError("Error: Can not set the OnSerialize rate higher than the overall SendRate.");
                value = sendRate;
            }

            sendIntervalOnSerialize = 1000 / value;
            if (photonMono != null)
            {
                photonMono.updateIntervalOnSerialize = sendIntervalOnSerialize;
            }
        }
    }

    private static int sendInterval = 50; // in miliseconds.

    private static int sendIntervalOnSerialize = 100; // in miliseconds. I.e. 100 = 100ms which makes 10 times/second

    /// <summary>
    /// Can be used to pause dispatching of incoming evtents (RPCs, Instantiates and anything else incoming).
    /// </summary>
    /// <remarks>
    /// While IsMessageQueueRunning == false, the OnPhotonSerializeView calls are not done and nothing is sent by
    /// a client. Also, incoming messages will be queued until you re-activate the message queue.
    ///
    /// This can be useful if you first want to load a level, then go on receiving data of PhotonViews and RPCs.
    /// The client will go on receiving and sending acknowledgements for incoming packages and your RPCs/Events.
    /// This adds "lag" and can cause issues when the pause is longer, as all incoming messages are just queued.
    /// </remarks>
    public static bool isMessageQueueRunning
    {
        get
        {
            return m_isMessageQueueRunning;
        }

        set
        {
            if (value) PhotonHandler.StartFallbackSendAckThread();
            networkingPeer.IsSendingOnlyAcks = !value;
            m_isMessageQueueRunning = value;
        }
    }

    /// <summary>Backup for property isMessageQueueRunning.</summary>
    private static bool m_isMessageQueueRunning = true;

    /// <summary>
    /// Used once per dispatch to limit unreliable commands per channel (so after a pause, many channels can still cause a lot of unreliable commands)
    /// </summary>
    public static int unreliableCommandsLimit
    {
        get
        {
            return networkingPeer.LimitOfUnreliableCommands;
        }

        set
        {
            networkingPeer.LimitOfUnreliableCommands = value;
        }
    }

    /// <summary>
    /// Photon network time, synched with the server.
    /// </summary>
    /// <remarks>
    /// v1.55<br/>
    /// This time value depends on the server's Environment.TickCount. It is different per server
    /// but inside a Room, all clients should have the same value (Rooms are on one server only).<br/>
    /// This is not a DateTime!<br/>
    ///
    /// Use this value with care: <br/>
    /// It can start with any positive value.<br/>
    /// It will "wrap around" from 4294967.295 to 0!
    /// </remarks>
    public static double time
    {
        get
        {
            uint u = (uint)ServerTimestamp;
            double t = u;
            return t / 1000;
        }
    }

    /// <summary>
    /// The current server's millisecond timestamp.
    /// </summary>
    /// <remarks>
    /// This can be useful to sync actions and events on all clients in one room.
    /// The timestamp is based on the server's Environment.TickCount.
    ///
    /// It will overflow from a positive to a negative value every so often, so
    /// be careful to use only time-differences to check the time delta when things
    /// happen.
    ///
    /// This is the basis for PhotonNetwork.time.
    /// </remarks>
    public static int ServerTimestamp
    {
        get
        {
            if (offlineMode)
            {
                if (UsePreciseTimer && startupStopwatch != null && startupStopwatch.IsRunning)
                {
                    return (int)startupStopwatch.ElapsedMilliseconds;
                }
                return Environment.TickCount;
            }

            return networkingPeer.ServerTimeInMilliSeconds;
        }
    }

	/// <summary>If true, PUN will use a Stopwatch to measure time since start/connect. This is more precise than the Environment.TickCount used by default.</summary>
    private static bool UsePreciseTimer = false;
    static Stopwatch startupStopwatch;

    /// <summary>
    /// Defines how long PUN keeps running a "fallback thread" to keep the connection after Unity's OnApplicationPause(true) call.
    /// </summary>
    /// <remarks>
    /// If you set BackgroundTimeout PUN will stop keeping the connection, BackgroundTimeout seconds after OnApplicationPause(true) got called.
    /// That means: After the set time, a regular timeout can happen.
    /// Your application will notice that timeout when it becomes active again.
    ///
    ///
    /// To handle the timeout, implement: OnConnectionFail() (this case will use the cause: DisconnectByServerTimeout).
    ///
    ///
    /// It's best practice to let inactive apps/connections time out after a while but allow taking calls, etc.
    /// So a reasonable value should be found.
    /// We think it could be 60 seconds.
    ///
    /// Set a value greater than 0.001f, if you want to limit how long an app can keep the connection in background.
    ///
    ///
    /// Info:
    /// PUN is running a "fallback thread" to send ACKs to the server, even when Unity is not calling Update() regularly.
    /// This helps keeping the connection while loading scenes and assets and when the app is in the background.
    ///
    /// Note:
    /// Some platforms (e.g. iOS) don't allow to keep a connection while the app is in background.
    /// In those cases, this value does not change anything, the app immediately loses connection in background.
    ///
    /// Unity's OnApplicationPause() callback is broken in some exports (Android) of some Unity versions.
    /// Make sure OnApplicationPause() gets the callbacks you'd expect on the platform you target!
    /// Check PhotonHandler.OnApplicationPause(bool pause), to see the implementation.
    ///
    /// </remarks>
    public static float BackgroundTimeout = 60.0f;

    /// <summary>
    /// Are we the master client?
    /// </summary>
    public static bool isMasterClient
    {
        get
        {
            if (offlineMode)
            {
                return true;
            }
            else
            {
                return networkingPeer.mMasterClientId == player.ID;
            }
        }
    }

    /// <summary>Is true while being in a room (connectionStateDetailed == ClientState.Joined).</summary>
    /// <remarks>
    /// Many actions can only be executed in a room, like Instantiate or Leave, etc.
    /// You can join a room in offline mode, too.
    /// </remarks>
    public static bool inRoom
    {
        get
        {
            // in offline mode, you can be in a room too and connectionStateDetailed then returns Joined like on online mode!
            return connectionStateDetailed == ClientState.Joined;
        }
    }

    /// <summary>
    /// True if we are in a room (client) and NOT the room's masterclient
    /// </summary>
    public static bool isNonMasterClientInRoom
    {
        get
        {
            return !isMasterClient && room != null;
        }
    }

    /// <summary>
    /// The count of players currently looking for a room (available on MasterServer in 5sec intervals).
    /// </summary>
    public static int countOfPlayersOnMaster
    {
        get
        {
            return networkingPeer.PlayersOnMasterCount;
        }
    }

    /// <summary>
    /// Count of users currently playing your app in some room (sent every 5sec by Master Server). Use playerList.Count to get the count of players in the room you're in!
    /// </summary>
    public static int countOfPlayersInRooms
    {
        get
        {
            return networkingPeer.PlayersInRoomsCount;
        }
    }

    /// <summary>
    /// The count of players currently using this application (available on MasterServer in 5sec intervals).
    /// </summary>
    public static int countOfPlayers
    {
        get
        {
            return networkingPeer.PlayersInRoomsCount + networkingPeer.PlayersOnMasterCount;
        }
    }

    /// <summary>
    /// The count of rooms currently in use (available on MasterServer in 5sec intervals).
    /// </summary>
    /// <remarks>
    /// While inside the lobby you can also check the count of listed rooms as: PhotonNetwork.GetRoomList().Length.
    /// Since PUN v1.25 this is only based on the statistic event Photon sends (counting all rooms).
    /// </remarks>
    public static int countOfRooms
    {
        get
        {
            return networkingPeer.RoomsCount;
        }
    }

    /// <summary>
    /// Enables or disables the collection of statistics about this client's traffic.
    /// </summary>
    /// <remarks>
    /// If you encounter issues with clients, the traffic stats are a good starting point to find solutions.
    /// Only with enabled stats, you can use GetVitalStats
    /// </remarks>
    public static bool NetworkStatisticsEnabled
    {
        get
        {
            return networkingPeer.TrafficStatsEnabled;
        }

        set
        {
            networkingPeer.TrafficStatsEnabled = value;
        }
    }

    /// <summary>
    /// Count of commands that got repeated (due to local repeat-timing before an ACK was received).
    /// </summary>
    /// <remarks>
    /// If this value increases a lot, there is a good chance that a timeout disconnect will happen due to bad conditions.
    /// </remarks>
    public static int ResentReliableCommands
    {
        get { return networkingPeer.ResentReliableCommands; }
    }

    /// <summary>Crc checks can be useful to detect and avoid issues with broken datagrams. Can be enabled while not connected.</summary>
    public static bool CrcCheckEnabled
    {
        get { return networkingPeer.CrcEnabled; }
        set
        {
            if (!connected && !connecting)
            {
                networkingPeer.CrcEnabled = value;
            }
            else
            {
                Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + networkingPeer.CrcEnabled);
            }
        }
    }

    /// <summary>If CrcCheckEnabled, this counts the incoming packages that don't have a valid CRC checksum and got rejected.</summary>
    public static int PacketLossByCrcCheck
    {
        get { return networkingPeer.PacketLossByCrc; }
    }

    /// <summary>Defines the number of times a reliable message can be resent before not getting an ACK for it will trigger a disconnect. Default: 5.</summary>
    /// <remarks>Less resends mean quicker disconnects, while more can lead to much more lag without helping. Min: 3. Max: 10.</remarks>
    public static int MaxResendsBeforeDisconnect
    {
        get { return networkingPeer.SentCountAllowance; }
        set
        {
            if (value < 3) value = 3;
            if (value > 10) value = 10;
            networkingPeer.SentCountAllowance = value;
        }
    }

    /// <summary>In case of network loss, reliable messages can be repeated quickly up to 3 times.</summary>
    /// <remarks>
    /// When reliable messages get lost more than once, subsequent repeats are delayed a bit
    /// to allow the network to recover.<br/>
    /// With this option, the repeats 2 and 3 can be sped up. This can help avoid timeouts but
    /// also it increases the speed in which gaps are closed.<br/>
    /// When you set this, increase PhotonNetwork.MaxResendsBeforeDisconnect to 6 or 7.
    /// </remarks>
    public static int QuickResends
    {
        get { return networkingPeer.QuickResendAttempts; }
        set
        {
            if (value < 0) value = 0;
            if (value > 3) value = 3;
            networkingPeer.QuickResendAttempts = (byte)value;
        }
    }

    /// <summary>
    /// Defines the delegate usable in OnEventCall.
    /// </summary>
    /// <remarks>Any eventCode &lt; 200 will be forwarded to your delegate(s).</remarks>
    /// <param name="eventCode">The code assigend to the incoming event.</param>
    /// <param name="content">The content the sender put into the event.</param>
    /// <param name="senderId">The ID of the player who sent the event. It might be 0, if the "room" sent the event.</param>
    public delegate void EventCallback(byte eventCode, object content, int senderId);

    /// <summary>Register your RaiseEvent handling methods here by using "+=".</summary>
    /// <remarks>Any eventCode &lt; 200 will be forwarded to your delegate(s).</remarks>
    /// <see cref="RaiseEvent"/>
    public static EventCallback OnEventCall;


    internal static int lastUsedViewSubId = 0;  // each player only needs to remember it's own (!) last used subId to speed up assignment
    internal static int lastUsedViewSubIdStatic = 0;  // per room, the master is able to instantiate GOs. the subId for this must be unique too
    internal static List<int> manuallyAllocatedViewIds = new List<int>();

    /// <summary>
    /// Static constructor used for basic setup.
    /// </summary>
    static PhotonNetwork()
    {
        #if UNITY_EDITOR
        if (PhotonServerSettings == null)
        {
            // create PhotonServerSettings
            CreateSettings();
        }

        if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
        {
            //Debug.Log(string.Format("PhotonNetwork.ctor() Not playing {0} {1}", UnityEditor.EditorApplication.isPlaying, UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode));
            return;
        }

        // This can happen when you recompile a script IN play made
        // This helps to surpress some errors, but will not fix breaking
        PhotonHandler[] photonHandlers = GameObject.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
        if (photonHandlers != null && photonHandlers.Length > 0)
        {
            Debug.LogWarning("Unity recompiled. Connection gets closed and replaced. You can connect as 'new' client.");
            foreach (PhotonHandler photonHandler in photonHandlers)
            {
                //Debug.Log("Handler: " + photonHandler + " photonHandler.gameObject: " + photonHandler.gameObject);
                photonHandler.gameObject.hideFlags = 0;
                GameObject.DestroyImmediate(photonHandler.gameObject);
                Component.DestroyImmediate(photonHandler);
            }
        }
        #endif

        Application.runInBackground = true;

        // Set up a MonoBehaviour to run Photon, and hide it
        GameObject photonGO = new GameObject();
        photonMono = (PhotonHandler)photonGO.AddComponent<PhotonHandler>();
        photonGO.name = "PhotonMono";
        photonGO.hideFlags = HideFlags.HideInHierarchy;


        // Set up the NetworkingPeer and use protocol of PhotonServerSettings
        ConnectionProtocol protocol = PhotonNetwork.PhotonServerSettings.Protocol;
        networkingPeer = new NetworkingPeer(string.Empty, protocol);
        networkingPeer.QuickResendAttempts = 2;
        networkingPeer.SentCountAllowance = 7;


        #if UNITY_XBOXONE
        Debug.Log("UNITY_XBOXONE is defined: Using AuthMode 'AuthOnceWss' and EncryptionMode 'DatagramEncryption'.");
        if (!PhotonPeer.NativeDatagramEncrypt)
        {
            Debug.LogError("XB1 builds need a Photon3Unity3d.dll which uses the native PhotonEncryptorPlugin. This dll does not!");
        }

        networkingPeer.AuthMode = AuthModeOption.AuthOnceWss;
        networkingPeer.EncryptionMode = EncryptionMode.DatagramEncryption;
        #endif

        if (UsePreciseTimer)
        {
            Debug.Log("Using Stopwatch as precision timer for PUN.");
            startupStopwatch = new Stopwatch();
            startupStopwatch.Start();
            networkingPeer.LocalMsTimestampDelegate = () => (int)startupStopwatch.ElapsedMilliseconds;
        }

        // Local player
        CustomTypes.Register();
    }

    /// <summary>
    /// While offline, the network protocol can be switched (which affects the ports you can use to connect).
    /// </summary>
    /// <remarks>
    /// When you switch the protocol, make sure to also switch the port for the master server. Default ports are:
    /// TCP: 4530
    /// UDP: 5055
    ///
    /// This could look like this:<br/>
    /// Connect(serverAddress, <udpport|tcpport>, appID, gameVersion)
    ///
    /// Or when you use ConnectUsingSettings(), the PORT in the settings can be switched like so:<br/>
    /// PhotonNetwork.PhotonServerSettings.ServerPort = 4530;
    ///
    /// The current protocol can be read this way:<br/>
    /// PhotonNetwork.networkingPeer.UsedProtocol
    ///
    /// This does not work with the native socket plugin of PUN+ on mobile!
    /// </remarks>
    /// <param name="cp">Network protocol to use as low level connection. UDP is default. TCP is not available on all platforms (see remarks).</param>
    public static void SwitchToProtocol(ConnectionProtocol cp)
    {
        Debug.Log("SwitchToProtocol: " + cp + " PhotonNetwork.connected: " + PhotonNetwork.connected);
        networkingPeer.TransportProtocol = cp;
    }


    /// <summary>Connect to Photon as configured in the editor (saved in PhotonServerSettings file).</summary>
    /// <remarks>
    /// This method will disable offlineMode (which won't destroy any instantiated GOs) and it
    /// will set isMessageQueueRunning to true.
    ///
    /// Your server configuration is created by the PUN Wizard and contains the AppId and
    /// region for Photon Cloud games and the server address if you host Photon yourself.
    /// These settings usually don't change often.
    ///
    /// To ignore the config file and connect anywhere call: PhotonNetwork.ConnectToMaster.
    ///
    /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
    /// https://www.photonengine.com/dashboard
    ///
    /// Connecting to the Photon Cloud might fail due to:
    /// - Invalid AppId (calls: OnFailedToConnectToPhoton(). check exact AppId value)
    /// - Network issues (calls: OnFailedToConnectToPhoton())
    /// - Invalid region (calls: OnConnectionFail() with DisconnectCause.InvalidRegion)
    /// - Subscription CCU limit reached (calls: OnConnectionFail() with DisconnectCause.MaxCcuReached. also calls: OnPhotonMaxCccuReached())
    ///
    /// More about the connection limitations:
    /// http://doc.exitgames.com/en/pun
    /// </remarks>
    /// <param name="gameVersion">This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).</param>
    public static bool ConnectUsingSettings(string gameVersion)
    {
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }
        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: " + serverSettingsAssetFile);
            return false;
        }
        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.NotSet)
        {
            Debug.LogError("You did not select a Hosting Type in your PhotonServerSettings. Please set it up or don't use ConnectUsingSettings().");
            return false;
        }

        SwitchToProtocol(PhotonServerSettings.Protocol);
        networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);

        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            offlineMode = true;
            return true;
        }

        if (offlineMode)
        {
            // someone can set offlineMode in code and then call ConnectUsingSettings() with non-offline settings. Warning for that case:
            Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
        }

        offlineMode = false; // Cleanup offline mode
        isMessageQueueRunning = true;
        networkingPeer.IsInitialConnect = true;

        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
        {
            networkingPeer.IsUsingNameServer = false;
            networkingPeer.MasterServerAddress = (PhotonServerSettings.ServerPort == 0) ? PhotonServerSettings.ServerAddress : PhotonServerSettings.ServerAddress + ":" + PhotonServerSettings.ServerPort;

            return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
        }

        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.BestRegion)
        {
            return ConnectToBestCloudServer(gameVersion);
        }

        return networkingPeer.ConnectToRegionMaster(PhotonServerSettings.PreferredRegion);
    }

    /// <summary>Connect to a Photon Master Server by address, port, appID and game(client) version.</summary>
    /// <remarks>
    /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
    /// https://www.photonengine.com/dashboard
    ///
    /// Connecting to the Photon Cloud might fail due to:
    /// - Invalid AppId (calls: OnFailedToConnectToPhoton(). check exact AppId value)
    /// - Network issues (calls: OnFailedToConnectToPhoton())
    /// - Invalid region (calls: OnConnectionFail() with DisconnectCause.InvalidRegion)
    /// - Subscription CCU limit reached (calls: OnConnectionFail() with DisconnectCause.MaxCcuReached. also calls: OnPhotonMaxCccuReached())
    ///
    /// More about the connection limitations:
    /// http://doc.exitgames.com/en/pun
    /// </remarks>
    /// <param name="masterServerAddress">The server's address (either your own or Photon Cloud address).</param>
    /// <param name="port">The server's port to connect to.</param>
    /// <param name="appID">Your application ID (Photon Cloud provides you with a GUID for your game).</param>
    /// <param name="gameVersion">This client's version number. Users are separated by gameversion (which allows you to make breaking changes).</param>
    public static bool ConnectToMaster(string masterServerAddress, int port, string appID, string gameVersion)
    {
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }

        if (offlineMode)
        {
            offlineMode = false; // Cleanup offline mode
            Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
        }

        if (!isMessageQueueRunning)
        {
            isMessageQueueRunning = true;
            Debug.LogWarning("ConnectToMaster() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
        }

        networkingPeer.SetApp(appID, gameVersion);
        networkingPeer.IsUsingNameServer = false;
        networkingPeer.IsInitialConnect = true;
        networkingPeer.MasterServerAddress = (port == 0) ? masterServerAddress : masterServerAddress + ":" + port;

        return networkingPeer.Connect(networkingPeer.MasterServerAddress, ServerConnection.MasterServer);
    }

	/// <summary>Can be used to reconnect to the master server after a disconnect.</summary>
	/// <remarks>
	/// After losing connection, you can use this to connect a client to the region Master Server again.
	/// Cache the room name you're in and use ReJoin(roomname) to return to a game.
	/// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
	/// </remarks>
    public static bool Reconnect()
    {
        if (string.IsNullOrEmpty(networkingPeer.MasterServerAddress))
        {
            Debug.LogWarning("Reconnect() failed. It seems the client wasn't connected before?! Current state: " + networkingPeer.PeerState);
            return false;
        }

        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("Reconnect() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }

        if (offlineMode)
        {
            offlineMode = false; // Cleanup offline mode
            Debug.LogWarning("Reconnect() disabled the offline mode. No longer offline.");
        }

        if (!isMessageQueueRunning)
        {
            isMessageQueueRunning = true;
            Debug.LogWarning("Reconnect() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
        }

        networkingPeer.IsUsingNameServer = false;
        networkingPeer.IsInitialConnect = false;
        return networkingPeer.ReconnectToMaster();
    }


    /// <summary>When the client lost connection during gameplay, this method attempts to reconnect and rejoin the room.</summary>
    /// <remarks>
    /// This method re-connects directly to the game server which was hosting the room PUN was in before.
    /// If the room was shut down in the meantime, PUN will call OnPhotonJoinRoomFailed and return this client to the Master Server.
    ///
    /// Check the return value, if this client will attempt a reconnect and rejoin (if the conditions are met).
    /// If ReconnectAndRejoin returns false, you can still attempt a Reconnect and ReJoin.
    ///
    /// Similar to PhotonNetwork.ReJoin, this requires you to use unique IDs per player (the UserID).
    /// </remarks>
    /// <returns>False, if there is no known room or game server to return to. Then, this client does not attempt the ReconnectAndRejoin.</returns>
    public static bool ReconnectAndRejoin()
    {
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }
        if (offlineMode)
        {
            offlineMode = false; // Cleanup offline mode
            Debug.LogWarning("ReconnectAndRejoin() disabled the offline mode. No longer offline.");
        }

        if (string.IsNullOrEmpty(networkingPeer.GameServerAddress))
        {
            Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client wasn't connected to a game server before (no address).");
            return false;
        }
        if (networkingPeer.enterRoomParamsCache == null)
        {
            Debug.LogWarning("ReconnectAndRejoin() failed. It seems the client doesn't have any previous room to re-join.");
            return false;
        }

        if (!isMessageQueueRunning)
        {
            isMessageQueueRunning = true;
            Debug.LogWarning("ReconnectAndRejoin() enabled isMessageQueueRunning. Needs to be able to dispatch incoming messages.");
        }

        networkingPeer.IsUsingNameServer = false;
        networkingPeer.IsInitialConnect = false;
        return networkingPeer.ReconnectAndRejoin();
    }


    /// <summary>
    /// Connect to the Photon Cloud region with the lowest ping (on platforms that support Unity's Ping).
    /// </summary>
    /// <remarks>
    /// Will save the result of pinging all cloud servers in PlayerPrefs. Calling this the first time can take +-2 seconds.
    /// The ping result can be overridden via PhotonNetwork.OverrideBestCloudServer(..)
    /// This call can take up to 2 seconds if it is the first time you are using this, all cloud servers will be pinged to check for the best region.
    ///
    /// The PUN Setup Wizard stores your appID in a settings file and applies a server address/port.
    /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
    /// https://www.photonengine.com/dashboard
    ///
    /// Connecting to the Photon Cloud might fail due to:
    /// - Invalid AppId (calls: OnFailedToConnectToPhoton(). check exact AppId value)
    /// - Network issues (calls: OnFailedToConnectToPhoton())
    /// - Invalid region (calls: OnConnectionFail() with DisconnectCause.InvalidRegion)
    /// - Subscription CCU limit reached (calls: OnConnectionFail() with DisconnectCause.MaxCcuReached. also calls: OnPhotonMaxCccuReached())
    ///
    /// More about the connection limitations:
    /// http://doc.exitgames.com/en/pun
    /// </remarks>
    /// <param name="gameVersion">This client's version number. Users are separated from each other by gameversion (which allows you to make breaking changes).</param>
    /// <returns>If this client is going to connect to cloud server based on ping. Even if true, this does not guarantee a connection but the attempt is being made.</returns>
    public static bool ConnectToBestCloudServer(string gameVersion)
    {
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }

        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: " + PhotonNetwork.serverSettingsAssetFile);
            return false;
        }

        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            return PhotonNetwork.ConnectUsingSettings(gameVersion);
        }

        networkingPeer.IsInitialConnect = true;
        networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);

        CloudRegionCode bestFromPrefs = PhotonHandler.BestRegionCodeInPreferences;
        if (bestFromPrefs != CloudRegionCode.none)
        {
            Debug.Log("Best region found in PlayerPrefs. Connecting to: " + bestFromPrefs);
            return networkingPeer.ConnectToRegionMaster(bestFromPrefs);
        }

        bool couldConnect = PhotonNetwork.networkingPeer.ConnectToNameServer();
        return couldConnect;
    }


    /// <summary>
    /// Connects to the Photon Cloud region of choice.
    /// </summary>
    public static bool ConnectToRegion(CloudRegionCode region, string gameVersion)
    {
        if (networkingPeer.PeerState != PeerStateValue.Disconnected)
        {
            Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + networkingPeer.PeerState);
            return false;
        }

        if (PhotonServerSettings == null)
        {
            Debug.LogError("Can't connect: ServerSettings asset must be in any 'Resources' folder as: " + PhotonNetwork.serverSettingsAssetFile);
            return false;
        }

        if (PhotonServerSettings.HostType == ServerSettings.HostingOption.OfflineMode)
        {
            return PhotonNetwork.ConnectUsingSettings(gameVersion);
        }

        networkingPeer.IsInitialConnect = true;
        networkingPeer.SetApp(PhotonServerSettings.AppID, gameVersion);

        if (region != CloudRegionCode.none)
        {
            Debug.Log("ConnectToRegion: " + region);
            return networkingPeer.ConnectToRegionMaster(region);
        }

        return false;
    }

    /// <summary>Overwrites the region that is used for ConnectToBestCloudServer(string gameVersion).</summary>
    /// <remarks>
    /// This will overwrite the result of pinging all cloud servers.<br/>
    /// Use this to allow your users to save a manually selected region in the player preferences.<br/>
    /// Note: You can also use PhotonNetwork.ConnectToRegion to (temporarily) connect to a specific region.
    /// </remarks>
    public static void OverrideBestCloudServer(CloudRegionCode region)
    {
        PhotonHandler.BestRegionCodeInPreferences = region;
    }

    /// <summary>Pings all cloud servers again to find the one with best ping (currently).</summary>
    public static void RefreshCloudServerRating()
    {
        throw new NotImplementedException("not available at the moment");
    }


    /// <summary>
    /// Resets the traffic stats and re-enables them.
    /// </summary>
    public static void NetworkStatisticsReset()
    {
        networkingPeer.TrafficStatsReset();
    }


    /// <summary>
    /// Only available when NetworkStatisticsEnabled was used to gather some stats.
    /// </summary>
    /// <returns>A string with vital networking statistics.</returns>
    public static string NetworkStatisticsToString()
    {
        if (networkingPeer == null || offlineMode)
        {
            return "Offline or in OfflineMode. No VitalStats available.";
        }

        return networkingPeer.VitalStatsToString(false);
    }

    /// <summary>
    /// Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.
    /// </summary>
    [Obsolete("Used for compatibility with Unity networking only. Encryption is automatically initialized while connecting.")]
    public static void InitializeSecurity()
    {
        return;
    }

    /// <summary>
    /// Helper function which is called inside this class to erify if certain functions can be used (e.g. RPC when not connected)
    /// </summary>
    /// <returns></returns>
    private static bool VerifyCanUseNetwork()
    {
        if (connected)
        {
            return true;
        }

        Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
        return false;
    }

    /// <summary>
    /// Makes this client disconnect from the photon server, a process that leaves any room and calls OnDisconnectedFromPhoton on completion.
    /// </summary>
    /// <remarks>
    /// When you disconnect, the client will send a "disconnecting" message to the server. This speeds up leave/disconnect
    /// messages for players in the same room as you (otherwise the server would timeout this client's connection).
    /// When used in offlineMode, the state-change and event-call OnDisconnectedFromPhoton are immediate.
    /// Offline mode is set to false as well.
    /// Once disconnected, the client can connect again. Use ConnectUsingSettings.
    /// </remarks>
    public static void Disconnect()
    {
        if (offlineMode)
        {
            offlineMode = false;
            offlineModeRoom = null;
            networkingPeer.State = ClientState.Disconnecting;
            networkingPeer.OnStatusChanged(StatusCode.Disconnect);
            return;
        }

        if (networkingPeer == null)
        {
            return; // Surpress error when quitting playmode in the editor
        }

        networkingPeer.Disconnect();
    }

    /// <summary>
    /// Requests the rooms and online status for a list of friends and saves the result in PhotonNetwork.Friends.
    /// </summary>
    /// <remarks>
    /// Works only on Master Server to find the rooms played by a selected list of users.
    ///
    /// The result will be stored in PhotonNetwork.Friends when available.
    /// That list is initialized on first use of OpFindFriends (before that, it is null).
    /// To refresh the list, call FindFriends again (in 5 seconds or 10 or 20).
    ///
    /// Users identify themselves by setting a unique username via PhotonNetwork.playerName
    /// or by PhotonNetwork.AuthValues. The user id set in AuthValues overrides the playerName,
    /// so make sure you know the ID your friends use to authenticate.
    /// The AuthValues are sent in OpAuthenticate when you connect, so the AuthValues must be
    /// set before you connect!
    ///
    /// Note: Changing a player's name doesn't make sense when using a friend list.
    ///
    /// The list of friends must be fetched from some other source (not provided by Photon).
    ///
    ///
    /// Internal:
    /// The server response includes 2 arrays of info (each index matching a friend from the request):
    /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
    /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
    /// </remarks>
    /// <param name="friendsToFind">Array of friend (make sure to use unique playerName or AuthValues).</param>
    /// <returns>If the operation could be sent (requires connection, only one request is allowed at any time). Always false in offline mode.</returns>
    public static bool FindFriends(string[] friendsToFind)
    {
        if (networkingPeer == null || isOfflineMode)
        {
            return false;
        }

        return networkingPeer.OpFindFriends(friendsToFind);
    }


    /// <summary>
    /// Creates a room with given name but fails if this room(name) is existing already. Creates random name for roomName null.
    /// </summary>
    /// <remarks>
    /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a roomName (a GUID as string).
    ///
    /// The created room is automatically placed in the currently used lobby (if any) or the default-lobby if you didn't explicitly join one.
    ///
    /// Call this only on the master server.
    /// Internally, the master will respond with a server-address (and roomName, if needed). Both are used internally
    /// to switch to the assigned game server and roomName.
    ///
    /// PhotonNetwork.autoCleanUpPlayerObjects will become this room's AutoCleanUp property and that's used by all clients that join this room.
    /// </remarks>
    /// <param name="roomName">Unique name of the room to create.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool CreateRoom(string roomName)
    {
        return CreateRoom(roomName, null, null, null);
    }

    /// <summary>
    /// Creates a room but fails if this room is existing already. Can only be called on Master Server.
    /// </summary>
    /// <remarks>
    /// When successful, this calls the callbacks OnCreatedRoom and OnJoinedRoom (the latter, cause you join as first player).
    /// If the room can't be created (because it exists already), OnPhotonCreateRoomFailed gets called.
    ///
    /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a roomName (a GUID as string).
    ///
    /// Rooms can be created in any number of lobbies. Those don't have to exist before you create a room in them (they get
    /// auto-created on demand). Lobbies can be useful to split room lists on the server-side already. That can help keep the room
    /// lists short and manageable.
    /// If you set a typedLobby parameter, the room will be created in that lobby (no matter if you are active in any).
    /// If you don't set a typedLobby, the room is automatically placed in the currently active lobby (if any) or the
    /// default-lobby.
    ///
    /// Call this only on the master server.
    /// Internally, the master will respond with a server-address (and roomName, if needed). Both are used internally
    /// to switch to the assigned game server and roomName.
    ///
    /// PhotonNetwork.autoCleanUpPlayerObjects will become this room's autoCleanUp property and that's used by all clients that join this room.
    /// </remarks>
    /// <param name="roomName">Unique name of the room to create. Pass null or "" to make the server generate a name.</param>
    /// <param name="roomOptions">Common options for the room like MaxPlayers, initial custom room properties and similar. See RoomOptions type..</param>
    /// <param name="typedLobby">If null, the room is automatically created in the currently used lobby (which is "default" when you didn't join one explicitly).</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        return CreateRoom(roomName, roomOptions, typedLobby, null);
    }

    /// <summary>
    /// Creates a room but fails if this room is existing already. Can only be called on Master Server.
    /// </summary>
    /// <remarks>
    /// When successful, this calls the callbacks OnCreatedRoom and OnJoinedRoom (the latter, cause you join as first player).
    /// If the room can't be created (because it exists already), OnPhotonCreateRoomFailed gets called.
    ///
    /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a roomName (a GUID as string).
    ///
    /// Rooms can be created in any number of lobbies. Those don't have to exist before you create a room in them (they get
    /// auto-created on demand). Lobbies can be useful to split room lists on the server-side already. That can help keep the room
    /// lists short and manageable.
    /// If you set a typedLobby parameter, the room will be created in that lobby (no matter if you are active in any).
    /// If you don't set a typedLobby, the room is automatically placed in the currently active lobby (if any) or the
    /// default-lobby.
    ///
    /// Call this only on the master server.
    /// Internally, the master will respond with a server-address (and roomName, if needed). Both are used internally
    /// to switch to the assigned game server and roomName.
    ///
    /// PhotonNetwork.autoCleanUpPlayerObjects will become this room's autoCleanUp property and that's used by all clients that join this room.
    ///
    /// You can define an array of expectedUsers, to block player slots in the room for these users.
    /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
    /// </remarks>
    /// <param name="roomName">Unique name of the room to create. Pass null or "" to make the server generate a name.</param>
    /// <param name="roomOptions">Common options for the room like MaxPlayers, initial custom room properties and similar. See RoomOptions type..</param>
    /// <param name="typedLobby">If null, the room is automatically created in the currently used lobby (which is "default" when you didn't join one explicitly).</param>
    /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool CreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            EnterOfflineRoom(roomName, roomOptions, true);
            return true;
        }
        if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
        {
            Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }

        typedLobby = typedLobby ?? ((networkingPeer.insideLobby) ? networkingPeer.lobby : null);  // use given lobby, or active lobby (if any active) or none

        EnterRoomParams opParams = new EnterRoomParams();
        opParams.RoomName = roomName;
        opParams.RoomOptions = roomOptions;
        opParams.Lobby = typedLobby;
        opParams.ExpectedUsers = expectedUsers;

        return networkingPeer.OpCreateGame(opParams);
    }


    /// <summary>Join room by roomname and on success calls OnJoinedRoom(). This is not affected by lobbies.</summary>
    /// <remarks>
    /// On success, the method OnJoinedRoom() is called on any script. You can implement it to react to joining a room.
    ///
    /// JoinRoom fails if the room is either full or no longer available (it might become empty while you attempt to join).
    /// Implement OnPhotonJoinRoomFailed() to get a callback in error case.
    ///
    /// To join a room from the lobby's listing, use RoomInfo.name as roomName here.
    /// Despite using multiple lobbies, a roomName is always "global" for your application and so you don't
    /// have to specify which lobby it's in. The Master Server will find the room.
    /// In the Photon Cloud, an application is defined by AppId, Game- and PUN-version.
    /// </remarks>
    /// <see cref="PhotonNetworkingMessage.OnPhotonJoinRoomFailed"/>
    /// <see cref="PhotonNetworkingMessage.OnJoinedRoom"/>
    /// <param name="roomName">Unique name of the room to join.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinRoom(string roomName)
    {
        return JoinRoom(roomName, null);
    }

    /// <summary>Join room by roomname and on success calls OnJoinedRoom(). This is not affected by lobbies.</summary>
    /// <remarks>
    /// On success, the method OnJoinedRoom() is called on any script. You can implement it to react to joining a room.
    ///
    /// JoinRoom fails if the room is either full or no longer available (it might become empty while you attempt to join).
    /// Implement OnPhotonJoinRoomFailed() to get a callback in error case.
    ///
    /// To join a room from the lobby's listing, use RoomInfo.name as roomName here.
    /// Despite using multiple lobbies, a roomName is always "global" for your application and so you don't
    /// have to specify which lobby it's in. The Master Server will find the room.
    /// In the Photon Cloud, an application is defined by AppId, Game- and PUN-version.
    ///
    /// You can define an array of expectedUsers, to block player slots in the room for these users.
    /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
    /// </remarks>
    /// <see cref="PhotonNetworkingMessage.OnPhotonJoinRoomFailed"/>
    /// <see cref="PhotonNetworkingMessage.OnJoinedRoom"/>
    /// <param name="roomName">Unique name of the room to join.</param>
    /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinRoom(string roomName, string[] expectedUsers)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            EnterOfflineRoom(roomName, null, true);
            return true;
        }
        if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
        {
            Debug.LogError("JoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("JoinRoom failed. A roomname is required. If you don't know one, how will you join?");
            return false;
        }


        EnterRoomParams opParams = new EnterRoomParams();
        opParams.RoomName = roomName;
        opParams.ExpectedUsers = expectedUsers;

        return networkingPeer.OpJoinRoom(opParams);
    }


    /// <summary>Lets you either join a named room or create it on the fly - you don't have to know if someone created the room already.</summary>
    /// <remarks>
    /// This makes it easier for groups of players to get into the same room. Once the group
    /// exchanged a roomName, any player can call JoinOrCreateRoom and it doesn't matter who
    /// actually joins or creates the room.
    ///
    /// The parameters roomOptions and typedLobby are only used when the room actually gets created by this client.
    /// You know if this client created a room, if you get a callback OnCreatedRoom (before OnJoinedRoom gets called as well).
    /// </remarks>
    /// <param name="roomName">Name of the room to join. Must be non null.</param>
    /// <param name="roomOptions">Options for the room, in case it does not exist yet. Else these values are ignored.</param>
    /// <param name="typedLobby">Lobby you want a new room to be listed in. Ignored if the room was existing and got joined.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby)
    {
        return JoinOrCreateRoom(roomName, roomOptions, typedLobby, null);
    }

    /// <summary>Lets you either join a named room or create it on the fly - you don't have to know if someone created the room already.</summary>
    /// <remarks>
    /// This makes it easier for groups of players to get into the same room. Once the group
    /// exchanged a roomName, any player can call JoinOrCreateRoom and it doesn't matter who
    /// actually joins or creates the room.
    ///
    /// The parameters roomOptions and typedLobby are only used when the room actually gets created by this client.
    /// You know if this client created a room, if you get a callback OnCreatedRoom (before OnJoinedRoom gets called as well).
    ///
    /// You can define an array of expectedUsers, to block player slots in the room for these users.
    /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
    /// </remarks>
    /// <param name="roomName">Name of the room to join. Must be non null.</param>
    /// <param name="roomOptions">Options for the room, in case it does not exist yet. Else these values are ignored.</param>
    /// <param name="typedLobby">Lobby you want a new room to be listed in. Ignored if the room was existing and got joined.</param>
    /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            EnterOfflineRoom(roomName, roomOptions, true);  // in offline mode, JoinOrCreateRoom assumes you create the room
            return true;
        }
        if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
        {
            Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
            return false;
        }

        typedLobby = typedLobby ?? ((networkingPeer.insideLobby) ? networkingPeer.lobby : null);  // use given lobby, or active lobby (if any active) or none

        EnterRoomParams opParams = new EnterRoomParams();
        opParams.RoomName = roomName;
        opParams.RoomOptions = roomOptions;
        opParams.Lobby = typedLobby;
        opParams.CreateIfNotExists = true;
        opParams.PlayerProperties = player.customProperties;
        opParams.ExpectedUsers = expectedUsers;

        return networkingPeer.OpJoinRoom(opParams);
    }

    /// <summary>
    /// Joins any available room of the currently used lobby and fails if none is available.
    /// </summary>
    /// <remarks>
    /// Rooms can be created in arbitrary lobbies which get created on demand.
    /// You can join rooms from any lobby without actually joining the lobby.
    /// Use the JoinRandomRoom overload with TypedLobby parameter.
    ///
    /// This method will only match rooms attached to one lobby! If you use many lobbies, you
    /// might have to repeat JoinRandomRoom, to find some fitting room.
    /// This method looks up a room in the currently active lobby or (if no lobby is joined)
    /// in the default lobby.
    ///
    /// If this fails, you can still create a room (and make this available for the next who uses JoinRandomRoom).
    /// Alternatively, try again in a moment.
    /// </remarks>
    public static bool JoinRandomRoom()
    {
        return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
    }

    /// <summary>
    /// Attempts to join an open room with fitting, custom properties but fails if none is currently available.
    /// </summary>
    /// <remarks>
    /// Rooms can be created in arbitrary lobbies which get created on demand.
    /// You can join rooms from any lobby without actually joining the lobby.
    /// Use the JoinRandomRoom overload with TypedLobby parameter.
    ///
    /// This method will only match rooms attached to one lobby! If you use many lobbies, you
    /// might have to repeat JoinRandomRoom, to find some fitting room.
    /// This method looks up a room in the currently active lobby or (if no lobby is joined)
    /// in the default lobby.
    ///
    /// If this fails, you can still create a room (and make this available for the next who uses JoinRandomRoom).
    /// Alternatively, try again in a moment.
    /// </remarks>
    /// <param name="expectedCustomRoomProperties">Filters for rooms that match these custom properties (string keys and values). To ignore, pass null.</param>
    /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
    {
        return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
    }

    /// <summary>
    /// Attempts to join an open room with fitting, custom properties but fails if none is currently available.
    /// </summary>
    /// <remarks>
    /// Rooms can be created in arbitrary lobbies which get created on demand.
    /// You can join rooms from any lobby without actually joining the lobby with this overload.
    ///
    /// This method will only match rooms attached to one lobby! If you use many lobbies, you
    /// might have to repeat JoinRandomRoom, to find some fitting room.
    /// This method looks up a room in the specified lobby or the currently active lobby (if none specified)
    /// or in the default lobby (if none active).
    ///
    /// If this fails, you can still create a room (and make this available for the next who uses JoinRandomRoom).
    /// Alternatively, try again in a moment.
    ///
    /// In offlineMode, a room will be created but no properties will be set and all parameters of this
    /// JoinRandomRoom call are ignored. The event/callback OnJoinedRoom gets called (see enum PhotonNetworkingMessage).
    ///
    /// You can define an array of expectedUsers, to block player slots in the room for these users.
    /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
    /// </remarks>
    /// <param name="expectedCustomRoomProperties">Filters for rooms that match these custom properties (string keys and values). To ignore, pass null.</param>
    /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
    /// <param name="matchingType">Selects one of the available matchmaking algorithms. See MatchmakingMode enum for options.</param>
    /// <param name="typedLobby">The lobby in which you want to lookup a room. Pass null, to use the default lobby. This does not join that lobby and neither sets the lobby property.</param>
    /// <param name="sqlLobbyFilter">A filter-string for SQL-typed lobbies.</param>
    /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
    /// <returns>If the operation got queued and will be sent.</returns>
    public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers, MatchmakingMode matchingType, TypedLobby typedLobby, string sqlLobbyFilter, string[] expectedUsers = null)
    {
        if (offlineMode)
        {
            if (offlineModeRoom != null)
            {
                Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
                return false;
            }
            EnterOfflineRoom("offline room", null, true);
            return true;
        }
        if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
        {
            Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }

        typedLobby = typedLobby ?? ((networkingPeer.insideLobby) ? networkingPeer.lobby : null);  // use given lobby, or active lobby (if any active) or none

        OpJoinRandomRoomParams opParams = new OpJoinRandomRoomParams();
        opParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
        opParams.ExpectedMaxPlayers = expectedMaxPlayers;
        opParams.MatchingType = matchingType;
        opParams.TypedLobby = typedLobby;
        opParams.SqlLobbyFilter = sqlLobbyFilter;
        opParams.ExpectedUsers = expectedUsers;

        return networkingPeer.OpJoinRandomRoom(opParams);
    }


	/// <summary>Can be used to return to a room after a disconnect and reconnect.</summary>
	/// <remarks>
	/// After losing connection, you might be able to return to a room and continue playing,
	/// if the client is reconnecting fast enough. Use Reconnect() and this method.
	/// Cache the room name you're in and use ReJoin(roomname) to return to a game.
	///
	/// Note: To be able to ReJoin any room, you need to use UserIDs!
	/// You also need to set RoomOptions.PlayerTtl.
	///
	/// <b>Important: Instantiate() and use of RPCs is not yet supported.</b>
	/// The ownership rules of PhotonViews prevent a seamless return to a game.
	/// Use Custom Properties and RaiseEvent with event caching instead.
	///
	/// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
	/// </remarks>
    public static bool ReJoinRoom(string roomName)
    {
        if (offlineMode)
        {
            Debug.LogError("ReJoinRoom failed due to offline mode.");
            return false;
        }
        if (networkingPeer.Server != ServerConnection.MasterServer || !connectedAndReady)
        {
            Debug.LogError("ReJoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
            return false;
        }
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("ReJoinRoom failed. A roomname is required. If you don't know one, how will you join?");
            return false;
        }

        EnterRoomParams opParams = new EnterRoomParams();
        opParams.RoomName = roomName;
        opParams.RejoinOnly = true;
        opParams.PlayerProperties = player.customProperties;

        return networkingPeer.OpJoinRoom(opParams);
    }


    /// <summary>
    /// Internally used helper-method to setup an offline room, the numbers for actor and master-client and to do the callbacks.
    /// </summary>
    private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
    {
        offlineModeRoom = new Room(roomName, roomOptions);
        networkingPeer.ChangeLocalID(1);
        offlineModeRoom.masterClientId = 1;

        if (createdRoom)
        {
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnCreatedRoom);
        }
        NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnJoinedRoom);
    }

    /// <summary>On MasterServer this joins the default lobby which list rooms currently in use.</summary>
    /// <remarks>
    /// The room list is sent and refreshed by the server. You can access this cached list by
    /// PhotonNetwork.GetRoomList().
    ///
    /// Per room you should check if it's full or not before joining. Photon also lists rooms that are
    /// full, unless you close and hide them (room.open = false and room.visible = false).
    ///
    /// In best case, you make your clients join random games, as described here:
    /// http://doc.exitgames.com/en/realtime/current/reference/matchmaking-and-lobby
    ///
    ///
    /// You can show your current players and room count without joining a lobby (but you must
    /// be on the master server). Use: countOfPlayers, countOfPlayersOnMaster, countOfPlayersInRooms and
    /// countOfRooms.
    ///
    /// You can use more than one lobby to keep the room lists shorter. See JoinLobby(TypedLobby lobby).
    /// When creating new rooms, they will be "attached" to the currently used lobby or the default lobby.
    ///
    /// You can use JoinRandomRoom without being in a lobby!
    /// Set autoJoinLobby = false before you connect, to not join a lobby. In that case, the
    /// connect-workflow will call OnConnectedToMaster (if you implement it) when it's done.
    /// </remarks>
    public static bool JoinLobby()
    {
        return JoinLobby(null);
    }

    /// <summary>On a Master Server you can join a lobby to get lists of available rooms.</summary>
    /// <remarks>
    /// The room list is sent and refreshed by the server. You can access this cached list by
    /// PhotonNetwork.GetRoomList().
    ///
    /// Any client can "make up" any lobby on the fly. Splitting rooms into multiple lobbies will
    /// keep each list shorter. However, having too many lists might ruin the matchmaking experience.
    ///
    /// In best case, you create a limited number of lobbies. For example, create a lobby per
    /// game-mode: "koth" for king of the hill and "ffa" for free for all, etc.
    ///
    /// There is no listing of lobbies at the moment.
    ///
    /// Sql-typed lobbies offer a different filtering model for random matchmaking. This might be more
    /// suited for skillbased-games. However, you will also need to follow the conventions for naming
    /// filterable properties in sql-lobbies! Both is explained in the matchmaking doc linked below.
    ///
    /// In best case, you make your clients join random games, as described here:
    /// http://confluence.exitgames.com/display/PTN/Op+JoinRandomGame
    ///
    ///
    /// Per room you should check if it's full or not before joining. Photon does list rooms that are
    /// full, unless you close and hide them (room.open = false and room.visible = false).
    ///
    /// You can show your games current players and room count without joining a lobby (but you must
    /// be on the master server). Use: countOfPlayers, countOfPlayersOnMaster, countOfPlayersInRooms and
    /// countOfRooms.
    ///
    /// When creating new rooms, they will be "attached" to the currently used lobby or the default lobby.
    ///
    /// You can use JoinRandomRoom without being in a lobby!
    /// Set autoJoinLobby = false before you connect, to not join a lobby. In that case, the
    /// connect-workflow will call OnConnectedToMaster (if you implement it) when it's done.
    /// </remarks>
    /// <param name="typedLobby">A typed lobby to join (must have name and type).</param>
    public static bool JoinLobby(TypedLobby typedLobby)
    {
        if (PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer)
        {
            if (typedLobby == null)
            {
                typedLobby = TypedLobby.Default;
            }

            bool sending = networkingPeer.OpJoinLobby(typedLobby);
            if (sending)
            {
                networkingPeer.lobby = typedLobby;

            }
            return sending;
        }

        return false;
    }

    /// <summary>Leave a lobby to stop getting updates about available rooms.</summary>
    /// <remarks>
    /// This does not reset PhotonNetwork.lobby! This allows you to join this particular lobby later
    /// easily.
    ///
    /// The values countOfPlayers, countOfPlayersOnMaster, countOfPlayersInRooms and countOfRooms
    /// are received even without being in a lobby.
    ///
    /// You can use JoinRandomRoom without being in a lobby.
    /// Use autoJoinLobby to not join a lobby when you connect.
    /// </remarks>
    public static bool LeaveLobby()
    {
        if (PhotonNetwork.connected && PhotonNetwork.Server == ServerConnection.MasterServer)
        {
            return networkingPeer.OpLeaveLobby();
        }

        return false;
    }

    /// <summary>Leave the current room and return to the Master Server where you can join or create rooms (see remarks).</summary>
    /// <remarks>
    /// This will clean up all (network) GameObjects with a PhotonView, unless you changed autoCleanUp to false.
    /// Returns to the Master Server.
    ///
    /// In OfflineMode, the local "fake" room gets cleaned up and OnLeftRoom gets called immediately.
    /// </remarks>
    public static bool LeaveRoom()
    {
        if (offlineMode)
        {
            offlineModeRoom = null;
            NetworkingPeer.SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
        }
        else
        {
            if (room == null)
            {
                Debug.LogWarning("PhotonNetwork.room is null. You don't have to call LeaveRoom() when you're not in one. State: " + PhotonNetwork.connectionStateDetailed);
            }
            return networkingPeer.OpLeave();
        }

        return true;
    }

    /// <summary>
    /// Gets currently known rooms as RoomInfo array. This is available and updated while in a lobby (check insideLobby).
    /// </summary>
    /// <remarks>
    /// This list is a cached copy of the internal rooms list so it can be accessed each frame if needed.
    /// Per RoomInfo you can check if the room is full by comparing playerCount and MaxPlayers before you allow a join.
    ///
    /// The name of a room must be used to join it (via JoinRoom).
    ///
    /// Closed rooms are also listed by lobbies but they can't be joined. While in a room, any player can set
    /// Room.visible and Room.open to hide rooms from matchmaking and close them.
    /// </remarks>
    /// <returns>RoomInfo[] of current rooms in lobby.</returns>
    public static RoomInfo[] GetRoomList()
    {
        if (offlineMode || networkingPeer == null)
        {
            return new RoomInfo[0];
        }

        return networkingPeer.mGameListCopy;
    }

    /// <summary>
    /// Sets this (local) player's properties and synchronizes them to the other players (don't modify them directly).
    /// </summary>
    /// <remarks>
    /// While in a room, your properties are synced with the other players.
    /// CreateRoom, JoinRoom and JoinRandomRoom will all apply your player's custom properties when you enter the room.
    /// The whole Hashtable will get sent. Minimize the traffic by setting only updated key/values.
    ///
    /// If the Hashtable is null, the custom properties will be cleared.
    /// Custom properties are never cleared automatically, so they carry over to the next room, if you don't change them.
    ///
    /// Don't set properties by modifying PhotonNetwork.player.customProperties!
    /// </remarks>
    /// <param name="customProperties">Only string-typed keys will be used from this hashtable. If null, custom properties are all deleted.</param>
    public static void SetPlayerCustomProperties(Hashtable customProperties)
    {
        if (customProperties == null)
        {
            customProperties = new Hashtable();
            foreach (object k in player.customProperties.Keys)
            {
                customProperties[(string)k] = null;
            }
        }

        if (room != null && room.isLocalClientInside)
        {
            player.SetCustomProperties(customProperties);
        }
        else
        {
            player.InternalCacheProperties(customProperties);
        }
    }

    /// <summary>
    /// Locally removes Custom Properties of "this" player. Important: This does not synchronize the change! Useful when you switch rooms.
    /// </summary>
    /// <remarks>
    /// Use this method with care. It can create inconsistencies of state between players!
    /// This only changes the player.customProperties locally. This can be useful to clear your
    /// Custom Properties between games (let's say they store which turn you made, kills, etc).
    ///
    /// SetPlayerCustomProperties() syncs and can be used to set values to null while in a room.
    /// That can be considered "removed" while in a room.
    ///
    /// If customPropertiesToDelete is null or has 0 entries, all Custom Properties are deleted (replaced with a new Hashtable).
    /// If you specify keys to remove, those will be removed from the Hashtable but other keys are unaffected.
    /// </remarks>
    /// <param name="customPropertiesToDelete">List of Custom Property keys to remove. See remarks.</param>
    public static void RemovePlayerCustomProperties(string[] customPropertiesToDelete)
    {
        if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || player.customProperties == null)
        {
            player.customProperties = new Hashtable();
            return;
        }

        // if a specific list of props should be deleted, we do that here
        for (int i = 0; i < customPropertiesToDelete.Length; i++)
        {
            string key = customPropertiesToDelete[i];
            if (player.customProperties.ContainsKey(key))
            {
                player.customProperties.Remove(key);
            }
        }
    }

    /// <summary>
    /// Sends fully customizable events in a room. Events consist of at least an EventCode (0..199) and can have content.
    /// </summary>
    /// <remarks>
    /// To receive the events someone sends, register your handling method in PhotonNetwork.OnEventCall.
    ///
    /// Example:
    /// private void OnEventHandler(byte eventCode, object content, int senderId)
    /// { Debug.Log("OnEventHandler"); }
    ///
    /// PhotonNetwork.OnEventCall += this.OnEventHandler;
    ///
    /// With the senderId, you can look up the PhotonPlayer who sent the event.
    /// It is best practice to assign a eventCode for each different type of content and action. You have to cast the content.
    ///
    /// The eventContent is optional. To be able to send something, it must be a "serializable type", something that
    /// the client can turn into a byte[] basically. Most basic types and arrays of them are supported, including
    /// Unity's Vector2, Vector3, Quaternion. Transforms or classes some project defines are NOT supported!
    /// You can make your own class a "serializable type" by following the example in CustomTypes.cs.
    ///
    ///
    /// The RaiseEventOptions have some (less intuitive) combination rules:
    /// If you set targetActors (an array of PhotonPlayer.ID values), the receivers parameter gets ignored.
    /// When using event caching, the targetActors, receivers and interestGroup can't be used. Buffered events go to all.
    /// When using cachingOption removeFromRoomCache, the eventCode and content are actually not sent but used as filter.
    /// </remarks>
    /// <param name="eventCode">A byte identifying the type of event. You might want to use a code per action or to signal which content can be expected. Allowed: 0..199.</param>
    /// <param name="eventContent">Some serializable object like string, byte, integer, float (etc) and arrays of those. Hashtables with byte keys are good to send variable content.</param>
    /// <param name="sendReliable">Makes sure this event reaches all players. It gets acknowledged, which requires bandwidth and it can't be skipped (might add lag in case of loss).</param>
    /// <param name="options">Allows more complex usage of events. If null, RaiseEventOptions.Default will be used (which is fine).</param>
    /// <returns>False if event could not be sent</returns>
    public static bool RaiseEvent(byte eventCode, object eventContent, bool sendReliable, RaiseEventOptions options)
    {
        if (!inRoom || eventCode >= 200)
        {
            Debug.LogWarning("RaiseEvent() failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
            return false;
        }

        return networkingPeer.OpRaiseEvent(eventCode, eventContent, sendReliable, options);
    }

    /// <summary>
    /// Allocates a viewID that's valid for the current/local player.
    /// </summary>
    /// <returns>A viewID that can be used for a new PhotonView.</returns>
    public static int AllocateViewID()
    {
        int manualId = AllocateViewID(player.ID);
        manuallyAllocatedViewIds.Add(manualId);
        return manualId;
    }


    /// <summary>
    /// Enables the Master Client to allocate a viewID that is valid for scene objects.
    /// </summary>
    /// <returns>A viewID that can be used for a new PhotonView or -1 in case of an error.</returns>
    public static int AllocateSceneViewID()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("Only the Master Client can AllocateSceneViewID(). Check PhotonNetwork.isMasterClient!");
            return -1;
        }

        int manualId = AllocateViewID(0);
        manuallyAllocatedViewIds.Add(manualId);
        return manualId;
    }

    // use 0 for scene-targetPhotonView-ids
    // returns viewID (combined owner and sub id)
    private static int AllocateViewID(int ownerId)
    {
        if (ownerId == 0)
        {
            // we look up a fresh subId for the owner "room" (mind the "sub" in subId)
            int newSubId = lastUsedViewSubIdStatic;
            int newViewId;
            int ownerIdOffset = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                newSubId = (newSubId + 1) % MAX_VIEW_IDS;
                if (newSubId == 0)
                {
                    continue;   // avoid using subID 0
                }

                newViewId = newSubId + ownerIdOffset;
                if (!networkingPeer.photonViewList.ContainsKey(newViewId))
                {
                    lastUsedViewSubIdStatic = newSubId;
                    return newViewId;
                }
            }

            // this is the error case: we didn't find any (!) free subId for this user
            throw new Exception(string.Format("AllocateViewID() failed. Room (user {0}) is out of 'scene' viewIDs. It seems all available are in use.", ownerId));
        }
        else
        {
            // we look up a fresh SUBid for the owner
            int newSubId = lastUsedViewSubId;
            int newViewId;
            int ownerIdOffset = ownerId * MAX_VIEW_IDS;
            for (int i = 1; i < MAX_VIEW_IDS; i++)
            {
                newSubId = (newSubId + 1) % MAX_VIEW_IDS;
                if (newSubId == 0)
                {
                    continue;   // avoid using subID 0
                }

                newViewId = newSubId + ownerIdOffset;
                if (!networkingPeer.photonViewList.ContainsKey(newViewId) && !manuallyAllocatedViewIds.Contains(newViewId))
                {
                    lastUsedViewSubId = newSubId;
                    return newViewId;
                }
            }

            throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of subIds, as all viewIDs are used.", ownerId));
        }
    }

    private static int[] AllocateSceneViewIDs(int countOfNewViews)
    {
        int[] viewIDs = new int[countOfNewViews];
        for (int view = 0; view < countOfNewViews; view++)
        {
            viewIDs[view] = AllocateViewID(0);
        }

        return viewIDs;
    }

    /// <summary>
    /// Unregister a viewID (of manually instantiated and destroyed networked objects).
    /// </summary>
    /// <param name="viewID">A viewID manually allocated by this player.</param>
    public static void UnAllocateViewID(int viewID)
    {
        manuallyAllocatedViewIds.Remove(viewID);

        if (networkingPeer.photonViewList.ContainsKey(viewID))
        {
            Debug.LogWarning(string.Format("UnAllocateViewID() should be called after the PhotonView was destroyed (GameObject.Destroy()). ViewID: {0} still found in: {1}", viewID, networkingPeer.photonViewList[viewID]));
        }
    }

    /// <summary>
    /// Instantiate a prefab over the network. This prefab needs to be located in the root of a "Resources" folder.
    /// </summary>
    /// <remarks>
    /// Instead of using prefabs in the Resources folder, you can manually Instantiate and assign PhotonViews. See doc.
    /// </remarks>
    /// <param name="prefabName">Name of the prefab to instantiate.</param>
    /// <param name="position">Position Vector3 to apply on instantiation.</param>
    /// <param name="rotation">Rotation Quaternion to apply on instantiation.</param>
    /// <param name="group">The group for this PhotonView.</param>
    /// <returns>The new instance of a GameObject with initialized PhotonView.</returns>
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group)
    {
        return Instantiate(prefabName, position, rotation, group, null);
    }

    /// <summary>
    /// Instantiate a prefab over the network. This prefab needs to be located in the root of a "Resources" folder.
    /// </summary>
    /// <remarks>Instead of using prefabs in the Resources folder, you can manually Instantiate and assign PhotonViews. See doc.</remarks>
    /// <param name="prefabName">Name of the prefab to instantiate.</param>
    /// <param name="position">Position Vector3 to apply on instantiation.</param>
    /// <param name="rotation">Rotation Quaternion to apply on instantiation.</param>
    /// <param name="group">The group for this PhotonView.</param>
    /// <param name="data">Optional instantiation data. This will be saved to it's PhotonView.instantiationData.</param>
    /// <returns>The new instance of a GameObject with initialized PhotonView.</returns>
    public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!connected || (InstantiateInRoomOnly && !inRoom))
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
            return null;
        }

        GameObject prefabGo;
        if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out prefabGo))
        {
            prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (UsePrefabCache)
            {
                PrefabCache.Add(prefabName, prefabGo);
            }
        }

        if (prefabGo == null)
        {
            Debug.LogError("Failed to Instantiate prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        // a scene object instantiated with network visibility has to contain a PhotonView
        if (prefabGo.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to Instantiate prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }

        Component[] views = (Component[])prefabGo.GetPhotonViewsInChildren();
        int[] viewIDs = new int[views.Length];
        for (int i = 0; i < viewIDs.Length; i++)
        {
            //Debug.Log("Instantiate prefabName: " + prefabName + " player.ID: " + player.ID);
            viewIDs[i] = AllocateViewID(player.ID);
        }

        // Send to others, create info
        Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, false);

        // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
        return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);
    }


    /// <summary>
    /// Instantiate a scene-owned prefab over the network. The PhotonViews will be controllable by the MasterClient. This prefab needs to be located in the root of a "Resources" folder.
    /// </summary>
    /// <remarks>
    /// Only the master client can Instantiate scene objects.
    /// Instead of using prefabs in the Resources folder, you can manually Instantiate and assign PhotonViews. See doc.
    /// </remarks>
    /// <param name="prefabName">Name of the prefab to instantiate.</param>
    /// <param name="position">Position Vector3 to apply on instantiation.</param>
    /// <param name="rotation">Rotation Quaternion to apply on instantiation.</param>
    /// <param name="group">The group for this PhotonView.</param>
    /// <param name="data">Optional instantiation data. This will be saved to it's PhotonView.instantiationData.</param>
    /// <returns>The new instance of a GameObject with initialized PhotonView.</returns>
    public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, int group, object[] data)
    {
        if (!connected || (InstantiateInRoomOnly && !inRoom))
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client should be in a room. Current connectionStateDetailed: " + PhotonNetwork.connectionStateDetailed);
            return null;
        }

        if (!isMasterClient)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Client is not the MasterClient in this room.");
            return null;
        }

        GameObject prefabGo;
        if (!UsePrefabCache || !PrefabCache.TryGetValue(prefabName, out prefabGo))
        {
            prefabGo = (GameObject)Resources.Load(prefabName, typeof(GameObject));
            if (UsePrefabCache)
            {
                PrefabCache.Add(prefabName, prefabGo);
            }
        }

        if (prefabGo == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". Verify the Prefab is in a Resources folder (and not in a subfolder)");
            return null;
        }

        // a scene object instantiated with network visibility has to contain a PhotonView
        if (prefabGo.GetComponent<PhotonView>() == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab:" + prefabName + ". Prefab must have a PhotonView component.");
            return null;
        }

        Component[] views = (Component[])prefabGo.GetPhotonViewsInChildren();
        int[] viewIDs = AllocateSceneViewIDs(views.Length);

        if (viewIDs == null)
        {
            Debug.LogError("Failed to InstantiateSceneObject prefab: " + prefabName + ". No ViewIDs are free to use. Max is: " + MAX_VIEW_IDS);
            return null;
        }

        // Send to others, create info
        Hashtable instantiateEvent = networkingPeer.SendInstantiate(prefabName, position, rotation, group, viewIDs, data, true);

        // Instantiate the GO locally (but the same way as if it was done via event). This will also cache the instantiationId
        return networkingPeer.DoInstantiate(instantiateEvent, networkingPeer.LocalPlayer, prefabGo);
    }

    /// <summary>
    /// The current roundtrip time to the photon server.
    /// </summary>
    /// <returns>Roundtrip time (to server and back).</returns>
    public static int GetPing()
    {
        return networkingPeer.RoundTripTime;
    }

    /// <summary>Refreshes the server timestamp (async operation, takes a roundtrip).</summary>
    /// <remarks>Can be useful if a bad connection made the timestamp unusable or imprecise.</remarks>
    public static void FetchServerTimestamp()
    {
        if (networkingPeer != null)
        {
            networkingPeer.FetchServerTimestamp();
        }
    }

    /// <summary>
    /// Can be used to immediately send the RPCs and Instantiates just called, so they are on their way to the other players.
    /// </summary>
    /// <remarks>
    /// This could be useful if you do a RPC to load a level and then load it yourself.
    /// While loading, no RPCs are sent to others, so this would delay the "load" RPC.
    /// You can send the RPC to "others", use this method, disable the message queue
    /// (by isMessageQueueRunning) and then load.
    /// </remarks>
    public static void SendOutgoingCommands()
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        while (networkingPeer.SendOutgoingCommands())
        {
        }
    }

    /// <summary>Request a client to disconnect (KICK). Only the master client can do this</summary>
    /// <remarks>Only the target player gets this event. That player will disconnect automatically, which is what the others will notice, too.</remarks>
    /// <param name="kickPlayer">The PhotonPlayer to kick.</param>
    public static bool CloseConnection(PhotonPlayer kickPlayer)
    {
        if (!VerifyCanUseNetwork())
        {
            return false;
        }

        if (!player.isMasterClient)
        {
            Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
            return false;
        }

        if (kickPlayer == null)
        {
            Debug.LogError("CloseConnection: No such player connected!");
            return false;
        }

        RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { kickPlayer.ID } };
        return networkingPeer.OpRaiseEvent(PunEvent.CloseConnection, null, true, options);
    }

    /// <summary>
    /// Asks the server to assign another player as Master Client of your current room.
    /// </summary>
    /// <remarks>
    /// RPCs and RaiseEvent have the option to send messages only to the Master Client of a room.
    /// SetMasterClient affects which client gets those messages.
    ///
    /// This method calls an operation on the server to set a new Master Client, which takes a roundtrip.
    /// In case of success, this client and the others get the new Master Client from the server.
    ///
    /// SetMasterClient tells the server which current Master Client should be replaced with the new one.
    /// It will fail, if anything switches the Master Client moments earlier. There is no callback for this
    /// error. All clients should get the new Master Client assigned by the server anyways.
    ///
    /// See also: PhotonNetwork.masterClient
    ///
    /// On v3 servers:
    /// The ReceiverGroup.MasterClient (usable in RPCs) is not affected by this (still points to lowest player.ID in room).
    /// Avoid using this enum value (and send to a specific player instead).
    ///
    /// If the current Master Client leaves, PUN will detect a new one by "lowest player ID". Implement OnMasterClientSwitched
    /// to get a callback in this case. The PUN-selected Master Client might assign a new one.
    ///
    /// Make sure you don't create an endless loop of Master-assigning! When selecting a custom Master Client, all clients
    /// should point to the same player, no matter who actually assigns this player.
    ///
    /// Locally the Master Client is immediately switched, while remote clients get an event. This means the game
    /// is tempoarily without Master Client like when a current Master Client leaves.
    ///
    /// When switching the Master Client manually, keep in mind that this user might leave and not do it's work, just like
    /// any Master Client.
    ///
    /// </remarks>
    /// <param name="masterClientPlayer">The player to become the next Master Client.</param>
    /// <returns>False when this operation couldn't be done. Must be in a room (not in offlineMode).</returns>
    public static bool SetMasterClient(PhotonPlayer masterClientPlayer)
    {
        if (!inRoom || !VerifyCanUseNetwork() || offlineMode)
        {
            if (logLevel == PhotonLogLevel.Informational) Debug.Log("Can not SetMasterClient(). Not in room or in offlineMode.");
            return false;
        }

        if (room.serverSideMasterClient)
        {
            Hashtable newProps = new Hashtable() { { GamePropertyKey.MasterClientId, masterClientPlayer.ID } };
            Hashtable prevProps = new Hashtable() { { GamePropertyKey.MasterClientId, networkingPeer.mMasterClientId } };
            return networkingPeer.OpSetPropertiesOfRoom(newProps, expectedProperties: prevProps, webForward: false);
        }
        else
        {
            if (!isMasterClient)
            {
                return false;
            }
            return networkingPeer.SetMasterClient(masterClientPlayer.ID, true);
        }
    }

    /// <summary>
    /// Network-Destroy the GameObject associated with the PhotonView, unless the PhotonView is static or not under this client's control.
    /// </summary>
    /// <remarks>
    /// Destroying a networked GameObject while in a Room includes:
    /// - Removal of the Instantiate call from the server's room buffer.
    /// - Removing RPCs buffered for PhotonViews that got created indirectly with the PhotonNetwork.Instantiate call.
    /// - Sending a message to other clients to remove the GameObject also (affected by network lag).
    ///
    /// Usually, when you leave a room, the GOs get destroyed automatically.
    /// If you have to destroy a GO while not in a room, the Destroy is only done locally.
    ///
    /// Destroying networked objects works only if they got created with PhotonNetwork.Instantiate().
    /// Objects loaded with a scene are ignored, no matter if they have PhotonView components.
    ///
    /// The GameObject must be under this client's control:
    /// - Instantiated and owned by this client.
    /// - Instantiated objects of players who left the room are controlled by the Master Client.
    /// - Scene-owned game objects are controlled by the Master Client.
    /// - GameObject can be destroyed while client is not in a room.
    /// </remarks>
    /// <returns>Nothing. Check error debug log for any issues.</returns>
    public static void Destroy(PhotonView targetView)
    {
        if (targetView != null)
        {
            networkingPeer.RemoveInstantiatedGO(targetView.gameObject, !inRoom);
        }
        else
        {
            Debug.LogError("Destroy(targetPhotonView) failed, cause targetPhotonView is null.");
        }
    }

    /// <summary>
    /// Network-Destroy the GameObject, unless it is static or not under this client's control.
    /// </summary>
    /// <remarks>
    /// Destroying a networked GameObject includes:
    /// - Removal of the Instantiate call from the server's room buffer.
    /// - Removing RPCs buffered for PhotonViews that got created indirectly with the PhotonNetwork.Instantiate call.
    /// - Sending a message to other clients to remove the GameObject also (affected by network lag).
    ///
    /// Usually, when you leave a room, the GOs get destroyed automatically.
    /// If you have to destroy a GO while not in a room, the Destroy is only done locally.
    ///
    /// Destroying networked objects works only if they got created with PhotonNetwork.Instantiate().
    /// Objects loaded with a scene are ignored, no matter if they have PhotonView components.
    ///
    /// The GameObject must be under this client's control:
    /// - Instantiated and owned by this client.
    /// - Instantiated objects of players who left the room are controlled by the Master Client.
    /// - Scene-owned game objects are controlled by the Master Client.
    /// - GameObject can be destroyed while client is not in a room.
    /// </remarks>
    /// <returns>Nothing. Check error debug log for any issues.</returns>
    public static void Destroy(GameObject targetGo)
    {
        networkingPeer.RemoveInstantiatedGO(targetGo, !inRoom);
    }

    /// <summary>
    /// Network-Destroy all GameObjects, PhotonViews and their RPCs of targetPlayer. Can only be called on local player (for "self") or Master Client (for anyone).
    /// </summary>
    /// <remarks>
    /// Destroying a networked GameObject includes:
    /// - Removal of the Instantiate call from the server's room buffer.
    /// - Removing RPCs buffered for PhotonViews that got created indirectly with the PhotonNetwork.Instantiate call.
    /// - Sending a message to other clients to remove the GameObject also (affected by network lag).
    ///
    /// Destroying networked objects works only if they got created with PhotonNetwork.Instantiate().
    /// Objects loaded with a scene are ignored, no matter if they have PhotonView components.
    /// </remarks>
    /// <returns>Nothing. Check error debug log for any issues.</returns>
    public static void DestroyPlayerObjects(PhotonPlayer targetPlayer)
    {
        if (player == null)
        {
            Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
        }

        DestroyPlayerObjects(targetPlayer.ID);
    }

    /// <summary>
    /// Network-Destroy all GameObjects, PhotonViews and their RPCs of this player (by ID). Can only be called on local player (for "self") or Master Client (for anyone).
    /// </summary>
    /// <remarks>
    /// Destroying a networked GameObject includes:
    /// - Removal of the Instantiate call from the server's room buffer.
    /// - Removing RPCs buffered for PhotonViews that got created indirectly with the PhotonNetwork.Instantiate call.
    /// - Sending a message to other clients to remove the GameObject also (affected by network lag).
    ///
    /// Destroying networked objects works only if they got created with PhotonNetwork.Instantiate().
    /// Objects loaded with a scene are ignored, no matter if they have PhotonView components.
    /// </remarks>
    /// <returns>Nothing. Check error debug log for any issues.</returns>
    public static void DestroyPlayerObjects(int targetPlayerId)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        if (player.isMasterClient || targetPlayerId == player.ID)
        {
            networkingPeer.DestroyPlayerObjects(targetPlayerId, false);
        }
        else
        {
            Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.isMasterClient);
        }
    }

    /// <summary>
    /// Network-Destroy all GameObjects, PhotonViews and their RPCs in the room. Removes anything buffered from the server. Can only be called by Master Client (for anyone).
    /// </summary>
    /// <remarks>
    /// Can only be called by Master Client (for anyone).
    /// Unlike the Destroy methods, this will remove anything from the server's room buffer. If your game
    /// buffers anything beyond Instantiate and RPC calls, that will be cleaned as well from server.
    ///
    /// Destroying all includes:
    /// - Remove anything from the server's room buffer (Instantiate, RPCs, anything buffered).
    /// - Sending a message to other clients to destroy everything locally, too (affected by network lag).
    ///
    /// Destroying networked objects works only if they got created with PhotonNetwork.Instantiate().
    /// Objects loaded with a scene are ignored, no matter if they have PhotonView components.
    /// </remarks>
    /// <returns>Nothing. Check error debug log for any issues.</returns>
    public static void DestroyAll()
    {
        if (isMasterClient)
        {
            networkingPeer.DestroyAll(false);
        }
        else
        {
            Debug.LogError("Couldn't call DestroyAll() as only the master client is allowed to call this.");
        }
    }

    /// <summary>
    /// Remove all buffered RPCs from server that were sent by targetPlayer. Can only be called on local player (for "self") or Master Client (for anyone).
    /// </summary>
    /// <remarks>
    /// This method requires either:
    /// - This is the targetPlayer's client.
    /// - This client is the Master Client (can remove any PhotonPlayer's RPCs).
    ///
    /// If the targetPlayer calls RPCs at the same time that this is called,
    /// network lag will determine if those get buffered or cleared like the rest.
    /// </remarks>
    /// <param name="targetPlayer">This player's buffered RPCs get removed from server buffer.</param>
    public static void RemoveRPCs(PhotonPlayer targetPlayer)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        if (!targetPlayer.isLocal && !isMasterClient)
        {
            Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
            return;
        }

        networkingPeer.OpCleanRpcBuffer(targetPlayer.ID);
    }

    /// <summary>
    /// Remove all buffered RPCs from server that were sent via targetPhotonView. The Master Client and the owner of the targetPhotonView may call this.
    /// </summary>
    /// <remarks>
    /// This method requires either:
    /// - The targetPhotonView is owned by this client (Instantiated by it).
    /// - This client is the Master Client (can remove any PhotonView's RPCs).
    /// </remarks>
    /// <param name="targetPhotonView">RPCs buffered for this PhotonView get removed from server buffer.</param>
    public static void RemoveRPCs(PhotonView targetPhotonView)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        networkingPeer.CleanRpcBufferIfMine(targetPhotonView);
    }

    /// <summary>
    /// Remove all buffered RPCs from server that were sent in the targetGroup, if this is the Master Client or if this controls the individual PhotonView.
    /// </summary>
    /// <remarks>
    /// This method requires either:
    /// - This client is the Master Client (can remove any RPCs per group).
    /// - Any other client: each PhotonView is checked if it is under this client's control. Only those RPCs are removed.
    /// </remarks>
    /// <param name="targetGroup">Interest group that gets all RPCs removed.</param>
    public static void RemoveRPCsInGroup(int targetGroup)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        networkingPeer.RemoveRPCsInGroup(targetGroup);
    }

    /// <summary>
    /// Internal to send an RPC on given PhotonView. Do not call this directly but use: PhotonView.RPC!
    /// </summary>
    internal static void RPC(PhotonView view, string methodName, PhotonTargets target, bool encrypt, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        if (room == null)
        {
            Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
            return;
        }

        if (networkingPeer != null)
        {
            if (PhotonNetwork.room.serverSideMasterClient)
            {
                networkingPeer.RPC(view, methodName, target, null, encrypt, parameters);
            }
            else
            {
                if (PhotonNetwork.networkingPeer.hasSwitchedMC && target == PhotonTargets.MasterClient)
                {
                    networkingPeer.RPC(view, methodName, PhotonTargets.Others, PhotonNetwork.masterClient, encrypt, parameters);
                }
                else
                {
                    networkingPeer.RPC(view, methodName, target, null, encrypt, parameters);
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    /// <summary>
    /// Internal to send an RPC on given PhotonView. Do not call this directly but use: PhotonView.RPC!
    /// </summary>
    internal static void RPC(PhotonView view, string methodName, PhotonPlayer targetPlayer, bool encrpyt, params object[] parameters)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        if (room == null)
        {
            Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("RPC can't be sent to target PhotonPlayer being null! Did not send \"" + methodName + "\" call.");
        }

        if (networkingPeer != null)
        {
            networkingPeer.RPC(view, methodName, PhotonTargets.Others, targetPlayer, encrpyt, parameters);
        }
        else
        {
            Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
        }
    }

    /// <summary>
    /// Populates SendMonoMessageTargets with currently existing GameObjects that have a Component of type.
    /// </summary>
    /// <param name="type">If null, this will use SendMonoMessageTargets as component-type (MonoBehaviour by default).</param>
    public static void CacheSendMonoMessageTargets(Type type)
    {
        if (type == null) type = SendMonoMessageTargetType;
        PhotonNetwork.SendMonoMessageTargets = FindGameObjectsWithComponent(type);
    }

    /// <summary>Finds the GameObjects with Components of a specific type (using FindObjectsOfType).</summary>
    /// <param name="type">Type must be a Component</param>
    /// <returns>HashSet with GameObjects that have a specific type of Component.</returns>
    public static HashSet<GameObject> FindGameObjectsWithComponent(Type type)
    {
        HashSet<GameObject> objectsWithComponent = new HashSet<GameObject>();

        Component[] targetComponents = (Component[]) GameObject.FindObjectsOfType(type);
        for (int index = 0; index < targetComponents.Length; index++)
        {
            objectsWithComponent.Add(targetComponents[index].gameObject);
        }

        return objectsWithComponent;
    }

    /// <summary>
    /// Enable/disable receiving on given group (applied to PhotonViews)
    /// </summary>
    /// <param name="group">The interest group to affect.</param>
    /// <param name="enabled">Sets if receiving from group to enabled (or not).</param>
    public static void SetReceivingEnabled(int group, bool enabled)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetReceivingEnabled(group, enabled);
    }


    /// <summary>
    /// Enable/disable receiving on given groups (applied to PhotonViews)
    /// </summary>
    /// <param name="enableGroups">The interest groups to enable (or null).</param>
    /// <param name="disableGroups">The interest groups to disable (or null).</param>
    public static void SetReceivingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetReceivingEnabled(enableGroups, disableGroups);
    }


    /// <summary>
    /// Enable/disable sending on given group (applied to PhotonViews)
    /// </summary>
    /// <param name="group">The interest group to affect.</param>
    /// <param name="enabled">Sets if sending to group is enabled (or not).</param>
    public static void SetSendingEnabled(int group, bool enabled)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        networkingPeer.SetSendingEnabled(group, enabled);
    }


    /// <summary>
    /// Enable/disable sending on given groups (applied to PhotonViews)
    /// </summary>
    /// <param name="enableGroups">The interest groups to enable sending on (or null).</param>
    /// <param name="disableGroups">The interest groups to disable sending on (or null).</param>
    public static void SetSendingEnabled(int[] enableGroups, int[] disableGroups)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }
        networkingPeer.SetSendingEnabled(enableGroups, disableGroups);
    }



    /// <summary>
    /// Sets level prefix for PhotonViews instantiated later on. Don't set it if you need only one!
    /// </summary>
    /// <remarks>
    /// Important: If you don't use multiple level prefixes, simply don't set this value. The
    /// default value is optimized out of the traffic.
    ///
    /// This won't affect existing PhotonViews (they can't be changed yet for existing PhotonViews).
    ///
    /// Messages sent with a different level prefix will be received but not executed. This affects
    /// RPCs, Instantiates and synchronization.
    ///
    /// Be aware that PUN never resets this value, you'll have to do so yourself.
    /// </remarks>
    /// <param name="prefix">Max value is short.MaxValue = 32767</param>
    public static void SetLevelPrefix(short prefix)
    {
        if (!VerifyCanUseNetwork())
        {
            return;
        }

        networkingPeer.SetLevelPrefix(prefix);
    }

    /// <summary>Wraps loading a level to pause the network mesage-queue. Optionally syncs the loaded level in a room.</summary>
    /// <remarks>
    /// To sync the loaded level in a room, set PhotonNetwork.automaticallySyncScene to true.
    /// The Master Client of a room will then sync the loaded level with every other player in the room.
    ///
    /// While loading levels, it makes sense to not dispatch messages received by other players.
    /// This method takes care of that by setting PhotonNetwork.isMessageQueueRunning = false and enabling
    /// the queue when the level was loaded.
    ///
    /// You should make sure you don't fire RPCs before you load another scene (which doesn't contain
    /// the same GameObjects and PhotonViews). You can call this in OnJoinedRoom.
    ///
    /// This uses Application.LoadLevel.
    /// </remarks>
    /// <param name='levelNumber'>
    /// Number of the level to load. When using level numbers, make sure they are identical on all clients.
    /// </param>
    public static void LoadLevel(int levelNumber)
    {
        networkingPeer.SetLevelInPropsIfSynced(levelNumber);

        PhotonNetwork.isMessageQueueRunning = false;
        networkingPeer.loadingLevelAndPausedNetwork = true;
        SceneManager.LoadScene(levelNumber);
    }

    /// <summary>Wraps loading a level to pause the network mesage-queue. Optionally syncs the loaded level in a room.</summary>
    /// <remarks>
    /// While loading levels, it makes sense to not dispatch messages received by other players.
    /// This method takes care of that by setting PhotonNetwork.isMessageQueueRunning = false and enabling
    /// the queue when the level was loaded.
    ///
    /// To sync the loaded level in a room, set PhotonNetwork.automaticallySyncScene to true.
    /// The Master Client of a room will then sync the loaded level with every other player in the room.
    ///
    /// You should make sure you don't fire RPCs before you load another scene (which doesn't contain
    /// the same GameObjects and PhotonViews). You can call this in OnJoinedRoom.
    ///
    /// This uses Application.LoadLevel.
    /// </remarks>
    /// <param name='levelName'>
    /// Name of the level to load. Make sure it's available to all clients in the same room.
    /// </param>
    public static void LoadLevel(string levelName)
    {
        networkingPeer.SetLevelInPropsIfSynced(levelName);

        PhotonNetwork.isMessageQueueRunning = false;
        networkingPeer.loadingLevelAndPausedNetwork = true;
        SceneManager.LoadScene(levelName);
    }


    /// <summary>
    /// This operation makes Photon call your custom web-service by name (path) with the given parameters.
    /// </summary>
    /// <remarks>
    /// This is a server-side feature which must be setup in the Photon Cloud Dashboard prior to use.<br/>
    /// See the Turnbased Feature Overview for a short intro.<br/>
    /// http://doc.photonengine.com/en/turnbased/current/getting-started/feature-overview
    ///<br/>
    /// The Parameters will be converted into JSon format, so make sure your parameters are compatible.
    ///
    /// See PhotonNetworkingMessage.OnWebRpcResponse on how to get a response.
    ///
    /// It's important to understand that the OperationResponse only tells if the WebRPC could be called.
    /// The content of the response contains any values your web-service sent and the error/success code.
    /// In case the web-service failed, an error code and a debug message are usually inside the
    /// OperationResponse.
    ///
    /// The class WebRpcResponse is a helper-class that extracts the most valuable content from the WebRPC
    /// response.
    /// </remarks>
    /// <example>
    /// Example callback implementation:<pre>
    ///
    /// public void OnWebRpcResponse(OperationResponse response)
    /// {
    ///     WebRpcResponse webResponse = new WebRpcResponse(operationResponse);
    ///     if (webResponse.ReturnCode != 0) { //...
    ///     }
    ///
    ///     switch (webResponse.Name) { //...
    ///     }
    ///     // and so on
    /// }</pre>
    /// </example>
    public static bool WebRpc(string name, object parameters)
    {
        return networkingPeer.WebRpc(name, parameters);
    }


#if UNITY_EDITOR
    [Conditional("UNITY_EDITOR")]
    public static void CreateSettings()
    {
        PhotonNetwork.PhotonServerSettings = (ServerSettings)Resources.Load(PhotonNetwork.serverSettingsAssetFile, typeof(ServerSettings));
        if (PhotonNetwork.PhotonServerSettings != null)
        {
            return;
        }

        // find out if ServerSettings can be instantiated (existing script check)
        ScriptableObject serverSettingTest = ScriptableObject.CreateInstance("ServerSettings");
        if (serverSettingTest == null)
        {
            Debug.LogError("missing settings script");
            return;
        }
        UnityEngine.Object.DestroyImmediate(serverSettingTest);


        // if still not loaded, create one
        if (PhotonNetwork.PhotonServerSettings == null)
        {
            string settingsPath = Path.GetDirectoryName(PhotonNetwork.serverSettingsAssetPath);
            if (!Directory.Exists(settingsPath))
            {
                Directory.CreateDirectory(settingsPath);
                AssetDatabase.ImportAsset(settingsPath);
            }

            PhotonNetwork.PhotonServerSettings = (ServerSettings)ScriptableObject.CreateInstance("ServerSettings");
            if (PhotonNetwork.PhotonServerSettings != null)
            {
                AssetDatabase.CreateAsset(PhotonNetwork.PhotonServerSettings, PhotonNetwork.serverSettingsAssetPath);
            }
            else
            {
                Debug.LogError("PUN failed creating a settings file. ScriptableObject.CreateInstance(\"ServerSettings\") returned null. Will try again later.");
            }
        }
    }


    /// <summary>
    /// Internally used by Editor scripts, called on Hierarchy change (includes scene save) to remove surplus hidden PhotonHandlers.
    /// </summary>
    public static void InternalCleanPhotonMonoFromSceneIfStuck()
    {
        PhotonHandler[] photonHandlers = GameObject.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
        if (photonHandlers != null && photonHandlers.Length > 0)
        {
            Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save it. This is not an issue.");
            foreach (PhotonHandler photonHandler in photonHandlers)
            {
                // Debug.Log("Removing Handler: " + photonHandler + " photonHandler.gameObject: " + photonHandler.gameObject);
                photonHandler.gameObject.hideFlags = 0;

                if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
                {
                    GameObject.DestroyImmediate(photonHandler.gameObject);
                }

                Component.DestroyImmediate(photonHandler);
            }
        }
    }
#endif

}
