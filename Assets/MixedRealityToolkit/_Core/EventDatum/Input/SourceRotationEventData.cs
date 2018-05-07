// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves an Input Source's rotation.
    /// </summary>
    public class SourceRotationEventData : InputEventData
    {
        /// <summary>
        /// The Pointer Rotation of the Input Source.
        /// </summary>
        public Quaternion PointerRotation { get; private set; }

        /// <summary>
        /// The Grip Rotation of the Input Source.
        /// </summary>
        public Quaternion GripRotation { get; private set; }

        /// <inheritdoc />
        public SourceRotationEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="pointerRotation"></param>
        /// <param name="gripRotation"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Quaternion pointerRotation, Quaternion gripRotation, object[] tags = null)
        {
            Initialize(inputSource, tags);
            PointerRotation = pointerRotation;
            GripRotation = gripRotation;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="pointerRotation"></param>
        /// <param name="gripRotation"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, Quaternion pointerRotation, Quaternion gripRotation, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PointerRotation = pointerRotation;
            GripRotation = gripRotation;
        }
    }
}
