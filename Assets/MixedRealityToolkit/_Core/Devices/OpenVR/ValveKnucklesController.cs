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
    public class ValveKnucklesController : GenericOpenVRController
    {
        public ValveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        private InputMappingAxisUtility.InputManagerAxis[] ValveKnucklesControllerAxisMappings;

        /// <inheritdoc />
        public override InputMappingAxisUtility.InputManagerAxis[] ControllerAxisMappings => ValveKnucklesControllerAxisMappings;

        /// <summary>
        /// Collection of input mapping constants, grouped in a single class for easier referencing.
        /// </summary>
        /// <remarks>
        /// Uses a fixed index array for controller input in the base / Generic class, as indicated in the array comments</remarks>
        private string[] ValveKnucklesInputMappings =
        {
            "VKNUCKLES_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 0 - TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL
            "VKNUCKLES_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 1 - TOUCHPAD_LEFT_CONTROLLER_VERTICAL
            "VKNUCKLES_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 2 - TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL
            "VKNUCKLES_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 3 - TOUCHPAD_RIGHT_CONTROLLER_VERTICAL
            "VKNUCKLES_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 4 - THUMBSTICK_LEFT_CONTROLLER_HORIZONTAL
            "VKNUCKLES_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 5 - THUMBSTICK_LEFT_CONTROLLER_VERTICAL
            "VKNUCKLES_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 6 - THUMBSTICK_RIGHT_CONTROLLER_HORIZONTAL
            "VKNUCKLES_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 7 - THUMBSTICK_RIGHT_CONTROLLER_VERTICAL
            "VKNUCKLES_TRIGGER_LEFT_CONTROLLER",               // 8 - TRIGGER_LEFT_CONTROLLER
            "VKNUCKLES_TRIGGER_RIGHT_CONTROLLER",              // 9 - TRIGGER_RIGHT_CONTROLLER
            "VKNUCKLES_GRIP_LEFT_CONTROLLER",                  // 10 - GRIP_LEFT_CONTROLLER
            "VKNUCKLES_GRIP_RIGHT_CONTROLLER",                 // 11 - GRIP_RIGHT_CONTROLLER
            "VKNUCKLES_INDEXFINGER_LEFT_CONTROLLER",           // 12 - INDEXFINGER_LEFT_CONTROLLER
            "VKNUCKLES_INDEXFINGER_RIGHT_CONTROLLER",          // 13 - INDEXFINGER_RIGHT_CONTROLLER
            "VKNUCKLES_MIDDLEFINGER_LEFT_CONTROLLER",          // 14 - MIDDLEFINGER_LEFT_CONTROLLER
            "VKNUCKLES_MIDDLEFINGER_RIGHT_CONTROLLER",         // 15 - MIDDLEFINGER_RIGHT_CONTROLLER
            "VKNUCKLES_RINGFINGER_LEFT_CONTROLLER",            // 16 - RINGFINGER_LEFT_CONTROLLER
            "VKNUCKLES_RINGFINGER_RIGHT_CONTROLLER",           // 17 - RINGFINGER_RIGHT_CONTROLLER
            "VKNUCKLES_PINKYFINGER_LEFT_CONTROLLER",           // 18 - PINKYFINGER_LEFT_CONTROLLER
            "VKNUCKLES_PINKYFINGER_RIGHT_CONTROLLER",          // 19 - PINKYFINGER_RIGHT_CONTROLLER
        };

        /// <inheritdoc />
        public override string[] VRInputMappings => ValveKnucklesInputMappings;

        /// <inheritdoc />
        public override void Initialise()
        {
            ValveKnucklesControllerAxisMappings = new InputMappingAxisUtility.InputManagerAxis[]
            {
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[0], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 1 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[1], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 2 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[2], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 4 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[3], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 5 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[8], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 9 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[9], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 10 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[10], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 11 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[11], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 12 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[12], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 20 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[13], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 21 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[14], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 22 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[15], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 23 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[16], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 24 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[17], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 25 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[18], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 26 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[19], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 27 }
            };
        }

        #endregion Base override configuration

        #region Update data functions

        /// <inheritdoc />
        public override void UpdateController(XRNodeState xrNodeState)
        {
            Debug.Assert(Interactions != null, "No interaction configuration for controller");
            if (Interactions == null) Enabled = false;

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
                    case DeviceInputType.SecondaryButton:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case DeviceInputType.IndexFinger:
                    case DeviceInputType.MiddleFinger:
                    case DeviceInputType.RingFinger:
                    case DeviceInputType.PinkyFinger:
                        UpdateFingerData(Interactions[i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            LastStateReading = xrNodeState;
        }

        /// <inheritdoc />
        protected void UpdateFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.IndexFinger:
                    {
                        //Get the current Trigger axis state - ** Does not WORK
                        var IndexFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                        //Update the interaction data source
                        interactionMapping.SetFloatValue(IndexFingerAxis);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, IndexFingerAxis);
                        }
                        break;
                    }
                case DeviceInputType.MiddleFinger:
                    {
                        //Get the current Trigger axis state - ** Does not WORK
                        var MiddleFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[14]) : Input.GetAxis(VRInputMappings[15]);

                        //Update the interaction data source
                        interactionMapping.SetFloatValue(MiddleFingerAxis);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, MiddleFingerAxis);
                        }
                        break;
                    }
                case DeviceInputType.RingFinger:
                    {
                        //Get the current Trigger axis state - ** Does not WORK
                        var RingFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                        //Update the interaction data source
                        interactionMapping.SetFloatValue(RingFingerAxis);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, RingFingerAxis);
                        }
                        break;
                    }
                case DeviceInputType.PinkyFinger:
                    {
                        //Get the current Trigger axis state - ** Does not WORK
                        var PinkyFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[18]) : Input.GetAxis(VRInputMappings[19]);

                        //Update the interaction data source
                        interactionMapping.SetFloatValue(PinkyFingerAxis);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, PinkyFingerAxis);
                        }
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <inheritdoc />
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Menu:
                    {
                        //Get the current Menu button state
                        var menuButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(menuButton);

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
                        break;
                    }
                case DeviceInputType.SecondaryButton:
                    {
                        //Get the current Menu button state
                        var secondaryButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton3) : Input.GetKey(KeyCode.JoystickButton1);
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(secondaryButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (secondaryButton)
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
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion Update data functions
    }
}
