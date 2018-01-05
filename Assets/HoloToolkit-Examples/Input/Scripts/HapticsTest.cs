// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

namespace HoloToolkit.Unity.Tests
{
    [RequireComponent(typeof(SetGlobalListener))]
    public class HapticsTest : MonoBehaviour, IInputHandler
    {
        void IInputHandler.OnInputDown(InputEventData eventData)
        {

#if UNITY_WSA
            InteractionInputSource inputSource = (InteractionInputSource)eventData.InputSource;

            if (inputSource != null)
            {
                switch (eventData.PressType)
                {
                    case InteractionSourcePressType.Grasp:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f);
                        return;
                    case InteractionSourcePressType.Menu:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f, 1.0f);
                        return;
                }
            }
#endif
        }

        public void OnInputPressed(InputPressedEventData eventData) { }

        public void OnInputPositionChanged(InputPositionEventData eventData) { }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
#if UNITY_WSA
            InteractionInputSource inputSource = eventData.InputSource as InteractionInputSource;
            if (inputSource != null)
            {
                if (eventData.PressType == InteractionSourcePressType.Grasp)
                {
                    inputSource.StopHaptics(eventData.SourceId);
                }
            }
#endif
        }
    }
}
