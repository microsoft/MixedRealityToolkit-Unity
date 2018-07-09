// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    // TODO - Implement
    public class OculusTouchController : GenericOpenVRController
    {
        public OculusTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource, MixedRealityInteractionMapping[] interactions)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        private InputMappingAxisUtility.InputManagerAxis[] OcculusTouchControllerAxisMappings;

        public override InputMappingAxisUtility.InputManagerAxis[] ControllerAxisMappings => OcculusTouchControllerAxisMappings;

        /// <summary>
        /// Collection of input mapping constants, grouped in a single class for easier referencing.
        /// </summary>
        /// <remarks>
        /// Uses a fixed index array for controller input in the base / Generic class, as indicated in the array comments</remarks>
        private string[] OcculusTouchInputMappings =
        {
            "OTOUCH_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 0 - TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL
            "OTOUCH_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 1 - TOUCHPAD_LEFT_CONTROLLER_VERTICAL
            "OTOUCH_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 2 - TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL
            "OTOUCH_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 3 - TOUCHPAD_RIGHT_CONTROLLER_VERTICAL
            "OTOUCH_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 4 - THUMBSTICK_LEFT_CONTROLLER_HORIZONTAL
            "OTOUCH_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 5 - THUMBSTICK_LEFT_CONTROLLER_VERTICAL
            "OTOUCH_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 6 - THUMBSTICK_RIGHT_CONTROLLER_HORIZONTAL
            "OTOUCH_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 7 - THUMBSTICK_RIGHT_CONTROLLER_VERTICAL
            "OTOUCH_TRIGGER_LEFT_CONTROLLER",               // 8 - TRIGGER_LEFT_CONTROLLER
            "OTOUCH_TRIGGER_RIGHT_CONTROLLER",              // 9 - TRIGGER_RIGHT_CONTROLLER
            "OTOUCH_GRIP_LEFT_CONTROLLER",                  // 10 - GRIP_LEFT_CONTROLLER
            "OTOUCH_GRIP_RIGHT_CONTROLLER"                  // 11 - GRIP_RIGHT_CONTROLLER
        };

        public override string[] VRInputMappings => OcculusTouchInputMappings;

        public override void Initialise()
        {
            OcculusTouchControllerAxisMappings = new InputMappingAxisUtility.InputManagerAxis[]
            {
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[0], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 1 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[1], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 2 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[2], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 4 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[3], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 5 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[8], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 9 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[9], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 10 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[10], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 11 },
                new InputMappingAxisUtility.InputManagerAxis() { Name = VRInputMappings[11], Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputMappingAxisUtility.MappingAxisType.JoystickAxis, Axis = 12 }
            };
        }

        #endregion Base override configuration

    }
}
