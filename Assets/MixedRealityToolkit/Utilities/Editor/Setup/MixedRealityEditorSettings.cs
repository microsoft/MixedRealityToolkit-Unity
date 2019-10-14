// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Sets Force Text Serialization and visible meta files in all projects that use the Mixed Reality Toolkit.
    /// </summary>
    [InitializeOnLoad]
    public class MixedRealityEditorSettings : IActiveBuildTargetChanged
    {
        public MixedRealityEditorSettings()
        {
            callbackOrder = 0;
        }

        private const string SessionKey = "_MixedRealityToolkit_Editor_ShownSettingsPrompts";
        private const string MSFT_AudioSpatializerPlugin = "MS HRTF Spatializer";
        private const int SpatialAwarenessDefaultLayer = 31;

        [Obsolete("Use the 'MixedRealityToolkitFiles' APIs.")]
        public static string MixedRealityToolkit_AbsoluteFolderPath
        {
            get
            {
                if (MixedRealityToolkitFiles.AreFoldersAvailable)
                {
                    if (Application.isEditor && MixedRealityToolkitFiles.MRTKDirectories.Count() > 1)
                    {
                        Debug.LogError($"A deprecated API '{nameof(MixedRealityEditorSettings)}.{nameof(MixedRealityToolkit_AbsoluteFolderPath)}' " +
                            "is being used, and there are more than one MRTK directory in the project; most likely due to ingestion as NuGet. " +
                            $"Update to use the '{nameof(MixedRealityToolkitFiles)}' APIs.");
                    }

                    return MixedRealityToolkitFiles.MRTKDirectories.First();
                }

                Debug.LogError("Unable to find the Mixed Reality Toolkit's directory!");
                return null;
            }
        }

        [Obsolete("Use the 'MixedRealityToolkitFiles' APIs.")]
        public static string MixedRealityToolkit_RelativeFolderPath
        {
            get { return MixedRealityToolkitFiles.GetAssetDatabasePath(MixedRealityToolkit_AbsoluteFolderPath); }
        }

        static MixedRealityEditorSettings()
        {
            // Detect when we enter player mode so we can try checking for optimal configuration
            EditorApplication.playModeStateChanged += OnPlayStateModeChanged;

            if (!IsNewSession || Application.isPlaying)
            {
                return;
            }

            ShowSettingsDialog();
        }

        private static void OnPlayStateModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode && MixedRealityPreferences.RunOptimalConfiguration)
            {
                CheckOptimalConfiguration();
            }
        }

        /// <summary>
        /// On load, show dialog to confirm MRTK can apply useful settings 
        /// </summary>
        private static void ShowSettingsDialog()
        {
            bool refresh = false;
            bool restart = false;

            if (!MixedRealityPreferences.IgnoreSettingsPrompt)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("The Mixed Reality Toolkit needs to apply the following settings to your project:\n\n");

                var forceTextSerialization = EditorSettings.serializationMode == SerializationMode.ForceText;

                if (!forceTextSerialization)
                {
                    builder.AppendLine("- Force Text Serialization");
                }

                var visibleMetaFiles = EditorSettings.externalVersionControl.Equals("Visible Meta Files");

                if (!visibleMetaFiles)
                {
                    builder.AppendLine("- Visible meta files");
                }

                if (!PlayerSettings.virtualRealitySupported)
                {
                    builder.AppendLine("- Enable XR Settings for your current platform");
                }

                var usingSinglePassInstancing = PlayerSettings.stereoRenderingPath == StereoRenderingPath.Instancing;
                if (!usingSinglePassInstancing)
                {
                    builder.AppendLine("- Set Single Pass Instanced rendering path");
                }

                // Only make change if not already set. Regardless of whether it is already SpatialAwareness or something user set
                var isSpatialLayerAvailable = string.IsNullOrEmpty(LayerMask.LayerToName(SpatialAwarenessDefaultLayer));
                if (isSpatialLayerAvailable)
                {
                    builder.AppendLine("- Set Default Spatial Awareness Layer");
                }

                builder.Append("\nWould you like to make these changes?");

                if (!forceTextSerialization || !visibleMetaFiles || !PlayerSettings.virtualRealitySupported || !usingSinglePassInstancing || isSpatialLayerAvailable)
                {
                    var choice = EditorUtility.DisplayDialogComplex("Apply Mixed Reality Toolkit Default Settings?", builder.ToString(), "Apply", "Ignore", "Later");

                    switch (choice)
                    {
                        case 0:
                            EditorSettings.serializationMode = SerializationMode.ForceText;
                            EditorSettings.externalVersionControl = "Visible Meta Files";
                            ApplyXRSettings();
                            PlayerSettings.stereoRenderingPath = StereoRenderingPath.Instancing;
                            if (isSpatialLayerAvailable)
                            {
                                if (EditorLayerExtensions.SetupLayer(SpatialAwarenessDefaultLayer, "Spatial Awareness"))
                                {
                                    Debug.LogWarning(string.Format($"Can't modify project layers. It's possible the format of the layers and tags data has changed in this version of Unity. Set layer {SpatialAwarenessDefaultLayer} to \"Spatial Awareness\" manually via Project Settings > Tags and Layers window."));
                                }
                            }
                            refresh = true;
                            break;
                        case 1:
                            MixedRealityPreferences.IgnoreSettingsPrompt = true;
                            break;
                        case 2:
                            break;
                    }
                }
            }

