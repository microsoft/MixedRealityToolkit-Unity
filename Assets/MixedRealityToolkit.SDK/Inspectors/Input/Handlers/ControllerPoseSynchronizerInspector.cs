// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(ControllerPoseSynchronizer))]
    public class ControllerPoseSynchronizerInspector : UnityEditor.Editor
    {
        private const string SynchronizationSettingsKey = "MRTK_Inspector_SynchronizationSettingsFoldout";
        private static readonly GUIContent[] HandednessSelections =
{
            new GUIContent("Left"),
            new GUIContent("Right"),
            new GUIContent("Both"),
        };

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
                var currentHandedness = handedness.intValue - 1;

                // Reset in case it was set to something other than left, right or both.
                if (currentHandedness < 0 || currentHandedness > 2) { currentHandedness = 0; }

                EditorGUI.BeginChangeCheck();
                var newHandednessIndex = EditorGUILayout.IntPopup(new GUIContent(handedness.displayName, handedness.tooltip), currentHandedness, HandednessSelections, null);

                if (EditorGUI.EndChangeCheck())
                {
                    handedness.intValue = currentHandedness + 1;
                }
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