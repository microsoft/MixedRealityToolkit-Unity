// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.SDK.UX.Pointers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.SDK.Inspectors.UX.Pointers
{
    [CustomEditor(typeof(TeleportPointer))]
    public class TeleportPointerInspector : LinePointerInspector
    {
        private SerializedProperty teleportAction;
        private SerializedProperty inputThreshold;
        private SerializedProperty angleOffset;
        private SerializedProperty teleportActivationAngle;
        private SerializedProperty rotateActivationAngle;
        private SerializedProperty rotationAmount;
        private SerializedProperty backStrafeActivationAngle;
        private SerializedProperty strafeAmount;
        private SerializedProperty upDirectionThreshold;
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
            teleportActivationAngle = serializedObject.FindProperty("teleportActivationAngle");
            rotateActivationAngle = serializedObject.FindProperty("rotateActivationAngle");
            rotationAmount = serializedObject.FindProperty("rotationAmount");
            backStrafeActivationAngle = serializedObject.FindProperty("backStrafeActivationAngle");
            strafeAmount = serializedObject.FindProperty("strafeAmount");
            upDirectionThreshold = serializedObject.FindProperty("upDirectionThreshold");
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
                EditorGUILayout.PropertyField(teleportActivationAngle);
                EditorGUILayout.PropertyField(rotateActivationAngle);
                EditorGUILayout.PropertyField(rotationAmount);
                EditorGUILayout.PropertyField(backStrafeActivationAngle);
                EditorGUILayout.PropertyField(strafeAmount);
                EditorGUILayout.PropertyField(upDirectionThreshold);
                EditorGUILayout.PropertyField(lineColorHotSpot);
                EditorGUILayout.PropertyField(validLayers);
                EditorGUILayout.PropertyField(invalidLayers);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}