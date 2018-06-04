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

        protected override void OnEnable()
        {
            base.OnEnable();

            trackedObjectReferenceProperty = serializedObject.FindProperty("trackedObjectToReference");
            transformTargetProperty = serializedObject.FindProperty("transformTarget");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(trackedObjectReferenceProperty);
            EditorGUILayout.PropertyField(transformTargetProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
