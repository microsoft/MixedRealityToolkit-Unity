// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SDK.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.Utilities.Solvers
{
    [CustomEditor(typeof(InBetween))]
    public class InBetweenEditor : Editor
    {
        private SerializedProperty trackedObjectProperty;
        private SerializedProperty transformTargetProperty;
        private InBetween solverInBetween;

        private static readonly string[] fieldsToExclude = new string[] { "m_Script" };

        private void OnEnable()
        {
            trackedObjectProperty = serializedObject.FindProperty("trackedObjectForSecondTransform");
            transformTargetProperty = serializedObject.FindProperty("secondTransformOverride");

            solverInBetween = target as InBetween;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            bool trackedObjectChanged = false;

            if (transformTargetProperty.objectReferenceValue == null)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(trackedObjectProperty);
                trackedObjectChanged = EditorGUI.EndChangeCheck();
            }

            EditorGUILayout.PropertyField(transformTargetProperty);

            DrawPropertiesExcluding(serializedObject, fieldsToExclude);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && trackedObjectChanged)
            {
                solverInBetween.AttachSecondTransformToNewTrackedObject();
            }
        }
    }
}
