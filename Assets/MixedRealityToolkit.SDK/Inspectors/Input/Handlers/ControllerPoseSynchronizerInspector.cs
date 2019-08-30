// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEngine;
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(ControllerPoseSynchronizer))]
    public class ControllerPoseSynchronizerInspector : UnityEditor.Editor
    {
        private const string SynchronizationSettingsKey = "MRTK_Inspector_SynchronizationSettingsFoldout";

        private static bool synchronizationSettingsFoldout = true;

        private SerializedProperty handedness;
        private SerializedProperty useSourcePoseData;
        private SerializedProperty poseAction;

        protected bool DrawHandednessProperty = true;

        protected virtual void OnEnable()
        {
            synchronizationSettingsFoldout = SessionState.GetBool(SynchronizationSettingsKey, synchronizationSettingsFoldout);
            handedness = serializedObject.FindProperty("handedness");
            useSourcePoseData = serializedObject.FindProperty("useSourcePoseData");
            poseAction = serializedObject.FindProperty("poseAction");
        }

        public override void OnInspectorGUI()
        {
            if (target != null)
            {
                InspectorUIUtility.RenderHelpURL(target.GetType());
            }

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            synchronizationSettingsFoldout = EditorGUILayout.Foldout(synchronizationSettingsFoldout, "Synchronization Settings", true);

            if (EditorGUI.EndChangeCheck())
            {
                SessionState.SetBool(SynchronizationSettingsKey, synchronizationSettingsFoldout);
            }

            if (!synchronizationSettingsFoldout) { return; }

            EditorGUI.indentLevel++;

            if (DrawHandednessProperty)
            {
                HandednessInspectorGUI.DrawControllerHandednessDropdown(handedness);
            }

            EditorGUILayout.PropertyField(useSourcePoseData);

            if (!useSourcePoseData.boolValue)
            {
                EditorGUILayout.PropertyField(poseAction);
            }

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
        }
    }
}