// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatas.Input
{
    /// <summary>
    /// Describes an input event that involves a source's rotation changing.
    /// </summary>
    public class SourceRotationEventData : InputEventData
    {
        /// <summary>
        /// The new rotation of the pointer source.
        /// </summary>
        public Quaternion PointerRotation { get; private set; }

        /// <summary>
        /// The new rotation of the grip source.
        /// </summary>
        public Quaternion GripRotation { get; private set; }

        public SourceRotationEventData(UnityEngine.EventSystems.EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, Quaternion pointerRotation, Quaternion gripRotation, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PointerRotation = pointerRotation;
            GripRotation = gripRotation;
        }
    }
}
