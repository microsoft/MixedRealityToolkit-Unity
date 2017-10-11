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
        private InputManagerAxis[] newInputAxes =
        {
            new InputManagerAxis() { name = "CONTROLLER_LEFT_STICK_HORIZONTAL", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 1 },
            new InputManagerAxis() { name = "CONTROLLER_LEFT_STICK_VERTICAL", dead = 0.19f, sensitivity = 1, invert = true, type = AxisType.JoystickAxis, axis = 2 },
            new InputManagerAxis() { name = "CONTROLLER_LEFT_STICK_CLICK", positiveButton = "joystick button 8", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "CONTROLLER_RIGHT_STICK_HORIZONTAL", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 4 },
            new InputManagerAxis() { name = "CONTROLLER_RIGHT_STICK_VERTICAL", dead = 0.19f, sensitivity = 1, invert = true, type = AxisType.JoystickAxis, axis = 5 },
            new InputManagerAxis() { name = "CONTROLLER_RIGHT_STICK_CLICK", positiveButton = "joystick button 9", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "CONTROLLER_LEFT_TRIGGER", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 9 },
            new InputManagerAxis() { name = "CONTROLLER_RIGHT_TRIGGER", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 10 },
            new InputManagerAxis() { name = "XBOX_SHARED_TRIGGER", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 3 },
            new InputManagerAxis() { name = "XBOX_LEFT_BUMPER", positiveButton = "joystick button 4", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_RIGHT_BUMPER", positiveButton = "joystick button 5", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_A", positiveButton = "joystick button 0", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_B", positiveButton = "joystick button 1", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_X", positiveButton = "joystick button 2", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_Y", positiveButton = "joystick button 3", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_VIEW", positiveButton = "joystick button 6", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_MENU", positiveButton = "joystick button 7", gravity = 1000, dead = 0.001f, sensitivity = 1000, type = AxisType.KeyOrMouseButton, axis = 1 },
            new InputManagerAxis() { name = "XBOX_DPAD_HORIZONTAL", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 6 },
            new InputManagerAxis() { name = "XBOX_DPAD_VERTICAL", dead = 0.19f, sensitivity = 1, type = AxisType.JoystickAxis, axis = 7 }
        };

        /// <summary>
        /// Used to map AxisType from a useful name to the int value the InputManager wants.
        /// </summary>
        public enum AxisType
        {
            KeyOrMouseButton,
            MouseMovement,
            JoystickAxis
        };

        /// <summary>
        /// Used to define an entire InputManagerAxis, with each variable defined by the same term the Inspector shows.
        /// </summary>
        public class InputManagerAxis
        {
            public string name;
            public string descriptiveName;
            public string descriptiveNegativeName;
            public string negativeButton;
            public string positiveButton;
            public string altNegativeButton;
            public string altPositiveButton;
            public float gravity;
            public float dead;
            public float sensitivity;
            public bool snap = false;
            public bool invert = false;
            public AxisType type;
            public int axis;
            public int joyNum;
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
                if (Values[axis.name] && !DoesAxisNameExist(axis.name))
                {
                    AddAxis(axis);
                }
            }

            inputManagerAsset.ApplyModifiedProperties();

            Close();
        }

        private void LoadSetting(InputManagerAxis axis)
        {
            Values[axis.name] = DoesAxisNameExist(axis.name);
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

            Names["XBOX_VIEW"] = "XBOX_VIEW";
            Descriptions["XBOX_VIEW"] =
                "Recommended\n\nUse this to get the pressed state of the view button or the menu button on the left motion controller, 0 or 1 if it's pressed.";

            Names["XBOX_MENU"] = "XBOX_MENU";
            Descriptions["XBOX_MENU"] =
                "Recommended\n\nUse this to get the pressed state of the menu button or the menu button on the right motion controller, 0 or 1 if it's pressed.";

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
                        axisProperty.stringValue = axis.name;
                        break;
                    case "descriptiveName":
                        axisProperty.stringValue = axis.descriptiveName;
                        break;
                    case "descriptiveNegativeName":
                        axisProperty.stringValue = axis.descriptiveNegativeName;
                        break;
                    case "negativeButton":
                        axisProperty.stringValue = axis.negativeButton;
                        break;
                    case "positiveButton":
                        axisProperty.stringValue = axis.positiveButton;
                        break;
                    case "altNegativeButton":
                        axisProperty.stringValue = axis.altNegativeButton;
                        break;
                    case "altPositiveButton":
                        axisProperty.stringValue = axis.altPositiveButton;
                        break;
                    case "gravity":
                        axisProperty.floatValue = axis.gravity;
                        break;
                    case "dead":
                        axisProperty.floatValue = axis.dead;
                        break;
                    case "sensitivity":
                        axisProperty.floatValue = axis.sensitivity;
                        break;
                    case "snap":
                        axisProperty.boolValue = axis.snap;
                        break;
                    case "invert":
                        axisProperty.boolValue = axis.invert;
                        break;
                    case "type":
                        axisProperty.intValue = (int)axis.type;
                        break;
                    case "axis":
                        axisProperty.intValue = axis.axis - 1;
                        break;
                    case "joyNum":
                        axisProperty.intValue = axis.joyNum;
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