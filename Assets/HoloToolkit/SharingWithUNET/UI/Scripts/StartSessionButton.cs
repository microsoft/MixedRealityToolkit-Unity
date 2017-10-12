// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.SharingWithUNET
{
    /// <summary>
    /// Starts a session when the user taps the control this script is attached to.
    /// </summary>
    public class StartSessionButton : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// Script which controls hosting and discovering sessions.
        /// </summary>
        private NetworkDiscoveryWithAnchors networkDiscovery;

        // Use this for initialization
        private void Start()
        {
            networkDiscovery = NetworkDiscoveryWithAnchors.Instance;

            if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque && !Application.isEditor)
            {
                Debug.Log("Only hololens can host for now");
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Called when a click event is detected
        /// </summary>
        /// <param name="eventData">Information about the click.</param>
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (networkDiscovery.running)
            {
                // Only let hololens host
                // We are also allowing the editor to host for testing purposes, but shared anchors
                // will currently not work in this mode.

                if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque || Application.isEditor)
                {
                    if (Application.isEditor)
                    {
                        Debug.Log("Unity editor can host, but World Anchors will not be shared");
                    }

                    networkDiscovery.StartHosting("DefaultName");
                    eventData.Use();
                }
            }
        }
    }
}
