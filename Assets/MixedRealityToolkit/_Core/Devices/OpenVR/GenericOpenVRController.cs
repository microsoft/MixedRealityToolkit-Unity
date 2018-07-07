// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    // TODO - Implement
    public class GenericOpenVRController : BaseController
    {
        public GenericOpenVRController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            //Verify the OpenVR Controller mappings are loaded
            InputMappingAxisUtility.ApplyMappings(OpenVRControllerAxisMappings);
        }

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastStateReading;

        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose currentControllerData = MixedRealityPose.ZeroIdentity;

        //TODO - Do we need a pointer Pose simulator?
        //private Vector3 currentPointerPosition = Vector3.zero;
        //private Quaternion currentPointerRotation = Quaternion.identity;
        //private MixedRealityPose currentPointerData = MixedRealityPose.ZeroIdentity;


        //TODO - Do we need a Grip Pose simulator?
        //private Vector3 currentGripPosition = Vector3.zero;
        //private Quaternion currentGripRotation = Quaternion.identity;
        //private MixedRealityPose currentGripData = MixedRealityPose.ZeroIdentity;

        private InputMappingAxisUtility.InputManagerAxis[] OpenVRControllerAxisMappings =
        {
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 1 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 2 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 4 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 5 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TRIGGER_LEFT_CONTROLLER,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 9 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_TRIGGER_RIGHT_CONTROLLER,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 10 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_GRIP_LEFT_CONTROLLER,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 11 },
            new InputMappingAxisUtility.InputManagerAxis() { Name = OpenVRInputMappings.OPENVR_GRIP_RIGHT_CONTROLLER,    Dead = 0.001f, Sensitivity = 1, Invert = false,  Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 12 }
        };


        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        public void UpdateController(XRNodeState xrNodeState)
        {
            Debug.Assert(Interactions != null);
            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    case DeviceInputType.SpatialPointer:
                    case DeviceInputType.PointerPosition:
                    case DeviceInputType.PointerRotation:
                        UpdateControllerData(xrNodeState, Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.GripPosition:
                    case DeviceInputType.GripRotation:
                    case DeviceInputType.GripPress:
                        UpdateGripData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                        UpdateMenuData(Interactions[i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            LastStateReading = xrNodeState;
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(XRNodeState State, MixedRealityInteractionMapping interactionMapping)
        {
            var lastState = TrackingState;

            XRNode nodeType = State.nodeType;
            if (nodeType == XRNode.LeftHand || nodeType == XRNode.RightHand)
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = State.TryGetPosition(out currentControllerPosition);
                IsPositionApproximate = false;

                IsRotationAvailable = State.TryGetRotation(out currentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;

                if (CameraCache.Main.transform.parent != null)
                {
                    currentControllerData.Position = CameraCache.Main.transform.parent.TransformPoint(currentControllerPosition);
                    currentControllerData.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentControllerRotation.eulerAngles));
                }
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            //Update the interaction data source
            interactionMapping.SetPoseValue(currentControllerData);

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                //Raise input system Event if it enabled
                InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentControllerData);
            }

            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(MixedRealityInteractionMapping interactionMapping)
        {
            //Get the current Grip button state
            var gripButton = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_GRIP_LEFT_CONTROLLER) : Input.GetAxis(OpenVRInputMappings.OPENVR_GRIP_RIGHT_CONTROLLER);
            switch (interactionMapping.AxisType)
            {
                case AxisType.Digital:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(gripButton > 0);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Grip pressed for [{ControllerHandedness}] hand, set to [{gripButton}]");

                            //Raise input system Event if it enabled
                            if (gripButton > 0)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case AxisType.SingleAxis:
                    {
                        //Update the interaction data source
                        interactionMapping.SetFloatValue(gripButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Grip pressed for [{ControllerHandedness}] hand, set to [{gripButton}]");

                            if (gripButton > 0)
                            {
                                //Raise input system Event if it enabled
                                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, gripButton);
                            }
                            else
                            {
                                //TODO - Needs InputReleased
                                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, gripButton);
                            }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        //Get the current Touchpad button Touch state
                        var touchpadTouchButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton16) : Input.GetKey(KeyCode.JoystickButton17);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(touchpadTouchButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"TouchpadTouch pressed for [{ControllerHandedness}] hand, set to [{touchpadTouchButton}]");

                            //Raise input system Event if it enabled
                            if (touchpadTouchButton)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        //Get the current Touchpad button state
                        var touchpadPressButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(touchpadPressButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"TouchpadPress pressed for [{ControllerHandedness}] hand, set to [{touchpadPressButton}]");

                            //Raise input system Event if it enabled
                            if (touchpadPressButton)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        var touchpadX = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL): Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL);
                        var touchpadY = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL): Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL);
                        var touchpadPosition = new Vector2(touchpadX, touchpadY);

                        //Update the interaction data source
                        interactionMapping.SetVector2Value(touchpadPosition);

                        //If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Touchpad pressed for [{ControllerHandedness}] hand, set to [{touchpadPosition}]");

                            //Raise input system Event if it enabled
                            InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, touchpadPosition);
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        //Get the current Thumbstick button state
                        var thumbstickButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(thumbstickButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"ThumbStickPress pressed for [{ControllerHandedness}] hand, set to [{thumbstickButton}]");

                            //Raise input system Event if it enabled
                            if (thumbstickButton)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        var thumbstickX = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL) : Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL);
                        var thumbstickY = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL) : Input.GetAxis(OpenVRInputMappings.OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL);
                        var thumbstickposition = new Vector2(thumbstickX, thumbstickY);

                        //Update the interaction data source
                        interactionMapping.SetVector2Value(thumbstickposition);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Thumbstick pressed for [{ControllerHandedness}] hand, set to [{thumbstickposition}]");

                            //Raise input system Event if it enabled
                            InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, thumbstickposition);
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                case DeviceInputType.Select:
                    {
                        //Get the current Trigger button state
                        var triggerButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton14) : Input.GetKey(KeyCode.JoystickButton15);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(triggerButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Trigger pressed for [{ControllerHandedness}] hand, set to [{triggerButton}]");

                            //Raise input system Event if it enabled
                            if (triggerButton)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        //Get the current Trigger axis state - ** Does not WORK
                        var triggerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(OpenVRInputMappings.OPENVR_TRIGGER_LEFT_CONTROLLER) : Input.GetAxis(OpenVRInputMappings.OPENVR_TRIGGER_RIGHT_CONTROLLER);

                        //Update the interaction data source
                        interactionMapping.SetFloatValue(triggerAxis);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            Debug.LogWarning($"Trigger pressed for [{ControllerHandedness}] hand, set to [{triggerAxis}] value");

                            if (triggerAxis > 0)
                            {
                                //Raise input system Event if it enabled
                                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, triggerAxis);
                            }
                            else
                            {
                                //TODO Needs "Released Event"
                                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, triggerAxis);
                            }
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Menu button state.
        /// </summary>
        /// <param name="interactionSourceState"></param>
        /// <param name="interactionMapping"></param>
        private void UpdateMenuData(MixedRealityInteractionMapping interactionMapping)
        {
            //Get the current Menu button state
            var menuButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);
            //Update the interaction data source
            interactionMapping.SetBoolValue(menuButton);

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                Debug.LogWarning($"Menu pressed for [{ControllerHandedness}] hand, set to [{menuButton}]");

                //Raise input system Event if it enabled
                if (menuButton)
                {
                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Update data functions

        #region Nested Types
        private class OpenVRInputMappings
        {
            public const string OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL";
            public const string OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL = "OPENVR_TOUCHPAD_LEFT_CONTROLLER_VERTICAL";
            public const string OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL";
            public const string OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL = "OPENVR_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL";
            public const string OPENVR_TRIGGER_LEFT_CONTROLLER = "OPENVR_TRIGGER_LEFT_CONTROLLER";
            public const string OPENVR_TRIGGER_RIGHT_CONTROLLER = "OPENVR_TRIGGER_RIGHT_CONTROLLER";
            public const string OPENVR_GRIP_LEFT_CONTROLLER = "OPENVR_GRIP_LEFT_CONTROLLER";
            public const string OPENVR_GRIP_RIGHT_CONTROLLER = "OPENVR_GRIP_RIGHT_CONTROLLER";
        }
        #endregion
    }
}
