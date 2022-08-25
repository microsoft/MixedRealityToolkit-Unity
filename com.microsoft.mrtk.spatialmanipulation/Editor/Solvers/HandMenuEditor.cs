// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    [CustomEditor(typeof(HandMenu))]
    [CanEditMultipleObjects]
    public class HandMenuEditor : UnityEditor.Editor
    {
        private SerializedProperty leftHandPosition;

        private SerializedProperty leftHandRotation;

        private SerializedProperty rightHandPosition;

        private SerializedProperty rightHandRotation;

        private static bool expanded = false;

        private string[] drawnProperties = new string[]
        {
            nameof(leftHandPosition),
            nameof(leftHandRotation),
            nameof(rightHandPosition),
            nameof(rightHandRotation)
        };

        protected virtual void OnEnable()
        {
            leftHandPosition = serializedObject.FindProperty(nameof(leftHandPosition));
            leftHandRotation = serializedObject.FindProperty(nameof(leftHandRotation));
            rightHandPosition = serializedObject.FindProperty(nameof(rightHandPosition));
            rightHandRotation = serializedObject.FindProperty(nameof(rightHandRotation));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, drawnProperties);
            EditorGUILayout.Space();

            if (expanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, "Fallback Bindings"))
            {
                EditorGUILayout.PropertyField(leftHandPosition);
                EditorGUILayout.PropertyField(leftHandRotation);
                EditorGUILayout.PropertyField(rightHandPosition);
                EditorGUILayout.PropertyField(rightHandRotation);
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}