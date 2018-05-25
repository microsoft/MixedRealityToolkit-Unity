// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    public class SpectatorViewNetworkManager : NetworkManager
    {
        /// <summary>
        /// Custom delegate for when a client connects
        /// </summary>
        public delegate void ClientConnectedCustomEvent();

        /// <summary>
        /// Component used to manage the discovery of new devices
        /// </summary>
        [SerializeField]
        [Tooltip("Component used to manage the discovery of new devices")]
        private SpectatorViewNetworkDiscovery spectatorViewNetworkDiscovery;

        /// <summary>
        /// Is the device a host or a client? (HoloLens or mobile?)
        /// </summary>
        private bool isHost;

        /// <summary>
        /// Component used to detect a AR marker from the HoloLens
        /// </summary>
        [SerializeField]
        [Tooltip("Component used to detect a AR marker from the HoloLens")]
        private MarkerDetectionHololens markerDetectionHololens;

        /// <summary>
        /// Component that generates the AR codes
        /// </summary>
        [SerializeField]
        [Tooltip("Component that generates the AR codes")]
        private MarkerGeneration3D markerGeneration3D;

        /// <summary>
        /// Component that manages the procedure of discovering new devices (mobile)
        /// </summary>
        [SerializeField]
        [Tooltip("Component that manages the procedure of discovering new devices (mobile)")]
        private NewDeviceDiscovery newDeviceDiscovery;

        /// <summary>
        /// Custom callback for when a client connects
        /// </summary>
        public ClientConnectedCustomEvent OnClientConnectedCustom;

        /// <summary>
        /// Component that syncs up the world
        /// </summary>
        [SerializeField]
        [Tooltip("Component that syncs up the world")]
        private WorldSync worldSync;

        /// <summary>
        /// Component used to manage the discovery of new devices
        /// </summary>
        public SpectatorViewNetworkDiscovery SpectatorViewNetworkDiscovery
        {
            get { return spectatorViewNetworkDiscovery; }
            set { spectatorViewNetworkDiscovery = value; }
        }

        /// <summary>
        /// Component used to detect a AR marker from the HoloLens
        /// </summary>
        public MarkerDetectionHololens MarkerDetectionHololens
        {
            get { return markerDetectionHololens; }
            set { markerDetectionHololens = value; }
        }

        /// <summary>
        /// Component that generates the AR codes
        /// </summary>
        public MarkerGeneration3D MarkerGeneration3D
        {
            get { return markerGeneration3D; }
            set { markerGeneration3D = value; }
        }

        /// <summary>
        /// Component that manages the procedure of discovering new devices (mobile)
        /// </summary>
        public NewDeviceDiscovery NewDeviceDiscovery
        {
            get { return newDeviceDiscovery; }
            set { newDeviceDiscovery = value; }
        }

        /// <summary>
        /// Component that syncs up the world
        /// </summary>
        public WorldSync WorldSync
        {
            get { return worldSync; }
            set { worldSync = value; }
        }

        private void Start()
        {
#if WINDOWS_UWP
            try
            {
                OpenCVUtils.CheckOpenCVWrapperHasLoaded();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                gameObject.SetActive(false);
                return;
            }
#endif
            isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            // Auto find components if necessary
            if (NewDeviceDiscovery == null)
            {
                NewDeviceDiscovery = FindObjectOfType<NewDeviceDiscovery>();
            }

            if (SpectatorViewNetworkDiscovery == null)
            {
                SpectatorViewNetworkDiscovery = FindObjectOfType<SpectatorViewNetworkDiscovery>();
            }
            // The host needs an aditional component
            if (isHost)
            {
                if (MarkerDetectionHololens == null)
                {
                    MarkerDetectionHololens = FindObjectOfType<MarkerDetectionHololens>();
                }
                NetworkServer.Reset(); // Reset the server to make sure that it starts clean
                StartHost();
            }
            else
            {
                if (MarkerGeneration3D == null)
                {
                    MarkerGeneration3D = FindObjectOfType<MarkerGeneration3D>();
                }

                WorldSync.OnWorldSyncCompleteClient += OnWorldSync;
            }
        }

        /// <summary>
        /// For the client, wait until the server has started and then start the discovery components
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            StartCoroutine(StartHostRoutine());
        }

        /// <summary>
        /// A routine that starts the host, it requires a couple of frames to properly start the components
        /// </summary>
        private IEnumerator StartHostRoutine()
        {
            SpectatorViewNetworkDiscovery.ManualStart();
            yield return null;
            NewDeviceDiscovery.ManualStart();
        }

        /// <summary>
        /// Called on the server.
        /// A new client has connected, sync up the world
        /// </summary>
        /// <param name="conn">Newly created connection between the server and client</param>
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            WorldSync.StartSyncing();
        }

        /// <summary>
        /// Called on the client.
        /// A client has been connected to the server
        /// </summary>
        /// <param name="conn">Newly created connection between the server and client</param>
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            if (OnClientConnectedCustom != null)
            {
                OnClientConnectedCustom();
            }
        }

        /// <summary>
        /// Called on the mobile when the world is synchronized
        /// </summary>
        private void OnWorldSync()
        {
            // Tells the mobile to stop broadcasting which signal the HoloLens to stop the camera
            StartCoroutine(StopBroadcastRoutine());
        }

        /// <summary>
        /// Stops broadcasting, it needs to wait a frame for it to be properly stopped
        /// </summary>
        private IEnumerator StopBroadcastRoutine()
        {
            if (NewDeviceDiscovery == null)
            {
                NewDeviceDiscovery = FindObjectOfType<NewDeviceDiscovery>();
            }
            NewDeviceDiscovery.StopBroadcast();
            yield return null;
        }
    }
}
