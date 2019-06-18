using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.Compilation;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class CSProjectInfo
    {
        private readonly List<CSProjectInfo> dependencies = new List<CSProjectInfo>();

        public Guid Guid { get; }

        public Assembly Assembly { get; }

        public AssemblyDefinitionInfo AssemblyDefinitionInfo { get; }

        public string ProjectFilePath { get; private set; }

        internal CSProjectInfo(Guid guid, AssemblyDefinitionInfo assemblyDefinitionInfo, Assembly assembly, string baseOutputPath)
        {
            Guid = guid;
            AssemblyDefinitionInfo = assemblyDefinitionInfo;
            Assembly = assembly;
            ProjectFilePath = Path.Combine(baseOutputPath, $"{assembly.name}.csproj");
        }

        internal void AddDependency(CSProjectInfo csProjectInfo)
        {
            dependencies.Add(csProjectInfo);
        }

        internal void ExportProject(string projectFileTemplateText, string commonPropsFilePath)
        {
            if (File.Exists(ProjectFilePath))
            {
                File.Delete(ProjectFilePath);
            }

            if (Utilities.TryGetXMLTemplate(projectFileTemplateText, "PROJECT_REFERENCE", out string projectReferenceTemplate)
                && Utilities.TryGetXMLTemplate(projectFileTemplateText, "SOURCE_INCLUDE", out string sourceIncludeTemplate))
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

                // Get the assembly references first from AssemblyDefinitionInfo if available (it's actually more correct), otherwise fallback to Assemby
                IEnumerable<string> references = AssemblyDefinitionInfo == null
                    ? Assembly.assemblyReferences.Select(t => t.name)
                    : (AssemblyDefinitionInfo.references ?? Array.Empty<string>());

                projectFileTemplateText = Utilities.ReplaceTokens(projectFileTemplateText, new Dictionary<string, string>()
                {
                    {"<!--PROJECT_GUID_TOKEN-->", Guid.ToString() },
                    {"##COMMON_PROPS_FILE_PATH##", commonPropsFilePath },
                    {"<!--ALLOW_UNSAFE_TOKEN-->", Assembly.compilerOptions.AllowUnsafeCode.ToString() },
                    {"<!--PROJECT_PLATFORMS_TOKEN-->", "StandaloneWindows" },
                    {"<!--DEFAULT_PLATFORM_TOKEN-->", "StandaloneWindows" },
                    {"<!--PROJECT_CONFIGURATIONS_TOKEN-->", "InEditor" },
                    {projectReferenceTemplate, string.Join("\r\n", references.Select(t=>projectReferenceTemplate.Replace("##REFERENCE_TOKEN##", $"{t}.csproj"))) },
                    {sourceIncludeTemplate, string.Join("\r\n", sourceIncludes) }
                });
            }
            else
            {
                Debug.LogError("Failed to find ProjectReference and/or Source_Include templates in the project template file.");
            }

            File.WriteAllText(ProjectFilePath, projectFileTemplateText);
        }

        private string GetProjectReferenceString(string path)
        {
            return $"<ProjectReference Include=\"{path}\" />";
        }
    }
}
