// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 649

namespace HoloToolkit.ARCapture
{
    /// <summary>
    ///
    /// </summary>
    public class ARCANetworkDiscovery : NetworkDiscovery
    {
        /// <summary>
        /// Event that represents a new session has been found
        /// </summary>
        public delegate void HololensSessionFoundEvent();

        /// <summary>
        /// Is the device a host or a client? (Hololens or mobile?)
        /// </summary>
        private bool isHost;

        /// <summary>
        /// Discovery starts when the component starts
        /// </summary>
        public bool AutoStart = true;

        /// <summary>
        /// Is the discovery component stopping?
        /// </summary>
        public bool IsStopping;

        [Tooltip("Component used to detect a AR marker from the HoloLens")]
        public MarkerDetectionHololens MarkerDetectionHololens;
        [Tooltip("Component that generates the AR codes")]
        public MarkerGeneration3D MarkerGeneration3D;
        [Tooltip("Component that manages the procedure of discovering new devices (mobile)")]
        public NewDeviceDiscovery NewDeviceDiscovery;

        /// <summary>
        /// Called when the phone finds a hololens session with its marker code.
        /// </summary>
        public HololensSessionFoundEvent OnHololensSessionFound;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        private void Awake()
        {
            isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            //The client doesn't have to wait for the server to be started. Just give it a couple of seconds and then start it
            if (!isHost)
            {
                Invoke("ManualStart", 3f);
            }
        }

        /// <summary>
        /// Starts the system in server or client mode depending on isHost
        /// </summary>
        public void ManualStart()
        {
            //Auto find components if necessary
            if (MarkerGeneration3D == null)
            {
                MarkerGeneration3D = FindObjectOfType<MarkerGeneration3D>();
            }
            if (MarkerDetectionHololens == null)
            {
                MarkerDetectionHololens = FindObjectOfType<MarkerDetectionHololens>();
            }
            if (NewDeviceDiscovery == null)
            {
                NewDeviceDiscovery = FindObjectOfType<NewDeviceDiscovery>();
            }

            //If it isn't supposed to start listening/broadcasting exit the method
            if (!AutoStart) return;

            Initialize();
            if (!isHost)
            {
                StartAsClient();
            }
            else
            {
                StartAsServer();
                if (MarkerDetectionHololens != null)
                {
                    MarkerDetectionHololens.OnMarkerDetected += OnMarkerDetected;
                }
            }
        }


        #region Host

        /// <summary>
        ///     Called on the server when a marker is detected. This will update the broadcast data
        /// </summary>
        /// <param name="markerId"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        private void OnMarkerDetected( int markerId, Vector3 pos, Quaternion rot )
        {
            var newData = "|" + markerId + "|";
            StartCoroutine(ChangeBroadcastData(newData));
        }

        /// <summary>
        ///     Runs in a routine since the stop and start of the broadcasting isn't synchronous
        /// </summary>
        /// <param name="newData"></param>
        /// <returns></returns>
        private IEnumerator ChangeBroadcastData( string newData )
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

        #endregion

        #region Client

#if !NETFX_CORE
        /// <summary>
        ///     This method gets called whenever it receives a broadcast. It'll then strip out the message of the
        ///     broadcast and decide whether it should connect to the sender of the broadcast or not
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="data"></param>
        public override void OnReceivedBroadcast( string fromAddress, string data )
        {
            if (isHost) return;

            //The data will be in the format of |number|garbage so we split it by | and get the first one.
            var parsedData = data.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (parsedData.Length == 0) return;

            int parsedMarkerId;
            int.TryParse(parsedData[0], out parsedMarkerId);

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
        ///     Stops the broadcast, waits for it to be fully stopped and then connects to the hololens
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private IEnumerator StopBroadcastAndConnect( string address )
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

        #endregion
    }
}
