// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem
{
    /// <summary>
    /// Data for spatial awareness events.
    /// </summary>
    public class MixedRealitySpatialAwarenessEventData : GenericBaseEventData
    {
        /// <summary>
        /// Identifier of the object associated with this event.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// <see cref="GameObject"/>, managed by the spatial awareness system, representing the data in this event.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <inheritdoc />
        public void Initialize(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            int id,
            GameObject gameObject)
        {
            BaseInitialize(spatialAwarenessSystem);
            Id = id;
            GameObject = gameObject;
        }
    }
}
