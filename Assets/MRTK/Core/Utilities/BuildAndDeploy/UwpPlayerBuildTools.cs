// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
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
        private const string AppLauncherPath = @"Assets\AppLauncherModel.glb";

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

        private static void AddAppLauncherModelToProject(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if model has already been added
            XmlNodeList nodes = root.SelectNodes($"//None[@Include = \"{AppLauncherPath}\"]");
            if (nodes.Count > 0)
            {
                return;
            }

            var newNodeDoc = new XmlDocument();
            newNodeDoc.LoadXml($"<None Include=\"{AppLauncherPath}\">" +
                            "<DeploymentContent>true</DeploymentContent>" +
                            "</None>");
            var newNode = doc.ImportNode(newNodeDoc.DocumentElement, true);
            var list = doc.GetElementsByTagName("ItemGroup");
            var items = list.Item(1);
            items.AppendChild(newNode);
            doc.Save(filePath);
        }

        private static void AddAppLauncherModelToFilter(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if model has already been added
            XmlNodeList nodes = root.SelectNodes($"//None[@Include = \"{AppLauncherPath}\"]");
            if (nodes.Count > 0)
            {
                return;
            }

            var newNodeDoc = new XmlDocument();
            newNodeDoc.LoadXml($"<None Include=\"{AppLauncherPath}\">" +
                            "<Filter>Assets</Filter>" +
                            "</None>");
            var newNode = doc.ImportNode(newNodeDoc.DocumentElement, true);
            var list = doc.GetElementsByTagName("ItemGroup");
            var items = list.Item(0);
            items.AppendChild(newNode);
            doc.Save(filePath);
        }

        private static void UpdateManifest(string filePath)
        {
            var text = File.ReadAllText(filePath);
            var doc = new XmlDocument();
            doc.LoadXml(text);
            var root = doc.DocumentElement;

            // Check to see if the element exists already
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("uap5", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
            XmlNodeList nodes = root.SelectNodes("//uap5:MixedRealityModel", nsmgr);
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes != null && node.Attributes["Path"].Value == AppLauncherPath)
                {
                    return;
                }
            }
            root.SetAttribute("xmlns:uap5", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");

            var ignoredValue = root.GetAttribute("IgnorableNamespaces");
            root.SetAttribute("IgnorableNamespaces", ignoredValue + " uap5");

            var newElement = doc.CreateElement("uap5", "MixedRealityModel", "http://schemas.microsoft.com/appx/manifest/uap/windows10/5");
            newElement.SetAttribute("Path", AppLauncherPath);
            var list = doc.GetElementsByTagName("uap:DefaultTile");
            var items = list.Item(0);
            items.AppendChild(newElement);

            doc.Save(filePath);
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

                    if (!string.IsNullOrEmpty(BuildDeployPreferences.AppLauncherModelLocation))
                    {
                        string appxPath = $"{buildDirectory}/{PlayerSettings.productName}";

                        Debug.Log($"3D App Launcher: {BuildDeployPreferences.AppLauncherModelLocation}, Destination: {appxPath}/{AppLauncherPath}");

                        FileUtil.ReplaceFile(BuildDeployPreferences.AppLauncherModelLocation, $"{appxPath}/{AppLauncherPath}");
                        AddAppLauncherModelToProject($"{appxPath}/{PlayerSettings.productName}.vcxproj");
                        AddAppLauncherModelToFilter($"{appxPath}/{PlayerSettings.productName}.vcxproj.filters");
                        UpdateManifest($"{appxPath}/Package.appxmanifest");
                    }

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
        /// Build the UWP Player.
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
