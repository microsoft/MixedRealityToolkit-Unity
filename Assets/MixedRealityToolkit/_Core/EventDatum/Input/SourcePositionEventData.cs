// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Internal.EventDatum.Input
{
    /// <summary>
    /// Describes an input event that involves an Input Source's spatial position.
    /// </summary>
    public class SourcePositionEventData : InputEventData
    {
        /// <summary>
        /// The Pointer Position of the Input Source.
        /// </summary>
        public Vector3 PointerPosition { get; private set; }

        /// <summary>
        /// The Grip Position of the Input Source.
        /// </summary>
        public Vector3 GripPosition { get; private set; }

        /// <inheritdoc />
        public SourcePositionEventData(EventSystem eventSystem) : base(eventSystem) { }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="pointerPosition"></param>
        /// <param name="gripPosition"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }

        /// <summary>
        /// Populates the event with data.
        /// </summary>
        /// <param name="inputSource"></param>
        /// <param name="handedness"></param>
        /// <param name="pointerPosition"></param>
        /// <param name="gripPosition"></param>
        /// <param name="tags"></param>
        public void Initialize(IMixedRealityInputSource inputSource, Handedness handedness, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }
    }
}
