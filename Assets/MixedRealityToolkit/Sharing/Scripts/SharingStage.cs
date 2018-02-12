// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Common;
using MixedRealityToolkit.Common.Extensions;
using MixedRealityToolkit.Sharing.Utilities;
using System;
using UnityEngine;

namespace MixedRealityToolkit.Sharing
{
    /// <summary>
    /// The SharingStage is in charge of managing the core networking layer for the application.
    /// </summary>
    public class SharingStage : Singleton<SharingStage>
    {
        /// <summary> 
        /// SharingManagerConnected event notifies when the sharing manager is created and connected.
        /// </summary> 
        public event EventHandler SharingManagerConnected;

        /// <summary> 
        /// SharingManagerDisconnected event notifies when the sharing manager is disconnected.
        /// </summary> 
        public event EventHandler SharingManagerDisconnected;

        /// <summary>
        /// Default username to use when joining a session.
        /// </summary>
        /// <remarks>Set the user name with the <see cref="UserName"/> property.</remarks>
        private const string DefaultUserName = "User ";

        /// <summary>
        /// Set whether this app should be a Primary or Secondary client.
        /// <para> Primary: Connects directly to the Session Server, can create/join/leave sessions.</para>
        /// <para> Secondary: Connects to a Primary client.  Cannot do any session management.</para>
        /// </summary>
        public ClientRole ClientRole = ClientRole.Primary;

        /// <summary>
        /// Address of the sharing server.
        /// </summary>
        [Tooltip("Address of the sharing server")]
        public string ServerAddress = "localhost";

        /// <summary>
        /// The current session.
        /// </summary>
        public string SessionName
        {
            get
            {
                return Manager.GetSessionManager() != null && Manager.GetSessionManager().GetCurrentSession() != null
                    ? Manager.GetSessionManager().GetCurrentSession().GetName().GetString()
                    : defaultSessionName;
            }
        }

        [SerializeField]
        [Tooltip("Name of the session to join.")]
        private string defaultSessionName = "DefaultSession";

        /// <summary>
        /// The name of the current room.
        /// </summary>
        public string RoomName
        {
            get
            {
                return CurrentRoomManager != null && CurrentRoomManager.GetCurrentRoom() != null
                    ? CurrentRoomManager.GetCurrentRoom().GetName().GetString()
                    : defaultRoomName;
            }
        }

        [SerializeField]
        [Tooltip("Name of the room to join.")]
        private string defaultRoomName = "DefaultRoom";

        /// <summary>
        /// Indicates if the room should kept around even after all users leave.
        /// </summary>
        public bool KeepRoomAlive;

        /// <summary>
        /// Port of the sharing server.
        /// </summary>
        [Tooltip("Port of the sharing server")]
        public int ServerPort = 20602;

        [Tooltip("Should the app connect to the server at startup")]
        [SerializeField]
        private bool connectOnAwake = true;

        /// <summary>
        /// Sharing manager used by the application.
        /// </summary>
        public SharingManager Manager { get; private set; }

        public bool AutoDiscoverServer;

        [Tooltip("Determines how often the discovery service should ping the network in search of a server.")]
        public float PingIntervalSec = 2;

        /// <summary>
        /// Set whether this app should provide audio input / output features.
        /// </summary>
        public bool IsAudioEndpoint = true;

        /// <summary>
        /// Pipes sharing server console output to Unity's output window for debugging.
        /// </summary>
        private ConsoleLogWriter logWriter;

        /// <summary>
        /// Root element of our data model
        /// </summary>
        public SyncRoot Root { get; private set; }

        /// <summary>
        /// Server sessions tracker.
        /// </summary>
        /// <remarks>Note that if this processes takes the role of a secondary client,
        ///  then the sessionsTracker will always be null.</remarks>
        public ServerSessionsTracker SessionsTracker { get; private set; }

        /// <summary>
        /// Current session users tracker.
        /// </summary>
        public SessionUsersTracker SessionUsersTracker { get; private set; }

        /// <summary>
        /// Sync State Listener sending events indicating the current sharing sync state.
        /// </summary>
        public SyncStateListener SyncStateListener { get; private set; }

        /// <summary>
        /// Unique ID used to uniquely identify anything spawned by this application's instance,
        /// in order to prevent conflicts when spawning objects.
        /// </summary>
        public string AppInstanceUniqueId { get; private set; }

