// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessProfile))]
    public class MixedRealitySpatialAwarenessProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        // General settings
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty updateInterval;

        // Mesh settings
        // todo

        // Surface Finding settings
        // todo

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            // General settings
            startupBehavior = serializedObject.FindProperty("startupBehavior");
            observationExtents = serializedObject.FindProperty("observationExtents");
            updateInterval = serializedObject.FindProperty("updateInterval");

            // Mesh settings
            // todo

            // Surface Finding settings
            // todo
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityManager())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityManager.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Awareness Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Spatial Awareness can help ground the experience in reality.", MessageType.Info);
            EditorGUILayout.Space();
            serializedObject.Update();

            EditorGUILayout.LabelField("General Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(startupBehavior);
            EditorGUILayout.PropertyField(observationExtents);
            EditorGUILayout.PropertyField(updateInterval);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
