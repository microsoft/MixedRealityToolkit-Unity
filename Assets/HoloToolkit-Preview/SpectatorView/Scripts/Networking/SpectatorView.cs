// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    public class SpectatorView : MonoBehaviour
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
        /// Component that manages the networking system of the application
        /// </summary>
        [SerializeField]
        [Tooltip("Component that manages the networking system of the application")]
        private NetworkManager networkManager;

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
        /// Component that manages the networking system of the application
        /// </summary>
        public NetworkManager NetworkManager
        {
            get { return networkManager; }
            set { networkManager = value; }
        }

        /// <summary>
        /// Component that syncs up the world
        /// </summary>
        public WorldSync WorldSync
        {
            get { return worldSync; }
            set { worldSync = value; }
        }

        /// <summary>
        /// Is the device a host or a client? (HoloLens or mobile?)
        /// </summary>
        public bool IsHost
        {
            get {return isHost; }
            private set {isHost = value; }
        }

        private void Start()
        {
            string[] errors;
            if (!DependenciesValid(out errors))
            {
                PrintValidationErrors(errors);
                gameObject.SetActive(false);
            }

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

            // The host needs an aditional component
            if (isHost)
            {
                NetworkServer.RegisterHandler(MsgType.Connect,  OnServerConnect);
                StartCoroutine(StartHostRoutine());
            }
            else
            {
                WorldSync.OnWorldSyncCompleteClient += OnWorldSync;
                NetworkServer.RegisterHandler(MsgType.Connect,  OnClientConnect);
            }
        }

        /// <summary>
        /// A routine that starts the host, it requires a couple of frames to properly start the components
        /// </summary>
        private IEnumerator StartHostRoutine()
        {
            yield return new WaitUntil(() => NetworkManager.IsClientConnected());
            SpectatorViewNetworkDiscovery.ManualStart();
            yield return null;
            NewDeviceDiscovery.ManualStart();
        }

        /// <summary>
        /// Called on the server.
        /// A new client has connected, sync up the world
        /// </summary>
        /// <param name="conn">Newly created connection between the server and client</param>
        private void OnServerConnect(NetworkMessage conn)
        {
            WorldSync.StartSyncing();
        }

        /// <summary>
        /// Called on the client.
        /// A client has been connected to the server
        /// </summary>
        /// <param name="conn">Newly created connection between the server and client</param>
        private void OnClientConnect(NetworkMessage conn)
        {
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

        #region Dependencies validation
        private void OnValidate()
        {
            //Check if the object is in the scene. Otherwise it'll also check the prefabs and we don't want that
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            string[] errors = null;
            if (!DependenciesValid(out errors))
            {
                PrintValidationErrors(errors);
            }
        }

        /// <summary>
        /// Checks all the dependencies for the script.
        /// </summary>
        /// <param name="errors">Out variable that will hold an element for every error, if any</param>
        /// <returns>Whether all the dependencies exist and are correctly linked up</returns>
        private bool DependenciesValid(out string[] errors)
        {
            var dependenciesValid = true;
            var errorsList = new List<string>();
            if (spectatorViewNetworkDiscovery == null)
            {
                errorsList.Add("SpectatorViewNetworkDiscovery reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            if (markerDetectionHololens == null)
            {
                errorsList.Add("MarkerDetectionHololens reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            if (markerGeneration3D == null)
            {
                errorsList.Add("MarkerGeneration3D reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            if (newDeviceDiscovery == null)
            {
                errorsList.Add("NewDeviceDiscovery reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            if (networkManager == null)
            {
                errorsList.Add("NetworkManager reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            if (worldSync == null)
            {
                errorsList.Add("WorldSync reference is null on SpectatorView.");
                dependenciesValid = false;
            }

            errors = errorsList.ToArray();
            return dependenciesValid;
        }

        /// <summary>
        /// Prints to the console an error for each
        /// </summary>
        /// <param name="errors"></param>
        private void PrintValidationErrors(string[] errors)
        {
            for (var i = 0; i < errors.Length; i++)
            {
                Debug.LogError(errors[i]);
            }
        }
        #endregion Dependencies validation
    }
}