        /// <summary>
        /// Invoked when the local user changes their user name.
        /// </summary>
        public event Action<string> UserNameChanged;

        /// <summary> 
        /// Enables Server Discovery on the network.
        /// </summary> 
        private DiscoveryClient discoveryClient;

        /// <summary> 
        /// Provides callbacks when server is discovered or lost. 
        /// </summary> 
        private DiscoveryClientAdapter discoveryClientAdapter;

        /// <summary>
        /// The current ping interval during AutoDiscovery updates.
        /// </summary>
        private float pingIntervalCurrent;

        /// <summary>
        /// True when AutoDiscovery is actively searching, otherwise false.
        /// </summary>
        private bool isTryingToFindServer;

        /// <summary>
        /// Show Detailed Information for sharing services.
        /// </summary>
        [Tooltip("Show Detailed Information for sharing services.")]
        public bool ShowDetailedLogs;

        public string UserName
        {
            get
            {
                using (User user = Manager.GetLocalUser())
                {
                    using (XString userName = user.GetName())
                    {
                        return userName.GetString();
                    }
                }
            }
            set
            {
                using (var userName = new XString(value))
                {
                    Manager.SetUserName(userName);
                }

                UserNameChanged.RaiseEvent(value);
            }
        }

        /// <summary>
        /// Provides updates when rooms change.
        /// </summary>
        public RoomManagerAdapter RoomManagerAdapter;

        public RoomManager CurrentRoomManager { get { return Manager != null ? Manager.GetRoomManager() : null; } }

        public Room CurrentRoom
        {
            get { return CurrentRoomManager != null ? CurrentRoomManager.GetCurrentRoom() : null; }
        }

        private NetworkConnectionAdapter networkConnectionAdapter;

        public NetworkConnection Connection
        {
            get { return Manager != null ? Manager.GetServerConnection() : null; }
        }

        /// <summary>
        /// Returns true if connected to a Sharing Service server.
        /// </summary>
        public bool IsConnected
        {
            get { return Manager != null && Connection != null && Connection.IsConnected(); }
        }

        protected override void InitializeInternal()
        {
            AppInstanceUniqueId = Guid.NewGuid().ToString();
            logWriter = new ConsoleLogWriter { ShowDetailedLogs = ShowDetailedLogs };

            if (AutoDiscoverServer)
            {
                AutoDiscoverInit();
            }
            else
            {
                ManagerInit(connectOnAwake);
            }
        }

        #region Unity Methods

        private void OnEnable()
        {
            Application.logMessageReceived += OnLogReceived;
        }

