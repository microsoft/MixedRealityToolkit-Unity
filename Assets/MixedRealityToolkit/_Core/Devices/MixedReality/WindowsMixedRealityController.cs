// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public struct WindowsMixedRealityController : IMixedRealityInputSource
    {
        private bool controllerTracked;
        private Vector3 controllerPosition, pointerPosition, gripPosition;
        private Quaternion controllerRotation, pointerRotation, gripRotation;

        public WindowsMixedRealityController(uint sourceId, Handedness handedness)
        {
            SourceId = sourceId;
            SourceName = sourceId.ToString() + handedness;
            InputSourceState = InputSourceState.None;
            Handedness = handedness;
            Pointers = new IMixedRealityPointer[0];
            Interactions = new Dictionary<InputType, InteractionDefinition>();

            controllerTracked = false;
            controllerPosition = pointerPosition = gripPosition = Vector3.zero;
            controllerRotation = pointerRotation = gripRotation = Quaternion.identity;
        }

        public uint SourceId { get; }

        public string SourceName { get; }

        public InputSourceState InputSourceState { get; private set; }

        public Handedness Handedness { get;  }
        
        public IMixedRealityPointer[] Pointers { get; private set; }
        
        public Dictionary<InputType, InteractionDefinition> Interactions { get;}

        #region Setup and Update functions

        public void SetupInputSource(InteractionSourceState interactionSourceState)
        {
            //Update the Tracked state of the controller
            UpdateControllerData(interactionSourceState);


            //Add the Controller Pointer
            Interactions.Add(InputType.Pointer, new InteractionDefinition()
            {
                AxisType = AxisType.SixDoF,
                Changed = false,
                InputType = InputType.Pointer
            });

            // Add the Controller trigger
            Interactions.Add(InputType.Trigger, new InteractionDefinition()
            {
                AxisType = AxisType.SingleAxis,
                Changed = false,
                InputType = InputType.Trigger
            });

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            if (interactionSourceState.source.supportsGrasp)
            {
                Interactions.Add(InputType.Grip, new InteractionDefinition()
                {
                    AxisType = AxisType.SixDoF,
                    Changed = false,
                    InputType = InputType.Grip
                });

                Interactions.Add(InputType.GripPress, new InteractionDefinition()
                {
                    AxisType = AxisType.SingleAxis,
                    Changed = false,
                    InputType = InputType.GripPress
                });

            }

            // If the controller has a menu button, add it to the controller capabilities
            if (interactionSourceState.source.supportsMenu)
            {
                Interactions.Add(InputType.Menu, new InteractionDefinition()
                {
                    AxisType = AxisType.Digital,
                    Changed = false,
                    InputType = InputType.Menu
                });
            }

            // If the controller has a Thumbstick, add it to the controller capabilities
            if (interactionSourceState.source.supportsThumbstick)
            {
                Interactions.Add(InputType.ThumbStick, new InteractionDefinition()
                {
                    AxisType = AxisType.DualAxis,
                    Changed = false,
                    InputType = InputType.ThumbStick
                });
                Interactions.Add(InputType.ThumbStickPress, new InteractionDefinition()
                {
                    AxisType = AxisType.Digital,
                    Changed = false,
                    InputType = InputType.ThumbStickPress
                });
            }

            // If the controller has a Touchpad, add it to the controller capabilities
            if (interactionSourceState.source.supportsTouchpad)
            {
                Interactions.Add(InputType.Touchpad, new InteractionDefinition()
                {
                    AxisType = AxisType.DualAxis,
                    Changed = false,
                    InputType = InputType.Touchpad
                });
                Interactions.Add(InputType.TouchpadTouch, new InteractionDefinition()
                {
                    AxisType = AxisType.Digital,
                    Changed = false,
                    InputType = InputType.TouchpadTouch
                });
                Interactions.Add(InputType.TouchpadPress, new InteractionDefinition()
                {
                    AxisType = AxisType.Digital,
                    Changed = false,
                    InputType = InputType.TouchpadPress
                });
            }
        }

        public void UpdateInputSource(InteractionSourceState interactionSourceState)
        {
            Debug.Assert(interactionSourceState.source.id == SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            // TODO - Do we need Kind?
            //Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            // Update Controller
            // TODO - Controller currently not accepted by InputSystem, only InteractionState captured
            UpdateControllerData(interactionSourceState);

            // Update Pointer
            UpdatePointerData(interactionSourceState);

            // Update Grip
            UpdateGripData(interactionSourceState);

            // Update Touchpad
            UpdateTouchPadData(interactionSourceState);

            // Update Thumbstick
            UpdateThumbStickData(interactionSourceState);

            // Update Trigger
            UpdateTriggerData(interactionSourceState);
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
                Interactions[InputType.TouchpadTouch].SetValue(interactionSourceState.touchpadTouched);
                Interactions[InputType.TouchpadPress].SetValue(interactionSourceState.touchpadPressed);
                Interactions[InputType.Touchpad].SetValue(interactionSourceState.touchpadPosition);
            }
        }

        private void UpdateThumbStickData(InteractionSourceState interactionSourceState)
        {
            Interactions[InputType.ThumbStickPress].SetValue(interactionSourceState.thumbstickPressed);
            Interactions[InputType.ThumbStick].SetValue(interactionSourceState.thumbstickPosition);
        }

        private void UpdateTriggerData(InteractionSourceState interactionSourceState)
        {
            Interactions[InputType.TriggerPress].SetValue(interactionSourceState.selectPressed);
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
        public bool SupportsInputCapabilities(InputType[] capabilities)
        {
            foreach (var interaction in Interactions)
            {
                for (int i = 0; i < capabilities.Length; i++)
                {
                    if (interaction.Key == capabilities[i])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool SupportsInputCapability(InputType capability)
        {
            return Interactions.ContainsKey(capability);
        }


        public void RegisterPointers(IMixedRealityPointer[] pointers)
        {
            if (pointers == null) { throw new System.ArgumentNullException(nameof(pointers)); }

            Pointers = pointers;
        }
    }
}
