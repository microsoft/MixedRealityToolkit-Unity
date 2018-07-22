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
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        //TODO - Update defaults
        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public static readonly MixedRealityInteractionMapping[] DefaultOculusTouchInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Trigger Near Touch",AxisType.Digital, DeviceInputType.TriggerNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trigger Touched", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trigger Selected", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Grip",AxisType.SingleAxis, DeviceInputType.Grip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Grip Press",AxisType.Digital, DeviceInputType.GripPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Grip Touch",AxisType.Digital, DeviceInputType.GripTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbstick Touch ", AxisType.Digital, DeviceInputType.ThumbStickTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbstick Near Touch ", AxisType.Digital, DeviceInputType.ThumbStickNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbstick Press ", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Primary Button Touch ", AxisType.Digital, DeviceInputType.ButtonTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Primary Button Press ", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Secondary Button Touch ", AxisType.Digital, DeviceInputType.SecondaryButtonTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Secondary Button Press ", AxisType.Digital, DeviceInputType.SecondaryButtonPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbrest Near Touch ", AxisType.Digital, DeviceInputType.ThumbNearTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Thumbrest Touch ", AxisType.Digital, DeviceInputType.ThumbTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Start Pressed", AxisType.Digital, DeviceInputType.Start, MixedRealityInputAction.None),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultOculusTouchInteractions);
        }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerMappingLibrary.GetInputManagerMappings(GetType().FullName);

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
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.TriggerNearTouch:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
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
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [OculusTouchController]");
                        Enabled = false;
                        break;
                }
            }
            LastStateReading = xrNodeState;
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
                    var thumbstickTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    //Update the interaction data source
                    interactionMapping.BoolData = thumbstickTouch.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (thumbstickTouch.Equals(1))
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
                    //Get the current Trigger button state
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
                    break;
                case DeviceInputType.SecondaryButtonPress:
                    //Get the current Trigger button state
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
                    break;
                case DeviceInputType.SecondaryButtonTouch:
                    //Get the current Trigger button state
                    var SecondaryButtonTouch = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton13) : Input.GetKey(KeyCode.JoystickButton11);

                    //Update the interaction data source
                    interactionMapping.BoolData = SecondaryButtonTouch;

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
                case DeviceInputType.ThumbTouch:
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
                case DeviceInputType.ThumbNearTouch:
                    //Get the current Trigger button state
                    var thumbNearTouch = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                    //Update the interaction data source
                    interactionMapping.BoolData = thumbNearTouch.Equals(1);

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (thumbNearTouch.Equals(1))
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

        #endregion Update data functions

    }
}
