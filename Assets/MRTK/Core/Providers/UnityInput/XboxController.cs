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
        }

        /// <summary>
        /// Default interactions for Xbox Controller using Unity Input System.
        /// </summary>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Left Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_1, ControllerMappingLibrary.AXIS_2, false, true),
            new MixedRealityInteractionMapping(1, "Left Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton8),
            new MixedRealityInteractionMapping(2, "Right Thumbstick", AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerMappingLibrary.AXIS_4, ControllerMappingLibrary.AXIS_5, false, true),
            new MixedRealityInteractionMapping(3, "Right Thumbstick Click", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton9),
            new MixedRealityInteractionMapping(4, "D-Pad", AxisType.DualAxis, DeviceInputType.DirectionalPad, ControllerMappingLibrary.AXIS_6, ControllerMappingLibrary.AXIS_7, false, true),
            new MixedRealityInteractionMapping(5, "Shared Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_3),
            new MixedRealityInteractionMapping(6, "Left Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_9),
            new MixedRealityInteractionMapping(7, "Right Trigger", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.AXIS_10),
            new MixedRealityInteractionMapping(8, "View", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton6),
            new MixedRealityInteractionMapping(9, "Menu", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton7),
            new MixedRealityInteractionMapping(10, "Left Bumper", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton4),
            new MixedRealityInteractionMapping(11, "Right Bumper", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton5),
            new MixedRealityInteractionMapping(12, "A", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton0),
            new MixedRealityInteractionMapping(13, "B", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton1),
            new MixedRealityInteractionMapping(14, "X", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton2),
            new MixedRealityInteractionMapping(15, "Y", AxisType.Digital, DeviceInputType.ButtonPress,KeyCode.JoystickButton3),
        };
    }
}