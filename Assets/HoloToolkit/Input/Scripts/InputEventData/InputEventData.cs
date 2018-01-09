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
        public Handedness Handedness { get; private set; }

#if UNITY_WSA
        public InteractionSourcePressType PressType { get; private set; }
#endif

        public InputEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = Handedness.None;
#if UNITY_WSA
            PressType = InteractionSourcePressType.None;
#endif
        }

        public void Initialize(IInputSource inputSource, Handedness handedness, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = handedness;
#if UNITY_WSA
            PressType = InteractionSourcePressType.None;
#endif
        }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, InteractionSourcePressType pressType, Handedness handedness, object[] tags)
        {
            BaseInitialize(inputSource, tags);
            Handedness = handedness;
            PressType = pressType;
        }
#endif
    }
}
