// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Microsoft.MixedReality.Toolkit.Internal.EventDatas.Input
{
    public class InputPressedEventData : InputEventData
    {
        /// <summary>
        /// The amount, from 0.0 to 1.0, that the select was pressed.
        /// </summary>
        public double PressedAmount { get; private set; }

        public InputPressedEventData(UnityEngine.EventSystems.EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, double pressedAmount, object[] tags = null)
        {
            Initialize(inputSource, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, KeyCode keyCode, double pressedAmount, object[] tags = null)
        {
            Initialize(inputSource, keyCode, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, double pressedAmount, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, KeyCode keyCode, double pressedAmount, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, keyCode, handedness, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, double pressedAmount, InputType inputType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, inputType, handedness, tags);
            PressedAmount = pressedAmount;
        }
    }
}