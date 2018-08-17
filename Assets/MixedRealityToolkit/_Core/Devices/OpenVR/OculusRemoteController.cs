// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    // TODO - Implement
    public class OculusRemoteController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public OculusRemoteController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(TrackingState.NotApplicable, Handedness.None, inputSource, interactions) { }

        public static readonly MixedRealityInteractionMapping[] DefaultInteractions =
        {
            // Button.One
            new MixedRealityInteractionMapping(0, "Unity Button Id 0", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton0),
            // Button.Two
            new MixedRealityInteractionMapping(1, "Unity Button Id 1", AxisType.Digital, DeviceInputType.ButtonPress, MixedRealityInputAction.None, KeyCode.JoystickButton1),
            // Button.DpadRight, Button.DpadLeft, Button.DpadUp, Button.DpadDown
            new MixedRealityInteractionMapping(2, "DPad", AxisType.DualAxis, DeviceInputType.DPad, MixedRealityInputAction.None, axisCodeX: ControllerMappingLibrary.MIXEDREALITY_AXIS5, axisCodeY: ControllerMappingLibrary.MIXEDREALITY_AXIS6),
            // Button.DpadUp
            new MixedRealityInteractionMapping(3, "DPad Up", AxisType.SingleAxis, DeviceInputType.ButtonPress, MixedRealityInputAction.None, axisCodeX: ControllerMappingLibrary.MIXEDREALITY_AXIS6),
            // Button.DpadDown
            new MixedRealityInteractionMapping(4, "DPad Down", AxisType.NegativeSingleAxis, DeviceInputType.ButtonPress, MixedRealityInputAction.None, axisCodeX: ControllerMappingLibrary.MIXEDREALITY_AXIS6),
            // Button.DpadRight
            new MixedRealityInteractionMapping(5, "DPad Right", AxisType.SingleAxis, DeviceInputType.ButtonPress, MixedRealityInputAction.None, axisCodeX: ControllerMappingLibrary.MIXEDREALITY_AXIS5),
            // Button.DpadLeft
            new MixedRealityInteractionMapping(6, "DPad Left", AxisType.NegativeSingleAxis, DeviceInputType.ButtonPress, MixedRealityInputAction.None, axisCodeX: ControllerMappingLibrary.MIXEDREALITY_AXIS5),

        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }
    }
}
