// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialMeshObserverProfile))]
    public abstract class BaseMixedRealitySpatialMeshObserverProfileInspector : BaseMixedRealitySpatialObserverProfileInspector
    {
        private SerializedProperty meshPhysicsLayerOverride;
        private SerializedProperty meshLevelOfDetail;
        private SerializedProperty meshTrianglesPerCubicMeter;
        private SerializedProperty meshRecalculateNormals;
        private SerializedProperty meshDisplayOption;
        private SerializedProperty meshVisibleMaterial;
        private SerializedProperty meshOcclusionMaterial;

        private static bool foldout = true;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            meshPhysicsLayerOverride = serializedObject.FindProperty("meshPhysicsLayerOverride");
            meshLevelOfDetail = serializedObject.FindProperty("meshLevelOfDetail");
            meshTrianglesPerCubicMeter = serializedObject.FindProperty("meshTrianglesPerCubicMeter");
            meshRecalculateNormals = serializedObject.FindProperty("meshRecalculateNormals");
            meshDisplayOption = serializedObject.FindProperty("meshDisplayOption");
            meshVisibleMaterial = serializedObject.FindProperty("meshVisibleMaterial");
            meshOcclusionMaterial = serializedObject.FindProperty("meshOcclusionMaterial");
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

            foldout = EditorGUILayout.Foldout(foldout, "Spatial Mesh Settings", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(meshPhysicsLayerOverride);
                EditorGUILayout.PropertyField(meshLevelOfDetail);
                EditorGUILayout.PropertyField(meshTrianglesPerCubicMeter);
                EditorGUILayout.PropertyField(meshRecalculateNormals);
                EditorGUILayout.PropertyField(meshDisplayOption);
                EditorGUILayout.PropertyField(meshVisibleMaterial);
                EditorGUILayout.PropertyField(meshOcclusionMaterial);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}