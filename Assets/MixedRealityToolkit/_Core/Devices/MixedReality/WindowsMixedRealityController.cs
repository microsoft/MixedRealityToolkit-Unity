// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public struct WindowsMixedRealityController : IMixedRealityController
    {
        #region Private properties

        private bool controllerTracked;
        private Vector3 controllerPosition;
        private Vector3 pointerPosition;
        private Vector3 gripPosition;
        private Quaternion controllerRotation;
        private Quaternion pointerRotation;
        private Quaternion gripRotation;

        #endregion Private properties

        public WindowsMixedRealityController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, Dictionary<DeviceInputType, IInteractionMapping> interactions = null) : this()
        {
            ControllerState = controllerState;
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions ?? new Dictionary<DeviceInputType, IInteractionMapping>();

            controllerTracked = false;
            controllerPosition = pointerPosition = gripPosition = Vector3.zero;
            controllerRotation = pointerRotation = gripRotation = Quaternion.identity;
        }

        #region IMixedRealityController Interface Members

        /// <inheritdoc/>
        public ControllerState ControllerState { get; private set; }

        /// <inheritdoc/>
        public Handedness ControllerHandedness { get; }

        /// <inheritdoc/>
        public IMixedRealityInputSource InputSource { get; private set; }

        /// <inheritdoc/>
        public Dictionary<DeviceInputType, IInteractionMapping> Interactions { get; private set; }

        /// <inheritdoc/>
        public void SetupInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            SetupFromInteractionSource(interactionSourceState);
        }

        /// <inheritdoc/>
        public void UpdateInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            UpdateFromInteractionSource(interactionSourceState);
        }

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        /// <summary>
        /// Read the Interaction Source State information and initialize an instance of a Windows Mixed Reality controller
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void SetupFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);

            MixedRealityControllerMapping controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController), ControllerHandedness);
            //MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping<WindowsMixedRealityController>(ControllerHandedness);
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
        private void SetupFromMapping(InteractionMapping[] mappings)
        {
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                switch (mappings[i].AxisType)
                {
                    case AxisType.Digital:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<bool>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.SingleAxis:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<float>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.DualAxis:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<Vector2>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDoFPosition:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<Vector3>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDoFRotation:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<Quaternion>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.SixDoF:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<Tuple<Vector3,Quaternion>>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.None:
                    case AxisType.Raw:
                    default:
                        Interactions.Add(mappings[i].InputType, new InteractionMapping<object>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                }
            }
        }

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
            //Add the Controller Pointer
            Interactions.Add(DeviceInputType.SpatialPointer, new InteractionMapping<Tuple<Vector3, Quaternion>>(1, AxisType.SixDoF, DeviceInputType.SpatialPointer, inputActions.GetActionByName("Select"))); // Note will convert these lookups to indexes

            // Add the Controller trigger
            Interactions.Add(DeviceInputType.Trigger, new InteractionMapping<float>(2, AxisType.SingleAxis, DeviceInputType.Trigger, inputActions.GetActionByName("Select")));

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            if (interactionSourceState.source.supportsGrasp)
            {
                Interactions.Add(DeviceInputType.SpatialGrip, new InteractionMapping<Tuple<Vector3, Quaternion>>(3, AxisType.SixDoF, DeviceInputType.SpatialGrip, inputActions.GetActionByName("Grip")));

                Interactions.Add(DeviceInputType.GripPress, new InteractionMapping<float>(4, AxisType.SingleAxis, DeviceInputType.GripPress, inputActions.GetActionByName("Grab")));
            }

            // If the controller has a menu button, add it to the controller capabilities
            if (interactionSourceState.source.supportsMenu)
            {
                Interactions.Add(DeviceInputType.Menu, new InteractionMapping<bool>(5, AxisType.Digital, DeviceInputType.Menu, inputActions.GetActionByName("Menu")));
            }

            // If the controller has a Thumbstick, add it to the controller capabilities
            if (interactionSourceState.source.supportsThumbstick)
            {
                Interactions.Add(DeviceInputType.ThumbStick, new InteractionMapping<Vector2>(6, AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerHandedness == Handedness.Left ? inputActions.GetActionByName("Walk") : inputActions.GetActionByName("Look")));
                Interactions.Add(DeviceInputType.ThumbStickPress, new InteractionMapping<bool>(7, AxisType.Digital, DeviceInputType.ThumbStickPress, inputActions.GetActionByName("Interact")));
            }

            // If the controller has a Touchpad, add it to the controller capabilities
            if (interactionSourceState.source.supportsTouchpad)
            {
                Interactions.Add(DeviceInputType.Touchpad, new InteractionMapping<Vector2>(8, AxisType.DualAxis, DeviceInputType.Touchpad, inputActions.GetActionByName("Inventory")));
                Interactions.Add(DeviceInputType.TouchpadTouch, new InteractionMapping<bool>(9, AxisType.Digital, DeviceInputType.TouchpadTouch, inputActions.GetActionByName("Pickup")));
                Interactions.Add(DeviceInputType.TouchpadPress, new InteractionMapping<bool>(10, AxisType.Digital, DeviceInputType.TouchpadPress, inputActions.GetActionByName("Pickup")));
            }
        }

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Debug.Assert(interactionSourceState.source.id == SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            // TODO - Do we need Kind?
            //Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            // Update Controller
            // TODO - Controller currently not accepted by InputSystem, only InteractionState captured
            // TODO - May need to be more granular with checks if we are allowing user to configure :S  
            // TODO - Need to think of a better way to validate options, multiple Contains aren't good, maybe an extension?
            UpdateControllerData(interactionSourceState);

            // Update Pointer
            if (Interactions.ContainsKey(DeviceInputType.SpatialPointer)) UpdatePointerData(interactionSourceState);

            // Update Grip
            if (Interactions.ContainsKey(DeviceInputType.SpatialGrip)) UpdateGripData(interactionSourceState);

            // Update Touchpad
            if (Interactions.ContainsKey(DeviceInputType.Touchpad) || Interactions.ContainsKey(DeviceInputType.TouchpadTouch)) UpdateTouchPadData(interactionSourceState);

            // Update Thumbstick
            if (Interactions.ContainsKey(DeviceInputType.Thumb)) UpdateThumbStickData(interactionSourceState);

            // Update Trigger
            if (Interactions.ContainsKey(DeviceInputType.Trigger)) UpdateTriggerData(interactionSourceState);
        }

        #region Update data functions

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            // Get Controller start position and tracked state
            controllerTracked = interactionSourceState.sourcePose.TryGetPosition(out controllerPosition);
            ControllerState = controllerTracked ? ControllerState.Tracked : ControllerState.NotTracked;

            // Get Controller start rotation
            interactionSourceState.sourcePose.TryGetRotation(out controllerRotation);
            if (controllerPosition == Vector3.zero || controllerRotation == Quaternion.identity)
            {
                Debug.LogWarning($"No controller data detected");
            }
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState)
        {
            interactionSourceState.sourcePose.TryGetPosition(out pointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out pointerRotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                pointerPosition = CameraCache.Main.transform.parent.TransformPoint(pointerPosition);
                pointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(pointerRotation.eulerAngles);
            }

            Interactions.SetDictionaryValue(DeviceInputType.SpatialPointer, new Tuple<Vector3, Quaternion>(pointerPosition, pointerRotation));
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateGripData(InteractionSourceState interactionSourceState)
        {
            interactionSourceState.sourcePose.TryGetPosition(out gripPosition, InteractionSourceNode.Grip);
            interactionSourceState.sourcePose.TryGetRotation(out gripRotation, InteractionSourceNode.Grip);

            if (CameraCache.Main.transform.parent != null)
            {
                gripPosition = CameraCache.Main.transform.parent.TransformPoint(gripPosition);
                gripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(gripRotation.eulerAngles);
            }

            Interactions.SetDictionaryValue(DeviceInputType.SpatialGrip, new Tuple<Vector3, Quaternion>(gripPosition, gripRotation));
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateTouchPadData(InteractionSourceState interactionSourceState)
        {
            if (interactionSourceState.touchpadTouched)
            {
                if (Interactions.ContainsKey(DeviceInputType.TouchpadTouch)) Interactions.SetDictionaryValue(DeviceInputType.TouchpadTouch, interactionSourceState.touchpadTouched);  //Interactions[DeviceInputType.TouchpadTouch].SetValue(interactionSourceState.touchpadTouched);
                if (Interactions.ContainsKey(DeviceInputType.TouchpadPress)) Interactions.SetDictionaryValue(DeviceInputType.TouchpadPress, interactionSourceState.touchpadPressed);  //Interactions[DeviceInputType.TouchpadPress].SetValue(interactionSourceState.touchpadPressed);
                if (Interactions.ContainsKey(DeviceInputType.Touchpad)) Interactions.SetDictionaryValue(DeviceInputType.Touchpad, interactionSourceState.touchpadPosition);  //Interactions[DeviceInputType.Touchpad].SetValue(interactionSourceState.touchpadPosition);
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateThumbStickData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(DeviceInputType.ThumbStickPress)) Interactions.SetDictionaryValue(DeviceInputType.ThumbStickPress, interactionSourceState.thumbstickPressed);  //Interactions[DeviceInputType.ThumbStickPress].SetValue(interactionSourceState.thumbstickPressed);

            Interactions.SetDictionaryValue(DeviceInputType.ThumbStick, interactionSourceState.thumbstickPosition);
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateTriggerData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(DeviceInputType.TriggerPress)) Interactions.SetDictionaryValue(DeviceInputType.TriggerPress, interactionSourceState.selectPressed);  //Interactions[DeviceInputType.TriggerPress].SetValue(interactionSourceState.selectPressed);

            Interactions.SetDictionaryValue(DeviceInputType.Trigger, interactionSourceState.selectPressedAmount);
        }

        #endregion Update data functions

        #endregion Setup and Update functions

        #region Utilities
        private static InteractionSourceState CheckIfValidInteractionSourceState<T>(T state)
        {
            InteractionSourceState interactionSourceState = (InteractionSourceState)Convert.ChangeType(state, typeof(InteractionSourceState));
            if (interactionSourceState.source.id == 0) { throw new ArgumentOutOfRangeException(nameof(state), "Incorrect state type provided to controller,did you send an InteractionSourceState?"); }

            return interactionSourceState;
        }
        #endregion Utilities
    }
}