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
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    [CustomEditor(typeof(MixedRealityToolkitConfigurationProfile))]
    public class MixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityToolkitConfigurationProfileInspector
    {
        private static readonly GUIContent TargetScaleContent = new GUIContent("Target Scale:");

        // Experience properties
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

        // Editor settings
        private SerializedProperty useServiceInspectors;
        private SerializedProperty renderDepthBuffer;

        private Func<bool>[] renderProfileFuncs;

        private static readonly string[] ProfileTabTitles = {
            "Experience Settings",
            "Camera",
            "Input",
            "Boundary",
            "Teleport",
            "Spatial Awareness",
            "Diagnostics",
            "Scene System",
            "Extensions",
            "Editor",
        };

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
            enableDiagnosticsSystem = serializedObject.FindProperty("enableDiagnosticsSystem");
            enableVerboseLogging = serializedObject.FindProperty("enableVerboseLogging");
            diagnosticsSystemType = serializedObject.FindProperty("diagnosticsSystemType");
            diagnosticsSystemProfile = serializedObject.FindProperty("diagnosticsSystemProfile");
            // Scene system configuration
            enableSceneSystem = serializedObject.FindProperty("enableSceneSystem");
            sceneSystemType = serializedObject.FindProperty("sceneSystemType");
            sceneSystemProfile = serializedObject.FindProperty("sceneSystemProfile");

            // Additional registered components configuration
            registeredServiceProvidersProfile = serializedObject.FindProperty("registeredServiceProvidersProfile");

            // Editor settings
            useServiceInspectors = serializedObject.FindProperty("useServiceInspectors");
            renderDepthBuffer = serializedObject.FindProperty("renderDepthBuffer");

            SelectedProfileTab = SessionState.GetInt(SelectedTabPreferenceKey, SelectedProfileTab);

            if (renderProfileFuncs == null)
            {
                renderProfileFuncs = new Func<bool>[]
                {
                    () => {
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            // Reconciling old Experience Scale property with the Experience Settings Profile
                            var oldExperienceSettigsScale = (experienceSettingsProfile.objectReferenceValue as MixedRealityExperienceSettingsProfile)?.TargetExperienceScale;

                            changed |= RenderProfile(experienceSettingsProfile, typeof(MixedRealityExperienceSettingsProfile), true, false,  null);

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
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableCameraSystem);

                            const string service = "Camera System";
                            if (enableCameraSystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.CameraSystemType, mrtkConfigProfile.CameraProfile != null);

                                EditorGUILayout.PropertyField(cameraSystemType);

                                changed |= RenderProfile(cameraProfile, typeof(MixedRealityCameraProfile), true, false);
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableInputSystem);

                            const string service = "Input System";
                            if (enableInputSystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.InputSystemType, mrtkConfigProfile.InputSystemProfile != null);

                                EditorGUILayout.PropertyField(inputSystemType);

                                changed |= RenderProfile(inputSystemProfile, null, true, false, typeof(IMixedRealityInputSystem));
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        if(mrtkConfigProfile.ExperienceSettingsProfile.IsNull())
                        {
                            // Alert that an experience settings profile has not been selected
                            GUILayout.Space(6f);
                            EditorGUILayout.HelpBox("Boundaries require an experience settings profile with a Room scale target experience scale.", MessageType.Warning);
                            GUILayout.Space(6f);
                        }
                        else
                        {
                            var experienceScale = mrtkConfigProfile.ExperienceSettingsProfile.TargetExperienceScale;
                            if (experienceScale != ExperienceScale.Room)
                            {
                                // Alert the user if the experience scale does not support boundary features.
                                GUILayout.Space(6f);
                                EditorGUILayout.HelpBox("Boundaries are only supported in Room scale experiences.", MessageType.Warning);
                                GUILayout.Space(6f);
                            }
                        }

                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableBoundarySystem);

                            const string service = "Boundary System";
                            if (enableBoundarySystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.BoundarySystemSystemType, mrtkConfigProfile.BoundaryVisualizationProfile != null);

#if UNITY_2019
                                xrPipelineUtility.RenderXRPipelineTabs();
#endif // UNITY_2019

                                EditorGUILayout.PropertyField(xrPipelineUtility.SelectedPipeline == SupportedUnityXRPipelines.XRSDK ? xrsdkBoundarySystemType : boundarySystemType);

                                changed |= RenderProfile(boundaryVisualizationProfile, null, true, false, typeof(IMixedRealityBoundarySystem));
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        const string service = "Teleport System";
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableTeleportSystem);
                            if (enableTeleportSystem.boolValue)
                            {
                                 // Teleport System does not have a profile scriptableobject so auto to true
                                CheckSystemConfiguration(service, mrtkConfigProfile.TeleportSystemSystemType,true);

                                EditorGUILayout.PropertyField(teleportSystemType);
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            return c.changed;
                        }
                    },
                    () => {
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            const string service = "Spatial Awareness System";
                            EditorGUILayout.PropertyField(enableSpatialAwarenessSystem);

                            if (enableSpatialAwarenessSystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.SpatialAwarenessSystemSystemType, mrtkConfigProfile.SpatialAwarenessSystemProfile != null);

                                EditorGUILayout.PropertyField(spatialAwarenessSystemType);

                                EditorGUILayout.HelpBox("Spatial Awareness settings are configured per observer.", MessageType.Info);

                                changed |= RenderProfile(spatialAwarenessSystemProfile, null, true, false, typeof(IMixedRealitySpatialAwarenessSystem));
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        EditorGUILayout.HelpBox("It is recommended to enable the Diagnostics system during development. Be sure to disable prior to building your shipping product.", MessageType.Warning);

                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableVerboseLogging);
                            EditorGUILayout.PropertyField(enableDiagnosticsSystem);

                            const string service = "Diagnostics System";
                            if (enableDiagnosticsSystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.DiagnosticsSystemSystemType, mrtkConfigProfile.DiagnosticsSystemProfile != null);

                                EditorGUILayout.PropertyField(diagnosticsSystemType);

                                changed |= RenderProfile(diagnosticsSystemProfile, typeof(MixedRealityDiagnosticsProfile));
                            }
                            else
                            {
                                RenderSystemDisabled(service);
                            }

                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        bool changed = false;
                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(enableSceneSystem);
                            const string service = "Scene System";
                            if (enableSceneSystem.boolValue)
                            {
                                CheckSystemConfiguration(service, mrtkConfigProfile.SceneSystemSystemType, mrtkConfigProfile.SceneSystemProfile != null);

                                EditorGUILayout.PropertyField(sceneSystemType);

                                changed |= RenderProfile(sceneSystemProfile, typeof(MixedRealitySceneSystemProfile), true, true, typeof(IMixedRealitySceneSystem));
                            }
                            changed |= c.changed;
                        }
                        return changed;
                    },
                    () => {
                        return RenderProfile(registeredServiceProvidersProfile, typeof(MixedRealityRegisteredServiceProvidersProfile), true, false);
                    },
                    () => {
                        EditorGUILayout.PropertyField(useServiceInspectors);

                        using (var c = new EditorGUI.ChangeCheckScope())
                        {
                            EditorGUILayout.PropertyField(renderDepthBuffer);
                            if (c.changed)
                            {
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
                            }
                        }
                        return false;
                    },
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

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            GUI.enabled = true; // Force enable so we can view profile defaults

            int prefsSelectedTab = SessionState.GetInt(SelectedTabPreferenceKey, 0);
            SelectedProfileTab = GUILayout.SelectionGrid(prefsSelectedTab, ProfileTabTitles, 1, GUILayout.MaxWidth(125));
            if (SelectedProfileTab != prefsSelectedTab)
            {
                SessionState.SetInt(SelectedTabPreferenceKey, SelectedProfileTab);
            }

            GUI.enabled = isGUIEnabled;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            using (new EditorGUI.IndentLevelScope())
            {
                changed |= renderProfileFuncs[SelectedProfileTab]();
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
