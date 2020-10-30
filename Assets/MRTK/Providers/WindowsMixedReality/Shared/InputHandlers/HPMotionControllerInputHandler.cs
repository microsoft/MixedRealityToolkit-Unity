// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Profiling;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;

#if HP_CONTROLLER_ENABLED
using Microsoft.MixedReality.Input;
using MotionControllerHandedness = Microsoft.MixedReality.Input.Handedness;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;
#endif

/// <summary>
/// Class for handling updating a controller via data provided by the HP Motion Controller's API
/// </summary>

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
#if HP_CONTROLLER_ENABLED
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


    public class HPMotionControllerInputHandler
    {
        private IMixedRealityInputSource InputSource;
        private Handedness ControllerHandedness;
        private MixedRealityInteractionMapping[] Interactions;

        public HPMotionControllerInputHandler(Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
        {
            ControllerHandedness = controllerHandedness;
            InputSource = inputSource;
            Interactions = interactions;
        }

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHander.UpdateController");

        /// <summary>
        /// Update the controller data from .
        /// </summary>
        public virtual void UpdateController(MotionControllerState controllerState)
        {
            using (UpdateControllerPerfMarker.Auto())
            {
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

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHander.UpdateButtonData");

        /// <summary>
        /// Update an interaction bool data type from a bool input
        /// </summary>
        /// <remarks>
        /// Raises an Input System "Input Down" event when the key is down, and raises an "Input Up" when it is released (e.g. a Button)
        /// </remarks>
        internal virtual void UpdateButtonData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateButtonDataPerfMarker.Auto())
            {
                // Handedness must be left or right in order to differentiate between buttons for the left and right hand.
                MotionControllerHandedness controllerHandedness = controllerState.MotionController.Handedness;

                Debug.Assert(controllerHandedness != MotionControllerHandedness.Unknown);
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
                            button = ControllerInput.Trigger;
                            break;
                        case DeviceInputType.GripTouch:
                            button = ControllerInput.Grasp;
                            break;
                        case DeviceInputType.PrimaryButtonPress:
                            button = controllerHandedness == MotionControllerHandedness.Left ? ControllerInput.X_Button : ControllerInput.A_Button;
                            break;
                        case DeviceInputType.SecondaryButtonPress:
                            button = controllerHandedness == MotionControllerHandedness.Left ? ControllerInput.Y_Button : ControllerInput.B_Button;
                            break;
                        case DeviceInputType.Menu:
                            button = ControllerInput.Menu;
                            break;
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

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHander.UpdateSingleAxisData");

        /// <summary>
        /// Update an interaction float data type from a SingleAxis (float) input
        /// </summary>
        /// <remarks>
        /// Raises a FloatInputChanged event when the float data changes
        /// </remarks>
        internal virtual void UpdateSingleAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
        {
            using (UpdateSingleAxisDataPerfMarker.Auto())
            {
                Debug.Assert(interactionMapping.AxisType == AxisType.SingleAxis);
                // First handle updating the bool values, since those events are only raised once the trigger/gripped is presssed
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.TriggerPress:
                        var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                        interactionMapping.BoolData = triggerData.Equals(1);
                        break;
                    case DeviceInputType.GripPress:
                        var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                        interactionMapping.BoolData = gripData.Equals(1);
                        break;
                    default:
                        break;
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

                // Next handle updating the float values
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

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHander.UpdateDualAxisData");

        /// <summary>
        /// Update the touchpad / thumbstick input from the device
        /// </summary>
        internal virtual void UpdateDualAxisData(MixedRealityInteractionMapping interactionMapping, MotionControllerState controllerState)
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
#endif
}
