// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.InputSystem.InputSources;
using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.InputSystem.EventData
{
    public class InputDualAxisPositionEventData : InputEventData
    {
        /// <summary>
        /// The Dual Axis Input Type
        /// </summary>
        public InputType InputType { get; private set; }

        /// <summary>
        /// Two values, typically from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// Typically this is Touch or Joystick data.
        /// </summary>
        public Vector2 DualAxisPosition { get; private set; }

        public InputDualAxisPositionEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, InputType inputType, Vector2 position, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            InputType = inputType;
            DualAxisPosition = position;
        }

        public void Initialize(IInputSource inputSource, InputType inputType, Vector2 position, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            InputType = inputType;
            DualAxisPosition = position;
        }
    }
}
