// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// XR SDK implementation of Windows Mixed Reality motion controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/MotionController",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class WindowsMixedRealityXRSDKMotionController : BaseWindowsMixedRealityXRSDKSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityXRSDKMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : this(trackingState, controllerHandedness, new WindowsMixedRealityControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        public WindowsMixedRealityXRSDKMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSourceDefinition definition,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, definition)
        { }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityXRSDKMotionController.UpdateButtonData");

        /// <inheritdoc />
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                InputFeatureUsage<bool> buttonUsage;

                // These mappings are flipped from the base class,
                // where thumbstick is primary and touchpad is secondary.
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TouchpadTouch:
                        buttonUsage = CommonUsages.primary2DAxisTouch;
                        break;
                    case DeviceInputType.TouchpadPress:
                        buttonUsage = CommonUsages.primary2DAxisClick;
                        break;
                    case DeviceInputType.ThumbStickPress:
                        buttonUsage = CommonUsages.secondary2DAxisClick;
                        break;
                    default:
                        base.UpdateButtonData(interactionMapping, inputDevice);
                        return;
                }

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

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityXRSDKMotionController.UpdateDualAxisData");

        /// <inheritdoc />
        protected override void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateDualAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

                InputFeatureUsage<Vector2> axisUsage;

                // These mappings are flipped from the base class,
                // where thumbstick is primary and touchpad is secondary.
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ThumbStick:
                        axisUsage = CommonUsages.secondary2DAxis;
                        break;
                    case DeviceInputType.Touchpad:
                        axisUsage = CommonUsages.primary2DAxis;
                        break;
                    default:
                        base.UpdateDualAxisData(interactionMapping, inputDevice);
                        return;
                }

                if (inputDevice.TryGetFeatureValue(axisUsage, out Vector2 axisData))
                {
                    // Update the interaction data source
                    interactionMapping.Vector2Data = axisData;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
                }
            }
        }

#if WINDOWS_UWP
        private WindowsMixedRealityControllerModelProvider controllerModelProvider;

        /// <inheritdoc />
        protected override bool TryRenderControllerModel(System.Type controllerType, InputSourceType inputSourceType)
        {
            if (GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUsePlatformModelsOverride(GetType(), ControllerHandedness))
            {
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
            else
            {
                TryRenderControllerModelWithModelProvider();
                return true;
            }
        }

        private async void TryRenderControllerModelWithModelProvider()
        {
            if (controllerModelProvider == null)
            {
                controllerModelProvider = new WindowsMixedRealityControllerModelProvider(ControllerHandedness);
            }

            GameObject controllerModel = await controllerModelProvider.TryGenerateControllerModelFromPlatformSDK();

            if (this != null)
            {
                if (controllerModel != null
                    && MixedRealityControllerModelHelpers.TryAddVisualizationScript(controllerModel, GetType(), ControllerHandedness)
                    && TryAddControllerModelToSceneHierarchy(controllerModel))
                {
                    controllerModel.SetActive(true);
                    return;
                }

                Debug.LogWarning("Failed to create controller model from driver; defaulting to BaseController behavior.");
                base.TryRenderControllerModel(GetType(), InputSource.SourceType);
            }

            if (controllerModel != null)
            {
                // If we didn't successfully set up the model and add it to the hierarchy (which returns early), set it inactive.
                controllerModel.SetActive(false);
            }
        }
#endif
    }
}