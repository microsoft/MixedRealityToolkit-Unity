// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement to react to manipulation gestures.
    /// </summary>
    public interface IMixedRealityManipulationHandler : IEventSystemHandler
    {
        /// <summary>
        /// Raised when a manipulation gesture is started.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the cumulative delta position from the last update of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnManipulationStarted(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when a manipulation gesture is updated.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the cumulative delta position from the last update of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnManipulationUpdated(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when a manipulation gesture is completed.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the cumulative delta position from the last update of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnManipulationCompleted(InputEventData<Vector3> eventData);

        /// <summary>
        /// Raised when a manipulation gesture is canceled.
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the cumulative delta position from the last update of the gesture.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnManipulationCanceled(InputEventData<Vector3> eventData);
    }
}
