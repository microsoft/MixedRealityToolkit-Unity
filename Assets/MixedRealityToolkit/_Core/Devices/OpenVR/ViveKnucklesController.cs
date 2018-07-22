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
    public class ViveKnucklesController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public ViveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        //TODO - Update defaults
        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public static readonly MixedRealityInteractionMapping[] DefaultViveKnucklesInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Grip Press",AxisType.Digital, DeviceInputType.GripPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(3, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(6, "Thumbstick Press ", AxisType.Digital, DeviceInputType.ThumbStickPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(7, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(8, "Trigger Pressed (Select)",AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(9, "Trigger Touched", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Menu Pressed", AxisType.Digital, DeviceInputType.Menu, MixedRealityInputAction.None),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultViveKnucklesInteractions);
        }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerMappingLibrary.GetInputManagerMappings(GetType().FullName);

        #endregion Base override configuration

        #region Update data functions

        /// <inheritdoc />
        public override void UpdateController(XRNodeState xrNodeState)
        {
            UpdateControllerData(xrNodeState);

            Debug.Assert(Interactions != null, "No interaction configuration for controller");
            if (Interactions == null) Enabled = false;

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
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(Interactions[i]);
                        break;
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
                    case DeviceInputType.ButtonPress:
                    case DeviceInputType.SecondaryButtonPress:
                        UpdateButtonData(Interactions[i]);
                        break;
                    case DeviceInputType.IndexFinger:
                    case DeviceInputType.MiddleFinger:
                    case DeviceInputType.RingFinger:
                    case DeviceInputType.PinkyFinger:
                        UpdateFingerData(Interactions[i]);
                        break;
                    default:
                        Debug.LogError($"Input [{Interactions[i].InputType}] is not handled for this controller [ViveKnucklesController]");
                        Enabled = false;
                        break;
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
                    //Get the current Trigger axis state - ** Does not WORK
                    var IndexFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    //Update the interaction data source
                    interactionMapping.FloatData = IndexFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, IndexFingerAxis);
                    }
                    break;
                case DeviceInputType.MiddleFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var MiddleFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[14]) : Input.GetAxis(VRInputMappings[15]);

                    //Update the interaction data source
                    interactionMapping.FloatData = MiddleFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, MiddleFingerAxis);
                    }
                    break;
                case DeviceInputType.RingFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var RingFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                    //Update the interaction data source
                    interactionMapping.FloatData = RingFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, RingFingerAxis);
                    }
                    break;
                case DeviceInputType.PinkyFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var PinkyFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[18]) : Input.GetAxis(VRInputMappings[19]);

                    //Update the interaction data source
                    interactionMapping.FloatData = PinkyFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, PinkyFingerAxis);
                    }
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <inheritdoc />
        protected override void UpdateButtonData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ButtonPress:
                    //Get the current Menu button state
                    var primaryButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton2) : Input.GetKey(KeyCode.JoystickButton0);
                    //Update the interaction data source
                    interactionMapping.BoolData = primaryButton;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        if (primaryButton)
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
                    //Get the current Menu button state
                    var secondaryButton = ControllerHandedness == Handedness.Left ? Input.GetKey(KeyCode.JoystickButton3) : Input.GetKey(KeyCode.JoystickButton1);
                    //Update the interaction data source
                    interactionMapping.BoolData = secondaryButton;

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
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion Update data functions
    }
}
