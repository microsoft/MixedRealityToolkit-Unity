// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.GamePad;
using MixedRealityToolkit.InputModule.InputSources;
using System;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.EditorScript
{
    [CustomEditor(typeof(XboxControllerInputSource))]
    public class XboxControllerInputSourceEditor : UnityEditor.Editor
    {
        private SerializedProperty horizontalAxisProperty;
        private SerializedProperty verticalAxisProperty;
        private SerializedProperty submitButtonProperty;
        private SerializedProperty cancelButtonProperty;
        private SerializedProperty customMappingsProperty;

        private static bool useCustomMapping;

        private void OnEnable()
        {
            horizontalAxisProperty = serializedObject.FindProperty("horizontalAxis");
            verticalAxisProperty = serializedObject.FindProperty("verticalAxis");
            submitButtonProperty = serializedObject.FindProperty("submitButton");
            cancelButtonProperty = serializedObject.FindProperty("cancelButton");
            customMappingsProperty = serializedObject.FindProperty("mapping");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Event System Overrides", new GUIStyle("Label") { fontStyle = FontStyle.Bold });

            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(horizontalAxisProperty);
            EditorGUILayout.PropertyField(verticalAxisProperty);
            EditorGUILayout.PropertyField(submitButtonProperty);
            EditorGUILayout.PropertyField(cancelButtonProperty);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            useCustomMapping = EditorGUILayout.Toggle("Use Custom Mapping", useCustomMapping);

            if (EditorGUI.EndChangeCheck())
            {
                customMappingsProperty.arraySize = Enum.GetNames(typeof(XboxControllerMappingTypes)).Length;

                for (int i = 0; i < customMappingsProperty.arraySize; i++)
                {
                    customMappingsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = i;
                    customMappingsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Value").stringValue = string.Empty;
                }
            }

            if (useCustomMapping)
            {
                ShowList(customMappingsProperty);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void ShowList(SerializedProperty list)
        {
            EditorGUILayout.Space();

            list.isExpanded = true;
            EditorGUI.indentLevel++;

            for (int i = 0; i < list.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list.GetArrayElementAtIndex(i).FindPropertyRelative("Type").enumValueIndex = i;
                EditorGUILayout.LabelField(((XboxControllerMappingTypes)i).ToString());
                list.GetArrayElementAtIndex(i).FindPropertyRelative("Value").stringValue =
                    EditorGUILayout.TextField(list.GetArrayElementAtIndex(i).FindPropertyRelative("Value").stringValue);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }
    }
}
