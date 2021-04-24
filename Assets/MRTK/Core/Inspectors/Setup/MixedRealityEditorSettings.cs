// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Editor runtime controller for showing Project Configuration window and performance checks logging in current Unity project
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged, IPreprocessBuildWithReport
    {
        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";
        private static readonly string[] UwpRecommendedAudioSpatializers = { "MS HRTF Spatializer", "Microsoft Spatializer" };

#if UNITY_ANDROID
        const string RenderingMode = "Single Pass Stereo";
#else
        const string RenderingMode = "Single Pass Instanced";
#endif

        public MixedRealityEditorSettings()
        {
            callbackOrder = 0;
        }

        static MixedRealityEditorSettings()
        {
            // Detect when we enter player mode so we can try checking for optimal configuration
            EditorApplication.playModeStateChanged += OnPlayStateModeChanged;

            // Subscribe to editor application update which will call us once the editor is initialized and running
            EditorApplication.update += OnInit;
        }

        private static void OnInit()
        {
            // We only want to execute once to initialize, unsubscribe from update event
            EditorApplication.update -= OnInit;

            ShowProjectConfigurationDialog();
        }

        /// <inheritdoc />
        public int callbackOrder { get; private set; }

        /// <inheritdoc />
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            IgnoreProjectConfigForSession = false;
        }

        /// <summary>
        /// Session state wrapper that tracks whether to ignore checking Project Configuration for the current Unity session
        /// </summary>
        public static bool IgnoreProjectConfigForSession
        {
            get => SessionState.GetBool(SessionKey, false);
            set => SessionState.SetBool(SessionKey, value);
        }

        /// <inheritdoc />
        public void OnPreprocessBuild(BuildReport report)
        {
            if (MixedRealityProjectPreferences.RunOptimalConfiguration)
            {
                LogBuildConfigurationWarnings();
            }
        }

        private static void OnPlayStateModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && MixedRealityProjectPreferences.RunOptimalConfiguration)
            {
                LogConfigurationWarnings();
            }
        }

        private static void ShowProjectConfigurationDialog()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode
                && !IgnoreProjectConfigForSession
                && !MixedRealityProjectPreferences.IgnoreSettingsPrompt)
            {
                if (!XRSettingsUtilities.XREnabled)
                {
                    MixedRealityProjectConfiguratorWindow.ShowWindowOnInit(false);
                }
                else if (!MixedRealityProjectConfigurator.IsProjectConfigured())
                {
                    MixedRealityProjectConfiguratorWindow.ShowWindowOnInit(true);
                }
            }
        }

        /// <summary>
        /// Checks critical project settings and suggests changes to optimize performance via logged warnings
        /// </summary>
        private static void LogConfigurationWarnings()
        {
            if (!XRSettingsUtilities.XREnabled)
            {
                Debug.LogWarning("There is no properly configured XR pipeline in the project! Please run the configurator by clicking on Mixed Reality (menu bar) -> Toolkit -> Utilities -> Configure Project for MRTK if the current settings are not desired.");
            }

            if (!MixedRealityOptimizeUtils.IsOptimalRenderingPath())
            {
                Debug.LogWarning($"XR stereo rendering mode not set to <b>{RenderingMode}</b>. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance.\nPlease note: The Mixed Reality Toolkit Tools package is required to use the Optimize Window.");
            }

            // If targeting Windows Mixed Reality platform
            if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
                if (!MixedRealityOptimizeUtils.IsDepthBufferSharingEnabled())
                {
                    // If depth buffer sharing not enabled, advise to enable setting
                    Debug.LogWarning("<b>Depth Buffer Sharing</b> is not enabled to improve hologram stabilization. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance.\nPlease note: The Mixed Reality Toolkit Tools package is required to use the Optimize Window.");
                }

                if (!MixedRealityOptimizeUtils.IsWMRDepthBufferFormat16bit())
                {
                    // If depth format is 24-bit, advise to consider 16-bit for performance.
                    Debug.LogWarning("<b>Depth Buffer Sharing</b> has 24-bit depth format selected. Consider using 16-bit for performance. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance.\nPlease note: The Mixed Reality Toolkit Tools package is required to use the Optimize Window.");
                }

                if (!UwpRecommendedAudioSpatializers.Contains(SpatializerUtilities.CurrentSpatializer))
                {
                    Debug.Log($"This application is not using the recommended <b>Audio Spatializer Plugin</b>. Go to <i>Project Settings</i> > <i>Audio</i> > <i>Spatializer Plugin</i> and select one of the following: {string.Join(", ", UwpRecommendedAudioSpatializers)}.");
                }
            }
            else if (SpatializerUtilities.CurrentSpatializer == null)
            {
                Debug.Log($"This application is not using an <b>Audio Spatializer Plugin</b>. Go to <i>Project Settings</i> > <i>Audio</i> > <i>Spatializer Plugin</i> and select one of the available options.");
            }
        }

        /// <summary>
        /// Checks critical project settings and suggests changes to optimize build performance via logged warnings
        /// </summary>
        private static void LogBuildConfigurationWarnings()
        {
            if (PlayerSettings.stripUnusedMeshComponents)
            {
                /// For more information please see <see href="https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/performance/perf-getting-started#optimize-mesh-data">Optimize Mesh Data</see>
                Debug.LogWarning("<b>Optimize Mesh Data</b> is enabled. This setting can drastically increase build times. It is recommended to disable this setting during development and re-enable during \"Master\" build creation. See <i>Player Settings</i> > <i>Other Settings</i> > <i>Optimize Mesh Data</i>");
            }
        }
    }
}
