// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class InputHandleCallbackFX : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private ParticleSystem particles = null;

        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        void IInputHandler.OnInputDown(InputEventData eventData)
        {
            // Nothing.
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            if (eventData.PressType == InteractionSourcePressInfo.Select)
            {
                FocusDetails? focusDetails = FocusManager.Instance.TryGetFocusDetails(eventData);

                if (focusDetails != null)
                {
                    particles.transform.position = focusDetails.Value.Point;
                    particles.Emit(60);

                    eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
                }
            }
        }
    }
}