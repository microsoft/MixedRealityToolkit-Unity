// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Build.DataStructures;
using MixedRealityToolkit.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Build
{
    /// <summary>
    /// Class containing various utility methods to build a WSA solution from a Unity project.
    /// </summary>
    public static class BuildSLNUtilities
    {
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
        /// Call this method to give other code an opportunity to override <see cref="BuildInfo"/> defaults.
        /// </summary>
        /// <param name="toConfigure">>The settings to configure.</param>
        /// <seealso cref="OverrideBuildDefaults"/>
        public static void RaiseOverrideBuildDefaults(ref BuildInfo toConfigure)
        {
            if (OverrideBuildDefaults != null)
            {
                OverrideBuildDefaults(ref toConfigure);
            }
        }

        // Build configurations. Exactly one of these should be defined for any given build.
        public const string BuildSymbolDebug = "DEBUG";
        public const string BuildSymbolRelease = "RELEASE";
        public const string BuildSymbolMaster = "MASTER";

        /// <summary>
        /// Event triggered when a build starts.
        /// </summary>
        public static event Action<BuildInfo> BuildStarted;

        /// <summary>
        /// Event triggered when a build completes.
        /// </summary>
        public static event Action<BuildInfo, string> BuildCompleted;

        public static void PerformBuild(BuildInfo buildInfo)
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
                //Unity automatically adds the DEBUG symbol if the BuildOptions.Development flag is
                //specified. In order to have debug symbols and the RELEASE symbols we have to
                //inject the symbol Unity relies on to enable the /debug+ flag of csc.exe which is "DEVELOPMENT_BUILD"
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

            string buildError = "Error";
            try
            {
                // For the WSA player, Unity builds into a target directory.
                // For other players, the OutputPath parameter indicates the
                // path to the target executable to build.
                if (buildInfo.BuildTarget == BuildTarget.WSAPlayer)
                {
                    Directory.CreateDirectory(buildInfo.OutputDirectory);
                }

                OnPreProcessBuild(buildInfo);
                buildError = BuildPipeline.BuildPlayer(
                    buildInfo.Scenes.ToArray(),
                    buildInfo.OutputDirectory,
                    buildInfo.BuildTarget,
                    buildInfo.BuildOptions).ToString();

                if (buildError.StartsWith("Error"))
                {
                    throw new Exception(buildError);
                }
            }
            finally
            {
                OnPostProcessBuild(buildInfo, buildError);

                if (buildInfo.BuildTarget == BuildTarget.WSAPlayer && EditorUserBuildSettings.wsaGenerateReferenceProjects)
                {
                    UwpProjectPostProcess.Execute(buildInfo.OutputDirectory);
                }

                PlayerSettings.colorSpace = oldColorSpace;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, oldBuildSymbols);

                if (oldWSAUWPBuildType.HasValue)
                {
                    EditorUserBuildSettings.wsaUWPBuildType = oldWSAUWPBuildType.Value;
                }

                EditorUserBuildSettings.wsaGenerateReferenceProjects = oldWSAGenerateReferenceProjects;

                EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
            }
        }

        public static void ParseBuildCommandLine(ref BuildInfo buildInfo)
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

        public static void PerformBuild_CommandLine()
        {
            var buildInfo = new BuildInfo
            {
                // Use scenes from the editor build settings.
                Scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path),

                // Configure a post build action to throw appropriate error code.
                PostBuildAction = (innerBuildInfo, buildError) =>
                {
                    if (!string.IsNullOrEmpty(buildError))
                    {
                        EditorApplication.Exit(1);
                    }
                }
            };

            RaiseOverrideBuildDefaults(ref buildInfo);

            ParseBuildCommandLine(ref buildInfo);

            PerformBuild(buildInfo);
        }

        public static void ParseBuildDescriptionFile(string filename, ref BuildInfo buildInfo)
        {
            Debug.Log(string.Format(CultureInfo.InvariantCulture, "Build: Using \"{0}\" as build description", filename));

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

        private static IEnumerable<string> ReadSceneList(XmlTextReader reader)
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
                                    Debug.Log(string.Format(CultureInfo.InvariantCulture, "Build: Adding scene \"{0}\"", reader.Value));
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

        private static IEnumerable<CopyDirectoryInfo> ReadCopyList(XmlTextReader reader)
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
                            string dest = null;
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
                                    dest = reader.Value;
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
                                // Either the file specifies the Destination as well, or else CopyDirectory will use Source for Destination
                                var info = new CopyDirectoryInfo { Source = source };

                                if (dest != null)
                                {
                                    info.Destination = dest;
                                }

                                if (filter != null)
                                {
                                    info.Filter = filter;
                                }

                                info.Recursive = recursive;

                                Debug.Log(string.Format(CultureInfo.InvariantCulture, @"Build: Adding {0}copy ""{1}\{2}"" => ""{3}""", info.Recursive ? "Recursive " : "", info.Source, info.Filter, info.Destination ?? info.Source));

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

        public static void CopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath, CopyDirectoryInfo directoryInfo)
        {
            sourceDirectoryPath = Path.Combine(sourceDirectoryPath, directoryInfo.Source);
            destinationDirectoryPath = Path.Combine(destinationDirectoryPath, directoryInfo.Destination ?? directoryInfo.Source);

            Debug.Log(string.Format(CultureInfo.InvariantCulture, @"{0} ""{1}\{2}"" to ""{3}""", directoryInfo.Recursive ? "Recursively copying" : "Copying", sourceDirectoryPath, directoryInfo.Filter, destinationDirectoryPath));

            foreach (string sourceFilePath in Directory.GetFiles(sourceDirectoryPath, directoryInfo.Filter, directoryInfo.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                string destinationFilePath = sourceFilePath.Replace(sourceDirectoryPath, destinationDirectoryPath);
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                    if (File.Exists(destinationFilePath))
                    {
                        File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                    }
                    File.Copy(sourceFilePath, destinationFilePath, true);
                    File.SetAttributes(destinationFilePath, FileAttributes.Normal);
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format(CultureInfo.InvariantCulture, "Failed to copy \"{0}\" to \"{1}\" with \"{2}\"", sourceFilePath, destinationFilePath, exception));
                }
            }
        }

        private static void OnPreProcessBuild(BuildInfo buildInfo)
        {
            // Raise the global event for listeners
            BuildStarted.RaiseEvent(buildInfo);

            // Call the pre-build action, if any
            if (buildInfo.PreBuildAction != null)
            {
                buildInfo.PreBuildAction(buildInfo);
            }
        }

        private static void OnPostProcessBuild(BuildInfo buildInfo, string buildError)
        {
            if (string.IsNullOrEmpty(buildError))
            {
                if (buildInfo.CopyDirectories != null)
                {
                    string inputProjectDirectoryPath = GetProjectPath();
                    string outputProjectDirectoryPath = Path.Combine(GetProjectPath(), buildInfo.OutputDirectory);
                    foreach (var directory in buildInfo.CopyDirectories)
                    {
                        CopyDirectory(inputProjectDirectoryPath, outputProjectDirectoryPath, directory);
                    }
                }
            }

            // Raise the global event for listeners
            BuildCompleted.RaiseEvent(buildInfo, buildError);

            // Call the post-build action, if any
            if (buildInfo.PostBuildAction != null)
            {
                buildInfo.PostBuildAction(buildInfo, buildError);
            }
        }

        public static string GetProjectPath()
        {
            return Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
        }
    }
}
