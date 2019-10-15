// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Editor runtime controller for showing Project Configuration window and performance checks logging in current Unity project
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged
    {
        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";
        private const string MSFT_AudioSpatializerPlugin = "MS HRTF Spatializer";

        public MixedRealityEditorSettings()
        {
            callbackOrder = 0;
        }

        static MixedRealityEditorSettings()
        {
            // Detect when we enter player mode so we can try checking for optimal configuration
            EditorApplication.playModeStateChanged += OnPlayStateModeChanged;

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
            get
            {
                return SessionState.GetBool(SessionKey, false);
            }

            set
            {
                SessionState.SetBool(SessionKey, value);
            }
        }

        private static void OnPlayStateModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && MixedRealityPreferences.RunOptimalConfiguration)
            {
                LogConfigurationWarnings();
            }
        }

        private static void ShowProjectConfigurationDialog()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode
                && !IgnoreProjectConfigForSession
                && !MixedRealityPreferences.IgnoreSettingsPrompt
                && !MixedRealityProjectConfigurator.IsProjectConfigured())
            {
                MixedRealityProjectConfiguratorWindow.ShowWindow();
            }
        }

        /// <summary>
        /// Checks critical project settings and suggests changes to optimize performance via logged warnings
        /// </summary>
        private static void LogConfigurationWarnings()
        {
            if (!PlayerSettings.virtualRealitySupported)
            {
                Debug.LogWarning("<b>Virtual reality supported</b> not enabled. Check <i>XR Settings</i> under <i>Player Settings</i>");
            }

            if (!MixedRealityOptimizeUtils.IsSinglePassInstanced())
            {
                Debug.LogWarning("XR stereo rendering mode not set to <b>Single Pass Instanced</b>. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance");
            }

            // If targeting Windows Mixed Reality platform
            if (MixedRealityOptimizeUtils.IsBuildTargetUWP())
            {
                if (!MixedRealityOptimizeUtils.IsDepthBufferSharingEnabled())
                {
                    // If depth buffer sharing not enabled, advise to enable setting
                    Debug.LogWarning("<b>Depth Buffer Sharing</b> is not enabled to improve hologram stabilization. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance");
                }

                if (!MixedRealityOptimizeUtils.IsWMRDepthBufferFormat16bit())
                {
                    // If depth format is 24-bit, advise to consider 16-bit for performance.
                    Debug.LogWarning("<b>Depth Buffer Sharing</b> has 24-bit depth format selected. Consider using 16-bit for performance. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance");
                }

                if (!AudioSettings.GetSpatializerPluginName().Equals(MSFT_AudioSpatializerPlugin))
                {
                    // If using UWP, developers should use the Microsoft Audio Spatilizer plugin
                    Debug.LogWarning("<b>Audio Spatializer Plugin</b> not currently set to <i>" + MSFT_AudioSpatializerPlugin + "</i>. Switch to <i>" + MSFT_AudioSpatializerPlugin + "</i> under <i>Project Settings</i> > <i>Audio</i> > <i>Spatializer Plugin</i>");
                }
            }
        }
    }
}
