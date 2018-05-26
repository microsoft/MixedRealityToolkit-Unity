// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
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

        public WindowsMixedRealityController(uint sourceId, Handedness handedness)
        {
            ControllerState = ControllerState.None;
            ControllerHandedness = handedness;

            InputSource = null;

            controllerTracked = false;
            controllerPosition = pointerPosition = gripPosition = Vector3.zero;
            controllerRotation = pointerRotation = gripRotation = Quaternion.identity;
            Interactions = new Dictionary<DeviceInputType, InteractionDefinition>();
        }

        #region IMixedRealityController Interface Members

        public ControllerState ControllerState { get; private set; }

        public Handedness ControllerHandedness { get; }

        public IMixedRealityInputSource InputSource { get; private set; }

        public Dictionary<DeviceInputType, InteractionDefinition> Interactions { get; private set; }

        public void SetupInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            SetupFromInteractionSource(interactionSourceState);
        }

        public void UpdateInputSource<T>(T state)
        {
            InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
            UpdateFromInteractionSource(interactionSourceState);
        }

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        public void SetupFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);

            MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController),ControllerHandedness);
            //MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping<WindowsMixedRealityController>(ControllerHandedness);
            if (controllerMapping.Interactions.Length > 0)
            {
                SetupFromMapping(controllerMapping.Interactions);
            }
            else
            {

                SetupWMRControllerDefaults(interactionSourceState);
            }
        }

        private void SetupFromMapping(InteractionDefinitionMapping[] mappings)
        {
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                Interactions.Add(mappings[i].InputType, new InteractionDefinition(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction, mappings[i].InputHoldAction));
            }
        }

        private void SetupWMRControllerDefaults(InteractionSourceState interactionSourceState)
        {
            InputAction[] inputActions = Managers.MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions;
            //Add the Controller Pointer
            Interactions.Add(DeviceInputType.SpatialPointer, new InteractionDefinition(1, AxisType.SixDoF, DeviceInputType.SpatialPointer, inputActions.GetActionByName("Select"))); // Note will convert these lookups to indexes

            // Add the Controller trigger
            Interactions.Add(DeviceInputType.Trigger, new InteractionDefinition(2, AxisType.SingleAxis, DeviceInputType.Trigger, inputActions.GetActionByName("Select")));

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            if (interactionSourceState.source.supportsGrasp)
            {
                Interactions.Add(DeviceInputType.SpatialGrip, new InteractionDefinition(3, AxisType.SixDoF, DeviceInputType.SpatialGrip, inputActions.GetActionByName("Grip")));

                Interactions.Add(DeviceInputType.GripPress, new InteractionDefinition(4, AxisType.SingleAxis, DeviceInputType.GripPress, inputActions.GetActionByName("Grab")));
            }

            // If the controller has a menu button, add it to the controller capabilities
            if (interactionSourceState.source.supportsMenu)
            {
                Interactions.Add(DeviceInputType.Menu, new InteractionDefinition(5, AxisType.Digital, DeviceInputType.Menu, inputActions.GetActionByName("Menu")));
            }

            // If the controller has a Thumbstick, add it to the controller capabilities
            if (interactionSourceState.source.supportsThumbstick)
            {
                Interactions.Add(DeviceInputType.ThumbStick, new InteractionDefinition(6, AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerHandedness == Handedness.Left ? inputActions.GetActionByName("Walk") : inputActions.GetActionByName("Look")));
                Interactions.Add(DeviceInputType.ThumbStickPress, new InteractionDefinition(7, AxisType.Digital, DeviceInputType.ThumbStickPress, inputActions.GetActionByName("Interact")));
            }

            // If the controller has a Touchpad, add it to the controller capabilities
            if (interactionSourceState.source.supportsTouchpad)
            {
                Interactions.Add(DeviceInputType.Touchpad, new InteractionDefinition(8, AxisType.DualAxis, DeviceInputType.Touchpad, inputActions.GetActionByName("Inventory")));
                Interactions.Add(DeviceInputType.TouchpadTouch, new InteractionDefinition(9, AxisType.Digital, DeviceInputType.TouchpadTouch, inputActions.GetActionByName("Pickup")));
                Interactions.Add(DeviceInputType.TouchpadPress, new InteractionDefinition(10, AxisType.Digital, DeviceInputType.TouchpadPress, inputActions.GetActionByName("Pickup")));
            }
        }

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

        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            controllerTracked = interactionSourceState.sourcePose.TryGetPosition(out controllerPosition);
            ControllerState = controllerTracked ? ControllerState.Tracked : ControllerState.NotTracked;
            //No Controller data available yet

            // Get Controller start position
            interactionSourceState.sourcePose.TryGetPosition(out controllerPosition);

            // Get Controller start rotation
            interactionSourceState.sourcePose.TryGetRotation(out controllerRotation);
        }

        private void UpdatePointerData(InteractionSourceState interactionSourceState)
        {
            interactionSourceState.sourcePose.TryGetPosition(out pointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out pointerRotation, InteractionSourceNode.Pointer);

            //TODO - Review, as we no longer have a MixedRealityCameraParent, this will cause issue.
            if (CameraCache.Main.transform.parent != null)
            {
                pointerPosition = CameraCache.Main.transform.parent.TransformPoint(pointerPosition);
                pointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(pointerRotation.eulerAngles);
            }
            Interactions[DeviceInputType.SpatialPointer].SetValue(new Tuple<Vector3, Quaternion>(pointerPosition, pointerRotation));
        }

        private void UpdateGripData(InteractionSourceState interactionSourceState)
        {
            interactionSourceState.sourcePose.TryGetPosition(out gripPosition, InteractionSourceNode.Grip);
            interactionSourceState.sourcePose.TryGetRotation(out gripRotation, InteractionSourceNode.Grip);

            //TODO - Review, as we no longer have a MixedRealityCameraParent, this will cause issue.
            if (CameraCache.Main.transform.parent != null)
            {
                gripPosition = CameraCache.Main.transform.parent.TransformPoint(gripPosition);
                gripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(gripRotation.eulerAngles);
            }
            Interactions[DeviceInputType.SpatialGrip].SetValue(new Tuple<Vector3, Quaternion>(gripPosition, gripRotation));
        }

        private void UpdateTouchPadData(InteractionSourceState interactionSourceState)
        {
            if (interactionSourceState.touchpadTouched)
            {
                if (Interactions.ContainsKey(DeviceInputType.TouchpadTouch)) Interactions[DeviceInputType.TouchpadTouch].SetValue(interactionSourceState.touchpadTouched);
                if (Interactions.ContainsKey(DeviceInputType.TouchpadPress)) Interactions[DeviceInputType.TouchpadPress].SetValue(interactionSourceState.touchpadPressed);
                if (Interactions.ContainsKey(DeviceInputType.Touchpad)) Interactions[DeviceInputType.Touchpad].SetValue(interactionSourceState.touchpadPosition);
            }
        }

        private void UpdateThumbStickData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(DeviceInputType.ThumbStickPress)) Interactions[DeviceInputType.ThumbStickPress].SetValue(interactionSourceState.thumbstickPressed);
            Interactions[DeviceInputType.ThumbStick].SetValue(interactionSourceState.thumbstickPosition);
        }

        private void UpdateTriggerData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(DeviceInputType.TriggerPress)) Interactions[DeviceInputType.TriggerPress].SetValue(interactionSourceState.selectPressed);
            Interactions[DeviceInputType.Trigger].SetValue(interactionSourceState.selectPressedAmount);
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
