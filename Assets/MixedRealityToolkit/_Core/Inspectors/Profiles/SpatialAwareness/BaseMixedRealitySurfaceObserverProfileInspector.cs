// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySurfaceObserverProfile))]
    public abstract class BaseMixedRealitySurfaceObserverProfileInspector : BaseMixedRealitySpatialObserverProfileInspector
    {
        private SerializedProperty surfacePhysicsLayerOverride;
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
        private readonly GUIContent platformMaterialContent = new GUIContent("Platform Material");
        private readonly GUIContent wallMaterialContent = new GUIContent("Wall Material");
        private readonly GUIContent minimumAreaContent = new GUIContent("Minimum Area");

        private bool foldout = true;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            surfacePhysicsLayerOverride = serializedObject.FindProperty("surfacePhysicsLayerOverride");
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

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            serializedObject.Update();

            foldout = EditorGUILayout.Foldout(foldout, "Surface Finding Settings", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(surfacePhysicsLayerOverride);
                EditorGUILayout.PropertyField(surfaceFindingMinimumArea, minimumAreaContent);
                EditorGUILayout.PropertyField(displayFloorSurfaces);
                EditorGUILayout.PropertyField(floorSurfaceMaterial, floorMaterialContent);
                EditorGUILayout.PropertyField(displayCeilingSurfaces);
                EditorGUILayout.PropertyField(ceilingSurfaceMaterial, ceilingMaterialContent);
                EditorGUILayout.PropertyField(displayWallSurfaces);
                EditorGUILayout.PropertyField(wallSurfaceMaterial, wallMaterialContent);
                EditorGUILayout.PropertyField(displayPlatformSurfaces);
                EditorGUILayout.PropertyField(platformSurfaceMaterial, platformMaterialContent);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}