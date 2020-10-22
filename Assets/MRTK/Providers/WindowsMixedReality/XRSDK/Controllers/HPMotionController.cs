﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Input;
using System;

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    public class MotionControllerState
    {
        public MotionControllerState(MotionController mc)
        {
            this.MotionController = mc;
        }
        public void Update(DateTime currentTime)
        {
            this.CurrentReading = MotionController.TryGetReadingAtTime(currentTime);
        }
        public MotionController MotionController { get; private set; }
        public MotionControllerReading CurrentReading { get; private set; }
    }

    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Toolkit.Utilities.Handedness.Left, Toolkit.Utilities.Handedness.Right },
        flags: MixedRealityControllerConfigurationFlags.UseCustomInteractionMappings)]
    public class HPMotionController : WindowsMixedRealityXRSDKMotionController
    {
        internal MotionControllerState MotionControllerState;

        public HPMotionController(TrackingState trackingState, Toolkit.Utilities.Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }


        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInteractionMapping(3, "Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInteractionMapping(4, "Grip Press", AxisType.Digital, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(5, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(6, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(7, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(8, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(9, "Button.X Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(10, "Button.Y Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(11, "Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(12, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(13, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress)
        };

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip),
            new MixedRealityInteractionMapping(2, "Grip Position", AxisType.SingleAxis, DeviceInputType.Grip),
            new MixedRealityInteractionMapping(3, "Grip Touch", AxisType.Digital, DeviceInputType.GripTouch),
            new MixedRealityInteractionMapping(4, "Grip Press", AxisType.Digital, DeviceInputType.GripPress),
            new MixedRealityInteractionMapping(5, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger),
            new MixedRealityInteractionMapping(6, "Trigger Touch", AxisType.Digital, DeviceInputType.TriggerTouch),
            new MixedRealityInteractionMapping(7, "Trigger Press", AxisType.Digital, DeviceInputType.TriggerPress),
            new MixedRealityInteractionMapping(8, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.Select),
            new MixedRealityInteractionMapping(9, "Button.A Press", AxisType.Digital, DeviceInputType.PrimaryButtonPress),
            new MixedRealityInteractionMapping(10, "Button.B Press", AxisType.Digital, DeviceInputType.SecondaryButtonPress),
            new MixedRealityInteractionMapping(11, "Menu Press", AxisType.Digital, DeviceInputType.Menu),
            new MixedRealityInteractionMapping(12, "Thumbstick Position", AxisType.DualAxis, DeviceInputType.ThumbStick),
            new MixedRealityInteractionMapping(13, "Thumbstick Press", AxisType.Digital, DeviceInputType.ThumbStickPress)
        };

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateController");

        public override void UpdateController(InputDevice inputDevice)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
                if (MotionControllerState != null)
                {
                    // If the Motion controller state is instantiated and tracked, use it to update the interaction bool data and the interaction source to update the 6-dof data
                    UpdateController(MotionControllerState);
                    base.UpdateSixDofData(inputDevice);
                }
                else
                {
                    // Otherwise, update normally
                    base.UpdateController(inputDevice);
                }
            }
        }

        /// <summary>
        /// Update the controller data from .
        /// </summary>
        public virtual void UpdateController(MotionControllerState controllerState)
        {
            if (!Enabled) { return; }

            using (UpdateControllerPerfMarker.Auto())
            {
                //base.UpdateController(interactionSourceState);
                for (int i = 0; i < Interactions?.Length; i++)
                {
                    switch (Interactions[i].AxisType)
                    {
                        case AxisType.None:
                            break;
                        case AxisType.Digital:
                            UpdateButtonData(Interactions[i], controllerState);
                            break;
                        case AxisType.SingleAxis:
                            UpdateSingleAxisData(Interactions[i], controllerState);
                            break;
                        case AxisType.DualAxis:
                            UpdateDualAxisData(Interactions[i], controllerState);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateButtonData");

        /// <summary>
        /// Update an interaction bool data type from a bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// </remarks>
        protected virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                // Handedness must be left or right in order to differentiate between buttons for the left and right hand.
                MixedReality.Input.Handedness controllerHandedness = controllerState.MotionController.Handedness;

                Debug.Assert(controllerHandedness != MixedReality.Input.Handedness.Unknown);
                Debug.Assert(interactionMapping.AxisType == AxisType.Digital);

                if (interactionMapping.InputType == DeviceInputType.TriggerPress)
                {
                    var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                    interactionMapping.BoolData = triggerData.Equals(1);
                }
                else if (interactionMapping.InputType == DeviceInputType.GripPress)
                {
                    var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                    interactionMapping.BoolData = gripData.Equals(1);
                }
                else
                {
                    ControllerInput button;

                    // Update the interaction data source
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.Select:
                        case DeviceInputType.TriggerTouch:
                        case DeviceInputType.TriggerNearTouch:
                            button = ControllerInput.Trigger;
                            break;
                        case DeviceInputType.GripTouch:
                        case DeviceInputType.GripNearTouch:
                            button = ControllerInput.Grasp;
                            break;
                        case DeviceInputType.ButtonPress:
                        case DeviceInputType.PrimaryButtonPress:
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.X_Button : ControllerInput.A_Button;
                            break;
                        case DeviceInputType.SecondaryButtonPress:
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.Y_Button : ControllerInput.B_Button;
                            break;
                        case DeviceInputType.Menu:
                            button = ControllerInput.Menu;
                            break;
                        case DeviceInputType.ThumbStickTouch:
                        case DeviceInputType.ThumbStickPress:
                            button = ControllerInput.Thumbstick;
                            break;
                        default:
                            return;
                    }


                    var buttonData = controllerState.CurrentReading.GetPressedValue(button);
                    interactionMapping.BoolData = buttonData > 0.0f;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    if (interactionMapping.BoolData)
                    {
                        CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }
            }
        }

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateSingleAxisData");

        /// <summary>
        /// Update an interaction float data type from a SingleAxis (float) input
        /// </summary>
        /// <remarks>
        /// Raises a FloatInputChanged event when the float data changes
        /// </remarks>
        protected virtual void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                        interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
                        break;
                    case DeviceInputType.GripPress:
                        var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                        interactionMapping.BoolData = !Mathf.Approximately(gripData, 0.0f);
                        break;
                    default:
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise bool input system event if it's available
                    if (interactionMapping.BoolData)
                    {
                        CoreServices.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                    else
                    {
                        CoreServices.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                    }
                }

                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.Trigger:
                        var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                        interactionMapping.FloatData = triggerData;
                        break;
                    case DeviceInputType.Grip:
                        var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                        interactionMapping.FloatData = gripData;
                        break;
                    default:
                        return;
                }

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise float input system event if it's enabled
                    CoreServices.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.FloatData);
                }
            }
        }

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateDualAxisData");

        /// <summary>
        /// Update the touchpad / thumbstick input from the device
        /// </summary>
        protected virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateDualAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.DualAxis);

                // Only process the reading if the input mapping is for the thumbstick
                if (interactionMapping.InputType != DeviceInputType.ThumbStick)
                    return;

                System.Numerics.Vector2 controllerAxisData = controllerState.CurrentReading.GetXYValue(ControllerInput.Thumbstick);
                Vector2 axisData = new Vector2(controllerAxisData.X, controllerAxisData.Y);

                // Update the interaction data source
                interactionMapping.Vector2Data = axisData;

                // If our value changed raise it.
                if (interactionMapping.Changed)
                {
                    // Raise input system event if it's enabled
                    CoreServices.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionMapping.Vector2Data);
                }
            }
        }
    }
}