// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Implementation of this interface causes a script to receive notifications of Touch events from HandTrackingInputSources
    /// </summary>
    public interface IMixedRealityTouchHandler : IEventSystemHandler
    {
        /// <summary>
        /// When a Touch motion has occurred, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchStarted(HandTrackingInputEventData eventData);

        /// <summary>
        /// When a Touch motion ends, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchCompleted(HandTrackingInputEventData eventData);

        /// <summary>
        /// When a Touch motion is updated, this handler receives the event.
        /// </summary>
        /// <remarks>
        /// A Touch motion is defined as occurring within the bounds of an object (transitive).
        /// </remarks>
        /// <param name="eventData">Contains information about the HandTrackingInputSource.</param>
        void OnTouchUpdated(HandTrackingInputEventData eventData);
    }
}
