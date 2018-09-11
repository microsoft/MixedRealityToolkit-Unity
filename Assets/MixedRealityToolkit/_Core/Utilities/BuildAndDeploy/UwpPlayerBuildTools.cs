// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Build
{
    /// <summary>
    /// Class containing various utility methods to build a WSA solution from a Unity project.
    /// </summary>
    public static class UwpPlayerBuildTools
    {
        // Build configurations. Exactly one of these should be defined for any given build.
        public const string BuildSymbolDebug = "DEBUG";
        public const string BuildSymbolRelease = "RELEASE";
        public const string BuildSymbolMaster = "MASTER";

        /// <summary>
        /// A method capable of configuring <see cref="BuildInfo"/> settings.
        /// </summary>
        /// <param name="toConfigure">The settings to configure.</param>
        public delegate void BuildInfoConfigurationMethod(ref BuildInfo toConfigure);

        /// <summary>
        /// Add a handler to this event to override <see cref="BuildInfo"/> defaults before a build.
        /// </summary>
        /// <seealso cref="RaiseOverrideBuildDefaults"/>
        public static event BuildInfoConfigurationMethod OverrideBuildDefaults;

        /// <summary>
        /// Event triggered when a build starts.
        /// </summary>
        public static event Action<BuildInfo> BuildStarted;

        /// <summary>
        /// Event triggered when a build completes.
        /// </summary>
        public static event Action<BuildInfo, BuildReport> BuildCompleted;

        /// <summary>
        /// Call this method to give other code an opportunity to override <see cref="BuildInfo"/> defaults.
        /// </summary>
        /// <param name="toConfigure">The settings to configure.</param>
        /// <seealso cref="OverrideBuildDefaults"/>
        public static void RaiseOverrideBuildDefaults(ref BuildInfo toConfigure)
        {
            OverrideBuildDefaults?.Invoke(ref toConfigure);
        }

        /// <summary>
        /// Get the Unity Project Root Path.
        /// </summary>
        /// <returns></returns>
        public static string GetProjectPath()
        {
            return Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
        }

        private static void CopyBuildDirectory(string sourceDirectoryPath, string destinationDirectoryPath, CopyDirectoryInfo directoryInfo)
        {
            sourceDirectoryPath = Path.Combine(sourceDirectoryPath, directoryInfo.Source);
            destinationDirectoryPath = Path.Combine(destinationDirectoryPath, directoryInfo.Destination ?? directoryInfo.Source);

            Debug.Log($"{(directoryInfo.Recursive ? "Recursively copying" : "Copying")} \"{sourceDirectoryPath}\\{directoryInfo.Filter}\" to \"{destinationDirectoryPath}\"");

            foreach (string sourceFilePath
                in Directory.GetFiles(
                    sourceDirectoryPath,
                    directoryInfo.Filter,
                    directoryInfo.Recursive ?
                        SearchOption.AllDirectories :
                        SearchOption.TopDirectoryOnly))
            {
                string destinationFilePath = Path.GetDirectoryName(sourceFilePath.Replace(sourceDirectoryPath, destinationDirectoryPath));
                Debug.Assert(!string.IsNullOrEmpty(destinationFilePath));

                try
                {
                    Directory.CreateDirectory(destinationFilePath);
                    Debug.Assert(File.Exists(destinationFilePath));
                    File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Failed to copy \"{sourceFilePath}\" to \"{destinationFilePath}\" with \"{exception}\"");
                }
            }
        }

        public static void BuildUwpPlayer(BuildInfo buildInfo)
        {
            BuildTargetGroup buildTargetGroup = GetGroup(buildInfo.BuildTarget);
            string oldBuildSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!string.IsNullOrEmpty(oldBuildSymbols))
            {
                if (buildInfo.HasConfigurationSymbol())
                {
                    buildInfo.AppendSymbols(BuildInfo.RemoveConfigurationSymbols(oldBuildSymbols));
                }
                else
                {
                    buildInfo.AppendSymbols(oldBuildSymbols.Split(';'));
                }
            }

            if ((buildInfo.BuildOptions & BuildOptions.Development) == BuildOptions.Development)
            {
                if (!buildInfo.HasConfigurationSymbol())
                {
                    buildInfo.AppendSymbols(BuildSymbolDebug);
                }

            }

            if (buildInfo.HasAnySymbols(BuildSymbolDebug))
            {
                buildInfo.BuildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging;
            }

            if (buildInfo.HasAnySymbols(BuildSymbolRelease))
            {
                // Unity automatically adds the DEBUG symbol if the BuildOptions.Development flag is
                // specified. In order to have debug symbols and the RELEASE symbols we have to
                // inject the symbol Unity relies on to enable the /debug+ flag of csc.exe which is "DEVELOPMENT_BUILD"
                buildInfo.AppendSymbols("DEVELOPMENT_BUILD");
            }

            BuildTarget oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup oldBuildTargetGroup = GetGroup(oldBuildTarget);

            EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildInfo.BuildTarget);

            WSAUWPBuildType? oldWSAUWPBuildType = EditorUserBuildSettings.wsaUWPBuildType;

            if (buildInfo.WSAUWPBuildType.HasValue)
            {
                EditorUserBuildSettings.wsaUWPBuildType = buildInfo.WSAUWPBuildType.Value;
            }

            var oldWSAGenerateReferenceProjects = EditorUserBuildSettings.wsaGenerateReferenceProjects;

            if (buildInfo.WSAGenerateReferenceProjects.HasValue)
            {
                EditorUserBuildSettings.wsaGenerateReferenceProjects = buildInfo.WSAGenerateReferenceProjects.Value;
            }

            var oldColorSpace = PlayerSettings.colorSpace;

            if (buildInfo.ColorSpace.HasValue)
            {
                PlayerSettings.colorSpace = buildInfo.ColorSpace.Value;
            }

            if (buildInfo.BuildSymbols != null)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildInfo.BuildSymbols);
            }

            // For the WSA player, Unity builds into a target directory.
            // For other players, the OutputPath parameter indicates the
            // path to the target executable to build.
            if (buildInfo.BuildTarget == BuildTarget.WSAPlayer)
            {
                Directory.CreateDirectory(buildInfo.OutputDirectory);
            }

            OnPreProcessBuild(buildInfo);

            EditorUtility.DisplayProgressBar("Build Pipeline", "Gathering Build Data...", 0.25f);

            BuildReport buildReport = default(BuildReport);
            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                    buildInfo.Scenes.ToArray(),
                    buildInfo.OutputDirectory,
                    buildInfo.BuildTarget,
                    buildInfo.BuildOptions);

                if (buildReport.summary.result != BuildResult.Succeeded)
                {
                    throw new Exception($"Build Result: {buildReport.summary.result.ToString()}");
                }
            }
            finally
            {
                OnPostProcessBuild(buildInfo, buildReport);

                if (buildInfo.BuildTarget == BuildTarget.WSAPlayer && EditorUserBuildSettings.wsaGenerateReferenceProjects)
                {
                    UwpProjectPostProcess.Execute(buildInfo.OutputDirectory);
                }

                PlayerSettings.colorSpace = oldColorSpace;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, oldBuildSymbols);

                EditorUserBuildSettings.wsaUWPBuildType = oldWSAUWPBuildType.Value;

                EditorUserBuildSettings.wsaGenerateReferenceProjects = oldWSAGenerateReferenceProjects;
                EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
            }
        }

        /// <summary>
        /// Used to trigger a build from the command line for continuous integration.
        /// </summary>
        public static void BuildUwpPlayer_CommandLine()
        {
            var buildInfo = new BuildInfo
            {
                // Use scenes from the editor build settings.
                Scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path),

                // Configure a post build action to throw appropriate error code.
                PostBuildAction = (innerBuildInfo, buildReport) =>
                {
                    if (buildReport.summary.result != BuildResult.Succeeded)
                    {
                        EditorApplication.Exit(1);
                    }
                }
            };

            RaiseOverrideBuildDefaults(ref buildInfo);
            ParseBuildCommandLine(ref buildInfo);
            BuildUwpPlayer(buildInfo);
        }

        private static void ParseBuildCommandLine(ref BuildInfo buildInfo)
        {
            string[] arguments = Environment.GetCommandLineArgs();

            buildInfo.IsCommandLine = true;

            for (int i = 0; i < arguments.Length; ++i)
            {
                // Can't use -buildTarget which is something Unity already takes as an argument for something.
                if (string.Equals(arguments[i], "-duskBuildTarget", StringComparison.InvariantCultureIgnoreCase))
                {
                    buildInfo.BuildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), arguments[++i]);
                }
                else if (string.Equals(arguments[i], "-wsaSDK", StringComparison.InvariantCultureIgnoreCase))
                {
                    string wsaSdkArg = arguments[++i];
                    buildInfo.WSASdk = (WSASDK)Enum.Parse(typeof(WSASDK), wsaSdkArg);
                }
                else if (string.Equals(arguments[i], "-wsaUwpSdk", StringComparison.InvariantCultureIgnoreCase))
                {
                    buildInfo.WSAUwpSdk = arguments[++i];
                }
                else if (string.Equals(arguments[i], "-wsaUWPBuildType", StringComparison.InvariantCultureIgnoreCase))
                {
                    buildInfo.WSAUWPBuildType = (WSAUWPBuildType)Enum.Parse(typeof(WSAUWPBuildType), arguments[++i]);
                }
                else if (string.Equals(arguments[i], "-wsaGenerateReferenceProjects", StringComparison.InvariantCultureIgnoreCase))
                {
                    buildInfo.WSAGenerateReferenceProjects = bool.Parse(arguments[++i]);
                }
                else if (string.Equals(arguments[i], "-buildOutput", StringComparison.InvariantCultureIgnoreCase))
                {
                    buildInfo.OutputDirectory = arguments[++i];
                }
                else if (string.Equals(arguments[i], "-x86", StringComparison.CurrentCultureIgnoreCase))
                {
                    buildInfo.BuildPlatform = arguments[++i].Substring(1);
                }
                else if (string.Equals(arguments[i], "-x64", StringComparison.CurrentCultureIgnoreCase))
                {
                    buildInfo.BuildPlatform = arguments[++i].Substring(1);
                }
                else if (string.Equals(arguments[i], "-buildDesc", StringComparison.InvariantCultureIgnoreCase))
                {
                    ParseBuildDescriptionFile(arguments[++i], ref buildInfo);
                }
                else if (string.Equals(arguments[i], "-unityBuildSymbols", StringComparison.InvariantCultureIgnoreCase))
                {
                    string newBuildSymbols = arguments[++i];
                    buildInfo.AppendSymbols(newBuildSymbols.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }
        }

        private static void ParseBuildDescriptionFile(string filename, ref BuildInfo buildInfo)
        {
            Debug.Log($"Build: Using \"{filename}\" as build description");

            // Parse the XML file
            var reader = new XmlTextReader(filename);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Equals(reader.Name, "SceneList", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Set the scenes we want to build
                            buildInfo.Scenes = ReadSceneList(reader);
                        }
                        else if (string.Equals(reader.Name, "CopyList", StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Set the directories we want to copy
                            buildInfo.CopyDirectories = ReadCopyList(reader);
                        }
                        break;
                }
            }
        }

        private static BuildTargetGroup GetGroup(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return BuildTargetGroup.Standalone;
                default:
                    return BuildTargetGroup.Unknown;
            }
        }

        private static IEnumerable<string> ReadSceneList(XmlReader reader)
        {
            var result = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Equals(reader.Name, "Scene", StringComparison.InvariantCultureIgnoreCase))
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                if (string.Equals(reader.Name, "Name", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    result.Add(reader.Value);
                                    Debug.Log($"Build: Adding scene \"{reader.Value}\"");
                                }
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (string.Equals(reader.Name, "SceneList", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return result;
                        }
                        break;
                }
            }

            return result;
        }

        private static IEnumerable<CopyDirectoryInfo> ReadCopyList(XmlReader reader)
        {
            var result = new List<CopyDirectoryInfo>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Equals(reader.Name, "Copy", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string source = null;
                            string destination = null;
                            string filter = null;
                            bool recursive = false;

                            while (reader.MoveToNextAttribute())
                            {
                                if (string.Equals(reader.Name, "Source", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    source = reader.Value;
                                }
                                else if (string.Equals(reader.Name, "Destination", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    destination = reader.Value;
                                }
                                else if (string.Equals(reader.Name, "Recursive", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    recursive = Convert.ToBoolean(reader.Value);
                                }
                                else if (string.Equals(reader.Name, "Filter", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    filter = reader.Value;
                                }
                            }

                            if (source != null)
                            {
                                // Either the file specifies the Destination as well, or else CopyBuildDirectory will use Source for Destination
                                var info = new CopyDirectoryInfo { Source = source };

                                if (destination != null)
                                {
                                    info.Destination = destination;
                                }

                                if (filter != null)
                                {
                                    info.Filter = filter;
                                }

                                info.Recursive = recursive;

                                Debug.Log($"Build: Adding {(info.Recursive ? "Recursive " : "")} copy \"{info.Source}\\{info.Filter}\" => \"{info.Destination ?? info.Source}\"");

                                result.Add(info);
                            }
                        }
                        break;

                    case XmlNodeType.EndElement:
                        if (string.Equals(reader.Name, "CopyList", StringComparison.InvariantCultureIgnoreCase))
                            return result;
                        break;
                }
            }

            return result;
        }

        private static void OnPreProcessBuild(BuildInfo buildInfo)
        {
            // Raise the global event for listeners
            BuildStarted?.Invoke(buildInfo);

            // Call the pre-build action, if any
            buildInfo.PreBuildAction?.Invoke(buildInfo);
        }

#if UNITY_2018_1_OR_NEWER
        private static async void OnPostProcessBuild(BuildInfo buildInfo, BuildReport buildReport)
        {
            if (buildReport.summary.result == BuildResult.Succeeded)
#else
        private static async void OnPostProcessBuild(BuildInfo buildInfo, string buildReport)
        {
            if (string.IsNullOrEmpty(buildReport))
#endif
            {
                string outputProjectDirectoryPath = Path.Combine(GetProjectPath(), buildInfo.OutputDirectory);
                if (buildInfo.CopyDirectories != null)
                {
                    string inputProjectDirectoryPath = GetProjectPath();
                    foreach (var directory in buildInfo.CopyDirectories)
                    {
                        CopyBuildDirectory(inputProjectDirectoryPath, outputProjectDirectoryPath, directory);
                    }
                }

                if (buildInfo.IsCommandLine && buildInfo.BuildAppx)
                {
                    await UwpAppxBuildTools.BuildAppxAsync(
                            PlayerSettings.productName,
                            true,
                            buildInfo.Configuration,
                            buildInfo.BuildPlatform,
                            outputProjectDirectoryPath,
                            true);
                }
            }

            // Raise the global event for listeners
            BuildCompleted?.Invoke(buildInfo, buildReport);

            // Call the post-build action, if any
            buildInfo.PostBuildAction?.Invoke(buildInfo, buildReport);
        }
    }
}
