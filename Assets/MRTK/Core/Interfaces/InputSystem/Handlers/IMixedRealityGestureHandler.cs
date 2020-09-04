// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Interface to implement for generic gesture input.
    /// </summary>
    public interface IMixedRealityGestureHandler : IMixedRealityBaseInputHandler
    {
        /// <summary>
        /// Gesture Started Event.
        /// </summary>
        void OnGestureStarted(InputEventData eventData);

        /// <summary>
        /// Gesture Updated Event.
        /// </summary>
        void OnGestureUpdated(InputEventData eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        void OnGestureCompleted(InputEventData eventData);

        /// <summary>
        /// Gesture Canceled Event.
        /// </summary>
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
        void OnGestureUpdated(InputEventData<T> eventData);

        /// <summary>
        /// Gesture Completed Event.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microsoft.MixedReality.Toolkit.Input.InputEventData{T}.InputData"/> for the associated gesture data.
        /// </remarks>
        void OnGestureCompleted(InputEventData<T> eventData);
    }
}