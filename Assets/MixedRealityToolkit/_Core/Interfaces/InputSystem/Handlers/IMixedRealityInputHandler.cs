// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers
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
        /// Input Pressed updates from Interactions, Keys, or any other simple input.
        /// <remarks>Includes the Pressed Amount if available.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputPressed(InputPressedEventData eventData);

        /// <summary>
        /// Input Position updates from Thumbsticks, Touchpads, or any other simple input with a position.
        /// </summary>
        /// <param name="eventData">InputDualAxisPositionEventData</param>
        void OnPositionInputChanged(InputEventData<Vector2> eventData);
    }
}