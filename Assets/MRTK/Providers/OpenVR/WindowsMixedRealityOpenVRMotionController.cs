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
        public WindowsMixedRealityOpenVRMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new WindowsMixedRealityControllerDefinition(inputSource, controllerHandedness);
        }

        private readonly WindowsMixedRealityControllerDefinition controllerDefinition;

        /// <inheritdoc />
        public override float PointerOffsetAngle { get; protected set; } = -30f;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions
        {
            get
            {
                MixedRealityInteractionMapping[] definitionInteractions = controllerDefinition.DefaultInteractions;
                MixedRealityInteractionMapping[] defaultLeftHandedInteractions = new MixedRealityInteractionMapping[definitionInteractions.Length];
                for (int i = 0; i < definitionInteractions.Length; i++)
                {
                    defaultLeftHandedInteractions[i] = new MixedRealityInteractionMapping(definitionInteractions[i], LeftHandedLegacyInputSupport[i]);
                }
                return defaultLeftHandedInteractions;
            }
        }

        private static readonly MixedRealityInteractionMappingLegacyInput[] LeftHandedLegacyInputSupport = new[]
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
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions
        {
            get
            {
                MixedRealityInteractionMapping[] definitionInteractions = controllerDefinition.DefaultInteractions;
                MixedRealityInteractionMapping[] defaultRightHandedInteractions = new MixedRealityInteractionMapping[definitionInteractions.Length];
                for (int i = 0; i < definitionInteractions.Length; i++)
                {
                    defaultRightHandedInteractions[i] = new MixedRealityInteractionMapping(definitionInteractions[i], RightHandedLegacyInputSupport[i]);
                }
                return defaultRightHandedInteractions;
            }
        }


        private static readonly MixedRealityInteractionMappingLegacyInput[] RightHandedLegacyInputSupport = new[]
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