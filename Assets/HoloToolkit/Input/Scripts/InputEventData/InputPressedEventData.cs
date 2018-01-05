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

        public void Initialize(IInputSource inputSource, uint sourceId, double pressedAmount, object[] tags = null)
        {
            Initialize(inputSource, sourceId, tags);
            PressedAmount = pressedAmount;
        }

        public void Initialize(IInputSource inputSource, uint sourceId, double pressAmount, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, sourceId, pressAmount, handedness, tags);
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, uint sourceId, double pressAmount, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, sourceId, pressAmount, pressType, handedness, tags);
        }
#endif
    }
}