// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.EventDatum.SpatialAwarenessSystem;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Handlers
{
    public interface IMixedRealitySpatialAwarenessObservationHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Called when a spatial observer adds a new observation.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnObservationAdded(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when a spatial observer updates a previous observation.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<T> eventData);

        /// <summary>
        /// Called when a spatial observer removes a previous observation.
        /// </summary>
        /// <param name="eventData">Data describing the event.</param>
        void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<T> eventData);
    }
}
