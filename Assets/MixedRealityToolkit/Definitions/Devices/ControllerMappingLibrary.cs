// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor.Setup;
using UnityEditor;
using UnityEngine;
#endif

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    /// <summary>
    /// Helper utility to manage all the required Axis configuration for platforms, where required
    /// </summary>
    public static class ControllerMappingLibrary
    {
        #region Constants

        /// <summary>
        /// Mouse: Position Horizontal Movement<para/>
        /// HTC Vive Controller: Left Controller Trackpad (2) Horizontal Movement<para/>
        /// Oculus Touch Controller: Axis2D.PrimaryThumbstick Horizontal Movement<para/>
        /// Valve Knuckles Controller: Left Controller Trackpad Horizontal Movement<para/>
        /// Windows Mixed Reality Motion Controller: Left Thumbstick Horizontal Movement<para/>
        /// Xbox Controller: Left Thumbstick Horizontal Movement<para/>
        /// </summary>
        public const string AXIS_1 = "AXIS_1";

        /// <summary>
        /// Mouse: Position Vertical Movement<para/>
        /// HTC Vive Controller: Left Controller Trackpad (2) Vertical Movement<para/>
        /// Oculus Touch Controller: Axis2D.PrimaryThumbstick Vertical Movement<para/>
        /// Valve Knuckles Controller: Left Controller Trackpad Vertical Movement<para/>
        /// Windows Mixed Reality Motion Controller: Left Thumbstick Vertical Movement<para/>
        /// Xbox Controller: Left Thumbstick Vertical Movement<para/>
        /// </summary>
        public const string AXIS_2 = "AXIS_2";

        /// <summary>
        /// Mouse: Scroll<para/>
        /// Xbox Controller: Shared Trigger<para/>
        /// </summary>
        public const string AXIS_3 = "AXIS_3";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trackpad (2) Horizontal Movement<para/>
        /// Oculus Touch Controller: Axis2D.SecondaryThumbstick Horizontal Movement<para/>
        /// Valve Knuckles Controller: Right Controller Trackpad Horizontal Movement<para/>
        /// Windows Mixed Reality Motion Controller: Right Thumbstick Horizontal Movement<para/>
        /// Xbox Controller: Right Thumbstick Vertical Movement<para/>
        /// </summary>
        public const string AXIS_4 = "AXIS_4";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trackpad (2) Vertical Movement<para/>
        /// Oculus Touch Controller: Axis2D.SecondaryThumbstick Vertical Movement<para/>
        /// Valve Knuckles Controller: Right Controller Trackpad Vertical Movement<para/>
        /// Windows Mixed Reality Motion Controller: Right Thumbstick Vertical Movement<para/>
        /// Xbox Controller: Right Thumbstick Vertical Movement<para/>
        /// </summary>
        public const string AXIS_5 = "AXIS_5";

        /// <summary>
        /// None
        /// </summary>
        public const string AXIS_6 = "AXIS_6";

        /// <summary>
        /// Xbox Controller: D-Pad Horizontal<para/>
        /// </summary>
        public const string AXIS_7 = "AXIS_7";

        /// <summary>
        /// Xbox Controller: D-Pad Vertical<para/>
        /// </summary>
        public const string AXIS_8 = "AXIS_8";

        /// <summary>
        /// HTC Vive Controller: Left Controller Trigger (7) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.PrimaryIndexTrigger Squeeze<para/>
        /// Valve Knuckles Controller: Left Controller Trigger Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Left Trigger Squeeze<para/>
        /// </summary>
        public const string AXIS_9 = "AXIS_9";

        /// <summary>
        /// HTC Vive Controller: Right Controller Trigger (7) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.SecondaryIndexTrigger Movement Squeeze<para/>
        /// Valve Knuckles Controller: Right Controller Trigger Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Right Trigger Squeeze<para/>
        /// </summary>
        public const string AXIS_10 = "AXIS_10";

        /// <summary>
        /// HTC Vive Controller: Left Controller Grip Button (8) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.PrimaryHandTrigger Squeeze<para/>
        /// Valve Knuckles Controller: 	Left Controller Grip Average Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Left Grip Squeeze<para/>
        /// </summary>
        public const string AXIS_11 = "AXIS_11";

        /// <summary>
        /// HTC Vive Controller: Right Controller Grip Button (8) Squeeze<para/>
        /// Oculus Touch Controller: Axis1D.SecondaryHandTrigger Squeeze<para/>
        /// Valve Knuckles Controller: Right Controller Grip Average Squeeze<para/>
        /// Windows Mixed Reality Motion Controller: Right Grip Squeeze<para/>
        /// </summary>
        public const string AXIS_12 = "AXIS_12";

        /// <summary>
        /// Oculus Touch Controller: Axis1D.PrimaryIndexTrigger Near Touch<para/>
        /// </summary>
        public const string AXIS_13 = "AXIS_13";

        /// <summary>
        /// Oculus Touch Controller: Axis1D.SecondaryIndexTrigger Near Touch<para/>
        /// </summary>
        public const string AXIS_14 = "AXIS_14";

        /// <summary>
        /// Oculus Touch Controller: Touch.PrimaryThumbRest Near Touch<para/>
        /// </summary>
        public const string AXIS_15 = "AXIS_15";

        /// <summary>
        /// Oculus Touch Controller: Button.SecondaryThumbstick Near Touch<para/>
        /// </summary>
        public const string AXIS_16 = "AXIS_16";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Left Touchpad Horizontal Movement<para/>
        /// </summary>
        public const string AXIS_17 = "AXIS_17";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Left Touchpad Vertical Movement<para/>
        /// </summary>
        public const string AXIS_18 = "AXIS_18";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Right Touchpad Horizontal Movement<para/>
        /// </summary>
        public const string AXIS_19 = "AXIS_19";

        /// <summary>
        /// Windows Mixed Reality Motion Controller: Right Touchpad Vertical Movement<para/>
        /// Valve Knuckles Controller: Left Controller Index Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_20 = "AXIS_20";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Index Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_21 = "AXIS_21";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Middle Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_22 = "AXIS_22";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Middle Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_23 = "AXIS_23";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Ring Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_24 = "AXIS_24";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Ring Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_25 = "AXIS_25";

        /// <summary>
        /// Valve Knuckles Controller: Left Controller Pinky Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_26 = "AXIS_26";

        /// <summary>
        /// Valve Knuckles Controller: Right Controller Pinky Finger Cap Sensor<para/>
        /// </summary>
        public const string AXIS_27 = "AXIS_27";

        #endregion Constants

#if UNITY_EDITOR

        #region InputAxisConfig

        /// <summary>
        /// Get the InputManagerAxis data needed to configure the Input Mappings for a controller
        /// </summary>
        /// <returns></returns>
        public static InputManagerAxis[] UnityInputManagerAxes => new[]
        {
            new InputManagerAxis { Name = AXIS_1,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1  },
            new InputManagerAxis { Name = AXIS_2,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 2  },
            new InputManagerAxis { Name = AXIS_3,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 3  },
            new InputManagerAxis { Name = AXIS_4,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4  },
            new InputManagerAxis { Name = AXIS_5,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 5  },
            new InputManagerAxis { Name = AXIS_6,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 6  },
            new InputManagerAxis { Name = AXIS_7,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 7  },
            new InputManagerAxis { Name = AXIS_8,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 8  },
            new InputManagerAxis { Name = AXIS_9,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 9  },
            new InputManagerAxis { Name = AXIS_10, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
            new InputManagerAxis { Name = AXIS_11, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
            new InputManagerAxis { Name = AXIS_12, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 12 },
            new InputManagerAxis { Name = AXIS_13, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 13 },
            new InputManagerAxis { Name = AXIS_14, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 14 },
            new InputManagerAxis { Name = AXIS_15, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 15 },
            new InputManagerAxis { Name = AXIS_16, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 16 },
            new InputManagerAxis { Name = AXIS_17, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 17 },
            new InputManagerAxis { Name = AXIS_18, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 18 },
            new InputManagerAxis { Name = AXIS_19, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 19 },
            new InputManagerAxis { Name = AXIS_20, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 20 },
            new InputManagerAxis { Name = AXIS_21, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 21 },
            new InputManagerAxis { Name = AXIS_22, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 22 },
            new InputManagerAxis { Name = AXIS_23, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 23 },
            new InputManagerAxis { Name = AXIS_24, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 24 },
            new InputManagerAxis { Name = AXIS_25, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 25 },
            new InputManagerAxis { Name = AXIS_26, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 26 },
            new InputManagerAxis { Name = AXIS_27, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 27 }
        };

        #endregion InputAxisConfig

        #region Controller Image Resources

        private static Texture2D genericControllerWhiteScaled;

        public static Texture2D GenericControllerWhiteScaled
        {
            get
            {
                if (genericControllerWhiteScaled == null)
                {
                    genericControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/Generic_controller_white_scaled.png", typeof(Texture2D));
                }

                return genericControllerWhiteScaled;
            }
        }

        private static Texture2D genericControllerBlackScaled;

        public static Texture2D GenericControllerBlackScaled
        {
            get
            {
                if (genericControllerBlackScaled == null)
                {
                    genericControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/Generic_controller_black_scaled.png", typeof(Texture2D));
                }

                return genericControllerBlackScaled;
            }
        }

        private static Texture2D xboxControllerWhite;

        public static Texture2D XboxControllerWhite
        {
            get
            {
                if (xboxControllerWhite == null)
                {
                    xboxControllerWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/XboxController_white.png", typeof(Texture2D));
                }

                return xboxControllerWhite;
            }
        }

        private static Texture2D xboxControllerWhiteScaled;

        public static Texture2D XboxControllerWhiteScaled
        {
            get
            {
                if (xboxControllerWhiteScaled == null)
                {
                    xboxControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/XboxController_white_scaled.png", typeof(Texture2D));
                }

                return xboxControllerWhiteScaled;
            }
        }

        private static Texture2D xboxControllerBlack;

        public static Texture2D XboxControllerBlack
        {
            get
            {
                if (xboxControllerBlack == null)
                {
                    xboxControllerBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/XboxController_black.png", typeof(Texture2D));
                }

                return xboxControllerBlack;
            }
        }

        private static Texture2D xboxControllerBlackScaled;

        public static Texture2D XboxControllerBlackScaled
        {
            get
            {
                if (xboxControllerBlackScaled == null)
                {
                    xboxControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/XboxController_black_scaled.png", typeof(Texture2D));
                }

                return xboxControllerBlackScaled;
            }
        }

        private static Texture2D oculusRemoteControllerWhite;

        public static Texture2D OculusRemoteControllerWhite
        {
            get
            {
                if (oculusRemoteControllerWhite == null)
                {
                    oculusRemoteControllerWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusRemoteController_white.png", typeof(Texture2D));
                }

                return oculusRemoteControllerWhite;
            }
        }

        private static Texture2D oculusRemoteControllerWhiteScaled;

        public static Texture2D OculusRemoteControllerWhiteScaled
        {
            get
            {
                if (oculusRemoteControllerWhiteScaled == null)
                {
                    oculusRemoteControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusRemoteController_white_scaled.png", typeof(Texture2D));
                }

                return oculusRemoteControllerWhiteScaled;
            }
        }

        private static Texture2D oculusRemoteControllerBlack;

        public static Texture2D OculusRemoteControllerBlack
        {
            get
            {
                if (oculusRemoteControllerBlack == null)
                {
                    oculusRemoteControllerBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusRemoteController_black.png", typeof(Texture2D));
                }

                return oculusRemoteControllerBlack;
            }
        }

        private static Texture2D oculusRemoteControllerBlackScaled;

        public static Texture2D OculusRemoteControllerBlackScaled
        {
            get
            {
                if (oculusRemoteControllerBlackScaled == null)
                {
                    oculusRemoteControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusRemoteController_black_scaled.png", typeof(Texture2D));
                }

                return oculusRemoteControllerBlackScaled;
            }
        }

        private static Texture2D wmrControllerLeftWhite;

        public static Texture2D WmrControllerLeftWhite
        {
            get
            {
                if (wmrControllerLeftWhite == null)
                {
                    wmrControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_left_white.png", typeof(Texture2D));
                }

                return wmrControllerLeftWhite;
            }
        }

        private static Texture2D wmrControllerLeftWhiteScaled;

        public static Texture2D WmrControllerLeftWhiteScaled
        {
            get
            {
                if (wmrControllerLeftWhiteScaled == null)
                {
                    wmrControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_left_white_scaled.png", typeof(Texture2D));
                }

                return wmrControllerLeftWhiteScaled;
            }
        }

        private static Texture2D wmrControllerLeftBlack;

        public static Texture2D WmrControllerLeftBlack
        {
            get
            {
                if (wmrControllerLeftBlack == null)
                {
                    wmrControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_left_black.png", typeof(Texture2D));
                }

                return wmrControllerLeftBlack;
            }
        }

        private static Texture2D wmrControllerLeftBlackScaled;

        public static Texture2D WmrControllerLeftBlackScaled
        {
            get
            {
                if (wmrControllerLeftBlackScaled == null)
                {
                    wmrControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_left_black_scaled.png", typeof(Texture2D));
                }

                return wmrControllerLeftBlackScaled;
            }
        }

        private static Texture2D wmrControllerRightWhite;

        public static Texture2D WmrControllerRightWhite
        {
            get
            {
                if (wmrControllerRightWhite == null)
                {
                    wmrControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_right_white.png", typeof(Texture2D));
                }

                return wmrControllerRightWhite;
            }
        }

        private static Texture2D wmrControllerRightWhiteScaled;

        public static Texture2D WmrControllerRightWhiteScaled
        {
            get
            {
                if (wmrControllerRightWhiteScaled == null)
                {
                    wmrControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_right_white_scaled.png", typeof(Texture2D));
                }

                return wmrControllerRightWhiteScaled;
            }
        }

        private static Texture2D wmrControllerRightBlack;

        public static Texture2D WmrControllerRightBlack
        {
            get
            {
                if (wmrControllerRightBlack == null)
                {
                    wmrControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_right_black.png", typeof(Texture2D));
                }

                return wmrControllerRightBlack;
            }
        }

        private static Texture2D wmrControllerRightBlackScaled;

        public static Texture2D WmrControllerRightBlackScaled
        {
            get
            {
                if (wmrControllerRightBlackScaled == null)
                {
                    wmrControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/MotionController_right_black_scaled.png", typeof(Texture2D));
                }

                return wmrControllerRightBlackScaled;
            }
        }

        private static Texture2D touchControllerLeftWhite;

        public static Texture2D TouchControllerLeftWhite
        {
            get
            {
                if (touchControllerLeftWhite == null)
                {
                    touchControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_left_white.png", typeof(Texture2D));
                }

                return touchControllerLeftWhite;
            }
        }

        private static Texture2D touchControllerLeftWhiteScaled;

        public static Texture2D TouchControllerLeftWhiteScaled
        {
            get
            {
                if (touchControllerLeftWhiteScaled == null)
                {
                    touchControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_left_white_scaled.png", typeof(Texture2D));
                }

                return touchControllerLeftWhiteScaled;
            }
        }

        private static Texture2D touchControllerLeftBlack;

        public static Texture2D TouchControllerLeftBlack
        {
            get
            {
                if (touchControllerLeftBlack == null)
                {
                    touchControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_left_black.png", typeof(Texture2D));
                }

                return touchControllerLeftBlack;
            }
        }

        private static Texture2D touchControllerLeftBlackScaled;

        public static Texture2D TouchControllerLeftBlackScaled
        {
            get
            {
                if (touchControllerLeftBlackScaled == null)
                {
                    touchControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_left_black_scaled.png", typeof(Texture2D));
                }

                return touchControllerLeftBlackScaled;
            }
        }

        private static Texture2D touchControllerRightWhite;

        public static Texture2D TouchControllerRightWhite
        {
            get
            {
                if (touchControllerRightWhite == null)
                {
                    touchControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_right_white.png", typeof(Texture2D));
                }

                return touchControllerRightWhite;
            }
        }

        private static Texture2D touchControllerRightWhiteScaled;

        public static Texture2D TouchControllerRightWhiteScaled
        {
            get
            {
                if (touchControllerRightWhiteScaled == null)
                {
                    touchControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_right_white_scaled.png", typeof(Texture2D));
                }

                return touchControllerRightWhiteScaled;
            }
        }

        private static Texture2D touchControllerRightBlack;

        public static Texture2D TouchControllerRightBlack
        {
            get
            {
                if (touchControllerRightBlack == null)
                {
                    touchControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_right_black.png", typeof(Texture2D));
                }

                return touchControllerRightBlack;
            }
        }

        private static Texture2D touchControllerRightBlackScaled;

        public static Texture2D TouchControllerRightBlackScaled
        {
            get
            {
                if (touchControllerRightBlackScaled == null)
                {
                    touchControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/OculusControllersTouch_right_black_scaled.png", typeof(Texture2D));
                }

                return touchControllerRightBlackScaled;
            }
        }

        private static Texture2D viveWandControllerLeftWhite;

        public static Texture2D ViveWandControllerLeftWhite
        {
            get
            {
                if (viveWandControllerLeftWhite == null)
                {
                    viveWandControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_left_white.png", typeof(Texture2D));
                }

                return viveWandControllerLeftWhite;
            }
        }

        private static Texture2D viveWandControllerLeftWhiteScaled;

        public static Texture2D ViveWandControllerLeftWhiteScaled
        {
            get
            {
                if (viveWandControllerLeftWhiteScaled == null)
                {
                    viveWandControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_left_white_scaled.png", typeof(Texture2D));
                }

                return viveWandControllerLeftWhiteScaled;
            }
        }

        private static Texture2D viveWandControllerLeftBlack;

        public static Texture2D ViveWandControllerLeftBlack
        {
            get
            {
                if (viveWandControllerLeftBlack == null)
                {
                    viveWandControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_left_black.png", typeof(Texture2D));
                }

                return viveWandControllerLeftBlack;
            }
        }

        private static Texture2D viveWandControllerLeftBlackScaled;

        public static Texture2D ViveWandControllerLeftBlackScaled
        {
            get
            {
                if (viveWandControllerLeftBlackScaled == null)
                {
                    viveWandControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_left_black_scaled.png", typeof(Texture2D));
                }

                return viveWandControllerLeftBlackScaled;
            }
        }

        private static Texture2D viveWandControllerRightWhite;

        public static Texture2D ViveWandControllerRightWhite
        {
            get
            {
                if (viveWandControllerRightWhite == null)
                {
                    viveWandControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_right_white.png", typeof(Texture2D));
                }

                return viveWandControllerRightWhite;
            }
        }

        private static Texture2D viveWandControllerRightWhiteScaled;

        public static Texture2D ViveWandControllerRightWhiteScaled
        {
            get
            {
                if (viveWandControllerRightWhiteScaled == null)
                {
                    viveWandControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_right_white_scaled.png", typeof(Texture2D));
                }

                return viveWandControllerRightWhiteScaled;
            }
        }

        private static Texture2D viveWandControllerRightBlack;

        public static Texture2D ViveWandControllerRightBlack
        {
            get
            {
                if (viveWandControllerRightBlack == null)
                {
                    viveWandControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_right_black.png", typeof(Texture2D));
                }

                return viveWandControllerRightBlack;
            }
        }

        private static Texture2D viveWandControllerRightBlackScaled;

        public static Texture2D ViveWandControllerRightBlackScaled
        {
            get
            {
                if (viveWandControllerRightBlackScaled == null)
                {
                    viveWandControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath($"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/_Core/Resources/Textures/ViveWandController_right_black_scaled.png", typeof(Texture2D));
                }

                return viveWandControllerRightBlackScaled;
            }
        }

        #endregion Controller Image Resources

        public static Texture2D GetControllerTexture(SupportedControllerType currentControllerType, Handedness handedness)
        {
            switch (currentControllerType)
            {
                case SupportedControllerType.ViveWand:
                    if (handedness == Handedness.Left)
                    {
                        return EditorGUIUtility.isProSkin ? ViveWandControllerLeftWhite : ViveWandControllerLeftBlack;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        return EditorGUIUtility.isProSkin ? ViveWandControllerRightWhite : ViveWandControllerRightBlack;
                    }

                    break;
                case SupportedControllerType.OculusTouch:
                    if (handedness == Handedness.Left)
                    {
                        return EditorGUIUtility.isProSkin ? TouchControllerLeftWhite : TouchControllerLeftBlack;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        return EditorGUIUtility.isProSkin ? TouchControllerRightWhite : TouchControllerRightBlack;
                    }

                    break;
                case SupportedControllerType.OculusRemote:
                    return EditorGUIUtility.isProSkin ? OculusRemoteControllerWhite : OculusRemoteControllerBlack;
                case SupportedControllerType.WindowsMixedReality:
                    if (handedness == Handedness.Left)
                    {
                        return EditorGUIUtility.isProSkin ? WmrControllerLeftWhite : WmrControllerLeftBlack;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        return EditorGUIUtility.isProSkin ? WmrControllerRightWhite : WmrControllerRightBlack;
                    }
                    else
                    {
                        // TODO Add HoloLens Image
                        return null;
                    }
                case SupportedControllerType.Xbox:
                    return EditorGUIUtility.isProSkin ? XboxControllerWhite : XboxControllerBlack;
            }

            return null;
        }

        public static Texture2D GetControllerTextureScaled(SupportedControllerType currentControllerType, Handedness handedness)
        {
            Texture2D texture = null;
            switch (currentControllerType)
            {
                case SupportedControllerType.ViveWand:
                    if (handedness == Handedness.Left)
                    {
                        texture = EditorGUIUtility.isProSkin ? ViveWandControllerLeftWhiteScaled : ViveWandControllerLeftBlackScaled;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        texture = EditorGUIUtility.isProSkin ? ViveWandControllerRightWhiteScaled : ViveWandControllerRightBlackScaled;
                    }

                    break;
                case SupportedControllerType.OculusTouch:
                    if (handedness == Handedness.Left)
                    {
                        texture = EditorGUIUtility.isProSkin ? TouchControllerLeftWhiteScaled : TouchControllerLeftBlackScaled;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        texture = EditorGUIUtility.isProSkin ? TouchControllerRightWhiteScaled : TouchControllerRightBlackScaled;
                    }

                    break;
                case SupportedControllerType.OculusRemote:
                    texture = EditorGUIUtility.isProSkin ? OculusRemoteControllerWhiteScaled : OculusRemoteControllerBlackScaled;
                    break;
                case SupportedControllerType.WindowsMixedReality:
                    if (handedness == Handedness.Left)
                    {
                        texture = EditorGUIUtility.isProSkin ? WmrControllerLeftWhiteScaled : WmrControllerLeftBlackScaled;
                    }
                    else if (handedness == Handedness.Right)
                    {
                        texture = EditorGUIUtility.isProSkin ? WmrControllerRightWhiteScaled : WmrControllerRightBlackScaled;
                    }
                    else
                    {
                        // TODO Add HoloLens Image
                    }

                    break;
                case SupportedControllerType.Xbox:
                    texture = EditorGUIUtility.isProSkin ? XboxControllerWhiteScaled : XboxControllerBlackScaled;
                    break;
            }

            if (texture == null)
            {
                texture = EditorGUIUtility.isProSkin ? GenericControllerWhiteScaled : GenericControllerBlackScaled;
            }

            return texture;
        }
#endif // UNITY_EDITOR
    }
}