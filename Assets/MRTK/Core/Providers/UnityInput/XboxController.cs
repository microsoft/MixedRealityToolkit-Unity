// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Xbox Controller using Unity Input System
    /// </summary>
    [MixedRealityController(
        SupportedControllerType.Xbox,
        new[] { Handedness.None },
        "Textures/XboxController")]
    public class XboxController : GenericJoystickController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public XboxController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new XboxControllerDefinition();
        }

        private readonly XboxControllerDefinition controllerDefinition;

        /// <summary>
        /// Default interactions for Xbox Controller using Unity Input System.
        /// </summary>
        public override MixedRealityInteractionMapping[] DefaultInteractions
        {
            get
            {
                System.Collections.Generic.IReadOnlyList<MixedRealityInteractionMapping> definitionInteractions = controllerDefinition?.GetDefaultInteractions();
                MixedRealityInteractionMapping[] defaultInteractions = new MixedRealityInteractionMapping[definitionInteractions.Count];
                for (int i = 0; i < definitionInteractions.Count; i++)
                {
                    defaultInteractions[i] = new MixedRealityInteractionMapping(definitionInteractions[i], LegacyInputSupport[i]);
                }
                return defaultInteractions;
            }
        }

        private static readonly MixedRealityInteractionMappingLegacyInput[] LegacyInputSupport = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_1, axisCodeY: ControllerMappingLibrary.AXIS_2, invertYAxis: true), // Left Thumbstick
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton8), // Left Thumbstick Click
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_4, axisCodeY: ControllerMappingLibrary.AXIS_5, invertYAxis: true), // Right Thumbstick
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton9), // Right Thumbstick Click
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_6, axisCodeY: ControllerMappingLibrary.AXIS_7, invertYAxis: true), // D-Pad
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_3), // Shared Trigger
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_9), // Left Trigger
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_10), // Right Trigger
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton6), // View
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton7), // Menu
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton4), // Left Bumper
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton5), // Right Bumper
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // A
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // B
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton2), // X
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton3), // Y
        };
    }
}