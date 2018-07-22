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
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public GenericOpenVRController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        /// <summary>
        /// The current source state reading for this OpenVR Controller.
        /// </summary>
        public XRNodeState LastStateReading;

        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;

        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        //TODO - Update defaults
        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public static readonly MixedRealityInteractionMapping[] DefaultOpenVRInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press",AxisType.Digital, DeviceInputType.GripPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbstick Press ", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Trigger Pressed (Select)",AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trigger Touched", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Menu Pressed", AxisType.Digital, DeviceInputType.Menu, MixedRealityInputAction.None),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultOpenVRInteractions);
        }

        #region Base override configuration

        /// <summary>
        /// Mapping method to expose the Unity Input Manager mapping configuration
        /// </summary>
        public override InputManagerAxis[] ControllerAxisMappings => ControllerMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <summary>
        /// Mapping method to expose this controllers Unity Input Manager mapping array
        /// </summary>
        public virtual string[] VRInputMappings => ControllerMappingLibrary.GetInputManagerMappings(GetType().FullName);

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
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.GripPosition:
                    case DeviceInputType.GripRotation:
                    case DeviceInputType.Grip:
                    case DeviceInputType.GripPress:
                    case DeviceInputType.GripTouch:
                        UpdateGripData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickTouch:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                        UpdateButtonData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [GenericOpenVRController]");
                        Enabled = false;
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
            interactionMapping.PoseData = currentPointerPose = currentControllerPose; // Currently no way to get pointer specific data, so we use the last controller pose.

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
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateGripData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.AxisType)
            {
                case AxisType.Digital:
                    var gripButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton4) : Input.GetKey(KeyCode.JoystickButton5);
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.GripPress:
                            //Update the interaction data source
                            interactionMapping.BoolData = gripButton;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                //Raise input system Event if it enabled
                                if (gripButton)
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
                    break;
                case AxisType.SingleAxis:
                    var gripButtonPress = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[10]) : Input.GetAxis(VRInputMappings[11]);
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.GripPress:
                            //Update the interaction data source
                            interactionMapping.BoolData = gripButtonPress.Equals(1);

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                //Raise input system Event if it enabled
                                if (gripButtonPress.Equals(1))
                                {
                                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                                else
                                {
                                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                                }
                            }
                            break;
                        case DeviceInputType.Grip:
                            //Update the interaction data source
                            interactionMapping.FloatData = gripButtonPress;

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                //Raise input system Event if it enabled
                                InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, gripButtonPress);
                            }
                            break;
                        case DeviceInputType.GripTouch:
                            //Update the interaction data source
                            interactionMapping.BoolData = (gripButtonPress > 0 && gripButtonPress < 0.1);

                            // If our value changed raise it.
                            if (interactionMapping.Changed)
                            {
                                //Raise input system Event if it enabled
                                if (gripButtonPress > 0)
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
                    break;
            }
            //Get the current Grip button state
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    //Get the current Touchpad button Touch state
                    var touchpadTouchButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton16) : Input.GetKey(KeyCode.JoystickButton17);

                    //Update the interaction data source
                    interactionMapping.BoolData = touchpadTouchButton;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
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
                case DeviceInputType.TouchpadPress:
                    //Get the current Touchpad button state
                    var touchpadPressButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                    //Update the interaction data source
                    interactionMapping.BoolData = touchpadPressButton;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
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
                case DeviceInputType.Touchpad:
                    var touchpadX = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[0]) : Input.GetAxis(VRInputMappings[2]);
                    var touchpadY = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[1]) : Input.GetAxis(VRInputMappings[3]);
                    var touchpadPosition = new Vector2(touchpadX, touchpadY);

                    //Update the interaction data source
                    interactionMapping.Vector2Data = touchpadPosition;

                    //If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, touchpadPosition);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    //Get the current Thumbstick button state
                    var thumbstickButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton8) : Input.GetKey(KeyCode.JoystickButton9);

                    //Update the interaction data source
                    interactionMapping.BoolData = thumbstickButton;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
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
                case DeviceInputType.ThumbStick:
                    var thumbstickX = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[4]) : Input.GetAxis(VRInputMappings[6]);
                    var thumbstickY = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[5]) : Input.GetAxis(VRInputMappings[7]);
                    var thumbstickposition = new Vector2(thumbstickX, thumbstickY);

                    //Update the interaction data source
                    interactionMapping.Vector2Data = thumbstickposition;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, thumbstickposition);
                    }
                    break;
                case DeviceInputType.ThumbStickTouch:
                    //Get the current Thumbstick button state
                    var thumbstickTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton16) : Input.GetKey(KeyCode.JoystickButton17);

                    //Update the interaction data source
                    interactionMapping.BoolData = thumbstickTouch;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (thumbstickTouch)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                case DeviceInputType.Select:
                    //Get the current Trigger axis state - ** Does not WORK
                    var triggerSelect = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[8]) : Input.GetAxis(VRInputMappings[9]);

                    //Update the interaction data source
                    interactionMapping.BoolData = triggerSelect > 0;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (triggerSelect > 0)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                case DeviceInputType.TriggerTouch:
                    //Get the current Trigger button state
                    var triggerTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton14) : Input.GetKey(KeyCode.JoystickButton15);

                    //Update the interaction data source
                    interactionMapping.BoolData = triggerTouch;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (triggerTouch)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                case DeviceInputType.Trigger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var triggerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[8]) : Input.GetAxis(VRInputMappings[9]);

                    //Update the interaction data source
                    interactionMapping.FloatData = triggerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, triggerAxis);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Update the buttons state.
        /// </summary>
        /// <param name="interactionSourceState"></param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            //Get the current Menu button state
            var menuButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);

            //Update the interaction data source
            interactionMapping.BoolData = menuButton;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
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
    }
}