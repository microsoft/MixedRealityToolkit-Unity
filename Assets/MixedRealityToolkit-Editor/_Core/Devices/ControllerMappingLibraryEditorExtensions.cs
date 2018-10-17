// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities.Editor;
using UnityEditor;
using UnityEngine;
#endif


namespace Microsoft.MixedReality.Toolkit.Core.Definitions.Devices
{
    /// <summary>
    /// Helper utility to manage all the required Axis configuration for platforms, where required
    /// </summary>
    public static class ControllerMappingLibraryEditorExtensions
    {
#if UNITY_EDITOR

        #region InputAxisConfig

        /// <summary>
        /// Get the InputManagerAxis data needed to configure the Input Mappings for a controller
        /// </summary>
        /// <returns></returns>
        public static InputManagerAxis[] UnityInputManagerAxes => new[]
        {
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_1,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 1  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_2,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 2  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_3,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 3  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_4,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 4  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_5,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 5  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_6,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 6  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_7,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 7  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_8,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 8  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_9,  Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 9  },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_10, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 10 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_11, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 11 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_12, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 12 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_13, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 13 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_14, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 14 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_15, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 15 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_16, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 16 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_17, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 17 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_18, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 18 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_19, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 19 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_20, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 20 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_21, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 21 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_22, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 22 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_23, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 23 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_24, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 24 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_25, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 25 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_26, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 26 },
            new InputManagerAxis { Name = ControllerMappingLibrary.AXIS_27, Dead = 0.001f, Sensitivity = 1, Invert = false, Type = InputManagerAxisType.JoystickAxis, Axis = 27 }
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
                    genericControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/Generic_controller_white_scaled.png", typeof(Texture2D));
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
                    genericControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/Generic_controller_black_scaled.png", typeof(Texture2D));
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
                    xboxControllerWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_white.png", typeof(Texture2D));
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
                    xboxControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_white_scaled.png", typeof(Texture2D));
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
                    xboxControllerBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_black.png", typeof(Texture2D));
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
                    xboxControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/XboxController_black_scaled.png", typeof(Texture2D));
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
                    oculusRemoteControllerWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusRemoteController_white.png", typeof(Texture2D));
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
                    oculusRemoteControllerWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusRemoteController_white_scaled.png", typeof(Texture2D));
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
                    oculusRemoteControllerBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusRemoteController_black.png", typeof(Texture2D));
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
                    oculusRemoteControllerBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusRemoteController_black_scaled.png", typeof(Texture2D));
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
                    wmrControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_white.png", typeof(Texture2D));
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
                    wmrControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_white_scaled.png", typeof(Texture2D));
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
                    wmrControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_black.png", typeof(Texture2D));
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
                    wmrControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_left_black_scaled.png", typeof(Texture2D));
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
                    wmrControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_white.png", typeof(Texture2D));
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
                    wmrControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_white_scaled.png", typeof(Texture2D));
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
                    wmrControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_black.png", typeof(Texture2D));
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
                    wmrControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MotionController_right_black_scaled.png", typeof(Texture2D));
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
                    touchControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_white.png", typeof(Texture2D));
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
                    touchControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_white_scaled.png", typeof(Texture2D));
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
                    touchControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_black.png", typeof(Texture2D));
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
                    touchControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_left_black_scaled.png", typeof(Texture2D));
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
                    touchControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_white.png", typeof(Texture2D));
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
                    touchControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_white_scaled.png", typeof(Texture2D));
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
                    touchControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_black.png", typeof(Texture2D));
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
                    touchControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/OculusControllersTouch_right_black_scaled.png", typeof(Texture2D));
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
                    viveWandControllerLeftWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_white.png", typeof(Texture2D));
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
                    viveWandControllerLeftWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_white_scaled.png", typeof(Texture2D));
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
                    viveWandControllerLeftBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_black.png", typeof(Texture2D));
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
                    viveWandControllerLeftBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_left_black_scaled.png", typeof(Texture2D));
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
                    viveWandControllerRightWhite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_white.png", typeof(Texture2D));
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
                    viveWandControllerRightWhiteScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_white_scaled.png", typeof(Texture2D));
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
                    viveWandControllerRightBlack = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_black.png", typeof(Texture2D));
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
                    viveWandControllerRightBlackScaled = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/ViveWandController_right_black_scaled.png", typeof(Texture2D));
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