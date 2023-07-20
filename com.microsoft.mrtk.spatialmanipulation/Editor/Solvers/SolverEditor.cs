// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    /// <summary>
    /// A custom editor for the <see cref="Solver"/> class.
    /// </summary>
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

        /// <summary>
        /// A Unity event function that is called when the script component has been enabled.
        /// </summary>
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

        /// <summary>
        /// Called by the Unity editor to render custom inspector UI for this component.
        /// </summary>
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
