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

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality
{
#if HP_CONTROLLER_ENABLED
    /// <summary>
    /// Class for accessing the data provided by the HP Motion Controller's API
    /// </summary>
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

    /// <summary>
    /// Class for handling updating a controller via data provided by the HP Motion Controller's API
    /// </summary>
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

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHandler.UpdateController");

        /// <summary>
        /// Update the controller data from .
        /// </summary>
        public virtual void UpdateController(MotionControllerState controllerState)
        {
            controllerState.Update(DateTime.Now);

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

        private static readonly ProfilerMarker UpdateButtonDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHandler.UpdateButtonData");

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

                if (interactionMapping.InputType == DeviceInputType.TriggerTouch)
                {
                    var triggerData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Trigger);
                    interactionMapping.BoolData = !Mathf.Approximately(triggerData, 0.0f);
                }
                else if (interactionMapping.InputType == DeviceInputType.GripTouch)
                {
                    var gripData = controllerState.CurrentReading.GetPressedValue(ControllerInput.Grasp);
                    interactionMapping.BoolData = !Mathf.Approximately(gripData, 0.0f);
                }
                else
                {
                    ControllerInput button;

                    // Update the interaction data source
                    // Interactions handled mirror the GenericXRSDKController to maintain parity. ThumbstickTouch and Touchpad are left out
                    // due to having no ControllerInput equivalents
                    switch (interactionMapping.InputType)
                    {
                        case DeviceInputType.Select:
                        case DeviceInputType.TriggerNearTouch:
                        case DeviceInputType.TriggerPress:
                            button = ControllerInput.Trigger;
                            break;
                        case DeviceInputType.GripNearTouch:
                        case DeviceInputType.GripPress:
                            button = ControllerInput.Grasp;
                            break;
                        case DeviceInputType.ButtonPress:
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
                    interactionMapping.BoolData = Mathf.Approximately(buttonData, 1.0f);
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

        private static readonly ProfilerMarker UpdateSingleAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHandler.UpdateSingleAxisData");

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
                // First handle updating the bool values, since those events are only raised once the trigger/gripped is pressed
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

        private static readonly ProfilerMarker UpdateDualAxisDataPerfMarker = new ProfilerMarker("[MRTK] HPMotionControllerInputHandler.UpdateDualAxisData");

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
                float xAxisData = AdjustForDeadzone(2.0f * (controllerAxisData.X - 0.5f));
                float yAxisData = AdjustForDeadzone(2.0f * (controllerAxisData.Y - 0.5f));
                Vector2 axisData = new Vector2(xAxisData, yAxisData);

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

        private const float INNER_DEADZONE = 0.25f;
        private const float OUTER_DEADZONE = 0.1f;
        /// <summary>
        /// Returns a float which snaps to 0, 1.0f, or -1.0f if the input parameter falls within certain deadzones
        /// </summary>
        /// <param name="f">float between -1.0f and 1.0f</param>
        /// <returns>A float adjusted to snap to certain values if the initial value fell within certain deadzones</returns>
        private float AdjustForDeadzone(float f)
        {
            if (Mathf.Abs(f) < INNER_DEADZONE)
            {
                return 0.0f;
            }
            else if (Mathf.Abs(f) > 1.0f-OUTER_DEADZONE)
            {
                return 1.0f * Mathf.Sign(f);
            }
            else
            {
                return f;
            }
        }
    }
#endif
}
