// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(InteractionDetector))]
    /// <summary>
    /// Custom editor for the basic interaction mode detector implementation.
    /// </summary>
    public class InteractionDetectorEditor : UnityEditor.Editor
    {
        private SerializedProperty interactor;

        private SerializedProperty controllers;

        private SerializedProperty detectHover;

        private SerializedProperty modeOnHover;

        private SerializedProperty detectSelect;

        private SerializedProperty modeOnSelect;

        public void OnEnable()
        {
            interactor = serializedObject.FindProperty("interactor");
            controllers = serializedObject.FindProperty("controllers");
            detectHover = serializedObject.FindProperty("detectHover");
            modeOnHover = serializedObject.FindProperty("modeOnHover");
            detectSelect = serializedObject.FindProperty("detectSelect");
            modeOnSelect = serializedObject.FindProperty("modeOnSelect");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(interactor);
            EditorGUILayout.PropertyField(controllers);

            EditorGUILayout.PropertyField(detectHover);

            if (detectHover.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(modeOnHover);
                }
            }

            EditorGUILayout.PropertyField(detectSelect);

            if (detectSelect.boolValue)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(modeOnSelect);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}