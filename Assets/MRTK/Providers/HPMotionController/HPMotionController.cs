// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Input;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.HP.Input
{
    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Utilities.Handedness.Left, Utilities.Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class HPMotionController : BaseController
    {
        public HPMotionController(TrackingState trackingState, Utilities.Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The current pose of this controller.
        /// </summary>
        protected MixedRealityPose CurrentControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The previous pose of this controller.
        /// </summary>
        protected MixedRealityPose LastControllerPose = MixedRealityPose.ZeroIdentity;

        /// <summary>
        /// The current position of this controller.
        /// </summary>
        protected Vector3 CurrentControllerPosition = Vector3.zero;

        /// <summary>
        /// The current rotation of this controller.
        /// </summary>
        protected Quaternion CurrentControllerRotation = Quaternion.identity;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateController");

        /// <summary>
        /// Update the controller data from .
        /// </summary>
        public virtual void UpdateController(MotionControllerState controllerState)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (!Enabled) { return; }

                if (Interactions == null)
                {
                    Debug.LogError($"No interaction configuration for {GetType().Name}");
                    Enabled = false;
                }

                /*********Position stuff is TODO for now since we aren't sure how to get it from motioncontroller*****/
                /*
                var lastState = TrackingState;
                LastControllerPose = CurrentControllerPose;

                // Check for position and rotation.
                IsPositionAvailable = controllerState.TryGetFeatureValue(CommonUsages.devicePosition, out CurrentControllerPosition);
                IsPositionApproximate = false;

                IsRotationAvailable = controllerState.TryGetFeatureValue(CommonUsages.deviceRotation, out CurrentControllerRotation);

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
                */

                for (int i = 0; i < Interactions?.Length; i++)
                {
                    switch (Interactions[i].AxisType)
                    {
                        case AxisType.None:
                            break;
                        case AxisType.Digital:
                            UpdateButtonData(Interactions[i], controllerState);
                            break;
                        case AxisType.SingleAxis:
                            UpdateSingleAxisData(Interactions[i], controllerState);
                            break;
                        case AxisType.DualAxis:
                            UpdateDualAxisData(Interactions[i], controllerState);
                            break;
                        case AxisType.SixDof:
                            UpdatePoseData(Interactions[i], controllerState);
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateButtonData");

        /// <summary>
        /// Update an interaction bool data type from a bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// </remarks>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                // Handedness must be left or right in order to differentiate between buttons for the left and right hand.
                MixedReality.Input.Handedness controllerHandedness = controllerState.MotionController.Handedness;


                Debug.Assert(controllerHandedness != MixedReality.Input.Handedness.Unknown);
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                if (interactionMapping.InputType == DeviceInputType.TriggerTouch)
                {
                    var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                    interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
                }
                else if (interactionMapping.InputType == DeviceInputType.GripTouch)
                {
                    var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                    interactionMapping.BoolData = !Mathf.Approximately(gripData, 0.0f);
                }
                else
                {
                    ControllerInput button;

                    // Update the interaction data source
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.Select:
                        case DeviceInputType.TriggerNearTouch:
                        case DeviceInputType.TriggerPress:
                            button = ControllerInput.Trigger;
                            break;
                        case DeviceInputType.GripNearTouch:
                        case DeviceInputType.GripPress:
                            button = ControllerInput.Grasp;
                            break;
                        case DeviceInputType.ButtonPress:
                        case DeviceInputType.PrimaryButtonPress:
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.X_Button : ControllerInput.A_Button;
                            break;
                        case DeviceInputType.SecondaryButtonPress:
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.Y_Button : ControllerInput.B_Button;
                            break;
                        case DeviceInputType.TouchpadTouch:
                        case DeviceInputType.TouchpadPress:
                            button = ControllerInput.Touchpad;
                            break;
                        case DeviceInputType.Menu:
                            button = ControllerInput.Menu;
                            break;
                        case DeviceInputType.ThumbStickTouch:
                        case DeviceInputType.ThumbStickPress:
                            button = ControllerInput.Thumbstick;
                            break;
                        default:
                            return;
                    }


                    var buttonData = controllerState.CurrentReading.GetPressedValue(button);
                    interactionMapping.BoolData = buttonData > 0.0f;
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

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateSingleAxisData");

        /// <summary>
        /// Update an interaction float data type from a SingleAxis (float) input
        /// </summary>
        /// <remarks>
        /// Raises a FloatInputChanged event when the float data changes
        /// </remarks>
        protected virtual void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                        interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
                        break;
                    case DeviceInputType.GripPress:
                        var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                        interactionMapping.BoolData = !Mathf.Approximately(gripData, 0.0f);
                        break;
                    default:
                        return;
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

                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                        interactionMapping.FloatData = triggerData;
                        break;
                    case DeviceInputType.Grip:
                        var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                        interactionMapping.FloatData = gripData;
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

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateDualAxisData");

        /// <summary>
        /// Update the touchpad / thumbstick input from the device
        /// </summary>
        protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
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

                System.Numerics.Vector2 controllerAxisData = controllerState.CurrentReading.GetXYValue(ControllerInput.Thumbstick);
                Vector2 axisData = new Vector2(controllerAxisData.X, controllerAxisData.Y);

                // Update the interaction data source
                interactionMapping.Vector2Data = axisData;

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
                }
            }
        }

        private static readonly ProfilerMarker UpdatePoseDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdatePoseData");

        /// <summary>
        /// Update spatial grip data.
        /// </summary>
        protected virtual void UpdatePoseData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
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
    }
}