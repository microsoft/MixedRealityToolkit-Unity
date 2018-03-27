// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.InputHandlers;
using UnityEngine;

namespace MixedRealityToolkit.Examples.SpatialMapping
{
    /// <summary>
    /// Class to test TapToPlace interactions, such as placement of object events.
    /// </summary>
    public class TapToPlaceTest : MonoBehaviour, IPlacementHandler
    {
        public void OnPlacingStarted(PlacementEventData eventData)
        {
            Debug.LogFormat("OnPlacingStarted\r\nSource: {0}  SourceId: {1}  Object: {2}", eventData.InputSource, eventData.SourceId, eventData.ObjectBeingPlaced.name);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }

        public void OnPlacingCompleted(PlacementEventData eventData)
        {
            Debug.LogFormat("OnPlacingCompleted\r\nSource: {0}  SourceId: {1}  Object: {2}", eventData.InputSource, eventData.SourceId, eventData.ObjectBeingPlaced.name);
            eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
        }
    }
}