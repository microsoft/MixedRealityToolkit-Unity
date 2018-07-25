// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to navigation gestures.
    /// </summary>
    public interface IMixedRealityNavigationHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when the navigation gesture is started.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the normalized offset of the starting and ending position of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnNavigationStarted(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when the navigation gesture is updated.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the normalized offset of the starting and ending position of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnNavigationUpdated(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when the navigation gesture is completed.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the normalized offset of the starting and ending position of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnNavigationCompleted(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when the navigation gesture is canceled.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the normalized offset of the starting and ending position of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnNavigationCanceled(InputEventData<Vector3> eventData);
    }
}
