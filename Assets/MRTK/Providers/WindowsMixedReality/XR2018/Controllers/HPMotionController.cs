// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Microsoft.MixedReality.Input;
using System;

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
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
    public class HPMotionController : WindowsMixedRealityController
    {
        public HPMotionController(TrackingState trackingState, Toolkit.Utilities.Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        private static readonly ProfilerMarker UpdateControllerPerfMarker = new ProfilerMarker("[MRTK] HPController.UpdateController");

        /// <summary>
        /// Update the controller data from .
        /// </summary>
        public virtual void UpdateController(MotionControllerState controllerState)
        {
            using (UpdateControllerPerfMarker.Auto())
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
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.X_Button : ControllerInput.A_Button;
                            break;
                        case DeviceInputType.SecondaryButtonPress:
                            button = controllerHandedness == MixedReality.Input.Handedness.Left ? ControllerInput.Y_Button : ControllerInput.B_Button;
                            break;
                        case DeviceInputType.TouchpadTouch:
                        case DeviceInputType.TouchpadPress:
                            button = ControllerInput.Touchpad;
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

                InputFeatureUsage<Vector2> axisUsage;

                // Update the interaction data source
                switch (interactionMapping.InputType)
                {
                    case DeviceInputType.ThumbStick:
                        axisUsage = CommonUsages.primary2DAxis;
                        break;
                    case DeviceInputType.Touchpad:
                        axisUsage = CommonUsages.secondary2DAxis;
                        break;
                    default:
                        return;
                }

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