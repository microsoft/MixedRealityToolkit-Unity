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
    public class MixedRealitySpatialAwarenessMeshEventData : MixedRealitySpatialAwarenessBaseEventData
    {
        /// <summary>
        /// The mesh data associated with <see cref="MeshId"/>.
        /// </summary>
        /// <remarks>For MeshDeleted events, the value will be null.</remarks>
        public Mesh MeshData { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessMeshEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Intialize(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            MixedRealitySpatialAwarenessEventType eventType,
            uint meshId,
            Vector3 position,
            Mesh meshData,
            GameObject meshObject)
        {
            base.Initialize(spatialAwarenessSystem, meshId, eventType, position, meshObject);
            MeshData = meshData;
        }
    }
}
