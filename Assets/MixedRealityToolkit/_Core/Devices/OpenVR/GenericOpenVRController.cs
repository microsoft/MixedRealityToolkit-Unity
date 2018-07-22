// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
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
        public XRNodeState LastStateReading;

        private Vector2 touchpadPosition = Vector2.zero;
        private Vector2 thumbstickPosition = Vector2.zero;
        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;

        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;


        public static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            new MixedRealityInteractionMapping(3, "Trigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            new MixedRealityInteractionMapping(5, "Grip Average", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            new MixedRealityInteractionMapping(6, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.PrimaryIndexTrigger
            new MixedRealityInteractionMapping(7, "Thumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.PrimaryIndexTrigger
            new MixedRealityInteractionMapping(8, "Thumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.PrimaryThumbstick
            new MixedRealityInteractionMapping(9, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            new MixedRealityInteractionMapping(13, "Unity Button Id 2", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMapping(14, "Unity Button Id 3", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton3),
            // Oculus Touch Controller - Button.Start Press
            new MixedRealityInteractionMapping(15, "Unity Button Id 7", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton7),
            // Oculus Touch Controller - Button.Three Touch
            new MixedRealityInteractionMapping(16, "Unity Button Id 12", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton12),
            // Oculus Touch Controller - Button.Four Touch
            new MixedRealityInteractionMapping(17, "Unity Button Id 13", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton13),
            // Oculus Touch Controller - Touch.PrimaryThumbRest Touch
            new MixedRealityInteractionMapping(18, "Thumb Rest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Touch.PrimaryThumbRest Near Touch
            new MixedRealityInteractionMapping(19, "Thumb Rest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Left Controller Index Finger Cap Sensor
            new MixedRealityInteractionMapping(20, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Left Controller Middle Finger Cap Sensor
            new MixedRealityInteractionMapping(21, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Left Controller Ring Finger Cap Sensor
            new MixedRealityInteractionMapping(22, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Left Controller Pinky Finger Cap Sensor
            new MixedRealityInteractionMapping(23, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, MixedRealityInputAction.None),
        };

        public static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            new MixedRealityInteractionMapping(3, "Trigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            new MixedRealityInteractionMapping(5, "Grip Average", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            // Oculus Touch Controller - Axis2D.SecondaryThumbstick
            new MixedRealityInteractionMapping(6, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.SecondaryIndexTrigger
            new MixedRealityInteractionMapping(7, "Thumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.SecondaryIndexTrigger
            new MixedRealityInteractionMapping(8, "Thumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Button.SecondaryThumbstick
            new MixedRealityInteractionMapping(9, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            new MixedRealityInteractionMapping(13, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMapping(14, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
            // Oculus Touch Controller - Button.One Touch
            new MixedRealityInteractionMapping(15, "Unity Button Id 10", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton10),
            // Oculus Touch Controller - Button.Two Touch
            new MixedRealityInteractionMapping(16, "Unity Button Id 11", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton11),
            // Oculus Touch Controller - Touch.SecondaryThumbRest Touch
            new MixedRealityInteractionMapping(17, "Thumb Rest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, MixedRealityInputAction.None),
            // Oculus Touch Controller - Touch.SecondaryThumbRest Near Touch
            new MixedRealityInteractionMapping(18, "Thumb Rest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Right Controller Index Finger Cap Sensor
            new MixedRealityInteractionMapping(19, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Right Controller Middle Finger Cap Sensor
            new MixedRealityInteractionMapping(20, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Right Controller Ring Finger Cap Sensor
            new MixedRealityInteractionMapping(21, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, MixedRealityInputAction.None),
            // Valve Knuckles Controller - Right Controller Pinky Finger Cap Sensor
            new MixedRealityInteractionMapping(22, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, MixedRealityInputAction.None),
        };

        #region Base override configuration

        /// <summary>
        /// Mapping method to expose the Unity Input Manager mapping configuration
        /// </summary>
        public override InputManagerAxis[] ControllerAxisMappings => ControllerInputAxisMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <summary>
        /// Mapping method to expose this controllers Unity Input Manager mapping array
        /// </summary>
        public virtual string[] VRInputMappings => ControllerInputAxisMappingLibrary.GetInputManagerMappings(GetType().FullName);

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            switch (controllerHandedness)
            {
                case Handedness.Left:
                    AssignControllerMappings(DefaultLeftHandedInteractions);
                    break;
                case Handedness.Right:
                    AssignControllerMappings(DefaultRightHandedInteractions);
                    break;
            }
        }

        #endregion Base override configuration

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
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.TriggerPress:
                        UpdateTriggerData(Interactions[i]);
                        break;
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
                        UpdateKnucklesThumbData(Interactions[i]);
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
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentPointerPose);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateGripData(MixedRealityInteractionMapping interactionMapping)
        {
            // Get the current grip button press state
            var gripButtonPress = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[10]) : Input.GetAxis(VRInputMappings[11]);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Grip:
                    // Update the interaction data source
                    interactionMapping.FloatData = gripButtonPress;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, gripButtonPress);
                    }
                    break;
                case DeviceInputType.GripPress:
                    // Update the interaction data source
                    interactionMapping.BoolData = gripButtonPress.Equals(1f);

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
                    break;
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Touchpad:
                    touchpadPosition.x = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[0]) : Input.GetAxis(VRInputMappings[2]);
                    touchpadPosition.y = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[1]) : Input.GetAxis(VRInputMappings[3]);

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
                    // Get the current Touchpad button Touch state
                    var touchpadTouchButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton16) : Input.GetKey(KeyCode.JoystickButton17);

                    // Update the interaction data source
                    interactionMapping.BoolData = touchpadTouchButton;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (touchpadTouchButton)
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
                    break;
                case DeviceInputType.TouchpadPress:
                    // Get the current Touchpad button state
                    var touchpadPressButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                    // Update the interaction data source
                    interactionMapping.BoolData = touchpadPressButton;

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
                    break;
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStick:
                    // Get the current input state
                    thumbstickPosition.x = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[4]) : Input.GetAxis(VRInputMappings[6]);
                    thumbstickPosition.y = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[5]) : Input.GetAxis(VRInputMappings[7]);

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
                    // Get the current input state
                    var thumbstickTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton16) : Input.GetKey(KeyCode.JoystickButton17);

                    // Update the interaction data source
                    interactionMapping.BoolData = thumbstickTouch;

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
                    break;
                case DeviceInputType.ThumbStickPress:
                    // Get the current input state
                    var thumbstickPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                    // Update the interaction data source
                    interactionMapping.BoolData = thumbstickPress;

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
                    break;
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Trigger:
                    // Get the current input state
                    var triggerPressAmount = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[8]) : Input.GetAxis(VRInputMappings[9]);

                    // Update the interaction data source
                    interactionMapping.FloatData = triggerPressAmount;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, triggerPressAmount);
                    }
                    break;
                case DeviceInputType.TriggerNearTouch:
                    // Get the current input state
                    var triggerNearTouchAmount = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    // Update the interaction data source
                    interactionMapping.FloatData = triggerNearTouchAmount;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, triggerNearTouchAmount);
                    }
                    break;
                case DeviceInputType.TriggerTouch:
                    // Get the current input state
                    var triggerTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton14) : Input.GetKey(KeyCode.JoystickButton15);

                    //Update the interaction data source
                    interactionMapping.BoolData = triggerTouch;

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
                    break;
                case DeviceInputType.TriggerPress:
                    // Get the current input state
                    var pressAmount = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[8]) : Input.GetAxis(VRInputMappings[9]);

                    // Update the interaction data source
                    interactionMapping.BoolData = pressAmount.Equals(1f);

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
                    break;
            }
        }

        /// <summary>
        /// Update the buttons state.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.InputType == DeviceInputType.ButtonPress);

            // Get the current input state
            var buttonPress = Input.GetKey(interactionMapping.KeyCode);

            // Update the interaction data source
            interactionMapping.BoolData = buttonPress;

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

        private void UpdateKnucklesFingerData(MixedRealityInteractionMapping mixedRealityInteractionMapping)
        {
            throw new NotImplementedException();
        }

        private void UpdateKnucklesThumbData(MixedRealityInteractionMapping mixedRealityInteractionMapping)
        {
            throw new NotImplementedException();
        }

        #endregion Update data functions
    }
}