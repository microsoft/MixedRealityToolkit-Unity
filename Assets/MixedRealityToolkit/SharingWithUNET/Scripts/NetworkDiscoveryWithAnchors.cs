// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if !UNITY_EDITOR && UNITY_WSA
using Windows.Networking;
using Windows.Networking.Connectivity;
#endif

namespace MixedRealityToolkit.SharingWithUNET
{
    /// <summary>
    /// Inherits from UNet's NetworkDiscovery script. 
    /// Adds automatic anchor management on discovery.
    /// If the script detects that it should be the server then
    /// the script starts the anchor creation and export process.
    /// If the script detects that it should be a client then the 
    /// script kicks off the anchor ingestion process.
    /// </summary>
    public class NetworkDiscoveryWithAnchors : NetworkDiscovery
    {
        /// <summary>
        /// Enables the Singleton pattern for this script.
        /// </summary>
        private static NetworkDiscoveryWithAnchors _Instance;
        public static NetworkDiscoveryWithAnchors Instance
        {
            get
            {
                NetworkDiscoveryWithAnchors[] objects = FindObjectsOfType<NetworkDiscoveryWithAnchors>();
                if (objects.Length != 1)
                {
                    Debug.LogFormat("Expected exactly 1 {0} but found {1}", typeof(NetworkDiscoveryWithAnchors).ToString(), objects.Length);
                }
                else
                {
                    _Instance = objects[0];
                }
                return _Instance;
            }
        }

        /// <summary>
        /// Class to track discovered session information.
        /// </summary>
        public class SessionInfo
        {
            public string SessionName;
            public string SessionIp;
        }

        /// <summary>
        /// Tracks if we are currently connected to a session.
        /// </summary>
        public bool Connected
        {
            get
            {
                // We are connected if we are the server or if we aren't running discovery
                return (isServer || !running);
            }
        }

        /// <summary>
        /// Event raised when the list of sessions changes.
        /// </summary>
        public event EventHandler<EventArgs> SessionListChanged;

        /// <summary>
        /// Keeps track of current remote sessions.
        /// </summary>
        [HideInInspector]
        public Dictionary<string, SessionInfo> remoteSessions = new Dictionary<string, SessionInfo>();

        /// <summary>
        /// Event raised when connected or disconnected.
        /// </summary>
        public event EventHandler<EventArgs> ConnectionStatusChanged;

        /// <summary>
        /// Controls how often a broadcast should be sent to clients
        /// looking to join our session.
        /// </summary>
        public int BroadcastInterval = 1000;

        /// <summary>
        /// Keeps track of the IP address of the system that sent the 
        /// broadcast.  We will use this IP address to connect and 
        /// download anchor data.
        /// </summary>
        public string ServerIp { get; private set; }

        /// <summary>
        /// Keeps track of the local IP address.
        /// </summary>
        public string LocalIp { get; set; }

        /// <summary>
        /// Sanity checks that our scene has everything we need to proceed.
        /// </summary>
        /// <returns>true if we have what we need, false otherwise.</returns>
        private bool CheckComponents()
        {
#if !UNITY_EDITOR && UNITY_WSA
            if (GenericNetworkTransmitter.Instance == null)
            {
                Debug.Log("Need a UNetNetworkTransmitter in the scene for sending anchor data");
                return false;
            }
#endif
            if (NetworkManager.singleton == null)
            {
                Debug.Log("Need a NetworkManager in the scene");
                return false;
            }

            return true;
        }

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_WSA
            // Find our local IP
            foreach (HostName hostName in NetworkInformation.GetHostNames())
            {
                if (hostName.DisplayName.Split(".".ToCharArray()).Length == 4)
                {
                    Debug.Log("Local IP " + hostName.DisplayName);
                    LocalIp = hostName.DisplayName;
                    break;
                }
            }
#else
            LocalIp = "editor" + UnityEngine.Random.Range(0, 999999).ToString(); ;
#endif
        }

        private void Start()
        {
            // Initializes NetworkDiscovery.
            Initialize();

            if (!CheckComponents())
            {
                Debug.Log("Invalid configuration detected. Network Discovery disabled.");
                Destroy(this);
                return;
            }

            broadcastInterval = BroadcastInterval;
            // Add our computer name to the broadcast data for use in the session name.
            broadcastData = GetLocalComputerName() + '\0';

            // Start listening for broadcasts.
            StartAsClient();
        }

        /// <summary>
        /// Gets the local computer name if it can.
        /// </summary>
        /// <returns></returns>
        private string GetLocalComputerName()
        {
#if !UNITY_EDITOR && UNITY_WSA
            foreach (HostName hostName in NetworkInformation.GetHostNames())
            {
                if (hostName.Type == HostNameType.DomainName)
                {

                    Debug.Log("My name is " + hostName.DisplayName);
                    return hostName.DisplayName;
                }
            }
            return "NotSureWhatMyNameIs";
#else
            return System.Environment.ExpandEnvironmentVariables("%ComputerName%");
#endif
        }

        /// <summary>
        /// If we haven't received a broadcast by the time this gets called
        /// we will start broadcasting and start creating an anchor.
        /// </summary>
        private void MaybeInitAsServer()
        {
            StartCoroutine(InitAsServer());
        }