#if !UNITY_2019_3_OR_NEWER
            if (PlayerSettings.scriptingRuntimeVersion != ScriptingRuntimeVersion.Latest)
            {
                PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Latest;
                restart = true;
            }
#endif

            if (refresh || restart)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            if (restart)
            {
                EditorApplication.OpenProject(Directory.GetParent(Application.dataPath).ToString());
            }
        }

        /// <summary>
        /// Discover and set the appropriate XR Settings for the current build target.
        /// </summary>
        private static void ApplyXRSettings()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            List<string> targetSDKs = new List<string>();
            foreach (string sdk in PlayerSettings.GetAvailableVirtualRealitySDKs(targetGroup))
            {
                if (sdk.Contains("OpenVR") || sdk.Contains("Windows"))
                {
                    targetSDKs.Add(sdk);
                }
            }

            if (targetSDKs.Count != 0)
            {
                PlayerSettings.SetVirtualRealitySDKs(targetGroup, targetSDKs.ToArray());
                PlayerSettings.SetVirtualRealitySupported(targetGroup, true);
            }
        }

        /// <summary>
        /// Returns true the first time it is called within this editor session, and false for all subsequent calls.
        /// </summary>
        /// <remarks>A new session is also true if the editor build target group is changed.</remarks>
        private static bool IsNewSession
        {
            get
            {
                if (SessionState.GetBool(SessionKey, false)) { return false; }

                SessionState.SetBool(SessionKey, true);
                return true;
            }
        }

        /// <summary>
        /// Finds the path of a directory relative to the project folder.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        internal static bool FindRelativeDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            string absolutePath;
            if (FindDirectory(directoryPathToSearch, directoryName, out absolutePath))
            {
                path = MixedRealityToolkitFiles.GetAssetDatabasePath(absolutePath);
                return true;
            }

            path = string.Empty;
            return false;
        }

        /// <summary>
        /// Finds the absolute path of a directory.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        internal static bool FindDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            path = string.Empty;

            var directories = Directory.GetDirectories(directoryPathToSearch);

            for (int i = 0; i < directories.Length; i++)
            {
                var name = Path.GetFileName(directories[i]);

                if (name != null && name.Equals(directoryName))
                {
                    path = directories[i];
                    return true;
                }

                if (FindDirectory(directories[i], directoryName, out path))
                {
                    return true;
                }
            }

            return false;
        }

        [Obsolete("Use MixedRealityToolkitFiles.GetAssetDatabasePath instead.")]
        internal static string MakePathRelativeToProject(string absolutePath) => MixedRealityToolkitFiles.GetAssetDatabasePath(absolutePath);


        /// <summary>
        /// Checks project critical project settings and suggests changes to optimize performance via logged warnings
        /// </summary>
        private static void CheckOptimalConfiguration()
        {
            if (!PlayerSettings.virtualRealitySupported)
            {
                Debug.LogWarning("<b>Virtual reality supported</b> not enabled. Check <i>XR Settings</i> under <i>Player Settings</i>");
            }

            if (PlayerSettings.stereoRenderingPath != StereoRenderingPath.Instancing)
            {
                Debug.LogWarning("XR stereo rendering mode not set to <b>Single Pass Instanced</b>. See <i>Mixed Reality Toolkit</i> > <i>Utilities</i> > <i>Optimize Window</i> tool for more information to improve performance");
            }

            // If targeting Windows Mixed Reality platform
            if (MixedRealityOptimizeUtils.IsBuildTargetWMR())
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

        /// <inheritdoc />
        public int callbackOrder { get; private set; }

        /// <inheritdoc />
        public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            SessionState.SetBool(SessionKey, false);
        }
    }
}
