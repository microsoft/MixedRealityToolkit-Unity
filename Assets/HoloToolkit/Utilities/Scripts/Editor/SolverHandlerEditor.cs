// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using HoloToolkit.Unity.InputModule;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity.UX
{
    [CustomEditor(typeof(SolverHandler))]
    public class SolverHandlerEditor : ControllerFinderEditor
    {
        private SerializedProperty trackedObjectReferenceProperty;
        private SerializedProperty transformTargetProperty;
        private SerializedProperty additionalOffsetProperty;
        private SerializedProperty additionalRotationProperty;
        private SolverHandler solverHandler;
        private GUIContent trackedTransformGUIContent = new GUIContent("Tracked Transform");

        protected override void OnEnable()
        {
            base.OnEnable();

            trackedObjectReferenceProperty = serializedObject.FindProperty("trackedObjectToReference");
            transformTargetProperty = serializedObject.FindProperty("transformTarget");
            additionalOffsetProperty = serializedObject.FindProperty("additionalOffset");
            additionalRotationProperty = serializedObject.FindProperty("additionalRotation");

            solverHandler = target as SolverHandler;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(trackedObjectReferenceProperty);
            bool trackedObjectChanged = EditorGUI.EndChangeCheck();
            
            if (!Application.isPlaying)
            {
                EditorGUILayout.PropertyField(additionalOffsetProperty);
                EditorGUILayout.PropertyField(additionalRotationProperty);
            }

            EditorGUILayout.PropertyField(transformTargetProperty, trackedTransformGUIContent);

            serializedObject.ApplyModifiedProperties();
            if (trackedObjectChanged)
            {
                solverHandler.TransformTarget = null;
                solverHandler.AttachToNewTrackedObject();
            }
        }
    }
}
