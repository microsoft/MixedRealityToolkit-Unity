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
        private SerializedProperty useMeshSystem;
        private SerializedProperty meshPhysicsLayer;
        private SerializedProperty meshLevelOfDetail;
        private SerializedProperty meshTrianglesPerCubicMeter;
        private SerializedProperty meshRecalculateNormals;
        private SerializedProperty displayMeshes;
        private SerializedProperty meshMaterial;

        // Surface Finding settings
        // todo

        private readonly GUIContent useSystemContent = new GUIContent("Use System");
        private readonly GUIContent physicsLayerContent = new GUIContent("Physics Layer");
        private readonly GUIContent lodContent = new GUIContent("Level of Detail");
        private readonly GUIContent trianglesPerCubicMeterContent = new GUIContent("Triangles/Cubic Meter");
        private readonly GUIContent materialContent = new GUIContent("Material");

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
            useMeshSystem = serializedObject.FindProperty("useMeshSystem");
            meshPhysicsLayer = serializedObject.FindProperty("meshPhysicsLayer");
            meshLevelOfDetail = serializedObject.FindProperty("meshLevelOfDetail");
            meshTrianglesPerCubicMeter = serializedObject.FindProperty("meshTrianglesPerCubicMeter");
            meshRecalculateNormals = serializedObject.FindProperty("meshRecalculateNormals");
            displayMeshes = serializedObject.FindProperty("displayMeshes");
            meshMaterial = serializedObject.FindProperty("meshMaterial");

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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mesh Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useMeshSystem, useSystemContent);
            EditorGUILayout.PropertyField(meshPhysicsLayer, physicsLayerContent);
            EditorGUILayout.PropertyField(meshLevelOfDetail, lodContent);
            EditorGUILayout.PropertyField(meshTrianglesPerCubicMeter, trianglesPerCubicMeterContent);
            EditorGUILayout.PropertyField(meshRecalculateNormals);
            EditorGUILayout.PropertyField(displayMeshes);
            EditorGUILayout.PropertyField(meshMaterial, materialContent);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Surface Finding Settings:", EditorStyles.boldLabel);


            serializedObject.ApplyModifiedProperties();
        }
    }
}
