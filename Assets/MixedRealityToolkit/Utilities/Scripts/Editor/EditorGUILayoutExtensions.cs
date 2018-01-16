// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.EditorScript
{
    /// <summary>
    /// Extensions for the UnityEditor.EditorGUILayout class.
    /// </summary>
    public static class EditorGUILayoutExtensions
    {
        public static bool Button(string text, params GUILayoutOption[] options)
        {
            return Button(text, GUI.skin.button, options);
        }

        public static bool Button(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIExtensions.Indent);
            bool pressed = GUILayout.Button(text, style, options);
            EditorGUILayout.EndHorizontal();
            return pressed;
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            Label(text, EditorStyles.label, options);
        }

        public static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUIExtensions.Indent);
            GUILayout.Label(text, style, options);
            EditorGUILayout.EndHorizontal();
        }

        public static T ObjectField<T>(GUIContent guiContent, T value, bool allowSceneObjects = false, GUILayoutOption[] guiLayoutOptions = null)
        {
            object objValue = value;

            if (objValue == null)
            {
                // We want to return null so we can display our blank field.
                return (T)objValue;
            }

            Type valueType = objValue.GetType();
            if (valueType == typeof(Material))
            {
                objValue = EditorGUILayout.ObjectField(guiContent, (Material)objValue, typeof(Material), allowSceneObjects, guiLayoutOptions);
            }
            else if (valueType == typeof(SceneAsset))
            {
                objValue = EditorGUILayout.ObjectField(guiContent, (SceneAsset)objValue, typeof(SceneAsset), allowSceneObjects, guiLayoutOptions);
            }
            else if (objValue is UnityEngine.Object)
            {
                objValue = EditorGUILayout.ObjectField(guiContent, (UnityEngine.Object)objValue, valueType, allowSceneObjects, guiLayoutOptions);
            }
            else
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Unimplemented value type: {0}.",
                        valueType),
                    "value");
            }

            return (T)objValue;
        }
    }
}
