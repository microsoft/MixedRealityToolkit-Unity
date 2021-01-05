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
        public HPMotionController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new HPMotionControllerDefinition(inputSource, controllerHandedness);
        }

        private readonly HPMotionControllerDefinition controllerDefinition;

        /// <inheritdoc />
        public override float PointerOffsetAngle { get; protected set; } = -30f;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions
        {
            get
            {
                MixedRealityInteractionMapping[] definitionInteractions = controllerDefinition.DefaultLeftHandedInteractions;
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
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions
        {
            get
            {
                MixedRealityInteractionMapping[] definitionInteractions = controllerDefinition.DefaultRightHandedInteractions;
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