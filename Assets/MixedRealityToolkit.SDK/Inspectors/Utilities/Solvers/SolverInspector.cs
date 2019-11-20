// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(Solver))]
    [CanEditMultipleObjects]
    public class SolverInspector : UnityEditor.Editor
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
            maintainScaleProperty = serializedObject.FindProperty("maintainScale");
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
