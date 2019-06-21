#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class CSProjectDependency
    {
        public CSProjectInfo DependencyProject { get; }

        public HashSet<BuildTarget> InEditorSupportedPlatforms { get; }

        public HashSet<BuildTarget> PlayerSupportedPlatforms { get; }

        public CSProjectDependency(CSProjectInfo dependencyProject, HashSet<BuildTarget> inEditorSupportedPlatforms, HashSet<BuildTarget> playerSupportedPlatforms)
        {
            DependencyProject = dependencyProject;
            InEditorSupportedPlatforms = inEditorSupportedPlatforms;
            PlayerSupportedPlatforms = playerSupportedPlatforms;
        }
    }

    public enum ProjectType
    {
        /// <summary>
        /// The project is backed by an Assembly-Definition file.
        /// </summary>
        AsmDef,

        /// <summary>
        /// The project is one of the pre-defined editor assemblies (Assembly-CSharp-Editor, etc).
        /// </summary>
        PredefinedEditorAssembly,

        /// <summary>
        /// The project is one of the pre-defined assemblies (Assembly-CSharp, etc).
        /// </summary>
        PredefinedAssembly
    }

    public class CSProjectInfo
    {
        private readonly List<CSProjectDependency> dependencies = new List<CSProjectDependency>();

        public Guid Guid { get; }

        public Assembly Assembly { get; }

        public AssemblyDefinitionInfo AssemblyDefinitionInfo { get; }

        public ProjectType ProjectType { get; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// In the editor, we can support all patforms if it's a pre-defined assembly, or an asmdef with Editor platform checked. 
        /// Otherwise we fallback to just the platforms specified in the editor.
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> InEditorPlatforms { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// In the player, we support any platform if pre-defined assembly, or the ones explicitly specified in the AsmDef player.
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> PlayerPlatforms { get; }

        public string Name => Assembly.name;

        public string ProjectFilePath { get; private set; }

        internal CSProjectInfo(Guid guid, AssemblyDefinitionInfo assemblyDefinitionInfo, Assembly assembly, string baseOutputPath)
        {
            Guid = guid;
            AssemblyDefinitionInfo = assemblyDefinitionInfo;
            Assembly = assembly;
            ProjectFilePath = Path.Combine(baseOutputPath, $"{assembly.name}.csproj");

            ProjectType = GetProjectType(assemblyDefinitionInfo, assembly);

            InEditorPlatforms = GetCompilationPlatforms(true);
            PlayerPlatforms = GetCompilationPlatforms(false);

            if (InEditorPlatforms.Count == 0 && PlayerPlatforms.Count == 0)
            {
                Debug.LogError($"The assembly project '{Name}' doesn't contain any supported in-editor or player platform targets.");
            }
        }

        private ProjectType GetProjectType(AssemblyDefinitionInfo assemblyDefinitionInfo, Assembly assembly)
        {
            if (assemblyDefinitionInfo != null)
            {
                return ProjectType.AsmDef;
            }

            switch (assembly.name)
            {
                case "Assembly-CSharp":
                case "Assembly-CSharp-firstpass":
                    return ProjectType.PredefinedAssembly;
                case "Assembly-CSharp-editor":
                case "Assembly-CSharp-firstpass-editor":
                    return ProjectType.PredefinedEditorAssembly;
                default:
                    Debug.LogError($"Predefined assembly '{assembly.name}' was not recognized, this generally means it should be added to the switch statement in CSProjectInfo:GetProjectType. Treating is as a PredefinedAssembly instead of PredefinedEditorAssembly.");
                    return ProjectType.PredefinedAssembly;
            }
        }

        private ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> GetCompilationPlatforms(bool inEditor)
        {
            bool returnAllPlatforms = ProjectType == ProjectType.PredefinedAssembly
                || (inEditor && ProjectType == ProjectType.PredefinedEditorAssembly)
                || (inEditor && ProjectType == ProjectType.AsmDef && AssemblyDefinitionInfo.EditorPlatformSupported);

            if (returnAllPlatforms)
            {
                return new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(CompilationSettings.Instance.AvailablePlatforms.ToDictionary(t => t.Key, t => t.Value));
            }

            bool returnNoPlatforms = (!inEditor && ProjectType == ProjectType.PredefinedEditorAssembly)
                || (!inEditor && ProjectType == ProjectType.AsmDef && AssemblyDefinitionInfo.TestAssembly);

            if (returnNoPlatforms)
            {
                return new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(new Dictionary<BuildTarget, CompilationSettings.CompilationPlatform>());
            }

            // We only are an asmdef at this point, as we handle all predefined assembly at line 130, inEditor && predefined editor assembly at line 130, and !inEditor && predefined editor assembly at line 139
            Func<KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform>, bool> predicate = AssemblyDefinitionInfo.includePlatforms.Length > 0
                ? predicate = (t) => AssemblyDefinitionInfo.includePlatforms.Contains(t.Value.Name)
                : predicate = (t) => !AssemblyDefinitionInfo.excludePlatforms.Contains(t.Value.Name);

            return new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(
                CompilationSettings.Instance.AvailablePlatforms
                    .Where(predicate)
                    .ToDictionary(t => t.Key, t => t.Value));
        }

        internal void AddDependency(CSProjectInfo csProjectInfo)
        {
            dependencies.Add(new CSProjectDependency(csProjectInfo,
                new HashSet<BuildTarget>(InEditorPlatforms.Keys.Intersect(csProjectInfo.InEditorPlatforms.Keys)),
                new HashSet<BuildTarget>(PlayerPlatforms.Keys.Intersect(csProjectInfo.PlayerPlatforms.Keys))));
        }

        internal void ExportProject(string projectFileTemplateText, string propsOutputFolder)
        {
            if (File.Exists(ProjectFilePath))
            {
                File.Delete(ProjectFilePath);
            }

            if (Utilities.TryGetXMLTemplate(projectFileTemplateText, "PROJECT_REFERENCE_SET", out string projectReferenceSetTemplate)
                && Utilities.TryGetXMLTemplate(projectFileTemplateText, "SOURCE_INCLUDE", out string sourceIncludeTemplate)
                && Utilities.TryGetXMLTemplate(projectFileTemplateText, "SUPPORTED_PLATFORM_BUILD_CONDITION", out string suportedPlatformBuildConditionTemplate))
            {
                List<string> sourceIncludes = new List<string>();
                foreach (string source in Assembly.sourceFiles)
                {
                    string normalized = Utilities.NormalizePath(source);
                    if (normalized.StartsWith("Packages"))
                    {
                        normalized = "PackagesCopy" + normalized.Substring("Packages".Length);
                    }
                    sourceIncludes.Add(Utilities.ReplaceTokens(sourceIncludeTemplate, new Dictionary<string, string>()
                    {
                        {"##RELATIVE_SOURCE_PATH##", $"..\\{normalized}" },
                        {"##PROJECT_LINK_PATH##", normalized.Replace("Assets\\", string.Empty) }
                    }));
                }

                bool bothConfigurations = PlayerPlatforms.Count > 0;

                List<string> supportedPlatformBuildConditions = new List<string>();
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "InEditor", InEditorPlatforms);
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "Player", PlayerPlatforms);

                Dictionary<string, string> tokens = new Dictionary<string, string>()
                {
                    { "<!--PROJECT_GUID_TOKEN-->", Guid.ToString() },
                    { "##COMMON_PROPS_FILE_PATH##", Path.Combine(propsOutputFolder, Compilation.CommonPropsFileName) },
                    //{ "##DEFAULT_PLATFORM_PROPS_FILE_PATH##", Path.Combine(propsOutputFolder, Compilation.GetPlatformCommonPropsFileName(CompilationSettings.Instance.AvailablePlatforms[BuildTarget.StandaloneWindows])) },
                    { "<!--ALLOW_UNSAFE_TOKEN-->", Assembly.compilerOptions.AllowUnsafeCode.ToString() },
                    { "<!--PROJECT_CONFIGURATIONS_TOKEN-->", bothConfigurations ? "InEditor;Player" : "InEditor" },
                    { projectReferenceSetTemplate, string.Join("\r\n", CreateProjectReferenceSet(projectReferenceSetTemplate, true), CreateProjectReferenceSet(projectReferenceSetTemplate, false)) },
                    { sourceIncludeTemplate, string.Join("\r\n", sourceIncludes) },
                    { suportedPlatformBuildConditionTemplate, string.Join("\r\n", supportedPlatformBuildConditions) },
                    { "##PLATFORM_PROPS_FOLDER_PATH_TOKEN##", propsOutputFolder }
                };

                projectFileTemplateText = Utilities.ReplaceTokens(projectFileTemplateText, tokens);
            }
            else
            {
                Debug.LogError("Failed to find ProjectReferenceSet and/or Source_Include templates in the project template file.");
            }

            File.WriteAllText(ProjectFilePath, projectFileTemplateText);
        }

        private void PopulateSupportedPlatformBuildConditions(List<string> supportedPlatformBuildConditions, string template, string configuration, IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> platforms)
        {
            foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> platform in platforms)
            {
                supportedPlatformBuildConditions.Add(Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                {
                    { "##SUPPORTED_CONFIGURATION_TOKEN##", configuration },
                    { "##SUPPORTED_PLATFORM_TOKEN##", platform.Value.AssemblyDefinitionPlatform.Name }
                }));
            }
        }

        private string CreateProjectReferenceSet(string template, bool inEditor)
        {
            if (Utilities.TryGetXMLTemplate(template, "PROJECT_REFERENCE", out string projectReferenceTemplate))
            {
                List<string> projectReferences = new List<string>();
                foreach (CSProjectDependency dependency in dependencies)
                {
                    List<string> platformConditions = GetPlatformConditions(inEditor ? InEditorPlatforms : PlayerPlatforms, inEditor ? dependency.InEditorSupportedPlatforms : dependency.PlayerSupportedPlatforms);

                    projectReferences.Add(Utilities.ReplaceTokens(projectReferenceTemplate, new Dictionary<string, string>()
                    {
                        { "##REFERENCE_TOKEN##", $"{dependency.DependencyProject.Name}.csproj" },
                        {"##CONDITION_TOKEN##", platformConditions.Count == 0 ? "false" : string.Join(" OR ", platformConditions)}
                    }));
                }

                return Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                {
                    {"##REFERENCE_CONFIGURATION_TOKEN##", inEditor ? "InEditor" : "Player" },
                    {projectReferenceTemplate, string.Join("\r\n", projectReferences) }
                });
            }
            else
            {
                Debug.LogError("Failed to find ProjectReference template in ProjectReferenceSet template");
                return template;
            }
        }

        private List<string> GetPlatformConditions(IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> platforms, HashSet<BuildTarget> dependencyPlatforms)
        {
            List<string> toReturn = new List<string>();

            foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> pair in platforms)
            {
                if (dependencyPlatforms.Contains(pair.Key))
                {
                    string platformName = pair.Value.AssemblyDefinitionPlatform.Name;
                    toReturn.Add($"'$(Platform)' == '{platformName}'");
                }
            }

            return toReturn;
        }

        private string GetDefaultPlatform(IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> platforms)
        {

            if (platforms.TryGetValue(BuildTarget.StandaloneWindows, out CompilationSettings.CompilationPlatform defaultPlatform)
                || platforms.TryGetValue(BuildTarget.StandaloneWindows64, out defaultPlatform)
                || platforms.TryGetValue(BuildTarget.WSAPlayer, out defaultPlatform)
                || platforms.TryGetValue(BuildTarget.Android, out defaultPlatform)
                || platforms.TryGetValue(BuildTarget.iOS, out defaultPlatform)
                || platforms.TryGetValue(BuildTarget.NoTarget, out defaultPlatform))
            {
                return defaultPlatform.AssemblyDefinitionPlatform.Name;
            }

            return string.Empty;
        }

        private string GetProjectReferenceString(string path)
        {
            return $"<ProjectReference Include=\"{path}\" />";
        }
    }
}
#endif