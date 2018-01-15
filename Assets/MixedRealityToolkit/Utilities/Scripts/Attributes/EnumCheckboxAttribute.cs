// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Displays an enum value as a set of checkboxes
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumCheckboxAttribute : DrawOverrideAttribute
    {
        public string DefaultName { get; private set; }
        public int DefaultValue { get; private set; }
        public int ValueOnZero { get; private set; }
        public bool IgnoreNone { get; private set; }
        public bool IgnoreAll { get; private set; }
        public string CustomLabel { get; private set; }

        public EnumCheckboxAttribute(string customLabel = null)
        {
            DefaultName = null;
            DefaultValue = 0;
            ValueOnZero = 0;
            IgnoreNone = true;
            IgnoreAll = true;
            CustomLabel = customLabel;
        }

        public EnumCheckboxAttribute(bool ignoreNone, bool ignoreAll, string customLabel = null)
        {
            DefaultName = null;
            DefaultValue = 0;
            ValueOnZero = 0;
            IgnoreNone = ignoreNone;
            IgnoreAll = ignoreAll;
            CustomLabel = customLabel;
        }

        public EnumCheckboxAttribute(string defaultName, object defaultValue, object valueOnZero = null, bool ignoreNone = true, bool ignoreAll = true, string customLabel = null)
        {
            DefaultName = defaultName;
            DefaultValue = Convert.ToInt32(defaultValue);
            ValueOnZero = Convert.ToInt32(valueOnZero);
            IgnoreNone = ignoreNone;
            IgnoreAll = ignoreAll;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            int value = EnumCheckbox(
                (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(field.Name) : CustomLabel),
                field.GetValue(target),
                DefaultName,
                DefaultValue,
                ValueOnZero,
                IgnoreNone,
                IgnoreAll);

            field.SetValue(target, value);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            int value = EnumCheckbox(
                (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(prop.Name) : CustomLabel),
                prop.GetValue(target, null),
                DefaultName,
                DefaultValue,
                ValueOnZero,
                IgnoreNone,
                IgnoreAll);

            prop.SetValue(target, value, null);
        }

        private static int EnumCheckbox(string label, object enumObj, string defaultName, object defaultVal, object valOnZero, bool ignoreNone = true, bool ignoreAll = true)
        {
            if (!enumObj.GetType().IsEnum)
            {
                throw new ArgumentException("enumObj must be an enum.");
            }

            // Convert enum value to an int64 so we can treat it as a flag set
            int enumFlags = Convert.ToInt32(enumObj);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.miniLabel);

            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = 2;
            GUILayout.Box("", styleHR);

            System.Array enumVals = Enum.GetValues(enumObj.GetType());
            int lastvalue = Convert.ToInt32(enumVals.GetValue(enumVals.GetLength(0) - 1));

            foreach (object enumVal in enumVals)
            {
                int flagVal = Convert.ToInt32(enumVal);
                if (ignoreNone && flagVal == 0 && enumVal.ToString().ToLower() == "none")
                {
                    continue;
                }
                if (ignoreAll && flagVal == lastvalue && enumVal.ToString().ToLower() == "all")
                {
                    continue;
                }
                bool selected = (flagVal & enumFlags) != 0;
                selected = EditorGUILayout.Toggle(enumVal.ToString(), selected);
                // If it's selected add it to the enumObj, otherwise remove it
                if (selected)
                {
                    enumFlags |= flagVal;
                }
                else
                {
                    enumFlags &= ~flagVal;
                }
            }
            if (!string.IsNullOrEmpty(defaultName))
            {
                if (GUILayout.Button(defaultName, EditorStyles.miniButton))
                {
                    enumFlags = Convert.ToInt32(defaultVal);
                }
            }
            EditorGUILayout.EndVertical();

            if (enumFlags == 0)
            {
                enumFlags = Convert.ToInt32(valOnZero);
            }
            return enumFlags;
        }
#endif

    }
}