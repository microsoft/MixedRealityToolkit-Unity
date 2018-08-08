// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    /// <summary>
    /// Open VR Implementation of the Windows Mixed Reality Motion Controllers.
    /// </summary>
    public class WindowsMixedRealityOpenVRController : GenericOpenVRController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public WindowsMixedRealityOpenVRController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
            : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The Generic OpenVR Controller default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public new static readonly MixedRealityInteractionMapping[] DefaultLeftHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS9),
            new MixedRealityInteractionMapping(3, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton14),
            new MixedRealityInteractionMapping(4, "Grip Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton4),
            new MixedRealityInteractionMapping(5, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.MIXEDREALITY_AXIS17, ControllerMappingLibrary.MIXEDREALITY_AXIS18),
            new MixedRealityInteractionMapping(6, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton18),
            new MixedRealityInteractionMapping(7, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton16),
            new MixedRealityInteractionMapping(8, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress,  KeyCode.JoystickButton6),
        };

        public new static readonly MixedRealityInteractionMapping[] DefaultRightHandedInteractions =
        {
            new MixedRealityInteractionMapping(0, "Spatial Pointer", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Spatial Grip", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Trigger Position", AxisType.SingleAxis, DeviceInputType.Trigger, ControllerMappingLibrary.MIXEDREALITY_AXIS10),
            new MixedRealityInteractionMapping(3, "Trigger Press (Select)", AxisType.Digital, DeviceInputType.TriggerPress,  KeyCode.JoystickButton15),
            new MixedRealityInteractionMapping(4, "Grip Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton5),
            new MixedRealityInteractionMapping(5, "Touchpad Position", AxisType.DualAxis, DeviceInputType.Touchpad, ControllerMappingLibrary.MIXEDREALITY_AXIS19, ControllerMappingLibrary.MIXEDREALITY_AXIS20),
            new MixedRealityInteractionMapping(6, "Touchpad Touch", AxisType.Digital, DeviceInputType.TouchpadTouch, KeyCode.JoystickButton19),
            new MixedRealityInteractionMapping(7, "Touchpad Press", AxisType.Digital, DeviceInputType.TouchpadPress, KeyCode.JoystickButton17),
            new MixedRealityInteractionMapping(8, "Menu Button", AxisType.Digital, DeviceInputType.ButtonPress, KeyCode.JoystickButton7),
        };

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(controllerHandedness == Handedness.Left ? DefaultLeftHandedInteractions : DefaultRightHandedInteractions);
        }
    }
}