// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(InBetween))]
    public class InBetweenEditor : UnityEditor.Editor
    {
        private SerializedProperty trackedObjectProperty;
        private SerializedProperty secondTrackedHandJointProperty;
        private SerializedProperty transformTargetProperty;
        private InBetween solverInBetween;

        private static readonly string[] fieldsToExclude = new string[] { "m_Script" };

        private void OnEnable()
        {
            trackedObjectProperty = serializedObject.FindProperty("trackedObjectForSecondTransform");
            secondTrackedHandJointProperty = serializedObject.FindProperty("secondTrackedHandJoint");
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

                if (trackedObjectProperty.intValue == (int)TrackedObjectType.HandJointLeft ||
                trackedObjectProperty.intValue == (int)TrackedObjectType.HandJointRight)
                {
                    EditorGUILayout.PropertyField(secondTrackedHandJointProperty);
                }
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
