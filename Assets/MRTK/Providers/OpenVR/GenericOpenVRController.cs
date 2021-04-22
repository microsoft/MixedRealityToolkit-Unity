// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.GenericOpenVR,
        new[] { Handedness.Left, Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings,
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class GenericOpenVRController : GenericJoystickController
    {
        public GenericOpenVRController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : this(trackingState, controllerHandedness, new GenericOpenVRControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        public GenericOpenVRController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSourceDefinition definition,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, definition, inputSource, interactions)
        {
            nodeType = controllerHandedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand;
        }

        private readonly XRNode nodeType;

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastXrNodeStateReading { get; protected set; }

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager.
        /// </summary>
        private readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <summary>
        /// A private static list of previously loaded controller models.
        /// </summary>
        private static readonly Dictionary<Handedness, GameObject> controllerDictionary = new Dictionary<Handedness, GameObject>(0);

        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMappingLegacyInput(),
            // HTC Vive Controller - Left Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Left Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Left Trigger Squeeze
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            // Windows Mixed Reality Controller - Left Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            // Windows Mixed Reality Controller - Left Grip Button Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2, invertYAxis: true),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Touchpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton16),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            // Windows Mixed Reality Controller - Left Menu Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton3),
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton18),
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_17, axisCodeY: ControllerMappingLibrary.AXIS_18),
            new MixedRealityInteractionMappingLegacyInput(),
        };

        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMappingLegacyInput(),
            // HTC Vive Controller - Right Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Right Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Right Trigger Squeeze
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            // Windows Mixed Reality Controller - Right Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            // Windows Mixed Reality Controller - Right Grip Button Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Touchpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton17),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            // Windows Mixed Reality Controller - Right Menu Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1),
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton19),
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_19, axisCodeY: ControllerMappingLibrary.AXIS_20),
            new MixedRealityInteractionMappingLegacyInput(),
        };

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] GenericOpenVRController.UpdateController");

        /// <inheritdoc />
        public override void UpdateController()
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (!Enabled) { return; }

                InputTracking.GetNodeStates(nodeStates);

                for (int i = 0; i < nodeStates.Count; i++)
                {
                    if (nodeStates[i].nodeType == nodeType)
                    {
                        var xrNodeState = nodeStates[i];
                        UpdateControllerData(xrNodeState);
                        LastXrNodeStateReading = xrNodeState;
                        break;
                    }
                }

                base.UpdateController();
            }
        }

        private static readonly ProfilerMarker UpdateControllerDataPerfMarker = new ProfilerMarker("[MRTK] GenericOpenVRController.UpdateControllerData");

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        protected void UpdateControllerData(XRNodeState state)
        {
            using (UpdateControllerDataPerfMarker.Auto())
            {
                var lastState = TrackingState;

                LastControllerPose = CurrentControllerPose;

                if (nodeType == XRNode.LeftHand || nodeType == XRNode.RightHand)
                {
                    // The source is either a hand or a controller that supports pointing.
                    // We can now check for position and rotation.
                    IsPositionAvailable = state.TryGetPosition(out CurrentControllerPosition);
                    IsPositionApproximate = false;

                    IsRotationAvailable = state.TryGetRotation(out CurrentControllerRotation);

                    // Devices are considered tracked if we receive position OR rotation data from the sensors.
                    TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                    CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(CurrentControllerPosition);
                    CurrentControllerRotation = MixedRealityPlayspace.Rotation * CurrentControllerRotation;
                }
                else
                {
                    // The input source does not support tracking.
                    TrackingState = TrackingState.NotApplicable;
                }

                CurrentControllerPose.Position = CurrentControllerPosition;
                CurrentControllerPose.Rotation = CurrentControllerRotation;

                // Raise input system events if it is enabled.
                if (lastState != TrackingState)
                {
                    CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
                }

                if (TrackingState == TrackingState.Tracked && LastControllerPose != CurrentControllerPose)
                {
                    if (IsPositionAvailable && IsRotationAvailable)
                    {
                        CoreServices.InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                    }
                    else if (IsPositionAvailable && !IsRotationAvailable)
                    {
                        CoreServices.InputSystem?.RaiseSourcePositionChanged(InputSource, this, CurrentControllerPosition);
                    }
                    else if (!IsPositionAvailable && IsRotationAvailable)
                    {
                        CoreServices.InputSystem?.RaiseSourceRotationChanged(InputSource, this, CurrentControllerRotation);
                    }
                }
            }
        }

        #region Controller model functions

        /// <inheritdoc />
        protected override bool TryRenderControllerModel(Type controllerType, InputSourceType inputSourceType)
        {
            MixedRealityControllerVisualizationProfile visualizationProfile = GetControllerVisualizationProfile();

            // Intercept this call if we are using the default driver provided models.
            if (visualizationProfile == null ||
                !visualizationProfile.GetUsePlatformModelsOverride(GetType(), ControllerHandedness))
            {
                return base.TryRenderControllerModel(controllerType, inputSourceType);
            }
            else if (controllerDictionary.TryGetValue(ControllerHandedness, out GameObject controllerModel))
            {
                TryAddControllerModelToSceneHierarchy(controllerModel);
                controllerModel.SetActive(true);
                return true;
            }

            Debug.Log("Trying to load controller model from platform SDK");

            GameObject controllerModelGameObject = new GameObject($"{ControllerHandedness} OpenVR Controller");

            bool failedToObtainControllerModel;

            var visualizationType = visualizationProfile.GetControllerVisualizationTypeOverride(GetType(), ControllerHandedness);
            if (visualizationType != null)
            {
                // Set the platform controller model to not be destroyed when the source is lost. It'll be disabled instead,
                // and re-enabled when the same controller is re-detected.
                if (controllerModelGameObject.AddComponent(visualizationType.Type) is IMixedRealityControllerPoseSynchronizer visualizer)
                {
                    visualizer.DestroyOnSourceLost = false;
                }

                OpenVRRenderModel openVRRenderModel = controllerModelGameObject.AddComponent<OpenVRRenderModel>();
                Material overrideMaterial = visualizationProfile.GetPlatformModelMaterialOverride(GetType(), ControllerHandedness);
                if (overrideMaterial != null)
                {
                    openVRRenderModel.shader = overrideMaterial.shader;
                }

                failedToObtainControllerModel = !openVRRenderModel.LoadModel(ControllerHandedness);

                if (!failedToObtainControllerModel)
                {
                    TryAddControllerModelToSceneHierarchy(controllerModelGameObject);
                    controllerDictionary.Add(ControllerHandedness, controllerModelGameObject);
                }
            }
            else
            {
                Debug.LogError("Controller visualization type not defined for controller visualization profile");
                failedToObtainControllerModel = true;
            }

            if (failedToObtainControllerModel)
            {
                Debug.LogWarning("Failed to create controller model from driver, defaulting to BaseController behavior");
                UnityEngine.Object.Destroy(controllerModelGameObject);
                return base.TryRenderControllerModel(GetType(), InputSourceType.Controller);
            }

            return true;
        }

        #endregion Controller model functions
    }
}
