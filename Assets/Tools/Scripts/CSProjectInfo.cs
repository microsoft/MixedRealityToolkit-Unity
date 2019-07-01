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
    public class ProjectDependencyBase<T>
    {
        public T Dependency { get; }

        public HashSet<BuildTarget> InEditorSupportedPlatforms { get; }

        public HashSet<BuildTarget> PlayerSupportedPlatforms { get; }

        public ProjectDependencyBase(T dependency, HashSet<BuildTarget> inEditorSupportedPlatforms, HashSet<BuildTarget> playerSupportedPlatforms)
        {
            Dependency = dependency;
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
        /// The project is backed by an Assembly-Definition file that only targets editor.
        /// </summary>
        EditorAsmDef,

        /// <summary>
        /// The project is one of the pre-defined editor assemblies (Assembly-CSharp-Editor, etc).
        /// </summary>
        PredefinedEditorAssembly,

        /// <summary>
        /// The project is one of the pre-defined assemblies (Assembly-CSharp, etc).
        /// </summary>
        PredefinedAssembly
    }

    public class ReferenceInfo
    {
        public Guid Guid { get; }

        public Uri ReferencePath { get; }

        public string Name { get; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// In the editor, we can support all patforms if it's a pre-defined assembly, or an asmdef with Editor platform checked. 
        /// Otherwise we fallback to just the platforms specified in the editor.
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> InEditorPlatforms { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// In the player, we support any platform if pre-defined assembly, or the ones explicitly specified in the AsmDef player.
        /// </remarks>
        public IReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform> PlayerPlatforms { get; protected set; }

        public ReferenceInfo(Guid guid, Uri referencePath, string name)
        {
            Guid = guid;
            ReferencePath = referencePath;
            Name = name;
        }
    }

    public class CSProjectInfo : ReferenceInfo
    {
        private readonly List<ProjectDependencyBase<CSProjectInfo>> csProjectDependencies = new List<ProjectDependencyBase<CSProjectInfo>>();
        private readonly List<ProjectDependencyBase<PluginAssemblyInfo>> pluginAssemblyDependencies = new List<ProjectDependencyBase<PluginAssemblyInfo>>();

        public Assembly Assembly { get; }

        public AssemblyDefinitionInfo AssemblyDefinitionInfo { get; }

        public ProjectType ProjectType { get; }

        internal CSProjectInfo(Guid guid, AssemblyDefinitionInfo assemblyDefinitionInfo, Assembly assembly, string baseOutputPath)
            : base(guid, new Uri(Path.Combine(baseOutputPath, $"{assembly.name}.csproj")), assembly.name)
        {
            AssemblyDefinitionInfo = assemblyDefinitionInfo;
            Assembly = assembly;

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
                return assemblyDefinitionInfo.EditorPlatformSupported && !assemblyDefinitionInfo.NonEditorPlatformSupported ? ProjectType.EditorAsmDef : ProjectType.AsmDef;
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
                || (inEditor && ProjectType == ProjectType.EditorAsmDef)
                || (inEditor && ProjectType == ProjectType.AsmDef && AssemblyDefinitionInfo.EditorPlatformSupported);

            if (returnAllPlatforms)
            {
                return new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(CompilationSettings.Instance.AvailablePlatforms.ToDictionary(t => t.Key, t => t.Value));
            }

            bool returnNoPlatforms = (!inEditor && ProjectType == ProjectType.PredefinedEditorAssembly)
                || (!inEditor && ProjectType == ProjectType.EditorAsmDef)
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
            AddDependency(csProjectDependencies, csProjectInfo);
        }

        internal void AddDependency(PluginAssemblyInfo pluginAssemblyInfo)
        {
            AddDependency(pluginAssemblyDependencies, pluginAssemblyInfo);
        }

        private void AddDependency<T>(List<ProjectDependencyBase<T>> items, T referenceInfo) where T : ReferenceInfo
        {
            items.Add(new ProjectDependencyBase<T>(referenceInfo,
                new HashSet<BuildTarget>(InEditorPlatforms.Keys.Intersect(referenceInfo.InEditorPlatforms.Keys)),
                new HashSet<BuildTarget>(PlayerPlatforms.Keys.Intersect(referenceInfo.PlayerPlatforms.Keys))));
        }

        internal void ExportProject(string projectFileTemplateText, string propsOutputFolder)
        {
            if (File.Exists(ReferencePath.AbsolutePath))
            {
                File.Delete(ReferencePath.AbsolutePath);
            }

            if (Utilities.TryGetXMLTemplate(projectFileTemplateText, "PROJECT_REFERENCE_SET", out string projectReferenceSetTemplate)
                && Utilities.TryGetXMLTemplate(projectFileTemplateText, "SOURCE_INCLUDE", out string sourceIncludeTemplate)
                && Utilities.TryGetXMLTemplate(projectFileTemplateText, "SUPPORTED_PLATFORM_BUILD_CONDITION", out string suportedPlatformBuildConditionTemplate))
            {
                List<string> sourceIncludes = new List<string>();
                Dictionary<string, string> sourceGuidToClassName = new Dictionary<string, string>();
                foreach (string source in Assembly.sourceFiles)
                {
                    ProcessSourceFile(source, sourceIncludeTemplate, sourceIncludes, sourceGuidToClassName);
                }

                File.WriteAllLines(Path.Combine(propsOutputFolder, $"{Guid.ToString()}.csmap"), sourceGuidToClassName.Select(t => $"{t.Key}:{t.Value}"));

                bool bothConfigurations = PlayerPlatforms.Count > 0;

                List<string> supportedPlatformBuildConditions = new List<string>();
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "InEditor", InEditorPlatforms);
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "Player", PlayerPlatforms);

                HashSet<string> inEditorSearchPaths = new HashSet<string>(), playerSearchPaths = new HashSet<string>();
                Dictionary<string, string> tokens = new Dictionary<string, string>()
                {
                    { "<!--PROJECT_GUID_TOKEN-->", Guid.ToString() },
                    { "<!--LANGUAGE_VERSION-->", Compilation.CSharpVersion },
                    { "<!--UNITY_EDITOR_INSTALL_FOLDER-->", Path.GetDirectoryName(EditorApplication.applicationPath) + "\\"},
                    { "<!--DEVELOPMENT_BUILD-->", "false" }, // Default to false
                    { "<!--OUTPUT_PATH_TOKEN-->", Path.Combine("..", "MRTKBuild") },
                    { "<!--ALLOW_UNSAFE_TOKEN-->", Assembly.compilerOptions.AllowUnsafeCode.ToString() },
                    { "<!--PROJECT_CONFIGURATIONS_TOKEN-->", bothConfigurations ? "InEditor;Player" : "InEditor" },
                    { "<!--SUPPORTED_PLATFORMS_TOKEN-->", string.Join(";", CompilationSettings.Instance.AvailablePlatforms.Select(t=>t.Value.Name)) },
                    { projectReferenceSetTemplate, string.Join("\r\n", CreateProjectReferencesSet(projectReferenceSetTemplate, inEditorSearchPaths, true), CreateProjectReferencesSet(projectReferenceSetTemplate, playerSearchPaths, false)) },
                    { sourceIncludeTemplate, string.Join("\r\n", sourceIncludes) },
                    { suportedPlatformBuildConditionTemplate, string.Join("\r\n", supportedPlatformBuildConditions) },
                    { "##PLATFORM_PROPS_FOLDER_PATH_TOKEN##", propsOutputFolder },
                    { "<!--INEDITOR_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", inEditorSearchPaths) },
                    { "<!--PLAYER_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", playerSearchPaths) },
                    { "<!--IS_EDITOR_ONLY_TARGET_TOKEN-->", (ProjectType ==  ProjectType.EditorAsmDef || ProjectType == ProjectType.PredefinedEditorAssembly).ToString() },
                    { "<!--COMMON_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", Compilation.CommonAssemblySearchPaths) },
                    { "<!--DEVELOPMENT_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", Compilation.DevelopmentAssemblySearchPaths) },
                    { "<!--COMMON_DEFINE_CONSTANTS-->", string.Join(";", CompilationSettings.Instance.CommonDefines) },
                    { "<!--COMMON_DEVELOPMENT_DEFINE_CONSTANTS-->", string.Join(";", CompilationSettings.Instance.DevelopmentBuildAdditionalDefines) },
                    { "<!--DEFAULT_PLATFORM_TOKEN-->", CompilationSettings.Instance.AvailablePlatforms[BuildTarget.StandaloneWindows].Name }
                };

                if (Utilities.TryGetXMLTemplate(projectFileTemplateText, "COMMON_REFERENCE", out string commonReferenceTemplate)
                    && Utilities.TryGetXMLTemplate(projectFileTemplateText, "DEVELOPMENT_REFERENCE", out string developmentReferenceTemplate))
                {
                    tokens.Add(commonReferenceTemplate, string.Join("\r\n", Compilation.GetReferenceEntries(commonReferenceTemplate, Compilation.CommonAssemblyReferences)));
                    tokens.Add(developmentReferenceTemplate, string.Join("\r\n", Compilation.GetReferenceEntries(developmentReferenceTemplate, Compilation.DevelopmentAssemblyReferences)));
                }

                projectFileTemplateText = Utilities.ReplaceTokens(projectFileTemplateText, tokens);
            }
            else
            {
                Debug.LogError("Failed to find ProjectReferenceSet and/or Source_Include templates in the project template file.");
            }

            File.WriteAllText(ReferencePath.AbsolutePath, projectFileTemplateText);
        }

        private void ProcessSourceFile(string sourceFile, string sourceIncludeTemplate, List<string> sourceIncludes, Dictionary<string, string> sourceGuidToClassName)
        {
            // Get the entry for the map
            string guid = AssetDatabase.AssetPathToGUID(sourceFile);
            MonoScript asset = AssetDatabase.LoadAssetAtPath<MonoScript>(sourceFile);
            string classNameToAdd = null;
            if (asset != null)
            {
                classNameToAdd = asset.GetClass()?.FullName;
            }
            sourceGuidToClassName.Add(guid, classNameToAdd);


            string normalized = Utilities.NormalizePath(sourceFile);
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

        private string CreateProjectReferencesSet(string template, HashSet<string> additionalSearchPaths, bool inEditor)
        {
            if (Utilities.TryGetXMLTemplate(template, "PROJECT_REFERENCE", out string projectReferenceTemplate)
                && Utilities.TryGetXMLTemplate(template, "PLUGIN_REFERENCE", out string pluginReferenceTemplate))
            {
                List<string> projectReferences = new List<string>();
                foreach (ProjectDependencyBase<CSProjectInfo> dependency in csProjectDependencies)
                {
                    List<string> platformConditions = GetPlatformConditions(inEditor ? InEditorPlatforms : PlayerPlatforms, inEditor ? dependency.InEditorSupportedPlatforms : dependency.PlayerSupportedPlatforms);

                    projectReferences.Add(Utilities.ReplaceTokens(projectReferenceTemplate, new Dictionary<string, string>()
                    {
                        { "##REFERENCE_TOKEN##", $"{dependency.Dependency.Name}.csproj" },
                        { "<!--HINT_PATH_TOKEN-->", dependency.Dependency.ReferencePath.AbsolutePath },
                        { "##CONDITION_TOKEN##", platformConditions.Count == 0 ? "false" : string.Join(" OR ", platformConditions)}
                    }));
                }

                List<string> pluginReferences = new List<string>();
                foreach (ProjectDependencyBase<PluginAssemblyInfo> dependency in pluginAssemblyDependencies)
                {
                    if (dependency.Dependency.Type == PluginType.Native)
                    {
                        //TODO Native plugins aren't yet supported
                        continue;
                    }
                    List<string> platformConditions = GetPlatformConditions(inEditor ? InEditorPlatforms : PlayerPlatforms, inEditor ? dependency.InEditorSupportedPlatforms : dependency.PlayerSupportedPlatforms);

                    pluginReferences.Add(Utilities.ReplaceTokens(pluginReferenceTemplate, new Dictionary<string, string>()
                    {
                        { "##REFERENCE_TOKEN##", dependency.Dependency.Name },
                        { "##CONDITION_TOKEN##", platformConditions.Count == 0 ? "'false'" : string.Join(" OR ", platformConditions)},
                        { "<!--HINT_PATH_TOKEN-->", dependency.Dependency.ReferencePath.AbsolutePath }
                    }));

                    additionalSearchPaths.Add(Path.GetDirectoryName(dependency.Dependency.ReferencePath.AbsolutePath));
                }

                return Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                {
                    {"##REFERENCE_CONFIGURATION_TOKEN##", inEditor ? "InEditor" : "Player" },
                    { projectReferenceTemplate, string.Join("\r\n", projectReferences) },
                    { pluginReferenceTemplate, string.Join("\r\n", pluginReferences) }
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
                    toReturn.Add($"'$(UnityPlatform)' == '{platformName}'");
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