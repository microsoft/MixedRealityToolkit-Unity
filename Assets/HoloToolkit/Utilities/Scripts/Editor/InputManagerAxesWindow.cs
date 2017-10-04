// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class InputManagerAxesWindow : AutoConfigureWindow<InputManagerAxesWindow.AxisNames>
    {
        private static List<string> axisNames = new List<string>();

        public enum AxisNames
        {
            CONTROLLER_LEFT_STICK_HORIZONTAL,
            CONTROLLER_LEFT_STICK_VERTICAL,
            CONTROLLER_LEFT_STICK_CLICK,
            CONTROLLER_RIGHT_STICK_HORIZONTAL,
            CONTROLLER_RIGHT_STICK_VERTICAL,
            CONTROLLER_RIGHT_STICK_CLICK,
            CONTROLLER_LEFT_TRIGGER,
            CONTROLLER_RIGHT_TRIGGER,
            XBOX_SHARED_TRIGGER,
            XBOX_LEFT_BUMPER,
            XBOX_RIGHT_BUMPER,
            XBOX_A,
            XBOX_B,
            XBOX_X,
            XBOX_Y,
            XBOX_VIEW,
            XBOX_MENU,
            XBOX_DPAD_HORIZONTAL,
            XBOX_DPAD_VERTICAL,
            MAX // Always leave at the end when adding new axes!
        }

        public enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        };

        public class InputAxis
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

        protected override void ApplySettings()
        {
            foreach(AxisNames axisName in Values.Keys)
            {
                if (Values[axisName] && !DoesAxisNameExist(axisName.ToString()))
                {
                    AddAxis(new InputAxis() { name = axisName.ToString(), snap = true });
                }
            }

            Close();
        }

        private void LoadSetting(AxisNames setting)
        {
            Values[setting] = DoesAxisNameExist(setting.ToString());
        }

        protected override void LoadSettings()
        {
            for (int i = 0; i < (int)AxisNames.MAX; i++)
            {
                LoadSetting((AxisNames)i);
            }
        }

        protected override void LoadStrings()
        {
            Names[AxisNames.CONTROLLER_LEFT_STICK_HORIZONTAL] = "CONTROLLER_LEFT_STICK_HORIZONTAL";
            Descriptions[AxisNames.CONTROLLER_LEFT_STICK_HORIZONTAL] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_LEFT_STICK_VERTICAL] = "CONTROLLER_LEFT_STICK_VERTICAL";
            Descriptions[AxisNames.CONTROLLER_LEFT_STICK_VERTICAL] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_LEFT_STICK_CLICK] = "CONTROLLER_LEFT_STICK_CLICK";
            Descriptions[AxisNames.CONTROLLER_LEFT_STICK_CLICK] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_RIGHT_STICK_HORIZONTAL] = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
            Descriptions[AxisNames.CONTROLLER_RIGHT_STICK_HORIZONTAL] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_RIGHT_STICK_VERTICAL] = "CONTROLLER_RIGHT_STICK_VERTICAL";
            Descriptions[AxisNames.CONTROLLER_RIGHT_STICK_VERTICAL] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_RIGHT_STICK_CLICK] = "CONTROLLER_RIGHT_STICK_CLICK";
            Descriptions[AxisNames.CONTROLLER_RIGHT_STICK_CLICK] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_LEFT_TRIGGER] = "CONTROLLER_LEFT_TRIGGER";
            Descriptions[AxisNames.CONTROLLER_LEFT_TRIGGER] =
                "Recommended\n\n";

            Names[AxisNames.CONTROLLER_RIGHT_TRIGGER] = "CONTROLLER_RIGHT_TRIGGER";
            Descriptions[AxisNames.CONTROLLER_RIGHT_TRIGGER] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_SHARED_TRIGGER] = "XBOX_SHARED_TRIGGER";
            Descriptions[AxisNames.XBOX_SHARED_TRIGGER] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_LEFT_BUMPER] = "XBOX_LEFT_BUMPER";
            Descriptions[AxisNames.XBOX_LEFT_BUMPER] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_RIGHT_BUMPER] = "XBOX_RIGHT_BUMPER";
            Descriptions[AxisNames.XBOX_RIGHT_BUMPER] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_A] = "XBOX_A";
            Descriptions[AxisNames.XBOX_A] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_B] = "XBOX_B";
            Descriptions[AxisNames.XBOX_B] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_X] = "XBOX_X";
            Descriptions[AxisNames.XBOX_X] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_Y] = "XBOX_Y";
            Descriptions[AxisNames.XBOX_Y] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_VIEW] = "XBOX_VIEW";
            Descriptions[AxisNames.XBOX_VIEW] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_MENU] = "XBOX_MENU";
            Descriptions[AxisNames.XBOX_MENU] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_DPAD_HORIZONTAL] = "XBOX_DPAD_HORIZONTAL";
            Descriptions[AxisNames.XBOX_DPAD_HORIZONTAL] =
                "Recommended\n\n";

            Names[AxisNames.XBOX_DPAD_VERTICAL] = "XBOX_DPAD_VERTICAL";
            Descriptions[AxisNames.XBOX_DPAD_VERTICAL] =
                "Recommended\n\n";
        }

        protected override void OnGuiChanged()
        {
        }

        private static void AddAxis(InputAxis axis)
        {
            SerializedObject inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(Object)));
            SerializedProperty axesProperty = inputManagerAsset.FindProperty("m_Axes");

            axesProperty.arraySize++;
            inputManagerAsset.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

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

            inputManagerAsset.ApplyModifiedProperties();

            axisNames.Add(axis.name.ToLower());
        }

        private static bool DoesAxisNameExist(string axisName)
        {
            if (axisNames.Count == 0)
            {
                SerializedObject inputManagerAsset = new SerializedObject(AssetDatabase.LoadAssetAtPath("ProjectSettings/InputManager.asset", typeof(Object)));
                SerializedProperty axes = inputManagerAsset.FindProperty("m_Axes");

                axes.Next(true);
                axes.Next(true);
                while (axes.Next(false))
                {
                    axisNames.Add(axes.displayName.ToLower());
                }
            }

            return axisNames.Contains(axisName.ToLower());
        }
    }
}