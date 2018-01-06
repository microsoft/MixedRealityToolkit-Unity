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

        public void Initialize(IInputSource inputSource, object[] tags = null)
        {
            BaseInitialize(inputSource, tags);
        }

        public void Initialize(IInputSource inputSource, Handedness handedness, object[] tags = null)
        {
            BaseInitialize(inputSource, handedness, tags);
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, InteractionSourcePressType pressType, Handedness handedness, object[] tags = null)
        {
            BaseInitialize(inputSource, pressType, handedness, tags);
        }
#endif
    }
}
