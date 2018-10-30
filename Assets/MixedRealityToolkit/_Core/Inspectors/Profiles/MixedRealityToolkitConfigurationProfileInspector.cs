// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Extensions.EditorClassExtensions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : MixedRealityBaseConfigurationProfileInspector
    {
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        // Experience properties
        private SerializedProperty targetExperienceScale;
        // Camera properties
        private SerializedProperty enableCameraProfile;
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
        private SerializedProperty spatialAwarenessProfile;
        // Diagnostic system properties
        private SerializedProperty enableDiagnosticsSystem;
        private SerializedProperty diagnosticsSystemType;
        private SerializedProperty diagnosticsSystemProfile;

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private MixedRealityToolkitConfigurationProfile configurationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            configurationProfile = target as MixedRealityToolkitConfigurationProfile;

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (EditorUtility.DisplayDialog(
                        "Attention!",
                        "There is no active Mixed Reality Toolkit in your scene!\n\nWould you like to create one now?",
                        "Yes",
                        "Later"))
                    {
                        var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
                        Debug.Assert(playspace != null);
                        MixedRealityToolkit.Instance.ActiveProfile = configurationProfile;
                    }
                    else
                    {
                        Debug.LogWarning("No Mixed Reality Toolkit in your scene.");
                        return;
                    }
                }
            }

            if (!MixedRealityToolkit.ConfirmInitialized())
            {
                return;
            }

            if (!MixedRealityToolkit.HasActiveProfile)
            {
                return;
            }

            // Experience configuration
            targetExperienceScale = serializedObject.FindProperty("targetExperienceScale");
            // Camera configuration
            enableCameraProfile = serializedObject.FindProperty("enableCameraProfile");
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
            spatialAwarenessProfile = serializedObject.FindProperty("spatialAwarenessProfile");
            // Diagnostics system configuration
            enableDiagnosticsSystem = serializedObject.FindProperty("enableDiagnosticsSystem");
            diagnosticsSystemType = serializedObject.FindProperty("diagnosticsSystemType");
            diagnosticsSystemProfile = serializedObject.FindProperty("diagnosticsSystemProfile");

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty("registeredServiceProvidersProfile");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            RenderMixedRealityToolkitLogo();

            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("Unable to find Mixed Reality Toolkit!", MessageType.Error);
                return;
            }

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
                var newProfile = profile.CreateAsset("Assets/MixedRealityToolkit-Generated/CustomProfiles") as MixedRealityToolkitConfigurationProfile;
                MixedRealityToolkit.Instance.ActiveProfile = newProfile;
                Selection.activeObject = newProfile;
            }

            EditorGUILayout.EndHorizontal();

            // We don't call the CheckLock method so won't get a duplicate message.
            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                GUI.enabled = false;
            }

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Experience configuration
            EditorGUILayout.LabelField("Experience Settings", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(targetExperienceScale, TargetScaleContent);
            ExperienceScale scale = (ExperienceScale)targetExperienceScale.intValue;
            string scaleDescription = string.Empty;

            switch (scale)
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

            // Camera Profile configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Camera Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableCameraProfile);

            changed |= RenderProfile(cameraProfile);

            // Input System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Input System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableInputSystem);
            EditorGUILayout.PropertyField(inputSystemType);
            changed |= RenderProfile(inputSystemProfile);

            // Boundary System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Boundary System Settings", EditorStyles.boldLabel);
            if (scale != ExperienceScale.Room)
            {
                // Alert the user if the experience scale does not support boundary features.
                GUILayout.Space(6f);
                EditorGUILayout.HelpBox("Boundaries are only supported in Room scale experiences.", MessageType.Warning);
                GUILayout.Space(6f);
            }
            EditorGUILayout.PropertyField(enableBoundarySystem);
            EditorGUILayout.PropertyField(boundarySystemType);
            changed |= RenderProfile(boundaryVisualizationProfile);

            // Teleport System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Teleport System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableTeleportSystem);
            EditorGUILayout.PropertyField(teleportSystemType);

            // Spatial Awareness System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Spatial Awareness System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableSpatialAwarenessSystem);
            EditorGUILayout.PropertyField(spatialAwarenessSystemType);
            changed |= RenderProfile(spatialAwarenessProfile);

            // Diagnostics System configuration
            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Diagnostics System Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableDiagnosticsSystem);
            EditorGUILayout.PropertyField(diagnosticsSystemType);
            changed |= RenderProfile(diagnosticsSystemProfile);

            GUILayout.Space(12f);
            EditorGUILayout.LabelField("Additional Service Providers", EditorStyles.boldLabel);
            changed |= RenderProfile(registeredServiceProvidersProfile);

            if (!changed)
            {
                changed = EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(configurationProfile);
            }
        }
    }
}