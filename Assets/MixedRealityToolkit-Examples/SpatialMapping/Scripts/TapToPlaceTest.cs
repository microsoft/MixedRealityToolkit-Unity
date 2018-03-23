// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.SpatialMapping;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.Examples.SpatialMapping
{
    /// <summary>
    /// Class to test TapToPlace interactions, such as placement of object events.
    /// </summary>
    public class TapToPlaceTest : MonoBehaviour, ITapToPlaceHandler
    {
        public void OnPlacingStarted(TapToPlaceEventData eventData)
        {
            Debug.LogFormat("Start placing object {0}", eventData.selectedObject.name);
        }

        public void OnPlacingCompleted(TapToPlaceEventData eventData)
        {
            Debug.LogFormat("Completed placing object {0}", eventData.selectedObject.name);
        }
    }
}