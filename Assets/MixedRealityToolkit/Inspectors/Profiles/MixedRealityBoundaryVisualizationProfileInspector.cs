// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Editor;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Boundary.Editor
{
    [CustomEditor(typeof(MixedRealityBoundaryVisualizationProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static bool showGeneralProperties = true;
        private SerializedProperty boundaryHeight;

        private static bool showFloorProperties = true;
        private SerializedProperty showFloor;
        private SerializedProperty floorMaterial;
        private SerializedProperty floorScale;
        private SerializedProperty floorPhysicsLayer;

        private static bool showPlayAreaProperties = true;
        private SerializedProperty showPlayArea;
        private SerializedProperty playAreaMaterial;
        private SerializedProperty playAreaPhysicsLayer;

        private static bool showTrackedAreaProperties = true;
        private SerializedProperty showTrackedArea;
        private SerializedProperty trackedAreaMaterial;
        private SerializedProperty trackedAreaPhysicsLayer;

        private static bool showWallProperties = true;
        private SerializedProperty showBoundaryWalls;
        private SerializedProperty boundaryWallMaterial;
        private SerializedProperty boundaryWallsPhysicsLayer;

        private static bool showCeilingProperties = true;
        private SerializedProperty showBoundaryCeiling;
        private SerializedProperty boundaryCeilingMaterial;
        private SerializedProperty ceilingPhysicsLayer;

        private readonly GUIContent showContent = new GUIContent("Show");
        private readonly GUIContent scaleContent = new GUIContent("Scale");
        private readonly GUIContent materialContent = new GUIContent("Material");

        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            boundaryHeight = serializedObject.FindProperty("boundaryHeight");

            showFloor = serializedObject.FindProperty("showFloor");
            floorMaterial = serializedObject.FindProperty("floorMaterial");
            floorScale = serializedObject.FindProperty("floorScale");
            floorPhysicsLayer = serializedObject.FindProperty("floorPhysicsLayer");

            showPlayArea = serializedObject.FindProperty("showPlayArea");
            playAreaMaterial = serializedObject.FindProperty("playAreaMaterial");
            playAreaPhysicsLayer = serializedObject.FindProperty("playAreaPhysicsLayer");

            showTrackedArea = serializedObject.FindProperty("showTrackedArea");
            trackedAreaMaterial = serializedObject.FindProperty("trackedAreaMaterial");
            trackedAreaPhysicsLayer = serializedObject.FindProperty("trackedAreaPhysicsLayer");

            showBoundaryWalls = serializedObject.FindProperty("showBoundaryWalls");
            boundaryWallMaterial = serializedObject.FindProperty("boundaryWallMaterial");
            boundaryWallsPhysicsLayer = serializedObject.FindProperty("boundaryWallsPhysicsLayer");

            showBoundaryCeiling = serializedObject.FindProperty("showBoundaryCeiling");
            boundaryCeilingMaterial = serializedObject.FindProperty("boundaryCeilingMaterial");
            ceilingPhysicsLayer = serializedObject.FindProperty("ceilingPhysicsLayer");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (DrawBacktrackProfileButton("Back to Configuration Profile", MixedRealityToolkit.Instance.ActiveProfile))
            {
                return;
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

            EditorGUILayout.Space();
            showGeneralProperties = EditorGUILayout.Foldout(showGeneralProperties, "General Settings", true);
            if (showGeneralProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(boundaryHeight);
                }
            }

            EditorGUILayout.Space();
            showFloorProperties = EditorGUILayout.Foldout(showFloorProperties, "Floor Settings", true);
            if (showFloorProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showFloor, showContent);
                    EditorGUILayout.PropertyField(floorMaterial, materialContent);
                    var prevWideMode = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = true;
                    EditorGUILayout.PropertyField(floorScale, scaleContent, GUILayout.ExpandWidth(true));
                    EditorGUIUtility.wideMode = prevWideMode;
                    EditorGUILayout.PropertyField(floorPhysicsLayer);
                }
            }

            EditorGUILayout.Space();
            showPlayAreaProperties = EditorGUILayout.Foldout(showPlayAreaProperties, "Play Area Settings", true);
            if (showPlayAreaProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showPlayArea, showContent);
                    EditorGUILayout.PropertyField(playAreaMaterial, materialContent);
                    EditorGUILayout.PropertyField(playAreaPhysicsLayer);
                }
            }

            EditorGUILayout.Space();
            showTrackedAreaProperties = EditorGUILayout.Foldout(showTrackedAreaProperties, "Tracked Area Settings", true);
            if (showTrackedAreaProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showTrackedArea, showContent);
                    EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);
                    EditorGUILayout.PropertyField(trackedAreaPhysicsLayer);
                }
            }

            EditorGUILayout.Space();
            showWallProperties = EditorGUILayout.Foldout(showWallProperties, "Boundary Wall Settings", true);
            if (showWallProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showBoundaryWalls, showContent);
                    EditorGUILayout.PropertyField(boundaryWallMaterial, materialContent);
                    EditorGUILayout.PropertyField(boundaryWallsPhysicsLayer);
                }
            }

            EditorGUILayout.Space();
            showCeilingProperties = EditorGUILayout.Foldout(showCeilingProperties, "Boundary Ceiling Settings", true);
            if (showCeilingProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(showBoundaryCeiling, showContent);
                    EditorGUILayout.PropertyField(boundaryCeilingMaterial, materialContent);
                    EditorGUILayout.PropertyField(ceilingPhysicsLayer);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
