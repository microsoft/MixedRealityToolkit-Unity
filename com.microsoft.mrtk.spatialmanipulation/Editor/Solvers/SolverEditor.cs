// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    [CustomEditor(typeof(Solver))]
    [CanEditMultipleObjects]
    public class SolverEditor : UnityEditor.Editor
    {
        private SerializedProperty updateLinkedTransformProperty;
        private SerializedProperty moveLerpTimeProperty;
        private SerializedProperty rotateLerpTimeProperty;
        private SerializedProperty scaleLerpTimeProperty;
        private SerializedProperty maintainScaleProperty;
        private SerializedProperty smoothingProperty;
        private SerializedProperty lifetimeProperty;

        protected virtual void OnEnable()
        {
            updateLinkedTransformProperty = serializedObject.FindProperty("updateLinkedTransform");
            moveLerpTimeProperty = serializedObject.FindProperty("moveLerpTime");
            rotateLerpTimeProperty = serializedObject.FindProperty("rotateLerpTime");
            scaleLerpTimeProperty = serializedObject.FindProperty("scaleLerpTime");
            maintainScaleProperty = serializedObject.FindProperty("maintainScaleOnInitialization");
            smoothingProperty = serializedObject.FindProperty("smoothing");
            lifetimeProperty = serializedObject.FindProperty("lifetime");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(updateLinkedTransformProperty);
            EditorGUILayout.PropertyField(moveLerpTimeProperty);
            EditorGUILayout.PropertyField(rotateLerpTimeProperty);
            EditorGUILayout.PropertyField(scaleLerpTimeProperty);
            EditorGUILayout.PropertyField(maintainScaleProperty);
            EditorGUILayout.PropertyField(smoothingProperty);
            EditorGUILayout.PropertyField(lifetimeProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
