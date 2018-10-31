// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.BoundarySystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityBoundaryVisualizationProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty boundaryHeight;
        private SerializedProperty showFloor;
        private SerializedProperty floorMaterial;
        private SerializedProperty floorScale;

        private SerializedProperty showPlayArea;
        private SerializedProperty playAreaMaterial;

        private SerializedProperty showTrackedArea;
        private SerializedProperty trackedAreaMaterial;

        private SerializedProperty showBoundaryWalls;
        private SerializedProperty boundaryWallMaterial;

        private SerializedProperty showBoundaryCeiling;
        private SerializedProperty boundaryCeilingMaterial;

        private readonly GUIContent showContent = new GUIContent("Show");
        private readonly GUIContent scaleContent = new GUIContent("Scale");
        private readonly GUIContent materialContent = new GUIContent("Material");

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            boundaryHeight = serializedObject.FindProperty("boundaryHeight");

            showFloor = serializedObject.FindProperty("showFloor");
            floorMaterial = serializedObject.FindProperty("floorMaterial");
            floorScale = serializedObject.FindProperty("floorScale");

            showPlayArea = serializedObject.FindProperty("showPlayArea");
            playAreaMaterial = serializedObject.FindProperty("playAreaMaterial");

            showTrackedArea = serializedObject.FindProperty("showTrackedArea");
            trackedAreaMaterial = serializedObject.FindProperty("trackedAreaMaterial");

            showBoundaryWalls = serializedObject.FindProperty("showBoundaryWalls");
            boundaryWallMaterial = serializedObject.FindProperty("boundaryWallMaterial");

            showBoundaryCeiling = serializedObject.FindProperty("showBoundaryCeiling");
            boundaryCeilingMaterial = serializedObject.FindProperty("boundaryCeilingMaterial");
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
            EditorGUILayout.LabelField("Boundary Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary visualizations can help users stay oriented and comfortable in the experience.", MessageType.Info);
            // Boundary settings depend on the experience scale
            if (MixedRealityToolkit.Instance.ActiveProfile.TargetExperienceScale != ExperienceScale.Room)
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("Boundary visualization is only supported in Room scale experiences.", MessageType.Warning);
            }
            EditorGUILayout.Space();

            CheckProfileLock(target);

            serializedObject.Update();
            EditorGUILayout.PropertyField(boundaryHeight);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Floor Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showFloor, showContent);
            EditorGUILayout.PropertyField(floorMaterial, materialContent);
            var prevWideMode = EditorGUIUtility.wideMode;
            EditorGUIUtility.wideMode = true;
            EditorGUILayout.PropertyField(floorScale, scaleContent, GUILayout.ExpandWidth(true));
            EditorGUIUtility.wideMode = prevWideMode;

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Play Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showPlayArea, showContent);
            EditorGUILayout.PropertyField(playAreaMaterial, materialContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Tracked Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showTrackedArea, showContent);
            EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary Wall Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showBoundaryWalls, showContent);
            EditorGUILayout.PropertyField(boundaryWallMaterial, materialContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary Ceiling Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showBoundaryCeiling, showContent);
            EditorGUILayout.PropertyField(boundaryCeilingMaterial, materialContent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
