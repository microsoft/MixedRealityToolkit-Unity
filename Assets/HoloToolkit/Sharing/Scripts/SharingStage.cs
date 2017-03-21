// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using HoloToolkit.Sharing.Utilities;
using HoloToolkit.Unity;

namespace HoloToolkit.Sharing
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
        /// Default username to use when joining a session.
        /// </summary>
        /// <remarks>User code should set the user name by setting the UserName property.</remarks>
        private const string DefaultUserName = "User ";

        /// <summary>
        /// Set whether this app should be a Primary or Secondary client.
        /// Primary: Connects directly to the Session Server, can create/join/leave sessions
        /// Secondary: Connects to a Primary client.  Cannot do any session management
        /// </summary>
        public ClientRole ClientRole = ClientRole.Primary;

        /// <summary>
        /// Address of the sharing server.
        /// </summary>
        [Tooltip("Address of the sharing server")]
        public string ServerAddress = "localhost";

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
        /// Pipes XTools console output to Unity's output window for debugging
        /// </summary>
        private ConsoleLogWriter logWriter;

        /// <summary>
        /// Root element of our data model
        /// </summary>
        public SyncRoot Root { get; private set; }

        /// <summary>
        /// Server sessions tracker.
        /// </summary>
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
        /// Enables Server Discovery on the network 
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

        [Tooltip("Show Detailed Information for server connections")]
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

        private NetworkConnectionAdapter networkConnectionAdapter;
        private NetworkConnection networkConnection;
        public NetworkConnection Connection
        {
            get
            {
                if (networkConnection == null && Manager != null)
                {
                    networkConnection = Manager.GetServerConnection();
                }
                return networkConnection;
            }
        }

        /// <summary>
        /// Returns true if connected to a Sharing Service server.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Manager != null && Connection != null)
                {
                    return Connection.IsConnected();
                }

                return false;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            AppInstanceUniqueId = Guid.NewGuid().ToString();
            logWriter = new ConsoleLogWriter();
            logWriter.ShowDetailedLogs = ShowDetailedLogs;

            if (AutoDiscoverServer)
            {
                AutoDiscoverInit();
            }
            else
            {
                Connect();
            }
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

            if (Manager != null)
            {
                // Force a disconnection so that we can stop and start Unity without connections hanging around
                Manager.GetPairedConnection().Disconnect();
                Manager.GetServerConnection().Disconnect();
            }

            // Release the Sharing resources
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

            if (networkConnection != null)
            {
                networkConnection.RemoveListener((byte)MessageID.StatusOnly, networkConnectionAdapter);
                networkConnection.Dispose();
                networkConnection = null;

                if (networkConnectionAdapter != null)
                {
                    networkConnectionAdapter.Dispose();
                    networkConnectionAdapter = null;
                }
            }

            if (Manager != null)
            {
                Manager.Dispose();
                Manager = null;
            }

            // Forces a garbage collection to try to clean up any additional reference to SWIG-wrapped objects
            GC.Collect();

            base.OnDestroy();
        }

        private void LateUpdate()
        {
            if (isTryingToFindServer)
            {
                AutoDiscoverUpdate();
            }

            if (Manager != null)
            {
                // Update the XToolsManager to processes any network messages that have arrived
                Manager.Update();
            }
        }

        private void Connect()
        {
            var config = new ClientConfig(ClientRole);
            config.SetIsAudioEndpoint(IsAudioEndpoint);
            config.SetLogWriter(logWriter);

            // Only set the server info is we are connecting on awake
            if (connectOnAwake)
            {
                config.SetServerAddress(ServerAddress);
                config.SetServerPort(ServerPort);
            }

            Manager = SharingManager.Create(config);

            //set up callbacks so that we know when we've connected successfully
            networkConnection = Manager.GetServerConnection();
            networkConnectionAdapter = new NetworkConnectionAdapter();
            networkConnectionAdapter.ConnectedCallback += NetworkConnectionAdapter_ConnectedCallback;
            networkConnection.AddListener((byte)MessageID.StatusOnly, networkConnectionAdapter);

            SyncStateListener = new SyncStateListener();
            Manager.RegisterSyncListener(SyncStateListener);

            Root = new SyncRoot(Manager.GetRootSyncObject());

            SessionsTracker = new ServerSessionsTracker(Manager.GetSessionManager());
            SessionUsersTracker = new SessionUsersTracker(SessionsTracker);

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

        private void NetworkConnectionAdapter_ConnectedCallback(NetworkConnection obj)
        {
            SendConnectedNotification();
        }

        private void SendConnectedNotification()
        {
            if (Manager.GetServerConnection().IsConnected())
            {
                //Send notification that we're connected 
                EventHandler connectedEvent = SharingManagerConnected;
                if (connectedEvent != null)
                {
                    connectedEvent(this, EventArgs.Empty);
                }
            }
            else
            {
                Log.Error(string.Format("Cannot connect to server {0}:{1}", ServerAddress, ServerPort.ToString()));
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

            //Start Finding Server 
            isTryingToFindServer = true;
        }

        private void AutoDiscoverUpdate()
        {
            //Searching Enabled-> Update DiscoveryClient to check results, Wait Interval then Ping network. 
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

        private void OnSystemDiscovered(DiscoveredSystem obj)
        {
            if (obj.GetRole() == SystemRole.SessionDiscoveryServerRole)
            {
                //Found a server. Stop pinging the network and connect 
                isTryingToFindServer = false;
                ServerAddress = obj.GetAddress();
                if (ShowDetailedLogs)
                {
                    Debug.Log("Server discovered at: " + ServerAddress);
                }
                Connect();
                if (ShowDetailedLogs)
                {
                    Debug.LogFormat("Connected to: {0}:{1}", ServerAddress, ServerPort.ToString());
                }
            }
        }

        public void ConnectToServer(string serverAddress, int port)
        {
            ServerAddress = serverAddress;
            ServerPort = port;
            ConnectToServer();
        }

        public void ConnectToServer()
        {
            Manager.SetServerConnectionInfo(ServerAddress, (uint)ServerPort);
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
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
                default:
                    if (ShowDetailedLogs)
                    {
                        Log.Info(logString);
                    }
                    break;
            }
        }
    }
}
