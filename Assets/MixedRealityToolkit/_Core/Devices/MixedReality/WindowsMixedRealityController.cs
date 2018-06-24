// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WindowsMixedRealityController : BaseController
    {
        public bool IsControllerTracked { get; private set; }

        private SixDof currentPointerData = new SixDof(Vector3.zero, Quaternion.identity);
        private SixDof currentGripData = new SixDof(Vector3.zero, Quaternion.identity);

        #region IMixedRealityController Interface Members

        public WindowsMixedRealityController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, List<IMixedRealityInteractionMapping> interactions = null)
                : base(controllerState, controllerHandedness, inputSource, interactions)
        {
            IsControllerTracked = false;
        }

        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void SetupFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);

            MixedRealityControllerMapping controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController), ControllerHandedness);
            if (controllerMapping.Interactions?.Length > 0)
            {
                SetupFromMapping(controllerMapping.Interactions);
            }
            else
            {
                SetupWMRControllerDefaults(interactionSourceState);
            }
        }

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        private void SetupFromMapping(IMixedRealityInteractionMapping[] mappings)
        {
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                //Interactions.Add(mappings[i].InputType, new MixedRealityInteractionMapping(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
            }
        }

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Controller.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; private set; }

        /// <summary>
        /// Create Interaction mappings from a device specific default set of action mappings
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void SetupWMRControllerDefaults(InteractionSourceState interactionSourceState)
        {
            InputAction[] inputActions = Managers.MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions;
            if (inputActions == null)
            {
                return;
            }

            // TODO Get from Managers.MixedRealityManager.Instance.ActiveProfile.ControllersProfile.MixedRealityControllerMappingProfiles

            ////Add the Controller Pointer
            //Interactions.Add(DeviceInputType.SpatialPointer, new InteractionMapping(1, AxisType.SixDof, DeviceInputType.SpatialPointer, inputActions.GetActionByName("Select")));

            //// Add the Controller trigger
            //Interactions.Add(DeviceInputType.Trigger, new InteractionMapping(2, AxisType.SingleAxis, DeviceInputType.Trigger, inputActions.GetActionByName("Select")));

            //// If the controller has a Grip / Grasp button, add it to the controller capabilities
            //if (interactionSourceState.source.supportsGrasp)
            //{
            //    Interactions.Add(DeviceInputType.SpatialGrip, new InteractionMapping(3, AxisType.SixDof, DeviceInputType.SpatialGrip, inputActions.GetActionByName("Grip")));

            //    Interactions.Add(DeviceInputType.GripPress, new InteractionMapping(4, AxisType.SingleAxis, DeviceInputType.GripPress, inputActions.GetActionByName("Grab")));
            //}

            //// If the controller has a menu button, add it to the controller capabilities
            //if (interactionSourceState.source.supportsMenu)
            //{
            //    Interactions.Add(DeviceInputType.Menu, new InteractionMapping(5, AxisType.Digital, DeviceInputType.Menu, inputActions.GetActionByName("Menu")));
            //}

            //// If the controller has a Thumbstick, add it to the controller capabilities
            //if (interactionSourceState.source.supportsThumbstick)
            //{
            //    Interactions.Add(DeviceInputType.ThumbStick, new InteractionMapping(6, AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerHandedness == Handedness.Left ? inputActions.GetActionByName("Walk") : inputActions.GetActionByName("Look")));
            //    Interactions.Add(DeviceInputType.ThumbStickPress, new InteractionMapping(7, AxisType.Digital, DeviceInputType.ThumbStickPress, inputActions.GetActionByName("Interact")));
            //}

            //// If the controller has a Touchpad, add it to the controller capabilities
            //if (interactionSourceState.source.supportsTouchpad)
            //{
            //    Interactions.Add(DeviceInputType.Touchpad, new InteractionMapping(8, AxisType.DualAxis, DeviceInputType.Touchpad, inputActions.GetActionByName("Inventory")));
            //    Interactions.Add(DeviceInputType.TouchpadTouch, new InteractionMapping(9, AxisType.Digital, DeviceInputType.TouchpadTouch, inputActions.GetActionByName("Pickup")));
            //    Interactions.Add(DeviceInputType.TouchpadPress, new InteractionMapping(10, AxisType.Digital, DeviceInputType.TouchpadPress, inputActions.GetActionByName("Pickup")));
            //}
        }

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(InteractionSourceState interactionSourceState)
        {
            UpdateControllerData(interactionSourceState);

            for (int i = 0; i < Interactions.Count; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                    case DeviceInputType.PointerPosition:
                    case DeviceInputType.PointerRotation:
                        UpdatePointerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.GripPosition:
                    case DeviceInputType.GripRotation:
                    case DeviceInputType.GripPress:
                        UpdateGripData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                    case DeviceInputType.ButtonPress:
                        {
                            Interactions[i].SetValue(interactionSourceState.menuPressed);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            LastSourceStateReading = interactionSourceState;

            // Get Controller start position and tracked state
            ControllerState = IsControllerTracked ? ControllerState.Tracked : ControllerState.NotTracked;
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            Vector3 position;
            Quaternion rotation;

            interactionSourceState.sourcePose.TryGetPosition(out position, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out rotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                position = CameraCache.Main.transform.parent.TransformPoint(position);
                rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(rotation.eulerAngles));
            }

            currentPointerData.Position = position;
            currentPointerData.Rotation = rotation;
            interactionMapping.SetValue(currentPointerData);
            InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, currentPointerData);
            //Interactions.SetDictionaryValue(DeviceInputType.SpatialPointer, currentPointerData);
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            Vector3 position;
            Quaternion rotation;

            interactionSourceState.sourcePose.TryGetPosition(out position, InteractionSourceNode.Grip);
            interactionSourceState.sourcePose.TryGetRotation(out rotation, InteractionSourceNode.Grip);

            if (CameraCache.Main.transform.parent != null)
            {
                position = CameraCache.Main.transform.parent.TransformPoint(position);
                rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(rotation.eulerAngles));
            }

            currentGripData.Position = position;
            currentGripData.Rotation = rotation;

            InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, currentGripData);
            //Interactions.SetDictionaryValue(DeviceInputType.SpatialGrip, currentGripData);
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTouchPadData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        interactionMapping.SetValue(interactionSourceState.touchpadTouched);

                        if (interactionSourceState.touchpadTouched)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        interactionMapping.SetValue(interactionSourceState.touchpadPressed);

                        if (interactionSourceState.touchpadPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        interactionMapping.SetValue(interactionSourceState.touchpadPosition);
                        InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.touchpadPosition);
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
        private void UpdateThumbStickData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        interactionMapping.SetValue(interactionSourceState.thumbstickPressed);

                        if (interactionSourceState.thumbstickPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        interactionMapping.SetValue(interactionSourceState.thumbstickPosition);
                        InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.thumbstickPosition);
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
        private void UpdateTriggerData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Select:
                case DeviceInputType.TriggerPress:
                    {
                        interactionMapping.SetValue(interactionSourceState.selectPressed);

                        if (interactionSourceState.selectPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        interactionMapping.SetValue(interactionSourceState.selectPressedAmount);
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.selectPressedAmount);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion Update data functions
    }
}