// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes Dual Axis Positional Event data, usually generated from a Joystick, or Touch input source.
    /// </summary>
    public class DualAxisInputEventData : InputEventData
    {
        /// <summary>
        /// Two values, typically from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// Typically this is Touch or Joystick data.
        /// </summary>
        public Vector2 DualAxisPosition { get; private set; }

        /// <inheritdoc />
        public DualAxisInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        /// <param name="position"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputType inputType, Vector2 position, object[] tags = null)
        {
            Initialize(inputSource, inputType, tags);
            DualAxisPosition = position;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputType"></param>
        /// <param name="position"></param>
        /// <param name="handedness"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputType inputType, Vector2 position, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, inputType, tags);
            DualAxisPosition = position;
        }
    }
}
