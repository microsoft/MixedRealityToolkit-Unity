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
        /// The mesh description associated with <see cref="MixedRealitySpatialAwarenessBaseEventData.Id"/>.
        /// </summary>
        public IMixedRealitySpatialAwarenessMeshDescription MeshDescription { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public MixedRealitySpatialAwarenessMeshEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <inheritdoc />
        public void Intialize(
            IMixedRealitySpatialAwarenessSystem spatialAwarenessSystem,
            SpatialAwarenessEventType eventType,
            uint meshId,
            IMixedRealitySpatialAwarenessMeshDescription description,
            GameObject meshObject)
        {
            base.Initialize(spatialAwarenessSystem, meshId, eventType, meshObject);
            MeshDescription = description;
        }
    }
}
