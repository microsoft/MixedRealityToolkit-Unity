// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(LinePointer))]
    public class LinePointerInspector : BaseControllerPointerInspector
    {
        private SerializedProperty lineColorSelected;
        private SerializedProperty lineColorValid;
        private SerializedProperty lineColorInvalid;
        private SerializedProperty lineColorNoTarget;
        private SerializedProperty lineColorLockFocus;
        private SerializedProperty lineCastResolution;
        private SerializedProperty lineRenderers;

        private bool linePointerFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            lineColorSelected = serializedObject.FindProperty("LineColorSelected");
            lineColorValid = serializedObject.FindProperty("LineColorValid");
            lineColorInvalid = serializedObject.FindProperty("LineColorInvalid");
            lineColorNoTarget = serializedObject.FindProperty("LineColorNoTarget");
            lineColorLockFocus = serializedObject.FindProperty("LineColorLockFocus");
            lineCastResolution = serializedObject.FindProperty("LineCastResolution");
            lineRenderers = serializedObject.FindProperty("lineRenderers");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            linePointerFoldout = EditorGUILayout.Foldout(linePointerFoldout, "Line Pointer Settings", true);

            if (linePointerFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(lineColorSelected);
                EditorGUILayout.PropertyField(lineColorValid);
                EditorGUILayout.PropertyField(lineColorInvalid);
                EditorGUILayout.PropertyField(lineColorNoTarget);
                EditorGUILayout.PropertyField(lineColorLockFocus);
                EditorGUILayout.PropertyField(lineCastResolution);
                EditorGUILayout.PropertyField(lineRenderers, true);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}