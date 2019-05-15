// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.GenericOpenVR,
        new[] { Handedness.Left, Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class GenericOpenVRController : GenericJoystickController
    {
        public GenericOpenVRController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            nodeType = controllerHandedness == Handedness.Left ? XRNode.LeftHand : XRNode.RightHand;
        }

        private XRNode nodeType;

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastXrNodeStateReading { get; protected set; }

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager
        /// </summary>
        private readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Left Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Left Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Left Trigger Squeeze
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            // Windows Mixed Reality Controller - Left Trigger Press (Select)
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton14),
            // HTC Vive Controller - Left Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.PrimaryIndexTrigger
            // Valve Knuckles Controller - Left Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_9),
            // HTC Vive Controller - Left Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.PrimaryHandTrigger
            // Valve Knuckles Controller - Left Controller Grip Average
            // Windows Mixed Reality Controller - Left Grip Button Press
            new MixedRealityInteractionMapping(4, "Grip Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_11),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Position
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, false, true),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Touchpad Touch
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton16),
            // HTC Vive Controller - Left Controller Trackpad (2)
            // Oculus Touch Controller - Button.PrimaryThumbstick
            // Valve Knuckles Controller - Left Controller Trackpad
            // Windows Mixed Reality Controller - Left Thumbstick Press
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton8),
            // HTC Vive Controller - Left Controller Menu Button (1)
            // Oculus Touch Controller - Button.Three Press
            // Valve Knuckles Controller - Left Controller Inner Face Button
            // Windows Mixed Reality Controller - Left Menu Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 2", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton2),
            // Oculus Touch Controller - Button.Four Press
            // Valve Knuckles Controller - Left Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 3", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(10, "WMR Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton18),
            new MixedRealityInteractionMapping(11, "WMR Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_17, ControllerMappingLibrary.AXIS_18),
        };

        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            // Controller Pose
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            // HTC Vive Controller - Right Controller Trigger (7) Squeeze
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger Squeeze
            // Valve Knuckles Controller - Right Controller Trigger Squeeze
            // Windows Mixed Reality Controller - Right Trigger Squeeze
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            // Windows Mixed Reality Controller - Right Trigger Press (Select)
            new MixedRealityInteractionMapping(2, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress, KeyCode.JoystickButton15),
            // HTC Vive Controller - Right Controller Trigger (7)
            // Oculus Touch Controller - Axis1D.SecondaryIndexTrigger
            // Valve Knuckles Controller - Right Controller Trigger
            new MixedRealityInteractionMapping(3, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, ControllerMappingLibrary.AXIS_10),
            // HTC Vive Controller - Right Controller Grip Button (8)
            // Oculus Touch Controller - Axis1D.SecondaryHandTrigger
            // Valve Knuckles Controller - Right Controller Grip Average
            // Windows Mixed Reality Controller - Right Grip Button Press
            new MixedRealityInteractionMapping(4, "Grip Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_12),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Axis2D.PrimaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Position
            new MixedRealityInteractionMapping(5, "Trackpad-Thumbstick Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, false, true),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Touchpad Touch
            new MixedRealityInteractionMapping(6, "Trackpad-Thumbstick Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton17),
            // HTC Vive Controller - Right Controller Trackpad (2)
            // Oculus Touch Controller - Button.SecondaryThumbstick
            // Valve Knuckles Controller - Right Controller Trackpad
            // Windows Mixed Reality Controller - Right Thumbstick Press
            new MixedRealityInteractionMapping(7, "Trackpad-Thumbstick Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            // HTC Vive Controller - Right Controller Menu Button (1)
            // Oculus Remote - Button.One Press
            // Oculus Touch Controller - Button.One Press
            // Valve Knuckles Controller - Right Controller Inner Face Button
            // Windows Mixed Reality Controller - Right Menu Button
            new MixedRealityInteractionMapping(8, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            // Oculus Remote - Button.Two Press
            // Oculus Touch Controller - Button.Two Press
            // Valve Knuckles Controller - Right Controller Outer Face Button
            new MixedRealityInteractionMapping(9, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(10, "WMR Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton19),
            new MixedRealityInteractionMapping(11, "WMR Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.AXIS_19, ControllerMappingLibrary.AXIS_20),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }

        /// <inheritdoc />
        public override void UpdateController()
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

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="state"></param>
        protected void UpdateControllerData(XRNodeState state)
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
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && LastControllerPose != CurrentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePoseChanged(InputSource, this, CurrentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    InputSystem?.RaiseSourcePositionChanged(InputSource, this, CurrentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    InputSystem?.RaiseSourceRotationChanged(InputSource, this, CurrentControllerRotation);
                }
            }
        }
    }
}