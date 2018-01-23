// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;

namespace HoloToolkit.Unity.InputModule
{
    [CustomEditor(typeof(AttachToController))]
    public class AttachToControllerEditor : Editor
    {
        private SerializedProperty handednessProperty;
        private SerializedProperty elementProperty;
        private SerializedProperty positionOffsetProperty;
        private SerializedProperty rotationOffsetProperty;
        private SerializedProperty scaleOffsetProperty;
        private SerializedProperty setScaleOnAttachProperty;
        private SerializedProperty setChildrenInactiveWhenDetachedProperty;

        private void OnEnable()
        {
            handednessProperty = serializedObject.FindProperty("handedness");
            elementProperty = serializedObject.FindProperty("element");
            positionOffsetProperty = serializedObject.FindProperty("PositionOffset");
            rotationOffsetProperty = serializedObject.FindProperty("RotationOffset");
            scaleOffsetProperty = serializedObject.FindProperty("ScaleOffset");
            setScaleOnAttachProperty = serializedObject.FindProperty("SetScaleOnAttach");
            setChildrenInactiveWhenDetachedProperty = serializedObject.FindProperty("SetChildrenInactiveWhenDetached");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(handednessProperty);
            EditorGUILayout.PropertyField(elementProperty);
            EditorGUILayout.PropertyField(positionOffsetProperty);
            EditorGUILayout.PropertyField(rotationOffsetProperty);
            EditorGUILayout.PropertyField(scaleOffsetProperty);
            EditorGUILayout.PropertyField(setScaleOnAttachProperty);
            EditorGUILayout.PropertyField(setChildrenInactiveWhenDetachedProperty);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
