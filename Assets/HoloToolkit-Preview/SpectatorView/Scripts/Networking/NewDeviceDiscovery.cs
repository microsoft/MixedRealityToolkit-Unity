// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloToolkit.Unity.Preview.SpectatorView
{
    /// <summary>
    /// In this instance the HoloLens will be listening to broadcasts from the mobile device to turn on the camera
    /// </summary>
    public class NewDeviceDiscovery : NetworkDiscovery
    {
        /// <summary>
        /// Component that manages the main flow of spectator view
        /// </summary>
        [SerializeField]
        [Tooltip("Component that manages the main flow, events and the main contact point with UNET multilens")]
        private SpectatorView spectatorView;

        /// <summary>
        /// Component used to detect a AR marker from the HoloLens
        /// </summary>
        [SerializeField]
        [Tooltip("Component used to detect a AR marker from the HoloLens")]
        private MarkerDetectionHololens markerDetectionHololens;

        /// <summary>
        /// Component used to detect a AR marker from the HoloLens
        /// </summary>
        public MarkerDetectionHololens MarkerDetectionHololens
        {
            get { return markerDetectionHololens; }
            set { markerDetectionHololens = value; }
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
                Invoke("ManualStart", 4f);
            }
        }

        /// <summary>
        /// Starts the system. In server mode or client mode depending on isHost
        /// </summary>
        public void ManualStart()
        {
            Initialize();

            // In this case the host will be listening for a phone to ping it in order to switch on the camera
            if (spectatorView.IsHost)
            {
                // As a host, we start listening for a ping to turn on the camera
                if (MarkerDetectionHololens == null)
                {
                    MarkerDetectionHololens = FindObjectOfType<MarkerDetectionHololens>();
                }
                StartAsClient();
            }
            else
            {
                StartAsServer();
            }
        }


        // When discovering new devices the host listens to broadcasts to tell it to start looking for devices

        #region Host

        /// <summary>
        /// Called on the HoloLens when receiving a broadcast from a phone. It'll keep alive the scanning
        /// </summary>
        /// <param name="fromAddress">IP address that broadcasted the message</param>
        /// <param name="data">Broadcast message read</param>
        public override void OnReceivedBroadcast( string fromAddress, string data )
        {
            base.OnReceivedBroadcast(fromAddress, data);
            MarkerDetectionHololens.StartCapture();
        }

        #endregion

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
                errorsList.Add("MarkerDetectionHololens reference is null on NewDeviceDiscovery.");
                dependenciesValid = false;
            }

            if (spectatorView == null)
            {
                errorsList.Add("SpectatorView reference is null on NewDeviceDiscovery.");
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
