// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityHandTrackingProfile))]
    public class MixedRealityHandTrackingProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty jointPrefab;
        private SerializedProperty palmPrefab;
        private SerializedProperty fingertipPrefab;
        private SerializedProperty handMeshPrefab;
        private SerializedProperty enableHandMeshVisualization;
        private SerializedProperty enableHandJointVisualization;

        protected override void OnEnable()
        {
            base.OnEnable();

            jointPrefab = serializedObject.FindProperty("jointPrefab");
            fingertipPrefab = serializedObject.FindProperty("fingertipPrefab");
            palmPrefab = serializedObject.FindProperty("palmPrefab");
            handMeshPrefab = serializedObject.FindProperty("handMeshPrefab");
            enableHandMeshVisualization = serializedObject.FindProperty("enableHandMeshVisualization");
            enableHandJointVisualization = serializedObject.FindProperty("enableHandJointVisualization");
        }

        public override void OnInspectorGUI()
        {
            RenderTitleDescriptionAndLogo(
                "Hand tracking settings",
                "Use this for platform-specific hand tracking settings.");

            if (MixedRealityInspectorUtility.CheckMixedRealityConfigured(true, !RenderAsSubProfile))
            {
                if (GUILayout.Button("Back to Input Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
                }
            }

            CheckProfileLock(target);

            serializedObject.Update();

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("General settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(jointPrefab);
            EditorGUILayout.PropertyField(palmPrefab);
            EditorGUILayout.PropertyField(fingertipPrefab);
            EditorGUILayout.PropertyField(handMeshPrefab);
            EditorGUILayout.PropertyField(enableHandMeshVisualization);
            EditorGUILayout.PropertyField(enableHandJointVisualization);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
