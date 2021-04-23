// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
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
