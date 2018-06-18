// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;

namespace HoloToolkit.Unity.UX
{
    [CustomEditor(typeof(SolverInBetween))]
    public class SolverInBetweenEditor : Editor
    {
        private SerializedProperty trackedObjectReferenceProperty;
        private SerializedProperty transformTargetProperty;
        private SolverInBetween solverInBetween;

        private static readonly string[] fieldsToExclude = new string[] { "m_Script" };

        private void OnEnable()
        {
            trackedObjectReferenceProperty = serializedObject.FindProperty("trackedObjectForSecondTransform");
            transformTargetProperty = serializedObject.FindProperty("secondTransformOverride");

            solverInBetween = target as SolverInBetween;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(trackedObjectReferenceProperty);
            bool trackedObjectChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(transformTargetProperty);

            DrawPropertiesExcluding(serializedObject, fieldsToExclude);

            serializedObject.ApplyModifiedProperties();

            if (trackedObjectChanged)
            {
                solverInBetween.AttachSecondTransformToNewTrackedObject();
            }
        }
    }
}
