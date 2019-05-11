// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        private Func<bool>[] RenderProfileFuncs;

        // Experience properties
        private static bool showExperienceProperties = true;
        private SerializedProperty targetExperienceScale;
        // Camera properties
        private static bool showCameraProperties = true;
        private SerializedProperty enableCameraSystem;
        private SerializedProperty cameraSystemType;
        private SerializedProperty cameraProfile;
        // Input system properties
        private static bool showInputProperties = true;
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputSystemProfile;
        // Boundary system properties
        private static bool showBoundaryProperties = true;
        private SerializedProperty enableBoundarySystem;
        private SerializedProperty boundarySystemType;
        private SerializedProperty boundaryVisualizationProfile;
        // Teleport system properties
        private static bool showTeleportProperties = true;
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;
        // Spatial Awareness system properties
        private static bool showSpatialAwarenessProperties = true;
        private SerializedProperty enableSpatialAwarenessSystem;
        private SerializedProperty spatialAwarenessSystemType;
        private SerializedProperty spatialAwarenessSystemProfile;
        // Diagnostic system properties
        private static bool showDiagnosticProperties = true;
        private SerializedProperty enableDiagnosticsSystem;
        private SerializedProperty diagnosticsSystemType;
        private SerializedProperty diagnosticsSystemProfile;

        // Additional registered components profile
        private static bool showRegisteredServiceProperties = true;
        private SerializedProperty registeredServiceProvidersProfile;

        // Editor settings
        private static bool showEditorSettings = true;
        private SerializedProperty useServiceInspectors;

        private MixedRealityToolkitConfigurationProfile configurationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }

            // Experience configuration
            targetExperienceScale = serializedObject.FindProperty("targetExperienceScale");
            // Camera configuration
            enableCameraSystem = serializedObject.FindProperty("enableCameraSystem");
            cameraSystemType = serializedObject.FindProperty("cameraSystemType");
            cameraProfile = serializedObject.FindProperty("cameraProfile");
            // Input system configuration
            enableInputSystem = serializedObject.FindProperty("enableInputSystem");
            inputSystemType = serializedObject.FindProperty("inputSystemType");
            inputSystemProfile = serializedObject.FindProperty("inputSystemProfile");
            // Boundary system configuration
            enableBoundarySystem = serializedObject.FindProperty("enableBoundarySystem");
            boundarySystemType = serializedObject.FindProperty("boundarySystemType");
            boundaryVisualizationProfile = serializedObject.FindProperty("boundaryVisualizationProfile");
            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty("enableTeleportSystem");
            teleportSystemType = serializedObject.FindProperty("teleportSystemType");
            // Spatial Awareness system configuration
            enableSpatialAwarenessSystem = serializedObject.FindProperty("enableSpatialAwarenessSystem");
            spatialAwarenessSystemType = serializedObject.FindProperty("spatialAwarenessSystemType");
            spatialAwarenessSystemProfile = serializedObject.FindProperty("spatialAwarenessSystemProfile");
            // Diagnostics system configuration
            enableDiagnosticsSystem = serializedObject.FindProperty("enableDiagnosticsSystem");
            diagnosticsSystemType = serializedObject.FindProperty("diagnosticsSystemType");
            diagnosticsSystemProfile = serializedObject.FindProperty("diagnosticsSystemProfile");

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty("registeredServiceProvidersProfile");

            // Editor settings
            useServiceInspectors = serializedObject.FindProperty("useServiceInspectors");

            if (this.RenderProfileFuncs == null)
            {
                this.RenderProfileFuncs = new Func<bool>[]
                {
                    () => {
                        EditorGUILayout.PropertyField(enableCameraSystem);
                        EditorGUILayout.PropertyField(cameraSystemType);
                        return RenderProfile(cameraProfile);
                    },
                    () => {
                         EditorGUILayout.PropertyField(enableInputSystem);
                        EditorGUILayout.PropertyField(inputSystemType);
                        return RenderProfile(inputSystemProfile, true, true, typeof(IMixedRealityInputSystem));
                    },
                    () => {
                        var experienceScale = (ExperienceScale)targetExperienceScale.intValue;
                        if (experienceScale != ExperienceScale.Room)
                        {
                            // Alert the user if the experience scale does not support boundary features.
                            GUILayout.Space(6f);
                            EditorGUILayout.HelpBox("Boundaries are only supported in Room scale experiences.", MessageType.Warning);
                            GUILayout.Space(6f);
                        }
                        EditorGUILayout.PropertyField(enableBoundarySystem);
                        EditorGUILayout.PropertyField(boundarySystemType);
                        return RenderProfile(boundaryVisualizationProfile, true, true, typeof(IMixedRealityBoundarySystem));
                    },
                    () => {
                        EditorGUILayout.PropertyField(enableTeleportSystem);
                        EditorGUILayout.PropertyField(teleportSystemType);
                        return false;
                    },
                    () => {
                        EditorGUILayout.PropertyField(enableSpatialAwarenessSystem);
                        EditorGUILayout.PropertyField(spatialAwarenessSystemType);
                        EditorGUILayout.HelpBox("Spatial Awareness settings are configured per observer.", MessageType.Info);
                        return RenderProfile(spatialAwarenessSystemProfile, true, true, typeof(IMixedRealitySpatialAwarenessSystem));
                    },
                    () => {
                        EditorGUILayout.HelpBox("It is recommended to enable the Diagnostics system during development. Be sure to disable prior to building your shipping product.", MessageType.Warning);
                        EditorGUILayout.PropertyField(enableDiagnosticsSystem);
                        EditorGUILayout.PropertyField(diagnosticsSystemType);
                        return RenderProfile(diagnosticsSystemProfile);
                    },
                    () => {
                        return RenderProfile(registeredServiceProvidersProfile);
                    },
                    () => {
                        EditorGUILayout.PropertyField(useServiceInspectors);
                        return false;
                    },
                };
            }
        }

        private static string[] TabTitles = { "Camera", "Input", "Boundary", "Teleport", "Spatial Mapping", "Diagnostics", "Extensions", "Editor" };
        private static int SelectedTab = 0;

        public override void OnInspectorGUI()
        {
            var configurationProfile = (MixedRealityToolkitConfigurationProfile)target;

            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("Unable to find Mixed Reality Toolkit!", MessageType.Error);
                return;
            }

            if (!configurationProfile.IsCustomProfile)
            {
                EditorGUILayout.HelpBox("The Mixed Reality Toolkit's core SDK profiles can be used to get up and running quickly.\n\n" +
                                        "You can use the default profiles provided, copy and customize the default profiles, or create your own.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Copy & Customize"))
                {
                    CreateCopyProfileValues();
                }

                if (GUILayout.Button("Create new profiles"))
                {
                    ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                    var newProfile = profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles") as MixedRealityToolkitConfigurationProfile;
                    UnityEditor.Undo.RecordObject(MixedRealityToolkit.Instance, "Create new profiles");
                    MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                    Selection.activeObject = newProfile;
                }

                EditorGUILayout.EndHorizontal();
            }

            // We don't call the CheckLock method so won't get a duplicate message.
            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            //var previousLabelWidth = EditorGUIUtility.labelWidth;
            //EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Experience configuration
            ExperienceScale experienceScale = (ExperienceScale)targetExperienceScale.intValue;

            EditorGUILayout.PropertyField(targetExperienceScale, TargetScaleContent);
            string scaleDescription = string.Empty;

            switch (experienceScale)
            {
                case ExperienceScale.OrientationOnly:
                    scaleDescription = "The user is stationary. Position data does not change.";
                    break;

                case ExperienceScale.Seated:
                    scaleDescription = "The user is stationary and seated. The origin of the world is at a neutral head-level position.";
                    break;

                case ExperienceScale.Standing:
                    scaleDescription = "The user is stationary and standing. The origin of the world is on the floor, facing forward.";
                    break;

                case ExperienceScale.Room:
                    scaleDescription = "The user is free to move about the room. The origin of the world is on the floor, facing forward. Boundaries are available.";
                    break;

                case ExperienceScale.World:
                    scaleDescription = "The user is free to move about the world. Relies upon knowledge of the environment (Spatial Anchors and Spatial Mapping).";
                    break;
            }

            if (scaleDescription != string.Empty)
            {
                GUILayout.Space(6f);
                EditorGUILayout.HelpBox(scaleDescription, MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
                    SelectedTab = GUILayout.SelectionGrid(SelectedTab, TabTitles, 1, EditorStyles.boldLabel, GUILayout.MaxWidth(125));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                    using (new EditorGUI.IndentLevelScope())
                    {
                        changed |= RenderProfileFuncs[SelectedTab]();
                    }
                EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            //EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(configurationProfile);
            }
        }
    }
}
