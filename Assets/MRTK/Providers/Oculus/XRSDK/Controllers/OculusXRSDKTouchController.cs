// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.```

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

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

        internal GameObject OculusControllerVisualization { get; private set; }

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

        /// <inheritdoc />
        protected override bool TryRenderControllerModel(System.Type controllerType, InputSourceType inputSourceType)
        {
            if (GetControllerVisualizationProfile() != null &&
                GetControllerVisualizationProfile().GetUsePlatformModelsOverride(GetType(), ControllerHandedness))
            {
                TryRenderControllerModelFromOculus();
                return true;
            }
            else
            {
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
        }

        private void TryRenderControllerModelFromOculus()
        {
#if OCULUSINTEGRATION_PRESENT
            OculusXRSDKDeviceManager deviceManager = CoreServices.GetInputSystemDataProvider<OculusXRSDKDeviceManager>();

            if (deviceManager.IsNotNull())
            {
                GameObject platformVisualization = null;
                if (ControllerHandedness == Handedness.Left)
                {
                    platformVisualization = deviceManager.leftControllerHelper.gameObject;
                }
                else if (ControllerHandedness == Handedness.Right)
                {
                    platformVisualization = deviceManager.rightControllerHelper.gameObject;
                }
                RegisterControllerVisualization(platformVisualization);
            }
#endif

            if (this != null)
            {
                if (OculusControllerVisualization != null
                    && MixedRealityControllerModelHelpers.TryAddVisualizationScript(OculusControllerVisualization, GetType(), ControllerHandedness)
                    && TryAddControllerModelToSceneHierarchy(OculusControllerVisualization))
                {
                    OculusControllerVisualization.SetActive(true);
                    return;
                }

                Debug.LogWarning("Failed to obtain Oculus controller model; defaulting to BaseController behavior.");
                base.TryRenderControllerModel(GetType(), InputSource.SourceType);
            }
        }

        private void RegisterControllerVisualization(GameObject visualization)
        {
            OculusControllerVisualization = visualization;
            if (GetControllerVisualizationProfile() != null &&
                !GetControllerVisualizationProfile().GetUsePlatformModelsOverride(GetType(), ControllerHandedness))
            {
                OculusControllerVisualization.SetActive(false);
            }
        }
    }
}
