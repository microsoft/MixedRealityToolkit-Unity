// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.```

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using System;

#if OCULUS_ENABLED
using Unity.XR.Oculus;
#endif

namespace Microsoft.MixedReality.Toolkit.XRSDK.Oculus.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusTouch,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/OculusControllersTouch",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class OculusXRSDKTouchController : GenericXRSDKController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusXRSDKTouchController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new OculusTouchControllerDefinition(controllerHandedness))
        { }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] OculusXRSDKController.UpdateButtonData");

        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                InputFeatureUsage<bool> buttonUsage;
                bool usingOculusButtonData = false;

#if OCULUS_ENABLED
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerTouch:
                        buttonUsage = OculusUsages.indexTouch;
                        usingOculusButtonData = true;
                        break;
                    case DeviceInputType.TriggerNearTouch:
                        buttonUsage = OculusUsages.indexTouch;
                        usingOculusButtonData = true;
                        break;
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.ThumbNearTouch:
                        buttonUsage = OculusUsages.thumbrest;
                        usingOculusButtonData = true;
                        break;
                }
#endif

                if (!usingOculusButtonData)
                {
                    base.UpdateButtonData(interactionMapping, inputDevice);
                }
                else
                {
                    if (inputDevice.TryGetFeatureValue(buttonUsage, out bool buttonPressed))
                    {
                        interactionMapping.BoolData = buttonPressed;
                    }

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

        /// <summary>
        /// Determines whether or not this controller is using MRTK for controller visualization
        /// 
        /// When false, the Oculus Touch controller model visualization will not be managed by MRTK
        /// Ensure that the Oculus Integration Package is installed and the Ovr Camera Rig is set correctly in the Oculus XRSDK Device Manager to use the
        /// Oculus Integration Package's visualization
        /// </summary>
        internal bool UseMRTKControllerVisualization
        {
            get
            {
                return Visualizer != null && Visualizer.GameObjectProxy != null && Visualizer.GameObjectProxy.activeSelf;
            }
            set
            {
                if(Visualizer != null && Visualizer.GameObjectProxy)
                {
                    Visualizer.GameObjectProxy.SetActive(value);
                }
            }
        }
    }
}
