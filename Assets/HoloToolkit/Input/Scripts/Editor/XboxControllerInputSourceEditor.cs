// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(XboxControllerInputSource))]
    public class XboxControllerInputSourceEditor : Editor
    {
        private SerializedProperty horizontalAxisProperty;
        private SerializedProperty verticalAxisProperty;
        private SerializedProperty submitButtonProperty;
        private SerializedProperty cancelButtonProperty;
        private SerializedProperty useCustomMappingProperty;
        private SerializedProperty customMappingsProperty;

        private void OnEnable()
        {
            horizontalAxisProperty = serializedObject.FindProperty("horizontalAxis");
            verticalAxisProperty = serializedObject.FindProperty("verticalAxis");
            submitButtonProperty = serializedObject.FindProperty("submitButton");
            cancelButtonProperty = serializedObject.FindProperty("cancelButton");
            useCustomMappingProperty = serializedObject.FindProperty("useCustomMapping");
            customMappingsProperty = serializedObject.FindProperty("customMappings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(horizontalAxisProperty);
            EditorGUILayout.PropertyField(verticalAxisProperty);
            EditorGUILayout.PropertyField(submitButtonProperty);
            EditorGUILayout.PropertyField(cancelButtonProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(useCustomMappingProperty);

            if (EditorGUI.EndChangeCheck())
            {
                customMappingsProperty.arraySize = useCustomMappingProperty.boolValue
                    ? Enum.GetNames(typeof(XboxControllerMappingTypes)).Length
                    : 0;
            }

            if (useCustomMappingProperty.boolValue)
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
