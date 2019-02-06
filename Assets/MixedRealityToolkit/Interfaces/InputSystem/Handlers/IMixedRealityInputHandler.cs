// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers
{
    /// <summary>
    /// Interface to implement for simple generic input.
    /// </summary>
    public interface IMixedRealityInputHandler : IEventSystemHandler
    {
        /// <summary>
        /// Input Up updates from Interactions, Keys, or any other simple input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputUp(InputEventData eventData);

        /// <summary>
        /// Input Down updates from Interactions, Keys, or any other simple input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputDown(InputEventData eventData);

        /// <summary>
        /// Input Pressed updates from Interactions, Keys, buttons, triggers, or any other simple input.
        /// </summary>
        /// <param name="eventData"></param>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the pressed amount, if available.
        /// </remarks>
        [Obsolete("Use IMixedRealityInputHandler<float>.OnInputChanged(InputEventData<float> eventData)")]
        void OnInputPressed(InputEventData<float> eventData);

        /// <summary>
        /// Input Position updates from Thumbsticks, Touchpads, or any other dual axis input with a position.
        /// </summary>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input position.
        /// </remarks>
        /// <param name="eventData"></param>
        [Obsolete("Use IMixedRealityInputHandler<Vector2>.OnInputChanged(InputEventData<Vector2> eventData)")]
        void OnPositionInputChanged(InputEventData<Vector2> eventData);
    }

    /// <summary>
    /// Interface to implement for more complex generic input.
    /// </summary>
    /// <typeparam name="T">The type of input to listen for.</typeparam>
    /// <remarks>
    /// Valid input types:
    /// </remarks>
    public interface IMixedRealityInputHandler<T> : IEventSystemHandler
    {
        /// <summary>
        /// Raised input event updates from the type of input specified in the interface handler implementation.
        /// </summary>
        /// <remarks>
        /// The <see cref="InputEventData{T}.InputData"/> is the current input data.
        /// </remarks>
        void OnInputChanged(InputEventData<T> eventData);
    }
}