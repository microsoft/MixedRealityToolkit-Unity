// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    // TODO - Implement
    public class HTCViveController : GenericOpenVRController
    {
        public HTCViveController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions) { }

        #region Base override configuration

        private InputMappingAxisUtility.InputManagerAxis[] HTCViveControllerAxisMappings;

        public override InputMappingAxisUtility.InputManagerAxis[] ControllerAxisMappings => HTCViveControllerAxisMappings;

        /// <summary>
        /// Collection of input mapping constants, grouped in a single class for easier referencing.
        /// </summary>
        /// <remarks>
        /// Uses a fixed index array for controller input in the base / Generic class, as indicated in the array comments</remarks>
        private string[] HTCViveInputMappings =
        {
            "VIVE_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 0 - TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL
            "VIVE_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 1 - TOUCHPAD_LEFT_CONTROLLER_VERTICAL
            "VIVE_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 2 - TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL
            "VIVE_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 3 - TOUCHPAD_RIGHT_CONTROLLER_VERTICAL
            "VIVE_TOUCHPAD_LEFT_CONTROLLER_HORIZONTAL",   // 4 - THUMBSTICK_LEFT_CONTROLLER_HORIZONTAL
            "VIVE_TOUCHPAD_LEFT_CONTROLLER_VERTICAL",     // 5 - THUMBSTICK_LEFT_CONTROLLER_VERTICAL
            "VIVE_TOUCHPAD_RIGHT_CONTROLLER_HORIZONTAL",  // 6 - THUMBSTICK_RIGHT_CONTROLLER_HORIZONTAL
            "VIVE_TOUCHPAD_RIGHT_CONTROLLER_VERTICAL",    // 7 - THUMBSTICK_RIGHT_CONTROLLER_VERTICAL
            "VIVE_TRIGGER_LEFT_CONTROLLER",               // 8 - TRIGGER_LEFT_CONTROLLER
            "VIVE_TRIGGER_RIGHT_CONTROLLER",              // 9 - TRIGGER_RIGHT_CONTROLLER
            "VIVE_GRIP_LEFT_CONTROLLER",                  // 10 - GRIP_LEFT_CONTROLLER
            "VIVE_GRIP_RIGHT_CONTROLLER"                  // 11 - GRIP_RIGHT_CONTROLLER
        };

        public override string[] VRInputMappings => HTCViveInputMappings;

        public override void Initialise()
        {
            HTCViveControllerAxisMappings = new InputMappingAxisUtility.InputManagerAxis[]
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
