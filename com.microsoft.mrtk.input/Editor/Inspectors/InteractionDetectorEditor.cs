// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    /// <summary>
    /// A custom editor for the <see cref="InteractionDetector"/> class.
    /// </summary>
    [CustomEditor(typeof(InteractionDetector))]
    public class InteractionDetectorEditor : UnityEditor.Editor
    {
        private SerializedProperty interactor;

        private SerializedProperty controllers;

        private SerializedProperty detectHover;

        private SerializedProperty modeOnHover;

        private SerializedProperty detectSelect;

        private SerializedProperty modeOnSelect;

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
        public void OnEnable()
        {
            interactor = serializedObject.FindProperty("interactor");
            controllers = serializedObject.FindProperty("controllers");
            detectHover = serializedObject.FindProperty("detectHover");
            modeOnHover = serializedObject.FindProperty("modeOnHover");
            detectSelect = serializedObject.FindProperty("detectSelect");
            modeOnSelect = serializedObject.FindProperty("modeOnSelect");
        }

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
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