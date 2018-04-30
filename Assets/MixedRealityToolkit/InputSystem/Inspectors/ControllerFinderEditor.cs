// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.InputSystem.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Inspectors
{
    [CustomEditor(typeof(ControllerFinder))]
    public abstract class ControllerFinderEditor : Editor
    {
        private static GUIStyle controllerOptionsGuiStyle;

        private SerializedProperty handednessProperty;
        private SerializedProperty elementProperty;

        protected virtual void Awake()
        {
            controllerOptionsGuiStyle = new GUIStyle("Label") { fontStyle = FontStyle.Bold };
        }

        protected virtual void OnEnable()
        {
            handednessProperty = serializedObject.FindProperty("handedness");
            elementProperty = serializedObject.FindProperty("element");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controller Options", controllerOptionsGuiStyle);
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(handednessProperty);
            EditorGUILayout.PropertyField(elementProperty);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}