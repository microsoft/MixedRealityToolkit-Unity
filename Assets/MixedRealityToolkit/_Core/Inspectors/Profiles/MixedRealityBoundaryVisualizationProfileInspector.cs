// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityBoundaryVisualizationProfile))]
    public class MixedRealityBoundaryVisualizationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty boundaryHeight;
        private SerializedProperty floorPlaneScale;
        private SerializedProperty playAreaMaterial;
        private SerializedProperty floorPlaneMaterial;
        private SerializedProperty trackedAreaMaterial;

        private readonly GUIContent scaleContent = new GUIContent("Scale");
        private readonly GUIContent materialContent = new GUIContent("Material");

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            boundaryHeight = serializedObject.FindProperty("boundaryHeight");
            floorPlaneScale = serializedObject.FindProperty("floorPlaneScale");
            playAreaMaterial = serializedObject.FindProperty("playAreaMaterial");
            floorPlaneMaterial = serializedObject.FindProperty("floorPlaneMaterial");
            trackedAreaMaterial = serializedObject.FindProperty("trackedAreaMaterial");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Boundary Visualization Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary visualizations can help users stay oriented and comfortable in the experience.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.PropertyField(boundaryHeight);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Play Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playAreaMaterial, materialContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Tracked Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Floor Plane Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(floorPlaneMaterial, materialContent);
            EditorGUILayout.PropertyField(floorPlaneScale, scaleContent);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
