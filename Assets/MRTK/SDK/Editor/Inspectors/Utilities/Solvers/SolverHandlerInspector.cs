// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(SolverHandler))]
    [CanEditMultipleObjects]
    public class SolverHandlerInspector : UnityEditor.Editor
    {
        private SerializedProperty trackedTargetProperty;
        private SerializedProperty trackedHandednessProperty;
        private SerializedProperty trackedHandJointProperty;
        private SerializedProperty transformOverrideProperty;
        private SerializedProperty additionalOffsetProperty;
        private SerializedProperty additionalRotationProperty;
        private SerializedProperty updateSolversProperty;
        private SolverHandler solverHandler;

        private static readonly GUIContent TrackedTypeLabel = new GUIContent("Tracked Target Type");

        protected void OnEnable()
        {
            trackedTargetProperty = serializedObject.FindProperty("trackedTargetType");
            trackedHandednessProperty = serializedObject.FindProperty("trackedHandedness");
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

            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            EditorGUI.BeginChangeCheck();

            InspectorUIUtility.DrawEnumSerializedProperty(trackedTargetProperty, TrackedTypeLabel, solverHandler.TrackedTargetType);

            if (!SolverHandler.IsValidTrackedObjectType(solverHandler.TrackedTargetType))
            {
                InspectorUIUtility.DrawWarning(" Current Tracked Target Type value of \""
                    + Enum.GetName(typeof(TrackedObjectType), solverHandler.TrackedTargetType)
                    + "\" is obsolete. Select MotionController or HandJoint values instead");
            }

            if (trackedTargetProperty.intValue == (int)TrackedObjectType.HandJoint ||
                trackedTargetProperty.intValue == (int)TrackedObjectType.ControllerRay)
            {
                EditorGUILayout.PropertyField(trackedHandednessProperty);
                if (trackedHandednessProperty.intValue > (int)Handedness.Both)
                {
                    InspectorUIUtility.DrawWarning("Only Handedness values of None, Left, Right, and Both are valid");
                }
            }

            if (trackedTargetProperty.intValue == (int)TrackedObjectType.HandJoint)
            {
                EditorGUILayout.PropertyField(trackedHandJointProperty);
            }
            else if (trackedTargetProperty.intValue == (int)TrackedObjectType.CustomOverride)
            {
                EditorGUILayout.PropertyField(transformOverrideProperty);
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
