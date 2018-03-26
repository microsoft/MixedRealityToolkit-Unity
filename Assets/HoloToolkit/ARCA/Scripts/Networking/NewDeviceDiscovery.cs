// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable 649
namespace ARCA
{
    /// <summary>
    ///     In this instance the HoloLens will be listening to broadcasts from the mobile device to turn on the camera
    /// </summary>
    public class NewDeviceDiscovery : NetworkDiscovery
    {
        bool isHost;

        [Tooltip("Component used to detect a AR marker from the HoloLens")]
        public MarkerDetectionHololens MarkerDetectionHololens;

        void Awake()
        {
            isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
            //The client doesn't have to wait for the server to be started. Just give it a couple of seconds and then start it
            if (!isHost)
            {
                Invoke("ManualStart", 4f);
            }
        }

        /// <summary>
        ///     Starts the system. In server mode or client mode depending on isHost
        /// </summary>
        public void ManualStart()
        {
            Initialize();

            //In this case the host will be listening for a phone to ping it in order to switch on the camera
            if (isHost)
            {
                //As a host, we start listening for a ping to turn on the camera
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


        //When discovering new devices the host listens to broadcasts to tell it to start looking for devices

        #region Host

        /// <summary>
        ///     Called on the HoloLens when receiving a broadcast from a phone. It'll keep alive the scanning
        /// </summary>
        /// <param name="fromAddress"></param>
        /// <param name="data"></param>
        public override void OnReceivedBroadcast( string fromAddress, string data )
        {
            base.OnReceivedBroadcast(fromAddress, data);
            MarkerDetectionHololens.StartCapture();
        }

        #endregion
    }
}
