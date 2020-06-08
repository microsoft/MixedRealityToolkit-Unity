// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Editor.SpatialAwareness
{
    [CustomEditor(typeof(Experimental.SpatialAwareness.MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile))]
    public class MixedRealitySpatialAwarenessSceneUnderstandingObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        // BaseSpatialAwarenessObserverProfile

        private SerializedProperty updateOnceOnLoad;
        private SerializedProperty updateInterval;

        // MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile

        private SerializedProperty autoUpdate;
        private SerializedProperty defaultPhysicsLayer;
        private SerializedProperty surfaceTypes;
        private SerializedProperty instantiationBatchRate;
        private SerializedProperty defaultMaterial;
        private SerializedProperty defaultWorldMeshMaterial;
        private SerializedProperty shouldLoadFromFile;
        private SerializedProperty serializedScene;
        private SerializedProperty requestMeshData;
        private SerializedProperty requestPlaneData;
        private SerializedProperty createGameObjects;
        private SerializedProperty inferRegions;
        private SerializedProperty firstUpdateDelay;
        private SerializedProperty worldMeshLevelOfDetail;
        private SerializedProperty usePersistentObjects;
        private SerializedProperty queryRadius;
        private SerializedProperty requestOcclusionMask;
        private SerializedProperty occlusionMaskResolution;
        private SerializedProperty orientScene;

        private const string ProfileTitle = "Scene Understanding Observer Settings";
        private const string ProfileDescription = "Settings for high-level environment representation";

        protected override void OnEnable()
        {
            base.OnEnable();

            updateOnceOnLoad = serializedObject.FindProperty("updateOnceOnLoad");
            firstUpdateDelay = serializedObject.FindProperty("firstUpdateDelay");
            autoUpdate = serializedObject.FindProperty("autoUpdate");
            updateInterval = serializedObject.FindProperty("updateInterval");

            shouldLoadFromFile = serializedObject.FindProperty("shouldLoadFromFile");
            serializedScene = serializedObject.FindProperty("serializedScene");

            worldMeshLevelOfDetail = serializedObject.FindProperty("worldMeshLevelOfDetail");
            usePersistentObjects = serializedObject.FindProperty("usePersistentObjects");

            instantiationBatchRate = serializedObject.FindProperty("instantiationBatchRate");
            defaultMaterial = serializedObject.FindProperty("defaultMaterial");
            defaultWorldMeshMaterial = serializedObject.FindProperty("defaultWorldMeshMaterial");
            requestPlaneData = serializedObject.FindProperty("requestPlaneData");
            requestMeshData = serializedObject.FindProperty("requestMeshData");
            createGameObjects = serializedObject.FindProperty("createGameObjects");

            defaultPhysicsLayer = serializedObject.FindProperty("defaultPhysicsLayer");
            surfaceTypes = serializedObject.FindProperty("surfaceTypes");
            inferRegions = serializedObject.FindProperty("inferRegions");
            queryRadius = serializedObject.FindProperty("queryRadius");
            requestOcclusionMask = serializedObject.FindProperty("requestOcclusionMask");
            occlusionMaskResolution = serializedObject.FindProperty("occlusionMaskResolution");
            orientScene = serializedObject.FindProperty("orientScene");
        }

        public override void OnInspectorGUI()
        {
            RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.SpatialAwareness);

            //using (new GUIEnabledWrapper(!IsProfileLock((BaseMixedRealityProfile)target)))
            using (new GUIEnabledWrapper())
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("Life cycle", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(updateOnceOnLoad);
                    EditorGUILayout.PropertyField(autoUpdate);
                    EditorGUILayout.PropertyField(updateInterval);
                    EditorGUILayout.PropertyField(firstUpdateDelay);
                }
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Observer", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(surfaceTypes);
                    EditorGUILayout.PropertyField(queryRadius);
                    EditorGUILayout.PropertyField(worldMeshLevelOfDetail);
                    EditorGUILayout.PropertyField(usePersistentObjects);
                    EditorGUILayout.PropertyField(inferRegions);
                    EditorGUILayout.PropertyField(requestPlaneData);
                    EditorGUILayout.PropertyField(requestMeshData);
                    EditorGUILayout.PropertyField(requestOcclusionMask);
                }
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Observer Debugging", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(shouldLoadFromFile);
                    EditorGUILayout.PropertyField(serializedScene);
                    EditorGUILayout.PropertyField(orientScene);
                    EditorGUILayout.PropertyField(createGameObjects);
                    EditorGUILayout.PropertyField(instantiationBatchRate);
                    EditorGUILayout.PropertyField(defaultPhysicsLayer);
                    EditorGUILayout.PropertyField(defaultMaterial);
                    EditorGUILayout.PropertyField(defaultWorldMeshMaterial);
                    EditorGUILayout.PropertyField(occlusionMaskResolution);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;

            return MixedRealityToolkit.IsInitialized && profile != null &&
                   MixedRealityToolkit.Instance.HasActiveProfile &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile.ObserverConfigurations != null &&
                   MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessSystemProfile.ObserverConfigurations.Any(s => s.ObserverProfile == profile);
        }
    }
}
