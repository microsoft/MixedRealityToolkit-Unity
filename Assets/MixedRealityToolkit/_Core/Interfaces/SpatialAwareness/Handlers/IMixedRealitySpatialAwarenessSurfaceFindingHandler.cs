// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers
{
    /// <summary>
    /// The event handler for all Spatial Awareness Surface Finding Events.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMixedRealitySpatialAwarenessSurfaceFindingHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Called when the spatial awareness surface finding subsystem adds a new planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceAdded(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem updates an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceUpdated(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem removes an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceRemoved(MixedRealitySpatialAwarenessEventData<T> eventData);
    }
}
