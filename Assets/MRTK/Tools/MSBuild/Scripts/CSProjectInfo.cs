// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// From which source was the project created.
    /// </summary>
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

    /// <summary>
    /// A class representing a CSProject to be outputted.
    /// </summary>
    public class CSProjectInfo : ReferenceItemInfo
    {
        private readonly List<CSProjectDependency<CSProjectInfo>> csProjectDependencies = new List<CSProjectDependency<CSProjectInfo>>();
        private readonly List<CSProjectDependency<PluginAssemblyInfo>> pluginAssemblyDependencies = new List<CSProjectDependency<PluginAssemblyInfo>>();

        /// <summary>
        /// The associated Assembly-Definition info if available.
        /// </summary>
        public AssemblyDefinitionInfo AssemblyDefinitionInfo { get; }

        /// <summary>
        /// The type of the project.
        /// </summary>
        public ProjectType ProjectType { get; }

        /// <summary>
        /// Gets a list of project dependencies.
        /// </summary>
        public IReadOnlyCollection<CSProjectDependency<CSProjectInfo>> ProjectDependencies { get; }

        /// <summary>
        /// Creates a new instance of the CSProject info.
        /// </summary>
        /// <param name="unityProjectInfo">Instance of parsed unity project info.</param>
        /// <param name="guid">The unique Guid of this reference item.</param>
        /// <param name="assemblyDefinitionInfo">The associated Assembly-Definition info.</param>
        /// <param name="assembly">The Unity assembly object associated with this csproj.</param>
        /// <param name="baseOutputPath">The output path where everything will be outputted.</param>
        internal CSProjectInfo(UnityProjectInfo unityProjectInfo, AssemblyDefinitionInfo assemblyDefinitionInfo, string baseOutputPath)
            : base(unityProjectInfo, assemblyDefinitionInfo.Guid, new Uri(Path.Combine(baseOutputPath, $"{assemblyDefinitionInfo.Name}.csproj")), assemblyDefinitionInfo.Name)
        {
            AssemblyDefinitionInfo = assemblyDefinitionInfo;

            ProjectType = GetProjectType(assemblyDefinitionInfo);

            InEditorPlatforms = GetCompilationPlatforms(true);
            PlayerPlatforms = GetCompilationPlatforms(false);

            if (InEditorPlatforms.Count == 0 && PlayerPlatforms.Count == 0)
            {
                Debug.LogError($"The assembly project '{Name}' doesn't contain any supported in-editor or player platform targets.");
            }

            ProjectDependencies = new ReadOnlyCollection<CSProjectDependency<CSProjectInfo>>(csProjectDependencies);
        }

        private ProjectType GetProjectType(AssemblyDefinitionInfo assemblyDefinitionInfo)
        {
            if (!assemblyDefinitionInfo.IsDefaultAssembly)
            {
                return assemblyDefinitionInfo.EditorPlatformSupported && !assemblyDefinitionInfo.NonEditorPlatformSupported ? ProjectType.EditorAsmDef : ProjectType.AsmDef;
            }

            switch (assemblyDefinitionInfo.Name)
            {
                case "Assembly-CSharp":
                case "Assembly-CSharp-firstpass":
                    return ProjectType.PredefinedAssembly;
                case "Assembly-CSharp-Editor":
                case "Assembly-CSharp-Editor-firstpass":
                    return ProjectType.PredefinedEditorAssembly;
                default:
                    throw new InvalidOperationException($"Predefined assembly '{assemblyDefinitionInfo.Name}' was not recognized, this generally means it should be added to the switch statement in CSProjectInfo:GetProjectType. Treating is as a PredefinedAssembly instead of PredefinedEditorAssembly.");
            }
        }

        private ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> GetCompilationPlatforms(bool inEditor)
        {
            // - handle all PredfinedAssemblies
            // - EditorAsmDef and PredefinedEditorAssembly for inEditor
            bool returnAllPlatforms = ProjectType == ProjectType.PredefinedAssembly
                || (inEditor && ProjectType == ProjectType.PredefinedEditorAssembly)
                || (inEditor && ProjectType == ProjectType.EditorAsmDef)
                || (inEditor && ProjectType == ProjectType.AsmDef && AssemblyDefinitionInfo.EditorPlatformSupported);

            if (returnAllPlatforms)
            {
                return new ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo>(UnityProjectInfo.AvailablePlatforms.ToDictionary(t => t.BuildTarget, t => t));
            }

            // - EditorAsmDef and PredefinedEditorAssembly for not inEditor
            bool returnNoPlatforms = (!inEditor && ProjectType == ProjectType.PredefinedEditorAssembly)
                || (!inEditor && ProjectType == ProjectType.EditorAsmDef)
                || (!inEditor && ProjectType == ProjectType.AsmDef && AssemblyDefinitionInfo.TestAssembly);

            if (returnNoPlatforms)
            {
                return new ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo>(new Dictionary<BuildTarget, CompilationPlatformInfo>());
            }

            // We only are an asmdef at this point, as above we handle all PredfinedAssemblies, then EditorAsmDef and PredefinedEditorAssembly for inEditor and not inEditor cases above
            Func<CompilationPlatformInfo, bool> predicate = AssemblyDefinitionInfo.includePlatforms.Length > 0
                ? predicate = (t) => AssemblyDefinitionInfo.includePlatforms.Contains(t.Name)
                : predicate = (t) => !AssemblyDefinitionInfo.excludePlatforms.Contains(t.Name);

            return new ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo>(
                UnityProjectInfo.AvailablePlatforms.Where(predicate)
                    .ToDictionary(t => t.BuildTarget, t => t));
        }

        /// <summary>
        /// Adds a dependency to the project.
        /// </summary>
        /// <param name="csProjectInfo">The C# dependency.</param>
        internal void AddDependency(CSProjectInfo csProjectInfo)
        {
            AddDependency(csProjectDependencies, csProjectInfo);
        }

        /// <summary>
        /// Adds a dependency to the project.
        /// </summary>
        /// <param name="pluginAssemblyInfo">The plugin dependency.</param>
        internal void AddDependency(PluginAssemblyInfo pluginAssemblyInfo)
        {
            AddDependency(pluginAssemblyDependencies, pluginAssemblyInfo);
        }

        private void AddDependency<T>(List<CSProjectDependency<T>> items, T referenceInfo) where T : ReferenceItemInfo
        {
            items.Add(new CSProjectDependency<T>(referenceInfo,
                new HashSet<BuildTarget>(InEditorPlatforms.Keys.Intersect(referenceInfo.InEditorPlatforms.Keys)),
                new HashSet<BuildTarget>(PlayerPlatforms.Keys.Intersect(referenceInfo.PlayerPlatforms.Keys))));
        }

        /// <summary>
        /// Exports the project given a template.
        /// </summary>
        /// <param name="projectFileTemplateText">The template of the csproj file.</param>
        /// <param name="projectFilesPath">The output folder where all the props were added.</param>
        internal void ExportProject(string projectFileTemplateText, string projectFilesPath)
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
                Dictionary<Guid, string> sourceGuidToClassName = new Dictionary<Guid, string>();
                foreach (SourceFileInfo source in AssemblyDefinitionInfo.GetSources())
                {
                    ProcessSourceFile(source, sourceIncludeTemplate, sourceIncludes, sourceGuidToClassName);
                }

                File.WriteAllLines(Path.Combine(projectFilesPath, $"{Guid.ToString()}.csmap"), sourceGuidToClassName.Select(t => $"{t.Key.ToString("N")}:{t.Value}"));

                List<string> supportedPlatformBuildConditions = new List<string>();
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "InEditor", InEditorPlatforms);
                PopulateSupportedPlatformBuildConditions(supportedPlatformBuildConditions, suportedPlatformBuildConditionTemplate, "Player", PlayerPlatforms);

                HashSet<string> inEditorSearchPaths = new HashSet<string>(), playerSearchPaths = new HashSet<string>();
                string projectReferences = string.Join("\r\n", CreateProjectReferencesSet(projectReferenceSetTemplate, inEditorSearchPaths, true), CreateProjectReferencesSet(projectReferenceSetTemplate, playerSearchPaths, false));
                Dictionary<string, string> tokens = new Dictionary<string, string>()
                {
                    { "<!--PROJECT_GUID_TOKEN-->", Guid.ToString() },
                    { "<!--ALLOW_UNSAFE_TOKEN-->", AssemblyDefinitionInfo.allowUnsafeCode.ToString() },
                    { "<!--LANGUAGE_VERSION_TOKEN-->", MSBuildTools.CSharpVersion },

                    { "<!--DEVELOPMENT_BUILD_TOKEN-->", "false" }, // Default to false

                    { "<!--IS_EDITOR_ONLY_TARGET_TOKEN-->", (ProjectType ==  ProjectType.EditorAsmDef || ProjectType == ProjectType.PredefinedEditorAssembly).ToString() },
                    { "<!--UNITY_EDITOR_INSTALL_FOLDER-->", Path.GetDirectoryName(EditorApplication.applicationPath) + "\\"},

                    { "<!--DEFAULT_PLATFORM_TOKEN-->", UnityProjectInfo.AvailablePlatforms.First(t=>t.BuildTarget == BuildTarget.StandaloneWindows).Name },

                    { "<!--SUPPORTED_PLATFORMS_TOKEN-->", string.Join(";", UnityProjectInfo.AvailablePlatforms.Select(t=>t.Name)) },

                    { "<!--INEDITOR_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", inEditorSearchPaths) },
                    { "<!--PLAYER_ASSEMBLY_SEARCH_PATHS_TOKEN-->", string.Join(";", playerSearchPaths) },

                    { "##PLATFORM_PROPS_FOLDER_PATH_TOKEN##", projectFilesPath },

                    { projectReferenceSetTemplate, projectReferences },
                    { sourceIncludeTemplate, string.Join("\r\n", sourceIncludes) },
                    { suportedPlatformBuildConditionTemplate, string.Join("\r\n", supportedPlatformBuildConditions) }
                };

                projectFileTemplateText = Utilities.ReplaceTokens(projectFileTemplateText, tokens, true);
            }
            else
            {
                Debug.LogError("Failed to find ProjectReferenceSet and/or Source_Include templates in the project template file.");
            }

            File.WriteAllText(ReferencePath.AbsolutePath, projectFileTemplateText);
        }

        private void ProcessSourceFile(SourceFileInfo sourceFile, string sourceIncludeTemplate, List<string> sourceIncludes, Dictionary<Guid, string> sourceGuidToClassName)
        {
            // Get the entry for the map
            sourceGuidToClassName.Add(sourceFile.Guid, sourceFile.ClassType?.FullName);

            string linkPath = Utilities.GetRelativePath(AssemblyDefinitionInfo.Directory.FullName, sourceFile.File.FullName);

            string relativeSourcePath;

            switch (sourceFile.AssetLocation)
            {
                case AssetLocation.BuiltInPackage:
                    relativeSourcePath = sourceFile.File.FullName;
                    return;
                case AssetLocation.Project:
                    relativeSourcePath = $"..\\..\\{Utilities.GetAssetsRelativePathFrom(sourceFile.File.FullName)}";
                    break;
                case AssetLocation.Package:
                    relativeSourcePath = $"..\\{Utilities.GetPackagesRelativePathFrom(sourceFile.File.FullName)}";
                    break;
                default: throw new InvalidDataException("Unknown asset location.");
            }

            sourceIncludes.Add(Utilities.ReplaceTokens(sourceIncludeTemplate, new Dictionary<string, string>()
            {
                {"##RELATIVE_SOURCE_PATH##", relativeSourcePath },
                {"##PROJECT_LINK_PATH##", linkPath }
            }));
        }

        private void PopulateSupportedPlatformBuildConditions(List<string> supportedPlatformBuildConditions, string template, string configuration, IReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> platforms)
        {
            foreach (KeyValuePair<BuildTarget, CompilationPlatformInfo> platform in platforms)
            {
                supportedPlatformBuildConditions.Add(Utilities.ReplaceTokens(template, new Dictionary<string, string>()
                {
                    { "##SUPPORTED_CONFIGURATION_TOKEN##", configuration },
                    { "##SUPPORTED_PLATFORM_TOKEN##", platform.Value.Name }
                }));
            }
        }

        private string CreateProjectReferencesSet(string template, HashSet<string> additionalSearchPaths, bool inEditor)
        {
            if (Utilities.TryGetXMLTemplate(template, "PROJECT_REFERENCE", out string projectReferenceTemplate)
                && Utilities.TryGetXMLTemplate(template, "PLUGIN_REFERENCE", out string pluginReferenceTemplate))
            {
                List<string> projectReferences = new List<string>();
                foreach (CSProjectDependency<CSProjectInfo> dependency in csProjectDependencies)
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
                foreach (CSProjectDependency<PluginAssemblyInfo> dependency in pluginAssemblyDependencies)
                {
                    if (dependency.Dependency.Type == PluginType.Native)
                    {
                        continue;
                    }
                    List<string> platformConditions = GetPlatformConditions(inEditor ? InEditorPlatforms : PlayerPlatforms, inEditor ? dependency.InEditorSupportedPlatforms : dependency.PlayerSupportedPlatforms);

                    pluginReferences.Add(Utilities.ReplaceTokens(pluginReferenceTemplate, new Dictionary<string, string>()
                    {
                        { "##REFERENCE_TOKEN##", dependency.Dependency.Name },
                        { "##CONDITION_TOKEN##", platformConditions.Count == 0 ? "false" : string.Join(" OR ", platformConditions)},
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

        private List<string> GetPlatformConditions(IReadOnlyDictionary<BuildTarget, CompilationPlatformInfo> platforms, HashSet<BuildTarget> dependencyPlatforms)
        {
            List<string> toReturn = new List<string>();

            foreach (KeyValuePair<BuildTarget, CompilationPlatformInfo> pair in platforms)
            {
                if (dependencyPlatforms.Contains(pair.Key))
                {
                    string platformName = pair.Value.Name;
                    toReturn.Add($"'$(UnityPlatform)' == '{platformName}'");
                }
            }

            return toReturn;
        }
    }
}
