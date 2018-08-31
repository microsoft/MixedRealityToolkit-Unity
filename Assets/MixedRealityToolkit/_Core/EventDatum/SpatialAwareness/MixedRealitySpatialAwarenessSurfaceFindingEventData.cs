// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem
{
    /// <summary>
    /// Event data sent by the spatial awareness mesh subsystem
    /// </summary>
    public class MixedRealitySpatialAwarenessSurfaceFindingEventData : MixedRealitySpatialAwarenessBaseEventData
    {
        /// <summary>
        /// The surface description associated with <see cref="MixedRealitySpatialAwarenessBaseEventData.Id"/>.
        /// </summary>
        public IMixedRealitySpatialAwarenessPlanarSurfaceDescription SurfaceDescription { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessSurfaceFindingEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Intialize(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            MixedRealitySpatialAwarenessEventType eventType,
            uint surfaceId,
            IMixedRealitySpatialAwarenessPlanarSurfaceDescription description,
            GameObject surfaceObject)
        {
            base.Initialize(spatialAwarenessSystem, surfaceId, eventType, surfaceObject);
            SurfaceDescription = description;
        }
    }
}
