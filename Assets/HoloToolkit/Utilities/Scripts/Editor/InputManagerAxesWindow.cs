// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Renders the UI and handles update logic for Mixed Reality Toolkit/Configure/Add Input Manager Axes.
    /// </summary>
    public class InputManagerAxesWindow : AutoConfigureWindow<string>
    {
        /// <summary>
        /// This is used to keep a local list of axis names, so we don't have to keep iterating through each SerializedProperty.
        /// </summary>
        private static List<string> axisNames = new List<string>();

        /// <summary>
        /// This is used to keep a single reference to InputManager.asset, refreshed when necessary.
        /// </summary>
        private static SerializedObject inputManagerAsset;

        /// <summary>
        /// Define new axes here adding a new InputManagerAxis to the array.
        /// </summary>
        private readonly InputManagerAxis[] newInputAxes =
        {
            new InputManagerAxis() { Name = "CONTROLLER_LEFT_STICK_HORIZONTAL", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 1 },
            new InputManagerAxis() { Name = "CONTROLLER_LEFT_STICK_VERTICAL", Dead = 0.19f, Sensitivity = 1, Invert = true, Type = AxisType.JoystickAxis, Axis = 2 },
            new InputManagerAxis() { Name = "CONTROLLER_LEFT_STICK_CLICK", PositiveButton = "joystick button 8", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "CONTROLLER_RIGHT_STICK_HORIZONTAL", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 4 },
            new InputManagerAxis() { Name = "CONTROLLER_RIGHT_STICK_VERTICAL", Dead = 0.19f, Sensitivity = 1, Invert = true, Type = AxisType.JoystickAxis, Axis = 5 },
            new InputManagerAxis() { Name = "CONTROLLER_RIGHT_STICK_CLICK", PositiveButton = "joystick button 9", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "CONTROLLER_LEFT_MENU", PositiveButton = "joystick button 6", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "CONTROLLER_RIGHT_MENU", PositiveButton = "joystick button 7", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "CONTROLLER_LEFT_TRIGGER", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 9 },
            new InputManagerAxis() { Name = "CONTROLLER_RIGHT_TRIGGER", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 10 },
            new InputManagerAxis() { Name = "XBOX_SHARED_TRIGGER", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 3 },
            new InputManagerAxis() { Name = "XBOX_LEFT_BUMPER", PositiveButton = "joystick button 4", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_RIGHT_BUMPER", PositiveButton = "joystick button 5", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_A", PositiveButton = "joystick button 0", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_B", PositiveButton = "joystick button 1", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_X", PositiveButton = "joystick button 2", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_Y", PositiveButton = "joystick button 3", Gravity = 1000, Dead = 0.001f, Sensitivity = 1000, Type = AxisType.KeyOrMouseButton, Axis = 1 },
            new InputManagerAxis() { Name = "XBOX_DPAD_HORIZONTAL", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 6 },
            new InputManagerAxis() { Name = "XBOX_DPAD_VERTICAL", Dead = 0.19f, Sensitivity = 1, Type = AxisType.JoystickAxis, Axis = 7 }
        };

        /// <summary>
        /// Used to map AxisType from a useful name to the int value the InputManager wants.
        /// </summary>
        private enum AxisType
        {
            KeyOrMouseButton,
            MouseMovement,
            JoystickAxis
        };

        /// <summary>
        /// Used to define an entire InputManagerAxis, with each variable defined by the same term the Inspector shows.
        /// </summary>
        private class InputManagerAxis
        {
            public string Name;
            public string DescriptiveName;
            public string DescriptiveNegativeName;
            public string NegativeButton;
            public string PositiveButton;
            public string AltNegativeButton;
            public string AltPositiveButton;
            public float Gravity;
            public float Dead;
            public float Sensitivity;
            public bool Snap = false;
            public bool Invert = false;
            public AxisType Type;
            public int Axis;
            public int JoyNum;
        }

        protected override void Awake()
        {
            base.Awake();

            // Grabs the actual asset file into a SerializedObject, so we can iterate through it and edit it.
            inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(Object)));
            RefreshLocalAxesList();
        }

        protected override void ApplySettings()
        {
            foreach (InputManagerAxis axis in newInputAxes)
            {
                if (Values[axis.Name] && !DoesAxisNameExist(axis.Name))
                {
                    AddAxis(axis);
                }
            }

            inputManagerAsset.ApplyModifiedProperties();

            Close();
        }

        private void LoadSetting(InputManagerAxis axis)
        {
            Values[axis.Name] = DoesAxisNameExist(axis.Name);
        }

        protected override void LoadSettings()
        {
            for (int i = 0; i < newInputAxes.Length; i++)
            {
                LoadSetting(newInputAxes[i]);
            }
        }

        protected override void LoadStrings()
        {
            Names["CONTROLLER_LEFT_STICK_HORIZONTAL"] = "CONTROLLER_LEFT_STICK_HORIZONTAL";
            Descriptions["CONTROLLER_LEFT_STICK_HORIZONTAL"] =
                "Recommended\n\nUse this to get the left thumbstick's X axis, from -1 to 1 going left to right. This is used for teleporting.";

            Names["CONTROLLER_LEFT_STICK_VERTICAL"] = "CONTROLLER_LEFT_STICK_VERTICAL";
            Descriptions["CONTROLLER_LEFT_STICK_VERTICAL"] =
                "Recommended\n\nUse this to get the left thumbstick's Y axis, from -1 to 1 going bottom to top. This is used for teleporting.";

            Names["CONTROLLER_LEFT_STICK_CLICK"] = "CONTROLLER_LEFT_STICK_CLICK";
            Descriptions["CONTROLLER_LEFT_STICK_CLICK"] =
                "Recommended\n\nUse this to get the left thumbstick's clicked state.";

            Names["CONTROLLER_RIGHT_STICK_HORIZONTAL"] = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
            Descriptions["CONTROLLER_RIGHT_STICK_HORIZONTAL"] =
                "Recommended\n\nUse this to get the right thumbstick's X axis, from -1 to 1 going left to right. This is used for teleporting.";

            Names["CONTROLLER_RIGHT_STICK_VERTICAL"] = "CONTROLLER_RIGHT_STICK_VERTICAL";
            Descriptions["CONTROLLER_RIGHT_STICK_VERTICAL"] =
                "Recommended\n\nUse this to get the right thumbstick's Y axis, from -1 to 1 going bottom to top. This is used for teleporting.";

            Names["CONTROLLER_RIGHT_STICK_CLICK"] = "CONTROLLER_RIGHT_STICK_CLICK";
            Descriptions["CONTROLLER_RIGHT_STICK_CLICK"] =
                "Recommended\n\nUse this to get the right thumbstick's clicked state.";

            Names["CONTROLLER_LEFT_TRIGGER"] = "CONTROLLER_LEFT_TRIGGER";
            Descriptions["CONTROLLER_LEFT_TRIGGER"] =
                "Recommended\n\nUse this to get the pressed state of the left trigger, from 0 to 1 as it's pressed.";

            Names["CONTROLLER_RIGHT_TRIGGER"] = "CONTROLLER_RIGHT_TRIGGER";
            Descriptions["CONTROLLER_RIGHT_TRIGGER"] =
                "Recommended\n\nUse this to get the pressed state of the right trigger, from 0 to 1 as it's pressed.";

            Names["CONTROLLER_LEFT_MENU"] = "CONTROLLER_LEFT_MENU";
            Descriptions["CONTROLLER_LEFT_MENU"] =
                "Recommended\n\nUse this to get the pressed state of the view button on an Xbox controller or the menu button on the left motion controller, 0 or 1 if it's pressed.";

            Names["CONTROLLER_RIGHT_MENU"] = "CONTROLLER_RIGHT_MENU";
            Descriptions["CONTROLLER_RIGHT_MENU"] =
                "Recommended\n\nUse this to get the pressed state of the menu button on an Xbox controller or the menu button on the right motion controller, 0 or 1 if it's pressed.";

            Names["XBOX_SHARED_TRIGGER"] = "XBOX_SHARED_TRIGGER";
            Descriptions["XBOX_SHARED_TRIGGER"] =
                "Recommended\n\nUse this to get the pressed state of the both triggers, the average of two numbers: from 0 to -1 as the left trigger is pressed and from 0 to 1 as the right trigger is pressed.";

            Names["XBOX_LEFT_BUMPER"] = "XBOX_LEFT_BUMPER";
            Descriptions["XBOX_LEFT_BUMPER"] =
                "Recommended\n\nUse this to get the pressed state of the left bumper, 0 or 1 if it's pressed.";

            Names["XBOX_RIGHT_BUMPER"] = "XBOX_RIGHT_BUMPER";
            Descriptions["XBOX_RIGHT_BUMPER"] =
                "Recommended\n\nUse this to get the pressed state of the right bumper, 0 or 1 if it's pressed.";

            Names["XBOX_A"] = "XBOX_A";
            Descriptions["XBOX_A"] =
                "Recommended\n\nUse this to get the pressed state of the A button, 0 or 1 if it's pressed.";

            Names["XBOX_B"] = "XBOX_B";
            Descriptions["XBOX_B"] =
                "Recommended\n\nUse this to get the pressed state of the B button, 0 or 1 if it's pressed.";

            Names["XBOX_X"] = "XBOX_X";
            Descriptions["XBOX_X"] =
                "Recommended\n\nUse this to get the pressed state of the X button, 0 or 1 if it's pressed.";

            Names["XBOX_Y"] = "XBOX_Y";
            Descriptions["XBOX_Y"] =
                "Recommended\n\nUse this to get the pressed state of the Y button, 0 or 1 if it's pressed.";

            Names["XBOX_DPAD_HORIZONTAL"] = "XBOX_DPAD_HORIZONTAL";
            Descriptions["XBOX_DPAD_HORIZONTAL"] =
                "Recommended\n\nUse this to get the pressed state of the X axis of the D-pad: -1 for the left side pressed, 1 for the right side pressed, 0 for neither pressed.";

            Names["XBOX_DPAD_VERTICAL"] = "XBOX_DPAD_VERTICAL";
            Descriptions["XBOX_DPAD_VERTICAL"] =
                "Recommended\n\nUse this to get the pressed state of the Y axis of the D-pad: -1 for the bottom side pressed, 1 for the top side pressed, 0 for neither pressed.";
        }

        protected override void OnGuiChanged()
        {
        }

        private static void AddAxis(InputManagerAxis axis)
        {
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

        /// <summary>
        /// Checks our local cache of axis names to see if an axis exists. This cache is refreshed if it's empty or if InputManager.asset has been changed.
        /// </summary>
        private static bool DoesAxisNameExist(string axisName)
        {
            if (axisNames.Count == 0 || inputManagerAsset.UpdateIfRequiredOrScript())
            {
                RefreshLocalAxesList();
            }

            return axisNames.Contains(axisName.ToLower());
        }

        /// <summary>
        /// Clears our local cache, then refills it by iterating through the m_Axes arrays and storing the display names.
        /// </summary>
        private static void RefreshLocalAxesList()
        {
            axisNames.Clear();

            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                axisNames.Add(axesProperty.GetArrayElementAtIndex(i).displayName.ToLower());
            }
        }
    }
}