        private void LateUpdate()
        {
            if (isTryingToFindServer)
            {
                AutoDiscoverUpdate();
            }

            if (Manager != null)
            {
                // Update the Sharing Manager to processes any network messages that have arrived.
                Manager.Update();
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogReceived;
        }

        protected override void OnDestroy()
        {
            if (discoveryClient != null)
            {
                discoveryClient.RemoveListener(discoveryClientAdapter);
                discoveryClient.Dispose();
                discoveryClient = null;

                if (discoveryClientAdapter != null)
                {
                    discoveryClientAdapter.Dispose();
                    discoveryClientAdapter = null;
                }
            }

            if (SessionUsersTracker != null)
            {
                SessionUsersTracker.Dispose();
                SessionUsersTracker = null;
            }

            if (SessionsTracker != null)
            {
                SessionsTracker.Dispose();
                SessionsTracker = null;
            }

            if (Connection != null)
            {
                Connection.RemoveListener((byte)MessageID.StatusOnly, networkConnectionAdapter);
                Connection.Dispose();

                if (networkConnectionAdapter != null)
                {
                    networkConnectionAdapter.Dispose();
                    networkConnectionAdapter = null;
                }
            }

            if (Manager != null)
            {
                // Force a disconnection so that we can stop and start Unity without connections hanging around.
                Manager.GetPairedConnection().Disconnect();
                Manager.GetServerConnection().Disconnect();
                Manager.Dispose();
                Manager = null;
            }

            // Forces a garbage collection to try to clean up any additional reference to SWIG-wrapped objects.
            GC.Collect();

            base.OnDestroy();
        }

        #endregion // Unity Methods

        #region Event Callbacks

        private void OnNetworkConnectionChanged(NetworkConnection networkConnection)
        {
            if (IsConnected)
            {
                if (SharingManagerConnected != null)
                {
                    SharingManagerConnected(this, EventArgs.Empty);
                }
            }
            else
            {
                if (SharingManagerDisconnected != null)
                {
                    SharingManagerDisconnected(this, EventArgs.Empty);
                }
            }
        }

        private void OnSystemDiscovered(DiscoveredSystem system)
        {
            if (system.GetRole() != SystemRole.SessionDiscoveryServerRole) { return; }

            // Found a server. Stop pinging the network and connect.
            discoveryClientAdapter.DiscoveredEvent -= OnSystemDiscovered;
            isTryingToFindServer = false;
            ServerAddress = system.GetAddress();

            if (ShowDetailedLogs)
            {
                Debug.Log("Server discovered at: " + ServerAddress);
            }

            ManagerInit(true);

            if (ShowDetailedLogs)
            {
                Debug.LogFormat("Connected to: {0}:{1}", ServerAddress, ServerPort.ToString());
            }
        }

        private void OnLogReceived(string logString, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Log.Error(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Warning:
                    Log.Warning(string.Format("{0} \n {1}", logString, stackTrace));
                    break;

                case LogType.Log:
                    if (ShowDetailedLogs)
                    {
                        Log.Info(logString);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, "Invalid Message Type");
            }
        }

        #endregion // Event Callbacks

        private void ManagerInit(bool setConnection)
        {
            var config = new ClientConfig(ClientRole);
            config.SetIsAudioEndpoint(IsAudioEndpoint);
            config.SetLogWriter(logWriter);

            if (setConnection)
            {
                config.SetServerAddress(ServerAddress);
                config.SetServerPort(ServerPort);
            }

            Manager = SharingManager.Create(config);

            // Set up callbacks so that we know when we've connected successfully.
            networkConnectionAdapter = new NetworkConnectionAdapter();
            networkConnectionAdapter.ConnectedCallback += OnNetworkConnectionChanged;
            networkConnectionAdapter.DisconnectedCallback += OnNetworkConnectionChanged;
            Connection.AddListener((byte)MessageID.StatusOnly, networkConnectionAdapter);

            SyncStateListener = new SyncStateListener();
            Manager.RegisterSyncListener(SyncStateListener);

            Root = new SyncRoot(Manager.GetRootSyncObject());

            SessionsTracker = new ServerSessionsTracker(Manager.GetSessionManager());
            SessionUsersTracker = new SessionUsersTracker(SessionsTracker);

            RoomManagerAdapter = new RoomManagerAdapter();

            CurrentRoomManager.AddListener(RoomManagerAdapter);

            using (var userName = new XString(DefaultUserName))
            {
#if UNITY_WSA && !UNITY_EDITOR
                Manager.SetUserName(SystemInfo.deviceName);
#else
                if (!string.IsNullOrEmpty(Environment.UserName))
                {
                    Manager.SetUserName(Environment.UserName);
                }
                else
                {
                    User localUser = Manager.GetLocalUser();
                    Manager.SetUserName(userName + localUser.GetID().ToString());
                }
#endif
            }
        }

        private void AutoDiscoverInit()
        {
            if (ShowDetailedLogs)
            {
                Debug.Log("Looking for servers...");
            }

            discoveryClientAdapter = new DiscoveryClientAdapter();
            discoveryClientAdapter.DiscoveredEvent += OnSystemDiscovered;

            discoveryClient = DiscoveryClient.Create();
            discoveryClient.AddListener(discoveryClientAdapter);

            // Start Finding Server.
            isTryingToFindServer = true;
        }

        private void AutoDiscoverUpdate()
        {
            // Searching Enabled-> Update DiscoveryClient to check results, Wait Interval then Ping network.
            pingIntervalCurrent += Time.deltaTime;
            if (pingIntervalCurrent > PingIntervalSec)
            {
                if (ShowDetailedLogs)
                {
                    Debug.Log("Looking for servers...");
                }

                pingIntervalCurrent = 0;
                discoveryClient.Ping();
            }

            discoveryClient.Update();
        }

        public void ConnectToServer(string serverAddress, int port)
        {
            ServerAddress = serverAddress;
            ServerPort = port;
            ConnectToServer();
        }

        public void ConnectToServer()
        {
            SessionsTracker.LeaveCurrentSession();
            Manager.SetServerConnectionInfo(ServerAddress, (uint)ServerPort);
        }
    }
}
