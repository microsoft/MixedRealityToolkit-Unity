// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
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

        public new static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Axis1D.PrimaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Axis1D.PrimaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Axis1D.PrimaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Axis1D.PrimaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Axis1D.PrimaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Axis2D.PrimaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Button.PrimaryThumbstick Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Button.PrimaryThumbstick Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Button.PrimaryThumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Button.Three Press", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(11, "Button.Four Press", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(12, "Button.Start Press", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(13, "Button.Three Touch", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton12),
            new MixedRealityInteractionMapping(14, "Button.Four Touch", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton13),
            new MixedRealityInteractionMapping(15, "Touch.PrimaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(16, "Touch.PrimaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
        };

        public new static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Axis1D.SecondaryIndexTrigger", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Axis1D.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Axis1D.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.TriggerNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Axis1D.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Axis1D.SecondaryHandTrigger Press", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Axis2D.SecondaryThumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Button.SecondaryIndexTrigger Touch", AxisType.Digital, DeviceInputType.ThumbStickTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Button.SecondaryIndexTrigger Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Button.SecondaryIndexTrigger Press", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Button.One Press", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(11, "Button.Two Press", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(12, "Button.One Touch", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton10),
            new MixedRealityInteractionMapping(13, "Button.Two Touch", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton11),
            new MixedRealityInteractionMapping(14, "Touch.SecondaryThumbRest Touch", AxisType.Digital, DeviceInputType.ThumbTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(15, "Touch.SecondaryThumbRest Near Touch", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
        };


        #endregion Base override configuration

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        public override void UpdateController(XRNodeState xrNodeState)
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
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
                    case DeviceInputType.GripPress:
                    case DeviceInputType.GripTouch:
                        UpdateGripData(Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickNearTouch:
                    case DeviceInputType.ThumbNearTouch:
                        UpdateThumbStickData(Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                    case DeviceInputType.TouchpadNearTouch:
                        UpdateTouchPadData(Interactions[i]);
                        break;
                    case DeviceInputType.ButtonPress:
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
        /// <param name="interactionMapping"></param>
        protected override void UpdateTouchPadData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateTouchPadData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadNearTouch:
                    var touchpadTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    //Update the interaction data source
                    interactionMapping.BoolData = touchpadTouch.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (touchpadTouch.Equals(1))
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
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected override void UpdateThumbStickData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateThumbStickData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickNearTouch:
                    // Get the primary thumbstick near touch state
                    var thumbstickTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    // Update the interaction data source
                    interactionMapping.BoolData = thumbstickTouch.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (thumbstickTouch.Equals(1))
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }

                    // Get the current secondary thumbstick near touch state
                    var secondaryThumbstickNearTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                    // Update the interaction data source
                    interactionMapping.BoolData = secondaryThumbstickNearTouch.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (secondaryThumbstickNearTouch.Equals(1))
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
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected override void UpdateTriggerData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateTriggerData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerNearTouch:
                    //Get the current Trigger button state
                    var triggerButton = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[14]) : Input.GetAxis(VRInputMappings[15]);

                    //Update the interaction data source
                    interactionMapping.BoolData = triggerButton.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (triggerButton.Equals(1))
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
        }

        /// <summary>
        /// Update the buttons state.
        /// </summary>
        /// <param name="interactionMapping"></param>
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            base.UpdateButtonData(interactionMapping);

            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ButtonPress:
                    // Get button one or Button three depending on the handedness of the controller.
                    var buttonPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);

                    //Update the interaction data source
                    interactionMapping.BoolData = buttonPress;

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

                    // Get button two or Button four depending on the handedness of the controller.
                    var secondaryButtonPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton3) : Input.GetKey(KeyCode.JoystickButton1);

                    //Update the interaction data source
                    interactionMapping.BoolData = secondaryButtonPress;

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

                    // Get the current secondary Trigger button state
                    var secondaryTriggerPress = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton3) : Input.GetKey(KeyCode.JoystickButton1);

                    //Update the interaction data source
                    interactionMapping.BoolData = secondaryTriggerPress;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (secondaryTriggerPress)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                case DeviceInputType.ButtonTouch:
                    //Get the current Trigger button state
                    var buttonTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton12) : Input.GetKey(KeyCode.JoystickButton10);

                    //Update the interaction data source
                    interactionMapping.BoolData = buttonTouch;

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

                    //Get the current secondary Trigger button state
                    var secondaryButtonTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton13) : Input.GetKey(KeyCode.JoystickButton11);

                    //Update the interaction data source
                    interactionMapping.BoolData = secondaryButtonTouch;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (secondaryButtonTouch)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                case DeviceInputType.Start:
                    //Get the current Trigger button state
                    var startButton = Input.GetKey(KeyCode.JoystickButton7);

                    //Update the interaction data source
                    interactionMapping.BoolData = startButton;

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
                case DeviceInputType.ThumbStickTouch:
                    //Get the current Trigger button state
                    var thumbPressTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton18) : Input.GetKey(KeyCode.JoystickButton19);

                    //Update the interaction data source
                    interactionMapping.BoolData = thumbPressTouch;

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
        }

        #endregion Update data functions

    }
}
