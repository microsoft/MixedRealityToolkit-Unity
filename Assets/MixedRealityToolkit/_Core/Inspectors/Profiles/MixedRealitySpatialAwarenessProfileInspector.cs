// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessProfile))]
    public class MixedRealitySpatialAwarenessProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        // General settings
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty isStationaryObserver;
        private SerializedProperty updateInterval;

        // Mesh settings
        private SerializedProperty useMeshSystem;
        private SerializedProperty meshPhysicsLayer;
        private SerializedProperty meshLevelOfDetail;
        private SerializedProperty meshTrianglesPerCubicMeter;
        private SerializedProperty meshRecalculateNormals;
        private SerializedProperty meshDisplayOption;
        private SerializedProperty meshVisibleMaterial;
        private SerializedProperty meshOcclusionMaterial;

        // Surface Finding settings
        private SerializedProperty useSurfaceFindingSystem;
        private SerializedProperty surfaceFindingPhysicsLayer;
        private SerializedProperty surfaceFindingMinimumArea;
        private SerializedProperty displayFloorSurfaces;
        private SerializedProperty floorSurfaceMaterial;
        private SerializedProperty displayCeilingSurfaces;
        private SerializedProperty ceilingSurfaceMaterial;
        private SerializedProperty displayWallSurfaces;
        private SerializedProperty wallSurfaceMaterial;
        private SerializedProperty displayPlatformSurfaces;
        private SerializedProperty platformSurfaceMaterial;

        private readonly GUIContent ceilingMaterialContent = new GUIContent("Ceiling Material");
        private readonly GUIContent floorMaterialContent = new GUIContent("Floor Material");
        private readonly GUIContent occlusionMaterialContent = new GUIContent("Occlusion Material");
        private readonly GUIContent platformMaterialContent = new GUIContent("Platform Material");
        private readonly GUIContent visibleMaterialContent = new GUIContent("Visible Material");
        private readonly GUIContent wallMaterialContent = new GUIContent("Wall Material");

        private readonly GUIContent displayOptionContent = new GUIContent("Display Option");
        private readonly GUIContent lodContent = new GUIContent("Level of Detail");
        private readonly GUIContent minimumAreaContent = new GUIContent("Minimum Area");
        private readonly GUIContent physicsLayerContent = new GUIContent("Physics Layer");
        private readonly GUIContent trianglesPerCubicMeterContent = new GUIContent("Triangles/Cubic Meter");
        private readonly GUIContent useSystemContent = new GUIContent("Use System");

        protected override void OnEnable()
        {
            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            // General settings
            startupBehavior = serializedObject.FindProperty("startupBehavior");
            observationExtents = serializedObject.FindProperty("observationExtents");
            isStationaryObserver = serializedObject.FindProperty("isStationaryObserver");
            updateInterval = serializedObject.FindProperty("updateInterval");

            // Mesh settings
            useMeshSystem = serializedObject.FindProperty("useMeshSystem");
            meshPhysicsLayer = serializedObject.FindProperty("meshPhysicsLayer");
            meshLevelOfDetail = serializedObject.FindProperty("meshLevelOfDetail");
            meshTrianglesPerCubicMeter = serializedObject.FindProperty("meshTrianglesPerCubicMeter");
            meshRecalculateNormals = serializedObject.FindProperty("meshRecalculateNormals");
            meshDisplayOption = serializedObject.FindProperty("meshDisplayOption");
            meshVisibleMaterial = serializedObject.FindProperty("meshMaterial");
            meshVisibleMaterial = serializedObject.FindProperty("meshVisibleMaterial");
            meshOcclusionMaterial = serializedObject.FindProperty("meshOcclusionMaterial");

            // Surface Finding settings
            useSurfaceFindingSystem = serializedObject.FindProperty("useSurfaceFindingSystem");
            surfaceFindingPhysicsLayer = serializedObject.FindProperty("surfaceFindingPhysicsLayer");
            surfaceFindingMinimumArea = serializedObject.FindProperty("surfaceFindingMinimumArea");
            displayFloorSurfaces = serializedObject.FindProperty("displayFloorSurfaces");
            floorSurfaceMaterial = serializedObject.FindProperty("floorSurfaceMaterial");
            displayCeilingSurfaces = serializedObject.FindProperty("displayCeilingSurfaces");
            ceilingSurfaceMaterial = serializedObject.FindProperty("ceilingSurfaceMaterial");
            displayWallSurfaces = serializedObject.FindProperty("displayWallSurfaces");
            wallSurfaceMaterial = serializedObject.FindProperty("wallSurfaceMaterial");
            displayPlatformSurfaces = serializedObject.FindProperty("displayPlatformSurfaces");
            platformSurfaceMaterial = serializedObject.FindProperty("platformSurfaceMaterial");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Awareness Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Spatial Awareness can enhance your experience by enabling objects to interact with the real world.", MessageType.Info);
            EditorGUILayout.Space();
            serializedObject.Update();

            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.LabelField("General Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(startupBehavior);
            EditorGUILayout.PropertyField(observationExtents);
            EditorGUILayout.PropertyField(isStationaryObserver);
            EditorGUILayout.PropertyField(updateInterval);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mesh Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useMeshSystem, useSystemContent);
            EditorGUILayout.PropertyField(meshPhysicsLayer, physicsLayerContent);
            EditorGUILayout.PropertyField(meshLevelOfDetail, lodContent);
            EditorGUILayout.PropertyField(meshTrianglesPerCubicMeter, trianglesPerCubicMeterContent);
            EditorGUILayout.PropertyField(meshRecalculateNormals);
            EditorGUILayout.PropertyField(meshDisplayOption, displayOptionContent);
            EditorGUILayout.PropertyField(meshVisibleMaterial, visibleMaterialContent);
            EditorGUILayout.PropertyField(meshOcclusionMaterial, occlusionMaterialContent);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Surface Finding Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(useSurfaceFindingSystem, useSystemContent);
            EditorGUILayout.PropertyField(surfaceFindingPhysicsLayer, physicsLayerContent);
            EditorGUILayout.PropertyField(surfaceFindingMinimumArea, minimumAreaContent);
            EditorGUILayout.PropertyField(displayFloorSurfaces);
            EditorGUILayout.PropertyField(floorSurfaceMaterial, floorMaterialContent);
            EditorGUILayout.PropertyField(displayCeilingSurfaces);
            EditorGUILayout.PropertyField(ceilingSurfaceMaterial, ceilingMaterialContent);
            EditorGUILayout.PropertyField(displayWallSurfaces);
            EditorGUILayout.PropertyField(wallSurfaceMaterial, wallMaterialContent);
            EditorGUILayout.PropertyField(displayPlatformSurfaces);
            EditorGUILayout.PropertyField(platformSurfaceMaterial, platformMaterialContent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
