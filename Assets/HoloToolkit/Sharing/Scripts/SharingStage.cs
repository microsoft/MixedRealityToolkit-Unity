// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity;
using UnityEngine;

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
        private const string DefaultUserName = "User0";

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
        public SharingManager Manager { get { return sharingMgr; } }
        private SharingManager sharingMgr;

        public bool AutoDiscoverServer = false;

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

        private float pingIntervalCurrent = 0; 
        private bool isTryingToFindServer = false; 

        public string UserName
        {
            get
            {
                using (User user = sharingMgr.GetLocalUser())
                {
                    using (XString userName = user.GetName())
                    {
                        return userName.GetString();
                    }
                }
            }
            set
            {
                using (XString userName = new XString(value))
                {
                    sharingMgr.SetUserName(userName);
                }

                UserNameChanged.RaiseEvent(value);
            }
        }

        private NetworkConnection connection;
        public NetworkConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = sharingMgr.GetServerConnection();
                }
                return connection;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            AppInstanceUniqueId = Guid.NewGuid().ToString();
            logWriter = new ConsoleLogWriter();

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
            base.OnDestroy();

            if (this.discoveryClient != null)
            {
                discoveryClient.RemoveListener(this.discoveryClientAdapter);
                discoveryClient.Dispose();
                discoveryClient = null;

                if (discoveryClientAdapter != null)
                {
                    discoveryClientAdapter.Dispose();
                    discoveryClientAdapter = null;
                }
            }

            if (sharingMgr != null)
            {
                // Force a disconnection so that we can stop and start Unity without connections hanging around
                sharingMgr.GetPairedConnection().Disconnect();
                sharingMgr.GetServerConnection().Disconnect();
            }

            // Release the Sharing resources
            if (this.SessionUsersTracker != null)
            {
                this.SessionUsersTracker.Dispose();
                this.SessionUsersTracker = null;
            }

            if (this.SessionsTracker != null)
            {
                this.SessionsTracker.Dispose();
                this.SessionsTracker = null;
            }

            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }

            if (sharingMgr != null)
            {
                sharingMgr.Dispose();
                sharingMgr = null;
            }

            // Forces a garbage collection to try to clean up any additional reference to SWIG-wrapped objects
            GC.Collect();
        }

        private void LateUpdate()
        {
            if (this.isTryingToFindServer)
            {
                AutoDiscoverUpdate();
            }

            if (sharingMgr != null)
            {
                // Update the XToolsManager to processes any network messages that have arrived
                sharingMgr.Update();
            }
        }

        private void Connect()
        {
            ClientConfig config = new ClientConfig(ClientRole);
            config.SetIsAudioEndpoint(IsAudioEndpoint);
            config.SetLogWriter(logWriter);

            // Only set the server info is we are connecting on awake
            if (connectOnAwake)
            {
                config.SetServerAddress(ServerAddress);
                config.SetServerPort(ServerPort);
            }

            sharingMgr = SharingManager.Create(config);

            SyncStateListener = new SyncStateListener();
            sharingMgr.RegisterSyncListener(SyncStateListener);

            Root = new SyncRoot(sharingMgr.GetRootSyncObject());

            this.SessionsTracker = new ServerSessionsTracker(sharingMgr.GetSessionManager());
            this.SessionUsersTracker = new SessionUsersTracker(this.SessionsTracker);

            using (XString userName = new XString(DefaultUserName))
            {
                sharingMgr.SetUserName(userName);
            }
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
                Log.Error(string.Format("Cannot connect to server {0}:{1}", ServerAddress, ServerPort)); 
            } 
        } 
 
        private void AutoDiscoverInit()
        {
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
                this.ServerAddress = obj.GetAddress(); 
                Debug.Log("System Discovered at: " + this.ServerAddress); 
                Connect(); 
                Debug.Log(string.Format("Connected to: {0}:{1}", this.ServerAddress, this.ServerPort)); 
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
            sharingMgr.SetServerConnectionInfo(ServerAddress, (uint)ServerPort);
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
                    Log.Info(logString);
                    break;
            }
        }
    }
}
