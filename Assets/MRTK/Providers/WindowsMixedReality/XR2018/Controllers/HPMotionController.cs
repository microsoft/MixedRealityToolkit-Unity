// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using System;
using UnityEngine.XR.WSA.Input;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Handedness.Left, Handedness.Right })]
    public class HPMotionController : WindowsMixedRealityController
    {
#if HP_CONTROLLER_ENABLED
        internal HPMotionControllerInputHandler inputHandler;
        internal MotionControllerState MotionControllerState;
#endif

        public HPMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
#if HP_CONTROLLER_ENABLED
            inputHandler = new HPMotionControllerInputHandler(controllerHandedness, inputSource, Interactions);
#endif
            controllerDefinition = new HPControllerDefinition(inputSource, controllerHandedness);
        }

        private readonly HPControllerDefinition controllerDefinition;
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => controllerDefinition.DefaultLeftHandedInteractions;
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => controllerDefinition.DefaultRightHandedInteractions;


#if UNITY_WSA
        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPMotionController.UpdateController");

        /// <inheritdoc />
        public override void UpdateController(InteractionSourceState interactionSourceState)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (!Enabled) { return; }

#if HP_CONTROLLER_ENABLED
                if (MotionControllerState != null)
                {
                    // If the Motion controller state is instantiated and tracked, use it to update the interaction bool data and the interaction source to update the 6-dof data
                    inputHandler.UpdateController(MotionControllerState);
                    UpdateSixDofData(interactionSourceState);
                }
                else
                {
                    // Otherwise, update normally
                    base.UpdateController(interactionSourceState);
                }
#else
                
                base.UpdateController(interactionSourceState);
#endif
            }
        }
#endif
    }
}
