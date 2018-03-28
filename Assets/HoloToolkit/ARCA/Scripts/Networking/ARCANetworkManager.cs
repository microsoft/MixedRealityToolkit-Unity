// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 649
namespace HoloToolkit.ARCapture
{
    public class ARCANetworkManager : NetworkManager
    {
        /// <summary>
        /// Custom delegate for when a client connects
        /// </summary>
        public delegate void ClientConnectedCustomEvent();

        /// <summary>
        /// Is the device a host or a client? (Hololens or mobile?)
        /// </summary>
        private bool isHost;

        [Tooltip("Component used to manage the discovery of new devices")]
        public ARCANetworkDiscovery ARCANetworkDiscovery;

        [Tooltip("Component used to detect a AR marker from the HoloLens")]
        public MarkerDetectionHololens MarkerDetectionHololens;
        [Tooltip("Component that generates the AR codes")]
        public MarkerGeneration3D MarkerGeneration3D;
        [Tooltip("Component that manages the procedure of discovering new devices (mobile)")]
        public NewDeviceDiscovery NewDeviceDiscovery;

        /// <summary>
        /// Custom callback for when a client connects
        /// </summary>
        public ClientConnectedCustomEvent OnClientConnectedCustom;

        [Tooltip("Component that syncs up the world")]
        public WorldSync WorldSync;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Start()
        {
            isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            //Auto find components if necessary
            if (NewDeviceDiscovery == null)
            {
                NewDeviceDiscovery = FindObjectOfType<NewDeviceDiscovery>();
            }
            if (ARCANetworkDiscovery == null)
            {
                ARCANetworkDiscovery = FindObjectOfType<ARCANetworkDiscovery>();
            }
            //The host needs an aditional component
            if (isHost)
            {
                if (MarkerDetectionHololens == null)
                {
                    MarkerDetectionHololens = FindObjectOfType<MarkerDetectionHololens>();
                }
                NetworkServer.Reset(); //Reset the server to make sure that it starts clean
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
        ///     For the client, wait until the server has started and then start the discovery components
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            StartCoroutine(StartHostRoutine());
        }

        /// <summary>
        /// A routine that starts the host, it requires a couple of frames to property start the components
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartHostRoutine()
        {
            ARCANetworkDiscovery.ManualStart();
            yield return null;
            NewDeviceDiscovery.ManualStart();
        }

        /// <summary>
        /// Called on the server.
        /// A new client has connected, sync up the world
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerConnect( NetworkConnection conn )
        {
            base.OnServerConnect(conn);
            WorldSync.StartSyncing();
        }

        /// <summary>
        /// Called on the client.
        /// A client has been connected to the server
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientConnect( NetworkConnection conn )
        {
            base.OnClientConnect(conn);
            if (OnClientConnectedCustom != null)
            {
                OnClientConnectedCustom();
            }
        }

        /// <summary>
        /// Called on the mobile when the world is sync
        /// </summary>
        private void OnWorldSync()
        {
            //Tells the mobile to stop broadcasting which signal the HoloLens to stop the camera
            StartCoroutine(StopBroadcastRoutine());
        }

        /// <summary>
        /// Stops broadcasting, it needs to wait a frame for it to be property stopped
        /// </summary>
        /// <returns></returns>
        IEnumerator StopBroadcastRoutine()
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
