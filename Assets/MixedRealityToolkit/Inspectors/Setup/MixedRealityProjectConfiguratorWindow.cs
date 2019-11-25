// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

using MRConfig = Microsoft.MixedReality.Toolkit.Utilities.Editor.MixedRealityProjectConfigurator.Configurations;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public class MixedRealityProjectConfiguratorWindow : EditorWindow
    {
        private readonly Dictionary<MRConfig, bool> trackToggles = new Dictionary<MRConfig, bool>()
        {
            {MRConfig.ForceTextSerialization, true },
            {MRConfig.VisibleMetaFiles, true },
            {MRConfig.VirtualRealitySupported, true },
            {MRConfig.SinglePassInstancing, true },
            {MRConfig.SpatialAwarenessLayer, true },
            {MRConfig.EnableMSBuildForUnity, true },
            // UWP Capabilities
            {MRConfig.MicrophoneCapability, true },
            {MRConfig.InternetClientCapability, true },
            {MRConfig.SpatialPerceptionCapability, true },
#if UNITY_2019_3_OR_NEWER
            {MRConfig.EyeTrackingCapability, true },
#endif
            // Android Settings
            {MRConfig.AndroidMultiThreadedRendering, true },
            {MRConfig.AndroidMinSdkVersion, true },

            // iOS Settings
            {MRConfig.IOSMinOSVersion, true },
            {MRConfig.IOSArchitecture, true },
            {MRConfig.IOSCameraUsageDescription, true },
        };

        private const string WindowKey = "_MixedRealityToolkit_Editor_MixedRealityProjectConfiguratorWindow";
        private const float Default_Window_Height = 640.0f;
        private const float Default_Window_Width = 400.0f;

        private readonly GUIContent ApplyButtonContent = new GUIContent("Apply", "Apply configurations to this Unity Project");
        private readonly GUIContent LaterButtonContent = new GUIContent("Later", "Do not show this pop-up notification until next session");
        private readonly GUIContent IgnoreButtonContent = new GUIContent("Ignore", "Modify this preference under Edit > Project Settings > MRTK");

        private bool showConfigurations = false;

        /// <summary>
        /// Show the MRTK Project Configurator utility window or focus if already opened
        /// </summary>
        [MenuItem("Mixed Reality Toolkit/Utilities/Configure Unity Project", false, 499)]
        public static void ShowWindow()
        {
            // There should be only one configurator window open as a "pop-up". If already open, then just force focus on our instance
            if (IsOpen)
            {
                Instance.Focus();
            }
            else
            {
                var window = ScriptableObject.CreateInstance<MixedRealityProjectConfiguratorWindow>();
                window.titleContent = new GUIContent("MRTK Project Configurator", EditorGUIUtility.IconContent("_Popup").image);
                window.position = new Rect(Screen.width / 2.0f, Screen.height / 2.0f, Default_Window_Height, Default_Window_Width);
                window.ShowUtility();
            }
        }

        public static MixedRealityProjectConfiguratorWindow Instance { get; private set; }

        public static bool IsOpen => Instance != null;

        private void OnEnable()
        {
            Instance = this;

            CompilationPipeline.assemblyCompilationStarted += CompilationPipeline_assemblyCompilationStarted;
        }

        private void CompilationPipeline_assemblyCompilationStarted(string obj)
        {
            // There should be only one pop-up window which is generally tracked by IsOpen
            // However, when recompiling, Unity will call OnDestroy for this window but not actually destroy the editor window
            // This ensure we have a clean close on recompiles when this EditorWindow was open beforehand
            Close();
        }

        private void OnGUI()
        {
            MixedRealityInspectorUtility.RenderMixedRealityToolkitLogo();

            RenderChoiceDialog();

            EditorGUILayout.Space();

            showConfigurations = EditorGUILayout.Foldout(showConfigurations, "Modify Configurations", true);
            if (showConfigurations)
            {
                RenderConfigurations();
            }
        }

        private void RenderChoiceDialog()
        {
            const string dialogTitle = "Apply Default Settings?";
            const string dialogContent = "The Mixed Reality Toolkit would like to auto-apply useful settings to this Unity project";
            EditorGUILayout.LabelField(dialogTitle, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(dialogContent);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(ApplyButtonContent))
                {
                    ApplyConfigurations();
                    Close();
                }

                if (GUILayout.Button(LaterButtonContent))
                {
                    MixedRealityEditorSettings.IgnoreProjectConfigForSession = true;
                    Close();
                }

                if (GUILayout.Button(IgnoreButtonContent))
                {
                    MixedRealityPreferences.IgnoreSettingsPrompt = true;
                    Close();
                }
            }
        }

        private void RenderConfigurations()
        {
            EditorGUILayout.LabelField("Enabled options will be applied to the project. Disabled items are already properly configured.");
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Project Settings", EditorStyles.boldLabel);
            RenderToggle(MRConfig.ForceTextSerialization, "Enable Force Text Serialization");
            RenderToggle(MRConfig.VisibleMetaFiles, "Enable Visible meta files");
            if (!MixedRealityOptimizeUtils.IsBuildTargetAndroid() && !MixedRealityOptimizeUtils.IsBuildTargetIOS())
            {
                RenderToggle(MRConfig.VirtualRealitySupported, "Enable VR Supported");
            }
            RenderToggle(MRConfig.SinglePassInstancing, "Set Single Pass Instanced rendering path");
            RenderToggle(MRConfig.SpatialAwarenessLayer, "Set Default Spatial Awareness Layer");
            RenderToggle(MRConfig.EnableMSBuildForUnity, "Enable MSBuild for Unity");
            EditorGUILayout.Space();

            if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
                EditorGUILayout.LabelField("UWP Capabilities", EditorStyles.boldLabel);

                RenderToggle(MRConfig.MicrophoneCapability, "Enable Microphone Capability");
                RenderToggle(MRConfig.InternetClientCapability, "Enable Internet Client Capability");
                RenderToggle(MRConfig.SpatialPerceptionCapability, "Enable Spatial Perception Capability");
#if UNITY_2019_3_OR_NEWER
                RenderToggle(MRConfig.EyeTrackingCapability, "Enable Eye Gaze Input Capability");
#endif
            }
            else
            {
                trackToggles[MRConfig.MicrophoneCapability] = false;
                trackToggles[MRConfig.InternetClientCapability] = false;
                trackToggles[MRConfig.SpatialPerceptionCapability] = false;
#if UNITY_2019_3_OR_NEWER
                trackToggles[MRConfig.EyeTrackingCapability] = false;
#endif
            }

            if (MixedRealityOptimizeUtils.IsBuildTargetAndroid())
            {
                EditorGUILayout.LabelField("Android Settings", EditorStyles.boldLabel);
                RenderToggle(MRConfig.AndroidMultiThreadedRendering, "Disable Multi-Threaded Rendering");
                RenderToggle(MRConfig.AndroidMinSdkVersion, "Set Minimum API Level");
            }

            if (MixedRealityOptimizeUtils.IsBuildTargetIOS())
            {
                EditorGUILayout.LabelField("iOS Settings", EditorStyles.boldLabel);
                RenderToggle(MRConfig.IOSMinOSVersion, "Set Required OS Version");
                RenderToggle(MRConfig.IOSArchitecture, "Set Required Architecture");
                RenderToggle(MRConfig.IOSCameraUsageDescription, "Set Camera Usage Descriptions");
            }
        }

        private void ApplyConfigurations()
        {
            var configurationFilter = new HashSet<MRConfig>();
            foreach (var item in trackToggles)
            {
                if (item.Value)
                {
                    configurationFilter.Add(item.Key);
                }
            }

            MixedRealityProjectConfigurator.ConfigureProject(configurationFilter);
        }

        private void RenderToggle(MRConfig configKey, string title)
        {
            bool configured = MixedRealityProjectConfigurator.IsConfigured(configKey);
            using (new EditorGUI.DisabledGroupScope(configured))
            {
                if (configured)
                {
                    EditorGUILayout.LabelField(new GUIContent(title, InspectorUIUtility.SuccessIcon));
                    trackToggles[configKey] = false;
                }
                else
                {
                    trackToggles[configKey] = EditorGUILayout.ToggleLeft(title, trackToggles[configKey]);
                }
            }
        }
    }
}