// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// A helper class to parse the state of the current Unity project.
    /// </summary>
    public class UnityProjectInfo
    {
        private IEnumerable<CompilationPlatformInfo> availablePlatforms;

        public IReadOnlyDictionary<string, CSProjectInfo> CSProjects { get; }

        public IReadOnlyCollection<PluginAssemblyInfo> Plugins { get; }

        public UnityProjectInfo(IEnumerable<CompilationPlatformInfo> availablePlatforms, string projectOutputPath)
        {
            this.availablePlatforms = availablePlatforms;

            Dictionary<string, Assembly> unityAssemblies = CompilationPipeline.GetAssemblies().ToDictionary(t => t.name);

            Dictionary<string, CSProjectInfo> csProjects = new Dictionary<string, CSProjectInfo>();
            CSProjects = new ReadOnlyDictionary<string, CSProjectInfo>(csProjects);

            foreach (KeyValuePair<string, Assembly> pair in unityAssemblies)
            {
                CSProjectInfo toAdd;
                string asmDefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(pair.Key);
                if (string.IsNullOrEmpty(asmDefPath))
                {
                    if (!pair.Key.StartsWith("Assembly-CSharp"))
                    {
                        Debug.LogError($"Failed to retrieve AsmDef for script assembly: {pair.Key}");
                    }

                    toAdd = new CSProjectInfo(availablePlatforms, Guid.NewGuid(), null, pair.Value, projectOutputPath);
                }
                else
                {
                    string guid = AssetDatabase.AssetPathToGUID(asmDefPath);
                    if (!Guid.TryParse(guid, out Guid guidResult))
                    {
                        Debug.LogError($"Failed to get GUID of the AsmDef at '{asmDefPath}' for assembly: {pair.Key}");
                    }
                    else
                    {
                        guidResult = Guid.NewGuid();
                    }

                    AssemblyDefinitionAsset assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmDefPath);
                    AssemblyDefinitionInfo assemblyDefinitionInfo = assemblyDefinitionAsset == null ? null : JsonUtility.FromJson<AssemblyDefinitionInfo>(assemblyDefinitionAsset.text);
                    assemblyDefinitionInfo?.Validate(availablePlatforms);
                    toAdd = new CSProjectInfo(availablePlatforms, guidResult, assemblyDefinitionInfo, pair.Value, projectOutputPath);
                }

                csProjects.Add(pair.Key, toAdd);
            }

            Plugins = new ReadOnlyCollection<PluginAssemblyInfo>(ScanForPluginDLLs());

            foreach (PluginAssemblyInfo plugin in Plugins)
            {
                if (plugin.Type == PluginType.Native)
                {
                    Debug.LogWarning($"Native plugin {plugin.ReferencePath.AbsolutePath} not yet supported for MSBuild project.");
                }
            }

            foreach (CSProjectInfo project in CSProjects.Values)
            {
                // Get the assembly references first from AssemblyDefinitionInfo if available (it's actually more correct), otherwise fallback to Assemby
                IEnumerable<string> references = project.AssemblyDefinitionInfo == null
                    ? project.Assembly.assemblyReferences.Select(t => t.name)
                    : (project.AssemblyDefinitionInfo.references ?? Array.Empty<string>());

                foreach (string reference in references)
                {
                    if (CSProjects.TryGetValue(reference, out CSProjectInfo dependency))
                    {
                        project.AddDependency(dependency);
                    }
                    else
                    {
                        Debug.LogError($"Failed to get dependency '{reference}' for project '{project.Name}'.");
                    }
                }

                foreach (PluginAssemblyInfo plugin in Plugins)
                {
                    if (plugin.AutoReferenced && plugin.Type != PluginType.Native)
                    {
                        project.AddDependency(plugin);
                    }
                }
            }
        }

        private List<PluginAssemblyInfo> ScanForPluginDLLs()
        {
            List<PluginAssemblyInfo> toReturn = new List<PluginAssemblyInfo>();

            foreach (string assetAssemblyPath in Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories))
            {
                string assetRelativePath = Utilities.GetAssetsRelativePathFrom(assetAssemblyPath);
                PluginImporter importer = (PluginImporter)AssetImporter.GetAtPath(assetRelativePath);
                PluginAssemblyInfo toAdd = new PluginAssemblyInfo(availablePlatforms, Guid.Parse(AssetDatabase.AssetPathToGUID(assetRelativePath)), assetAssemblyPath, importer.isNativePlugin ? PluginType.Native : PluginType.Managed);
                toReturn.Add(toAdd);
            }

            foreach (string packageDllPath in Directory.GetFiles(Utilities.GetFullPathFromPackagesRelative("Packages"), "*.dll", SearchOption.AllDirectories))
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
                PluginAssemblyInfo toAdd = new PluginAssemblyInfo(availablePlatforms, guid, packageDllPath, isManaged ? PluginType.Managed : PluginType.Native);
                toReturn.Add(toAdd);
            }

            return toReturn;
        }

        private string GetProjectEntry(CSProjectInfo projectInfo, string projectEntryTemplateBody)
        {
            StringBuilder toReturn = new StringBuilder(Utilities.ReplaceTokens(projectEntryTemplateBody, new Dictionary<string, string>() {
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
            string solutionFilePath = Path.Combine(generatedProjectPath, "MRTK.sln");

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

                File.WriteAllLines(Path.Combine(generatedProjectPath, "buildall.bat"), orderedProjects.Select(t => $"dotnet build {t.ReferencePath.AbsolutePath} -p:\"UnityConfiguration=%1;UnityPlatform=%2;MSBuildExtensionsPath=%~3\" --no-dependencies\nif %ERRORLEVEL% GEQ 1 EXIT /B 1"));

                IEnumerable<string> projectEntries = orderedProjects.Select(t => GetProjectEntry(t, projectEntryTemplateBody));

                string[] twoConfigs = new string[] {
                    configurationPlatformEntryBody.Replace("<Configuration>", "InEditor"),
                    configurationPlatformEntryBody.Replace("<Configuration>", "Player")
                };

                IEnumerable<string> configPlatforms = twoConfigs
                    .SelectMany(t => availablePlatforms.Select(p => t.Replace("<Platform>", p.Name.ToString())));

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
                        foreach (CompilationPlatformInfo platform in availablePlatforms)
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
                    { configurationPlatformEntry, string.Join(string.Empty, configPlatforms)},
                    { configurationPlatformMappingTemplate, string.Join(string.Empty, configurationMappings) },
                    { configurationPlatformEnabledTemplate, string.Join(string.Empty, disabled) }
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
#endif