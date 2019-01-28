// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
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
            // TODO: needed? if so, updates are required
            //EditorGUILayout.HelpBox("Spatial Awareness can enhance your experience by enabling objects to interact with the real world.", MessageType.Info);
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
                    EditorGUILayout.PropertyField(observationExtents);
                    EditorGUILayout.PropertyField(isStationaryObserver);
                    EditorGUILayout.PropertyField(updateInterval);
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
                    EditorGUILayout.HelpBox("The value of Triangls per Cubic Meter is ignored unless Level of Detail is set to Custom.", MessageType.Info);
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
