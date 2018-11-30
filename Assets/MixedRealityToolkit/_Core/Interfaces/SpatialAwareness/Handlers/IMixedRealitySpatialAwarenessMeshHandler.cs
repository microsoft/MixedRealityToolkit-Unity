// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers
{
    /// <summary>
    /// The event handler for all Spatial Awareness Mesh Events.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessMeshHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Called when the spatial awareness mesh subsystem adds a mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshAdded(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness mesh subsystem updates an existing mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshUpdated(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness mesh subsystem removes an existing mesh.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnMeshRemoved(MixedRealitySpatialAwarenessEventData<T> eventData);
    }
}
