// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.InputHandlers
{
    /// <summary>
    /// Interface to implement to react to simple generic input.
    /// </summary>
    public interface IInputHandler : IEventSystemHandler
    {
        /// <summary>
        /// Input Up updates from Buttons, Keys, or any other simple input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputUp(InputEventData eventData);

        /// <summary>
        /// Input Down updates from Buttons, Keys, or any other simple input.
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputDown(InputEventData eventData);

        /// <summary>
        /// Input Pressed updates from Buttons, Keys, or any other simple input.
        /// <remarks>Includes the Pressed Amount if available.</remarks>
        /// </summary>
        /// <param name="eventData"></param>
        void OnInputPressed(InputPressedEventData eventData);

        /// <summary>
        /// Input Position updates from Thumbsticks, Touchpads, or any other simple input with a position.
        /// </summary>
        /// <param name="eventData">InputPositionEventData</param>
        void OnInputPositionChanged(InputPositionEventData eventData);
    }
}