// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// Class that exposes the MSBuild project generation operation.
    /// </summary>
    public static class MSBuildTools
    {
        private static readonly HashSet<BuildTarget> supportedBuildTargets = new HashSet<BuildTarget>()
        {
            BuildTarget.StandaloneWindows,
            BuildTarget.StandaloneWindows64,
            BuildTarget.iOS,
            BuildTarget.Android,
            BuildTarget.WSAPlayer
        };

        public const string CSharpVersion = "7.3";
        //public const string UWPTargetPlatformVersion = "10.0.18362.0";

        private static readonly string uwpMinPlatformVersion = EditorUserBuildSettings.wsaMinUWPSDK;// "10.0.14393.0";
        private static readonly string uwpTargetPlatformVersion = EditorUserBuildSettings.wsaUWPSDK;

        [MenuItem("MSBuild/Generate C# SDK Projects")]
        public static void GenerateSDKProjects()
        {
            // Create a copy of the packages as they might change after we create the MSBuild project
            MakePackagesCopy();

            Utilities.EnsureCleanDirectory(Application.dataPath.Replace("Assets", "MRTKBuild"));

            string path = Application.dataPath.Replace("Assets", "MSBuild");
            try
            {
                Utilities.EnsureCleanDirectory(path);
            }
            catch (IOException ex)
            {
                if (ex.Message.Contains(@".vs\MRTK\v15\Server\sqlite3\db.lock"))
                {
                    Debug.LogError("Generated project appears to be still open with Visual Studio.");
                    throw new InvalidDataException("Generated project appears to be still open with Visual Studio.", ex);
                }
                else
                {
                    throw;
                }
            }

            List<CompilationPlatformInfo> platforms = CompilationPipeline.GetAssemblyDefinitionPlatforms()
                .Where(t => supportedBuildTargets.Contains(t.BuildTarget))
                .Select(CompilationPlatformInfo.GetCompilationPlatform)
                .ToList();

            CompilationPlatformInfo editorPlatform = CompilationPlatformInfo.GetEditorPlatform();

            string propsOutputFolder = Application.dataPath.Replace("Assets", "MSBuild");
            CreateCommonPropsFile(platforms, editorPlatform, propsOutputFolder);
            UnityProjectInfo unityProjectInfo = new UnityProjectInfo(platforms);

            //// Read the solution template
            string solutionTemplateText = File.ReadAllText(Utilities.GetAssetsRelativePathFrom(TemplateFiles.Instance.MSBuildSolutionTemplatePath));

            //// Read the project template
            string projectTemplateText = File.ReadAllText(Utilities.GetAssetsRelativePathFrom(TemplateFiles.Instance.SDKProjectFileTemplatePath));

            unityProjectInfo.ExportSolution(solutionTemplateText, projectTemplateText, propsOutputFolder);

            foreach (string otherFile in TemplateFiles.Instance.OtherFiles)
            {
                File.Copy(Utilities.GetFullPathFromKnownRelative(otherFile), Path.Combine(propsOutputFolder, Path.GetFileName(otherFile)));
            }

            Debug.Log("Completed.");
        }

        private static void MakePackagesCopy()
        {
            string packageCache = Path.Combine(Application.dataPath, "..", "Library/PackageCache");
            string[] directories = Directory.GetDirectories(packageCache);

            string outputDirectory = Path.Combine(Application.dataPath, "..", Utilities.PackagesCopyFolderName);
            Utilities.EnsureCleanDirectory(outputDirectory);

            foreach (string directory in directories)
            {
                Utilities.CopyDirectory(directory, Path.Combine(outputDirectory, Path.GetFileName(directory).Split('@')[0]));
            }
        }

        private static void CreateCommonPropsFile(IEnumerable<CompilationPlatformInfo> availablePlatforms, CompilationPlatformInfo editorPlatform, string propsOutputFolder)
        {
            foreach (CompilationPlatformInfo platform in availablePlatforms)
            {
                // Check for specialized template, otherwise get the common one
                ProcessPlatformTemplateForConfiguration(platform, propsOutputFolder, true);
                ProcessPlatformTemplateForConfiguration(platform, propsOutputFolder, false);
            }

            ProcessPlatformTemplateForConfiguration(editorPlatform, propsOutputFolder, true);
        }

        private static void ProcessPlatformTemplateForConfiguration(CompilationPlatformInfo platform, string propsOutputFolder, bool inEditorConfiguration)
        {
            string configuration = inEditorConfiguration ? "InEditor" : "Player";

            string platformTemplate = File.ReadAllText(TemplateFiles.Instance.GetTemplateFilePathForPlatform(platform.Name, configuration));

            string platformPropsText;
            if (inEditorConfiguration)
            {
                platformPropsText = ProcessPlatformTemplate(platformTemplate, platform.Name, configuration, platform.BuildTarget, platform.TargetFramework,
                    platform.CommonPlatformReferences.Concat(platform.AdditionalInEditorReferences), platform.CommonPlatformDefines.Concat(platform.AdditionalInEditorDefines));
            }
            else
            {
                platformPropsText = ProcessPlatformTemplate(platformTemplate, platform.Name, configuration, platform.BuildTarget, platform.TargetFramework,
                    platform.CommonPlatformReferences.Concat(platform.AdditionalPlayerReferences), platform.CommonPlatformDefines.Concat(platform.AdditionalPlayerDefines));
            }

            File.WriteAllText(Path.Combine(propsOutputFolder, $"{platform.Name}.{configuration}.props"), platformPropsText);
        }

        private static string ProcessPlatformTemplate(string platformTemplate, string platformName, string configuration, BuildTarget buildTarget, TargetFramework targetFramework, IEnumerable<string> references, IEnumerable<string> defines, params HashSet<string>[] priorToCheck)
        {
            if (Utilities.TryGetXMLTemplate(platformTemplate, "PLATFORM_COMMON_REFERENCE", out string platformCommonReferenceTemplate))
            {
                ProcessReferences(buildTarget, references, out HashSet<string> platformAssemblySearchPaths, out HashSet<string> platformAssemblyReferencePaths, priorToCheck);

                string targetUWPPlatform = uwpTargetPlatformVersion;
                if (string.IsNullOrWhiteSpace(targetUWPPlatform))
                {
                    targetUWPPlatform = Utilities.GetUWPSDKs().Max().ToString(4);
                }

                Dictionary<string, string> platformTokens = new Dictionary<string, string>()
                {
                    {"<!--TARGET_FRAMEWORK_TOKEN-->", targetFramework.AsMSBuildString() },
                    {"<!--PLATFORM_COMMON_DEFINE_CONSTANTS-->", string.Join(";", defines) },
                    {"<!--PLATFORM_COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", platformAssemblySearchPaths)},

                    // These are UWP specific, but they will be no-op if not needed
                    { "<!--UWP_TARGET_PLATFORM_VERSION_TOKEN-->", uwpTargetPlatformVersion ?? "$(LatestTargetPlatformVersion)" },
                    { "<!--UWP_MIN_PLATFORM_VERSION_TOKEN-->", uwpMinPlatformVersion }
                };

                platformTokens.Add(platformCommonReferenceTemplate, string.Join("\r\n", GetReferenceEntries(platformCommonReferenceTemplate, platformAssemblyReferencePaths)));

                return Utilities.ReplaceTokens(platformTemplate, platformTokens);
            }
            else
            {
                Debug.LogError($"Invalid platform template format for '{platformName}' with configuration '{configuration}'");
                return platformTemplate;
            }
        }

        public static IEnumerable<string> GetReferenceEntries(string template, IEnumerable<string> references)
        {
            return references.Select(t => Utilities.ReplaceTokens(template, new Dictionary<string, string>()
            {
                { "##REFERENCE_TOKEN##", Path.GetFileNameWithoutExtension(t) },
                { "<!--HINT_PATH_TOKEN-->", t }
            }));
        }

        private static void ProcessReferences(BuildTarget buildTarget, IEnumerable<string> references, out HashSet<string> searchPaths, out HashSet<string> referenceNames, params HashSet<string>[] priorToCheck)
        {
            searchPaths = new HashSet<string>();
            referenceNames = new HashSet<string>();

            foreach (string reference in references)
            {
                string directory = Path.GetDirectoryName(reference);
                string fileName = Path.GetFileName(reference);
                if (!priorToCheck.Any(t => t.Contains(directory))) // Don't add duplicates
                {
                    searchPaths.Add(directory);
                }

                if (!referenceNames.Add(reference))
                {
                    Debug.LogError($"Duplicate assembly reference found for platform '{buildTarget}' - {reference} ignoring.");
                }
            }
        }
    }
}
#endif