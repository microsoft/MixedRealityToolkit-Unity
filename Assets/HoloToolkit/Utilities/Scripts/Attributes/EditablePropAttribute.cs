// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Displays a prop as editable in the inspector
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EditablePropAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public EditablePropAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            switch (prop.PropertyType.Name)
            {
                case "Boolean":
                    bool boolValue = (bool)prop.GetValue(target, null);
                    boolValue = EditorGUILayout.Toggle(!string.IsNullOrEmpty (CustomLabel) ? CustomLabel : SplitCamelCase(prop.Name), boolValue);
                    prop.SetValue(target, boolValue, null);
                    break;

                case "Int32":
                    int intValue = (int)prop.GetValue(target, null);
                    intValue = EditorGUILayout.IntField(!string.IsNullOrEmpty(CustomLabel) ? CustomLabel : SplitCamelCase(prop.Name), intValue);
                    prop.SetValue(target, intValue, null);
                    break;

                case "Single":
                    float singleValue = (float)prop.GetValue(target, null);
                    singleValue = EditorGUILayout.FloatField(!string.IsNullOrEmpty(CustomLabel) ? CustomLabel : SplitCamelCase(prop.Name), singleValue);
                    prop.SetValue(target, singleValue, null);
                    break;

                default:
                    throw new NotImplementedException("No drawer for type " + prop.PropertyType.Name);
            }
        }
#endif

    }
}