// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes Two Degrees of Freedom event data, usually generated from a Joystick, or Touch input source.
    /// </summary>
    public class TwoDoFInputEventData : InputEventData
    {
        /// <summary>
        /// Two values, typically from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// Typically this is Touch or Joystick data.
        /// </summary>
        public Vector2 Position { get; private set; } = Vector2.zero;

        /// <inheritdoc />
        public TwoDoFInputEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputAction inputAction, Vector2 position)
        {
            Initialize(inputSource, inputAction);
            Position = position;
        }

        /// <summary>
        /// Used to initialize/reset the event and populate the data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="inputAction"></param>
        /// <param name="position"></param>
        /// <param name="handedness"></param>
        public void Initialize(IMixedRealityInputSource inputSource, InputAction inputAction, Vector2 position, Handedness handedness)
        {
            Initialize(inputSource, handedness, inputAction);
            Position = position;
        }
    }
}
