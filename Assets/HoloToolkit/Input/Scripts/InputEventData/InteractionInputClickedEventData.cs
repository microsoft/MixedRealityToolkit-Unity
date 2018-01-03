// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.InputModule
{
    public class InteractionInputClickedEventData : InputClickedEventData
    {
#if UNITY_WSA
        public InteractionSourcePressType PressType { get; private set; }
#endif

        public Handedness Handedness { get; private set; }

        public InteractionInputClickedEventData(EventSystem eventSystem) : base(eventSystem) { }

#if UNITY_WSA
        public void Initialize(IInputSource inputSource, uint sourceId, InteractionSourcePressType pressType, Handedness handedness, int tapCount, object[] tags = null)
        {
            Initialize(inputSource, sourceId, tapCount, tags);
            PressType = pressType;
            Handedness = handedness;
        }
#endif
    }
}
