// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Attributes;
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

        private static Dictionary<Tuple<Type, Handedness, string>, Texture2D> cachedTextures = new Dictionary<Tuple<Type, Handedness, string>, Texture2D>();

        public static Texture2D GetControllerTexture(Type controllerType, Handedness handedness)
        {
            return GetControllerTextureCached(controllerType, handedness, "");
        }

        public static Texture2D GetControllerTextureScaled(Type controllerType, Handedness handedness)
        {
            return GetControllerTextureCached(controllerType, handedness, "_scaled");
        }

        private static Texture2D GetControllerTextureCached(Type controllerType, Handedness handedness, string suffix)
        {
            Texture2D texture;

            var key = new Tuple<Type, Handedness, string>(controllerType, handedness, suffix);
            if (cachedTextures.TryGetValue(key, out texture))
            {
                return texture;
            }

            texture = GetControllerTextureInternal(controllerType, handedness, suffix);
            cachedTextures.Add(key, texture);
            return texture;
        }

        private static Texture2D GetControllerTextureInternal(Type controllerType, Handedness handedness, string suffix)
        {
            if (controllerType != null)
            {
                var attr = MixedRealityControllerAttribute.Find(controllerType);
                if (attr != null)
                {
                    if (attr.TexturePath.Length > 0)
                    {
                        Texture2D texture = GetControllerTextureInternal(attr.TexturePath, handedness, suffix);
                        if (texture != null)
                        {
                            return texture;
                        }
                    }
                }
            }

            return GetControllerTextureInternal("StandardAssets/Textures/Generic_controller", Handedness.None, suffix);
        }

        private static Texture2D GetControllerTextureInternal(string relativeTexturePath, Handedness handedness, string suffix)
        {
            string handednessSuffix = string.Empty;
            if (handedness == Handedness.Left) handednessSuffix = "_left";
            else if (handedness == Handedness.Right) handednessSuffix = "_right";

            string themeSuffix = EditorGUIUtility.isProSkin ? "_white" : "_black";

            string fullTexturePath = $"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/{relativeTexturePath}{handednessSuffix}{themeSuffix}{suffix}.png";
            return (Texture2D)AssetDatabase.LoadAssetAtPath(fullTexturePath, typeof(Texture2D));
        }

#endif // UNITY_EDITOR
    }
}