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
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings,
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class GenericXRSDKController : BaseController, IMixedRealityHapticFeedback
    {
        public GenericXRSDKController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        { }

        public GenericXRSDKController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null,
            IMixedRealityInputSourceDefinition definition = null)
            : base(trackingState, controllerHandedness, inputSource, interactions, definition)
        { }

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

        /// <summary>
        /// The most recent input device that this controller represents.
        /// </summary>
        private InputDevice lastInputDevice;

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
                    return;
                }

                UpdateSixDofData(inputDevice);

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
                    }
                }

                lastInputDevice = inputDevice;
            }
        }

        protected virtual void UpdateSixDofData(InputDevice inputDevice)
        {
            if (!UpdateSourceData(inputDevice))
            {
                return;
            }

            UpdateVelocity(inputDevice);

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
                    case AxisType.SixDof:
                        UpdatePoseData(Interactions[i], inputDevice);
                        break;
                }
            }
        }

        private static readonly ProfilerMarker UpdateSourceDataPerfMarker = new ProfilerMarker("[MRTK] BaseWindowsMixedRealitySource.UpdateSourceData");

        /// <summary>
        /// Update the source input from the device.
        /// </summary>
        /// <param name="inputDevice">The InputDevice retrieved from the platform.</param>
        /// <returns>Whether position or rotation was successfully updated.</returns>
        public bool UpdateSourceData(InputDevice inputDevice)
        {
            using (UpdateSourceDataPerfMarker.Auto())
            {
                TrackingState lastState = TrackingState;
                LastControllerPose = CurrentControllerPose;

                // Check for position and rotation.
                bool isPositionAvailable = inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
                bool isRotationAvailable = inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

                if (isPositionAvailable || isRotationAvailable)
                {
                    IsPositionAvailable = isPositionAvailable;
                    IsPositionApproximate = false;
                    IsRotationAvailable = isRotationAvailable;

                    // Devices are considered tracked if we receive position OR rotation data from the sensors.
                    TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                    CurrentControllerPosition = MixedRealityPlayspace.TransformPoint(position);
                    CurrentControllerRotation = MixedRealityPlayspace.Rotation * rotation;

                    CurrentControllerPose.Position = CurrentControllerPosition;
                    CurrentControllerPose.Rotation = CurrentControllerRotation;

                    // Raise input system events if it is enabled.
                    if (lastState != TrackingState)
                    {
                        CoreServices.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
                    }

                    return true;
                }

                return false;
            }
        }

        private static readonly ProfilerMarker UpdateVelocityPerfMarker = new ProfilerMarker("[MRTK] GenericXRSDKController.UpdateVelocity");

        public void UpdateVelocity(InputDevice inputDevice)
        {
            using (UpdateVelocityPerfMarker.Auto())
            {
                if (inputDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 newVelocity))
                {
                    Velocity = newVelocity;
                }

                if (inputDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 newAngularVelocity))
                {
                    AngularVelocity = newAngularVelocity;
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
                else if (interactionMapping.InputType == DeviceInputType.GripTouch
                    && inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripData))
                {
                    interactionMapping.BoolData = !Mathf.Approximately(gripData, 0.0f);
                }
                else
                {
                    InputFeatureUsage<bool> buttonUsage;

                    // Update the interaction data source
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.Select:
                        case DeviceInputType.TriggerNearTouch:
                        case DeviceInputType.TriggerPress:
                            buttonUsage = CommonUsages.triggerButton;
                            break;
                        case DeviceInputType.GripNearTouch:
                        case DeviceInputType.GripPress:
                            buttonUsage = CommonUsages.gripButton;
                            break;
                        case DeviceInputType.ButtonPress:
                        case DeviceInputType.PrimaryButtonPress:
                            buttonUsage = CommonUsages.primaryButton;
                            break;
                        case DeviceInputType.PrimaryButtonTouch:
                            buttonUsage = CommonUsages.primaryTouch;
                            break;
                        case DeviceInputType.SecondaryButtonPress:
                            buttonUsage = CommonUsages.secondaryButton;
                            break;
                        case DeviceInputType.SecondaryButtonTouch:
                            buttonUsage = CommonUsages.secondaryTouch;
                            break;
                        case DeviceInputType.TouchpadTouch:
                            buttonUsage = CommonUsages.secondary2DAxisTouch;
                            break;
                        case DeviceInputType.TouchpadPress:
                            buttonUsage = CommonUsages.secondary2DAxisClick;
                            break;
                        case DeviceInputType.Menu:
                            buttonUsage = CommonUsages.menuButton;
                            break;
                        case DeviceInputType.ThumbStickTouch:
                            buttonUsage = CommonUsages.primary2DAxisTouch;
                            break;
                        case DeviceInputType.ThumbStickPress:
                            buttonUsage = CommonUsages.primary2DAxisClick;
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
                // First handle updating the bool values, since those events are only raised once the trigger/gripped is presssed
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
                        {
                            interactionMapping.BoolData = triggerPressed;
                        }
                        break;
                    case DeviceInputType.GripPress:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed))
                        {
                            interactionMapping.BoolData = gripPressed;
                        }
                        break;
                    default:
                        break;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise bool input system event if it's available
                    if (interactionMapping.BoolData)
                    {
                        CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }

                // Next handle updating the float values
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerData))
                        {
                            interactionMapping.FloatData = triggerData;
                        }
                        break;
                    case DeviceInputType.Grip:
                        if (inputDevice.TryGetFeatureValue(CommonUsages.grip, out float gripData))
                        {
                            interactionMapping.FloatData = gripData;
                        }
                        break;
                    default:
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise float input system event if it's enabled
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
                        axisUsage = CommonUsages.primary2DAxis;
                        break;
                    case DeviceInputType.Touchpad:
                        axisUsage = CommonUsages.secondary2DAxis;
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
                    case DeviceInputType.SpatialPointer:
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

        /// <inheritdoc />
        public bool StartHapticImpulse(float intensity, float durationInSeconds = float.MaxValue)
        {
            if (lastInputDevice.TryGetHapticCapabilities(out HapticCapabilities hapticCapabilities) && hapticCapabilities.supportsImpulse)
            {
                if (Mathf.Approximately(durationInSeconds, float.MaxValue))
                {
                    lastInputDevice.SendHapticImpulse(0, intensity);
                }
                else
                {
                    lastInputDevice.SendHapticImpulse(0, intensity, durationInSeconds);
                }
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void StopHapticFeedback() => lastInputDevice.StopHaptics();
    }
}
