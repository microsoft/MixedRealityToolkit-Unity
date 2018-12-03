// ----------------------------------------------------------------------------
// <copyright file="PhotonNetwork.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// PhotonNetwork is the central class of the PUN package.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using System.Diagnostics;
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;
    using UnityEngine.SceneManagement;

    using Photon.Realtime;
    using Debug = UnityEngine.Debug;
    using Hashtable = ExitGames.Client.Photon.Hashtable;

    #if UNITY_EDITOR
    using UnityEditor;
    using System.IO;
    #endif


    public struct InstantiateParameters
    {
        public int[] viewIDs;
        public byte objLevelPrefix;
        public object[] data;
        public byte @group;
        public Quaternion rotation;
        public Vector3 position;
        public string prefabName;
        public Player creator;
        public int timestamp;

        public InstantiateParameters(string prefabName, Vector3 position, Quaternion rotation, byte @group, object[] data, byte objLevelPrefix, int[] viewIDs, Player creator, int timestamp)
        {
            this.prefabName = prefabName;
            this.position = position;
            this.rotation = rotation;
            this.@group = @group;
            this.data = data;
            this.objLevelPrefix = objLevelPrefix;
            this.viewIDs = viewIDs;
            this.creator = creator;
            this.timestamp = timestamp;
        }
    }


    /// <summary>
    /// The main class to use the PhotonNetwork plugin.
    /// This class is static.
    /// </summary>
    /// \ingroup publicApi
    public static partial class PhotonNetwork
    {
        /// <summary>Version number of PUN. Used in the AppVersion, which separates your playerbase in matchmaking.</summary>
        public const string PunVersion = "2.5";

        /// <summary>Version number of your game. Setting this updates the AppVersion, which separates your playerbase in matchmaking.</summary>
        /// <remarks>
        /// In PUN, the GameVersion is only one component of the LoadBalancingClient.AppVersion.
        /// Setting the GameVersion will also set the LoadBalancingClient.AppVersion to: value+'_'+ PhotonNetwork.PunVersion.
        ///
        /// The AppVersion is used to split your playerbase as needed.
        /// One AppId may have various AppVersions and each is a separate set of users for matchmaking.
        ///
        /// The AppVersion gets sent in the "Authenticate" step.
        /// This means you can set the GameVersion right after calling ConnectUsingSettings (e.g.) and the new value will be used on the server.
        /// Once the client is connected, authentication is done and the value won't be sent to the server anymore.
        /// </remarks>
        public static string GameVersion
        {
            get { return gameVersion; }
            set
            {
                gameVersion = value;
                NetworkingClient.AppVersion = string.Format("{0}_{1}", value, PhotonNetwork.PunVersion);
            }
        }

        private static string gameVersion;

        /// <summary>Sent to Photon Server to specifiy the "Virtual AppId".</summary>
        /// <remarks>Sent with the operation Authenticate. When using PUN, you should set the GameVersion or use ConnectUsingSettings().</remarks>
        public static string AppVersion
        {
            get { return NetworkingClient.AppVersion; }
        }

        /// <summary>This Monobehaviour allows Photon to run an Update loop.</summary>
        private static readonly PhotonHandler photonMono;

        /// <summary>The LoadBalancingClient is part of Photon Realtime and wraps up multiple servers and states for PUN.</summary>
        public static LoadBalancingClient NetworkingClient;

        /// <summary>
        /// The maximum number of assigned PhotonViews <i>per player</i> (or scene). See the [General Documentation](@ref general) topic "Limitations" on how to raise this limitation.
        /// </summary>
        public static readonly int MAX_VIEW_IDS = 1000; // VIEW & PLAYER LIMIT CAN BE EASILY CHANGED, SEE DOCS


        /// <summary>Name of the PhotonServerSettings file (used to load and by PhotonEditor to save new files).</summary>
        internal const string ServerSettingsFileName = "PhotonServerSettings";

        /// <summary>Serialized server settings, written by the Setup Wizard for use in ConnectUsingSettings.</summary>
        public static ServerSettings PhotonServerSettings = (ServerSettings)Resources.Load(PhotonNetwork.ServerSettingsFileName, typeof(ServerSettings));

        /// <summary>Currently used server address (no matter if master or game server).</summary>
        public static string ServerAddress { get { return (NetworkingClient != null) ? NetworkingClient.CurrentServerAddress : "<not connected>"; } }

        /// <summary>Currently used Cloud Region (if any). As long as the client is not on a Master Server or Game Server, the region is not yet defined.</summary>
        public static string CloudRegion { get { return (NetworkingClient != null && IsConnected && Server!=ServerConnection.NameServer) ? NetworkingClient.CloudRegion : null; } }


        /// <summary>Key to save the "Best Region Summary" in the Player Preferences.</summary>
        private const string PlayerPrefsKey = "PUNCloudBestRegion";

        /// <summary>Used to store and access the "Best Region Summary" in the Player Preferences.</summary>
        public static string BestRegionSummaryInPreferences
        {
            get
            {
                return PlayerPrefs.GetString(PlayerPrefsKey, null);
            }
            internal set
            {
                if (String.IsNullOrEmpty(value))
                {
                    PlayerPrefs.DeleteKey(PlayerPrefsKey);
                }
                else
                {
                    PlayerPrefs.SetString(PlayerPrefsKey, value.ToString());
                }
            }
        }

        /// <summary>
        /// False until you connected to Photon initially. True in offline mode, while connected to any server and even while switching servers.
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                if (OfflineMode)
                {
                    return true;
                }

                if (NetworkingClient == null)
                {
                    return false;
                }

                return NetworkingClient.IsConnected;
            }
        }

        /// <summary>
        /// A refined version of connected which is true only if your connection to the server is ready to accept operations like join, leave, etc.
        /// </summary>
        public static bool IsConnectedAndReady
        {
            get
            {
                if (OfflineMode)
                {
                    return true;
                }
                if (NetworkingClient == null)
                {
                    return false;
                }

                return NetworkingClient.IsConnectedAndReady;
            }
        }

        /// <summary>
        /// Directly provides the network-level client state, unless in OfflineMode.
        /// </summary>
        /// <remarks>
        /// In context of PUN, you should usually use IsConnected or IsConnectedAndReady.
        ///
        /// This is the lower level connection state. Keep in mind that PUN uses more than one server,
        /// so the client may become Disconnected, even though it's just switching servers.
        ///
        /// While OfflineMode is true, this is ClientState.Joined (after create/join) or ConnectedToMasterserver in all other cases.
        /// </remarks>
        public static ClientState NetworkClientState
        {
            get
            {
                if (OfflineMode)
                {
                    return (offlineModeRoom != null) ? ClientState.Joined : ClientState.ConnectedToMasterserver;
                }

                if (NetworkingClient == null)
                {
                    return ClientState.Disconnected;
                }

                return NetworkingClient.State;
            }
        }

        /// <summary>Tracks, which Connect method was called last. </summary>
        /// <remarks>
        /// ConnectToMaster sets this to ConnectToMaster.
        /// ConnectToRegion sets this to ConnectToRegion.
        /// ConnectToBestCloudServer sets this to ConnectToBest.
        /// PhotonNetwork.ConnectUsingSettings will call either ConnectToMaster, ConnectToRegion or ConnectToBest, depending on the settings.
        /// </remarks>
        public static ConnectMethod ConnectMethod = ConnectMethod.NotCalled;


        /// <summary>The server (type) this client is currently connected or connecting to.</summary>
        /// <remarks>Photon uses 3 different roles of servers: Name Server, Master Server and Game Server.</remarks>
        public static ServerConnection Server { get { return (PhotonNetwork.NetworkingClient != null) ? PhotonNetwork.NetworkingClient.Server : ServerConnection.NameServer; } }

        /// <summary>
        /// A user's authentication values used during connect.
        /// </summary>
        /// <remarks>
        /// Set these before calling Connect if you want custom authentication.
        /// These values set the userId, if and how that userId gets verified (server-side), etc..
        ///
        /// If authentication fails for any values, PUN will call your implementation of OnCustomAuthenticationFailed(string debugMessage).
        /// See <see cref="Photon.Realtime.IConnectionCallbacks.OnCustomAuthenticationFailed"/>.
        /// </remarks>
        public static AuthenticationValues AuthValues
        {
            get { return (NetworkingClient != null) ? NetworkingClient.AuthValues : null; }
            set { if (NetworkingClient != null) NetworkingClient.AuthValues = value; }
        }

        /// <summary>
        /// The lobby that will be used when PUN joins a lobby or creates a game.
        /// </summary>
        /// <remarks>
        /// The default lobby uses an empty string as name.
        /// So when you connect or leave a room, PUN automatically gets you into a lobby again.
        ///
        /// Check PhotonNetwork.InLobby if the client is in a lobby.
        /// (@ref masterServerAndLobby)
        /// </remarks>
        public static TypedLobby CurrentLobby
        {
            get { return NetworkingClient.CurrentLobby; }
            set { NetworkingClient.CurrentLobby = value; }
        }

        /// <summary>
        /// Get the room we're currently in (also when in OfflineMode). Null if we aren't in any room.
        /// </summary>
        /// <remarks>
        /// LoadBalancing Client is not aware of the Photon Offline Mode, so never use PhotonNetwork.NetworkingClient.CurrentRoom will be null if you are using OffLine Mode, while PhotonNetwork.CurrentRoom will be set when offlineMode is true
        /// </remarks>
        public static Room CurrentRoom
        {
            get
            {
                if (offlineMode)
                {
                    return offlineModeRoom;
                }

                return NetworkingClient == null ? null : NetworkingClient.CurrentRoom;
            }
        }

        /// <summary>
        /// Controls how verbose PUN is.
        /// </summary>
        public static PunLogLevel LogLevel = PunLogLevel.ErrorsOnly;

        /// <summary>
        /// This client's Player instance is always available, unless the app shuts down.
        /// </summary>
        /// <remarks>
        /// Useful (e.g.) to set the Custom Player Properties or the NickName for this client anytime.
        /// When the client joins a room, the Custom Properties and other values are synced.
        /// </remarks>
        public static Player LocalPlayer
        {
            get
            {
                if (NetworkingClient == null)
                {
                    return null; // Surpress ExitApplication errors
                }

                return NetworkingClient.LocalPlayer;
            }
        }

        /// <summary>
        /// Set to synchronize the player's nickname with everyone in the room(s) you enter. This sets PhotonNetwork.player.NickName.
        /// </summary>
        /// <remarks>
        /// The NickName is just a nickname and does not have to be unique or backed up with some account.<br/>
        /// Set the value any time (e.g. before you connect) and it will be available to everyone you play with.<br/>
        /// Access the names of players by: Player.NickName. <br/>
        /// PhotonNetwork.PlayerListOthers is a list of other players - each contains the NickName the remote player set.
        /// </remarks>
        public static string NickName
        {
            get
            {
                return NetworkingClient.NickName;
            }

            set
            {
                NetworkingClient.NickName = value;
            }
        }

        /// <summary>
        /// A sorted copy of the players-list of the current room. This is using Linq, so better cache this value. Update when players join / leave.
        /// </summary>
        public static Player[] PlayerList
        {
            get
            {
                Room room = CurrentRoom;
                if (room != null)
                {
                    // TODO: implement more effectively. maybe cache?!
                    return room.Players.Values.OrderBy((x) => x.ActorNumber).ToArray();
                }
                return new Player[0];
            }
        }

        /// <summary>
        /// A sorted copy of the players-list of the current room, excluding this client. This is using Linq, so better cache this value. Update when players join / leave.
        /// </summary>
        public static Player[] PlayerListOthers
        {
            get
            {
                Room room = CurrentRoom;
                if (room != null)
                {
                    // TODO: implement more effectively. maybe cache?!
                    return room.Players.Values.OrderBy((x) => x.ActorNumber).Where(x => !x.IsLocal).ToArray();
                }
                return new Player[0];
            }
        }


        /// <summary>
        /// The minimum difference that a Vector2 or Vector3(e.g. a transforms rotation) needs to change before we send it via a PhotonView's OnSerialize/ObservingComponent.
        /// </summary>
        /// <remarks>
        /// Note that this is the sqrMagnitude. E.g. to send only after a 0.01 change on the Y-axix, we use 0.01f*0.01f=0.0001f. As a remedy against float inaccuracy we use 0.000099f instead of 0.0001f.
        /// </remarks>
        public static float PrecisionForVectorSynchronization = 0.000099f;

        /// <summary>
        /// The minimum angle that a rotation needs to change before we send it via a PhotonView's OnSerialize/ObservingComponent.
        /// </summary>
        public static float PrecisionForQuaternionSynchronization = 1.0f;

        /// <summary>
        /// The minimum difference between floats before we send it via a PhotonView's OnSerialize/ObservingComponent.
        /// </summary>
        public static float PrecisionForFloatSynchronization = 0.01f;


        /// <summary>
        /// Offline mode can be set to re-use your multiplayer code in singleplayer game modes.
        /// When this is on PhotonNetwork will not create any connections and there is near to
        /// no overhead. Mostly usefull for reusing RPC's and PhotonNetwork.Instantiate
        /// </summary>
        public static bool OfflineMode
        {
            get
            {
                return offlineMode;
            }

            set
            {
                if (value == offlineMode)
                {
                    return;
                }

                if (value && IsConnected)
                {
                    Debug.LogError("Can't start OFFLINE mode while connected!");
                    return;
                }

                if (NetworkingClient.IsConnected)
                {
                    NetworkingClient.Disconnect(); // Cleanup (also calls OnLeftRoom to reset stuff)
                }

                offlineMode = value;

                if (offlineMode)
                {
                    NetworkingClient.ChangeLocalID(-1);
                    //SendMonoMessage(PhotonNetworkingMessage.OnConnectedToMaster);
                    NetworkingClient.ConnectionCallbackTargets.OnConnectedToMaster();
                }
                else
                {
                    offlineModeRoom = null;
                    NetworkingClient.ChangeLocalID(-1);
                }
            }
        }

        private static bool offlineMode = false;
        private static Room offlineModeRoom = null;


        /// <summary>Defines if all clients in a room should load the same level as the Master Client (if that used PhotonNetwork.LoadLevel).</summary>
        /// <remarks>
        /// To synchronize the loaded level, the Master Client should use PhotonNetwork.LoadLevel.
        /// All clients will load the new scene immediately when they enter a room (even before the callback OnJoinedRoom) or on change.
        ///
        /// Internally, a Custom Room Property is set for the loaded scene. When a client reads that
        /// and is not in the same scene yet, it will immediately pause the Message Queue
        /// (PhotonNetwork.IsMessageQueueRunning = false) and load. When the scene finished loading,
        /// PUN will automatically re-enable the Message Queue.
        /// </remarks>
        public static bool AutomaticallySyncScene
        {
            get
            {
                return automaticallySyncScene;
            }
            set
            {
                automaticallySyncScene = value;
                if (automaticallySyncScene && CurrentRoom != null)
                {
                    LoadLevelIfSynced();
                }
            }
        }

        private static bool automaticallySyncScene = false;

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
        public static bool EnableLobbyStatistics
        {
            get
            {
                return NetworkingClient.EnableLobbyStatistics;
            }
        }


        /// <summary>True while this client is in a lobby.</summary>
        /// <remarks>
        /// Implement IPunCallbacks.OnRoomListUpdate() for a notification when the list of rooms
        /// becomes available or updated.
        ///
        /// You are automatically leaving any lobby when you join a room!
        /// Lobbies only exist on the Master Server (whereas rooms are handled by Game Servers).
        /// </remarks>
        public static bool InLobby
        {
            get
            {
                return NetworkingClient.InLobby;
            }
        }

        /// <summary>
        /// Defines how many times per second PhotonNetwork should send a package. If you change
        /// this, do not forget to also change 'SerializationRate'.
        /// </summary>
        /// <remarks>
        /// Less packages are less overhead but more delay.
        /// Setting the SendRate to 50 will create up to 50 packages per second (which is a lot!).
        /// Keep your target platform in mind: mobile networks are slower and less reliable.
        /// </remarks>
        public static int SendRate
        {
            get
            {
                return 1000 / sendFrequency;
            }

            set
            {
                sendFrequency = 1000 / value;
                if (photonMono != null)
                {
                    photonMono.UpdateInterval = sendFrequency;
                }

                if (value < SerializationRate)
                {
                    // SerializationRate needs to be <= SendRate
                    SerializationRate = value;
                }
            }
        }

        private static int sendFrequency = 50; // in miliseconds.

        /// <summary>
        /// Defines how many times per second OnPhotonSerialize should be called on PhotonViews.
        /// </summary>
        /// <remarks>
        /// Choose this value in relation to PhotonNetwork.SendRate. OnPhotonSerialize will create updates and messages to be sent.<br/>
        /// A lower rate takes up less performance but will cause more lag.
        /// </remarks>
        public static int SerializationRate
        {
            get
            {
                return 1000 / serializationFrequency;
            }

            set
            {
                if (value > SendRate)
                {
                    Debug.LogError("Error: Can not set the OnSerialize rate higher than the overall SendRate.");
                    value = SendRate;
                }

                serializationFrequency = 1000 / value;
                if (photonMono != null)
                {
                    photonMono.UpdateIntervalOnSerialize = serializationFrequency;
                }
            }
        }

        private static int serializationFrequency = 100; // in miliseconds. I.e. 100 = 100ms which makes 10 times/second

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
        public static bool IsMessageQueueRunning
        {
            get
            {
                return isMessageQueueRunning;
            }

            set
            {
                NetworkingClient.LoadBalancingPeer.IsSendingOnlyAcks = !value;
                isMessageQueueRunning = value;
            }
        }

        /// <summary>Backup for property IsMessageQueueRunning.</summary>
        private static bool isMessageQueueRunning = true;


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
        public static double Time
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
        /// be careful to use only time-differences to check the Time delta when things
        /// happen.
        ///
        /// This is the basis for PhotonNetwork.Time.
        /// </remarks>
        public static int ServerTimestamp
        {
            get
            {
                if (OfflineMode)
                {
                    if (StartupStopwatch != null && StartupStopwatch.IsRunning)
                    {
                        return (int)StartupStopwatch.ElapsedMilliseconds;
                    }
                    return Environment.TickCount;
                }

                return NetworkingClient.LoadBalancingPeer.ServerTimeInMilliSeconds;   // TODO: implement ServerTimeInMilliSeconds in LBC
            }
        }

        /// <summary>Used for Photon/PUN timing, as Time.time can't be called from Threads.</summary>
        private static readonly Stopwatch StartupStopwatch;

        /// <summary>
        /// Defines how many seconds PUN keeps the connection, after Unity's OnApplicationPause(true) call. Default: 60 seconds.
        /// </summary>
        /// <remarks>
        /// It's best practice to disconnect inactive apps/connections after a while but to also allow users to take calls, etc..
        /// We think a reasonable backgroung timeout is 60 seconds.
        ///
        /// To handle the timeout, implement: OnDisconnected(), as usual.
        /// Your application will "notice" the background disconnect when it becomes active again (running the Update() loop).
        ///
        /// If you need to separate this case from others, you need to track if the app was in the background
        /// (there is no special callback by PUN).
        ///
        /// A value below 0.1 seconds will disable this timeout (careful: connections can be kept indefinitely).
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
        /// </remarks>
        public static float BackgroundTimeout = 60.0f;

        /// <summary>
        /// Are we the master client?
        /// </summary>
        public static bool IsMasterClient
        {
            get
            {
                if (OfflineMode)
                {
                    return true;
                }

                return NetworkingClient.CurrentRoom != null && NetworkingClient.CurrentRoom.MasterClientId == LocalPlayer.ActorNumber;  // TODO: implement MasterClient shortcut in LBC?
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
        /// With OfflineMode == true, this always returns the PhotonNetwork.player.
        /// </remarks>
        public static Player MasterClient
        {
            get
            {
                if (OfflineMode)
                {
                    return PhotonNetwork.LocalPlayer;
                }

                if (NetworkingClient == null || NetworkingClient.CurrentRoom == null)
                {
                    return null;
                }

                return NetworkingClient.CurrentRoom.GetPlayer(NetworkingClient.CurrentRoom.MasterClientId);
            }
        }

        /// <summary>Is true while being in a room (NetworkClientState == ClientState.Joined).</summary>
        /// <remarks>
        /// Aside from polling this value, game logic should implement IMatchmakingCallbacks in some class
        /// and react when that gets called.<br/>
        ///
        /// Many actions can only be executed in a room, like Instantiate or Leave, etc.<br/>
        /// A client can join a room in offline mode. In that case, don't use LoadBalancingClient.InRoom, which
        /// does not cover offline mode.
        /// </remarks>
        public static bool InRoom
        {
            get
            {
                // in offline mode, you can be in a room too and NetworkClientState then returns Joined like on online mode!
                return NetworkClientState == ClientState.Joined;
            }
        }


        /// <summary>
        /// The count of players currently looking for a room (available on MasterServer in 5sec intervals).
        /// </summary>
        public static int CountOfPlayersOnMaster
        {
            get
            {
                return NetworkingClient.PlayersOnMasterCount;
            }
        }

        /// <summary>
        /// Count of users currently playing your app in some room (sent every 5sec by Master Server).
        /// Use PhotonNetwork.PlayerList.Length or PhotonNetwork.CurrentRoom.PlayerCount to get the count of players in the room you're in!
        /// </summary>
        public static int CountOfPlayersInRooms
        {
            get
            {
                return NetworkingClient.PlayersInRoomsCount;
            }
        }

        /// <summary>
        /// The count of players currently using this application (available on MasterServer in 5sec intervals).
        /// </summary>
        public static int CountOfPlayers
        {
            get
            {
                return NetworkingClient.PlayersInRoomsCount + NetworkingClient.PlayersOnMasterCount;
            }
        }

        /// <summary>
        /// The count of rooms currently in use (available on MasterServer in 5sec intervals).
        /// </summary>
        /// <remarks>
        /// While inside the lobby you can also check the count of listed rooms as: PhotonNetwork.GetRoomList().Length.
        /// Since PUN v1.25 this is only based on the statistic event Photon sends (counting all rooms).
        /// </remarks>
        public static int CountOfRooms
        {
            get
            {
                return NetworkingClient.RoomsCount;
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
                return NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled;
            }

            set
            {
                NetworkingClient.LoadBalancingPeer.TrafficStatsEnabled = value;
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
            get { return NetworkingClient.LoadBalancingPeer.ResentReliableCommands; }
        }

        /// <summary>Crc checks can be useful to detect and avoid issues with broken datagrams. Can be enabled while not connected.</summary>
        public static bool CrcCheckEnabled
        {
            get { return NetworkingClient.LoadBalancingPeer.CrcEnabled; }
            set
            {
                if (!IsConnected)
                {
                    NetworkingClient.LoadBalancingPeer.CrcEnabled = value;
                }
                else
                {
                    Debug.Log("Can't change CrcCheckEnabled while being connected. CrcCheckEnabled stays " + NetworkingClient.LoadBalancingPeer.CrcEnabled);
                }
            }
        }

        /// <summary>If CrcCheckEnabled, this counts the incoming packages that don't have a valid CRC checksum and got rejected.</summary>
        public static int PacketLossByCrcCheck
        {
            get { return NetworkingClient.LoadBalancingPeer.PacketLossByCrc; }
        }

        /// <summary>Defines the number of times a reliable message can be resent before not getting an ACK for it will trigger a disconnect. Default: 5.</summary>
        /// <remarks>Less resends mean quicker disconnects, while more can lead to much more lag without helping. Min: 3. Max: 10.</remarks>
        public static int MaxResendsBeforeDisconnect
        {
            get { return NetworkingClient.LoadBalancingPeer.SentCountAllowance; }
            set
            {
                if (value < 3) value = 3;
                if (value > 10) value = 10;
                NetworkingClient.LoadBalancingPeer.SentCountAllowance = value;
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
            get { return NetworkingClient.LoadBalancingPeer.QuickResendAttempts; }
            set
            {
                if (value < 0) value = 0;
                if (value > 3) value = 3;
                NetworkingClient.LoadBalancingPeer.QuickResendAttempts = (byte)value;
            }
        }


        /// <summary>Switch to alternative ports for a UDP connection to the Public Cloud.</summary>
        /// <remarks>
        /// This should be used when a customer has issues with connection stability. Some players
        /// reported better connectivity for Steam games. The effect might vary, which is why the
        /// alternative ports are not the new default.
        ///
        /// The alternative (server) ports are 27000 up to 27003.
        ///
        /// The values are appplied by replacing any incoming server-address string accordingly.
        /// You only need to set this to true though.
        ///
        /// This value does not affect TCP or WebSocket connections.
        /// </remarks>
        public static bool UseAlternativeUdpPorts
        {
            get { return (NetworkingClient == null) ? false :  NetworkingClient.UseAlternativeUdpPorts; }
            set { if (NetworkingClient != null) NetworkingClient.UseAlternativeUdpPorts = value; }
        }


        private static int lastUsedViewSubId = 0;  // each player only needs to remember it's own (!) last used subId to speed up assignment
        private static int lastUsedViewSubIdStatic = 0;  // per room, the master is able to instantiate GOs. the subId for this must be unique too


        /// <summary>
        /// Static constructor used for basic setup.
        /// </summary>
        static PhotonNetwork()
        {
            #if UNITY_EDITOR

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                //Debug.Log(string.Format("PhotonNetwork.ctor() Not playing {0} {1}", UnityEditor.EditorApplication.isPlaying, UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode));
                return;
            }

            // This can happen when you recompile a script IN play mode
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

            if (PhotonServerSettings != null)
            {
                Application.runInBackground = PhotonServerSettings.RunInBackground;
            }

            PrefabPool = new DefaultPool();


            // Set up the NetworkingPeer and use protocol of PhotonServerSettings
            ConnectionProtocol protocol = PhotonNetwork.PhotonServerSettings.AppSettings.Protocol;
            NetworkingClient = new LoadBalancingClient(protocol);
            NetworkingClient.LoadBalancingPeer.QuickResendAttempts = 2;
            NetworkingClient.LoadBalancingPeer.SentCountAllowance = 7;

            NetworkingClient.EventReceived += OnEvent;
            NetworkingClient.OpResponseReceived += OnOperation;
            NetworkingClient.StateChanged += (previousState, state) =>
                                                    {
														if ( 
															(previousState == ClientState.Joined && state == ClientState.Disconnected) || 
															(Server == ServerConnection.GameServer && (state == ClientState.Disconnecting || state == ClientState.DisconnectingFromGameserver))
															)
														{
										                	LeftRoomCleanup();
														}
                                                    };



            // RPC shortcut lookup creation (from list of RPCs, which is updated by Editor scripts)
            rpcShortcuts = new Dictionary<string, int>(PhotonNetwork.PhotonServerSettings.RpcList.Count);
            for (int index = 0; index < PhotonNetwork.PhotonServerSettings.RpcList.Count; index++)
            {
                var name = PhotonNetwork.PhotonServerSettings.RpcList[index];
                rpcShortcuts[name] = index;
            }


            StartupStopwatch = new Stopwatch();
            StartupStopwatch.Start();
            NetworkingClient.LoadBalancingPeer.LocalMsTimestampDelegate = () => (int)StartupStopwatch.ElapsedMilliseconds;


            // PUN custom types (typical for Unity)
            CustomTypes.Register();


            // Create a (hidden) GameObject to run Photon/PUN
            GameObject photonGO = new GameObject();
            photonMono = (PhotonHandler)photonGO.AddComponent<PhotonHandler>();
            photonGO.name = "PhotonMono";
            photonGO.hideFlags = HideFlags.HideInHierarchy;
        }


        /// <summary>Connect to Photon as configured in the PhotonServerSettings file.</summary>
        /// <remarks>
        /// Implement IConnectionCallbacks, to make your game logic aware of state changes.
        /// Especially, IConnectionCallbacks.ConnectedToMasterserver is useful to react when
        /// the client can do matchmaking.
        ///
        /// This method will disable OfflineMode (which won't destroy any instantiated GOs) and it
        /// will set IsMessageQueueRunning to true.
        ///
        /// Your Photon configuration is created by the PUN Wizard and contains the AppId,
        /// region for Photon Cloud games, the server address among other things.
        ///
        /// To ignore the settings file, set the relevant values and connect by calling
        /// ConnectToMaster, ConnectToRegion.
        ///
        /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
        /// https://dashboard.photonengine.com
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Invalid AppId
        /// - Network issues
        /// - Invalid region
        /// - Subscription CCU limit reached
        /// - etc.
        ///
        /// In general check out the <see cref="DisconnectCause"/> from the <see cref="IConnectionCallbacks.OnDisconnected"/> callback.
        ///  </remarks>
        public static bool ConnectUsingSettings()
        {
            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                Debug.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }
            if (PhotonServerSettings == null)
            {
                Debug.LogError("Can't connect: Loading settings failed. ServerSettings asset must be in any 'Resources' folder as: " + ServerSettingsFileName);
                return false;
            }


            // apply the AppSettings.Protocol
            #if UNITY_WEBGL
            if (PhotonServerSettings.AppSettings.Protocol == ConnectionProtocol.WebSocket || PhotonServerSettings.AppSettings.Protocol == ConnectionProtocol.WebSocketSecure)
            {
                NetworkingClient.LoadBalancingPeer.TransportProtocol = PhotonServerSettings.AppSettings.Protocol;
            }
            #else
            NetworkingClient.LoadBalancingPeer.TransportProtocol = PhotonServerSettings.AppSettings.Protocol;
            #endif


            if (PhotonServerSettings.StartInOfflineMode)
            {
                OfflineMode = true;
                return true;
            }

            if (OfflineMode)
            {
                OfflineMode = false; // Cleanup offline mode
                // someone can set OfflineMode in code and then call ConnectUsingSettings() with non-offline settings. Warning for that case:
                Debug.LogWarning("ConnectUsingSettings() disabled the offline mode. No longer offline.");
            }


            IsMessageQueueRunning = true;
            NetworkingClient.AppId = PhotonServerSettings.AppSettings.AppIdRealtime;
            GameVersion = PhotonServerSettings.AppSettings.AppVersion;
            NetworkingClient.EnableLobbyStatistics = PhotonNetwork.PhotonServerSettings.AppSettings.EnableLobbyStatistics;

            SetupLogging();


            if (PhotonServerSettings.AppSettings.IsMasterServerAddress)
            {
                NetworkingClient.LoadBalancingPeer.SerializationProtocolType = SerializationProtocol.GpBinaryV16;   // this is a workaround to use On Premise Servers, which don't support GpBinaryV18 yet.
                return ConnectToMaster(PhotonServerSettings.AppSettings.Server, PhotonServerSettings.AppSettings.Port, PhotonServerSettings.AppSettings.AppIdRealtime);
            }


            if (!PhotonServerSettings.AppSettings.IsDefaultNameServer)
            {
                NetworkingClient.NameServerHost = PhotonServerSettings.AppSettings.Server;
            }

            if (PhotonServerSettings.AppSettings.IsBestRegion)
            {
                return ConnectToBestCloudServer();
            }

            return ConnectToRegion(PhotonServerSettings.AppSettings.FixedRegion);
        }


        /// <summary>Connect to a Photon Master Server by address, port, appID.</summary>
        /// <remarks>
        /// To connect to the Photon Cloud, a valid AppId must be in the settings file (shown in the Photon Cloud Dashboard).
        /// https://dashboard.photonengine.com
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Invalid AppId
        /// - Network issues
        /// - Invalid region
        /// - Subscription CCU limit reached
        /// - etc.
        ///
        /// In general check out the <see cref="DisconnectCause"/> from the <see cref="IConnectionCallbacks.OnDisconnected"/> callback.
        /// </remarks>
        /// <param name="masterServerAddress">The server's address (either your own or Photon Cloud address).</param>
        /// <param name="port">The server's port to connect to.</param>
        /// <param name="appID">Your application ID (Photon Cloud provides you with a GUID for your game).</param>
        public static bool ConnectToMaster(string masterServerAddress, int port, string appID)
        {
            // TODO: refactor NetworkingClient.LoadBalancingPeer.PeerState to not use the peer but LBC.connected or so
            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                Debug.LogWarning("ConnectToMaster() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            if (OfflineMode)
            {
                OfflineMode = false; // Cleanup offline mode
                Debug.LogWarning("ConnectToMaster() disabled the offline mode. No longer offline.");
            }

            if (!IsMessageQueueRunning)
            {
                IsMessageQueueRunning = true;
                Debug.LogWarning("ConnectToMaster() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
            }

            SetupLogging();
            ConnectMethod = ConnectMethod.ConnectToMaster;

            NetworkingClient.IsUsingNameServer = false;
            NetworkingClient.MasterServerAddress = (port == 0) ? masterServerAddress : masterServerAddress + ":" + port;
            NetworkingClient.AppId = appID;

            return NetworkingClient.Connect();
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
        /// https://dashboard.photonengine.com
        ///
        /// Connecting to the Photon Cloud might fail due to:
        /// - Invalid AppId
        /// - Network issues
        /// - Invalid region
        /// - Subscription CCU limit reached
        /// - etc.
        ///
        /// In general check out the <see cref="DisconnectCause"/> from the <see cref="IConnectionCallbacks.OnDisconnected"/> callback.
        /// </remarks>
        /// <returns>If this client is going to connect to cloud server based on ping. Even if true, this does not guarantee a connection but the attempt is being made.</returns>
        public static bool ConnectToBestCloudServer()
        {
            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                Debug.LogWarning("ConnectToBestCloudServer() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            SetupLogging();
            ConnectMethod = ConnectMethod.ConnectToBest;

            // Connecting to "Best Region" begins with connecting to the Name Server.
            bool couldConnect = PhotonNetwork.NetworkingClient.ConnectToNameServer();
            return couldConnect;
        }


        /// <summary>
        /// Connects to the Photon Cloud region of choice.
        /// </summary>
        public static bool ConnectToRegion(string region)
        {
            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected && NetworkingClient.Server != ServerConnection.NameServer)
            {
                Debug.LogWarning("ConnectToRegion() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            SetupLogging();
            ConnectMethod = ConnectMethod.ConnectToRegion;

            if (!string.IsNullOrEmpty(region))
            {
                return NetworkingClient.ConnectToRegionMaster(region);
            }

            return false;
        }


        /// <summary>
        /// Makes this client disconnect from the photon server, a process that leaves any room and calls OnDisconnected on completion.
        /// </summary>
        /// <remarks>
        /// When you disconnect, the client will send a "disconnecting" message to the server. This speeds up leave/disconnect
        /// messages for players in the same room as you (otherwise the server would timeout this client's connection).
        /// When used in OfflineMode, the state-change and event-call OnDisconnected are immediate.
        /// Offline mode is set to false as well.
        /// Once disconnected, the client can connect again. Use ConnectUsingSettings.
        /// </remarks>
        public static void Disconnect()
        {
            if (OfflineMode)
            {
                OfflineMode = false;
                offlineModeRoom = null;
                NetworkingClient.State = ClientState.Disconnecting;
                NetworkingClient.OnStatusChanged(StatusCode.Disconnect);
                return;
            }

            if (NetworkingClient == null)
            {
                return; // Surpress error when quitting playmode in the editor
            }

            NetworkingClient.Disconnect();
        }

        /// <summary>Can be used to reconnect to the master server after a disconnect.</summary>
        /// <remarks>
        /// After losing connection, you can use this to connect a client to the region Master Server again.
        /// Cache the room name you're in and use RejoinRoom(roomname) to return to a game.
        /// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
        /// </remarks>
        public static bool Reconnect()
        {
            if (string.IsNullOrEmpty(NetworkingClient.MasterServerAddress))
            {
                Debug.LogWarning("Reconnect() failed. It seems the client wasn't connected before?! Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                Debug.LogWarning("Reconnect() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }

            if (OfflineMode)
            {
                OfflineMode = false; // Cleanup offline mode
                Debug.LogWarning("Reconnect() disabled the offline mode. No longer offline.");
            }

            if (!IsMessageQueueRunning)
            {
                IsMessageQueueRunning = true;
                Debug.LogWarning("Reconnect() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
            }

            NetworkingClient.IsUsingNameServer = false;
            return NetworkingClient.ReconnectToMaster();
        }


        /// <summary>
        /// Resets the traffic stats and re-enables them.
        /// </summary>
        public static void NetworkStatisticsReset()
        {
            NetworkingClient.LoadBalancingPeer.TrafficStatsReset();
        }


        /// <summary>
        /// Only available when NetworkStatisticsEnabled was used to gather some stats.
        /// </summary>
        /// <returns>A string with vital networking statistics.</returns>
        public static string NetworkStatisticsToString()
        {
            if (NetworkingClient == null || OfflineMode)
            {
                return "Offline or in OfflineMode. No VitalStats available.";
            }

            return NetworkingClient.LoadBalancingPeer.VitalStatsToString(false);
        }


        /// <summary>
        /// Helper function which is called inside this class to erify if certain functions can be used (e.g. RPC when not connected)
        /// </summary>
        /// <returns></returns>
        private static bool VerifyCanUseNetwork()
        {
            if (IsConnected)
            {
                return true;
            }

            Debug.LogError("Cannot send messages when not connected. Either connect to Photon OR use offline mode!");
            return false;
        }


        /// <summary>
        /// The current roundtrip time to the photon server.
        /// </summary>
        /// <returns>Roundtrip time (to server and back).</returns>
        public static int GetPing()
        {
            return NetworkingClient.LoadBalancingPeer.RoundTripTime;
        }

        /// <summary>Refreshes the server timestamp (async operation, takes a roundtrip).</summary>
        /// <remarks>Can be useful if a bad connection made the timestamp unusable or imprecise.</remarks>
        public static void FetchServerTimestamp()
        {
            if (NetworkingClient != null)
            {
                NetworkingClient.LoadBalancingPeer.FetchServerTimestamp();
            }
        }

        /// <summary>
        /// Can be used to immediately send the RPCs and Instantiates just called, so they are on their way to the other players.
        /// </summary>
        /// <remarks>
        /// This could be useful if you do a RPC to load a level and then load it yourself.
        /// While loading, no RPCs are sent to others, so this would delay the "load" RPC.
        /// You can send the RPC to "others", use this method, disable the message queue
        /// (by IsMessageQueueRunning) and then load.
        /// </remarks>
        public static void SendAllOutgoingCommands()
        {
            if (!VerifyCanUseNetwork())
            {
                return;
            }

            while (NetworkingClient.LoadBalancingPeer.SendOutgoingCommands())
            {
            }
        }

        /// <summary>Request a client to disconnect (KICK). Only the master client can do this</summary>
        /// <remarks>Only the target player gets this event. That player will disconnect automatically, which is what the others will notice, too.</remarks>
        /// <param name="kickPlayer">The Player to kick.</param>
        public static bool CloseConnection(Player kickPlayer)
        {
            if (!VerifyCanUseNetwork())
            {
                return false;
            }

            if (!LocalPlayer.IsMasterClient)
            {
                Debug.LogError("CloseConnection: Only the masterclient can kick another player.");
                return false;
            }

            if (kickPlayer == null)
            {
                Debug.LogError("CloseConnection: No such player connected!");
                return false;
            }

            RaiseEventOptions options = new RaiseEventOptions() { TargetActors = new int[] { kickPlayer.ActorNumber } };
            return NetworkingClient.OpRaiseEvent(PunEvent.CloseConnection, null, options, SendOptions.SendReliable);
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
        /// See also: PhotonNetwork.MasterClient
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
        /// <returns>False when this operation couldn't be done. Must be in a room (not in OfflineMode).</returns>
        public static bool SetMasterClient(Player masterClientPlayer)
        {
            if (!InRoom || !VerifyCanUseNetwork() || OfflineMode)
            {
                if (LogLevel == PunLogLevel.Informational) Debug.Log("Can not SetMasterClient(). Not in room or in OfflineMode.");
                return false;
            }

            Hashtable newProps = new Hashtable() { { GamePropertyKey.MasterClientId, masterClientPlayer.ActorNumber } };
            Hashtable prevProps = new Hashtable() { { GamePropertyKey.MasterClientId, NetworkingClient.CurrentRoom.MasterClientId } };
            return NetworkingClient.OpSetPropertiesOfRoom(newProps, expectedProperties: prevProps);
        }


        /// <summary>
        /// Joins a random room that matches the filter. Will callback: OnJoinedRoom or OnJoinRandomFailed.
        /// </summary>
        /// <remarks>
        /// Used for random matchmaking. You can join any room or one with specific properties defined in opJoinRandomRoomParams.
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed, in another lobby or not visible).
        /// It may also fail when actually joining the room which was found. Rooms may close, become full or empty anytime.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        public static bool JoinRandomRoom()
        {
            return JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, null, null);
        }

        /// <summary>
        /// Joins a random room that matches the filter. Will callback: OnJoinedRoom or OnJoinRandomFailed.
        /// </summary>
        /// <remarks>
        /// Used for random matchmaking. You can join any room or one with specific properties defined in opJoinRandomRoomParams.
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed, in another lobby or not visible).
        /// It may also fail when actually joining the room which was found. Rooms may close, become full or empty anytime.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="expectedCustomRoomProperties">Filters for rooms that match these custom properties (string keys and values). To ignore, pass null.</param>
        /// <param name="expectedMaxPlayers">Filters for a particular maxplayer setting. Use 0 to accept any maxPlayer value.</param>
        /// <returns>If the operation got queued and will be sent.</returns>
        public static bool JoinRandomRoom(Hashtable expectedCustomRoomProperties, byte expectedMaxPlayers)
        {
            return JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, null, null);
        }

        /// <summary>
        /// Joins a random room that matches the filter. Will callback: OnJoinedRoom or OnJoinRandomFailed.
        /// </summary>
        /// <remarks>
        /// Used for random matchmaking. You can join any room or one with specific properties defined in opJoinRandomRoomParams.
        ///
        /// This operation fails if no rooms are fitting or available (all full, closed, in another lobby or not visible).
        /// It may also fail when actually joining the room which was found. Rooms may close, become full or empty anytime.
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
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
            if (OfflineMode)
            {
                if (offlineModeRoom != null)
                {
                    Debug.LogError("JoinRandomRoom failed. In offline mode you still have to leave a room to enter another.");
                    return false;
                }
                EnterOfflineRoom("offline room", null, true);
                return true;
            }
            if (NetworkingClient.Server != ServerConnection.MasterServer || !IsConnectedAndReady)
            {
                Debug.LogError("JoinRandomRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }

            typedLobby = typedLobby ?? ((NetworkingClient.InLobby) ? NetworkingClient.CurrentLobby : null);  // use given lobby, or active lobby (if any active) or none

            OpJoinRandomRoomParams opParams = new OpJoinRandomRoomParams();
            opParams.ExpectedCustomRoomProperties = expectedCustomRoomProperties;
            opParams.ExpectedMaxPlayers = expectedMaxPlayers;
            opParams.MatchingType = matchingType;
            opParams.TypedLobby = typedLobby;
            opParams.SqlLobbyFilter = sqlLobbyFilter;
            opParams.ExpectedUsers = expectedUsers;

            return NetworkingClient.OpJoinRandomRoom(opParams);
        }


        /// <summary>
        /// Creates a new room. Will callback: OnCreatedRoom and OnJoinedRoom or OnCreateRoomFailed.
        /// </summary>
        /// <remarks>
        /// When successful, this calls the callbacks OnCreatedRoom and OnJoinedRoom (the latter, cause you join as first player).
        /// In all error cases, OnCreateRoomFailed gets called.
        ///
        /// Creating a room will fail if the room name is already in use or when the RoomOptions clashing
        /// with one another. Check the EnterRoomParams reference for the various room creation options.
        ///
        /// If you don't want to create a unique room-name, pass null or "" as name and the server will assign a roomName (a GUID as string).
        ///
        /// This method can only be called while the client is connected to a Master Server so you should
        /// implement the callback OnConnectedToMaster.
        /// Check the return value to make sure the operation will be called on the server.
        /// Note: There will be no callbacks if this method returned false.
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="roomName">Unique name of the room to create. Pass null or "" to make the server generate a name.</param>
        /// <param name="roomOptions">Common options for the room like MaxPlayers, initial custom room properties and similar. See RoomOptions type..</param>
        /// <param name="typedLobby">If null, the room is automatically created in the currently used lobby (which is "default" when you didn't join one explicitly).</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation got queued and will be sent.</returns>
        public static bool CreateRoom(string roomName, RoomOptions roomOptions = null, TypedLobby typedLobby = null, string[] expectedUsers = null)
        {
            if (OfflineMode)
            {
                if (offlineModeRoom != null)
                {
                    Debug.LogError("CreateRoom failed. In offline mode you still have to leave a room to enter another.");
                    return false;
                }
                EnterOfflineRoom(roomName, roomOptions, true);
                return true;
            }
            if (NetworkingClient.Server != ServerConnection.MasterServer || !IsConnectedAndReady)
            {
                Debug.LogError("CreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }

            typedLobby = typedLobby ?? ((NetworkingClient.InLobby) ? NetworkingClient.CurrentLobby : null);  // use given lobby, or active lobby (if any active) or none

            EnterRoomParams opParams = new EnterRoomParams();
            opParams.RoomName = roomName;
            opParams.RoomOptions = roomOptions;
            opParams.Lobby = typedLobby;
            opParams.ExpectedUsers = expectedUsers;

            return NetworkingClient.OpCreateRoom(opParams);
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
        ///
        /// If you set room properties in roomOptions, they get ignored when the room is existing already.
        /// This avoids changing the room properties by late joining players.
        ///
        /// You can define an array of expectedUsers, to block player slots in the room for these users.
        /// The corresponding feature in Photon is called "Slot Reservation" and can be found in the doc pages.
        ///
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <param name="roomName">Name of the room to join. Must be non null.</param>
        /// <param name="roomOptions">Options for the room, in case it does not exist yet. Else these values are ignored.</param>
        /// <param name="typedLobby">Lobby you want a new room to be listed in. Ignored if the room was existing and got joined.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation got queued and will be sent.</returns>
        public static bool JoinOrCreateRoom(string roomName, RoomOptions roomOptions, TypedLobby typedLobby, string[] expectedUsers = null)
        {
            if (OfflineMode)
            {
                if (offlineModeRoom != null)
                {
                    Debug.LogError("JoinOrCreateRoom failed. In offline mode you still have to leave a room to enter another.");
                    return false;
                }
                EnterOfflineRoom(roomName, roomOptions, true);  // in offline mode, JoinOrCreateRoom assumes you create the room
                return true;
            }
            if (NetworkingClient.Server != ServerConnection.MasterServer || !IsConnectedAndReady)
            {
                Debug.LogError("JoinOrCreateRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("JoinOrCreateRoom failed. A roomname is required. If you don't know one, how will you join?");
                return false;
            }

            typedLobby = typedLobby ?? ((NetworkingClient.InLobby) ? NetworkingClient.CurrentLobby : null);  // use given lobby, or active lobby (if any active) or none

            EnterRoomParams opParams = new EnterRoomParams();
            opParams.RoomName = roomName;
            opParams.RoomOptions = roomOptions;
            opParams.Lobby = typedLobby;
            opParams.CreateIfNotExists = true;
            opParams.PlayerProperties = LocalPlayer.CustomProperties;
            opParams.ExpectedUsers = expectedUsers;

            return NetworkingClient.OpJoinRoom(opParams);
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
        ///
        /// More about PUN matchmaking:
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        /// </remarks>
        /// <see cref="OnJoinRoomFailed"/>
        /// <see cref="OnJoinedRoom"/>
        /// <param name="roomName">Unique name of the room to join.</param>
        /// <param name="expectedUsers">Optional list of users (by UserId) who are expected to join this game and who you want to block a slot for.</param>
        /// <returns>If the operation got queued and will be sent.</returns>
        public static bool JoinRoom(string roomName, string[] expectedUsers = null)
        {
            if (OfflineMode)
            {
                if (offlineModeRoom != null)
                {
                    Debug.LogError("JoinRoom failed. In offline mode you still have to leave a room to enter another.");
                    return false;
                }
                EnterOfflineRoom(roomName, null, true);
                return true;
            }
            if (NetworkingClient.Server != ServerConnection.MasterServer || !IsConnectedAndReady)
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

            return NetworkingClient.OpJoinRoom(opParams);
        }


        /// <summary>
        /// Rejoins a room by roomName (using the userID internally to return).  Will callback: OnJoinedRoom or OnJoinRoomFailed.
        /// </summary>
        /// <remarks>
        /// After losing connection, you might be able to return to a room and continue playing,
        /// if the client is reconnecting fast enough. Use Reconnect() and this method.
        /// Cache the room name you're in and use RejoinRoom(roomname) to return to a game.
        ///
        /// Note: To be able to Rejoin any room, you need to use UserIDs!
        /// You also need to set RoomOptions.PlayerTtl.
        ///
        /// <b>Important: Instantiate() and use of RPCs is not yet supported.</b>
        /// The ownership rules of PhotonViews prevent a seamless return to a game, if you use PhotonViews.
        /// Use Custom Properties and RaiseEvent with event caching instead.
        ///
        /// Common use case: Press the Lock Button on a iOS device and you get disconnected immediately.
        /// 
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        /// </remarks>
        public static bool RejoinRoom(string roomName)
        {
            if (OfflineMode)
            {
                Debug.LogError("RejoinRoom failed due to offline mode.");
                return false;
            }
            if (NetworkingClient.Server != ServerConnection.MasterServer || !IsConnectedAndReady)
            {
                Debug.LogError("RejoinRoom failed. Client is not on Master Server or not yet ready to call operations. Wait for callback: OnJoinedLobby or OnConnectedToMaster.");
                return false;
            }
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogError("RejoinRoom failed. A roomname is required. If you don't know one, how will you join?");
                return false;
            }

            EnterRoomParams opParams = new EnterRoomParams();
            opParams.RoomName = roomName;
            opParams.RejoinOnly = true;
            opParams.PlayerProperties = LocalPlayer.CustomProperties;

            return NetworkingClient.OpJoinRoom(opParams);
        }


        /// <summary>When the client lost connection during gameplay, this method attempts to reconnect and rejoin the room.</summary>
        /// <remarks>
        /// This method re-connects directly to the game server which was hosting the room PUN was in before.
        /// If the room was shut down in the meantime, PUN will call OnJoinRoomFailed and return this client to the Master Server.
        ///
        /// Check the return value, if this client will attempt a reconnect and rejoin (if the conditions are met).
        /// If ReconnectAndRejoin returns false, you can still attempt a Reconnect and Rejoin.
        ///
        /// Similar to PhotonNetwork.RejoinRoom, this requires you to use unique IDs per player (the UserID).
        /// 
        /// Rejoining room will not send any player properties. Instead client will receive up-to-date ones from server.
        /// If you want to set new player properties, do it once rejoined.
        /// </remarks>
        /// <returns>False, if there is no known room or game server to return to. Then, this client does not attempt the ReconnectAndRejoin.</returns>
        public static bool ReconnectAndRejoin()
        {
            if (NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                Debug.LogWarning("ReconnectAndRejoin() failed. Can only connect while in state 'Disconnected'. Current state: " + NetworkingClient.LoadBalancingPeer.PeerState);
                return false;
            }
            if (OfflineMode)
            {
                OfflineMode = false; // Cleanup offline mode
                Debug.LogWarning("ReconnectAndRejoin() disabled the offline mode. No longer offline.");
            }

            if (!IsMessageQueueRunning)
            {
                IsMessageQueueRunning = true;
                Debug.LogWarning("ReconnectAndRejoin() enabled IsMessageQueueRunning. Needs to be able to dispatch incoming messages.");
            }

            NetworkingClient.IsUsingNameServer = false;
            return NetworkingClient.ReconnectAndRejoin();
        }


        /// <summary>Leave the current room and return to the Master Server where you can join or create rooms (see remarks).</summary>
        /// <remarks>
        /// This will clean up all (network) GameObjects with a PhotonView, unless you changed autoCleanUp to false.
        /// Returns to the Master Server.
        ///
        /// In OfflineMode, the local "fake" room gets cleaned up and OnLeftRoom gets called immediately.
        ///
        /// In a room with playerTTL &lt; 0, LeaveRoom just turns a client inactive. The player stays in the room's player list
        /// and can return later on. Setting becomeInactive to false deliberately, means to "abandon" the room, despite the
        /// playerTTL allowing you to come back.
        ///
        /// In a room with playerTTL == 0, become inactive has no effect (clients are removed from the room right away).
        /// </remarks>
        /// <param name="becomeInactive">If this client becomes inactive in a room with playerTTL &lt; 0. Defaults to true.</param>
        public static bool LeaveRoom(bool becomeInactive = true)
        {
            if (OfflineMode)
            {
                offlineModeRoom = null;
                //SendMonoMessage(PhotonNetworkingMessage.OnLeftRoom);
                NetworkingClient.MatchMakingCallbackTargets.OnLeftRoom();
            }
            else
            {
                if (CurrentRoom == null)
                {
                    Debug.LogWarning("PhotonNetwork.CurrentRoom is null. You don't have to call LeaveRoom() when you're not in one. State: " + PhotonNetwork.NetworkClientState);
                }
                else
                {
                    becomeInactive = becomeInactive && CurrentRoom.PlayerTtl != 0; // in a room with playerTTL == 0, the operation "leave" will never turn a client inactive
                }
                return NetworkingClient.OpLeaveRoom(becomeInactive);
            }

            return true;
        }



        /// <summary>
        /// Internally used helper-method to setup an offline room, the numbers for actor and master-client and to do the callbacks.
        /// </summary>
        private static void EnterOfflineRoom(string roomName, RoomOptions roomOptions, bool createdRoom)
        {
            offlineModeRoom = new Room(roomName, roomOptions, true);
            NetworkingClient.ChangeLocalID(1);
            offlineModeRoom.masterClientId = 1;
            offlineModeRoom.AddPlayer(PhotonNetwork.LocalPlayer);
            offlineModeRoom.LoadBalancingClient = PhotonNetwork.NetworkingClient;


            if (createdRoom)
            {
                NetworkingClient.MatchMakingCallbackTargets.OnCreatedRoom();
            }

            NetworkingClient.MatchMakingCallbackTargets.OnJoinedRoom();
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
        /// https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby
        ///
        ///
        /// You can show your current players and room count without joining a lobby (but you must
        /// be on the master server). Use: CountOfPlayers, CountOfPlayersOnMaster, CountOfPlayersInRooms and
        /// CountOfRooms.
        ///
        /// You can use more than one lobby to keep the room lists shorter. See JoinLobby(TypedLobby lobby).
        /// When creating new rooms, they will be "attached" to the currently used lobby or the default lobby.
        ///
        /// You can use JoinRandomRoom without being in a lobby!
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
        /// https://doc.photonengine.com/en-us/realtime/current/reference/matchmaking-and-lobby
        ///
        ///
        /// Per room you should check if it's full or not before joining. Photon does list rooms that are
        /// full, unless you close and hide them (room.open = false and room.visible = false).
        ///
        /// You can show your games current players and room count without joining a lobby (but you must
        /// be on the master server). Use: CountOfPlayers, CountOfPlayersOnMaster, CountOfPlayersInRooms and
        /// CountOfRooms.
        ///
        /// When creating new rooms, they will be "attached" to the currently used lobby or the default lobby.
        ///
        /// You can use JoinRandomRoom without being in a lobby!
        /// </remarks>
        /// <param name="typedLobby">A typed lobby to join (must have name and type).</param>
        public static bool JoinLobby(TypedLobby typedLobby)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.Server == ServerConnection.MasterServer)
            {
                return NetworkingClient.OpJoinLobby(typedLobby);
            }

            return false;
        }

        /// <summary>Leave a lobby to stop getting updates about available rooms.</summary>
        /// <remarks>
        /// This does not reset PhotonNetwork.lobby! This allows you to join this particular lobby later
        /// easily.
        ///
        /// The values CountOfPlayers, CountOfPlayersOnMaster, CountOfPlayersInRooms and CountOfRooms
        /// are received even without being in a lobby.
        ///
        /// You can use JoinRandomRoom without being in a lobby.
        /// </remarks>
        public static bool LeaveLobby()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.Server == ServerConnection.MasterServer)
            {
                return NetworkingClient.OpLeaveLobby();
            }

            return false;
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
        /// Users identify themselves by setting a unique userId in the PhotonNetwork.AuthValues.
        /// See remarks of AuthenticationValues for info about how this is set and used.
        ///
        /// The list of friends must be fetched from some other source (not provided by Photon).
        ///
        ///
        /// Internal:
        /// The server response includes 2 arrays of info (each index matching a friend from the request):
        /// ParameterCode.FindFriendsResponseOnlineList = bool[] of online states
        /// ParameterCode.FindFriendsResponseRoomIdList = string[] of room names (empty string if not in a room)
        /// </remarks>
        /// <param name="friendsToFind">Array of friend (make sure to use unique NickName or AuthValues).</param>
        /// <returns>If the operation could be sent (requires connection, only one request is allowed at any time). Always false in offline mode.</returns>
        public static bool FindFriends(string[] friendsToFind)
        {
            if (NetworkingClient == null || offlineMode)
            {
                return false;
            }

            return NetworkingClient.OpFindFriends(friendsToFind);
        }

        /// <summary>Fetches a custom list of games from the server, matching a SQL-like "where" clause, then triggers OnRoomListUpdate callback.</summary>
        /// <remarks>
        /// Operation is only available for lobbies of type SqlLobby.
        /// This is an async request.
        ///
        /// Note: You don't have to join a lobby to query it. Rooms need to be "attached" to a lobby, which can be done
        /// via the typedLobby parameter in CreateRoom, JoinOrCreateRoom, etc..
        ///
        /// When done, OnRoomListUpdate gets called.
        /// </remarks>
        /// <see cref="https://doc.photonengine.com/en-us/pun/v2/lobby-and-matchmaking/matchmaking-and-lobby/#sql_lobby_type"/>
        /// <param name="typedLobby">The lobby to query. Has to be of type SqlLobby.</param>
        /// <param name="sqlLobbyFilter">The sql query statement.</param>
        /// <returns>If the operation could be sent (has to be connected).</returns>
        public static bool GetCustomRoomList(TypedLobby typedLobby, string sqlLobbyFilter)
        {
            return NetworkingClient.OpGetGameList(typedLobby, sqlLobbyFilter);
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
                foreach (object k in LocalPlayer.CustomProperties.Keys)
                {
                    customProperties[(string)k] = null;
                }
            }

            if (CurrentRoom != null)
            {
                LocalPlayer.SetCustomProperties(customProperties);
            }
            else
            {
                LocalPlayer.InternalCacheProperties(customProperties);
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
            // TODO: decide if this option makes sense

            if (customPropertiesToDelete == null || customPropertiesToDelete.Length == 0 || LocalPlayer.CustomProperties == null)
            {
                LocalPlayer.CustomProperties = new Hashtable();
                return;
            }

            // if a specific list of props should be deleted, we do that here
            for (int i = 0; i < customPropertiesToDelete.Length; i++)
            {
                string key = customPropertiesToDelete[i];
                if (LocalPlayer.CustomProperties.ContainsKey(key))
                {
                    LocalPlayer.CustomProperties.Remove(key);
                }
            }
        }

        /// <summary>
        /// Sends fully customizable events in a room. Events consist of at least an EventCode (0..199) and can have content.
        /// </summary>
        /// <remarks>
        /// To receive events, implement IOnEventCallback in any class and register it via PhotonNetwork.AddCallbackTarget.
        /// See <see cref="IOnEventCallback.OnEvent"/>.
        ///
        /// The eventContent is optional. If set, eventContent must be a "serializable type", something that
        /// the client can turn into a byte[] basically. Most basic types and arrays of them are supported, including
        /// Unity's Vector2, Vector3, Quaternion. Transforms are not supported.
        ///
        /// You can turn a class into a "serializable type" by following the example in CustomTypes.cs.
        ///
        /// The RaiseEventOptions have some (less intuitive) combination rules:
        /// If you set targetActors (an array of Player.ID values), the receivers parameter gets ignored.
        /// When using event caching, the targetActors, receivers and interestGroup can't be used. Buffered events go to all.
        /// When using cachingOption removeFromRoomCache, the eventCode and content are actually not sent but used as filter.
        /// </remarks>
        /// <param name="eventCode">A byte identifying the type of event. You might want to use a code per action or to signal which content can be expected. Allowed: 0..199.</param>
        /// <param name="eventContent">Some serializable object like string, byte, integer, float (etc) and arrays of those. Hashtables with byte keys are good to send variable content.</param>
        /// <param name="raiseEventOptions">Allows more complex usage of events. If null, RaiseEventOptions.Default will be used (which is fine).</param>
        /// <param name="sendOptions">Send options for reliable, encryption etc..</param>
        /// <returns>False if event could not be sent.</returns>
        public static bool RaiseEvent(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            if (offlineMode)
            {
                if (raiseEventOptions.Receivers == ReceiverGroup.Others)
                {
                    return true;
                }

                EventData evData = new EventData { Code = eventCode, Parameters = new Dictionary<byte, object> { {ParameterCode.Data, eventContent} } };
                NetworkingClient.OnEvent(evData);
                return true;
            }

            if (!InRoom || eventCode >= 200)
            {
                Debug.LogWarning("RaiseEvent(" + eventCode + ") failed. Your event is not being sent! Check if your are in a Room and the eventCode must be less than 200 (0..199).");
                return false;
            }

            return NetworkingClient.OpRaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        }

        /// <summary>Sends PUN-specific events to the server, unless in offlineMode.</summary>
        /// <param name="eventCode">A byte identifying the type of event.</param>
        /// <param name="eventContent">Serializable object or container.</param>
        /// <param name="raiseEventOptions">Allows more complex usage of events. If null, RaiseEventOptions.</param>
        /// <param name="sendOptions">Send options for reliable, encryption etc..</param>
        /// <returns>False if event could not be sent</returns>
        private static bool RaiseEventInternal(byte eventCode, object eventContent, RaiseEventOptions raiseEventOptions, SendOptions sendOptions)
        {
            if (offlineMode)
            {
                return false;
            }

            if (!InRoom)
            {
                Debug.LogWarning("RaiseEvent(" + eventCode + ") failed. Your event is not being sent! Check if your are in a Room");
                return false;
            }

            return NetworkingClient.OpRaiseEvent(eventCode, eventContent, raiseEventOptions, sendOptions);
        }


        /// <summary>
        /// Allocates a viewID for the current/local player.
        /// </summary>
        /// <returns></returns>
        public static bool AllocateViewID(PhotonView view)
        {
            if (view.ViewID != 0)
            {
                Debug.LogError("AllocateViewID() can't be used for PhotonViews that already have a viewID. This view is: " + view.ToString());
                return false;
            }

            int manualId = AllocateViewID(LocalPlayer.ActorNumber);
            view.ViewID = manualId;
            return true;
        }


        /// <summary>
        /// Enables the Master Client to allocate a viewID that is valid for scene objects.
        /// </summary>
        /// <returns></returns>
        public static bool AllocateSceneViewID(PhotonView view)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("Only the Master Client can AllocateSceneViewID(). Check PhotonNetwork.IsMasterClient!");
                return false;
            }

            if (view.ViewID != 0)
            {
                Debug.LogError("AllocateSceneViewID() can't be used for PhotonViews that already have a viewID. This view is: " + view.ToString());
                return false;
            }

            int manualId = AllocateViewID(0);
            view.ViewID = manualId;
            return true;
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
                    if (!photonViewList.ContainsKey(newViewId))
                    {
                        lastUsedViewSubIdStatic = newSubId;
                        return newViewId;
                    }
                }

                // this is the error case: we didn't find any (!) free subId for this user
                throw new Exception(string.Format("AllocateViewID() failed. The room (user {0}) is out of 'scene' viewIDs. It seems all available are in use.", ownerId));
            }
            else
            {
                // we look up a fresh SUBid for the owner
                int newSubId = lastUsedViewSubId;
                int newViewId;
                int ownerIdOffset = ownerId * MAX_VIEW_IDS;
                for (int i = 1; i <= MAX_VIEW_IDS; i++)
                {
                    newSubId = (newSubId + 1) % MAX_VIEW_IDS;
                    if (newSubId == 0)
                    {
                        continue;   // avoid using subID 0
                    }

                    newViewId = newSubId + ownerIdOffset;
                    if (!photonViewList.ContainsKey(newViewId))
                    {
                        lastUsedViewSubId = newSubId;
                        return newViewId;
                    }
                }

                throw new Exception(string.Format("AllocateViewID() failed. User {0} is out of viewIDs. It seems all available are in use.", ownerId));
            }
        }


        public static GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            Pun.InstantiateParameters netParams = new InstantiateParameters(prefabName, position, rotation, group, data, currentLevelPrefix, null, LocalPlayer, ServerTimestamp);
            return NetworkInstantiate(netParams, false);
        }

        public static GameObject InstantiateSceneObject(string prefabName, Vector3 position, Quaternion rotation, byte group = 0, object[] data = null)
        {
            if (LocalPlayer.IsMasterClient)
            {
                Pun.InstantiateParameters netParams = new InstantiateParameters(prefabName, position, rotation, group, data, currentLevelPrefix, null, LocalPlayer, ServerTimestamp);
                return NetworkInstantiate(netParams, true);
            }

            return null;
        }

        private static GameObject NetworkInstantiate(Hashtable networkEvent, Player creator)
        {

            // some values always present:
            string prefabName = (string)networkEvent[(byte)0];
            int serverTime = (int)networkEvent[(byte)6];
            int instantiationId = (int)networkEvent[(byte)7];

            Vector3 position;
            if (networkEvent.ContainsKey((byte)1))
            {
                position = (Vector3)networkEvent[(byte)1];
            }
            else
            {
                position = Vector3.zero;
            }

            Quaternion rotation = Quaternion.identity;
            if (networkEvent.ContainsKey((byte)2))
            {
                rotation = (Quaternion)networkEvent[(byte)2];
            }

            byte group = 0;
            if (networkEvent.ContainsKey((byte)3))
            {
                group = (byte)networkEvent[(byte)3];
            }

            byte objLevelPrefix = 0;
            if (networkEvent.ContainsKey((byte)8))
            {
                objLevelPrefix = (byte)networkEvent[(byte)8];
            }

            int[] viewsIDs;
            if (networkEvent.ContainsKey((byte)4))
            {
                viewsIDs = (int[])networkEvent[(byte)4];
            }
            else
            {
                viewsIDs = new int[1] { instantiationId };
            }

            object[] incomingInstantiationData;
            if (networkEvent.ContainsKey((byte)5))
            {
                incomingInstantiationData = (object[])networkEvent[(byte)5];
            }
            else
            {
                incomingInstantiationData = null;
            }

            // SetReceiving filtering
            if (group != 0 && !allowedReceivingGroups.Contains(group))
            {
                return null; // Ignore group
            }


            Pun.InstantiateParameters netParams = new InstantiateParameters(prefabName, position, rotation, group, incomingInstantiationData, objLevelPrefix, viewsIDs, creator, serverTime);
            return NetworkInstantiate(netParams, false, true);
        }

        
        private static readonly HashSet<string> PrefabsWithoutMagicCallback = new HashSet<string>();

        private static GameObject NetworkInstantiate(Pun.InstantiateParameters parameters, bool sceneObject = false, bool instantiateEvent = false)
        {
            //Instantiate(name, pos, rot)
            //pv[] GetPhotonViewsInChildren()
            //if (event==null) init send-params
            //Setup of PVs and callback
            //if (event == null) SendInstantiate(name, pos, rot, etc...)

            GameObject go = null;
            PhotonView[] photonViews;

            go = prefabPool.Instantiate(parameters.prefabName, parameters.position, parameters.rotation);
            

            if (go == null)
            {
                Debug.LogError("Failed to network-Instantiate: " + parameters.prefabName);
                return null;
            }

            if (go.activeSelf)
            {
                Debug.LogWarning("PrefabPool.Instantiate() should return an inactive GameObject. " + prefabPool.GetType().Name + " returned an active object. PrefabId: " + parameters.prefabName);
            }


            photonViews = go.GetPhotonViewsInChildren();


            if (photonViews.Length == 0)
            {
                Debug.LogError("PhotonNetwork.Instantiate() can only instantiate objects with a PhotonView component. This prefab does not have one: " + parameters.prefabName);
                return null;
            }

            bool localInstantiate = !instantiateEvent && LocalPlayer.Equals(parameters.creator);
            if (localInstantiate)
            {
                // init viewIDs array, so it can be filled (below), before it gets sent
                parameters.viewIDs = new int[photonViews.Length];
            }

            for (int i = 0; i < photonViews.Length; i++)
            {
                if (localInstantiate)
                {
                    // when this client instantiates a GO, it has to allocate viewIDs accordingly.
                    // SCENE objects are created as actorNumber 0 (no matter which number this player has).
                    parameters.viewIDs[i] = (sceneObject) ? AllocateViewID(0) : AllocateViewID(parameters.creator.ActorNumber);
                }

                photonViews[i].didAwake = false;
                photonViews[i].ViewID = 0;

                photonViews[i].Prefix = parameters.objLevelPrefix;
                photonViews[i].InstantiationId = parameters.viewIDs[0];   
                photonViews[i].isRuntimeInstantiated = true;
                photonViews[i].InstantiationData = parameters.data;

                photonViews[i].didAwake = true;
                photonViews[i].ViewID = parameters.viewIDs[i];    // with didAwake true and viewID == 0, this will also register the view
            }

            if (localInstantiate)
            {
                // send instantiate network event
                SendInstantiate(parameters, sceneObject);
            }

            go.SetActive(true);

            // if IPunInstantiateMagicCallback is implemented on any script of the instantiated GO, let's call it directly:
            if (!PrefabsWithoutMagicCallback.Contains(parameters.prefabName))
            {
                var list = go.GetComponents<IPunInstantiateMagicCallback>();
                if (list.Length > 0)
                {
                    PhotonMessageInfo pmi = new PhotonMessageInfo(parameters.creator, parameters.timestamp, photonViews[0]);
                    foreach (IPunInstantiateMagicCallback callbackComponent in list)
                    {
                        callbackComponent.OnPhotonInstantiate(pmi);
                    }
                }
                else
                {
                    PrefabsWithoutMagicCallback.Add(parameters.prefabName);
                }
            }

            return go;
        }


        private static readonly Hashtable SendInstantiateEvHashtable = new Hashtable();                             // SendInstantiate reuses this to reduce GC
        private static readonly RaiseEventOptions SendInstantiateRaiseEventOptions = new RaiseEventOptions();       // SendInstantiate reuses this to reduce GC

        internal static bool SendInstantiate(Pun.InstantiateParameters parameters, bool sceneObject = false)
        {
            // first viewID is now also the gameobject's instantiateId
            int instantiateId = parameters.viewIDs[0];   // LIMITS PHOTONVIEWS&PLAYERS

            SendInstantiateEvHashtable.Clear();     // SendInstantiate reuses this Hashtable to reduce GC

            SendInstantiateEvHashtable[(byte)0] = parameters.prefabName;

            if (parameters.position != Vector3.zero)
            {
                SendInstantiateEvHashtable[(byte)1] = parameters.position;
            }

            if (parameters.rotation != Quaternion.identity)
            {
                SendInstantiateEvHashtable[(byte)2] = parameters.rotation;
            }

            if (parameters.group != 0)
            {
                SendInstantiateEvHashtable[(byte)3] = parameters.group;
            }

            // send the list of viewIDs only if there are more than one. else the instantiateId is the viewID
            if (parameters.viewIDs.Length > 1)
            {
                SendInstantiateEvHashtable[(byte)4] = parameters.viewIDs; // LIMITS PHOTONVIEWS&PLAYERS
            }

            if (parameters.data != null)
            {
                SendInstantiateEvHashtable[(byte)5] = parameters.data;
            }

            if (currentLevelPrefix > 0)
            {
                SendInstantiateEvHashtable[(byte)8] = currentLevelPrefix;    // photonview's / object's level prefix
            }

            SendInstantiateEvHashtable[(byte)6] = PhotonNetwork.ServerTimestamp;
            SendInstantiateEvHashtable[(byte)7] = instantiateId;


            SendInstantiateRaiseEventOptions.CachingOption = (sceneObject) ? EventCaching.AddToRoomCacheGlobal : EventCaching.AddToRoomCache;

            return PhotonNetwork.RaiseEventInternal(PunEvent.Instantiation, SendInstantiateEvHashtable, SendInstantiateRaiseEventOptions, new SendOptions() { Reliability = true });
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
                RemoveInstantiatedGO(targetView.gameObject, !InRoom);
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
            RemoveInstantiatedGO(targetGo, !InRoom);
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
        public static void DestroyPlayerObjects(Player targetPlayer)
        {
            if (LocalPlayer == null)
            {
                Debug.LogError("DestroyPlayerObjects() failed, cause parameter 'targetPlayer' was null.");
            }

            DestroyPlayerObjects(targetPlayer.ActorNumber);
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
            if (LocalPlayer.IsMasterClient || targetPlayerId == LocalPlayer.ActorNumber)
            {
                DestroyPlayerObjects(targetPlayerId, false);
            }
            else
            {
                Debug.LogError("DestroyPlayerObjects() failed, cause players can only destroy their own GameObjects. A Master Client can destroy anyone's. This is master: " + PhotonNetwork.IsMasterClient);
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
            if (IsMasterClient)
            {
                DestroyAll(false);
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
        /// - This client is the Master Client (can remove any Player's RPCs).
        ///
        /// If the targetPlayer calls RPCs at the same time that this is called,
        /// network lag will determine if those get buffered or cleared like the rest.
        /// </remarks>
        /// <param name="targetPlayer">This player's buffered RPCs get removed from server buffer.</param>
        public static void RemoveRPCs(Player targetPlayer)
        {
            if (!VerifyCanUseNetwork())
            {
                return;
            }

            if (!targetPlayer.IsLocal && !IsMasterClient)
            {
                Debug.LogError("Error; Only the MasterClient can call RemoveRPCs for other players.");
                return;
            }

            OpCleanActorRpcBuffer(targetPlayer.ActorNumber);
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

            CleanRpcBufferIfMine(targetPhotonView);
        }


        /// <summary>
        /// Internal to send an RPC on given PhotonView. Do not call this directly but use: PhotonView.RPC!
        /// </summary>
        internal static void RPC(PhotonView view, string methodName, RpcTarget target, bool encrypt, params object[] parameters)
        {
            if (!VerifyCanUseNetwork())
            {
                return;
            }

            if (CurrentRoom == null)
            {
                Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
                return;
            }

            if (NetworkingClient != null)
            {
                RPC(view, methodName, target, null, encrypt, parameters);
            }
            else
            {
                Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
            }
        }

        /// <summary>
        /// Internal to send an RPC on given PhotonView. Do not call this directly but use: PhotonView.RPC!
        /// </summary>
        internal static void RPC(PhotonView view, string methodName, Player targetPlayer, bool encrpyt, params object[] parameters)
        {
            if (!VerifyCanUseNetwork())
            {
                return;
            }

            if (CurrentRoom == null)
            {
                Debug.LogWarning("RPCs can only be sent in rooms. Call of \"" + methodName + "\" gets executed locally only, if at all.");
                return;
            }

            if (LocalPlayer == null)
            {
                Debug.LogError("RPC can't be sent to target Player being null! Did not send \"" + methodName + "\" call.");
            }

            if (NetworkingClient != null)
            {
                RPC(view, methodName, RpcTarget.Others, targetPlayer, encrpyt, parameters);
            }
            else
            {
                Debug.LogWarning("Could not execute RPC " + methodName + ". Possible scene loading in progress?");
            }
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
                if (targetComponents[index] != null)
                {
                    objectsWithComponent.Add(targetComponents[index].gameObject);
                }
            }

            return objectsWithComponent;
        }


        /// <summary>Enable/disable receiving events from a given Interest Group.</summary>
        /// <remarks>
        /// A client can tell the server which Interest Groups it's interested in.
        /// The server will only forward events for those Interest Groups to that client (saving bandwidth and performance).
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/gameplay/interestgroups
        ///
        /// See: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/package-demos/culling-demo
        /// </remarks>
        /// <param name="group">The interest group to affect.</param>
        /// <param name="enabled">Sets if receiving from group to enabled (or not).</param>
        public static void SetInterestGroups(byte group, bool enabled)
        {
            if (!VerifyCanUseNetwork())
            {
                return;
            }

            if (enabled)
            {
                byte[] groups = new byte[1] { (byte)group };
                SetInterestGroups(null, groups);
            }
            else
            {
                byte[] groups = new byte[1] { (byte)group };
                SetInterestGroups(groups, null);
            }
        }


        /// <summary>Wraps loading a level to pause the network message-queue. Optionally syncs the loaded level in a room.</summary>
        /// <remarks>
        /// To sync the loaded level in a room, set PhotonNetwork.AutomaticallySyncScene to true.
        /// The Master Client of a room will then sync the loaded level with every other player in the room.
        ///
        /// While loading levels, it makes sense to not dispatch messages received by other players.
        /// This method takes care of that by setting PhotonNetwork.IsMessageQueueRunning = false and enabling
        /// the queue when the level was loaded.
        ///
        /// You should make sure you don't fire RPCs before you load another scene (which doesn't contain
        /// the same GameObjects and PhotonViews). You can call this in OnJoinedRoom.
        ///
        /// This uses SceneManager.LoadSceneAsync().
        ///
        /// Check the progress of the LevelLoading using PhotonNetwork.LevelLoadingProgress (-1 means no loading, then it ranges from 0 to 1)
        /// </remarks>
        /// <param name='levelNumber'>
        /// Number of the level to load. When using level numbers, make sure they are identical on all clients.
        /// </param>
        public static void LoadLevel(int levelNumber)
        {
            if (PhotonNetwork.AutomaticallySyncScene)
            {
                SetLevelInPropsIfSynced(levelNumber);
            }

            PhotonNetwork.IsMessageQueueRunning = false;
            loadingLevelAndPausedNetwork = true;
            _AsyncLevelLoadingOperation = SceneManager.LoadSceneAsync(levelNumber,LoadSceneMode.Single);
        }

        /// <summary>Wraps loading a level to pause the network message-queue. Optionally syncs the loaded level in a room.</summary>
        /// <remarks>
        /// While loading levels, it makes sense to not dispatch messages received by other players.
        /// This method takes care of that by setting PhotonNetwork.IsMessageQueueRunning = false and enabling
        /// the queue when the level was loaded.
        ///
        /// To sync the loaded level in a room, set PhotonNetwork.AutomaticallySyncScene to true.
        /// The Master Client of a room will then sync the loaded level with every other player in the room.
        ///
        /// You should make sure you don't fire RPCs before you load another scene (which doesn't contain
        /// the same GameObjects and PhotonViews). You can call this in OnJoinedRoom.
        ///
        /// This uses SceneManager.LoadSceneAsync().
        ///
        /// Check the progress of the LevelLoading using PhotonNetwork.LevelLoadingProgress (-1 means no loading, then it ranges from 0 to 1)
        /// </remarks>
        /// <param name='levelName'>
        /// Name of the level to load. Make sure it's available to all clients in the same room.
        /// </param>
        public static void LoadLevel(string levelName)
        {
            if (PhotonNetwork.AutomaticallySyncScene)
            {
                SetLevelInPropsIfSynced(levelName);
            }

            PhotonNetwork.IsMessageQueueRunning = false;
            loadingLevelAndPausedNetwork = true;
            _AsyncLevelLoadingOperation = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        }

        /// <summary>
        /// This operation makes Photon call your custom web-service by name (path) with the given parameters.
        /// </summary>
        /// <remarks>
        /// This is a server-side feature which must be setup in the Photon Cloud Dashboard prior to use.
        /// <see cref="https://doc.photonengine.com/en-us/pun/v2/gameplay/web-extensions/webrpc"/>
        /// The Parameters will be converted into JSon format, so make sure your parameters are compatible.
        ///
        /// See <see cref="Photon.Realtime.IWebRpcCallback.OnWebRpcResponse"/> on how to get a response.
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
            return NetworkingClient.OpWebRpc(name, parameters);
        }

        /// <summary>
        /// Applies default log settings if they are not set up programmatically.
        /// </summary>
        private static void SetupLogging()
        {
            // only apply Settings if LogLevel is default ( see ServerSettings.cs), else it means it's been set programmatically
            if (PhotonNetwork.LogLevel == PunLogLevel.ErrorsOnly)
            {
                PhotonNetwork.LogLevel = PhotonServerSettings.PunLogging;
            }

            // only apply Settings if LogLevel is default ( see ServerSettings.cs), else it means it's been set programmatically
            if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugOut == DebugLevel.ERROR)
            {
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.DebugOut = PhotonServerSettings.AppSettings.NetworkLogging;
            }
        }


        #if UNITY_EDITOR

        /// <summary>
        /// Finds the asset path base on its name or search query: https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        /// </summary>
        /// <returns>The asset path.</returns>
        /// <param name="asset">Asset.</param>
        public static string FindAssetPath(string asset)
        {
            string[] guids = AssetDatabase.FindAssets (asset, null);
            if (guids.Length != 1)
            {
                return string.Empty;
            } else
            {
                return AssetDatabase.GUIDToAssetPath (guids [0]);
            }
        }


        /// <summary>
        /// Finds the pun asset folder. Something like Assets/Photon Unity Networking/Resources/
        /// </summary>
        /// <returns>The pun asset folder.</returns>
        public static string FindPunAssetFolder()
        {
            string _thisPath =	FindAssetPath("PhotonClasses");
            string _PunFolderPath = string.Empty;

            //Debug.Log("FindPunAssetFolder "+_thisPath);
            string[] subdirectoryEntries = _thisPath.Split ('/');
            foreach (string dir in subdirectoryEntries)
            {
                if (!string.IsNullOrEmpty (dir))
                {
                    _PunFolderPath += dir +"/";

                    if (string.Equals (dir, "PhotonUnityNetworking"))
                    {
                        //	Debug.Log("_PunFolderPath "+_PunFolderPath);
                        return _PunFolderPath;
                    }
                }
            }

            //Debug.Log("_PunFolderPath fallback to default Assets/Photon Unity Networking/");

            return "Assets/Photon/PhotonUnityNetworking/";
        }




        [Conditional("UNITY_EDITOR")]
        public static void CreateSettings()
        {
            PhotonNetwork.PhotonServerSettings = (ServerSettings)Resources.Load(PhotonNetwork.ServerSettingsFileName, typeof(ServerSettings));
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
                string _PunResourcesPath = PhotonNetwork.FindPunAssetFolder();

                _PunResourcesPath += "Resources/";


                string serverSettingsAssetPath = _PunResourcesPath+ PhotonNetwork.ServerSettingsFileName + ".asset";
                string settingsPath = Path.GetDirectoryName(serverSettingsAssetPath);
                if (!Directory.Exists(settingsPath))
                {
                    Directory.CreateDirectory(settingsPath);
                    AssetDatabase.ImportAsset(settingsPath);
                }

                PhotonNetwork.PhotonServerSettings = (ServerSettings)ScriptableObject.CreateInstance("ServerSettings");
                if (PhotonNetwork.PhotonServerSettings != null)
                {
                    AssetDatabase.CreateAsset(PhotonNetwork.PhotonServerSettings, serverSettingsAssetPath);
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
        /// <remarks>This is done in this class, because the Editor assembly can't access PhotonHandler.</remarks>
        public static void InternalCleanPhotonMonoFromSceneIfStuck()
        {
            PhotonHandler[] photonHandlers = GameObject.FindObjectsOfType(typeof(PhotonHandler)) as PhotonHandler[];
            if (photonHandlers != null && photonHandlers.Length > 0)
            {
                Debug.Log("Cleaning up hidden PhotonHandler instances in scene. Please save the scene to fix the problem.");
                foreach (PhotonHandler photonHandler in photonHandlers)
                {
                    // Debug.Log("Removing Handler: " + photonHandler + " photonHandler.gameObject: " + photonHandler.gameObject);
                    if (photonHandler.gameObject != null && photonHandler.gameObject.name == "PhotonMono")
                    {
                        photonHandler.gameObject.hideFlags = 0;
                        GameObject.DestroyImmediate(photonHandler.gameObject);
                    }

                    Component.DestroyImmediate(photonHandler);
                }
            }
        }
        #endif
    }
}
