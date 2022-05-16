// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// A helper class to parse the state of the current Unity project.
    /// </summary>
    public class UnityProjectInfo
    {
        /// <summary>
        /// These package references are excluded depending on the Unity version as certain assemblies are only supported in specific versions of Unity.
        /// </summary>
        private static readonly HashSet<string> ExcludedPackageReferences = new HashSet<string>()
        {
#if !UNITY_2019_3_OR_NEWER
            "Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus",
            "Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.Editor",
            "Microsoft.MixedReality.Toolkit.Providers.XRSDK.Oculus.Handtracking.Editor",
            "Microsoft.MixedReality.Toolkit.Providers.XRSDK.WindowsMixedReality",
            "Microsoft.MixedReality.Toolkit.Providers.XRSDK",
            "Microsoft.MixedReality.Toolkit.SDK.Experimental.Interactive",
            "Microsoft.MixedReality.Toolkit.SDK.Experimental.Editor.Interactive"
#endif
#if UNITY_2020_2_OR_NEWER
            "Microsoft.MixedReality.Toolkit.Services.BoundarySystem",
            "Microsoft.MixedReality.Toolkit.Providers.WindowsMixedReality"
#endif
        };

        /// <summary>
        /// Gets the name of this Unity Project.
        /// </summary>
        public string UnityProjectName { get; }

        /// <summary>
        /// Gets the available platforms for this Unity project.
        /// </summary>
        internal IEnumerable<CompilationPlatformInfo> AvailablePlatforms { get; }

        /// <summary>
        /// Gets all the parsed CSProjects for this Unity project.
        /// </summary>
        public IReadOnlyDictionary<string, CSProjectInfo> CSProjects { get; }

        /// <summary>
        /// Gets all the parsed DLLs for this Unity project.
        /// </summary>
        public IReadOnlyCollection<PluginAssemblyInfo> Plugins { get; }

        /// <summary>
        /// Starting from Unity 2019 some plugins are shipped with Unity in its source form. These plugins need to be handled specially.
        /// </summary>
        public static IReadOnlyDictionary<string, string> SpecialPluginNameMappingUnity2019 { get; } = new Dictionary<string, string>
        {
            { "Unity.ugui" , "UnityEngine.UI" },
            { "Unity.ugui.Editor" , "UnityEditor.UI" }
        };

        public UnityProjectInfo(IEnumerable<CompilationPlatformInfo> availablePlatforms, string projectOutputPath)
        {
            AvailablePlatforms = availablePlatforms;

            UnityProjectName = Application.productName;

            if (string.IsNullOrWhiteSpace(UnityProjectName))
            {
                UnityProjectName = "UnityProject";
            }

            Plugins = new ReadOnlyCollection<PluginAssemblyInfo>(ScanForPluginDLLs());

            foreach (PluginAssemblyInfo plugin in Plugins)
            {
                if (plugin.Type == PluginType.Native)
                {
                    Debug.Log($"Native plugin {plugin.ReferencePath.AbsolutePath} not yet supported for MSBuild project.");
                }
            }

            CSProjects = new ReadOnlyDictionary<string, CSProjectInfo>(CreateUnityProjects(projectOutputPath));
        }

        private Dictionary<string, CSProjectInfo> CreateUnityProjects(string projectOutputPath)
        {
            // Not all of these will be converted to C# objects, only the ones found to be referenced
            Dictionary<string, AssemblyDefinitionInfo> asmDefInfoMap = new Dictionary<string, AssemblyDefinitionInfo>();
            HashSet<string> builtInPackagesWithoutSource = new HashSet<string>();

            // Parse the builtInPackagesFirst
            DirectoryInfo builtInPackagesDirectory = new DirectoryInfo(Utilities.BuiltInPackagesPath);
            foreach (DirectoryInfo packageDirectory in builtInPackagesDirectory.GetDirectories())
            {
                FileInfo[] asmDefFiles = packageDirectory.GetFiles("*.asmdef", SearchOption.AllDirectories);

                if (asmDefFiles.Length == 0)
                {
                    builtInPackagesWithoutSource.Add(packageDirectory.Name.ToLower());
                    continue;
                }

                foreach (FileInfo fileInfo in asmDefFiles)
                {
                    AssemblyDefinitionInfo assemblyDefinitionInfo = AssemblyDefinitionInfo.Parse(fileInfo, this, null, true);
                    asmDefInfoMap.Add(Path.GetFileNameWithoutExtension(fileInfo.Name), assemblyDefinitionInfo);
                }
            }

            Dictionary<string, Assembly> unityAssemblies = CompilationPipeline.GetAssemblies().ToDictionary(t => t.name);
            Dictionary<string, CSProjectInfo> projectsMap = new Dictionary<string, CSProjectInfo>();
            Queue<string> projectsToProcess = new Queue<string>();
            // Parse the unity assemblies
            foreach (KeyValuePair<string, Assembly> pair in unityAssemblies)
            {
                if (!asmDefInfoMap.TryGetValue(pair.Key, out AssemblyDefinitionInfo assemblyDefinitionInfo))
                {
                    string asmDefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(pair.Key);
                    if (string.IsNullOrEmpty(asmDefPath))
                    {
                        if (!pair.Key.StartsWith("Assembly-CSharp"))
                        {
                            throw new InvalidOperationException($"Failed to retrieve AsmDef for script assembly: {pair.Key}");
                        }

                        assemblyDefinitionInfo = AssemblyDefinitionInfo.GetDefaultAssemblyCSharpInfo(pair.Value);
                        projectsToProcess.Enqueue(pair.Key);
                    }
                    else
                    {
                        assemblyDefinitionInfo = AssemblyDefinitionInfo.Parse(new FileInfo(Utilities.GetFullPathFromKnownRelative(asmDefPath)), this, pair.Value);

                        if (asmDefPath.StartsWith("Assets/"))
                        {
                            // Add as mandatory
                            projectsToProcess.Enqueue(pair.Key);
                        }
                    }

                    asmDefInfoMap.Add(pair.Key, assemblyDefinitionInfo);
                }
            }

            while (projectsToProcess.Count > 0)
            {
                string projectKey = projectsToProcess.Dequeue();

                if (!projectsMap.ContainsKey(projectKey))
                {
                    GetProjectInfo(projectsMap, asmDefInfoMap, builtInPackagesWithoutSource, projectKey, projectOutputPath);
                }
            }

            // Ignore test projects when generating docs with Unity 2019
#if UNITY_2019_3_OR_NEWER
            projectsMap.Remove("Microsoft.MixedReality.Toolkit.Tests.EditModeTests");
            projectsMap.Remove("Microsoft.MixedReality.Toolkit.Tests.PlayModeTests");
#endif
            return projectsMap;
        }

        private CSProjectInfo GetProjectInfo(Dictionary<string, CSProjectInfo> projectsMap, Dictionary<string, AssemblyDefinitionInfo> asmDefInfoMap, HashSet<string> builtInPackagesWithoutSource, string projectKey, string projectOutputPath)
        {
#if UNITY_2019_3_OR_NEWER
            if (SpecialPluginNameMappingUnity2019.TryGetValue(projectKey, out string pluginName))
            {
                projectKey = pluginName;
            }
#endif
            if (projectsMap.TryGetValue(projectKey, out CSProjectInfo value))
            {
                return value;
            }

            if (!asmDefInfoMap.TryGetValue(projectKey, out AssemblyDefinitionInfo assemblyDefinitionInfo))
            {
                Debug.Log($"Can't find an asmdef for project: {projectKey}, skipping.");
                return null;
            }

            CSProjectInfo toReturn = new CSProjectInfo(this, assemblyDefinitionInfo, projectOutputPath);
            projectsMap.Add(projectKey, toReturn);

            if (!assemblyDefinitionInfo.BuiltInPackage)
            {
                foreach (PluginAssemblyInfo plugin in Plugins.Where(t => t.Type != PluginType.Native))
                {
                    if (plugin.AutoReferenced || assemblyDefinitionInfo.PrecompiledAssemblyReferences.Contains(plugin.Name))
                    {
                        toReturn.AddDependency(plugin);
                    }
                }
            }

            foreach (string reference in toReturn.AssemblyDefinitionInfo.References)
            {
                if (ExcludedPackageReferences.Contains(reference))
                {
                    Debug.LogWarning($"Skipping processing {reference} for {toReturn.Name}, as it's marked as excluded.");
                    continue;
                }

                string packageCandidate = $"com.{reference.ToLower()}";
                if (builtInPackagesWithoutSource.Any(t => packageCandidate.StartsWith(t)))
                {
                    Debug.LogWarning($"Skipping processing {reference} for {toReturn.Name}, as it's a built-in package without source.");
                    continue;
                }

                toReturn.AddDependency(GetProjectInfo(projectsMap, asmDefInfoMap, builtInPackagesWithoutSource, reference, projectOutputPath));
            }

            // Manually add special plugin dependencies to the projects
#if UNITY_2019_3_OR_NEWER
            if (toReturn.Name.StartsWith("Microsoft.MixedReality.Toolkit") || toReturn.Name.StartsWith("Unity.TextMeshPro"))
            {
                string[] plugins = SpecialPluginNameMappingUnity2019.Values.OrderByDescending(p => p).ToArray();
                foreach (var plugin in plugins)
                {
                    if (projectsMap.TryGetValue(plugin, out CSProjectInfo projectInfo))
                    {
                        toReturn.AddDependency(projectInfo);
                    }
                    else
                    {
                        CSProjectInfo newProjInfo = new CSProjectInfo(this, asmDefInfoMap[plugin], projectOutputPath);
                        if (plugin == plugins[1])
                        {
                            newProjInfo.AddDependency(projectsMap[plugins[0]]);
                        }
                        projectsMap.Add(plugin, newProjInfo);
                        toReturn.AddDependency(newProjInfo);
                    }
                }
            }
#endif
            return toReturn;
        }

        private List<PluginAssemblyInfo> ScanForPluginDLLs()
        {
            List<PluginAssemblyInfo> toReturn = new List<PluginAssemblyInfo>();

            foreach (string assetAssemblyPath in Directory.GetFiles(Utilities.AssetPath, "*.dll", SearchOption.AllDirectories))
            {
                string assetRelativePath = Utilities.GetAssetsRelativePathFrom(assetAssemblyPath);
                PluginImporter importer = (PluginImporter)AssetImporter.GetAtPath(assetRelativePath);
                PluginAssemblyInfo toAdd = new PluginAssemblyInfo(this, Guid.Parse(AssetDatabase.AssetPathToGUID(assetRelativePath)), assetAssemblyPath, importer.isNativePlugin ? PluginType.Native : PluginType.Managed);
                toReturn.Add(toAdd);
            }

            foreach (string packageDllPath in Directory.GetFiles(Utilities.PackagesCopyPath, "*.dll", SearchOption.AllDirectories))
            {
                string metaPath = packageDllPath + ".meta";

                if (!File.Exists(metaPath))
                {
                    Debug.LogWarning($"Skipping a packages DLL that didn't have an associated meta: '{packageDllPath}'");
                    continue;
                }
                Guid guid;
                using (StreamReader reader = new StreamReader(metaPath))
                {
                    string guidLine = reader.ReadUntil("guid");
                    if (!Guid.TryParse(guidLine.Split(':')[1].Trim(), out guid))
                    {
                        Debug.LogWarning($"Skipping a packages DLL that didn't have a valid guid in the .meta file: '{packageDllPath}'");
                        continue;
                    }
                }

                bool isManaged = Utilities.IsManagedAssembly(packageDllPath);
                PluginAssemblyInfo toAdd = new PluginAssemblyInfo(this, guid, packageDllPath, isManaged ? PluginType.Managed : PluginType.Native);
                toReturn.Add(toAdd);
            }

            return toReturn;
        }

        private string GetProjectEntry(CSProjectInfo projectInfo, string projectEntryTemplateBody)
        {
            StringBuilder toReturn = new StringBuilder();
            toReturn.AppendLine(Utilities.ReplaceTokens(projectEntryTemplateBody, new Dictionary<string, string>() {
                        { "<PROJECT_NAME>", projectInfo.Name },
                        { "<PROJECT_RELATIVE_PATH>", Path.GetFileName(projectInfo.ReferencePath.AbsolutePath) },
                        { "<PROJECT_GUID>", projectInfo.Guid.ToString().ToUpper() } }));
            if (projectInfo.ProjectDependencies.Count > 0)
            {
                string projectDependencyStartSection = "    ProjectSection(ProjectDependencies) = postProject";
                string projectDependencyGuid = "        {<DependencyGuid>} = {<DependencyGuid>}";
                string projectDependencyStopSection = "    EndProjectSection";
                toReturn.AppendLine(projectDependencyStartSection);

                foreach (CSProjectDependency<CSProjectInfo> project in projectInfo.ProjectDependencies)
                {
                    toReturn.AppendLine(projectDependencyGuid.Replace("<DependencyGuid>", project.Dependency.Guid.ToString().ToUpper()));
                }

                toReturn.AppendLine(projectDependencyStopSection);
            }
            toReturn.Append("EndProject");
            return toReturn.ToString();
        }

        /// <summary>
        /// Exports the project info into a solution file, and the CSProject files.
        /// </summary>
        /// <param name="solutionTemplateText">The solution file template text.</param>
        /// <param name="projectFileTemplateText">The project file template text.</param>
        /// <param name="generatedProjectPath">The output folder of the platform props.</param>
        public void ExportSolution(string solutionTemplateText, string projectFileTemplateText, string generatedProjectPath)
        {
            string solutionFilePath = Path.Combine(generatedProjectPath, $"{UnityProjectName}.sln");

            if (File.Exists(solutionFilePath))
            {
                File.Delete(solutionFilePath);
            }

            if (Utilities.TryGetTextTemplate(solutionTemplateText, "PROJECT", out string projectEntryTemplate, out string projectEntryTemplateBody)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM", out string configurationPlatformEntry, out string configurationPlatformEntryBody)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM_MAPPING", out string configurationPlatformMappingTemplate, out string configurationPlatformMappingTemplateBody)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM_ENABLED", out string configurationPlatformEnabledTemplate, out string configurationPlatformEnabledTemplateBody))
            {
                CSProjectInfo[] unorderedProjects = CSProjects.Select(t => t.Value).ToArray();
                List<CSProjectInfo> orderedProjects = new List<CSProjectInfo>();

                while (orderedProjects.Count < unorderedProjects.Length)
                {
                    bool oneRemoved = false;
                    for (int i = 0; i < unorderedProjects.Length; i++)
                    {
                        if (unorderedProjects[i] == null)
                        {
                            continue;
                        }

                        if (unorderedProjects[i].ProjectDependencies.Count == 0 || unorderedProjects[i].ProjectDependencies.All(t => orderedProjects.Contains(t.Dependency)))
                        {
                            orderedProjects.Add(unorderedProjects[i]);

                            unorderedProjects[i] = null;
                            oneRemoved = true;
                        }
                    }

                    if (!oneRemoved)
                    {
                        Debug.LogError($"Possible circular dependency.");
                        break;
                    }
                }

                IEnumerable<string> projectEntries = orderedProjects.Select(t => GetProjectEntry(t, projectEntryTemplateBody));

                string[] twoConfigs = new string[] {
                    configurationPlatformEntryBody.Replace("<Configuration>", "InEditor"),
                    configurationPlatformEntryBody.Replace("<Configuration>", "Player")
                };

                IEnumerable<string> configPlatforms = twoConfigs
                    .SelectMany(t => AvailablePlatforms.Select(p => t.Replace("<Platform>", p.Name.ToString())));

                List<string> configurationMappings = new List<string>();
                List<string> disabled = new List<string>();

                foreach (CSProjectInfo project in orderedProjects.Select(t => t))
                {
                    string ConfigurationTemplateReplace(string template, string guid, string configuration, string platform)
                    {
                        return Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                        {
                            { "<PROJECT_GUID_TOKEN>", guid.ToString().ToUpper() },
                            { "<PROJECT_CONFIGURATION_TOKEN>", configuration },
                            { "<PROJECT_PLATFORM_TOKEN>", platform },
                            { "<SOLUTION_CONFIGURATION_TOKEN>", configuration },
                            { "<SOLUTION_PLATFORM_TOKEN>", platform },
                        });
                    }

                    void ProcessMappings(Guid guid, string configuration, IReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> platforms)
                    {
                        foreach (CompilationPlatformInfo platform in AvailablePlatforms)
                        {
                            configurationMappings.Add(ConfigurationTemplateReplace(configurationPlatformMappingTemplateBody, guid.ToString(), configuration, platform.Name));

                            if (platforms.ContainsKey(platform.BuildTarget))
                            {
                                configurationMappings.Add(ConfigurationTemplateReplace(configurationPlatformEnabledTemplateBody, guid.ToString(), configuration, platform.Name));
                            }
                        }
                    }

                    ProcessMappings(project.Guid, "InEditor", project.InEditorPlatforms);
                    ProcessMappings(project.Guid, "Player", project.PlayerPlatforms);
                }

                solutionTemplateText = Utilities.ReplaceTokens(solutionTemplateText, new Dictionary<string, string>()
                {
                    { projectEntryTemplate, string.Join(Environment.NewLine, projectEntries)},
                    { configurationPlatformEntry, string.Join(Environment.NewLine, configPlatforms)},
                    { configurationPlatformMappingTemplate, string.Join(Environment.NewLine, configurationMappings) },
                    { configurationPlatformEnabledTemplate, string.Join(Environment.NewLine, disabled) }
                });
            }
            else
            {
                Debug.LogError("Failed to find Project and/or Configuration_Platform templates in the solution template file.");
            }

            foreach (CSProjectInfo project in CSProjects.Values)
            {
                project.ExportProject(projectFileTemplateText, generatedProjectPath);
            }

            File.WriteAllText(solutionFilePath, solutionTemplateText);
        }
    }
}
