// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    public class InputPressedEventData : InputEventData
    {
        /// <summary>
        /// The amount, from 0.0 to 1.0, that the select was pressed.
        /// </summary>
        public double PressedAmount { get; private set; }

        public InputPressedEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, double pressedAmount, object[] tags = null)
        {
            Initialize(inputSource, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, double pressedAmount, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PressedAmount = pressedAmount;
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, double pressedAmount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, pressType, handedness, tags);
            PressedAmount = pressedAmount;
        }
#endif
    }
}