// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
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

        private Vector2 dualAxisPosition = Vector2.zero;
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
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS9),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton15),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS9),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            new MixedRealityInteractionMapping(4, "Grip", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS11),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS1, ControllerMappingLibrary.MIXEDREALITY_AXIS2),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton16),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton8),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 2", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 3", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton3),
        };

        public static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS10),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None, KeyCode.JoystickButton15),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS10),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            new MixedRealityInteractionMapping(4, "Grip", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS12),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None, KeyCode.None, ControllerMappingLibrary.MIXEDREALITY_AXIS4, ControllerMappingLibrary.MIXEDREALITY_AXIS5),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None, KeyCode.JoystickButton17),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None, KeyCode.JoystickButton9),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        public void UpdateController(XRNodeState xrNodeState)
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
                        UpdatePointerData(xrNodeState, Interactions[i]);
                        break;
                    case DeviceInputType.Trigger:
                        UpdateSingleAxisData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.Touchpad:
                        UpdateDualAxisData(Interactions[i]);
                        break;
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.ThumbStickNearTouch:
                    case DeviceInputType.ThumbStickTouch:
                    case DeviceInputType.ThumbStickPress:
                    case DeviceInputType.TouchpadNearTouch:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                    case DeviceInputType.ButtonPress:
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.ThumbNearTouch:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case DeviceInputType.IndexFinger:
                    case DeviceInputType.MiddleFinger:
                    case DeviceInputType.RingFinger:
                    case DeviceInputType.PinkyFinger:
                        UpdateSingleAxisData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [GenericOpenVRController]");
                        break;
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

        /// <summary>
        /// Update Spatial Pointer Data.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdatePointerData(XRNodeState xrNodeState, MixedRealityInteractionMapping interactionMapping)
        {
            xrNodeState.TryGetPosition(out currentControllerPosition);
            xrNodeState.TryGetRotation(out currentControllerRotation);

            if (CameraCache.Main.transform.parent != null)
            {
                currentControllerPose.Position = CameraCache.Main.transform.parent.TransformPoint(currentControllerPosition);
                currentControllerPose.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentControllerRotation.eulerAngles));
            }

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
        /// Update an Interaction Float data type from a SingleAxis (float) input 
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Pressed" event when the float data changes
        /// </remarks>
        /// <param name="interactionMapping"></param>
        protected void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);

            var singleAxisValue = Input.GetAxis(interactionMapping.AxisCodeX);

            // Update the interaction data source
            interactionMapping.FloatData = singleAxisValue;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, singleAxisValue);
            }
        }

        /// <summary>
        /// Update the Touchpad / Thumbstick input from the device (in OpenVR, touchpad and thumbstick are the same input control)
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

            dualAxisPosition.x = Input.GetAxis(interactionMapping.AxisCodeX);
            dualAxisPosition.y = Input.GetAxis(interactionMapping.AxisCodeY);

            // Update the interaction data source
            interactionMapping.Vector2Data = dualAxisPosition;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, dualAxisPosition);
            }
        }

        /// <summary>
        /// Update an Interaction Bool data type from a Bool input 
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// Also raises a "Pressed" event while pressed
        /// </remarks>
        protected void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            Debug.Assert(interactionMapping.AxisType == AxisType.Digital || interactionMapping.AxisType == AxisType.SingleAxis);

            switch (interactionMapping.AxisType)
            {
                case AxisType.Digital:
                    var keyButton = Input.GetKey(interactionMapping.KeyCode);

                    // Update the interaction data source
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
                    break;
                case AxisType.SingleAxis:
                    var axisButtonPressAmount = Input.GetAxis(interactionMapping.AxisCodeX);

                    // Update the interaction data source
                    interactionMapping.BoolData = axisButtonPressAmount.Equals(1);

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
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, axisButtonPressAmount);
                        }
                    }
                    break;
            }
        }
    }
}