// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace MixedRealityToolkit.Utilities.Attributes
{
    // Displays an enum value as a dropdown mask
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumFlagsAttribute : DrawOverrideAttribute
    {
#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            int enumValue = Convert.ToInt32(field.GetValue(target));
            List<string> displayOptions = new List<string>();
            foreach (object value in Enum.GetValues(field.FieldType))
            {
                displayOptions.Add(value.ToString());
            }
            enumValue = EditorGUILayout.MaskField(SplitCamelCase(field.Name), enumValue, displayOptions.ToArray());
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            int enumValue = Convert.ToInt32(prop.GetValue(target, null));
            List<string> displayOptions = new List<string>();
            foreach (object value in Enum.GetValues(prop.PropertyType))
            {
                displayOptions.Add(value.ToString());
            }
            enumValue = EditorGUILayout.MaskField(SplitCamelCase(prop.Name), enumValue, displayOptions.ToArray());
        }
#endif
    }
}