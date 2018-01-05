// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Describes an input event that has a source id.
    /// </summary>
    public class InputEventData : BaseInputEventData
    {
        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, object[] tags = null)
        {
            BaseInitialize(inputSource, sourceId, tags);
        }

        public void Initialize(IInputSource inputSource, uint sourceId, Handedness handedness, object[] tags = null)
        {
            BaseInitialize(inputSource, sourceId, handedness, tags);
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, uint sourceId, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            BaseInitialize(inputSource, sourceId, pressType, handedness, tags);
        }
#endif
    }
}
