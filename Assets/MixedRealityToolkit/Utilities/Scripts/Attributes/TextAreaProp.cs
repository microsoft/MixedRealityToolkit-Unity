// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HoloToolkit.Unity
{
    // Displays a text property as a text area
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextAreaProp : DrawOverrideAttribute
    {
        public int FontSize { get; private set; }

        public TextAreaProp(int fontSize = -1)
        {
            FontSize = fontSize;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            string propValue = (string)prop.GetValue(target, null);
            EditorGUILayout.LabelField(SplitCamelCase(prop.Name), EditorStyles.miniBoldLabel);
            GUIStyle textAreaStyle = EditorStyles.textArea;
            if (FontSize > 0)
            {
                textAreaStyle.fontSize = FontSize;
            }
            propValue = EditorGUILayout.TextArea(propValue, textAreaStyle);
            prop.SetValue(target, propValue, null);
        }
#endif

    }
}