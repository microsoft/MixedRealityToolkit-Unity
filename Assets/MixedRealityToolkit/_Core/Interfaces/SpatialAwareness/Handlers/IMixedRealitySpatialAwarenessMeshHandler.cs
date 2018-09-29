// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers
{
    public interface IMixedRealitySpatialAwarenessMeshHandler : IEventSystemHandler
    {
        /// <summary>
        /// Called when the spatial awareness mesh subsystem adds a mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshAdded(MixedRealitySpatialAwarenessEventData eventData);

        /// <summary>
        /// Called when the spatial awareness mesh subsystem updates an existing mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshUpdated(MixedRealitySpatialAwarenessEventData eventData);

        /// <summary>
        /// Called when the spatial awareness mesh subsystem removes an existing mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshRemoved(MixedRealitySpatialAwarenessEventData eventData);
    }
}
