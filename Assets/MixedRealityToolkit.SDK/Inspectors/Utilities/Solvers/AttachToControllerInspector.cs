// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers
{
    [CustomEditor(typeof(AttachToController))]
    public class AttachToControllerInspector : ControllerFinderInspector
    {
        private SerializedProperty positionOffsetProperty;
        private SerializedProperty rotationOffsetProperty;
        private SerializedProperty scaleOffsetProperty;
        private SerializedProperty setScaleOnAttachProperty;
        private SerializedProperty setChildrenInactiveWhenDetachedProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            positionOffsetProperty = serializedObject.FindProperty("PositionOffset");
            rotationOffsetProperty = serializedObject.FindProperty("RotationOffset");
            scaleOffsetProperty = serializedObject.FindProperty("ScaleOffset");
            setScaleOnAttachProperty = serializedObject.FindProperty("SetScaleOnAttach");
            setChildrenInactiveWhenDetachedProperty = serializedObject.FindProperty("SetChildrenInactiveWhenDetached");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Attachment Options", new GUIStyle("Label") { fontStyle = FontStyle.Bold });
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(positionOffsetProperty);
            EditorGUILayout.PropertyField(rotationOffsetProperty);
            EditorGUILayout.PropertyField(scaleOffsetProperty);
            EditorGUILayout.PropertyField(setScaleOnAttachProperty);
            EditorGUILayout.PropertyField(setChildrenInactiveWhenDetachedProperty);

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
