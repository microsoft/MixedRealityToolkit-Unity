// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityBoundaryVisualizationProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
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

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

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
            EditorGUILayout.LabelField("Boundary Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary visualizations can help users stay oriented and comfortable in the experience.", MessageType.Info);
            EditorGUILayout.Space();

            if (!CheckMixedRealityManager())
            {
                return;
            }

            serializedObject.Update();

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Floor Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showFloor, showContent);
            EditorGUILayout.PropertyField(floorMaterial, materialContent);
            EditorGUILayout.PropertyField(floorScale, scaleContent);

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
