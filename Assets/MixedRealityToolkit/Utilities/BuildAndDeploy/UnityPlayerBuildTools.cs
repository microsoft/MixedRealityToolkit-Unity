// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    /// <summary>
    /// Cross platform player build tools
    /// </summary>
    public static class UnityPlayerBuildTools
    {
        // Build configurations. Exactly one of these should be defined for any given build.
        public const string BuildSymbolDebug = "debug";
        public const string BuildSymbolRelease = "release";
        public const string BuildSymbolMaster = "master";

        /// <summary>
        /// Starts the build process
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <returns>The <see href="https://docs.unity3d.com/ScriptReference/Build.Reporting.BuildReport.html">BuildReport</see> from Unity's <see href="https://docs.unity3d.com/ScriptReference/BuildPipeline.html">BuildPipeline</see></returns>
        public static BuildReport BuildUnityPlayer(IBuildInfo buildInfo)
        {
            EditorUtility.DisplayProgressBar("Build Pipeline", "Gathering Build Data...", 0.25f);

            // Call the pre-build action, if any
            buildInfo.PreBuildAction?.Invoke(buildInfo);

            BuildTargetGroup buildTargetGroup = buildInfo.BuildTarget.GetGroup();
            string playerBuildSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            if (!string.IsNullOrEmpty(playerBuildSymbols))
            {
                if (buildInfo.HasConfigurationSymbol())
                {
                    buildInfo.AppendWithoutConfigurationSymbols(playerBuildSymbols);
                }
                else
                {
                    buildInfo.AppendSymbols(playerBuildSymbols.Split(';'));
                }
            }

            if (!string.IsNullOrEmpty(buildInfo.BuildSymbols))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, buildInfo.BuildSymbols);
            }

            if ((buildInfo.BuildOptions & BuildOptions.Development) == BuildOptions.Development &&
                !buildInfo.HasConfigurationSymbol())
            {
                buildInfo.AppendSymbols(BuildSymbolDebug);
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

            var oldColorSpace = PlayerSettings.colorSpace;

            if (buildInfo.ColorSpace.HasValue)
            {
                PlayerSettings.colorSpace = buildInfo.ColorSpace.Value;
            }

            if (buildInfo.ScriptingBackend.HasValue)
            {
                PlayerSettings.SetScriptingBackend(buildTargetGroup, buildInfo.ScriptingBackend.Value);

#if !UNITY_2019_1_OR_NEWER
                // When building the .NET backend, also build the C# projects, as the
                // intent of this build process is to prove that it's possible build
                // a solution where the local dev loop can be accomplished in the
                // generated C# projects.
                if (buildInfo.ScriptingBackend == ScriptingImplementation.WinRTDotNET)
                {
                    EditorUserBuildSettings.wsaGenerateReferenceProjects = true;
                }
#endif
            }

            BuildTarget oldBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            BuildTargetGroup oldBuildTargetGroup = oldBuildTarget.GetGroup();

            if (EditorUserBuildSettings.activeBuildTarget != buildInfo.BuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildInfo.BuildTarget);
            }

            switch (buildInfo.BuildTarget)
            {
                case BuildTarget.Android:
                    buildInfo.OutputDirectory = $"{buildInfo.OutputDirectory}/{PlayerSettings.productName}.apk";
                    break;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    buildInfo.OutputDirectory = $"{buildInfo.OutputDirectory}/{PlayerSettings.productName}.exe";
                    break;
            }

            BuildReport buildReport = default;

            try
            {
                buildReport = BuildPipeline.BuildPlayer(
                    buildInfo.Scenes.ToArray(),
                    buildInfo.OutputDirectory,
                    buildInfo.BuildTarget,
                    buildInfo.BuildOptions);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }

            PlayerSettings.colorSpace = oldColorSpace;

            if (EditorUserBuildSettings.activeBuildTarget != oldBuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(oldBuildTargetGroup, oldBuildTarget);
            }

            // Call the post-build action, if any
            buildInfo.PostBuildAction?.Invoke(buildInfo, buildReport);

            return buildReport;
        }

        /// <summary>
        /// Force Unity To Write Project Files
        /// </summary>
        public static void SyncSolution()
        {
            var syncVs = Type.GetType("UnityEditor.SyncVS,UnityEditor");
            var syncSolution = syncVs.GetMethod("SyncSolution", BindingFlags.Public | BindingFlags.Static);
            syncSolution.Invoke(null, null);
        }

        /// <summary>
        /// Start a build using Unity's command line.
        /// </summary>
        public static async void StartCommandLineBuild(bool keepApplicationAlive = false)
        {
            // We don't need stack traces on all our logs. Makes things a lot easier to read.
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Debug.Log($"Starting command line build for {EditorUserBuildSettings.activeBuildTarget}...");
            EditorAssemblyReloadManager.LockReloadAssemblies = true;

            bool success;
            try
            {
                SyncSolution();
                switch (EditorUserBuildSettings.activeBuildTarget)
                {
                    case BuildTarget.WSAPlayer:
                        success = await UwpPlayerBuildTools.BuildPlayer(new UwpBuildInfo(true));
                        break;
                    default:
                        var buildInfo = new BuildInfo(true) as IBuildInfo;
                        ParseBuildCommandLine(ref buildInfo);
                        var buildResult = BuildUnityPlayer(buildInfo);
                        success = buildResult.summary.result == BuildResult.Succeeded;
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Build Failed!\n{e.Message}\n{e.StackTrace}");
                success = false;
            }

            Debug.Log($"Exiting command line build... Build success? {success}");
            if (keepApplicationAlive) return; // leave function here, without closing the editor
            EditorApplication.Exit(success ? 0 : 1);
        }

        internal static bool CheckBuildScenes()
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                return EditorUtility.DisplayDialog("Attention!",
                                                   "No scenes are present in the build settings.\n" +
                                                   "The current scene will be the one built.\n\n" +
                                                   "Do you want to cancel and add one?",
                                                   "Continue Anyway", "Cancel Build");
            }

            return true;
        }

        /// <summary>
        /// Get the Unity Project Root Path.
        /// </summary>
        /// <returns>The full path to the project's root.</returns>
        public static string GetProjectPath()
        {
            return Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
        }

        public static void ParseBuildCommandLine(ref IBuildInfo buildInfo)
        {
            string[] arguments = Environment.GetCommandLineArgs();

            for (int i = 0; i < arguments.Length; ++i)
            {
                switch (arguments[i])
                {
                    case "-autoIncrement":
                        buildInfo.AutoIncrement = true;
                        break;
                    case "-sceneList":
                        buildInfo.Scenes = buildInfo.Scenes.Union(SplitSceneList(arguments[++i]));
                        break;
                    case "-sceneListFile":
                        string path = arguments[++i];
                        if (File.Exists(path))
                        {
                            buildInfo.Scenes = buildInfo.Scenes.Union(SplitSceneList(File.ReadAllText(path)));
                        }
                        else
                        {
                            Debug.LogWarning($"Scene list file at '{path}' does not exist.");
                        }
                        break;
                    case "-buildOutput":
                        buildInfo.OutputDirectory = arguments[++i];
                        break;
                    case "-colorSpace":
                        buildInfo.ColorSpace = (ColorSpace)Enum.Parse(typeof(ColorSpace), arguments[++i]);
                        break;
                    case "-scriptingBackend":
                        buildInfo.ScriptingBackend = (ScriptingImplementation)Enum.Parse(typeof(ScriptingImplementation), arguments[++i]);
                        break;
                    case "-x86":
                    case "-x64":
                    case "-arm":
                        buildInfo.BuildPlatform = arguments[i].Substring(1);
                        break;
                    case "-debug":
                    case "-master":
                    case "-release":
                        buildInfo.Configuration = arguments[i].Substring(1).ToLower();
                        break;
                    case "-logDirectory":
                        buildInfo.LogDirectory = arguments[++i];
                        break;
                }
            }
        }

        private static IEnumerable<string> SplitSceneList(string sceneList)
        {
            return from scene in sceneList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   select scene.Trim();
        }

        /// <summary>
        /// Restores any nuget packages at the path specified.
        /// </summary>
        /// <param name="nugetPath"></param>
        /// <param name="storePath"></param>
        /// <returns>True, if the nuget packages were successfully restored.</returns>
        public static async Task<bool> RestoreNugetPackagesAsync(string nugetPath, string storePath)
        {
            Debug.Assert(File.Exists(nugetPath));
            Debug.Assert(Directory.Exists(storePath));

            await new Process().StartProcessAsync(nugetPath, $"restore \"{storePath}/project.json\"");

            return File.Exists($"{storePath}\\project.lock.json");
        }
    }
}