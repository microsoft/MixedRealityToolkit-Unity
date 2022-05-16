// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.OpenXR
{
    /// <summary>
    /// Open XR + XR SDK implementation of <see href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#_microsoft_mixed_reality_motion_controller_profile">interaction_profiles/microsoft/motion_controller</see>.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/MotionController")]
    public class MicrosoftMotionController : GenericXRSDKController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MicrosoftMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, new WindowsMixedRealityControllerDefinition(controllerHandedness))
        { }

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        private static readonly ProfilerMarker UpdatePoseDataPerfMarker = new ProfilerMarker("[MRTK] WindowsMixedRealityOpenXRController.UpdatePoseData");

        /// <summary>
        /// Update spatial pointer and spatial grip data.
        /// </summary>
        protected override void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdatePoseDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.SpatialPointer:
                        if (inputDevice.TryGetFeatureValue(CustomUsages.PointerPosition, out currentPointerPosition))
                        {
                            currentPointerPose.Position = MixedRealityPlayspace.TransformPoint(currentPointerPosition);
                        }

                        if (inputDevice.TryGetFeatureValue(CustomUsages.PointerRotation, out currentPointerRotation))
                        {
                            currentPointerPose.Rotation = MixedRealityPlayspace.Rotation * currentPointerRotation;
                        }

                        interactionMapping.PoseData = currentPointerPose;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system event if it's enabled
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
                        }
                        break;
                    default:
                        base.UpdatePoseData(interactionMapping, inputDevice);
                        break;
                }
            }
        }

#if MSFT_OPENXR
        private OpenXRControllerModelProvider controllerModelProvider;

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
                controllerModelProvider = new OpenXRControllerModelProvider(ControllerHandedness);
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
#endif // MSFT_OPENXR
    }
}
