// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation.Editor
{
    [CustomEditor(typeof(SolverHandler))]
    [CanEditMultipleObjects]
    public class SolverHandlerEditor : UnityEditor.Editor
    {
        private SerializedProperty trackedTargetProperty;
        private SerializedProperty trackedHandednessProperty;
        private SerializedProperty trackedHandJointProperty;
        private SerializedProperty transformOverrideProperty;
        private SerializedProperty additionalOffsetProperty;
        private SerializedProperty additionalRotationProperty;
        private SerializedProperty leftInteractor;
        private SerializedProperty rightInteractor;
        private SerializedProperty updateSolversProperty;

        private SolverHandler solverHandler;

        protected void OnEnable()
        {
            trackedTargetProperty = serializedObject.FindProperty("trackedTargetType");
            trackedHandednessProperty = serializedObject.FindProperty("trackedHandedness");
            trackedHandJointProperty = serializedObject.FindProperty("trackedHandJoint");
            transformOverrideProperty = serializedObject.FindProperty("transformOverride");
            additionalOffsetProperty = serializedObject.FindProperty("additionalOffset");
            additionalRotationProperty = serializedObject.FindProperty("additionalRotation");
            leftInteractor = serializedObject.FindProperty("leftInteractor");
            rightInteractor = serializedObject.FindProperty("rightInteractor");
            updateSolversProperty = serializedObject.FindProperty("updateSolvers");

            solverHandler = target as SolverHandler;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(trackedTargetProperty);

            if (trackedTargetProperty.intValue == (int)TrackedObjectType.HandJoint ||
                trackedTargetProperty.intValue == (int)TrackedObjectType.ControllerRay)
            {
                EditorGUILayout.PropertyField(trackedHandednessProperty);
            }

            if (trackedTargetProperty.intValue == (int)TrackedObjectType.HandJoint)
            {
                EditorGUILayout.PropertyField(trackedHandJointProperty);
            }
            else if (trackedTargetProperty.intValue == (int)TrackedObjectType.CustomOverride)
            {
                EditorGUILayout.PropertyField(transformOverrideProperty);
            }
            else if (trackedTargetProperty.intValue == (int)TrackedObjectType.ControllerRay)
            {
                EditorGUILayout.PropertyField(leftInteractor);
                EditorGUILayout.PropertyField(rightInteractor);
            }

            EditorGUILayout.PropertyField(additionalOffsetProperty);
            EditorGUILayout.PropertyField(additionalRotationProperty);

            bool trackedObjectChanged = EditorGUI.EndChangeCheck();

            EditorGUILayout.PropertyField(updateSolversProperty);

            serializedObject.ApplyModifiedProperties();

            if (EditorApplication.isPlaying && trackedObjectChanged)
            {
                solverHandler.RefreshTrackedObject();
            }
        }
    }
}
