// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine.XR.WSA.Input;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A HP Motion Controller Instance.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Handedness.Left, Handedness.Right })]
    public class HPMotionController : WindowsMixedRealityController
    {
#if HP_CONTROLLER_ENABLED
        internal HPMotionControllerInputHandler InputHandler = null;
        internal MotionControllerState MotionControllerState = null;
#endif

        public HPMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new HPMotionControllerDefinition(controllerHandedness), inputSource, interactions)
        {
#if HP_CONTROLLER_ENABLED
            InputHandler = new HPMotionControllerInputHandler(controllerHandedness, inputSource, Interactions);
#endif
        }

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
                    // If the Motion controller state is instantiated and tracked, use it to update the interaction bool data
                    // the interaction source updates the 6-DoF data first since some interaction mappings rely on 6-DoF data
                    UpdateSixDofData(interactionSourceState);
                    InputHandler.UpdateController(MotionControllerState);
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
