// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using System;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// XR SDK implementation of HP Motion controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Handedness.Left, Handedness.Right })]
    public class HPMotionController : WindowsMixedRealityXRSDKMotionController
    {
#if HP_CONTROLLER_ENABLED
        internal HPMotionControllerInputHandler InputHandler;
        internal MotionControllerState MotionControllerState;
#endif

        public HPMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
#if HP_CONTROLLER_ENABLED
            InputHandler = new HPMotionControllerInputHandler(controllerHandedness, inputSource, Interactions);
#endif
            controllerDefinition = new HPMotionControllerDefinition(inputSource, controllerHandedness);
        }

        private readonly HPMotionControllerDefinition controllerDefinition;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => controllerDefinition.DefaultLeftHandedInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => controllerDefinition.DefaultRightHandedInteractions;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateController");

        /// <inheritdoc />
        public override void UpdateController(InputDevice inputDevice)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (!Enabled) { return; }

#if HP_CONTROLLER_ENABLED
                if (MotionControllerState != null)
                {
                    // If the Motion controller state is instantiated and tracked, use it to update the interaction bool data 
                    // the interaction source updates the 6-dof data first since some interaction mappings rely on 6dof data
                    base.UpdateSixDofData(inputDevice);
                    InputHandler.UpdateController(MotionControllerState);
                }
                else
                {
                    // Otherwise, update normally
                    base.UpdateController(inputDevice);
                }
#else
                
                    base.UpdateController(inputDevice);
#endif
            }
        }
    }
}
