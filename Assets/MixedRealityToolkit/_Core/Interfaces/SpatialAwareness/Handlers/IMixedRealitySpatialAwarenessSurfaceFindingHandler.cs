// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers
{
    public interface IMixedRealitySpatialAwarenessSurfaceFindingHandler : IEventSystemHandler
    {
        /// <summary>
        /// Called when the spatial awareness surface finding subsystem adds a new planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceAdded(MixedRealitySpatialAwarenessEventData eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem updates an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceUpdated(MixedRealitySpatialAwarenessEventData eventData);

        /// <summary>
        /// Called when the spatial awareness surface finding subsystem removes an existing planar surface.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnSurfaceRemoved(MixedRealitySpatialAwarenessEventData eventData);
    }
}
