// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Win32;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Contains utility functions for building for the device
    /// </summary>
    public class BuildDeployTools
    {
        public const string DefaultMSBuildVersion = "15.0";

        public static bool CanBuild()
        {
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.IL2CPP && IsIl2CppAvailable())
            {
                return true;
            }

            return PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET && IsDotNetAvailable();
        }

        public static bool IsDotNetAvailable()
        {
            return Directory.Exists(EditorApplication.applicationContentsPath + "\\PlaybackEngines\\MetroSupport\\Managed\\UAP");
        }

        public static bool IsIl2CppAvailable()
        {
            return Directory.Exists(EditorApplication.applicationContentsPath + "\\PlaybackEngines\\MetroSupport\\Managed\\il2cpp");
        }

        /// <summary>
        /// Displays a dialog if no scenes are present in the build and returns true if build can proceed.
        /// </summary>
        /// <returns></returns>
        public static bool CheckBuildScenes()
        {
            if (EditorBuildSettings.scenes.Length == 0)
            {
                return EditorUtility.DisplayDialog("Attention!",
                    "No scenes are present in the build settings!\n\n Do you want to cancel and add one?",
                    "Continue Anyway", "Cancel Build");
            }

            return true;
        }

        /// <summary>
        /// Do a build configured for Mixed Reality Applications, returns the error from BuildPipeline.BuildPlayer
        /// </summary>
        public static bool BuildSLN()
        {
            return BuildSLN(BuildDeployPrefs.BuildDirectory, false);
        }

        public static bool BuildSLN(string buildDirectory, bool showDialog = true)
        {
            // Use BuildSLNUtilities to create the SLN
            bool buildSuccess = false;

            if (CheckBuildScenes() == false)
            {
                return false;
            }

            var buildInfo = new BuildInfo
            {
                // These properties should all match what the Standalone.proj file specifies
                OutputDirectory = buildDirectory,
                Scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path),
                BuildTarget = BuildTarget.WSAPlayer,
                WSASdk = WSASDK.UWP,
                WSAUWPBuildType = EditorUserBuildSettings.wsaUWPBuildType,
                WSAUwpSdk = EditorUserBuildSettings.wsaUWPSDK,

                // Configure a post build action that will compile the generated solution
                PostBuildAction = (innerBuildInfo, buildError) =>
                {
                    if (!string.IsNullOrEmpty(buildError))
                    {
                        EditorUtility.DisplayDialog(PlayerSettings.productName + " WindowsStoreApp Build Failed!", buildError, "OK");
                    }
                    else
                    {
                        if (showDialog)
                        {
                            if (!EditorUtility.DisplayDialog(PlayerSettings.productName, "Build Complete", "OK", "Build AppX"))
                            {
                                BuildAppxFromSLN(
                                    PlayerSettings.productName,
                                    BuildDeployPrefs.MsBuildVersion,
                                    BuildDeployPrefs.ForceRebuild,
                                    BuildDeployPrefs.BuildConfig,
                                    BuildDeployPrefs.BuildPlatform,
                                    BuildDeployPrefs.BuildDirectory,
                                    BuildDeployPrefs.IncrementBuildVersion);
                            }
                        }

                        buildSuccess = true;
                    }
                }
            };

            BuildSLNUtilities.RaiseOverrideBuildDefaults(ref buildInfo);

            BuildSLNUtilities.PerformBuild(buildInfo);

            return buildSuccess;
        }

        public static string CalcMSBuildPath(string msBuildVersion)
        {
            if (msBuildVersion.Equals("14.0"))
            {
                using (RegistryKey key =
                    Registry.LocalMachine.OpenSubKey(
                        string.Format(@"Software\Microsoft\MSBuild\ToolsVersions\{0}", msBuildVersion)))
                {
                    if (key != null)
                    {
                        var msBuildBinFolder = (string)key.GetValue("MSBuildToolsPath");
                        return Path.Combine(msBuildBinFolder, "msbuild.exe");
                    }
                }
            }

            // If we got this far then we don't have VS 2015 installed and need to use msBuild 15
            msBuildVersion = "15.0";

            // For MSBuild 15+ we should to use vswhere to give us the correct instance
            string output = @"/C vswhere -version " + msBuildVersion + " -products * -requires Microsoft.Component.MSBuild -property installationPath";

            // get the right program files path based on whether the PC is x86 or x64
            string programFiles = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer";

            var vswherePInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                Arguments = output,
                WorkingDirectory = programFiles
            };

            using (var vswhereP = new Process())
            {
                vswhereP.StartInfo = vswherePInfo;
                vswhereP.Start();
                output = vswhereP.StandardOutput.ReadToEnd();
                vswhereP.WaitForExit();
            }

            string[] paths = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (paths.Length > 0)
            {
                // if there are multiple 2017 installs,
                // prefer enterprise, then pro, then community
                string bestPath = paths.OrderBy(p => p.ToLower().Contains("enterprise"))
                                        .ThenBy(p => p.ToLower().Contains("professional"))
                                        .ThenBy(p => p.ToLower().Contains("community")).First();

                return bestPath + @"\MSBuild\" + msBuildVersion + @"\Bin\MSBuild.exe";
            }

            Debug.LogError("Unable to find a valid path to Visual Studio Instance!");
            return string.Empty;
        }

        public static bool RestoreNugetPackages(string nugetPath, string storePath)
        {
            Debug.Assert(File.Exists(nugetPath));
            Debug.Assert(Directory.Exists(storePath));

            var nugetPInfo = new ProcessStartInfo
            {
                FileName = nugetPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = "restore \"" + storePath + "/project.json\""
            };

            using (var nugetP = new Process())
            {
                nugetP.StartInfo = nugetPInfo;
                nugetP.Start();
                nugetP.WaitForExit();
                nugetP.Close();
                nugetP.Dispose();
            }

            return File.Exists(storePath + "\\project.lock.json");
        }

        public static bool BuildAppxFromSLN(string productName, string msBuildVersion, bool forceRebuildAppx, string buildConfig, string buildPlatform, string buildDirectory, bool incrementVersion, bool showDialog = true)
        {
            EditorUtility.DisplayProgressBar("Build AppX", "Building AppX Package...", 0);
            string slnFilename = Path.Combine(buildDirectory, PlayerSettings.productName + ".sln");

            if (!File.Exists(slnFilename))
            {
                Debug.LogError("Unable to find Solution to build from!");
                EditorUtility.ClearProgressBar();
                return false;
            }

            // Get and validate the msBuild path...
            var msBuildPath = CalcMSBuildPath(msBuildVersion);

            if (!File.Exists(msBuildPath))
            {
                Debug.LogErrorFormat("MSBuild.exe is missing or invalid (path={0}).", msBuildPath);
                EditorUtility.ClearProgressBar();
                return false;
            }

            // Get the path to the NuGet tool
            string unity = Path.GetDirectoryName(EditorApplication.applicationPath);
            System.Diagnostics.Debug.Assert(unity != null, "unity != null");
            string storePath = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), buildDirectory));
            string solutionProjectPath = Path.GetFullPath(Path.Combine(storePath, productName + @".sln"));

            // Bug in Unity editor that doesn't copy project.json and project.lock.json files correctly if solutionProjectPath is not in a folder named UWP.
            if (!File.Exists(storePath + "\\project.json"))
            {
                File.Copy(unity + @"\Data\PlaybackEngines\MetroSupport\Tools\project.json", storePath + "\\project.json");
            }

            string nugetPath = Path.Combine(unity, @"Data\PlaybackEngines\MetroSupport\Tools\NuGet.exe");
            string assemblyCSharp = storePath + "/GeneratedProjects/UWP/Assembly-CSharp";
            string assemblyCSharpFirstPass = storePath + "/GeneratedProjects/UWP/Assembly-CSharp-firstpass";
            bool restoreFirstPass = Directory.Exists(assemblyCSharpFirstPass);

            // Before building, need to run a nuget restore to generate a json.lock file. Failing to do
            // this breaks the build in VS RTM
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET &&
                (!RestoreNugetPackages(nugetPath, storePath) ||
                 !RestoreNugetPackages(nugetPath, storePath + "\\" + productName) ||
                 EditorUserBuildSettings.wsaGenerateReferenceProjects && !RestoreNugetPackages(nugetPath, assemblyCSharp) ||
                 EditorUserBuildSettings.wsaGenerateReferenceProjects && restoreFirstPass && !RestoreNugetPackages(nugetPath, assemblyCSharpFirstPass)))
            {
                Debug.LogError("Failed to restore nuget packages");
                EditorUtility.ClearProgressBar();
                return false;
            }

            EditorUtility.DisplayProgressBar("Build AppX", "Building AppX Package...", 25);

            // Ensure that the generated .appx version increments by modifying
            // Package.appxmanifest
            if (incrementVersion)
            {
                IncrementPackageVersion();
            }

            // Now do the actual build
            var pInfo = new ProcessStartInfo
            {
                FileName = msBuildPath,
                CreateNoWindow = false,
                Arguments = string.Format("\"{0}\" /t:{1} /p:Configuration={2} /p:Platform={3} /verbosity:m",
                    solutionProjectPath,
                    forceRebuildAppx ? "Rebuild" : "Build",
                    buildConfig,
                    buildPlatform)
            };

            // Uncomment out to debug by copying into command window
            //Debug.Log("\"" + vs + "\"" + " " + pInfo.Arguments);

            var process = new Process { StartInfo = pInfo };

            try
            {
                if (!process.Start())
                {
                    Debug.LogError("Failed to start Cmd process!");
                    EditorUtility.ClearProgressBar();
                    return false;
                }

                process.WaitForExit();

                EditorUtility.ClearProgressBar();

                if (process.ExitCode == 0 &&
                    showDialog &&
                    !EditorUtility.DisplayDialog("Build AppX", "AppX Build Successful!", "OK", "Open AppX Folder"))
                {
                    Process.Start("explorer.exe", "/f /open," + Path.GetFullPath(BuildDeployPrefs.BuildDirectory + "/" + PlayerSettings.productName + "/AppPackages"));
                }

                if (process.ExitCode != 0)
                {
                    Debug.LogError("MSBuild error (code = " + process.ExitCode + ")");
                    EditorUtility.DisplayDialog(PlayerSettings.productName + " build Failed!", "Failed to build appx from solution. Error code: " + process.ExitCode, "OK");
                    return false;
                }

                process.Close();
                process.Dispose();

            }
            catch (Exception e)
            {
                Debug.LogError("Cmd Process EXCEPTION: " + e);
                EditorUtility.ClearProgressBar();
                return false;
            }

            return true;
        }

        private static void IncrementPackageVersion()
        {
            // Find the manifest, assume the one we want is the first one
            string[] manifests = Directory.GetFiles(BuildDeployPrefs.AbsoluteBuildDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError("Unable to find Package.appxmanifest file for build (in path - " + BuildDeployPrefs.AbsoluteBuildDirectory + ")");
                return;
            }

            string manifest = manifests[0];
            var rootNode = XElement.Load(manifest);
            var identityNode = rootNode.Element(rootNode.GetDefaultNamespace() + "Identity");

            if (identityNode == null)
            {
                Debug.LogError("Package.appxmanifest for build (in path - " + BuildDeployPrefs.AbsoluteBuildDirectory + ") is missing an <Identity /> node");
                return;
            }

            // We use XName.Get instead of string -> XName implicit conversion because
            // when we pass in the string "Version", the program doesn't find the attribute.
            // Best guess as to why this happens is that implicit string conversion doesn't set the namespace to empty
            var versionAttr = identityNode.Attribute(XName.Get("Version"));

            if (versionAttr == null)
            {
                Debug.LogError("Package.appxmanifest for build (in path - " + BuildDeployPrefs.AbsoluteBuildDirectory + ") is missing a version attribute in the <Identity /> node.");
                return;
            }

            // Assume package version always has a '.' between each number.
            // According to https://msdn.microsoft.com/en-us/library/windows/apps/br211441.aspx
            // Package versions are always of the form Major.Minor.Build.Revision.
            // Note: Revision number reserved for Windows Store, and a value other than 0 will fail WACK.
            var version = PlayerSettings.WSA.packageVersion;
            var newVersion = new Version(version.Major, version.Minor, version.Build + 1, version.Revision);

            PlayerSettings.WSA.packageVersion = newVersion;
            versionAttr.Value = newVersion.ToString();
            rootNode.Save(manifest);
        }
    }
}
