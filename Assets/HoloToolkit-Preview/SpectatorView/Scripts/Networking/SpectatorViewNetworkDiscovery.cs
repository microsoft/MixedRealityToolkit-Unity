// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// </summary>
    public class SpectatorViewNetworkDiscovery : NetworkDiscovery
    {
        /// <summary>
        /// Event that represents a new session has been found
        /// </summary>
        public delegate void HololensSessionFoundEvent();

        /// <summary>
        /// Discovery starts when the component starts
        /// </summary>
        [SerializeField]
        private bool autoStart = true;

        /// <summary>
        /// Is the discovery component stopping?
        /// </summary>
        [SerializeField]
        private bool isStopping;

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
        /// Called when the phone finds a hololens session with its marker code.
        /// </summary>
        public HololensSessionFoundEvent OnHololensSessionFound;

        /// <summary>
        /// Component that manages the main flow of spectator view
        /// </summary>
        [SerializeField]
        [Tooltip("Component that manages the main flow, events and the main contact point with UNET multilens")]
        private SpectatorView spectatorView;

        /// <summary>
        /// Discovery starts when the component starts
        /// </summary>
        public bool AutoStart
        {
            get { return autoStart; }
            set { autoStart = value; }
        }

        /// <summary>
        /// Is the discovery component stopping?
        /// </summary>
        public bool IsStopping
        {
            get { return isStopping; }
            set { isStopping = value; }
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
        /// Component that manages the main flow of spectator view
        /// </summary>
        public SpectatorView SpectatorView
        {
            get { return spectatorView; }
            set { spectatorView = value; }
        }

        private void Awake()
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
            // The client doesn't have to wait for the server to be started, but this works best if the component
            // waits for the remaining networking bits to have warmed up,
            // just give it a couple of seconds and then start it
            if (!spectatorView.IsHost)
            {
                Invoke("ManualStart", 3f);
            }
        }

        /// <summary>
        /// Starts the system in server or client mode depending on isHost
        /// </summary>
        public void ManualStart()
        {
            // If it isn't supposed to start listening/broadcasting exit the method
            if (!AutoStart)
            {
                return;
            }

            Initialize();
            if (!spectatorView.IsHost)
            {
                StartAsClient();
            }
            else
            {
                StartAsServer();
                if (MarkerDetectionHololens != null)
                {
                    markerDetectionHololens.OnMarkerDetected += OnMarkerDetected;
                }
            }
        }

        #region Host

        /// <summary>
        /// Called on the server when a marker is detected. This will update the broadcast data
        /// </summary>
        /// <param name="markerId">ID of the marker detected by the HoloLens</param>
        /// <param name="pos">World position of the marker</param>
        /// <param name="rot">Rotation of the marker (as seen from the HoloLens)</param>
        private void OnMarkerDetected(int markerId, Vector3 pos, Quaternion rot)
        {
            string newData = "|" + markerId + "|";
            StartCoroutine(ChangeBroadcastData(newData));
        }

        /// <summary>
        /// Runs in a routine since the stop and start of the broadcasting isn't synchronous
        /// </summary>
        /// <param name="newData">New broadcast message</param>
        private IEnumerator ChangeBroadcastData(string newData)
        {
            if (newData != broadcastData && !IsStopping)
            {
                IsStopping = true;
                StopBroadcast();
                yield return null; //Wait one frame for the broadcast to be stopped
                broadcastData = newData;
                Initialize();
                yield return null; //Wait one frame for the broadcast to initialize
                StartAsServer();
                IsStopping = false;
            }
        }

        #endregion Host

        #region Client

#if !WINDOWS_UWP
        /// <summary>
        /// This method gets called whenever it receives a broadcast. It'll then strip out the message of the
        /// broadcast and decide whether it should connect to the sender of the broadcast or not
        /// </summary>
        /// <param name="fromAddress">IP address that broadcasted the message</param>
        /// <param name="data">Broadcast message read</param>
        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            if (spectatorView.IsHost)
            {
                return;
            }

            //The data will be in the format of |number|garbage so we split it by | and get the first one.
            var parsedData = data.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (parsedData.Length == 0)
            {
                return;
            }

            int parsedMarkerId;
            if (!int.TryParse(parsedData[0], out parsedMarkerId))
            {
                return;
            }


            //We found a server which data was our last generated markerId. That means that server just scanned our maker.
            //Join it!
            if (parsedMarkerId == MarkerGeneration3D.MarkerId && !IsStopping &&
                !NetworkManager.singleton.IsClientConnected())
            {
                if (OnHololensSessionFound != null)
                {
                    OnHololensSessionFound();
                }
                IsStopping = true;
                StartCoroutine(StopBroadcastAndConnect(fromAddress));
            }
        }

        /// <summary>
        /// Stops the broadcast, waits for it to be fully stopped and then connects to the hololens
        /// </summary>
        /// <param name="address">IP address to connect to</param>
        private IEnumerator StopBroadcastAndConnect(string address)
        {
            IsStopping = true;
            StopBroadcast();
            yield return null; //Wait one frame for the broadcast to be stopped
            NetworkManager.singleton.networkAddress = address;
            NetworkManager.singleton.StartClient();
            yield return new WaitUntil(() => NetworkManager.singleton.IsClientConnected());
            IsStopping = false;
        }

#endif

        #endregion Client

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

            if (markerDetectionHololens == null)
            {
                errorsList.Add("MarkerDetectionHololens reference is null on SpectatorViewNetworkDiscovery.");
                dependenciesValid = false;
            }

            if (markerGeneration3D == null)
            {
                errorsList.Add("MarkerGeneration3D reference is null on SpectatorViewNetworkDiscovery.");
                dependenciesValid = false;
            }

            if (newDeviceDiscovery == null)
            {
                errorsList.Add("NewDeviceDiscovery reference is null on SpectatorViewNetworkDiscovery.");
                dependenciesValid = false;
            }

            if (spectatorView == null)
            {
                errorsList.Add("SpectatorView reference is null on SpectatorViewNetworkDiscovery.");
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
