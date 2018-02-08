// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.SharingWithUNET
{
    /// <summary>
    /// Triggers resetting the shared anchor when clicked.
    /// </summary>
    public class ResetAnchorButton : MonoBehaviour, IPointerHandler
    {
        public void OnPointerUp(ClickEventData eventData) { }

        public void OnPointerDown(ClickEventData eventData) { }

        /// <summary>
        /// When clicked we will reset the anchor if we are the server
        /// </summary>
        /// <param name="eventData">Information about the event</param>
        public void OnPointerClicked(ClickEventData eventData)
        {
            if (NetworkDiscoveryWithAnchors.Instance.isServer)
            {
#if UNITY_WSA
                UNetAnchorManager.Instance.MakeNewAnchor();
#endif
                eventData.Use();
            }
            else
            {
                Debug.Log("Only the server can reset the anchor at this time.");
            }
        }
    }
}