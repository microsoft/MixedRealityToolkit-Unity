// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using System;
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
        /// The time at which the event occurred.
        /// </summary>
        /// <remarks>
        /// The value will be in the device's configured time zone.
        /// </remarks>
        public DateTime EventTime { get; private set; }

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
            base.BaseInitialize(spatialAwarenessSystem);
            Id = id;
            EventTime = DateTime.Now;
            GameObject = gameObject;
        }
    }
}
