// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Core.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Providers.Oculus
{
    [MixedRealityController(
        SupportedControllerType.OculusGoRemote,
        new[] { Handedness.None },
        "StandardAssets/Textures/OculusGoRemoteController")]
    public class OculusGoRemoteController : GenericOculusController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusGoRemoteController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.ThreeDofRotation, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Trigger", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(2, "Back", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(2, "PrimaryTouchpad Click", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(2, "PrimaryTouchpad Axis", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5)
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }
    }
}
