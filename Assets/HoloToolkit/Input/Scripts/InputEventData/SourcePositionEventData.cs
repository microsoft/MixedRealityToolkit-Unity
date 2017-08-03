// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that a source moving.
    /// </summary>
    public class SourcePositionEventData : BaseInputEventData
    {
        /// <summary>
        /// The new position of the source.
        /// </summary>
        public Vector3 PointerPosition { get; private set; }
        public Vector3 GripPosition { get; private set; }

        public SourcePositionEventData(EventSystem eventSystem) : base(eventSystem)
        {
        }

        public void Initialize(IInputSource inputSource, uint sourceId, object tag, Vector3 pointerPosition, Vector3 gripPosition)
        {
            BaseInitialize(inputSource, sourceId, tag);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }
    }
}