#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class UnityProjectInfo
    {
        private const string SDKProjectTypeGuid = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";

        public static UnityProjectInfo CreateProjectInfo()
        {
            return new UnityProjectInfo();
        }

        public IReadOnlyDictionary<string, CSProjectInfo> CSProjects { get; }

        public IReadOnlyDictionary<string, PluginAssemblyInfo> Plugins { get; }

        private UnityProjectInfo()
        {
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

                    toAdd = new CSProjectInfo(Guid.NewGuid(), null, pair.Value, Application.dataPath.Replace("Assets", "MSBuild"));
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
                    assemblyDefinitionInfo?.Validate();
                    toAdd = new CSProjectInfo(guidResult, assemblyDefinitionInfo, pair.Value, Application.dataPath.Replace("Assets", "MSBuild"));
                }

                csProjects.Add(pair.Key, toAdd);
            }

            Plugins = new ReadOnlyDictionary<string, PluginAssemblyInfo>(ScanForPluginDLLs());

            foreach (PluginAssemblyInfo plugin in Plugins.Values)
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


                // TODO Find a better way to filter which plugins should be included; project.Assembly.allReferences isn't correct 
                foreach (PluginAssemblyInfo plugin in Plugins.Values)
                {
                    if (plugin.AutoReferenced && plugin.Type != PluginType.Native)
                    {
                        project.AddDependency(plugin);
                    }
                }
            }

        }

        private Dictionary<string, PluginAssemblyInfo> ScanForPluginDLLs()
        {
            return Directory.GetFiles(Application.dataPath, "*.dll", SearchOption.AllDirectories)
                .Select(t => new { FullPath = t, AssetsRelativePath = Utilities.AbsolutePathToAssetsRelative(t) })
                .Select(t => new PluginAssemblyInfo(Guid.Parse(AssetDatabase.AssetPathToGUID(t.AssetsRelativePath)), t.AssetsRelativePath, t.FullPath))
                .ToDictionary(t => t.AssetsRelativePath);
        }

        public void ExportSolution(string solutionTemplateText, string projectFileTemplateText, string propsOutputFolder)
        {
            string solutionFilePath = Path.Combine(Application.dataPath.Replace("Assets", "MSBuild"), "MRTK.sln");

            if (File.Exists(solutionFilePath))
            {
                File.Delete(solutionFilePath);
            }

            if (Utilities.TryGetTextTemplate(solutionTemplateText, "PROJECT", out string projectEntryTemplate)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM", out string configurationPlatformEntry)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM_MAPPING", out string configurationPlatformMappingTemplate)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM_ENABLED", out string configurationPlatformEnabledTemplate))
            {
                IEnumerable<string> projectEntries = CSProjects.Select(t =>
                    Utilities.ReplaceTokens(projectEntryTemplate, new Dictionary<string, string>() {
                        {"#PROJECT_TEMPLATE ", string.Empty },
                        { "<PROJECT_NAME>", t.Value.Name },
                        { "<PROJECT_RELATIVE_PATH>", Path.GetFileName(t.Value.ReferencePath.AbsolutePath) },
                        { "<PROJECT_GUID>", t.Value.Guid.ToString() } }));

                string[] twoConfigs = new string[] {
                    configurationPlatformEntry.Replace("#CONFIGURATION_PLATFORM_TEMPLATE ", string.Empty).Replace("<Configuration>", "InEditor"),
                    configurationPlatformEntry.Replace("#CONFIGURATION_PLATFORM_TEMPLATE ", string.Empty).Replace("<Configuration>", "Player")
                };

                IEnumerable<string> configPlatforms = CompilationSettings.Instance.AvailablePlatforms.Values
                    .SelectMany(p => twoConfigs.Select(t => t.Replace("<Platform>", p.Name.ToString())));

                List<string> configurationMappings = new List<string>();
                List<string> enabledConfigurations = new List<string>();

                foreach (CSProjectInfo project in CSProjects.Values)
                {
                    string ConfigurationTemplateReplace(string template, string guid, string configuration, string platform)
                    {
                        return Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                        {
                            { "<PROJECT_GUID_TOKEN>", guid },
                            { "<PROJECT_CONFIGURATION_TOKEN>", configuration },
                            { "<PROJECT_PLATFORM_TOKEN>", platform },
                            { "<SOLUTION_CONFIGURATION_TOKEN>", configuration },
                            { "<SOLUTION_PLATFORM_TOKEN>", platform },
                        });
                    }

                    void ProcessMappings(Guid guid, string configuration, IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> platforms)
                    {
                        foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> platform in CompilationSettings.Instance.AvailablePlatforms)
                        {
                            configurationMappings.Add(ConfigurationTemplateReplace(configurationPlatformMappingTemplate, guid.ToString(), configuration, platform.Value.Name));

                            if (!platforms.ContainsKey(platform.Key))
                            {
                                enabledConfigurations.Add(ConfigurationTemplateReplace(configurationPlatformEnabledTemplate, guid.ToString(), configuration, platform.Value.Name));
                            }
                        }
                    }

                    ProcessMappings(project.Guid, "InEditor", project.InEditorPlatforms);
                    ProcessMappings(project.Guid, "Player", project.PlayerPlatforms);
                }

                solutionTemplateText = Utilities.ReplaceTokens(solutionTemplateText, new Dictionary<string, string>()
                {
                    { projectEntryTemplate, string.Join(string.Empty, projectEntries)},
                    { configurationPlatformEntry, string.Join(string.Empty, configPlatforms)},
                    { configurationPlatformMappingTemplate, string.Join(string.Empty, configurationMappings) },
                    { configurationPlatformEnabledTemplate, string.Join(string.Empty, enabledConfigurations) }
                });
            }
            else
            {
                Debug.LogError("Failed to find Project and/or Configuration_Platform templates in the solution template file.");
            }

            foreach (CSProjectInfo project in CSProjects.Values)
            {
                project.ExportProject(projectFileTemplateText, propsOutputFolder);
            }

            File.WriteAllText(solutionFilePath, solutionTemplateText);
        }
    }
}
#endif