// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class InputPositionEventData : InputEventData
    {
        /// <summary>
        /// Two values, from -1.0 to 1.0 in the X-axis and Y-axis, representing where the input control is positioned.
        /// </summary>
        public Vector2 Position { get; private set; }

        public Handedness Handedness { get; private set; }

        public InputType InputType { get; private set; }

        public InputPositionEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, Vector2 position, InputType inputType, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, sourceId, tags);
            Position = position;
            InputType = inputType;
            Handedness = handedness;
        }
    }
}
