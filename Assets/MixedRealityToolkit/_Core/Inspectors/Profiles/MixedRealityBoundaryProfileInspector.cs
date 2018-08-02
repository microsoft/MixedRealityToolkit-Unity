// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    [CustomEditor(typeof(MixedRealityBoundaryProfile))]
    public class MixedRealityBoundaryProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private SerializedProperty showPlayArea;
        private SerializedProperty playAreaMaterial;
        private readonly GUIContent showPlayAreaTitle = new GUIContent("Show Play Area Bounds");
        private readonly GUIContent playAreaMaterialTitle = new GUIContent("Play Area Material");

        private SerializedProperty showFloorPlane;
        private SerializedProperty floorPlaneMaterial;
        private SerializedProperty floorPlaneScale;
        private readonly GUIContent showFloorPlaneTitle = new GUIContent("Show Floor Plane");
        private readonly GUIContent floorPlaneMaterialTitle = new GUIContent("Floor Plane Material");
        private readonly GUIContent floorPlaneScaleTitle = new GUIContent("Floor Plane Scale");

        private void OnEnable()
        {
            if (!CheckMixedRealityManager(false))
            {
                return;
            }

            // Find the play area properties.
            showPlayArea = serializedObject.FindProperty("showPlayArea");
            playAreaMaterial = serializedObject.FindProperty("playAreaMaterial");

            // Find the floor plane properties.
            showFloorPlane = serializedObject.FindProperty("showFloorPlane");
            floorPlaneMaterial = serializedObject.FindProperty("floorPlaneMaterial");
            floorPlaneScale = serializedObject.FindProperty("floorPlaneScale");
        }

        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            EditorGUILayout.LabelField("Boundary Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Boundary displays can help users stay oriented and comfortable in the experience.", MessageType.Info);
            EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Play Area Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showPlayArea, showPlayAreaTitle);
            if (showPlayArea.boolValue)
            {
                EditorGUILayout.PropertyField(playAreaMaterial, playAreaMaterialTitle);
            }

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Floor Plane Settings:", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(showFloorPlane, showFloorPlaneTitle);
            if (showFloorPlane.boolValue)
            {
                EditorGUILayout.PropertyField(floorPlaneMaterial, floorPlaneMaterialTitle);
                EditorGUILayout.PropertyField(floorPlaneScale, floorPlaneScaleTitle);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
