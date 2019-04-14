// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(SolverHandler))]
    [CanEditMultipleObjects]
    public class SolverHandlerInspector : ControllerFinderInspector
    {
        private SerializedProperty trackedObjectProperty;
        private SerializedProperty trackedHandJointProperty;
        private SerializedProperty transformTargetProperty;
        private SerializedProperty additionalOffsetProperty;
        private SerializedProperty additionalRotationProperty;
        private SerializedProperty updateSolversProperty;
        private SolverHandler solverHandler;
        private readonly GUIContent trackedTransformGUIContent = new GUIContent("Tracked Transform");

        protected override void OnEnable()
        {
            base.OnEnable();

            trackedObjectProperty = serializedObject.FindProperty("trackedObjectToReference");
            trackedHandJointProperty = serializedObject.FindProperty("trackedHandJoint");
            transformTargetProperty = serializedObject.FindProperty("transformTarget");
            additionalOffsetProperty = serializedObject.FindProperty("additionalOffset");
            additionalRotationProperty = serializedObject.FindProperty("additionalRotation");
            updateSolversProperty = serializedObject.FindProperty("updateSolvers");

            solverHandler = target as SolverHandler;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            bool trackedObjectChanged = false;

            if (transformTargetProperty.objectReferenceValue == null || Application.isPlaying)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(trackedObjectProperty);
                if (trackedObjectProperty.enumValueIndex == (int)TrackedObjectType.HandJointLeft ||
                trackedObjectProperty.enumValueIndex == (int)TrackedObjectType.HandJointRight)
                {
                    EditorGUILayout.PropertyField(trackedHandJointProperty);
                }
                else if (trackedObjectProperty.enumValueIndex == (int)TrackedObjectType.MotionControllerLeft ||
                    trackedObjectProperty.enumValueIndex == (int)TrackedObjectType.MotionControllerRight)
                {
                    // TODO: Add tracked controller element back in. Pending visualization system updates.
                    // EditorGUILayout.PropertyField(trackedControllerElementProperty);
                }

                trackedObjectChanged = EditorGUI.EndChangeCheck();
            }

            EditorGUILayout.PropertyField(transformTargetProperty, trackedTransformGUIContent);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(additionalOffsetProperty);
            EditorGUILayout.PropertyField(additionalRotationProperty);
            bool additionalOffsetChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(updateSolversProperty);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && trackedObjectChanged)
            {
                solverHandler.RefreshTrackedObject();
            }

            if (Application.isPlaying && additionalOffsetChanged)
            {
                solverHandler.AdditionalOffset = additionalOffsetProperty.vector3Value;
                solverHandler.AdditionalRotation = additionalRotationProperty.vector3Value;
            }
        }
    }
}
