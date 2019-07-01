using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class TemplateFiles
    {
        private const string TemplateFilesFolderName = "MSBuildTemplates";
        private const string MSBuildSolutionTemplateName = "SolutionTemplate.sln";
        private const string SDKProjectFileTemplateName = "SDKProjectTemplate.csproj";
        private const string PlatformPropsTemplateName = "Platform.Configuration.Template.props";
        private const string EditorPropsTemplateName = "Editor.InEditor.Template.props";
        private const string SpecifcPlatformPropsTemplateRegex = @"[a-zA-Z]+\.[a-zA-Z]+\.Template\.props";

        private const string GeneratedCSMetaTemplateName = "GereneratedCSMeta.template";

        private static TemplateFiles instance;

        public static TemplateFiles Instance => instance ?? (instance = new TemplateFiles());

        public static string GetPlatformSpecificTemplateName(string platform, string configuration = "Configuration")
        {
            return $"{platform}.{configuration}.Template.props";
        }

        private string generatedCSMetaTemplateText;

        public string MSBuildSolutionTemplatePath { get; }

        public string SDKProjectFileTemplatePath { get; }

        public string PlatformPropsTemplatePath { get; }

        public string GeneratedCSMetaTemplatePath { get; }

        public IReadOnlyDictionary<string, string> PlatformTemplates { get; }

        public IReadOnlyList<string> OtherFiles { get; }

        public string GeneratedCSMetaTemplateText => generatedCSMetaTemplateText ?? (generatedCSMetaTemplateText = File.ReadAllText(GeneratedCSMetaTemplatePath));

        private TemplateFiles()
        {
            string[] templateFolders = AssetDatabase.FindAssets(TemplateFilesFolderName);
            Utilities.GetPathsFromGuidsInPlace(templateFolders);

            if (templateFolders.Length == 0)
            {
                Debug.LogError($"Templates folder '{TemplateFilesFolderName}' not found.");
            }
            else if (templateFolders.Length > 1)
            {
                Debug.LogWarning($"Stange, more than one directory exists for template files:\n {string.Join("\n", templateFolders)}");
            }

            string[] files = AssetDatabase.FindAssets("*", templateFolders);
            Utilities.GetPathsFromGuidsInPlace(files);

            Dictionary<string, string> fileNamesMaps = files.ToDictionary(t => Path.GetFileName(t));

            MSBuildSolutionTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "MSBuild Solution", MSBuildSolutionTemplateName);
            SDKProjectFileTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "SDK Project", SDKProjectFileTemplateName);
            PlatformPropsTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "Platform Props", PlatformPropsTemplateName);
            GeneratedCSMetaTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "Generated CS Template Name", GeneratedCSMetaTemplateName);

            // Get specific platforms
            Dictionary<string, string> platformTemplates = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> pair in fileNamesMaps)
            {
                if (Regex.IsMatch(pair.Key, SpecifcPlatformPropsTemplateRegex))
                {
                    platformTemplates.Add(pair.Key, pair.Value);
                }
            }

            foreach (KeyValuePair<string, string> pair in platformTemplates)
            {
                fileNamesMaps.Remove(pair.Key);
            }

            PlatformTemplates = new ReadOnlyDictionary<string, string>(platformTemplates);

            OtherFiles = new ReadOnlyCollection<string>(fileNamesMaps.Values.ToList());
        }

        private string GetExpectedTemplatesPath(Dictionary<string, string> fileNamesMaps, string displayName, string fileName)
        {
            if (fileNamesMaps.TryGetValue(fileName, out string path))
            {
                fileNamesMaps.Remove(fileName);
                return path;
            }
            else
            {
                Debug.LogError($"Could not find {displayName} template with filename '{fileName}'");
                return string.Empty;
            }
        }
    }
}
