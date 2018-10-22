// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.SpatialAwarenessSystem.Handlers
{
    /// <summary>
    /// Class providing the default implementation of the <see cref="IMixedRealitySpatialAwarenessSurfaceFindingHandler"/> interface.
    /// </summary>
    public class MixedRealitySpatialAwarenessSurfaceFindingHandler : MonoBehaviour, IMixedRealitySpatialAwarenessSurfaceFindingHandler
    {
        /// <inheritdoc />
        public virtual void OnSurfaceAdded(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to access the plane data on arrival.
        }

        /// <inheritdoc />
        public virtual void OnSurfaceUpdated(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to access the plane data on update.
        }

        /// <inheritdoc />
        public virtual void OnSurfaceRemoved(MixedRealitySpatialAwarenessEventData eventData)
        {
            // Custom implementations can use this event to respond to plane removal.
        }
    }
}
