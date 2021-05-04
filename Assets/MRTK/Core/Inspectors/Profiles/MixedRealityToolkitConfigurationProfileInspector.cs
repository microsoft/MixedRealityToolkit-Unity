// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.﻿

using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.Diagnostics;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Rendering;
using Microsoft.MixedReality.Toolkit.SceneSystem;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public enum SubsystemProfile
    {
        ExperienceSettings,
        Camera,
        Input,
        Boundary,
        Teleport,
        SpatialAwareness,
        Diagnostics,
        SceneSystem,
        Extensions,
    }

    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        // Experience properties
        private SerializedProperty experienceSettingsType;
        private SerializedProperty experienceSettingsProfile;

        // Tracking the old experience scale property for compatibility
        private SerializedProperty experienceScaleMigration;

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
        private SerializedProperty xrsdkBoundarySystemType;
        private SerializedProperty boundaryVisualizationProfile;
        // Teleport system properties
        private SerializedProperty enableTeleportSystem;
        private SerializedProperty teleportSystemType;
        // Spatial Awareness system properties
        private SerializedProperty enableSpatialAwarenessSystem;
        private SerializedProperty spatialAwarenessSystemType;
        private SerializedProperty spatialAwarenessSystemProfile;
        // Diagnostic system properties
        private SerializedProperty renderDepthBuffer;
        private SerializedProperty enableDiagnosticsSystem;
        private SerializedProperty enableVerboseLogging;
        private SerializedProperty diagnosticsSystemType;
        private SerializedProperty diagnosticsSystemProfile;
        // Scene system properties
        private SerializedProperty enableSceneSystem;
        private SerializedProperty sceneSystemType;
        private SerializedProperty sceneSystemProfile;

        // Additional registered components profile
        private SerializedProperty registeredServiceProvidersProfile;

        private Func<bool>[] renderProfileFuncs;

        private List<SubsystemProfile> enabledSubsystems;

        private static int SelectedProfileTab = 0;
        private const string SelectedTabPreferenceKey = "SelectedProfileTab";

        private readonly XRPipelineUtility xrPipelineUtility = new XRPipelineUtility();

        protected override void OnEnable()
        {
            base.OnEnable();

            if (target == null)
            {
                // Either when we are recompiling, or the inspector window is hidden behind another one, the target can get destroyed (null) and thereby will raise an ArgumentException when accessing serializedObject. For now, just return.
                return;
            }

            MixedRealityToolkitConfigurationProfile mrtkConfigProfile = target as MixedRealityToolkitConfigurationProfile;

            // Experience configuration
            experienceSettingsType = serializedObject.FindProperty("experienceSettingsType");
            experienceSettingsProfile = serializedObject.FindProperty("experienceSettingsProfile");
            experienceScaleMigration = serializedObject.FindProperty("targetExperienceScale");

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
            xrsdkBoundarySystemType = serializedObject.FindProperty("xrsdkBoundarySystemType");
            boundaryVisualizationProfile = serializedObject.FindProperty("boundaryVisualizationProfile");
#if UNITY_2019
            xrPipelineUtility.Enable();
#endif // UNITY_2019

            // Teleport system configuration
            enableTeleportSystem = serializedObject.FindProperty("enableTeleportSystem");
            teleportSystemType = serializedObject.FindProperty("teleportSystemType");
            // Spatial Awareness system configuration
            enableSpatialAwarenessSystem = serializedObject.FindProperty("enableSpatialAwarenessSystem");
            spatialAwarenessSystemType = serializedObject.FindProperty("spatialAwarenessSystemType");
            spatialAwarenessSystemProfile = serializedObject.FindProperty("spatialAwarenessSystemProfile");
            // Diagnostics system configuration
            renderDepthBuffer = serializedObject.FindProperty("renderDepthBuffer");
            enableVerboseLogging = serializedObject.FindProperty("enableVerboseLogging");
            enableDiagnosticsSystem = serializedObject.FindProperty("enableDiagnosticsSystem");
            diagnosticsSystemType = serializedObject.FindProperty("diagnosticsSystemType");
            diagnosticsSystemProfile = serializedObject.FindProperty("diagnosticsSystemProfile");
            // Scene system configuration
            enableSceneSystem = serializedObject.FindProperty("enableSceneSystem");
            sceneSystemType = serializedObject.FindProperty("sceneSystemType");
            sceneSystemProfile = serializedObject.FindProperty("sceneSystemProfile");

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty("registeredServiceProvidersProfile");

            // Getting the names of all active subsystems
            RefreshEnabledSubsystemList();

            SelectedProfileTab = SessionState.GetInt(SelectedTabPreferenceKey, SelectedProfileTab);

            if (renderProfileFuncs == null)
            {
                renderProfileFuncs = new Func<bool>[]
                {
                    () => {
                        // Experience Settings
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            // Reconciling old Experience Scale property with the Experience Settings Profile
                            var oldExperienceSettigsScale = (experienceSettingsProfile.objectReferenceValue as MixedRealityExperienceSettingsProfile)?.TargetExperienceScale;

                            changed |= RenderProfile(experienceSettingsProfile, typeof(MixedRealityExperienceSettingsProfile), true, false,  null, true);

                            // Experience configuration
                            if(!mrtkConfigProfile.ExperienceSettingsProfile.IsNull())
                            {                            
                                // If the Experience Scale property changed, make sure we also alter the configuration profile's target experience scale property for compatibility
                                var newExperienceSettigs = (experienceSettingsProfile.objectReferenceValue as MixedRealityExperienceSettingsProfile)?.TargetExperienceScale;
                                if(oldExperienceSettigsScale.HasValue && newExperienceSettigs.HasValue && oldExperienceSettigsScale != newExperienceSettigs)
                                {
                                    experienceScaleMigration.intValue = (int)newExperienceSettigs;
                                    experienceScaleMigration.serializedObject.ApplyModifiedProperties();
                                }
                                // If we have not changed the Experience Settings profile and it's value is out of sync with the top level configuration profile, display a migration prompt
                                else if ((ExperienceScale)experienceScaleMigration.intValue != mrtkConfigProfile.ExperienceSettingsProfile.TargetExperienceScale)
                                {
                                    Color errorColor = Color.Lerp(Color.white, Color.red, 0.5f);
                                    Color defaultColor = GUI.color;

                                    GUI.color = errorColor;
                                    EditorGUILayout.HelpBox("A previous version of this profile has a different Experience Scale, displayed below. Please modify the Experience Setting Profile's Target Experience Scale or select your desired scale below", MessageType.Warning);
                                    var oldValue = experienceScaleMigration.intValue;
                                    EditorGUILayout.PropertyField(experienceScaleMigration);
                                    if (oldValue != experienceScaleMigration.intValue)
                                    {
                                        mrtkConfigProfile.ExperienceSettingsProfile.TargetExperienceScale = (ExperienceScale)experienceScaleMigration.intValue;
                                    }
                                    GUI.color = defaultColor;
                                }


                                ExperienceScale experienceScale = mrtkConfigProfile.ExperienceSettingsProfile.TargetExperienceScale;
                                string targetExperienceSummary = GetExperienceDescription(experienceScale);
                                if (!string.IsNullOrEmpty(targetExperienceSummary))
                                {
                                    EditorGUILayout.HelpBox(targetExperienceSummary, MessageType.None);
                                    EditorGUILayout.Space();
                                }
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Camera System
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Camera System";
                            changed |= RenderSubsystem(service, SubsystemProfile.Camera, cameraSystemType, cameraProfile, null, typeof(MixedRealityCameraProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Input System
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Input System";
                            changed |= RenderSubsystem(service, SubsystemProfile.Input, inputSystemType, inputSystemProfile, mrtkConfigProfile.InputSystemType, typeof(MixedRealityInputSystemProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Boundary System
                        var experienceScale = mrtkConfigProfile.ExperienceSettingsProfile.TargetExperienceScale;
                        if (experienceScale != ExperienceScale.Room)
                        {
                            GUILayout.Space(6f);
                            EditorGUILayout.HelpBox("Boundaries are only supported in Room scale experiences.", MessageType.Warning);
                            GUILayout.Space(6f);
                        }

                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Boundary System";
#if UNITY_2019
                            xrPipelineUtility.RenderXRPipelineTabs();
#endif // UNITY_2019
                            var selectedBoundarySystemType = xrPipelineUtility.SelectedPipeline == SupportedUnityXRPipelines.XRSDK ? xrsdkBoundarySystemType : boundarySystemType;
                            changed |= RenderSubsystem(service, SubsystemProfile.Boundary, selectedBoundarySystemType, boundaryVisualizationProfile, mrtkConfigProfile.BoundarySystemSystemType, typeof(MixedRealityBoundaryVisualizationProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Teleport System
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Teleport System";
                            changed |= RenderSubsystem(service, SubsystemProfile.Teleport, teleportSystemType, null, null, null);
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Spatial Awareness System
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Spatial Awareness System";
                            EditorGUILayout.HelpBox("Spatial Awareness settings are configured per observer.", MessageType.Info);
                            changed |= RenderSubsystem(service, SubsystemProfile.SpatialAwareness, spatialAwarenessSystemType, spatialAwarenessSystemProfile, null, typeof(MixedRealitySpatialAwarenessSystemProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Diagnostic System
                        EditorGUILayout.HelpBox("It is recommended to enable the Diagnostics system during development. Be sure to disable prior to building your shipping product.", MessageType.Warning);

                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(renderDepthBuffer);
                            if (renderDepthBuffer.boolValue)
                            {
                                CameraCache.Main.gameObject.AddComponent<DepthBufferRenderer>();
                            }
                            else
                            {
                                foreach (var dbr in FindObjectsOfType<DepthBufferRenderer>())
                                {
                                    UnityObjectExtensions.DestroyObject(dbr);
                                }
                            }
                            EditorGUILayout.PropertyField(enableVerboseLogging);

                            const string service = "Diagnostics System";
                            changed |= RenderSubsystem(service, SubsystemProfile.Diagnostics, diagnosticsSystemType, diagnosticsSystemProfile, null, typeof(MixedRealityDiagnosticsProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        // Scene System
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Scene System";
                            changed |= RenderSubsystem(service, SubsystemProfile.SceneSystem, sceneSystemType, sceneSystemProfile, null, typeof(MixedRealitySceneSystemProfile));
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        return RenderProfile(registeredServiceProvidersProfile, typeof(MixedRealityRegisteredServiceProvidersProfile), true, false);
                    }
                };
            }
        }

        public override void OnInspectorGUI()
        {
            var configurationProfile = (MixedRealityToolkitConfigurationProfile)target;
            serializedObject.Update();

            if (!RenderMRTKLogoAndSearch())
            {
                CheckEditorPlayMode();
                return;
            }

            CheckEditorPlayMode();

            if (!MixedRealityToolkit.IsInitialized)
            {
                EditorGUILayout.HelpBox("No Mixed Reality Toolkit found in scene.", MessageType.Warning);
                if (InspectorUIUtility.RenderIndentedButton("Add Mixed Reality Toolkit instance to scene"))
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
                    SerializedProperty targetProperty = null;
                    UnityEngine.Object selectionTarget = null;
                    // If we have an active MRTK instance, find its config profile serialized property
                    if (MixedRealityToolkit.IsInitialized)
                    {
                        selectionTarget = MixedRealityToolkit.Instance;
                        SerializedObject mixedRealityToolkitObject = new SerializedObject(MixedRealityToolkit.Instance);
                        targetProperty = mixedRealityToolkitObject.FindProperty("activeProfile");
                    }
                    MixedRealityProfileCloneWindow.OpenWindow(null, target as BaseMixedRealityProfile, targetProperty, selectionTarget);
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

            bool isGUIEnabled = !IsProfileLock((BaseMixedRealityProfile)target) && GUI.enabled;
            GUI.enabled = isGUIEnabled;

            bool changed = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            GUI.enabled = true; // Force enable so we can view profile defaults

            int prefsSelectedTab = SessionState.GetInt(SelectedTabPreferenceKey, 0);
            string[] subsystemNames = enabledSubsystems.Select(x => ObjectNames.NicifyVariableName(x.ToString())).ToArray();
            SelectedProfileTab = GUILayout.SelectionGrid(prefsSelectedTab, subsystemNames, 1, GUILayout.MaxWidth(125));
            if (SelectedProfileTab != prefsSelectedTab)
            {
                SessionState.SetInt(SelectedTabPreferenceKey, SelectedProfileTab);
            }

            GUI.enabled = isGUIEnabled;
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Add MRTK System"))
            {
                MixedRealitySubsystemManagementWindow.OpenWindow(serializedObject, this);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            using (new EditorGUI.IndentLevelScope())
            {
                int selectedSubsystemIndex = (int)enabledSubsystems[SelectedProfileTab];
                changed |= renderProfileFuncs[selectedSubsystemIndex]();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

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

        /// <summary>
        /// Checks if a system is enabled and the service type or validProfile is null, then displays warning message to the user
        /// </summary>
        /// <param name="service">name of service being tested</param>
        /// <param name="systemType">Selected implementation type for service</param>
        /// <param name="validProfile">true if profile scriptableobject property is not null, false otherwise</param>
        protected void CheckSystemConfiguration(string service, SystemType systemType, bool validProfile)
        {
            if (systemType?.Type == null || !validProfile)
            {
                EditorGUILayout.HelpBox($"{service} is enabled but will not be initialized because the System Type and/or Profile is not set.", MessageType.Warning);
            }
        }

        /// <summary>
        /// Render helpbox that provided service string is disabled and none of its functionality will be loaded at runtime
        /// </summary>
        protected static void RenderSystemDisabled(string service)
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox($"The {service} is disabled.\n\nThis module will not be loaded and thus none of its features will be available at runtime.", MessageType.Info);
            EditorGUILayout.Space();
        }

        public bool RenderSubsystem(string service,  SubsystemProfile subsystem, SerializedProperty subsystemTypeProperty, SerializedProperty subsystemProfile, System.Type subsystemType = null, System.Type profileType = null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(subsystemTypeProperty);

            // display the GenericMenu when pressing a button
            if (GUILayout.Button(EditorGUIUtility.IconContent("_Menu"), EditorStyles.miniButtonRight, GUILayout.Width(24f)))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove System"), false, () => RemoveSubsystem(subsystem));

                // display the menu
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            if (subsystemProfile != null)
            {
                CheckSystemConfiguration(service, subsystemType, subsystemProfile.objectReferenceValue != null);
                return RenderProfile(subsystemProfile, profileType, true, false, subsystemType);
            }
            else
            {
                return true;
            }
        }

        public void RefreshEnabledSubsystemList()
        {
            enabledSubsystems = GetEnabledSubsystemList();
        }

        private List<SubsystemProfile> GetEnabledSubsystemList()
        {
            List<SubsystemProfile> subsystemList = new List<SubsystemProfile>();
            subsystemList.Add(SubsystemProfile.ExperienceSettings);
            if (enableCameraSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.Camera);
            }
            if (enableInputSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.Input);
            }
            if (enableBoundarySystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.Boundary);
            }
            if (enableTeleportSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.Teleport);
            }
            if (enableSpatialAwarenessSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.SpatialAwareness);
            }
            if (enableDiagnosticsSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.Diagnostics);
            }
            if (enableSceneSystem.boolValue)
            {
                subsystemList.Add(SubsystemProfile.SceneSystem);
            }
            subsystemList.Add(SubsystemProfile.Extensions);

            return subsystemList;
        }

        public void AddSubsystem(SubsystemProfile subprofileType)
        {
            switch (subprofileType)
            {
                case SubsystemProfile.Camera:
                    enableCameraSystem.boolValue = true;
                    break;
                case SubsystemProfile.Input:
                    enableInputSystem.boolValue = true;
                    break;
                case SubsystemProfile.Boundary:
                    enableBoundarySystem.boolValue = true;
                    break;
                case SubsystemProfile.Teleport:
                    enableTeleportSystem.boolValue = true;
                    break;
                case SubsystemProfile.SpatialAwareness:
                    enableSpatialAwarenessSystem.boolValue = true;
                    break;
                case SubsystemProfile.Diagnostics:
                    enableDiagnosticsSystem.boolValue = true;
                    break;
                case SubsystemProfile.SceneSystem:
                    enableSceneSystem.boolValue = true;
                    break;
                case SubsystemProfile.Extensions:
                    break;
            }
            serializedObject.ApplyModifiedProperties();
            RefreshEnabledSubsystemList();
            Repaint();
        }

        public void RemoveSubsystem(SubsystemProfile subprofileType)
        {
            switch (subprofileType)
            {
                case SubsystemProfile.Camera:
                    enableCameraSystem.boolValue = false;
                    break;
                case SubsystemProfile.Input:
                    enableInputSystem.boolValue = false;
                    break;
                case SubsystemProfile.Boundary:
                    enableBoundarySystem.boolValue = false;
                    break;
                case SubsystemProfile.Teleport:
                    enableTeleportSystem.boolValue = false;
                    break;
                case SubsystemProfile.SpatialAwareness:
                    enableSpatialAwarenessSystem.boolValue = false;
                    break;
                case SubsystemProfile.Diagnostics:
                    enableDiagnosticsSystem.boolValue = false;
                    break;
                case SubsystemProfile.SceneSystem:
                    enableSceneSystem.boolValue = false;
                    break;
                case SubsystemProfile.Extensions:
                    break;
            }
            serializedObject.ApplyModifiedProperties();
            RefreshEnabledSubsystemList();
            Repaint();
        }

        public bool GetSubsystemStatus(SubsystemProfile subprofileType)
        {
            switch (subprofileType)
            {
                case SubsystemProfile.Camera:
                    return enableCameraSystem.boolValue;
                case SubsystemProfile.Input:
                    return enableInputSystem.boolValue;
                case SubsystemProfile.Boundary:
                    return enableBoundarySystem.boolValue;
                case SubsystemProfile.Teleport:
                    return enableTeleportSystem.boolValue;
                case SubsystemProfile.SpatialAwareness:
                    return enableSpatialAwarenessSystem.boolValue;
                case SubsystemProfile.Diagnostics:
                    return enableDiagnosticsSystem.boolValue;
                case SubsystemProfile.SceneSystem:
                    return enableSceneSystem.boolValue;
                case SubsystemProfile.Extensions:
                    break;
            }
            return false;
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