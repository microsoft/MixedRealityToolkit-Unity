// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.ViveWand,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/ViveWandController",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class ViveWandController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ViveWandController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new ViveWandControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2), // Trackpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton16), // Trackpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8), // Trackpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // Menu Button
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5), // Trackpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton17), // Trackpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9), // Trackpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Menu Button
        };
    }
}