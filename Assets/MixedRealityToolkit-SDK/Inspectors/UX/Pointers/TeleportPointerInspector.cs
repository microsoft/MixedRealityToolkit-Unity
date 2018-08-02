// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(ParabolicTeleportPointer))]
    public class ParabolicTeleportPointerInspector : TeleportPointerInspector
    {
        private SerializedProperty minParabolaVelocity;
        private SerializedProperty maxParabolaVelocity;

        private bool parabolicTeleportFoldout = true;

        protected override void OnEnable()
        {
            base.OnEnable();

            minParabolaVelocity = serializedObject.FindProperty("minParabolaVelocity");
            maxParabolaVelocity = serializedObject.FindProperty("maxParabolaVelocity");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            parabolicTeleportFoldout = EditorGUILayout.Foldout(parabolicTeleportFoldout, "Parabolic Pointer Options", true);

            if (parabolicTeleportFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(minParabolaVelocity);
                EditorGUILayout.PropertyField(maxParabolaVelocity);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(TeleportPointer))]
    public class TeleportPointerInspector : LinePointerInspector
    {
        private SerializedProperty teleportAction;
        private SerializedProperty inputThreshold;
        private SerializedProperty angleOffset;
        private SerializedProperty minValidDot;
        private SerializedProperty lineColorHotSpot;
        private SerializedProperty validLayers;
        private SerializedProperty invalidLayers;

        private bool teleportPointerFoldout = true;

        protected override void OnEnable()
        {
            DrawBasePointerActions = false;
            base.OnEnable();

            teleportAction = serializedObject.FindProperty("teleportAction");
            inputThreshold = serializedObject.FindProperty("inputThreshold");
            angleOffset = serializedObject.FindProperty("angleOffset");
            minValidDot = serializedObject.FindProperty("minValidDot");
            lineColorHotSpot = serializedObject.FindProperty("LineColorHotSpot");
            validLayers = serializedObject.FindProperty("ValidLayers");
            invalidLayers = serializedObject.FindProperty("InvalidLayers");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            teleportPointerFoldout = EditorGUILayout.Foldout(teleportPointerFoldout, "Teleport Pointer Settings", true);

            if (teleportPointerFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(teleportAction);
                EditorGUILayout.PropertyField(inputThreshold);
                EditorGUILayout.PropertyField(angleOffset);
                EditorGUILayout.PropertyField(minValidDot);
                EditorGUILayout.PropertyField(lineColorHotSpot);
                EditorGUILayout.PropertyField(validLayers);
                EditorGUILayout.PropertyField(invalidLayers);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}