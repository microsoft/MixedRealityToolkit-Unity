// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Describes placement of objects events.
    /// </summary>
    public class PlacementEventData : GenericBaseEventData
    {
        /// <summary>
        /// The game object that is being placed.
        /// </summary>
        public GameObject ObjectBeingPlaced { get; private set; }

        /// <inheritdoc />
        public PlacementEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        public void Initialize(IMixedRealityEventSource eventSource, GameObject objectBeingPlaced)
        {
            BaseInitialize(eventSource);
            ObjectBeingPlaced = objectBeingPlaced;
        }
    }
}
