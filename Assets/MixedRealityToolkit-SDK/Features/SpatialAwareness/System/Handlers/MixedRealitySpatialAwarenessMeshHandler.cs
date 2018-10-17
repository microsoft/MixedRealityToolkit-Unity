// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem.Handlers
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessMeshHandler"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessMeshHandler : MonoBehaviour, IMixedRealitySpatialAwarenessMeshHandler
    {
        /// <inheritdoc />
        public void OnMeshAdded(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to access the mesh data on arrival.
        }

        /// <inheritdoc />
        public void OnMeshUpdated(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to access the mesh data on update.
        }

        /// <inheritdoc />
        public void OnMeshRemoved(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to respond to mesh removal.
        }
    }
}
