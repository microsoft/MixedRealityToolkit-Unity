// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    [MixedRealityController(
        SupportedControllerType.GenericUnity,
        new[] { Handedness.Left, Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class GenericXRSDKController : BaseController
    {
        public GenericXRSDKController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The current pose of this XR SDK controller.
        /// </summary>
        protected MixedRealityPose CurrentControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The previous pose of this XR SDK controller.
        /// </summary>
        protected MixedRealityPose LastControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The current position of this XR SDK controller.
        /// </summary>
        protected Vector3 CurrentControllerPosition = Vector3.zero;

        /// <summary>
        /// The current rotation of this XR SDK controller.
        /// </summary>
        protected Quaternion CurrentControllerRotation = Quaternion.identity;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdateController");

        /// <summary>
        /// Update the controller data from XR SDK.
        /// </summary>
        public virtual void UpdateController(InputDevice inputDevice)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (!Enabled) { return; }

                if (Interactions == null)
                {
                    Debug.LogError($"No interaction configuration for {GetType().Name}");
                    Enabled = false;
                }

                var lastState = TrackingState;
                LastControllerPose = CurrentControllerPose;

                // Check for position and rotation.
                IsPositionAvailable = inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out CurrentControllerPosition);
                IsPositionApproximate = false;

                IsRotationAvailable = inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out CurrentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(CurrentControllerPosition);
                CurrentControllerRotation = MixedRealityPlayspace.Rotation * CurrentControllerRotation;

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

                for (int i = 0; i < Interactions?.Length; i++)
                {
                    switch (Interactions[i].AxisType)
                    {
                        case AxisType.None:
                            break;
                        case AxisType.Digital:
                            UpdateButtonData(Interactions[i], inputDevice);
                            break;
                        case AxisType.SingleAxis:
                            UpdateSingleAxisData(Interactions[i], inputDevice);
                            break;
                        case AxisType.DualAxis:
                            UpdateDualAxisData(Interactions[i], inputDevice);
                            break;
                        case AxisType.SixDof:
                            UpdatePoseData(Interactions[i], inputDevice);
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdateButtonData");

        /// <summary>
        /// Update an interaction bool data type from a bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// </remarks>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                if (interactionMapping.InputType == DeviceInputType.TriggerTouch
                    && inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerData))
                {
                    interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
                }
                else
                {
                    InputFeatureUsage<bool> buttonUsage;

                    // Update the interaction data source
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.Select:
                            buttonUsage = CommonUsages.triggerButton;
                            break;
                        case DeviceInputType.TouchpadTouch:
                            buttonUsage = CommonUsages.primary2DAxisTouch;
                            break;
                        case DeviceInputType.TouchpadPress:
                            buttonUsage = CommonUsages.primary2DAxisClick;
                            break;
                        case DeviceInputType.Menu:
                            buttonUsage = CommonUsages.menuButton;
                            break;
                        case DeviceInputType.ThumbStickPress:
                            buttonUsage = CommonUsages.secondary2DAxisClick;
                            break;
                        default:
                            return;
                    }

                    if (inputDevice.TryGetFeatureValue(buttonUsage, out bool buttonPressed))
                    {
                        interactionMapping.BoolData = buttonPressed;
                    }
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

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdateSingleAxisData");

        /// <summary>
        /// Update an interaction float data type from a SingleAxis (float) input
        /// </summary>
        /// <remarks>
        /// Raises a FloatInputChanged event when the float data changes
        /// </remarks>
        protected virtual void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool buttonPressed))
                        {
                            interactionMapping.BoolData = buttonPressed;
                        }

                        // If our bool value changed raise it.
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

                        if (inputDevice.TryGetFeatureValue(CommonUsages.grip, out float buttonData))
                        {
                            interactionMapping.FloatData = buttonData;
                        }
                        break;
                    case DeviceInputType.Trigger:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerData))
                        {
                            interactionMapping.FloatData = triggerData;
                        }
                        break;
                    default:
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                }
            }
        }

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdateDualAxisData");

        /// <summary>
        /// Update the touchpad / thumbstick input from the device
        /// </summary>
        protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdateDualAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

                InputFeatureUsage<Vector2> axisUsage;

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ThumbStick:
                        axisUsage = CommonUsages.secondary2DAxis;
                        break;
                    case DeviceInputType.Touchpad:
                        axisUsage = CommonUsages.primary2DAxis;
                        break;
                    default:
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

        private static readonly ProfilerMarker UpdatePoseDataPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdatePoseData");

        /// <summary>
        /// Update spatial grip data.
        /// </summary>
        protected virtual void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, InputDevice inputDevice)
        {
            using (UpdatePoseDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SixDof);

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.SpatialGrip:
                        interactionMapping.PoseData = CurrentControllerPose;

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            // Raise input system event if it's enabled
                            CoreServices.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
                        }
                        break;
                }
            }
        }
    }
}