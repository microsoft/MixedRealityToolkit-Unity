// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using System.Collections.Generic;
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
        /// The list of filename extensions that are valid VCProjects.
        /// </summary>
        private static readonly string[] VcProjExtensions = { "vcsproj", "vcxproj" };

        /// <summary>
        /// Build the UWP appx bundle for this project.  Requires that <see cref="UwpPlayerBuildTools.BuildPlayer(string,bool,CancellationToken)"/> has already be run or a user has
        /// previously built the Unity Player with the WSA Player as the Build Target.
        /// </summary>
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

            int exitCode;

            // Building the solution requires first restoring NuGet packages - when built through
            // Visual Studio, VS does this automatically - when building via msbuild like we're doing here,
            // we have to do that step manually.
            // We use msbuild for nuget restore by default, but if a path to nuget.exe is supplied then we use that executable
            if (string.IsNullOrEmpty(buildInfo.NugetExecutablePath))
            {
                exitCode = await Run(msBuildPath,
                $"\"{solutionProjectPath}\" /t:restore {GetMSBuildLoggingCommand(buildInfo.LogDirectory, "nugetRestore.log")}",
                !Application.isBatchMode,
                cancellationToken);
            }
            else
            {
                exitCode = await Run(buildInfo.NugetExecutablePath,
                $"restore \"{solutionProjectPath}\"",
                !Application.isBatchMode,
                cancellationToken);
            }
            
            if (exitCode != 0)
            {
                IsBuilding = false;
                return false;
            }

            // Need to add ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch to MixedRealityToolkit.vcxproj
            if (buildInfo.BuildPlatform == "arm64")
            {
                if (!UpdateVSProj(buildInfo))
                {
                    return IsBuilding = false;
                }
            }

            // Now that NuGet packages have been restored, we can run the actual build process.
            exitCode = await Run(msBuildPath,
                $"\"{solutionProjectPath}\" {(buildInfo.Multicore ? "/m /nr:false" : "")} /t:{(buildInfo.RebuildAppx ? "Rebuild" : "Build")} /p:Configuration={buildInfo.Configuration} /p:Platform={buildInfo.BuildPlatform} {(string.IsNullOrEmpty(buildInfo.PlatformToolset) ? string.Empty : $"/p:PlatformToolset={buildInfo.PlatformToolset}")} {GetMSBuildLoggingCommand(buildInfo.LogDirectory, "buildAppx.log")}",
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
                string arguments = findOption.arguments;
                if (string.IsNullOrWhiteSpace(EditorUserBuildSettings.wsaUWPVisualStudioVersion))
                {
                    arguments += " -latest";
                }
                else
                {
                    // Add version number with brackets to find only the specified version
                    arguments += $" -version [{EditorUserBuildSettings.wsaUWPVisualStudioVersion}]";
                }

                var result = await new Process().StartProcessAsync(
                new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = arguments,
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
                            string bestPath = paths.OrderByDescending(p => p.ToLower().Contains("enterprise"))
                                .ThenByDescending(p => p.ToLower().Contains("professional"))
                                .ThenByDescending(p => p.ToLower().Contains("community")).First();

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

        private static bool UpdateVSProj(IBuildInfo buildInfo)
        {
            // For ARM64 builds we need to add ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch
            // to vcxproj file in order to ensure that the build passes
            string projectFilePath = GetProjectFilePath(buildInfo);
            if (projectFilePath == null)
            {
                return false;
            }

            var rootNode = XElement.Load(projectFilePath);
            var defaultNamespace = rootNode.GetDefaultNamespace();
            var propertyGroupNode = rootNode.Element(defaultNamespace + "PropertyGroup");

            if (propertyGroupNode == null)
            {
                propertyGroupNode = new XElement(defaultNamespace + "PropertyGroup", new XAttribute("Label", "Globals"));
                rootNode.Add(propertyGroupNode);
            }

            var newNode = propertyGroupNode.Element(defaultNamespace + "ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch");
            if (newNode != null)
            {
                // If this setting already exists in the project, ensure its value is "None"
                newNode.Value = "None";
            }
            else
            {
                propertyGroupNode.Add(new XElement(defaultNamespace + "ResolveAssemblyWarnOrErrorOnTargetArchitectureMismatch", "None"));
            }

            rootNode.Save(projectFilePath);

            return true;
        }

        /// <summary>
        /// Given the project name and build path, resolves the valid VcProject file (i.e. .vcsproj, vcxproj)
        /// </summary>
        /// <returns>A valid path if the project file exists, null otherwise</returns>
        private static string GetProjectFilePath(IBuildInfo buildInfo)
        {
            string projectName = PlayerSettings.productName;
            foreach (string extension in VcProjExtensions)
            {
                string projectFilePath = Path.Combine(Path.GetFullPath(buildInfo.OutputDirectory), projectName, $"{projectName}.{extension}");
                if (File.Exists(projectFilePath))
                {
                    return projectFilePath;
                }
            }

            string projectDirectory = Path.Combine(Path.GetFullPath(buildInfo.OutputDirectory), projectName);
            string combinedExtensions = String.Join("|", VcProjExtensions);
            Debug.LogError($"Cannot find project file {projectDirectory} given names {projectName}.{combinedExtensions}");
            return null;
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
            AddCapabilities(buildInfo, rootNode);

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
            // According to https://msdn.microsoft.com/library/windows/apps/br211441.aspx
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

        /// <summary>
        /// Updates 'Assembly-CSharp.csproj' file according to the values set in buildInfo.
        /// </summary>
        /// <param name="buildInfo">An IBuildInfo containing a valid OutputDirectory</param>
        /// <remarks>Only used with the .NET backend in Unity 2018 or older, with Unity C# Projects enabled.</remarks>
        public static void UpdateAssemblyCSharpProject(IBuildInfo buildInfo)
        {
#if !UNITY_2019_1_OR_NEWER
            if (!EditorUserBuildSettings.wsaGenerateReferenceProjects ||
                PlayerSettings.GetScriptingBackend(BuildTargetGroup.WSA) != ScriptingImplementation.WinRTDotNET)
            {
                // Assembly-CSharp.csproj is only generated when the above is true
                return;
            }

            string projectFilePath = GetAssemblyCSharpProjectFilePath(buildInfo);
            if (projectFilePath == null)
            {
                throw new FileNotFoundException("Unable to find 'Assembly-CSharp.csproj' file.");
            }

            var rootElement = XElement.Load(projectFilePath);
            var uwpBuildInfo = buildInfo as UwpBuildInfo;
            Debug.Assert(uwpBuildInfo != null);

            if (uwpBuildInfo.AllowUnsafeCode)
            {
                AllowUnsafeCode(rootElement);
            }

            rootElement.Save(projectFilePath);
#endif // !UNITY_2019_1_OR_NEWER
        }

        /// <summary>
        /// Gets the 'Assembly-CSharp.csproj' files path in the project output directory.
        /// </summary>
        private static string GetAssemblyCSharpProjectFilePath(IBuildInfo buildInfo)
        {
            var fullPathOutputDirectory = Path.GetFullPath(buildInfo.OutputDirectory);
            Debug.Log($"Searching for 'Assembly-CSharp.csproj' in {fullPathOutputDirectory}...");

            // Find the manifest, assume the one we want is the first one
            string[] manifests = Directory.GetFiles(fullPathOutputDirectory, "Assembly-CSharp.csproj", SearchOption.AllDirectories);

            if (manifests.Length == 0)
            {
                Debug.LogError($"Unable to find 'Assembly-CSharp.csproj' file for build (in path - {fullPathOutputDirectory})");
                return null;
            }

            if (manifests.Length > 1)
            {
                Debug.LogWarning("Found more than one 'Assembly-CSharp.csproj' in the target build folder!");
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
                EditorUserBuildSettings.wsaMinUWPSDK = UwpBuildDeployPreferences.MIN_PLATFORM_VERSION.ToString();
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
        /// Adds capabilities according to the values in the buildInfo to the manifest file.
        /// </summary>
        /// <param name="buildInfo">An IBuildInfo containing a valid OutputDirectory and all capabilities</param>
        public static void AddCapabilities(IBuildInfo buildInfo, XElement rootElement = null)
        {
            var manifestFilePath = GetManifestFilePath(buildInfo);
            if (manifestFilePath == null)
            {
                throw new FileNotFoundException("Unable to find manifest file");
            }

            rootElement = rootElement ?? XElement.Load(manifestFilePath);
            var uwpBuildInfo = buildInfo as UwpBuildInfo;

            Debug.Assert(uwpBuildInfo != null);
            if (uwpBuildInfo.DeviceCapabilities != null)
            {
                AddCapabilities(rootElement, uwpBuildInfo.DeviceCapabilities);
            }
            if (uwpBuildInfo.GazeInputCapabilityEnabled)
            {
                AddGazeInputCapability(rootElement);
            }

            if (uwpBuildInfo.ResearchModeCapabilityEnabled && EditorUserBuildSettings.wsaSubtarget == WSASubtarget.HoloLens)
            {
                AddResearchModeCapability(rootElement);
            }

            rootElement.Save(manifestFilePath);
        }

        /// <summary>
        /// Adds a capability to the given rootNode, which must be the read AppX manifest from
        /// the build output.
        /// </summary>
        /// <param name="rootNode">An XElement containing the AppX manifest from 
        /// the build output</param>
        /// <param name="capability">The added capabilities tag as XName</param>
        /// <param name="value">Value of the Name-XAttribute of the added capability</param>
        public static void AddCapability(XElement rootNode, XName capability, string value)
        {
            // If the capabilities container tag is missing, make sure it gets added.
            var capabilitiesTag = rootNode.GetDefaultNamespace() + "Capabilities";
            XElement capabilitiesNode = rootNode.Element(capabilitiesTag);
            if (capabilitiesNode == null)
            {
                capabilitiesNode = new XElement(capabilitiesTag);
                rootNode.Add(capabilitiesNode);
            }

            XElement existingCapability = capabilitiesNode.Elements(capability)
                .FirstOrDefault(element => element.Attribute("Name")?.Value == value);

            // Only add the capability if it isn't there already.
            if (existingCapability == null)
            {
                capabilitiesNode.Add(
                    new XElement(capability, new XAttribute("Name", value)));
            }
        }

        /// <summary>
        /// Adds the 'Gaze Input' capability to the manifest.
        /// </summary>
        /// <remarks>
        /// <para>This is a workaround for versions of Unity which don't have native support
        /// for the 'Gaze Input' capability in its Player Settings preference location.
        /// Note that this function is only public to poke a hole for testing - do not
        /// take a dependency on this function.</para>
        /// </remarks>
        public static void AddGazeInputCapability(XElement rootNode)
        {
            AddCapability(rootNode, rootNode.GetDefaultNamespace() + "DeviceCapability", "gazeInput");
        }

        /// <summary>
        /// Adds the given capabilities to the manifest.
        /// </summary>
        public static void AddCapabilities(XElement rootNode, List<string> capabilities)
        {
            foreach (string capability in capabilities)
            {
                AddCapability(rootNode, rootNode.GetDefaultNamespace() + "DeviceCapability", capability);
            }
        }

        /// <summary>
        /// Adds the 'Research Mode' capability to the manifest.
        /// </summary>
        /// <remarks>
        /// <para>This is only for research projects and should not be used in production.
        /// For further information take a look at https://docs.microsoft.com/windows/mixed-reality/research-mode.
        /// Note that this function is only public to poke a hole for testing - do not
        /// take a dependency on this function.</para>
        /// </remarks>
        public static void AddResearchModeCapability(XElement rootNode)
        {
            // Add rescap Namespace to package tag
            XNamespace rescapNs = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities";
            var rescapAttribute = rootNode.Attribute(XNamespace.Xmlns + "rescap");
            if (rescapAttribute == null)
            {
                rescapAttribute = new XAttribute(XNamespace.Xmlns + "rescap", rescapNs);
                rootNode.Add(rescapAttribute);
            }

            // Add rescap to IgnorableNamespaces
            var ignNsAttribute = rootNode.Attribute("IgnorableNamespaces");
            if (ignNsAttribute == null)
            {
                ignNsAttribute = new XAttribute("IgnorableNamespaces", "rescap");
                rootNode.Add(ignNsAttribute);
            }

            if (!ignNsAttribute.Value.Contains("rescap"))
            {
                ignNsAttribute.Value += " rescap";
            }

            AddCapability(rootNode, rescapNs + "Capability", "perceptionSensorsExperimental");
        }

        /// <summary>
        /// Enables unsafe code in the generated Assembly-CSharp project.
        /// </summary>
        /// <remarks>
        /// <para>This is not required by the research mode, but not using unsafe code with
        /// direct memory access results in poor performance. So it is recommended
        /// to use unsafe code to an extent.</para>
        /// <para>For further information take a look at https://docs.microsoft.com/windows/mixed-reality/research-mode. </para>
        /// <para>Note that this function is only public to poke a hole for testing - do not
        /// take a dependency on this function.</para>
        /// </remarks>
        public static void AllowUnsafeCode(XElement rootNode)
        {
            foreach (XElement propertyGroupNode in rootNode.Descendants(rootNode.GetDefaultNamespace() + "PropertyGroup"))
            {
                if (propertyGroupNode.Attribute("Condition") != null)
                {
                    var allowUnsafeBlocks = propertyGroupNode.Element(propertyGroupNode.GetDefaultNamespace() + "AllowUnsafeBlocks");
                    if (allowUnsafeBlocks == null)
                    {
                        allowUnsafeBlocks = new XElement(propertyGroupNode.GetDefaultNamespace() + "AllowUnsafeBlocks");
                        propertyGroupNode.Add(allowUnsafeBlocks);
                    }
                    allowUnsafeBlocks.Value = "true";
                }
            }
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

        private static readonly VSWhereFindOption[] VSWhereFindOptions =
        {
            // This find option corresponds to the version of vswhere that ships with VS2019.
            new VSWhereFindOption(
                @"/C vswhere -all -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe",
                ""),
            // This find option corresponds to the version of vswhere that ships with VS2017 - this doesn't have
            // support for the -find command switch.
            new VSWhereFindOption(
                @"/C vswhere -all -products * -requires Microsoft.Component.MSBuild -property installationPath",
                "\\MSBuild\\15.0\\Bin\\MSBuild.exe"),
        };
    }
}
