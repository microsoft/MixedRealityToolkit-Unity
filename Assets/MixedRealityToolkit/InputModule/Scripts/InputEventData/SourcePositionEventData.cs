﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.InputSources;
using MixedRealityToolkit.InputModule.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MixedRealityToolkit.InputModule.EventData
{
    /// <summary>
    /// Describes an input event that a source moving.
    /// </summary>
    public class SourcePositionEventData : InputEventData
    {
        /// <summary>
        /// The new position of the source.
        /// </summary>
        public Vector3 PointerPosition { get; private set; }

        public Vector3 GripPosition { get; private set; }

        public SourcePositionEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, Vector3 pointerPosition, Vector3 gripPosition, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }

        public void Initialize(IInputSource inputSource, Vector3 pointerPosition, Vector3 gripPosition, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, handedness, tags);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }
    }
}
