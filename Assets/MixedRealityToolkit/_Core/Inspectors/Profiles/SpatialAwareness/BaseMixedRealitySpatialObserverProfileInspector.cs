// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers;
using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(BaseMixedRealitySpatialObserverProfile))]
    public abstract class BaseMixedRealitySpatialObserverProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private SerializedProperty startupBehavior;
        private SerializedProperty observationExtents;
        private SerializedProperty isStationaryObserver;
        private SerializedProperty updateInterval;
        private SerializedProperty physicsLayer;

        private bool foldout = true;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            startupBehavior = serializedObject.FindProperty("startupBehavior");
            observationExtents = serializedObject.FindProperty("observationExtents");
            isStationaryObserver = serializedObject.FindProperty("isStationaryObserver");
            updateInterval = serializedObject.FindProperty("updateInterval");
            physicsLayer = serializedObject.FindProperty("physicsLayer");
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled)
            {
                EditorGUILayout.HelpBox("The Spatial Awareness Observer Data Provider requires that the spatial awareness system be enabled.", MessageType.Error);

                if (GUILayout.Button("Back to Configuration Profile"))
                {
                    Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
                }

                return;
            }

            if (GUILayout.Button("Back to Spatial Awareness Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile.SpatialAwarenessProfile;
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Observer Options", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The Spatial Awareness Observer Data Provider supplies the Spatial Awareness system with all the data it needs to understand the world around you.", MessageType.Info);
            EditorGUILayout.Space();

            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            serializedObject.Update();

            foldout = EditorGUILayout.Foldout(foldout, "General Settings", true);

            if (foldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(startupBehavior);
                EditorGUILayout.PropertyField(observationExtents);
                EditorGUILayout.PropertyField(isStationaryObserver);
                EditorGUILayout.PropertyField(updateInterval);
                EditorGUILayout.PropertyField(physicsLayer);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}