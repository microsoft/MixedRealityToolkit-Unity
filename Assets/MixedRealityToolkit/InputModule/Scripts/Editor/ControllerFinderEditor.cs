// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using MixedRealityToolkit.InputModule.Utilities;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.InputModule.EditorScript
{
    [CustomEditor(typeof(ControllerFinder))]
    public abstract class ControllerFinderEditor : Editor
    {
        private SerializedProperty handednessProperty;
        private SerializedProperty elementProperty;

        protected virtual void OnEnable()
        {
            handednessProperty = serializedObject.FindProperty("handedness");
            elementProperty = serializedObject.FindProperty("element");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Options", new GUIStyle("Label") { fontStyle = FontStyle.Bold });
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(handednessProperty);
            EditorGUILayout.PropertyField(elementProperty);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
