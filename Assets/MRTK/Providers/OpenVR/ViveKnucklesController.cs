// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.ViveKnuckles,
        new[] { Handedness.Left, Handedness.Right },
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class ViveKnucklesController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ViveKnucklesController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new ViveKnucklesControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Average
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2), // Trackpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton16), // Trackpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8), // Trackpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // Inner Face Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton3), // Outer Face Button
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_20), // Index Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_22), // Middle Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_24), // Ring Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_26), // Pinky Finger Cap Sensor
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Average
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5), // Trackpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton17), // Trackpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9), // Trackpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Inner Face Button
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // Outer Face Button
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_21), // Index Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_23), // Middle Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_25), // Ring Finger Cap Sensor
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_27), // Pinky Finger Cap Sensor
        };
    }
}