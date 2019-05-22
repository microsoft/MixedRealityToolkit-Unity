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
        private SerializedProperty boundaryHeight;

        private SerializedProperty showFloor;
        private SerializedProperty floorMaterial;
        private SerializedProperty floorScale;
        private SerializedProperty floorPhysicsLayer;

        private SerializedProperty showPlayArea;
        private SerializedProperty playAreaMaterial;
        private SerializedProperty playAreaPhysicsLayer;

        private SerializedProperty showTrackedArea;
        private SerializedProperty trackedAreaMaterial;
        private SerializedProperty trackedAreaPhysicsLayer;

        private SerializedProperty showBoundaryWalls;
        private SerializedProperty boundaryWallMaterial;
        private SerializedProperty boundaryWallsPhysicsLayer;

        private SerializedProperty showBoundaryCeiling;
        private SerializedProperty boundaryCeilingMaterial;
        private SerializedProperty ceilingPhysicsLayer;

        private const string ProfileTitle = "Boundary Visualization Settings";
        private const string ProfileDescription = "Boundary visualizations can help users stay oriented and comfortable in the experience.";

        private readonly GUIContent showContent = new GUIContent("Show");
        private readonly GUIContent scaleContent = new GUIContent("Scale");
        private readonly GUIContent materialContent = new GUIContent("Material");

        protected override void OnEnable()
        {
            base.OnEnable();

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
            RenderProfileHeader(ProfileTitle, ProfileDescription, target);

            using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(boundaryHeight);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Floor Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showFloor, showContent);
                    EditorGUILayout.PropertyField(floorMaterial, materialContent);
                    var prevWideMode = EditorGUIUtility.wideMode;
                    EditorGUIUtility.wideMode = true;
                    EditorGUILayout.PropertyField(floorScale, scaleContent, GUILayout.ExpandWidth(true));
                    EditorGUIUtility.wideMode = prevWideMode;
                    EditorGUILayout.PropertyField(floorPhysicsLayer);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Play Area Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showPlayArea, showContent);
                    EditorGUILayout.PropertyField(playAreaMaterial, materialContent);
                    EditorGUILayout.PropertyField(playAreaPhysicsLayer);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Tracked Area Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showTrackedArea, showContent);
                    EditorGUILayout.PropertyField(trackedAreaMaterial, materialContent);
                    EditorGUILayout.PropertyField(trackedAreaPhysicsLayer);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Boundary Wall Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showBoundaryWalls, showContent);
                    EditorGUILayout.PropertyField(boundaryWallMaterial, materialContent);
                    EditorGUILayout.PropertyField(boundaryWallsPhysicsLayer);
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Boundary Ceiling Settings", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(showBoundaryCeiling, showContent);
                    EditorGUILayout.PropertyField(boundaryCeilingMaterial, materialContent);
                    EditorGUILayout.PropertyField(ceilingPhysicsLayer);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile.BoundaryVisualizationProfile;
        }
    }
}
