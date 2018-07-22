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
    public class ValveKnucklesController : GenericOpenVRController
    {
        public ValveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerInputAxisMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerInputAxisMappingLibrary.GetInputManagerMappings(GetType().FullName);

        public new static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Grip Average", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(13, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(14, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton3),
            new MixedRealityInteractionMapping(20, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(21, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(22, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(23, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, MixedRealityInputAction.None),
        };

        public new static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(4, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(5, "Grip Average", AxisType.SingleAxis, DeviceInputType.TriggerPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(10, "Trackpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(11, "Trackpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(12, "Trackpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(13, "Inner Face Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(14, "Outer Face Button", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(19, "Index Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.IndexFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(20, "Middle Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.MiddleFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(21, "Ring Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.RingFinger, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(22, "Pinky Finger Cap Sensor", AxisType.SingleAxis, DeviceInputType.PinkyFinger, MixedRealityInputAction.None),
        };

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
                    case DeviceInputType.PointerPosition:
                    case DeviceInputType.PointerRotation:
                        UpdatePointerData(Interactions[i]);
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
                    case DeviceInputType.ButtonPress:
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

        protected void UpdateFingerData(MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.IndexFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var indexFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[12]) : Input.GetAxis(VRInputMappings[13]);

                    //Update the interaction data source
                    interactionMapping.FloatData = indexFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, indexFingerAxis);
                    }
                    break;
                case DeviceInputType.MiddleFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var middleFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[14]) : Input.GetAxis(VRInputMappings[15]);

                    //Update the interaction data source
                    interactionMapping.FloatData = middleFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, middleFingerAxis);
                    }
                    break;
                case DeviceInputType.RingFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var ringFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[16]) : Input.GetAxis(VRInputMappings[17]);

                    //Update the interaction data source
                    interactionMapping.FloatData = ringFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, ringFingerAxis);
                    }
                    break;
                case DeviceInputType.PinkyFinger:
                    //Get the current Trigger axis state - ** Does not WORK
                    var pinkyFingerAxis = ControllerHandedness == Handedness.Left ? Input.GetAxis(VRInputMappings[18]) : Input.GetAxis(VRInputMappings[19]);

                    //Update the interaction data source
                    interactionMapping.FloatData = pinkyFingerAxis;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, pinkyFingerAxis);
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
                    //Get the current primary button state
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

                    //Get the current secondary button state
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
