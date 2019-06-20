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

        private readonly Dictionary<string, Assembly> unityAssemblies;

        public static UnityProjectInfo CreateProjectInfo()
        {
            return new UnityProjectInfo();
        }

        public IReadOnlyDictionary<string, Assembly> UnityAssemblies { get; }

        public IReadOnlyDictionary<string, CSProjectInfo> CSProjects { get; private set; }

        private UnityProjectInfo()
        {
            unityAssemblies = CompilationPipeline.GetAssemblies().ToDictionary(t => t.name);
            UnityAssemblies = new ReadOnlyDictionary<string, Assembly>(unityAssemblies);

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
            }

        }

        public void ExportSolution(string solutionTemplateText, string projectFileTemplateText, string commonPropsFilePath)
        {
            string solutionFilePath = Path.Combine(Application.dataPath.Replace("Assets", "MSBuild"), "MRTK.sln");

            if (File.Exists(solutionFilePath))
            {
                File.Delete(solutionFilePath);
            }

            if (Utilities.TryGetTextTemplate(solutionTemplateText, "PROJECT", out string projectEntryTemplate)
                && Utilities.TryGetTextTemplate(solutionTemplateText, "CONFIGURATION_PLATFORM", out string configurationPlatformEntry))
            {
                IEnumerable<string> projectEntries = CSProjects.Select(t =>
                    Utilities.ReplaceTokens(projectEntryTemplate, new Dictionary<string, string>() {
                        {"#PROJECT_TEMPLATE ", string.Empty },
                        { "<PROJECT_NAME>", t.Value.Name },
                        { "<PROJECT_RELATIVE_PATH>", Path.GetFileName(t.Value.ProjectFilePath) },
                        { "<PROJECT_GUID>", t.Value.Guid.ToString() } }));

                string[] twoConfigs = new string[] {
                    configurationPlatformEntry.Replace("#CONFIGURATION_PLATFORM_TEMPLATE ", string.Empty).Replace("<Configuration>", "InEditor"),
                    configurationPlatformEntry.Replace("#CONFIGURATION_PLATFORM_TEMPLATE ", string.Empty).Replace("<Configuration>", "Player")
                };

                IEnumerable<string> configPlatforms = CompilationSettings.Instance.AvailablePlatforms.Keys
                    .SelectMany(p => twoConfigs.Select(t => t.Replace("<Platform>", p.Name.ToString())));

                solutionTemplateText = Utilities.ReplaceTokens(solutionTemplateText, new Dictionary<string, string>()
                {
                    {projectEntryTemplate, string.Join(string.Empty, projectEntries)},
                    {configurationPlatformEntry, string.Join(string.Empty, configPlatforms)}
                });
            }
            else
            {
                Debug.LogError("Failed to find Project and/or Configuration_Platform templates in the solution template file.");
            }

            foreach (CSProjectInfo project in CSProjects.Values)
            {
                project.ExportProject(projectFileTemplateText, commonPropsFilePath);
            }

            File.WriteAllText(solutionFilePath, solutionTemplateText);
        }
    }
}
#endif