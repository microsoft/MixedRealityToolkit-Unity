// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if WINDOWS_UWP
#if WMR_ENABLED
using UnityEngine.XR.WindowsMR;
#endif // WMR_ENABLED
using Windows.UI.Input.Spatial;
#endif // WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// XR SDK implementation of Windows Mixed Reality motion controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "StandardAssets/Textures/MotionController")]
    public class WindowsMixedRealityXRSDKMotionController : BaseWindowsMixedRealityXRSDKSource
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityXRSDKMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new WindowsMixedRealityControllerDefinition(inputSource, controllerHandedness);
//#if DOTNETWINRT_PRESENT
#if WINDOWS_UWP
            List<object> states = new List<object>();
            XRSubsystemHelpers.InputSubsystem?.GetCurrentSourceStates(states);

            foreach (SpatialInteractionSourceState sourceState in states)
            {
                if (sourceState.Source.Handedness.ToMRTKHandedness() == ControllerHandedness)
                {
                    controllerModelProvider = new WindowsMixedRealityControllerModelProvider(this, sourceState.Source);
                }
            }
#endif
        }

        private readonly WindowsMixedRealityControllerDefinition controllerDefinition;
//#if DOTNETWINRT_PRESENT
#if WINDOWS_UWP
        private readonly WindowsMixedRealityControllerModelProvider controllerModelProvider;
#endif

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => controllerDefinition?.DefaultInteractions;

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

//#if DOTNETWINRT_PRESENT
#if WINDOWS_UWP
        /// <inheritdoc />
        protected override bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            DebugUtilities.Log("trying to render controller");
            // Intercept this call if we are using the default driver provided models.
            // Note: Obtaining models from the driver will require access to the InteractionSource.
            // It's unclear whether the interaction source will be available during setup, so we attempt to create
            // the controller model on an input update
            if (controllerModelProvider.UnableToGenerateSDKModel ||
                GetControllerVisualizationProfile() == null ||
                !GetControllerVisualizationProfile().GetUseDefaultModelsOverride(GetType(), ControllerHandedness))
            {
                DebugUtilities.Log("defaulting to base rendering");
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
            else
            {
                DebugUtilities.Log("trying to render from drivers");
                TryRenderControllerModelWithModelProvider();
            }

            return !controllerModelProvider.UnableToGenerateSDKModel;
        }

        private async void TryRenderControllerModelWithModelProvider()
        {
            GameObject controllerModel = await Task.Run(() => controllerModelProvider.TryGenerateControllerModelFromPlatformSDK());

            if (controllerModel != null)
            {
                DebugUtilities.Log("rendering model from sdk");
                TryAddControllerModelToSceneHierarchy(controllerModel);
            }
        }
#endif
    }
}