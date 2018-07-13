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
    public class ValveKnucklesController : GenericOpenVRController
    {
        public ValveKnucklesController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        /// <inheritdoc />
        public override InputManagerAxis[] ControllerAxisMappings => ControllerInputAxisMappingLibrary.GetInputManagerAxes(GetType().FullName);

        /// <inheritdoc />
        public override string[] VRInputMappings => ControllerInputAxisMappingLibrary.GetInputManagerMappings(GetType().FullName);

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
