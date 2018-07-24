// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices
{
    /// <summary>
    /// Helper utility to manage all the required Axis configuration for platforms, where required
    /// </summary>
    public static class ControllerMappingLibrary
    {
        #region Controller axis mapping configuration

        //Axis and Input mapping configuration for each controller type.  Centralized here... Because Unity..

        #region Constants

        /// <summary>
        /// TOUCHPAD-THUMSTICK_LEFT_CONTROLLER_HORIZONTAL
        /// </summary>
        public const string MIXEDREALITY_AXIS1 = "MIXEDREALITY_AXIS1";

        /// <summary>
        /// TOUCHPAD-THUMSTICK_LEFT_CONTROLLER_VERTICAL
        /// </summary>
        public const string MIXEDREALITY_AXIS2 = "MIXEDREALITY_AXIS2";

        /// <summary>
        /// TOUCHPAD-THUMSTICK_RIGHT_CONTROLLER_HORIZONTAL
        /// </summary>
        public const string MIXEDREALITY_AXIS4 = "MIXEDREALITY_AXIS4";

        /// <summary>
        /// TOUCHPAD-THUMSTICK_RIGHT_CONTROLLER_VERTICAL
        /// </summary>
        public const string MIXEDREALITY_AXIS5 = "MIXEDREALITY_AXIS5";

        /// <summary>
        /// TRIGGER_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS9 = "MIXEDREALITY_AXIS9";

        /// <summary>
        /// TRIGGER_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS10 = "MIXEDREALITY_AXIS10";

        /// <summary>
        /// GRIP_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS11 = "MIXEDREALITY_AXIS11";

        /// <summary>
        /// GRIP_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS12 = "MIXEDREALITY_AXIS12";

        /// <summary>
        /// TRIGGER_NEARTOUCH_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS13 = "MIXEDREALITY_AXIS13";

        /// <summary>
        /// TRIGGER_NEARTOUCH_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS14 = "MIXEDREALITY_AXIS14";

        /// <summary>
        /// THUMBSTICK_NEARTOUCH_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS15 = "MIXEDREALITY_AXIS15";

        /// <summary>
        /// THUMBSTICK_NEARTOUCH_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS16 = "MIXEDREALITY_AXIS16";

        /// <summary>
        /// THUMBREST_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS17 = "MIXEDREALITY_AXIS17";

        /// <summary>
        /// THUMBREST_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS18 = "MIXEDREALITY_AXIS18";

        /// <summary>
        /// INDEXFINGER_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS20 = "MIXEDREALITY_AXIS20";

        /// <summary>
        /// INDEXFINGER_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS21 = "MIXEDREALITY_AXIS21";

        /// <summary>
        /// MIDDLEFINGER_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS22 = "MIXEDREALITY_AXIS22";

        /// <summary>
        /// MIDDLEFINGER_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS23 = "MIXEDREALITY_AXIS23";

        /// <summary>
        /// RINGFINGER_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS24 = "MIXEDREALITY_AXIS24";

        /// <summary>
        /// RINGFINGER_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS25 = "MIXEDREALITY_AXIS25";

        /// <summary>
        /// PINKYFINGER_LEFT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS26 = "MIXEDREALITY_AXIS26";

        /// <summary>
        /// PINKYFINGER_RIGHT_CONTROLLER
        /// </summary>
        public const string MIXEDREALITY_AXIS27 = "MIXEDREALITY_AXIS27";

        #endregion Constants

        #region InputAxisConfig

        /// <summary>
        /// Get the InputManagerAxis data needed to configure the Input Mappings for a controller
        /// </summary>
        /// <param name="type">The type of controller to retrieve configuration for</param>
        /// <returns></returns>
        public static InputManagerAxis[] GetOpenVRInputManagerAxes
        {
            get
            {
                return new InputManagerAxis[]
                {
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS1, Dead = 0.1f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS2, Dead = 0.1f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 2 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS4, Dead = 0.1f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS5, Dead = 0.1f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 5 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS9, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 9 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS10, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS11, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS12, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 12 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS13, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 13 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS14, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 14 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS15, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 15 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS16, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 16 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS17, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 17 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS18, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 18 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS20, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 20 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS21, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 21 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS22, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 22 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS23, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 23 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS24, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 24 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS25, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 25 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS26, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 26 },
                    new InputManagerAxis { Name = MIXEDREALITY_AXIS27, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 27 }
                 };
            }
        }
        #endregion

        #endregion Controller axis mapping configuration

        #region Interaction Mapping Default Resolution
        // TODO: Find a better way.
        /// <summary>
        /// Retrieve the defaults for a specific controller type
        /// </summary>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        public static MixedRealityInteractionMapping[] GetMappingsForControllerType(SystemType controllerType, Handedness handedness)
        {
            if (controllerType == null)
            {
                return null;
            }

            if (controllerType == typeof(WindowsMixedRealityController))
            {
                return WindowsMixedRealityController.DefaultInteractions;
            }
            else if (controllerType == typeof(OculusTouchController))
            {
                return handedness == Handedness.Left ? OculusTouchController.DefaultLeftHandedInteractions : OculusTouchController.DefaultRightHandedInteractions;
            }
            else if (controllerType == typeof(ViveWandController))
            {
                return handedness == Handedness.Left ? ViveWandController.DefaultLeftHandedInteractions : ViveWandController.DefaultRightHandedInteractions;
            }
            else if (controllerType == typeof(ViveKnucklesController))
            {
                return handedness == Handedness.Left ? ViveKnucklesController.DefaultLeftHandedInteractions : ViveKnucklesController.DefaultRightHandedInteractions;
            }
            else if (controllerType == typeof(GenericOpenVRController))
            {
                return handedness == Handedness.Left ? GenericOpenVRController.DefaultLeftHandedInteractions : GenericOpenVRController.DefaultRightHandedInteractions;
            }

            // Unconfigured Controller type
            return null;
        }

        #endregion Interaction Mapping Default Resolution

    }
}