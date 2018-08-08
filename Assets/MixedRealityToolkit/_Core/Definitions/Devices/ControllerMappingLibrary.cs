// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR;
using Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality;

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
        /// HTC Vive Controller: Left Controller Trackpad (2) Horizontal Movement<para/>
        /// Oculus Touch Controller: Axis2D.PrimaryThumbstick Horizontal Movement<para/>
        /// Valve Knuckles Controller: Left Controller Trackpad Horizontal Movement<para/>
        /// Windows Mixed Reality Motion Controller: Left Thumbstick Horizontal Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS1 = "MIXEDREALITY_AXIS1";

        /// <summary>
        /// HTC Vive Controller: Left Controller Trackpad (2) Vertical Movement<para/>
        /// Oculus Touch Controller: Axis2D.PrimaryThumbstick Vertical Movement<para/>
        /// Valve Knuckles Controller: Left Controller Trackpad Vertical Movement<para/>
        /// Windows Mixed Reality Motion Controller: Left Thumbstick Vertical Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS2 = "MIXEDREALITY_AXIS2";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trackpad (2) Horizontal Movement<para/>
        /// Oculus Touch Controller: Axis2D.SecondaryThumbstick Horizontal Movement<para/>
        /// Valve Knuckles Controller: Right Controller Trackpad Horizontal Movement<para/>
        /// Windows Mixed Reality Motion Controller: Right Thumbstick Horizontal Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS4 = "MIXEDREALITY_AXIS4";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trackpad (2) Vertical Movement<para/>
        /// Oculus Touch Controller: Axis2D.SecondaryThumbstick Vertical Movement<para/>
        /// Valve Knuckles Controller: Right Controller Trackpad Vertical Movement<para/>
        /// Windows Mixed Reality Motion Controller: Right Thumbstick Vertical Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS5 = "MIXEDREALITY_AXIS5";

        /// <summary>
        /// HTC Vive Controller: Left Controller Trigger (7) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.PrimaryIndexTrigger Squeeze<para/>
        /// Valve Knuckles Controller: Left Controller Trigger Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Left Trigger Squeeze<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS9 = "MIXEDREALITY_AXIS9";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trigger (7) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.SecondaryIndexTrigger Movement Squeeze<para/>
        /// Valve Knuckles Controller: Right Controller Trigger Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Right Trigger Squeeze<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS10 = "MIXEDREALITY_AXIS10";

        /// <summary>
        /// HTC Vive Controller: Left Controller Grip Button (8) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.PrimaryHandTrigger Squeeze<para/>
        /// Valve Knuckles Controller: 	Left Controller Grip Average Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Left Grip Squeeze<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS11 = "MIXEDREALITY_AXIS11";

        /// <summary>
        /// HTC Vive Controller: Right Controller Grip Button (8) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.SecondaryHandTrigger Squeeze<para/>
        /// Valve Knuckles Controller: Right Controller Grip Average Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Right Grip Squeeze<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS12 = "MIXEDREALITY_AXIS12";

        /// <summary>
        /// Oculus Touch Controller: Axis1D.PrimaryIndexTrigger Near Touch<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS13 = "MIXEDREALITY_AXIS13";

        /// <summary>
        /// Oculus Touch Controller: Axis1D.SecondaryIndexTrigger Near Touch<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS14 = "MIXEDREALITY_AXIS14";

        /// <summary>
        /// Oculus Touch Controller: Touch.PrimaryThumbRest Near Touch<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS15 = "MIXEDREALITY_AXIS15";

        /// <summary>
        /// Oculus Touch Controller: Button.SecondaryThumbstick Near Touch<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS16 = "MIXEDREALITY_AXIS16";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Left Touchpad Horizontal Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS17 = "MIXEDREALITY_AXIS17";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Left Touchpad Vertical Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS18 = "MIXEDREALITY_AXIS18";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Right Touchpad Horizontal Movement<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS19 = "MIXEDREALITY_AXIS19";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Right Touchpad Vertical Movement<para/>
        /// Valve Knuckles Controller: Left Controller Index Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS20 = "MIXEDREALITY_AXIS20";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Index Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS21 = "MIXEDREALITY_AXIS21";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Middle Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS22 = "MIXEDREALITY_AXIS22";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Middle Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS23 = "MIXEDREALITY_AXIS23";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Ring Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS24 = "MIXEDREALITY_AXIS24";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Ring Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS25 = "MIXEDREALITY_AXIS25";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Pinky Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS26 = "MIXEDREALITY_AXIS26";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Pinky Finger Cap Sensor<para/>
        /// </summary>
        public const string MIXEDREALITY_AXIS27 = "MIXEDREALITY_AXIS27";

        #endregion Constants

        #region InputAxisConfig

        /// <summary>
        /// Get the InputManagerAxis data needed to configure the Input Mappings for a controller
        /// </summary>
        /// <returns></returns>
        public static InputManagerAxis[] UnityInputManagerAxes => new[]
        {
            new InputManagerAxis { Name = MIXEDREALITY_AXIS1,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1  },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS2,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 2  },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS4,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4  },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS5,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 5  },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS9,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 9  },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS10, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS11, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS12, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 12 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS13, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 13 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS14, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 14 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS15, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 15 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS16, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 16 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS17, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 17 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS18, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 18 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS19, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 19 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS20, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 20 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS21, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 21 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS22, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 22 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS23, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 23 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS24, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 24 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS25, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 25 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS26, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 26 },
            new InputManagerAxis { Name = MIXEDREALITY_AXIS27, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 27 }
        };

        #endregion

        #endregion Controller axis mapping configuration

        #region Interaction Mapping Default Resolution
        // TODO: Find a better way.
        /// <summary>
        /// Retrieve the defaults for a specific controller type
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="handedness"></param>
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

            // For our open VR controllers we expect either left or right handedness
            if (handedness != Handedness.Left && handedness != Handedness.Right)
            {
                return null;
            }

            if (controllerType == typeof(OculusTouchController))
            {
                return handedness == Handedness.Left ? OculusTouchController.DefaultLeftHandedInteractions : OculusTouchController.DefaultRightHandedInteractions;
            }

            if (controllerType == typeof(ViveWandController))
            {
                return handedness == Handedness.Left ? ViveWandController.DefaultLeftHandedInteractions : ViveWandController.DefaultRightHandedInteractions;
            }

            if (controllerType == typeof(ViveKnucklesController))
            {
                return handedness == Handedness.Left ? ViveKnucklesController.DefaultLeftHandedInteractions : ViveKnucklesController.DefaultRightHandedInteractions;
            }

            if (controllerType == typeof(WindowsMixedRealityOpenVRController))
            {
                return handedness == Handedness.Left ? WindowsMixedRealityOpenVRController.DefaultLeftHandedInteractions : WindowsMixedRealityOpenVRController.DefaultRightHandedInteractions;
            }

            if (controllerType == typeof(GenericOpenVRController))
            {
                return handedness == Handedness.Left ? GenericOpenVRController.DefaultLeftHandedInteractions : GenericOpenVRController.DefaultRightHandedInteractions;
            }

            // Unconfigured Controller type
            return null;
        }

        #endregion Interaction Mapping Default Resolution

    }
}