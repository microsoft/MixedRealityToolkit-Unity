//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Xml.Linq;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Contains utility functions for building for the device
    /// </summary>
    public class BuildDeployTools
    {
        public static readonly string DefaultMSBuildVersion = "14.0";

        public static bool BuildSLN(string buildDirectory, bool showDialog = true)
        {
            // Use BuildSLNUtilities to create the SLN
            bool buildSuccess = false;

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
                using (Microsoft.Win32.RegistryKey key =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                        string.Format(@"Software\Microsoft\MSBuild\ToolsVersions\{0}", msBuildVersion)))
                {
                    if (key == null)
                    {
                        return null;
                    }

                    var msBuildBinFolder = (string)key.GetValue("MSBuildToolsPath");
                    return Path.Combine(msBuildBinFolder, "msbuild.exe");
                }
            }

            // For MSBuild 15+ we should to use vswhere to give us the correct instance
            string output = @"/C cd ""%ProgramFiles(x86)%\Microsoft Visual Studio\Installer"" && vswhere -version " + msBuildVersion + " -products * -requires Microsoft.Component.MSBuild -property installationPath";

            var vswherePInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                Arguments = output
            };

            using (var vswhereP = new System.Diagnostics.Process())
            {
                vswhereP.StartInfo = vswherePInfo;
                vswhereP.Start();
                output = vswhereP.StandardOutput.ReadToEnd();
                vswhereP.WaitForExit();
                vswhereP.Close();
                vswhereP.Dispose();
            }

            output = output + @"\MSBuild\" + msBuildVersion + @"\Bin\MSBuild.exe";
            output = output.Replace(Environment.NewLine, "");
            return output;
        }

        public static bool RestoreNugetPackages(string nugetPath, string storePath)
        {
            var nugetPInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = nugetPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = "restore \"" + storePath + "/project.json\""
            };

            using (var nugetP = new System.Diagnostics.Process())
            {
                nugetP.StartInfo = nugetPInfo;
                nugetP.Start();
                nugetP.WaitForExit();
                nugetP.Close();
                nugetP.Dispose();
            }

            return File.Exists(storePath + "\\project.lock.json");
        }

        public static bool BuildAppxFromSLN(string productName, string msBuildVersion, bool forceRebuildAppx, string buildConfig, string buildDirectory, bool incrementVersion, bool showDialog = true)
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
            var vs = CalcMSBuildPath(msBuildVersion);

            if (!File.Exists(vs))
            {
                Debug.LogError("MSBuild.exe is missing or invalid (path=" + vs + "). Note that the default version is " + DefaultMSBuildVersion);
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

            // Before building, need to run a nuget restore to generate a json.lock file. Failing to do
            // this breaks the build in VS RTM
            if (!RestoreNugetPackages(nugetPath, storePath) ||
                !RestoreNugetPackages(nugetPath, storePath + "\\" + productName) ||
                EditorUserBuildSettings.wsaGenerateReferenceProjects && !RestoreNugetPackages(nugetPath, storePath + "/GeneratedProjects/UWP/Assembly-CSharp") ||
                EditorUserBuildSettings.wsaGenerateReferenceProjects && !RestoreNugetPackages(nugetPath, storePath + "/GeneratedProjects/UWP/Assembly-CSharp-firstpass"))
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
            var pInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = vs,
                CreateNoWindow = false,
                Arguments = string.Format("\"{0}\" /t:{2} /p:Configuration={1} /p:Platform=x86 /verbosity:m",
                    solutionProjectPath,
                    buildConfig,
                    forceRebuildAppx ? "Rebuild" : "Build")
            };

            // Uncomment out to debug by copying into command window
            //Debug.Log("\"" + vs + "\"" + " " + pInfo.Arguments);

            var process = new System.Diagnostics.Process { StartInfo = pInfo };

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
                    !EditorUtility.DisplayDialog("Build AppX", "AppX Build Successful!", "OK", "Open Project Folder"))
                {
                    System.Diagnostics.Process.Start("explorer.exe", "/select," + storePath);
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

            // Assume package version always has a '.'.
            // According to https://msdn.microsoft.com/en-us/library/windows/apps/br211441.aspx
            // Package versions are always of the form Major.Minor.Build.Revision
            var version = new Version(versionAttr.Value);
            var newVersion = new Version(version.Major, version.Minor, version.Build, version.Revision + 1);

            versionAttr.Value = newVersion.ToString();
            rootNode.Save(manifest);
        }
    }
}
