// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public class OculusTouchController : GenericOpenVRController
    {
        public OculusTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerInputAxisMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerInputAxisMappingLibrary.GetInputManagerMappings(GetType().FullName);

        #endregion Base override configuration

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
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
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.TriggerNearTouch:
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
                    case DeviceInputType.ThumbStickNearTouch:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                    case DeviceInputType.TouchpadNearTouch:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonPress:
                    case DeviceInputType.ButtonTouch:
                    case DeviceInputType.SecondaryButtonPress:
                    case DeviceInputType.SecondaryButtonTouch:
                    case DeviceInputType.Start:
                    case DeviceInputType.ThumbTouch:
                    case DeviceInputType.ThumbNearTouch:
                        UpdateButtonData(Interactions[i]);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Input [{Interactions[i].InputType}] is not handled for this controller [OculusTouchController]");
                }
            }
            LastStateReading = xrNodeState;
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected override void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateTouchPadData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadNearTouch:
                    {
                        var touchpadTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(touchpadTouch == 1);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (touchpadTouch == 1)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
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
        protected override void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateThumbStickData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickNearTouch:
                    {
                        var thumbstickTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(thumbstickTouch == 1);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (thumbstickTouch == 1)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
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
        protected override void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateTriggerData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerNearTouch:
                    {
                        //Get the current Trigger button state
                        var triggerButton = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[14]) : Input.GetAxis(VRInputMappings[15]);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(triggerButton == 1);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (triggerButton == 1)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
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
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateButtonData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ButtonPress:
                    {
                        //Get the current Trigger button state
                        var buttonPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(buttonPress);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (buttonPress)
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
                case DeviceInputType.ButtonTouch:
                    {
                        //Get the current Trigger button state
                        var buttonTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton12) : Input.GetKey(KeyCode.JoystickButton10);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(buttonTouch);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (buttonTouch)
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
                case DeviceInputType.SecondaryButtonPress:
                    {
                        //Get the current Trigger button state
                        var secondaryButtonPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton3) : Input.GetKey(KeyCode.JoystickButton1);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(secondaryButtonPress);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (secondaryButtonPress)
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
                case DeviceInputType.SecondaryButtonTouch:
                    {
                        //Get the current Trigger button state
                        var SecondaryButtonTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton13) : Input.GetKey(KeyCode.JoystickButton11);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(SecondaryButtonTouch);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (SecondaryButtonTouch)
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
                case DeviceInputType.Start:
                    {
                        //Get the current Trigger button state
                        var startButton = Input.GetKey(KeyCode.JoystickButton7);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(startButton);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (startButton)
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
                case DeviceInputType.ThumbTouch:
                    {
                        //Get the current Trigger button state
                        var thumbPressTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton18) : Input.GetKey(KeyCode.JoystickButton19);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(thumbPressTouch);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (thumbPressTouch)
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
                case DeviceInputType.ThumbNearTouch:
                    {
                        //Get the current Trigger button state
                        var thumbNearTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                        //Update the interaction data source
                        interactionMapping.SetBoolValue(thumbNearTouch == 1);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (thumbNearTouch == 1)
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
                    break;
            }
        }

        #endregion Update data functions

    }
}
