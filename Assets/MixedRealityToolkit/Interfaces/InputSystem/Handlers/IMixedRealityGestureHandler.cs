// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement for generic gesture input.
    /// </summary>
    public interface IMixedRealityGestureHandler : IEventSystemHandler
    {
        /// <summary>
        /// Gesture Started Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureStarted(InputEventData eventData);

        /// <summary>
        /// Gesture Updated Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureUpdated(InputEventData eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureCompleted(InputEventData eventData);

        /// <summary>
        /// Gesture Canceled Event.
        /// </summary>
        /// <param name="eventData"></param>
        void OnGestureCanceled(InputEventData eventData);
    }

    /// <summary>
    /// Interface to implement for generic gesture input.
    /// </summary>
    /// <typeparam name="T">The type of data you want to listen for.</typeparam>
    public interface IMixedRealityGestureHandler<T> : IMixedRealityGestureHandler
    {
        /// <summary>
        /// Gesture Updated Event.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microsoft.MixedReality.Toolkit.Input.InputEventData{T}.InputData"/> for the associated gesture data.
        /// </remarks>
        /// <param name="eventData"></param>
        void OnGestureUpdated(InputEventData<T> eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microsoft.MixedReality.Toolkit.Input.InputEventData{T}.InputData"/> for the associated gesture data.
        /// </remarks>
        /// <param name="eventData"></param>
        void OnGestureCompleted(InputEventData<T> eventData);
    }
}