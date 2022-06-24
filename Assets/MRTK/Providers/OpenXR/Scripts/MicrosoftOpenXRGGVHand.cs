// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// A GGV (Gaze, Gesture, and Voice) hand instance for OpenXR.
    /// Used only for the purposes of acting on the select keyword detected by HoloLens 2.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.GGVHand,
        new[] { Handedness.Left, Handedness.Right, Handedness.None })]
    internal class MicrosoftOpenXRGGVHand : GenericXRSDKController
    {
        public MicrosoftOpenXRGGVHand(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new SimpleHandDefinition(controllerHandedness))
        { }

        internal void UpdateVoiceState(bool isPressed)
        {
            MixedRealityInteractionMapping interactionMapping = null;

            for (int i = 0; i < Interactions?.Length; i++)
            {
                MixedRealityInteractionMapping currentInteractionMapping = Interactions[i];

                if (currentInteractionMapping.AxisType == AxisType.Digital && currentInteractionMapping.InputType == DeviceInputType.Select)
                {
                    interactionMapping = currentInteractionMapping;
                    break;
                }
            }

            if (interactionMapping == null)
            {
                return;
            }

            interactionMapping.BoolData = isPressed;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system event if it's enabled
                if (interactionMapping.BoolData)
                {
                    CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }
    }
}
