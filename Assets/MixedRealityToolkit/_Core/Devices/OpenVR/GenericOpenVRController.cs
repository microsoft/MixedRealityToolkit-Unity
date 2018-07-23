// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public class GenericOpenVRController : BaseController
    {
        public GenericOpenVRController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastStateReading { get; protected set; }

        private Vector2 touchpadPosition = Vector2.zero;
        private Vector2 thumbstickPosition = Vector2.zero;
        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;

        public static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, "OPENVR_TRIGGER_LEFT_CONTROLLER"),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton15),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, "OPENVR_TRIGGER_LEFT_CONTROLLER"),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            new MixedRealityInteractionMapping(5, "Grip", AxisType.SingleAxis, DeviceInputType.Grip, MixedRealityInputAction.None, KeyCode.None, "OPENVR_GRIP_LEFT_CONTROLLER"),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(10, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(11, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton16),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(12, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton8),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            new MixedRealityInteractionMapping(13, "Unity Button Id 2", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMapping(14, "Unity Button Id 3", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton3),
        };

        public static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, "OPENVR_TRIGGER_RIGHT_CONTROLLER"),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton15),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, "OPENVR_TRIGGER_RIGHT_CONTROLLER"),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            new MixedRealityInteractionMapping(5, "Grip", AxisType.SingleAxis, DeviceInputType.Grip, MixedRealityInputAction.None, KeyCode.None, "OPENVR_GRIP_RIGHT_CONTROLLER"),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(10, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(11, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton17),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(12, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton9),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            new MixedRealityInteractionMapping(13, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMapping(14, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        public virtual void UpdateController(XRNodeState xrNodeState)
        {
            UpdateControllerData(xrNodeState);

            Debug.Assert(Interactions != null, "No interaction configuration for controller");
            if (Interactions == null) { Enabled = false; }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdatePointerData(Interactions[i]);
                        break;
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.Grip:
                    case DeviceInputType.GripNearTouch:
                    case DeviceInputType.GripTouch:
                    case DeviceInputType.GripPress:
                        UpdateGripData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickNearTouch:
                    case DeviceInputType.ThumbStickTouch:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonPress:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.ThumbNearTouch:
                        UpdateOculusTouchThumbRestData(Interactions[i]);
                        break;
                    case DeviceInputType.IndexFinger:
                    case DeviceInputType.MiddleFinger:
                    case DeviceInputType.RingFinger:
                    case DeviceInputType.PinkyFinger:
                        UpdateKnucklesFingerData(Interactions[i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Input [{Interactions[i].InputType}] is not handled for this controller [GenericOpenVRController]");
                }
            }
            LastStateReading = xrNodeState;
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="state"></param>
        protected void UpdateControllerData(XRNodeState state)
        {
            var lastState = TrackingState;

            XRNode nodeType = state.nodeType;

            if (nodeType == XRNode.LeftHand || nodeType == XRNode.RightHand)
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = state.TryGetPosition(out currentControllerPosition);
                IsPositionApproximate = false;

                IsRotationAvailable = state.TryGetRotation(out currentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                if (CameraCache.Main.transform.parent != null)
                {
                    currentControllerPose.Position = CameraCache.Main.transform.parent.TransformPoint(currentControllerPosition);
                    currentControllerPose.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentControllerRotation.eulerAngles));
                }
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            lastControllerPose = currentControllerPose;
            currentControllerPose.Position = currentControllerPosition;
            currentControllerPose.Rotation = currentControllerRotation;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastControllerPose != currentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentControllerRotation);
                }
            }
        }

        protected void UpdatePointerData(MixedRealityInteractionMapping interactionMapping)
        {
            // TODO: configure an offset pointer position for each OpenVR Controller?
            // Update the interaction data source
            interactionMapping.PoseData = currentControllerPose; // Currently no way to get pointer specific data, so we use the last controller pose.

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.PoseData);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateGripData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Grip:
                    // Get the current input state
                    UpdateSingleAxisFromAxis(interactionMapping);
                    break;
                case DeviceInputType.GripTouch:
                    // Get the current input state
                    UpdateButtonFromKey(interactionMapping);
                    break;
                case DeviceInputType.GripNearTouch:
                case DeviceInputType.GripPress:
                    // Get the current input state
                    UpdateButtonFromAxis(interactionMapping);
                    break;
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Touchpad:
                    touchpadPosition.x = ControllerHandedness == Handedness.Left ? Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS1) : Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS4);
                    touchpadPosition.y = ControllerHandedness == Handedness.Left ? Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS2) : Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS5);

                    // Update the interaction data source
                    interactionMapping.Vector2Data = touchpadPosition;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, touchpadPosition);
                    }
                    break;
                case DeviceInputType.TouchpadTouch:
                case DeviceInputType.TouchpadPress:
                    // Get the current Touchpad button state
                    UpdateButtonFromKey(interactionMapping);
                    break;
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStick:
                    // Get the current input state
                    thumbstickPosition.x = ControllerHandedness == Handedness.Left ? Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS1) : Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS4);
                    thumbstickPosition.y = ControllerHandedness == Handedness.Left ? Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS2) : Input.GetAxis(ControllerMappingLibrary.MIXEDREALITY_OPENVR_AXIS5);

                    //Update the interaction data source
                    interactionMapping.Vector2Data = thumbstickPosition;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, thumbstickPosition);
                    }
                    break;
                case DeviceInputType.ThumbStickTouch:
                case DeviceInputType.ThumbStickPress:
                    // Get the current input state
                    UpdateButtonFromKey(interactionMapping);
                    break;
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Trigger:
                    // Get the current input state
                    UpdateSingleAxisFromAxis(interactionMapping);
                    break;
                case DeviceInputType.TriggerTouch:
                    // Get the current input state
                    UpdateButtonFromKey(interactionMapping);
                    break;
                case DeviceInputType.TriggerNearTouch:
                case DeviceInputType.TriggerPress:
                    // Get the current input state
                    UpdateButtonFromAxis(interactionMapping);
                    break;
            }
        }

        /// <summary>
        /// Update the buttons state.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.InputType == DeviceInputType.ButtonPress);

            // Get the current input state
            UpdateButtonFromKey(interactionMapping);
        }

        /// <summary>
        /// Update the Oculus Touch Thumb rest Data.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateOculusTouchThumbRestData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbTouch:
                    // Get the current input state
                    UpdateButtonFromKey(interactionMapping);
                    break;
                case DeviceInputType.ThumbNearTouch:
                    // Get the current input state
                    UpdateButtonFromAxis(interactionMapping);
                    break;
            }
        }

        /// <summary>
        /// Update the Valve Knuckles Finger Data.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateKnucklesFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            UpdateSingleAxisFromAxis(interactionMapping);
        }

        #endregion Update data functions

        #region Event Functions

        protected void UpdateSingleAxisFromAxis(MixedRealityInteractionMapping interactionMapping)
        {
            var singleAxisValue = Input.GetAxis(interactionMapping.AxisCode);

            // Update the interaction data source
            interactionMapping.FloatData = singleAxisValue;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, singleAxisValue);
            }
        }

        protected void UpdateButtonFromAxis(MixedRealityInteractionMapping interactionMapping)
        {
            var axisButton = Input.GetAxis(interactionMapping.AxisCode);

            // Update the interaction data source
            interactionMapping.BoolData = axisButton.Equals(1);

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, axisButton);
            }
        }

        protected void UpdateButtonFromKey(MixedRealityInteractionMapping interactionMapping)
        {
            var keyButton = Input.GetKey(interactionMapping.KeyCode);

            //Update the interaction data source
            interactionMapping.BoolData = keyButton;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                if (interactionMapping.BoolData)
                {
                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
            else
            {
                if (interactionMapping.BoolData)
                {
                    InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Event Functions
    }
}