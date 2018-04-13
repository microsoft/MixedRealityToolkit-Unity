// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace MixedRealityToolkit.InputModule.EventData
{
    public enum InputPositionType
    {
        None,
        Touch,
        Thumbstick
    }

    public class InputPositionEventData : InputEventData
    {
        public InputPositionType InputPositionType { get; private set; }

        /// <summary>
        /// Two values, typically from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// Typically this is Touch or Joystick data.
        /// </summary>
        public Vector2 InputPosition { get; private set; }

        public InputPositionEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, InputPositionType inputType, Vector2 inputPosition, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            InputPositionType = inputType;
            InputPosition = inputPosition;
        }

        public void Initialize(IInputSource inputSource, InputPositionType inputType, Vector2 inputPosition, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            InputPositionType = inputType;
            InputPosition = inputPosition;
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, Vector2 inputPosition, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, pressType, handedness, tags);
            InputPosition = inputPosition;

            switch (pressType)
            {
                case InteractionSourcePressType.Thumbstick:
                    InputPositionType = InputPositionType.Thumbstick;
                    break;
                case InteractionSourcePressType.Touchpad:
                    InputPositionType = InputPositionType.Touch;
                    break;
            }
        }
#endif
    }
}
