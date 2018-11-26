// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using HoloToolkit.Unity.InputModule;

namespace HoloToolkit.Unity.Tests
{
    [RequireComponent(typeof(SetGlobalListener))]
    public class HapticsTest : MonoBehaviour, IInputHandler
    {
        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            InteractionInputSource inputSource = eventData.InputSource as InteractionInputSource;
            if (inputSource != null)
            {
                switch (eventData.PressType)
                {
                    case InteractionSourcePressInfo.Grasp:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f);
                        return;
                    case InteractionSourcePressInfo.Menu:
                        inputSource.StartHaptics(eventData.SourceId, 1.0f, 1.0f);
                        return;
                }
            }
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            InteractionInputSource inputSource = eventData.InputSource as InteractionInputSource;
            if (inputSource != null)
            {
                if (eventData.PressType == InteractionSourcePressInfo.Grasp)
                {
                    inputSource.StopHaptics(eventData.SourceId);
                }
            }
        }
    }
}
