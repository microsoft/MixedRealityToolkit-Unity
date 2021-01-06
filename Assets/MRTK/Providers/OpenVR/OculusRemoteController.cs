// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusRemote,
        new[] { Handedness.None },
        "Textures/OculusRemoteController")]
    public class OculusRemoteController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusRemoteController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
            controllerDefinition = new OculusRemoteControllerDefinition();
        }

        OculusRemoteControllerDefinition controllerDefinition;

        /// <inheritdoc />
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
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_5, axisCodeY: ControllerMappingLibrary.AXIS_6), // D-Pad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Button.One
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // Button.Two
        };
    }
}
