// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessMeshObserverProfile))]
    public class MixedRealitySpatialAwarenessMeshObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        // General settings
        private static bool showGeneralProperties = true;
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty observerVolumeType;
        private SerializedProperty isStationaryObserver;
        private SerializedProperty updateInterval;

        // Physics settings
        private static bool showPhysicsProperties = true;
        private SerializedProperty meshPhysicsLayer;
        private SerializedProperty recalculateNormals;

        // Level of Detail settings
        private static bool showLevelOfDetailProperties = true;
        private SerializedProperty levelOfDetail;
        private SerializedProperty trianglesPerCubicMeter;

        // Display settings
        private static bool showDisplayProperties = true;
        private SerializedProperty displayOption;
        private SerializedProperty visibleMaterial;
        private SerializedProperty occlusionMaterial;

        private readonly GUIContent displayOptionContent = new GUIContent("Display Option");
        private readonly GUIContent lodContent = new GUIContent("Level of Detail");
        private readonly GUIContent volumeTypeContent = new GUIContent("Observer Shape");
        private readonly GUIContent physicsLayerContent = new GUIContent("Physics Layer");
        private readonly GUIContent trianglesPerCubicMeterContent = new GUIContent("Triangles/Cubic Meter");

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            // General settings
            startupBehavior = serializedObject.FindProperty("startupBehavior");
            observationExtents = serializedObject.FindProperty("observationExtents");
            observerVolumeType = serializedObject.FindProperty("observerVolumeType");
            isStationaryObserver = serializedObject.FindProperty("isStationaryObserver");
            updateInterval = serializedObject.FindProperty("updateInterval");

            // Mesh settings
            meshPhysicsLayer = serializedObject.FindProperty("meshPhysicsLayer");
            recalculateNormals = serializedObject.FindProperty("recalculateNormals");
            levelOfDetail = serializedObject.FindProperty("levelOfDetail");
            trianglesPerCubicMeter = serializedObject.FindProperty("trianglesPerCubicMeter");
            displayOption = serializedObject.FindProperty("displayOption");
            visibleMaterial = serializedObject.FindProperty("visibleMaterial");
            occlusionMaterial = serializedObject.FindProperty("occlusionMaterial");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Awareness Mesh Observer Profile", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Configuration settings for how the real-world environment will be perceived and displayed.", MessageType.Info);
            EditorGUILayout.Space();
            serializedObject.Update();

            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            showGeneralProperties = EditorGUILayout.Foldout(showGeneralProperties, "General Settings", true);
            if (showGeneralProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(startupBehavior);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(updateInterval);
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(isStationaryObserver);
                    EditorGUILayout.PropertyField(observerVolumeType, volumeTypeContent);
                    string message = string.Empty;
                    if (observerVolumeType.intValue == (int)VolumeType.AxisAlignedCube)
                    {
                        message = "Observed meshes will be aligned to the world coordinate space.";
                    }
                    else if (observerVolumeType.intValue == (int)VolumeType.UserAlignedCube)
                    {
                        message = "Observed meshes will be aligned to the user's coordinate space.";
                    }
                    else if (observerVolumeType.intValue == (int)VolumeType.Sphere)
                    {
                        message = "The X value of the Observation Extents will be used as the sphere radius.";
                    }
                    EditorGUILayout.HelpBox(message, MessageType.Info);
                    EditorGUILayout.PropertyField(observationExtents);
                }
            }

            EditorGUILayout.Space();
            showPhysicsProperties = EditorGUILayout.Foldout(showPhysicsProperties, "Physics Settings", true);
            if (showPhysicsProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(meshPhysicsLayer, physicsLayerContent);
                    EditorGUILayout.PropertyField(recalculateNormals);
                }
            }

            EditorGUILayout.Space();
            showLevelOfDetailProperties = EditorGUILayout.Foldout(showLevelOfDetailProperties, "Level of Detail Settings", true);
            if (showLevelOfDetailProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(levelOfDetail, lodContent);
                    EditorGUILayout.PropertyField(trianglesPerCubicMeter, trianglesPerCubicMeterContent);
                    EditorGUILayout.HelpBox("The value of Triangles per Cubic Meter is ignored unless Level of Detail is set to Custom.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();
            showDisplayProperties = EditorGUILayout.Foldout(showDisplayProperties, "Display Settings", true);
            if (showDisplayProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(displayOption, displayOptionContent);
                    EditorGUILayout.PropertyField(visibleMaterial);
                    EditorGUILayout.PropertyField(occlusionMaterial);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
