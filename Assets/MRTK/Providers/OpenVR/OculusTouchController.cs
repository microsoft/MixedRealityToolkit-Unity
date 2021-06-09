// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusTouch,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/OculusControllersTouch",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class OculusTouchController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusTouchController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new OculusTouchControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Axis1D.PrimaryIndexTrigger
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14), // Axis1D.PrimaryIndexTrigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_13), // Axis1D.PrimaryIndexTrigger Near Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Axis1D.PrimaryIndexTrigger Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Axis1D.PrimaryHandTrigger Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2), // Axis2D.PrimaryThumbstick
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton16), // Button.PrimaryThumbstick Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_15), // Button.PrimaryThumbstick Near Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8), // Button.PrimaryThumbstick Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // Button.Three Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton3), // Button.Four Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton6), // Button.Start Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton12), // Button.Three Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton13), // Button.Four Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton18), // Touch.PrimaryThumbRest Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_17), // Touch.PrimaryThumbRest Near Touch
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Axis1D.SecondaryIndexTrigger
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15), // Axis1D.SecondaryIndexTrigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_14), // Axis1D.SecondaryIndexTrigger Near Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Axis1D.SecondaryIndexTrigger Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Axis1D.SecondaryHandTrigger Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5), // Axis2D.SecondaryThumbstick
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton17), // Button.SecondaryThumbstick Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_16), // Button.SecondaryThumbstick Near Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9), // Button.SecondaryThumbstick Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // Button.One Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Button.Two Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton11), // Button.One Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton10), // Button.Two Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton19), // Touch.SecondaryThumbRest Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_18), // Touch.SecondaryThumbRest Near Touch
        };
    }
}
