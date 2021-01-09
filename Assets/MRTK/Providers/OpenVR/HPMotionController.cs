// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    /// <summary>
    /// Open VR Implementation of the HP Motion Controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.HPMotionController,
        new[] { Handedness.Left, Handedness.Right })]
    public class HPMotionController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public HPMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new HPMotionControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        /// <inheritdoc />
        public override float PointerOffsetAngle { get; protected set; } = -30f;

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Grip
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton3), // Button.X Press
            new MixedRealityInteractionMappingLegacyInput(), // Button.Y Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // Menu Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2, invertYAxis: true), // Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton18), // Thumbstick Press
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Grip
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Touch
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // Button.A Press
            new MixedRealityInteractionMappingLegacyInput(), // Button.B Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Menu Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true), // Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton19), // Thumbstick Press
        };
    }
}