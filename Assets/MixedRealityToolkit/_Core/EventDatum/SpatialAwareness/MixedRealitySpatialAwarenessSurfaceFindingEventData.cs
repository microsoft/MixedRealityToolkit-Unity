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
        /// Axis aligned bounding box containing the surface.
        /// </summary>
        /// <remarks>For SurfaceDeleted events, the value will be null.</remarks>
        public Bounds BoundingBox { get; private set; }

        /// <summary>
        /// The normal of the surface.
        /// </summary>
        /// <remarks>For SurfaceDeleted events, the value will be Vector3.zero.</remarks>
        public Vector3 Normal { get; private set; }

        /// <summary>
        /// The semantic (ex: Floor) associated with the surface.
        /// </summary>
        public MixedRealitySpatialAwarenessSurfaceTypes SurfaceType { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessSurfaceFindingEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Intialize(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            MixedRealitySpatialAwarenessEventType eventType,
            uint surfaceId,
            Vector3 position,
            Bounds boundingBox,
            Vector3 normal,
            MixedRealitySpatialAwarenessSurfaceTypes surfaceType,
            GameObject surfaceObject)
        {
            base.Initialize(spatialAwarenessSystem, surfaceId, eventType, position, surfaceObject);
            BoundingBox = boundingBox;
            Normal = normal;
            SurfaceType = surfaceType;
        }
    }
}
