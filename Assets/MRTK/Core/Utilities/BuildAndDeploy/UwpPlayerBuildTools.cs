// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    /// <summary>
    /// Class containing various utility methods to build a WSA solution from a Unity project.
    /// </summary>
    public static class UwpPlayerBuildTools
    {
        private static void ParseBuildCommandLine(ref UwpBuildInfo buildInfo)
        {
            IBuildInfo iBuildInfo = buildInfo;
            UnityPlayerBuildTools.ParseBuildCommandLine(ref iBuildInfo);

            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-buildAppx":
                        buildInfo.BuildAppx = true;
                        break;
                    case "-rebuildAppx":
                        buildInfo.RebuildAppx = true;
                        break;
                    case "-targetUwpSdk":
                        // Note: the min sdk target cannot be changed.
                        EditorUserBuildSettings.wsaUWPSDK = arguments[++i];
                        break;
                }
            }
        }

        /// <summary>
        /// Do a build configured for UWP Applications to the specified path, returns the error from <see cref="BuildPlayer(UwpBuildInfo, CancellationToken)"/>
        /// </summary>
        /// <param name="showDialog">Should the user be prompted to build the appx as well?</param>
        /// <returns>True, if build was successful.</returns>
        public static async Task<bool> BuildPlayer(string buildDirectory, bool showDialog = true, CancellationToken cancellationToken = default)
        {
            if (UnityPlayerBuildTools.CheckBuildScenes() == false)
            {
                return false;
            }

            var buildInfo = new UwpBuildInfo
            {
                OutputDirectory = buildDirectory,
                Scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled && !string.IsNullOrEmpty(scene.path)).Select(scene => scene.path),
                BuildAppx = !showDialog,
                GazeInputCapabilityEnabled = UwpBuildDeployPreferences.GazeInputCapabilityEnabled,

                // Configure Appx build preferences for post build action
                RebuildAppx = UwpBuildDeployPreferences.ForceRebuild,
                Configuration = UwpBuildDeployPreferences.BuildConfig,
                BuildPlatform = EditorUserBuildSettings.wsaArchitecture,
                PlatformToolset = UwpBuildDeployPreferences.PlatformToolset,
                AutoIncrement = BuildDeployPreferences.IncrementBuildVersion,
                Multicore = UwpBuildDeployPreferences.MulticoreAppxBuildEnabled,
                ResearchModeCapabilityEnabled = UwpBuildDeployPreferences.ResearchModeCapabilityEnabled,
                AllowUnsafeCode = UwpBuildDeployPreferences.AllowUnsafeCode,

                // Configure a post build action that will compile the generated solution
                PostBuildAction = PostBuildAction
            };

            async void PostBuildAction(IBuildInfo innerBuildInfo, BuildReport buildReport)
            {
                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    EditorUtility.DisplayDialog($"{PlayerSettings.productName} WindowsStoreApp Build {buildReport.summary.result}!", "See console for details", "OK");
                }
                else
                {
                    var uwpBuildInfo = innerBuildInfo as UwpBuildInfo;
                    Debug.Assert(uwpBuildInfo != null);
                    UwpAppxBuildTools.AddCapabilities(uwpBuildInfo);
                    UwpAppxBuildTools.UpdateAssemblyCSharpProject(uwpBuildInfo);

                    if (showDialog &&
                        !EditorUtility.DisplayDialog(PlayerSettings.productName, "Build Complete", "OK", "Build AppX"))
                    {
                        EditorAssemblyReloadManager.LockReloadAssemblies = true;
                        await UwpAppxBuildTools.BuildAppxAsync(uwpBuildInfo, cancellationToken);
                        EditorAssemblyReloadManager.LockReloadAssemblies = false;
                    }
                }
            }

            return await BuildPlayer(buildInfo, cancellationToken);
        }

        /// <summary>
        /// Build the Uwp Player.
        /// </summary>
        public static async Task<bool> BuildPlayer(UwpBuildInfo buildInfo, CancellationToken cancellationToken = default)
        {
            #region Gather Build Data

            if (buildInfo.IsCommandLine)
            {
                ParseBuildCommandLine(ref buildInfo);
            }

            #endregion Gather Build Data

            BuildReport buildReport = UnityPlayerBuildTools.BuildUnityPlayer(buildInfo);

            bool success = buildReport != null && buildReport.summary.result == BuildResult.Succeeded;

            if (success && buildInfo.BuildAppx)
            {
                success &= await UwpAppxBuildTools.BuildAppxAsync(buildInfo, cancellationToken);
            }

            return success;
        }
    }
}