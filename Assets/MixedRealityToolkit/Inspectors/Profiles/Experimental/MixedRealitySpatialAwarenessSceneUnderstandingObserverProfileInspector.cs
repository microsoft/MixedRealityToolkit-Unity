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

        private SerializedProperty startupBehavior;
        private SerializedProperty updateInterval;

        // MixedRealitySpatialAwarenessSceneUnderstandingObserverProfile

        private SerializedProperty autoUpdate;
        private SerializedProperty physicsLayer;
        private SerializedProperty surfaceTypes;
        private SerializedProperty instantiationBatchRate;
        private SerializedProperty defaultMaterial;
        private SerializedProperty shouldLoadFromFile;
        private SerializedProperty serializedScene;
        private SerializedProperty generateMeshes;
        private SerializedProperty generatePlanes;
        private SerializedProperty generateEnvironmentMesh;
        private SerializedProperty renderInferredRegions;
        private SerializedProperty firstUpdateDelay;
        private SerializedProperty levelOfDetail;
        private SerializedProperty usePersistentObjects;
        private SerializedProperty queryRadius;
        private SerializedProperty visualizeOcclusionMask;
        private SerializedProperty occlusionMaskResolution;

        private const string ProfileTitle = "Scene Understanding Observer Settings";
        private const string ProfileDescription = "Settings for high-level environment representation";

        protected override void OnEnable()
        {
            base.OnEnable();

            startupBehavior = serializedObject.FindProperty("startupBehavior");
            autoUpdate = serializedObject.FindProperty("autoUpdate");
            updateInterval = serializedObject.FindProperty("updateInterval");

            firstUpdateDelay = serializedObject.FindProperty("firstUpdateDelay");

            shouldLoadFromFile = serializedObject.FindProperty("shouldLoadFromFile");
            serializedScene = serializedObject.FindProperty("serializedScene");

            levelOfDetail = serializedObject.FindProperty("levelOfDetail");
            usePersistentObjects = serializedObject.FindProperty("usePersistentObjects");

            instantiationBatchRate = serializedObject.FindProperty("instantiationBatchRate");
            defaultMaterial = serializedObject.FindProperty("defaultMaterial");
            generatePlanes = serializedObject.FindProperty("generatePlanes");
            generateMeshes = serializedObject.FindProperty("generateMeshes");
            generateEnvironmentMesh = serializedObject.FindProperty("generateEnvironmentMesh");

            physicsLayer = serializedObject.FindProperty("physicsLayer");
            surfaceTypes = serializedObject.FindProperty("surfaceTypes");
            renderInferredRegions = serializedObject.FindProperty("renderInferredRegions");
            queryRadius = serializedObject.FindProperty("queryRadius");
            visualizeOcclusionMask = serializedObject.FindProperty("visualizeOcclusionMask");
            occlusionMaskResolution = serializedObject.FindProperty("occlusionMaskResolution");
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
                    EditorGUILayout.PropertyField(startupBehavior);
                    EditorGUILayout.PropertyField(autoUpdate);
                    EditorGUILayout.PropertyField(updateInterval);
                    EditorGUILayout.PropertyField(firstUpdateDelay);
                }
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Storage", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(shouldLoadFromFile);
                    EditorGUILayout.PropertyField(serializedScene);
                }
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Observer", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(queryRadius);
                    EditorGUILayout.PropertyField(levelOfDetail);
                    EditorGUILayout.PropertyField(usePersistentObjects);
                    EditorGUILayout.PropertyField(renderInferredRegions);
                }
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Instantiation", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(surfaceTypes);
                    EditorGUILayout.PropertyField(instantiationBatchRate);
                    EditorGUILayout.PropertyField(generatePlanes);
                    EditorGUILayout.PropertyField(generateMeshes);
                    EditorGUILayout.PropertyField(generateEnvironmentMesh);
                    EditorGUILayout.PropertyField(physicsLayer);
                    EditorGUILayout.PropertyField(defaultMaterial);
                    EditorGUILayout.PropertyField(visualizeOcclusionMask);
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
