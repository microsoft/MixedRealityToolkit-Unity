// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.InputModule.Focus;
using MixedRealityToolkit.InputModule.InputHandlers;
using MixedRealityToolkit.InputModule.InputSources;
using UnityEngine;

namespace MixedRealityToolkit.Examples.InputModule
{
    public class InputHandleCallbackFX : MonoBehaviour, IInputHandler
    {
        [SerializeField]
        private ParticleSystem particles = null;

        private void Start()
        {
            InputManager.Instance.PushFallbackInputHandler(gameObject);
        }

        void IInputHandler.OnInputUp(InputEventData eventData)
        {
            FocusDetails focusDetails;
            if (FocusManager.Instance.TryGetFocusDetails(eventData, out focusDetails))
            {
                particles.transform.position = focusDetails.Point;
                particles.Emit(60);
            }
        }

        void IInputHandler.OnInputDown(InputEventData eventData) { }

        void IInputHandler.OnInputPressed(InputPressedEventData eventData) { }

        void IInputHandler.OnInputPositionChanged(InputPositionEventData eventData) { }
    }
}