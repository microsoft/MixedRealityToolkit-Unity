// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an Input Event where a button, key, or trigger was pressed.
    /// </summary>
    public class InputPressedEventData : InputEventData
    {
        /// <summary>
        /// The amount, from 0.0 to 1.0, that the select was pressed.
        /// </summary>
        public double PressedAmount { get; private set; }

        /// <inheritdoc />
        public InputPressedEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, double pressedAmount)
        {
            Initialize(inputSource);
            PressedAmount = pressedAmount;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, double pressedAmount)
        {
            Initialize(inputSource, handedness);
            PressedAmount = pressedAmount;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, KeyCode keyCode, double pressedAmount)
        {
            Initialize(inputSource, keyCode);
            PressedAmount = pressedAmount;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputType inputType, double pressedAmount)
        {
            Initialize(inputSource, inputType);
            PressedAmount = pressedAmount;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="keyCode"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, KeyCode keyCode, double pressedAmount)
        {
            Initialize(inputSource, handedness, keyCode);
            PressedAmount = pressedAmount;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="inputType"></param>
        /// <param name="pressedAmount"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, InputType inputType, double pressedAmount)
        {
            Initialize(inputSource, handedness, inputType);
            PressedAmount = pressedAmount;
        }
    }
}