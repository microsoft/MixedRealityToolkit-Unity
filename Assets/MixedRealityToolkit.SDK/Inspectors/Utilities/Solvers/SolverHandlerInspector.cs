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
        private SerializedProperty trackedTargetProperty;
        private SerializedProperty trackedHandnessProperty;
        private SerializedProperty trackedHandJointProperty;
        private SerializedProperty transformOverrideProperty;
        private SerializedProperty additionalOffsetProperty;
        private SerializedProperty additionalRotationProperty;
        private SerializedProperty updateSolversProperty;
        private SolverHandler solverHandler;

        protected override void OnEnable()
        {
            base.OnEnable();

            trackedTargetProperty = serializedObject.FindProperty("trackedTargetType");
            trackedHandnessProperty = serializedObject.FindProperty("trackedHandness");
            trackedHandJointProperty = serializedObject.FindProperty("trackedHandJoint");
            transformOverrideProperty = serializedObject.FindProperty("transformOverride");
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

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(trackedTargetProperty);

            if (trackedTargetProperty.enumValueIndex == (int)TrackedObjectType.HandJoint ||
                trackedTargetProperty.enumValueIndex == (int)TrackedObjectType.MotionController)
            {
                EditorGUILayout.PropertyField(trackedHandnessProperty);
            }

            if (trackedTargetProperty.enumValueIndex == (int)TrackedObjectType.HandJoint)
            {
                EditorGUILayout.PropertyField(trackedHandJointProperty);
            }
            else if (trackedTargetProperty.enumValueIndex == (int)TrackedObjectType.CustomOverride)
            {
                EditorGUILayout.PropertyField(transformOverrideProperty);
            }

            EditorGUILayout.PropertyField(additionalOffsetProperty);
            EditorGUILayout.PropertyField(additionalRotationProperty);

            trackedObjectChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(updateSolversProperty);

            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying && trackedObjectChanged)
            {
                solverHandler.RefreshTrackedObject();
            }

            /*
            if (Application.isPlaying && additionalOffsetChanged)
            {
                solverHandler.AdditionalOffset = additionalOffsetProperty.vector3Value;
                solverHandler.AdditionalRotation = additionalRotationProperty.vector3Value;
            }*/
        }
    }
}
