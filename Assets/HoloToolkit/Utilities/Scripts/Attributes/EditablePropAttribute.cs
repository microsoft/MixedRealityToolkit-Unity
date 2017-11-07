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
                    boolValue = EditorGUILayout.Toggle(SplitCamelCase(prop.Name), boolValue);
                    prop.SetValue(target, boolValue, null);
                    break;

                default:
                    throw new NotImplementedException("No drawer for type " + prop.PropertyType.Name);
            }
        }
#endif

    }
}