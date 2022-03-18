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
        "Textures/OculusRemoteController",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
    public class OculusRemoteController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusRemoteController(
            TrackingState trackingState,
            Handedness controllerHandedness,
            IMixedRealityInputSource inputSource = null,
            MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, new OculusRemoteControllerDefinition(), inputSource, interactions)
        { }

        /// <inheritdoc />
        protected override MixedRealityInteractionMappingLegacyInput[] LegacyInputSupport { get; } = new[]
        {
            new MixedRealityInteractionMappingLegacyInput(axisCodeX: ControllerMappingLibrary.AXIS_5, axisCodeY: ControllerMappingLibrary.AXIS_6), // D-Pad Position
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton0), // Button.One
            new MixedRealityInteractionMappingLegacyInput(keyCode: KeyCode.JoystickButton1), // Button.Two
        };
    }
}
