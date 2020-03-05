// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    [MixedRealityController(
        SupportedControllerType.OculusRemote,
        new[] { Handedness.None },
        "StandardAssets/Textures/OculusRemoteController")]
    public class OculusRemoteController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OculusRemoteController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "D-Pad Position", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_5, ControllerMappingLibrary.AXIS_6),
            new MixedRealityInteractionMapping(1, "Button.One", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(2, "Button.Two", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton1),
        };
    }
}
