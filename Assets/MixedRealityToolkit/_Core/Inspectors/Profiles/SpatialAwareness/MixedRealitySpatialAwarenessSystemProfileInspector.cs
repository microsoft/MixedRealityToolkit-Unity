// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles.SpatialAwareness
{
    [CustomEditor(typeof(MixedRealitySpatialAwarenessSystemProfile))]
    public class MixedRealitySpatialAwarenessSystemProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent SpatialObserverAddButtonContent = new GUIContent("+ Add a New Spatial Observer");
        private static readonly GUIContent SpatialObserverMinusButtonContent = new GUIContent("-", "Remove Spatial Observer");

        private SerializedProperty registeredSpatialObserverDataProviders;

        private bool[] foldouts = null;

        /// <inheritdoc />
        protected override void OnEnable()
        {
            base.OnEnable();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured(false))
            {
                return;
            }

            registeredSpatialObserverDataProviders = serializedObject.FindProperty("registeredSpatialObserverDataProviders");
            foldouts = new bool[registeredSpatialObserverDataProviders.arraySize];
        }

        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return;
            }

            if (GUILayout.Button("Back to Configuration Profile"))
            {
                Selection.activeObject = MixedRealityToolkit.Instance.ActiveProfile;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Spatial Awareness System Settings", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Spatial Awareness can enhance your experience by enabling objects to interact with the real world.\n\nBelow is a list of registered Spatial Observers that can gather data about your environment.", MessageType.Info);
            EditorGUILayout.Space();
            serializedObject.Update();

            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(SpatialObserverAddButtonContent, EditorStyles.miniButton))
            {
                registeredSpatialObserverDataProviders.arraySize++;
                var spatialObserverConfiguration = registeredSpatialObserverDataProviders.GetArrayElementAtIndex(registeredSpatialObserverDataProviders.arraySize - 1);
                var spatialObserverType = spatialObserverConfiguration.FindPropertyRelative("spatialObserverType");
                var spatialObserverName = spatialObserverConfiguration.FindPropertyRelative("spatialObserverName");
                var priority = spatialObserverConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = spatialObserverConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = spatialObserverConfiguration.FindPropertyRelative("profile");

                spatialObserverType.FindPropertyRelative("reference").stringValue = string.Empty;
                spatialObserverName.stringValue = "New Spatial Observer Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                profile.objectReferenceValue = null;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[registeredSpatialObserverDataProviders.arraySize];
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < registeredSpatialObserverDataProviders?.arraySize; i++)
            {
                var spatialObserverConfiguration = registeredSpatialObserverDataProviders.GetArrayElementAtIndex(i);
                var spatialObserverType = spatialObserverConfiguration.FindPropertyRelative("spatialObserverType");
                var spatialObserverName = spatialObserverConfiguration.FindPropertyRelative("spatialObserverName");
                var priority = spatialObserverConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = spatialObserverConfiguration.FindPropertyRelative("runtimePlatform");
                var profile = spatialObserverConfiguration.FindPropertyRelative("profile");

                EditorGUILayout.BeginHorizontal();
                foldouts[i] = EditorGUILayout.Foldout(foldouts[i], spatialObserverName.stringValue, true);

                if (GUILayout.Button(SpatialObserverMinusButtonContent, EditorStyles.miniButtonRight, GUILayout.Width(24f)))
                {
                    registeredSpatialObserverDataProviders.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();
                    foldouts = new bool[registeredSpatialObserverDataProviders.arraySize];
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }

                EditorGUILayout.EndHorizontal();

                if (foldouts[i])
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(spatialObserverType);
                    EditorGUILayout.PropertyField(spatialObserverName);
                    EditorGUILayout.PropertyField(priority);
                    EditorGUILayout.PropertyField(runtimePlatform);
                    RenderProfile(profile, false);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
