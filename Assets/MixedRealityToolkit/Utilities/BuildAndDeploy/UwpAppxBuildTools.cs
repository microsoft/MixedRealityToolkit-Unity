// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Microsoft.MixedReality.Toolkit.Build.Editor
{
    public static class UwpAppxBuildTools
    {
        /// <summary>
        /// Query the build process to see if we're already building.
        /// </summary>
        public static bool IsBuilding { get; private set; } = false;

        /// <summary>
        /// Build the UWP appx bundle for this project.  Requires that <see cref="UwpPlayerBuildTools.BuildPlayer(string,bool,CancellationToken)"/> has already be run or a user has
        /// previously built the Unity Player with the WSA Player as the Build Target.
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>True, if the appx build was successful.</returns>
        public static async Task<bool> BuildAppxAsync(UwpBuildInfo buildInfo, CancellationToken cancellationToken = default)
        {
            if (!EditorAssemblyReloadManager.LockReloadAssemblies)
            {
                Debug.LogError("Lock Reload assemblies before attempting to build appx!");
                return false;
            }

            if (IsBuilding)
            {
                Debug.LogWarning("Build already in progress!");
                return false;
            }

            if (Application.isBatchMode)
            {
                // We don't need stack traces on all our logs. Makes things a lot easier to read.
                Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            }

            Debug.Log("Starting Unity Appx Build...");

            IsBuilding = true;
            string slnFilename = Path.Combine(buildInfo.OutputDirectory, $"{PlayerSettings.productName}.sln");

            if (!File.Exists(slnFilename))
            {
                Debug.LogError("Unable to find Solution to build from!");
                return IsBuilding = false;
            }

            // Get and validate the msBuild path...
            var msBuildPath = await FindMsBuildPathAsync();

            if (!File.Exists(msBuildPath))
            {
                Debug.LogError($"MSBuild.exe is missing or invalid!\n{msBuildPath}");
                return IsBuilding = false;
            }

            // Ensure that the generated .appx version increments by modifying Package.appxmanifest
            try
            {
                if (!UpdateAppxManifest(buildInfo))
                {
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to update appxmanifest!\n{e.Message}");
                return IsBuilding = false;
            }

            string storagePath = Path.GetFullPath(Path.Combine(Path.Combine(Application.dataPath, ".."), buildInfo.OutputDirectory));
            string solutionProjectPath = Path.GetFullPath(Path.Combine(storagePath, $@"{PlayerSettings.productName}.sln"));

            // Building the solution requires first restoring NuGet packages - when built through
            // Visual Studio, VS does this automatically - when building via msbuild like we're doing here,
            // we have to do that step manually.
            int exitCode = await Run(msBuildPath,
                $"\"{solutionProjectPath}\" /t:restore {GetMSBuildLoggingCommand(buildInfo.LogDirectory, "nugetRestore.log")}",
                !Application.isBatchMode,
                cancellationToken);
            if (exitCode != 0)
            {
                IsBuilding = false;
                return false;
            }

            // Now that NuGet packages have been restored, we can run the actual build process.
            exitCode = await Run(msBuildPath, 
                $"\"{solutionProjectPath}\" /t:{(buildInfo.RebuildAppx ? "Rebuild" : "Build")} /p:Configuration={buildInfo.Configuration} /p:Platform={buildInfo.BuildPlatform} {GetMSBuildLoggingCommand(buildInfo.LogDirectory, "buildAppx.log")}",
                !Application.isBatchMode,
                cancellationToken);
            AssetDatabase.SaveAssets();

            IsBuilding = false;
            return exitCode == 0;
        }

        private static async Task<int> Run(string fileName, string args, bool showDebug, CancellationToken cancellationToken)
        {
            Debug.Log($"Running command: {fileName} {args}");

            var processResult = await new Process().StartProcessAsync(
                fileName, args, !Application.isBatchMode, cancellationToken);

            switch (processResult.ExitCode)
            {
                case 0:
                    Debug.Log($"Command successful");

                    if (Application.isBatchMode)
                    {
                        Debug.Log(string.Join("\n", processResult.Output));
                    }
                    break;
                case -1073741510:
                    Debug.LogWarning("The build was terminated either by user's keyboard input CTRL+C or CTRL+Break or closing command prompt window.");
                    break;
                default:
                    {
                        if (processResult.ExitCode != 0)
                        {
                            Debug.Log($"Command failed, errorCode: {processResult.ExitCode}");

                            if (Application.isBatchMode)
                            {
                                var output = "Command output:\n";

                                foreach (var message in processResult.Output)
                                {
                                    output += $"{message}\n";
                                }

                                output += "Command errors:";

                                foreach (var error in processResult.Errors)
                                {
                                    output += $"{error}\n";
                                }

                                Debug.LogError(output);
                            }
                        }
                        break;
                    }
            }
            return processResult.ExitCode;
        }

        private static async Task<string> FindMsBuildPathAsync()
        {
            // Finding msbuild.exe involves different work depending on whether or not users
            // have VS2017 or VS2019 installed.
            foreach (VSWhereFindOption findOption in VSWhereFindOptions)
            {
                var result = await new Process().StartProcessAsync(
                new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = findOption.arguments,
                    WorkingDirectory = @"C:\Program Files (x86)\Microsoft Visual Studio\Installer"
                });

                foreach (var path in result.Output)
                {
                    if (!string.IsNullOrEmpty(path))
                    {
                        string[] paths = path.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (paths.Length > 0)
                        {
                            // if there are multiple visual studio installs,
                            // prefer enterprise, then pro, then community
                            string bestPath = paths.OrderBy(p => p.ToLower().Contains("enterprise"))
                                .ThenBy(p => p.ToLower().Contains("professional"))
                                .ThenBy(p => p.ToLower().Contains("community")).First();

                            string finalPath = $@"{bestPath}{findOption.pathSuffix}";
                            if (File.Exists(finalPath))
                            {
                                return finalPath;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        private static bool UpdateAppxManifest(IBuildInfo buildInfo)
        {
            string manifestFilePath = GetManifestFilePath(buildInfo);
            if (manifestFilePath == null)
            {
                // Error has already been logged
                return false;
            }

            var rootNode = XElement.Load(manifestFilePath);
            var identityNode = rootNode.Element(rootNode.GetDefaultNamespace() + "Identity");

            if (identityNode == null)
            {
                Debug.LogError($"Package.appxmanifest for build (in path - {manifestFilePath}) is missing an <Identity /> node");
                return false;
            }

            var dependencies = rootNode.Element(rootNode.GetDefaultNamespace() + "Dependencies");

            if (dependencies == null)
            {
                Debug.LogError($"Package.appxmanifest for build (in path - {manifestFilePath}) is missing <Dependencies /> node.");
                return false;
            }

            UpdateDependenciesElement(dependencies, rootNode.GetDefaultNamespace());

            // The gaze input capability might already exist - this is okay, it will
            // only add it if required and it's not already present.
            var uwpBuildInfo = buildInfo as UwpBuildInfo;
            if (uwpBuildInfo != null && uwpBuildInfo.GazeInputCapabilityEnabled)
            {
                AddGazeInputCapability(rootNode);
            }

            // We use XName.Get instead of string -> XName implicit conversion because
            // when we pass in the string "Version", the program doesn't find the attribute.
            // Best guess as to why this happens is that implicit string conversion doesn't set the namespace to empty
            var versionAttr = identityNode.Attribute(XName.Get("Version"));

            if (versionAttr == null)
            {
                Debug.LogError($"Package.appxmanifest for build (in path - {manifestFilePath}) is missing a Version attribute in the <Identity /> node.");
                return false;
            }

            // Assume package version always has a '.' between each number.
            // According to https://msdn.microsoft.com/en-us/library/windows/apps/br211441.aspx
            // Package versions are always of the form Major.Minor.Build.Revision.
            // Note: Revision number reserved for Windows Store, and a value other than 0 will fail WACK.
            var version = PlayerSettings.WSA.packageVersion;
            var newVersion = new Version(version.Major, version.Minor, buildInfo.AutoIncrement ? version.Build + 1 : version.Build, version.Revision);

            PlayerSettings.WSA.packageVersion = newVersion;
            versionAttr.Value = newVersion.ToString();
            rootNode.Save(manifestFilePath);
            return true;
        }

        /// <summary>
        /// Gets the AppX manifest path in the project output directory.
        /// </summary>
        private static string GetManifestFilePath(IBuildInfo buildInfo)
        {
            var fullPathOutputDirectory = Path.GetFullPath(buildInfo.OutputDirectory);
            Debug.Log($"Searching for appx manifest in {fullPathOutputDirectory}...");

            // Find the manifest, assume the one we want is the first one
            string[] manifests = Directory.GetFiles(fullPathOutputDirectory, "Package.appxmanifest", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError($"Unable to find Package.appxmanifest file for build (in path - {fullPathOutputDirectory})");
                return null;
            }

            if (manifests.Length > 1)
            {
                Debug.LogWarning("Found more than one appxmanifest in the target build folder!");
            }

            return manifests[0];
        }

        private static void UpdateDependenciesElement(XElement dependencies, XNamespace defaultNamespace)
        {
            var values = (PlayerSettings.WSATargetFamily[])Enum.GetValues(typeof(PlayerSettings.WSATargetFamily));

            if (string.IsNullOrWhiteSpace(EditorUserBuildSettings.wsaUWPSDK))
            {
                var windowsSdkPaths = Directory.GetDirectories(@"C:\Program Files (x86)\Windows Kits\10\Lib");

                for (int i = 0; i < windowsSdkPaths.Length; i++)
                {
                    windowsSdkPaths[i] = windowsSdkPaths[i].Substring(windowsSdkPaths[i].LastIndexOf(@"\", StringComparison.Ordinal) + 1);
                }

                EditorUserBuildSettings.wsaUWPSDK = windowsSdkPaths[windowsSdkPaths.Length - 1];
            }

            string maxVersionTested = EditorUserBuildSettings.wsaUWPSDK;

            if (string.IsNullOrWhiteSpace(EditorUserBuildSettings.wsaMinUWPSDK))
            {
                EditorUserBuildSettings.wsaMinUWPSDK = UwpBuildDeployPreferences.MIN_SDK_VERSION.ToString();
            }

            string minVersion = EditorUserBuildSettings.wsaMinUWPSDK;

            // Clear any we had before.
            dependencies.RemoveAll();

            foreach (PlayerSettings.WSATargetFamily family in values)
            {
                if (PlayerSettings.WSA.GetTargetDeviceFamily(family))
                {
                    dependencies.Add(
                        new XElement(defaultNamespace + "TargetDeviceFamily",
                        new XAttribute("Name", $"Windows.{family}"),
                        new XAttribute("MinVersion", minVersion),
                        new XAttribute("MaxVersionTested", maxVersionTested)));
                }
            }

            if (!dependencies.HasElements)
            {
                dependencies.Add(
                    new XElement(defaultNamespace + "TargetDeviceFamily",
                    new XAttribute("Name", "Windows.Universal"),
                    new XAttribute("MinVersion", minVersion),
                    new XAttribute("MaxVersionTested", maxVersionTested)));
            }
        }

        /// Gets the subpart of the msbuild.exe command to save log information
        /// in the given logFileName.
        /// </summary>
        /// <remarks>
        /// Will return an empty string if logDirectory is not set.
        /// </remarks>
        private static string GetMSBuildLoggingCommand(string logDirectory, string logFileName)
        {
            if (String.IsNullOrEmpty(logDirectory))
            {
                Debug.Log($"Not logging {logFileName} because no logDirectory was provided");
                return "";
            }

            return $"-fl -flp:logfile={Path.Combine(logDirectory, logFileName)};verbosity=detailed";
        }

        /// <summary>
        /// Adds the 'Gaze Input' capability to the manifest.
        /// </summary>
        /// <remarks>
        /// This is a workaround for versions of Unity which don't have native support
        /// for the 'Gaze Input' capability in its Player Settings preference location.
        /// Note that this function is only public to poke a hole for testing - do not
        /// take a dependency on this function.
        /// </remarks>
        public static void AddGazeInputCapability(XElement rootNode)
        {
            // If the capabilities container tag is missing, make sure it gets added.
            var capabilitiesTag = rootNode.GetDefaultNamespace() + "Capabilities";
            XElement capabilitiesNode = rootNode.Element(capabilitiesTag);
            if (capabilitiesNode == null)
            {
                capabilitiesNode = new XElement(capabilitiesTag);
                rootNode.Add(capabilitiesNode);
            }

            var gazeInputCapability = rootNode.GetDefaultNamespace() + "DeviceCapability";
            XElement existingGazeInputCapability = capabilitiesNode.Elements(gazeInputCapability)
                .FirstOrDefault(element => element.Attribute("Name")?.Value == "gazeInput");

            // Only add the capability if isn't there already.
            if (existingGazeInputCapability == null)
            {
                capabilitiesNode.Add(
                    new XElement(gazeInputCapability, new XAttribute("Name", "gazeInput")));
            }
        }

        /// <summary>
        /// An overload of AddGazeInputCapability that will read the AppX manifest from
        /// the build output and update the manifest file with the gazeInput capability.
        /// </summary>
        /// <param name="buildInfo">An IBuildInfo containing a valid OutputDirectory</param>
        public static void AddGazeInputCapability(IBuildInfo buildInfo)
        {
            string manifestFilePath = GetManifestFilePath(buildInfo);
            if (manifestFilePath == null)
            {
                throw new FileNotFoundException("Unable to find manifest file");
            }

            var rootElement = XElement.Load(manifestFilePath);
            AddGazeInputCapability(rootElement);
            rootElement.Save(manifestFilePath);
        }

        /// <summary>
        /// This struct controls the behavior of the arguments that are used
        /// when finding msbuild.exe.
        /// </summary>
        private struct VSWhereFindOption
        {
            public VSWhereFindOption(string args, string suffix)
            {
                arguments = args;
                pathSuffix = suffix;
            }

            /// <summary>
            /// Used to populate the Arguments of ProcessStartInfo when invoking
            /// vswhere.
            /// </summary>
            public string arguments;

            /// <summary>
            /// This string is added as a suffix to the result of the vswhere path
            /// search.
            /// </summary>
            public string pathSuffix;
        }

        private static VSWhereFindOption[] VSWhereFindOptions =
        {
            // This find option corresponds to the version of vswhere that ships with VS2019.
            new VSWhereFindOption(
                $@"/C vswhere -all -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe",
                ""),
            // This find option corresponds to the versin of vswhere that ships with VS2017 - this doesn't have
            // support for the -find command switch.
            new VSWhereFindOption(
                $@"/C vswhere -all -products * -requires Microsoft.Component.MSBuild -property installationPath",
                "\\MSBuild\\15.0\\Bin\\MSBuild.exe"),
        };
    }
}