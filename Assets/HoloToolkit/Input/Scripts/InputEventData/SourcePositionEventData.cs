// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that a source moving.
    /// </summary>
    public class SourcePositionEventData : InteractionInputEventData
    {
        /// <summary>
        /// The new position of the source.
        /// </summary>
        public Vector3 PointerPosition { get; private set; }

        public Vector3 GripPosition { get; private set; }

        public SourcePositionEventData(EventSystem eventSystem) : base(eventSystem) { }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, uint sourceId, Vector3 pointerPosition, Vector3 gripPosition, Handedness handedness, object[] tags = null)
        {
            Initialize(inputSource, sourceId, InteractionSourcePressType.None, handedness, tags);
            PointerPosition = pointerPosition;
            GripPosition = gripPosition;
        }
#endif
    }
}
