// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        // Experience properties
        private SerializedProperty targetExperienceScale;
        // Camera properties
        private SerializedProperty enableCameraSystem;
        private SerializedProperty cameraSystemType;
        private SerializedProperty cameraProfile;
        // Input system properties
        private SerializedProperty enableInputSystem;
        private SerializedProperty inputSystemType;
        private SerializedProperty inputSystemProfile;
        // Boundary system properties
        private SerializedProperty enableBoundarySystem;
        private SerializedProperty boundarySystemType;
        private SerializedProperty boundaryVisualizationProfile;
        // Teleport system properties
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;
        // Spatial Awareness system properties
        private SerializedProperty enableSpatialAwarenessSystem;
        private SerializedProperty spatialAwarenessSystemType;
        private SerializedProperty spatialAwarenessSystemProfile;
        // Diagnostic system properties
        private SerializedProperty enableDiagnosticsSystem;
        private SerializedProperty diagnosticsSystemType;
        private SerializedProperty diagnosticsSystemProfile;

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        // Editor settings
        private SerializedProperty useServiceInspectors;

        private MixedRealityToolkitConfigurationProfile configurationProfile;
        private Func<bool>[] RenderProfileFuncs;

        private static string[] ProfileTabTitles = { "Camera", "Input", "Boundary", "Teleport", "Spatial Mapping", "Diagnostics", "Extensions", "Editor" };
        private static int SelectedProfileTab = 0;

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
                        return RenderProfile(cameraProfile, typeof(MixedRealityCameraProfile), true, false);
                    },
                    () => {
                         EditorGUILayout.PropertyField(enableInputSystem);
                        EditorGUILayout.PropertyField(inputSystemType);
                        return RenderProfile(inputSystemProfile, null, true, false, typeof(IMixedRealityInputSystem));
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
                        return RenderProfile(boundaryVisualizationProfile, null, true, false, typeof(IMixedRealityBoundarySystem));
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
                        return RenderProfile(spatialAwarenessSystemProfile, null, true, false, typeof(IMixedRealitySpatialAwarenessSystem));
                    },
                    () => {
                        EditorGUILayout.HelpBox("It is recommended to enable the Diagnostics system during development. Be sure to disable prior to building your shipping product.", MessageType.Warning);
                        EditorGUILayout.PropertyField(enableDiagnosticsSystem);
                        EditorGUILayout.PropertyField(diagnosticsSystemType);
                        return RenderProfile(diagnosticsSystemProfile, typeof(MixedRealityDiagnosticsProfile));
                    },
                    () => {
                        return RenderProfile(registeredServiceProvidersProfile, typeof(MixedRealityRegisteredServiceProvidersProfile), true, false);
                    },
                    () => {
                        EditorGUILayout.PropertyField(useServiceInspectors);
                        return false;
                    },
                };
            }
        }

        public override void OnInspectorGUI()
        {
            var configurationProfile = (MixedRealityToolkitConfigurationProfile)target;
            serializedObject.Update();

            RenderMRTKLogo();

            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("No Mixed Reality Toolkit found in scene.", MessageType.Warning);
                if (MixedRealityEditorUtility.RenderIndentedButton("Add Mixed Reality Toolkit instance to scene"))
                {
                    MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(configurationProfile);
                }
            }

            if (!configurationProfile.IsCustomProfile)
            {
                EditorGUILayout.HelpBox("The Mixed Reality Toolkit's core SDK profiles can be used to get up and running quickly.\n\n" +
                                        "You can use the default profiles provided, copy and customize the default profiles, or create your own.", MessageType.Warning);
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Copy & Customize"))
                {
                    var originalSelection = Selection.activeObject;
                    CreateCustomProfile(target as BaseMixedRealityProfile);
                    Selection.activeObject = originalSelection;
                }

                if (MixedRealityToolkit.IsInitialized)
                {
                    if (GUILayout.Button("Create new profiles"))
                    {
                        ScriptableObject profile = CreateInstance(nameof(MixedRealityToolkitConfigurationProfile));
                        var newProfile = profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles") as MixedRealityToolkitConfigurationProfile;
                        UnityEditor.Undo.RecordObject(MixedRealityToolkit.Instance, "Create new profiles");
                        MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                        Selection.activeObject = newProfile;
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            }

            bool isGUIEnabled = !IsProfileLock((BaseMixedRealityProfile)target);
            GUI.enabled = isGUIEnabled;

            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Experience configuration
            ExperienceScale experienceScale = (ExperienceScale)targetExperienceScale.intValue;
            EditorGUILayout.PropertyField(targetExperienceScale, TargetScaleContent);

            string scaleDescription = GetExperienceDescription(experienceScale);
            if (!string.IsNullOrEmpty(scaleDescription))
            {
                EditorGUILayout.HelpBox(scaleDescription, MessageType.Info);
                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            GUI.enabled = true; // Force enable so we can view profile defaults
            SelectedProfileTab = GUILayout.SelectionGrid(SelectedProfileTab, ProfileTabTitles, 1, EditorStyles.boldLabel, GUILayout.MaxWidth(125));
            GUI.enabled = isGUIEnabled;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            using (new EditorGUI.IndentLevelScope())
            {
                changed |= RenderProfileFuncs[SelectedProfileTab]();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            serializedObject.ApplyModifiedProperties();
            GUI.enabled = true;

            if (changed && MixedRealityToolkit.IsInitialized)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(configurationProfile);
            }
        }

        protected override bool IsProfileInActiveInstance()
        {
            var profile = target as BaseMixedRealityProfile;
            return MixedRealityToolkit.IsInitialized && profile != null &&
                   profile == MixedRealityToolkit.Instance.ActiveProfile;
        }

        private static string GetExperienceDescription(ExperienceScale experienceScale)
        {
            switch (experienceScale)
            {
                case ExperienceScale.OrientationOnly:
                    return "The user is stationary. Position data does not change.";
                case ExperienceScale.Seated:
                    return "The user is stationary and seated. The origin of the world is at a neutral head-level position.";
                case ExperienceScale.Standing:
                    return "The user is stationary and standing. The origin of the world is on the floor, facing forward.";
                case ExperienceScale.Room:
                    return "The user is free to move about the room. The origin of the world is on the floor, facing forward. Boundaries are available.";
                case ExperienceScale.World:
                    return "The user is free to move about the world. Relies upon knowledge of the environment (Spatial Anchors and Spatial Mapping).";
            }

            return null;
        }
    }
}