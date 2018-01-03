// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine.EventSystems;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

public class InteractionInputEventData : InputEventData
{
#if UNITY_WSA
    public InteractionSourcePressType PressType { get; private set; }
#endif

    public Handedness Handedness { get; private set; }

    public InteractionInputEventData(EventSystem eventSystem) : base(eventSystem) { }

#if UNITY_WSA
    public void Initialize(IInputSource inputSource, uint sourceId, InteractionSourcePressType pressType, Handedness handedness, object[] tags)
    {
        Initialize(inputSource, sourceId, tags);
        PressType = pressType;
        Handedness = handedness;
    }
#endif
}
