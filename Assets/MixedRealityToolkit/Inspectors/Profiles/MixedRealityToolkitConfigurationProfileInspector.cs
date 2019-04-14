// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        const string HideNoActiveToolkitWarningKey = "MRTK_HideNoActiveToolkitWarningKey";
        private static bool HideNoActiveToolkitWarning = true;

        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        // Experience properties
        private static bool showExperienceProperties = true;
        private SerializedProperty targetExperienceScale;
        // Camera properties
        private static bool showCameraProperties = true;
        private SerializedProperty enableCameraProfile;
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

        private MixedRealityToolkitConfigurationProfile configurationProfile;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }

            configurationProfile = target as MixedRealityToolkitConfigurationProfile;

            // Create The MR Manager if none exists.
            if (!MixedRealityToolkit.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    HideNoActiveToolkitWarning = SessionState.GetBool(HideNoActiveToolkitWarningKey, false);
                    if (!HideNoActiveToolkitWarning)
                    {
                        NoActiveToolkitWarning.OpenWindow(configurationProfile);
                    }
                    return; 
                }
            }

            if (!MixedRealityToolkit.ConfirmInitialized())
            {
                return;
            }

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
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
            spatialAwarenessSystemProfile = serializedObject.FindProperty("spatialAwarenessSystemProfile");
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

            var previousLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 160f;
            EditorGUI.BeginChangeCheck();
            bool changed = false;

            // Experience configuration
            EditorGUILayout.Space();
            ExperienceScale experienceScale = (ExperienceScale)targetExperienceScale.intValue;
            showExperienceProperties = EditorGUILayout.Foldout(showExperienceProperties, "Experience Settings", true);
            if (showExperienceProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
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
                }
            }

            // Camera Profile configuration
            EditorGUILayout.Space();
            showCameraProperties = EditorGUILayout.Foldout(showCameraProperties, "Camera Settings", true);
            if (showCameraProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableCameraProfile);
                    changed |= RenderProfile(cameraProfile);
                }
            }

            // Input System configuration
            EditorGUILayout.Space();
            showInputProperties = EditorGUILayout.Foldout(showInputProperties, "Input System Settings", true);
            if (showInputProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableInputSystem);
                    EditorGUILayout.PropertyField(inputSystemType);
                    changed |= RenderProfile(inputSystemProfile);
                }
            }

            // Boundary System configuration
            EditorGUILayout.Space();
            showBoundaryProperties = EditorGUILayout.Foldout(showBoundaryProperties, "Boundary System Settings", true);
            if (showBoundaryProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    if (experienceScale != ExperienceScale.Room)
                    {
                        // Alert the user if the experience scale does not support boundary features.
                        GUILayout.Space(6f);
                        EditorGUILayout.HelpBox("Boundaries are only supported in Room scale experiences.", MessageType.Warning);
                        GUILayout.Space(6f);
                    }
                    EditorGUILayout.PropertyField(enableBoundarySystem);
                    EditorGUILayout.PropertyField(boundarySystemType);
                    changed |= RenderProfile(boundaryVisualizationProfile);
                }
            }

            // Teleport System configuration
            EditorGUILayout.Space();
            showTeleportProperties = EditorGUILayout.Foldout(showTeleportProperties, "Teleport System Settings", true);
            if (showTeleportProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableTeleportSystem);
                    EditorGUILayout.PropertyField(teleportSystemType);
                }
            }

            // Spatial Awareness System configuration
            EditorGUILayout.Space();
            showSpatialAwarenessProperties = EditorGUILayout.Foldout(showSpatialAwarenessProperties, "Spatial Awareness System Settings", true);
            if (showSpatialAwarenessProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(enableSpatialAwarenessSystem);
                    EditorGUILayout.PropertyField(spatialAwarenessSystemType);
                    EditorGUILayout.HelpBox("Spatial Awareness settings are configured per observer.", MessageType.Info);
                    changed |= RenderProfile(spatialAwarenessSystemProfile);
                }
            }

            // Diagnostics System configuration
            EditorGUILayout.Space();
            showDiagnosticProperties = EditorGUILayout.Foldout(showDiagnosticProperties, "Diagnostics System Settings", true);
            if (showDiagnosticProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.HelpBox("It is recommended to enable the Diagnostics system during development. Be sure to disable prior to building your shipping product.", MessageType.Warning);
                    EditorGUILayout.PropertyField(enableDiagnosticsSystem);
                    EditorGUILayout.PropertyField(diagnosticsSystemType);
                    changed |= RenderProfile(diagnosticsSystemProfile);
                }
            }

            // Registered Services configuration
            EditorGUILayout.Space();
            showRegisteredServiceProperties = EditorGUILayout.Foldout(showRegisteredServiceProperties, "Extension Services", true);
            if (showRegisteredServiceProperties)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    changed |= RenderProfile(registeredServiceProvidersProfile);
                }
            }

            if (!changed)
            {
                changed |= EditorGUI.EndChangeCheck();
            }

            EditorGUIUtility.labelWidth = previousLabelWidth;
            serializedObject.ApplyModifiedProperties();

            if (changed)
            {
                EditorApplication.delayCall += () => MixedRealityToolkit.Instance.ResetConfiguration(configurationProfile);
            }
        }

        private class NoActiveToolkitWarning : EditorWindow
        {
            private static NoActiveToolkitWarning activeWindow;
            private MixedRealityToolkitConfigurationProfile configurationProfile;
            private bool hideWarning = false;

            public static void OpenWindow(MixedRealityToolkitConfigurationProfile configurationProfile)
            {
                // If we already have an active window, bail
                if (activeWindow != null)
                    return;

                activeWindow = EditorWindow.GetWindow<NoActiveToolkitWarning>();
                activeWindow.configurationProfile = configurationProfile;
                activeWindow.maxSize = new Vector2(400, 80);
                activeWindow.minSize = new Vector2(400, 80);
                activeWindow.titleContent = new GUIContent("No Active Toolkit Found");

                activeWindow.Show(true); 
            }

            private void OnGUI()
            {
                EditorGUILayout.HelpBox("There is no active Mixed Reality Toolkit in your scene. Would you like to create one now?", MessageType.Warning);

                hideWarning = EditorGUILayout.Toggle("Don't show this again", hideWarning);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Yes"))
                {
                    var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
                    Debug.Assert(playspace != null);
                    MixedRealityToolkit.Instance.ActiveProfile = configurationProfile;

                    SessionState.SetBool(HideNoActiveToolkitWarningKey, hideWarning);
                    Close();
                }

                if (GUILayout.Button("No"))
                {
                    SessionState.SetBool(HideNoActiveToolkitWarningKey, hideWarning);
                    Close();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}