// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Utilities
{
    /// <summary>
    /// Utility class for Unity's Input Manager Mappings.
    /// Input from all types should be defined here for use 
    /// though out the entire toolkit.
    /// </summary>
    public static class InputMappingAxisUtility
    {
        #region Mixed Reality Xbox and Motion Controller constants

        public const string CONTROLLER_LEFT_STICK_HORIZONTAL = "CONTROLLER_LEFT_STICK_HORIZONTAL";
        public const string CONTROLLER_LEFT_STICK_VERTICAL = "CONTROLLER_LEFT_STICK_VERTICAL";
        public const string CONTROLLER_RIGHT_STICK_HORIZONTAL = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
        public const string CONTROLLER_RIGHT_STICK_VERTICAL = "CONTROLLER_RIGHT_STICK_VERTICAL";
        public const string XBOX_DPAD_HORIZONTAL = "XBOX_DPAD_HORIZONTAL";
        public const string XBOX_DPAD_VERTICAL = "XBOX_DPAD_VERTICAL";
        public const string CONTROLLER_LEFT_TRIGGER = "CONTROLLER_LEFT_TRIGGER";
        public const string CONTROLLER_RIGHT_TRIGGER = "CONTROLLER_RIGHT_TRIGGER";
        public const string XBOX_SHARED_TRIGGER = "XBOX_SHARED_TRIGGER";
        public const string XBOX_A = "XBOX_A";
        public const string XBOX_B = "XBOX_B";
        public const string XBOX_X = "XBOX_X";
        public const string XBOX_Y = "XBOX_Y";
        public const string CONTROLLER_LEFT_MENU = "CONTROLLER_LEFT_MENU";
        public const string CONTROLLER_RIGHT_MENU = "CONTROLLER_RIGHT_MENU";
        public const string CONTROLLER_LEFT_BUMPER_OR_GRIP = "XBOX_LEFT_BUMPER";
        public const string CONTROLLER_RIGHT_BUMPER_OR_GRIP = "XBOX_RIGHT_BUMPER";
        public const string CONTROLLER_LEFT_STICK_CLICK = "CONTROLLER_LEFT_STICK_CLICK";
        public const string CONTROLLER_RIGHT_STICK_CLICK = "CONTROLLER_RIGHT_STICK_CLICK";

        #endregion Mixed Reality Xbox and Motion Controller constants

        #region Nested Types

        /// <summary>
        /// Used to map AxisType from a useful name to the int value the InputManager wants.
        /// </summary>
        public enum MappingAxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement,
            JoystickAxis
        };

        /// <summary>
        /// Used to define an entire InputManagerAxis, with each variable defined by the same term the Inspector shows.
        /// </summary>
        public class InputManagerAxis
        {
            public string Name = "";
            public string DescriptiveName = "";
            public string DescriptiveNegativeName = "";
            public string NegativeButton = "";
            public string PositiveButton = "";
            public string AltNegativeButton = "";
            public string AltPositiveButton = "";
            public float Gravity = 0.0f;
            public float Dead = 0.0f;
            public float Sensitivity = 0.0f;
            public bool Snap = false;
            public bool Invert = false;
            public MappingAxisType Type = default(MappingAxisType);
            public int Axis = 0;
            public int JoyNum = 0;
        }

        #endregion Nested Types

        #region Mixed Reality Default Mappings configuration

        /// <summary>
        /// This is used to keep a local list of axis names, so we don't have to keep iterating through each SerializedProperty.
        /// </summary>
        private static List<string> AxisNames = new List<string>();

        /// <summary>
        /// This is used to keep a single reference to InputManager.asset, refreshed when necessary.
        /// </summary>
        private static SerializedObject inputManagerAsset;

        /// <summary>
        /// Define new axes here adding a new InputManagerAxis to the array.
        /// </summary>
        private static readonly InputManagerAxis[] MRTKDefaultInputAxis =
        {
            new InputManagerAxis() { Name = CONTROLLER_LEFT_STICK_HORIZONTAL,  Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_LEFT_STICK_VERTICAL,    Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = MappingAxisType.JoystickAxis, Axis = 2 },
            new InputManagerAxis() { Name = XBOX_SHARED_TRIGGER,               Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 3 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_STICK_HORIZONTAL, Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 4 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_STICK_VERTICAL,   Dead = 0.19f, Sensitivity = 1, Invert = true,  Type = MappingAxisType.JoystickAxis, Axis = 5 },
            new InputManagerAxis() { Name = XBOX_DPAD_HORIZONTAL,              Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 6 },
            new InputManagerAxis() { Name = XBOX_DPAD_VERTICAL,                Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 7 },
            new InputManagerAxis() { Name = CONTROLLER_LEFT_TRIGGER,           Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 9 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_TRIGGER,          Dead = 0.19f, Sensitivity = 1, Invert = false, Type = MappingAxisType.JoystickAxis, Axis = 10 },

            new InputManagerAxis() { Name = XBOX_A,                          PositiveButton = "joystick button 0", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = XBOX_B,                          PositiveButton = "joystick button 1", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = XBOX_X,                          PositiveButton = "joystick button 2", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = XBOX_Y,                          PositiveButton = "joystick button 3", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_LEFT_BUMPER_OR_GRIP,  PositiveButton = "joystick button 4", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_BUMPER_OR_GRIP, PositiveButton = "joystick button 5", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_LEFT_MENU,            PositiveButton = "joystick button 6", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_MENU,           PositiveButton = "joystick button 7", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_LEFT_STICK_CLICK,     PositiveButton = "joystick button 8", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = CONTROLLER_RIGHT_STICK_CLICK,    PositiveButton = "joystick button 9", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = MappingAxisType.KeyOrMouseButton, Axis = 1 },
        };

        public static readonly IEnumerable<string> MRTKDefaultInputMappingNames = new []
        {
            CONTROLLER_LEFT_STICK_HORIZONTAL,
            CONTROLLER_LEFT_STICK_VERTICAL,
            XBOX_SHARED_TRIGGER,
            CONTROLLER_RIGHT_STICK_HORIZONTAL,
            CONTROLLER_RIGHT_STICK_VERTICAL,
            XBOX_DPAD_HORIZONTAL,
            XBOX_DPAD_VERTICAL,
            CONTROLLER_LEFT_TRIGGER,
            CONTROLLER_RIGHT_TRIGGER,
            XBOX_A,
            XBOX_B,
            XBOX_X,
            XBOX_Y,
            CONTROLLER_LEFT_BUMPER_OR_GRIP,
            CONTROLLER_RIGHT_BUMPER_OR_GRIP,
            CONTROLLER_LEFT_MENU,
            CONTROLLER_RIGHT_MENU,
            CONTROLLER_LEFT_STICK_CLICK,
            CONTROLLER_RIGHT_STICK_CLICK
        };

        /// <summary>
        /// As axes in newInputAxes are removed or renamed, move them here for proper clean-up in user projects.
        /// </summary>
        private static readonly InputManagerAxis[] ObsoleteMRTKInputAxes = { };

        #endregion Mixed Reality Default Mappings configuration

        #region Mappings Functions

        /// <summary>
        /// Simple static function to apply Unity InputManager Axis configuration
        /// </summary>
        /// <remarks>
        /// This only exists as the Unity input manager CANNOT map Axis to an id, it has to be through a mapping
        /// </remarks>
        /// <param name="useToolkitAxes">Apply the Mixed Reality Toolkit defaults</param>
        /// <param name="removeMappings">If true, removes the selected mappings, either the defaults or the axisMappings array</param>
        /// <param name="axisMappings">Optional array of Axis Mappings, to configure your own custom set</param>
        public static void ApplyMappings(InputManagerAxis[] axisMappings = null)
        {
            AssureInputManagerReference();

            if (axisMappings == null)
            {
                foreach (InputManagerAxis axis in MRTKDefaultInputAxis)
                {
                    if (!DoesAxisNameExist(axis.Name))
                    {
                        AddAxis(axis);
                    }
                }
            }
            else
            {
                foreach (InputManagerAxis axis in axisMappings)
                {
                    if (!DoesAxisNameExist(axis.Name))
                    {
                        AddAxis(axis);
                    }
                }
            }

            inputManagerAsset.ApplyModifiedProperties();
        }

        /// <summary>
        /// Simple static function to apply Unity InputManager Axis configuration
        /// </summary>
        /// <remarks>
        /// This only exists as the Unity input manager CANNOT map Axis to an id, it has to be through a mapping
        /// </remarks>
        /// <param name="useToolkitAxes">Apply the Mixed Reality Toolkit defaults</param>
        /// <param name="removeMappings">If true, removes the selected mappings, either the defaults or the axisMappings array</param>
        /// <param name="axisMappings">Optional array of Axis Mappings, to configure your own custom set</param>
        public static void RemoveMappings(InputManagerAxis[] axisMappings = null)
        {
            AssureInputManagerReference();

            if (axisMappings == null)
            {
                foreach (InputManagerAxis axis in MRTKDefaultInputAxis)
                {
                    if (DoesAxisNameExist(axis.Name))
                    {
                        RemoveAxis(axis.Name);
                    }
                }

                foreach (InputManagerAxis axis in ObsoleteMRTKInputAxes)
                {
                    if (DoesAxisNameExist(axis.Name))
                    {
                        RemoveAxis(axis.Name);
                    }
                }
            }
            else
            {
                foreach (InputManagerAxis axis in axisMappings)
                {
                    if (DoesAxisNameExist(axis.Name))
                    {
                        RemoveAxis(axis.Name);
                    }
                }
            }

            inputManagerAsset.ApplyModifiedProperties();
        }

        private static void AddAxis(InputManagerAxis axis)
        {
            AssureInputManagerReference();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            // Creates a new axis by incrementing the size of the m_Axes array.
            axesProperty.arraySize++;

            // Get the new axis be querying for the last array element.
            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            // Iterate through all the properties of the new axis.
            while (axisProperty.Next(true))
            {
                switch (axisProperty.name)
                {
                    case "m_Name":
                        axisProperty.stringValue = axis.Name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.DescriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.DescriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.NegativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.PositiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.AltNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.AltPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.Gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.Dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.Sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.Snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.Invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.Type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.Axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.JoyNum;
                        break;
                }
            }
        }

        private static void RemoveAxis(string axis)
        {
            AssureInputManagerReference();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            RefreshLocalAxesList();

            // This loop accounts for multiple axes with the same name.
            while (AxisNames.Contains(axis))
            {
                int index = AxisNames.IndexOf(axis);
                axesProperty.DeleteArrayElementAtIndex(index);
                AxisNames.RemoveAt(index);
            }
        }

        /// <summary>
        /// Checks our local cache of axis names to see if an axis exists. This cache is refreshed if it's empty or if InputManager.asset has been changed.
        /// </summary>
        public static bool DoesAxisNameExist(string axisName)
        {
            AssureInputManagerReference();

            if (AxisNames.Count == 0 || inputManagerAsset.UpdateIfRequiredOrScript())
            {
                RefreshLocalAxesList();
            }

            return AxisNames.Contains(axisName);
        }

        /// <summary>
        /// Clears our local cache, then refills it by iterating through the m_Axes arrays and storing the display names.
        /// </summary>
        private static void RefreshLocalAxesList()
        {
            AssureInputManagerReference();

            AxisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                AxisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName);
            }
        }

        private static void AssureInputManagerReference()
        {
            if (inputManagerAsset == null)
            {
                // Grabs the actual asset file into a SerializedObject, so we can iterate through it and edit it.
                inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(UnityEngine.Object)));
            }
        }

        #endregion
    }
}