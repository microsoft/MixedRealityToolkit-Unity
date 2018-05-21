// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections;
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
            SourceId = sourceId;
            SourceName = $"{sourceId}_{handedness}";
            ControllerState = ControllerState.None;
            Handedness = handedness;
            Pointers = new IMixedRealityPointer[0];
            Interactions = new Dictionary<DeviceInputType, InteractionDefinition>();
            Capabilities = null;

            controllerTracked = false;
            controllerPosition = pointerPosition = gripPosition = Vector3.zero;
            controllerRotation = pointerRotation = gripRotation = Quaternion.identity;
        }

        #region IMixedRealityInputSource Interface Members

        public uint SourceId { get; }

        public string SourceName { get; }

        public ControllerState ControllerState { get; private set; }

        public Handedness Handedness { get; }

        public IMixedRealityPointer[] Pointers { get; private set; }

        public Dictionary<DeviceInputType, InteractionDefinition> Interactions { get; }

        public DeviceInputType[] Capabilities { get; private set; }

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

        public bool SupportsCapabilities(DeviceInputType[] inputInfo)
        {
            return Capabilities == inputInfo;
        }

        public bool SupportsCapability(DeviceInputType inputInfo)
        {
            for (int i = 0; i < Capabilities.Length; i++)
            {
                if (Capabilities[i] == inputInfo)
                {
                    return true;
                }
            }

            return false;
        }


        public void RegisterPointers(IMixedRealityPointer[] pointers)
        {
            if (pointers == null) { throw new System.ArgumentNullException(nameof(pointers)); }

            Pointers = pointers;
        }

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        public void SetupFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);

            MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController), Handedness);
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
            Capabilities = new InputType[mappings.Length];
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                Interactions.Add(mappings[i].InputType, new InteractionDefinition(i, mappings[i].AxisType, mappings[i].InputAction));

                // Add capability for Input Type
                Capabilities[i] = mappings[i].InputType;
            }
        }

        private void SetupWMRControllerDefaults(InteractionSourceState interactionSourceState)
        {
            //Add the Controller Pointer
            Interactions.Add(InputType.Pointer, new InteractionDefinition(1, AxisType.SixDoF, InputAction.Pointer));

            // Add the Controller trigger
            Interactions.Add(InputType.Trigger, new InteractionDefinition(2, AxisType.SingleAxis, Handedness == Handedness.Left ? InputAction.LeftTrigger : InputAction.RightTrigger));

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            if (interactionSourceState.source.supportsGrasp)
            {
                Interactions.Add(InputType.Grip, new InteractionDefinition(3, AxisType.SixDoF, InputAction.Grip));

                Interactions.Add(InputType.GripPress, new InteractionDefinition(4, AxisType.SingleAxis, InputAction.ActionOne));
            }

            // If the controller has a menu button, add it to the controller capabilities
            if (interactionSourceState.source.supportsMenu)
            {
                Interactions.Add(InputType.Menu, new InteractionDefinition(5, AxisType.Digital, InputAction.ActionTwo));
            }

            // If the controller has a Thumbstick, add it to the controller capabilities
            if (interactionSourceState.source.supportsThumbstick)
            {
                Interactions.Add(InputType.ThumbStick, new InteractionDefinition(6, AxisType.DualAxis, Handedness == Handedness.Left ? InputAction.LeftThumbstick : InputAction.RightThumbstick));
                Interactions.Add(InputType.ThumbStickPress, new InteractionDefinition(7, AxisType.Digital, InputAction.LeftThumbstickPressed));
            }

            // If the controller has a Touchpad, add it to the controller capabilities
            if (interactionSourceState.source.supportsTouchpad)
            {
                Interactions.Add(InputType.Touchpad, new InteractionDefinition(8, AxisType.DualAxis, Handedness == Handedness.Left ? InputAction.LeftTouch : InputAction.RightTouch));
                Interactions.Add(InputType.TouchpadTouch, new InteractionDefinition(9, AxisType.Digital, InputAction.LeftTouchTouched));
                Interactions.Add(InputType.TouchpadPress, new InteractionDefinition(10, AxisType.Digital, Handedness == Handedness.Left ? InputAction.LeftTouchPressed : InputAction.RightTouchPressed));
            }
        }

        public void UpdateFromInteractionSource(InteractionSourceState interactionSourceState)
        {
            Debug.Assert(interactionSourceState.source.id == SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            // TODO - Do we need Kind?
            //Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            // Update Controller
            // TODO - Controller currently not accepted by InputSystem, only InteractionState captured
            // TODO - May need to be more granular with checks if we are allowing user to configure :S  
            // TODO - Need to think of a better way to validate options, multiple Contains aren't good, maybe an extension?
            UpdateControllerData(interactionSourceState);

            // Update Pointer
            if (Interactions.ContainsKey(InputType.Pointer)) UpdatePointerData(interactionSourceState);

            // Update Grip
            if (Interactions.ContainsKey(InputType.Grip)) UpdateGripData(interactionSourceState);

            // Update Touchpad
            if (Interactions.ContainsKey(InputType.Touchpad) || Interactions.ContainsKey(InputType.TouchpadTouch)) UpdateTouchPadData(interactionSourceState);

            // Update Thumbstick
            if (Interactions.ContainsKey(InputType.Thumb)) UpdateThumbStickData(interactionSourceState);

            // Update Trigger
            if (Interactions.ContainsKey(InputType.Trigger)) UpdateTriggerData(interactionSourceState);
        }

        #region Update data functions

        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            controllerTracked = interactionSourceState.sourcePose.TryGetPosition(out controllerPosition);
            InputSourceState = controllerTracked ? InputSourceState.Tracked : InputSourceState.NotTracked;
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
            Interactions[InputType.Pointer].SetValue(new Tuple<Vector3, Quaternion>(pointerPosition, pointerRotation));
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
            Interactions[InputType.Grip].SetValue(new Tuple<Vector3, Quaternion>(gripPosition, gripRotation));
        }

        private void UpdateTouchPadData(InteractionSourceState interactionSourceState)
        {
            if (interactionSourceState.touchpadTouched)
            {
                if (Interactions.ContainsKey(InputType.TouchpadTouch)) Interactions[InputType.TouchpadTouch].SetValue(interactionSourceState.touchpadTouched);
                if (Interactions.ContainsKey(InputType.TouchpadPress)) Interactions[InputType.TouchpadPress].SetValue(interactionSourceState.touchpadPressed);
                if (Interactions.ContainsKey(InputType.Touchpad)) Interactions[InputType.Touchpad].SetValue(interactionSourceState.touchpadPosition);
            }
        }

        private void UpdateThumbStickData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(InputType.ThumbStickPress)) Interactions[InputType.ThumbStickPress].SetValue(interactionSourceState.thumbstickPressed);
            Interactions[InputType.ThumbStick].SetValue(interactionSourceState.thumbstickPosition);
        }

        private void UpdateTriggerData(InteractionSourceState interactionSourceState)
        {
            if (Interactions.ContainsKey(InputType.TriggerPress)) Interactions[InputType.TriggerPress].SetValue(interactionSourceState.selectPressed);
            Interactions[InputType.Trigger].SetValue(interactionSourceState.selectPressedAmount);
        }

        #endregion Update data functions

        #endregion Setup and Update functions

        //TODO - Needs re-implementing, as there may be more than left == right
        #region IEquality Implementation

        public static bool Equals(IMixedRealityInputSource left, IMixedRealityInputSource right)
        {
            return left.Equals(right);
        }

        public new bool Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityInputSource)obj);
        }

        private bool Equals(IMixedRealityInputSource other)
        {
            return other != null && SourceId == other.SourceId && string.Equals(SourceName, other.SourceName);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)SourceId;
                hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation

        //TODO - Needs re-implementing as it returns true if ONE capability is supported, but not multiples

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
