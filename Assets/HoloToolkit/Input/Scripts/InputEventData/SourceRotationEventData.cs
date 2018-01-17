// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that involves a source's rotation changing.
    /// </summary>
    public class SourceRotationEventData : InputEventData
    {
        /// <summary>
        /// The new rotation of the source.
        /// </summary>
        public Quaternion PointerRotation { get; private set; }
        public Quaternion GripRotation { get; private set; }

        public SourceRotationEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, Quaternion pointerRotation, Quaternion gripRotation, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PointerRotation = pointerRotation;
            GripRotation = gripRotation;
        }
    }
}
