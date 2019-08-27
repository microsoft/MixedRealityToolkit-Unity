// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Input.Editor
{
    [CustomEditor(typeof(ControllerPoseSynchronizer))]
    public class ControllerPoseSynchronizerInspector : UnityEditor.Editor
    {
        private const string SynchronizationSettingsKey = "MRTK_Inspector_SynchronizationSettingsFoldout";
        private static readonly string[] HandednessLabels = { "Left", "Right" };

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
                var currentHandedness = (Handedness)handedness.enumValueIndex;
                var handIndex = currentHandedness == Handedness.Right ? 1 : 0;

                EditorGUI.BeginChangeCheck();
                var newHandednessIndex = EditorGUILayout.Popup(handedness.displayName, handIndex, HandednessLabels);

                if (EditorGUI.EndChangeCheck())
                {
                    currentHandedness = newHandednessIndex == 0 ? Handedness.Left : Handedness.Right;
                    handedness.enumValueIndex = (int)currentHandedness;
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