        private IEnumerator InitAsServer()
        {
            Debug.Log("Acting as host");
#if !UNITY_EDITOR && UNITY_WSA
            NetworkManager.singleton.serverBindToIP = true;
            NetworkManager.singleton.serverBindAddress = LocalIp;
#endif

            // StopBroadcast will also 'StopListening'
            StopBroadcast();

            // Work-around when building to the HoloLens with "Compile with .NET Native tool chain".
            // Need a frame of delay after StopBroadcast() otherwise clients won't connect.
            yield return null;

            // Starting as a 'host' makes us both a client and a server.
            // There are nuances to this in UNet's sync system, so do make sure
            // to test behavior of your networked objects on both a host and a client 
            // device.
            NetworkManager.singleton.StartHost();

            // Work-around when building to the HoloLens with "Compile with .NET Native tool chain".
            // Need a frame of delay between StartHost() and StartAsServer() otherwise clients won't connect.
            yield return null;

            // Start broadcasting for other clients.
            StartAsServer();

#if !UNITY_EDITOR && UNITY_WSA
            // Start creating an anchor.
            UNetAnchorManager.Instance.CreateAnchor();
#else
            Debug.LogWarning("This script will need modification to work in the Unity Editor");
#endif
        }

        /// <summary>
        /// Called by UnityEngine when a broadcast is received. 
        /// </summary>
        /// <param name="fromAddress">When the broadcast came from</param>
        /// <param name="data">The data in the broad cast. Not currently used, but could
        /// be used for differentiating rooms or similar.</param>
        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            ServerIp = fromAddress.Substring(fromAddress.LastIndexOf(':') + 1);
            SessionInfo sessionInfo;
            if (remoteSessions.TryGetValue(ServerIp, out sessionInfo) == false)
            {
                Debug.Log("new session: " + fromAddress);
                Debug.Log(data);
                remoteSessions.Add(ServerIp, new SessionInfo() { SessionIp = ServerIp, SessionName = data });
                SignalSessionListEvent();
            }
        }


        /// <summary>
        /// Call to stop listening for sessions.
        /// </summary>
        public void StopListening()
        {
            StopBroadcast();
            remoteSessions.Clear();
        }

        /// <summary>
        /// Call to start listening for sessions.
        /// </summary>
        public void StartListening()
        {
            StopListening();
            StartAsClient();
        }

        /// <summary>
        /// Call to join a session
        /// </summary>
        /// <param name="session">Information about the session to join</param>
        public void JoinSession(SessionInfo session)
        {
            StopListening();
            // We have to parse the server IP to make the string friendly to the windows APIs.
            ServerIp = session.SessionIp;
            NetworkManager.singleton.networkAddress = ServerIp;
#if !UNITY_EDITOR && UNITY_WSA
            // Tell the network transmitter the IP to request anchor data from if needed.
            GenericNetworkTransmitter.Instance.SetServerIp(ServerIp);
#else
            Debug.LogWarning("This script will need modification to work in the Unity Editor");
#endif
            // And join the networked experience as a client.
            NetworkManager.singleton.StartClient();
            SignalConnectionStatusEvent();
        }

        /// <summary>
        /// Call to create a session 
        /// </summary>
        /// <param name="SessionName">The name of the session if a name can't be calculated</param>
        public void StartHosting(string SessionName)
        {
            StopListening();

#if !UNITY_EDITOR && UNITY_WSA
            NetworkManager.singleton.serverBindToIP = true;
            NetworkManager.singleton.serverBindAddress = LocalIp;
#endif
            // Starting as a 'host' makes us both a client and a server.
            // There are nuances to this in UNet's sync system, so do make sure
            // to test behavior of your networked objects on both a host and a client 
            // device.
            NetworkManager.singleton.StartHost();
            // Start broadcasting for other clients.
            StartAsServer();

#if !UNITY_EDITOR && UNITY_WSA
            // Invoke creating an anchor in a couple frames to give all the Unet network objects time to spawn.
            Invoke("InvokeCreateAnchor", 0.25f);
#else
            Debug.LogWarning("This script will need modification to work in the Unity Editor");
#endif

            SignalSessionListEvent();
            SignalConnectionStatusEvent();
        }

        /// <summary>
        /// The UNetAnchorManager won't be ready immediately after a scene is started so we defer calling 
        /// create anchor using unity's 'Invoke' on this function.
        /// </summary>
        void InvokeCreateAnchor()
        {
            UNetAnchorManager.Instance.CreateAnchor();
        }

        /// <summary>
        /// Called when sessions have been added or removed
        /// </summary>
        void SignalSessionListEvent()
        {
            EventHandler<EventArgs> sessionListChanged = SessionListChanged;
            if (sessionListChanged != null)
            {
                sessionListChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when we have joined or left a session.
        /// </summary>
        void SignalConnectionStatusEvent()
        {
            EventHandler<EventArgs> connectionEvent = this.ConnectionStatusChanged;
            if (connectionEvent != null)
            {
                connectionEvent(this, EventArgs.Empty);
            }
        }
    }
}