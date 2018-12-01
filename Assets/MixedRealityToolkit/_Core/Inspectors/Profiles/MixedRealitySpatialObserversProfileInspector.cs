// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealitySpatialObserverDataProvidersProfile))]
    public class MixedRealitySpatialObserversProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent SpatialObserverAddButtonContent = new GUIContent("+ Add a New Spatial Observer");
        private static readonly GUIContent SpatialObserverMinusButtonContent = new GUIContent("-", "Remove Spatial Observer");

        private SerializedProperty registeredSpatialObserverDataProviders;

        private bool[] foldouts = null;

        protected override void OnEnable()
        {
            if (!CheckMixedRealityConfigured(false))
            {
                return;
            }

            registeredSpatialObserverDataProviders = serializedObject.FindProperty("registeredSpatialObserverDataProviders");
            foldouts = new bool[registeredSpatialObserverDataProviders.arraySize];
        }


        public override void OnInspectorGUI()
        {
            RenderMixedRealityToolkitLogo();
            if (!CheckMixedRealityConfigured())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.ActiveProfile.IsSpatialAwarenessSystemEnabled)
            {
                EditorGUILayout.HelpBox("No spatial awareness system is enabled, or you need to specify the type in the main configuration profile.", MessageType.Error);

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
            EditorGUILayout.LabelField("Registered Spatial Observer Data Providers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The platform specific spatial observers registered with the spatial awareness system.", MessageType.Info);
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

                spatialObserverType.FindPropertyRelative("reference").stringValue = string.Empty;
                spatialObserverName.stringValue = "New Spatial Observer Data Provider";
                priority.intValue = 5;
                runtimePlatform.intValue = 0;
                serializedObject.ApplyModifiedProperties();
                foldouts = new bool[registeredSpatialObserverDataProviders.arraySize];
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < registeredSpatialObserverDataProviders.arraySize; i++)
            {
                var spatialObserverConfiguration = registeredSpatialObserverDataProviders.GetArrayElementAtIndex(i);
                var spatialObserverType = spatialObserverConfiguration.FindPropertyRelative("spatialObserverType");
                var spatialObserverName = spatialObserverConfiguration.FindPropertyRelative("spatialObserverName");
                var priority = spatialObserverConfiguration.FindPropertyRelative("priority");
                var runtimePlatform = spatialObserverConfiguration.FindPropertyRelative("runtimePlatform");

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
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}