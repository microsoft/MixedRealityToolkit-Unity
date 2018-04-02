// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using MixedRealityToolkit.Common.EditorScript;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MixedRealityToolkit.Build
{
    public class UwpAppxBuildTools
    {
        private const string VsInstallerPath = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer";

        /// <summary>
        /// Query the build process to see if we're already building.
        /// </summary>
        public static bool IsBuilding { get; private set; } = false;

        private static bool CheckBuildScenes()
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
        /// Do a build configured for UWP Applications to the specified path, returns the error from <see cref="UwpPlayerBuildTools.BuildUwpPlayer"/>
        /// </summary>
        /// <returns>True, if build was successful.</returns>
        public static bool BuildUnityPlayer(string buildDirectory, bool showDialog = true)
        {
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
                PostBuildAction = async (innerBuildInfo, buildError) =>
                {
                    if (!string.IsNullOrEmpty(buildError))
                    {
                        EditorUtility.DisplayDialog($"{PlayerSettings.productName} WindowsStoreApp Build Failed!", buildError, "OK");
                    }
                    else
                    {
                        if (showDialog)
                        {

                            if (!EditorUtility.DisplayDialog(PlayerSettings.productName, "Build Complete", "OK", "Build AppX"))
                            {
                                EditorAssemblyReloadManager.LockReloadAssemblies = true;
                                await BuildAppxAsync(
                                     PlayerSettings.productName,
                                     BuildDeployPreferences.ForceRebuild,
                                     BuildDeployPreferences.BuildConfig,
                                     BuildDeployPreferences.BuildPlatform,
                                     BuildDeployPreferences.BuildDirectory,
                                     BuildDeployPreferences.IncrementBuildVersion);
                                EditorAssemblyReloadManager.LockReloadAssemblies = false;
                            }
                        }

                        buildSuccess = true;
                    }
                }
            };

            UwpPlayerBuildTools.RaiseOverrideBuildDefaults(ref buildInfo);
            UwpPlayerBuildTools.BuildUwpPlayer(buildInfo);

            return buildSuccess;
        }

        /// <summary>
        /// Build the UWP appx bundle for this project.  Requires that <see cref="BuildUnityPlayer"/> has already be run or a user has
        /// previously built the Unity Player with the WSA Player as the Build Target.
        /// </summary>
        /// <param name="productName">The applications product name. Typically <see cref="PlayerSettings.productName"/></param>
        /// <param name="forceRebuildAppx">Should we force rebuild the appx bundle?</param>
        /// <param name="buildConfig">Debug, Release, or Master configurations are valid.</param>
        /// <param name="buildPlatform">x86 or x64 build platforms are valid.</param>
        /// <param name="buildDirectory">The directory where the built Unity Player Solution is located.</param>
        /// <param name="incrementVersion">Should we increment the appx version number?</param>
        /// <returns></returns>
        public static async Task<bool> BuildAppxAsync(string productName, bool forceRebuildAppx, string buildConfig, string buildPlatform, string buildDirectory, bool incrementVersion)
        {
            if (IsBuilding)
            {
                Debug.LogWarning("Build already in progress!");
                return false;
            }

            IsBuilding = true;
            string slnFilename = Path.Combine(buildDirectory, $"{PlayerSettings.productName}.sln");

            if (!File.Exists(slnFilename))
            {
                Debug.LogError("Unable to find Solution to build from!");
                return IsBuilding = false;
            }

            // Get and validate the msBuild path...
            var msBuildPath = await FindMsBuildPathAsync();

            if (!File.Exists(msBuildPath))
            {
                Debug.LogError("MSBuild.exe is missing or invalid!");
                return IsBuilding = false;
            }

            // Get the path to the NuGet tool
            string unity = Path.GetDirectoryName(EditorApplication.applicationPath);
            System.Diagnostics.Debug.Assert(unity != null, "Unable to determine the unity editor path.");
            string storePath = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), buildDirectory));
            string solutionProjectPath = Path.GetFullPath(Path.Combine(storePath, $@"{productName}.sln"));

            // Bug in Unity editor that doesn't copy project.json and project.lock.json files correctly if solutionProjectPath is not in a folder named UWP.
            if (!File.Exists($"{storePath}\\project.json"))
            {
                File.Copy($@"{unity}\Data\PlaybackEngines\MetroSupport\Tools\project.json", $"{storePath}\\project.json");
            }

            string nugetPath = Path.Combine(unity, @"Data\PlaybackEngines\MetroSupport\Tools\NuGet.exe");
            string assemblyCSharp = $"{storePath}/GeneratedProjects/UWP/Assembly-CSharp";
            string assemblyCSharpFirstPass = $"{storePath}/GeneratedProjects/UWP/Assembly-CSharp-firstpass";
            bool restoreFirstPass = Directory.Exists(assemblyCSharpFirstPass);

            // Before building, need to run a nuget restore to generate a json.lock file. Failing to do
            // this breaks the build in VS RTM
            if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) == ScriptingImplementation.WinRTDotNET &&
                (!RestoreNugetPackages(nugetPath, storePath) ||
                 !RestoreNugetPackages(nugetPath, $"{storePath}\\{productName}") ||
                 EditorUserBuildSettings.wsaGenerateReferenceProjects && !RestoreNugetPackages(nugetPath, assemblyCSharp) ||
                 EditorUserBuildSettings.wsaGenerateReferenceProjects && restoreFirstPass && !RestoreNugetPackages(nugetPath, assemblyCSharpFirstPass)))
            {
                Debug.LogError("Failed to restore nuget packages!");
                return IsBuilding = false;
            }

            // Ensure that the generated .appx version increments by modifying Package.appxmanifest
            if (incrementVersion && !IncrementPackageVersion())
            {
                Debug.LogError("Failed to increment package version!");
                return IsBuilding = false;
            }

            // Now do the actual build
            var pInfo = new ProcessStartInfo
            {
                FileName = msBuildPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Arguments = $"\"{solutionProjectPath}\" /t:{(forceRebuildAppx ? "Rebuild" : "Build")} /p:Configuration={buildConfig} /p:Platform={buildPlatform} /verbosity:m"
            };

            int exitCode = await BuildAppxAsync(pInfo);

            if (exitCode == 0)
            {
                Debug.Log("Appx Build Successful!");
            }
            else if (exitCode == -1073741510)
            {
                Debug.LogWarning("The build was terminated either by user's keyboard input CTRL+C or CTRL+Break or closing command prompt window.");
            }
            else if (exitCode != 0)
            {
                Debug.LogError($"{PlayerSettings.productName} appx build Failed! (ErrorCode: {exitCode})");
            }

            IsBuilding = false;
            return exitCode == 0;
        }

        private static async Task<string> FindMsBuildPathAsync()
        {
            // For MSBuild 15+ we should to use vswhere to give us the correct instance
            string output = $@"/C vswhere -version 15.0 -products * -requires Microsoft.Component.MSBuild -property installationPath";

            var vswherePInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                Arguments = output,
                WorkingDirectory = VsInstallerPath
            };

            using (var vswhereP = new Process())
            {
                vswhereP.StartInfo = vswherePInfo;
                vswhereP.Start();
                output = await vswhereP.StandardOutput.ReadToEndAsync();
                vswhereP.WaitForExit();
            }

            string[] paths = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            if (paths.Length > 0)
            {
                // if there are multiple visual studio installs,
                // prefer enterprise, then pro, then community
                string bestPath = paths.OrderBy(p => p.ToLower().Contains("enterprise"))
                                        .ThenBy(p => p.ToLower().Contains("professional"))
                                        .ThenBy(p => p.ToLower().Contains("community")).First();

                return $@"{bestPath}\MSBuild\15.0\Bin\MSBuild.exe";
            }

            Debug.LogError("Unable to find a valid path to a Visual Studio Instance!");
            return string.Empty;
        }

        private static bool RestoreNugetPackages(string nugetPath, string storePath)
        {
            Debug.Assert(File.Exists(nugetPath));
            Debug.Assert(Directory.Exists(storePath));

            var nugetPInfo = new ProcessStartInfo
            {
                FileName = nugetPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = $"restore \"{storePath}/project.json\""
            };

            using (var nugetP = new Process())
            {
                nugetP.StartInfo = nugetPInfo;
                nugetP.Start();
                nugetP.WaitForExit();
                nugetP.Close();
                nugetP.Dispose();
            }

            return File.Exists($"{storePath}\\project.lock.json");
        }

        private static bool IncrementPackageVersion()
        {
            // Find the manifest, assume the one we want is the first one
            string[] manifests = Directory.GetFiles(BuildDeployPreferences.AbsoluteBuildDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError($"Unable to find Package.appxmanifest file for build (in path - {BuildDeployPreferences.AbsoluteBuildDirectory})");
                return false;
            }

            string manifest = manifests[0];
            var rootNode = XElement.Load(manifest);
            var identityNode = rootNode.Element(rootNode.GetDefaultNamespace() + "Identity");

            if (identityNode == null)
            {
                Debug.LogError($"Package.appxmanifest for build (in path - {BuildDeployPreferences.AbsoluteBuildDirectory}) is missing an <Identity /> node");
                return false;
            }

            // We use XName.Get instead of string -> XName implicit conversion because
            // when we pass in the string "Version", the program doesn't find the attribute.
            // Best guess as to why this happens is that implicit string conversion doesn't set the namespace to empty
            var versionAttr = identityNode.Attribute(XName.Get("Version"));

            if (versionAttr == null)
            {
                Debug.LogError($"Package.appxmanifest for build (in path - {BuildDeployPreferences.AbsoluteBuildDirectory}) is missing a version attribute in the <Identity /> node.");
                return false;
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
            return true;
        }

        private static async Task<int> BuildAppxAsync(ProcessStartInfo pInfo)
        {
            // there is no non-generic TaskCompletionSource
            var tcs = new TaskCompletionSource<int>();

            var process = new Process
            {
                StartInfo = pInfo,
                EnableRaisingEvents = true
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Close();
                process.Dispose();
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Debug.LogError(args.Data);
                }
            };

            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    Debug.Log(args.Data);
                }
            };

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            return await tcs.Task;
        }
    }
}
