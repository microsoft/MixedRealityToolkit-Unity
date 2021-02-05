// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    /// <summary>
    /// Open VR Implementation of the Windows Mixed Reality Motion Controllers.
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.WindowsMixedReality,
        new[] { Handedness.Left, Handedness.Right },
        "Textures/MotionController")]
    public class WindowsMixedRealityOpenVRMotionController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public WindowsMixedRealityOpenVRMotionController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new WindowsMixedRealityControllerDefinition(controllerHandedness), inputSource, interactions)
        { }

        /// <inheritdoc />
        public override float PointerOffsetAngle { get; protected set; } = -30f;

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Grip
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_11), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton14), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_17, axisCodeY: ControllerMappingLibrary.AXIS_18, invertYAxis: true), // Touchpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton16), // Touchpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8), // Touchpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // Menu Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2, invertYAxis: true), // Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton18), // Thumbstick Press
        };

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Pointer
            new MixedRealityInteractionMappingLegacyInput(), // Spatial Grip
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_12), // Grip Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Position
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Trigger Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton15), // Trigger Press (Select)
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_19, axisCodeY: ControllerMappingLibrary.AXIS_20, invertYAxis: true), // Touchpad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton17), // Touchpad Touch
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9), // Touchpad Press
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Menu Press
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true), // Thumbstick Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton19), // Thumbstick Press
        };
    }
}
