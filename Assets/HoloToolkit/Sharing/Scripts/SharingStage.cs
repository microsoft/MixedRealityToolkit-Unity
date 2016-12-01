using System;
using HoloToolkit.Unity;
using UnityEngine;

namespace HoloToolkit.Sharing
{
    public class SharingStage : Singleton<SharingStage>
    {
        /// <summary>
        /// SharingManagerConnected event notifies when the sharing manager is created and connected.
        /// </summary>
        public event EventHandler SharingManagerConnected;

        /// <summary>
        /// Set whether this app should be a Primary or Secondary client.
        /// Primary: Connects directly to the Session Server, can create/join/leave sessions
        /// Secondary: Connects to a Primary client.  Cannot do any session management
        /// </summary>
        public ClientRole ClientRole = ClientRole.Primary;

        public string ServerAddress = "localhost";
        public int ServerPort = 20602;

        private SharingManager sharingMgr;
        public SharingManager Manager { get { return sharingMgr; } }

        /// <summary>
        /// Set whether this app should provide audio input / output features.
        /// </summary>
        public bool IsAudioEndpoint = true;

        public bool AutoDiscoverServer;

        [Tooltip("Determines how often the discovery service should ping the network in search of a server.")]
        public float PingIntervalSec = 2;

        /// <summary>
        /// Pipes XTools console output to Unity's output window for debugging
        /// </summary>
        private ConsoleLogWriter logWriter;

        /// <summary>
        /// Enables Server Discovery on the network
        /// </summary>
        private DiscoveryClient discoveryClient;
        /// <summary>
        /// Provides callbacks when server is discovered or lost.
        /// </summary>
        private DiscoveryClientAdapter discoveryClientAdapter;

        /// <summary>
        /// Provides callbacks for when we connect to a server.
        /// </summary>
        private NetworkConnectionAdapter networkConnectionAdapter;

        private NetworkConnection networkConnection;
    
        private float pingIntervalCurrent;
        private bool isTryingToFindServer;

        protected override void Awake()
        {
            base.Awake();

            logWriter = new ConsoleLogWriter();

            if (AutoDiscoverServer)
            {
                AutoDiscoverInit();
            }

            networkConnectionAdapter = new NetworkConnectionAdapter();
        }

        private void Start()
        {
            if (!AutoDiscoverServer)
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

            if (sharingMgr != null)
            {
                // Force a disconnection so that we can stop and start Unity without connections hanging around
                sharingMgr.GetPairedConnection().Disconnect();
                sharingMgr.GetServerConnection().Disconnect();

                // Release the XTools manager so that it cleans up the C++ copy
                sharingMgr.Dispose();
                sharingMgr = null;
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
            if (sharingMgr != null)
            {
                // Update the XToolsManager to processes any network messages that have arrived
                sharingMgr.Update();
            }
        }

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        private void Connect()
        {
            ClientConfig config = new ClientConfig(ClientRole);
            config.SetIsAudioEndpoint(IsAudioEndpoint);
            config.SetLogWriter(logWriter);
            config.SetServerAddress(ServerAddress);
            config.SetServerPort(ServerPort);
            config.SetProfilerEnabled(false);

            sharingMgr = SharingManager.Create(config);

            //set up callbacks so that we know when we've connected successfully
            networkConnection = sharingMgr.GetServerConnection();
            networkConnectionAdapter = new NetworkConnectionAdapter();
            networkConnectionAdapter.ConnectedCallback += NetworkConnectionAdapter_ConnectedCallback;
            networkConnection.AddListener((byte)MessageID.StatusOnly, networkConnectionAdapter);
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
                ServerAddress = obj.GetAddress();
                Debug.Log("System Discovered at: " + ServerAddress);
                Connect();
                Debug.Log(string.Format("Connected to: {0}:{1}", ServerAddress, ServerPort));
            }
        }

        void HandleLog(string logString, string stackTrace, LogType type)
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
