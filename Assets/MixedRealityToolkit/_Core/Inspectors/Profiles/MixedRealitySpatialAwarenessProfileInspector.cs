// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessProfile))]
    public class MixedRealitySpatialAwarenessProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty autoStart;
        private SerializedProperty observationExtents;
        private SerializedProperty physicsLayer;
        private SerializedProperty updateInterval;

        private GUIContent updateIntervalContent = new GUIContent("Update Interval (seconds)");

        // Mesh properties
        private SerializedProperty trianglesPerCubicMeter;
        private SerializedProperty recalculateNormals;

        // Surface properties
        private SerializedProperty minimumSurfaceArea;

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            autoStart = serializedObject.FindProperty("autoStart");
            observationExtents = serializedObject.FindProperty("observationExtents");
            physicsLayer = serializedObject.FindProperty("physicsLayer");
            updateInterval = serializedObject.FindProperty("updateInterval");

            // Mesh properties
            trianglesPerCubicMeter = serializedObject.FindProperty("trianglesPerCubicMeter");
            recalculateNormals = serializedObject.FindProperty("recalculateNormals");

            // Surface properties
            minimumSurfaceArea = serializedObject.FindProperty("minimumSurfaceArea");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Spatial Awareness Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Spatial Awareness brings the real world environment into your experiences.", MessageType.Info);
            EditorGUILayout.Space();

            if (!CheckMixedRealityManager())
            {
                return;
            }

            serializedObject.Update();

            GUILayout.Space(12f);
            EditorGUILayout.PropertyField(autoStart);
            EditorGUILayout.PropertyField(observationExtents);
            EditorGUILayout.PropertyField(physicsLayer);
            EditorGUILayout.PropertyField(updateInterval, updateIntervalContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Mesh Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trianglesPerCubicMeter);
            EditorGUILayout.PropertyField(recalculateNormals);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Surface Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(minimumSurfaceArea);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
