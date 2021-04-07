// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Linq;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.WindowsSceneUnderstanding.Experimental.Editor
{
    [CustomEditor(typeof(SceneUnderstandingObserverProfile))]
    public class SceneUnderstandingObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty updateOnceInitialized;
        private SerializedProperty firstAutoUpdateDelay;
        private SerializedProperty autoUpdate;
        private SerializedProperty updateInterval;
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

            updateOnceInitialized = serializedObject.FindProperty("updateOnceInitialized");
            firstAutoUpdateDelay = serializedObject.FindProperty("firstAutoUpdateDelay");
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
            if (!RenderProfileHeader(ProfileTitle, ProfileDescription, target, true, BackProfileType.SpatialAwareness))
            {
                return;
            }

            using (new EditorGUI.DisabledGroupScope(IsProfileLock((BaseMixedRealityProfile)target)))
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("Life cycle", EditorStyles.boldLabel);
                {
                    EditorGUILayout.PropertyField(updateOnceInitialized);
                    EditorGUILayout.PropertyField(autoUpdate);
                    EditorGUILayout.PropertyField(updateInterval);
                    EditorGUILayout.PropertyField(firstAutoUpdateDelay);
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
