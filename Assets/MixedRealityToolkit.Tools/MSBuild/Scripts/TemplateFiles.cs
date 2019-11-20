// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// A helper class to manage (and locate) all the templates.
    /// </summary>
    public class TemplateFiles
    {
        private const string TemplateFilesFolderName = "MSBuildTemplates";
        private const string MSBuildSolutionTemplateName = "SolutionTemplate.sln.template";
        private const string SDKProjectFileTemplateName = "SDKProjectTemplate.csproj.template";
        private const string PlatformPropsTemplateName = "Platform.Configuration.Template.props.template";
        private const string EditorPropsTemplateName = "Editor.InEditor.Template.props.template";
        private const string SpecifcPlatformPropsTemplateRegex = @"[a-zA-Z]+\.[a-zA-Z]+\.Template\.props.template";
        private const string PluginMetaFileTemplateRegex = @"Plugin\.([a-zA-Z]*)\.meta.template";

        private static TemplateFiles instance;

        /// <summary>
        /// Gets the singleton instance (created on demand) of this class.
        /// </summary>
        public static TemplateFiles Instance => instance ?? (instance = new TemplateFiles());

        /// <summary>
        /// Gets the MSBuild Solution file (.sln) template path.
        /// </summary>
        public string MSBuildSolutionTemplatePath { get; }

        /// <summary>
        /// Gets the MSBuild C# SDK Project file (.csproj) template path.
        /// </summary>
        public string SDKProjectFileTemplatePath { get; }

        /// <summary>
        /// Gets the MSBuild Platform Props file (.props) template path.
        /// </summary>
        public string PlatformPropsTemplatePath { get; }

        /// <summary>
        /// Gets a list of specialized platform templates.
        /// </summary>
        public IReadOnlyDictionary<string, string> PlatformTemplates { get; }

        /// <summary>
        /// Gets a list of meta files for plugins templates.
        /// </summary>
        public IReadOnlyDictionary<BuildTargetGroup, FileInfo> PluginMetaTemplatePaths { get; }

        /// <summary>
        /// Gets a list of all other files included among the templates.
        /// </summary>
        public IReadOnlyList<string> OtherFiles { get; }

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
                Debug.LogWarning($"Strange, more than one directory exists for template files:\n {string.Join("\n", templateFolders)}");
            }

            string[] files = AssetDatabase.FindAssets("*", templateFolders);
            Utilities.GetPathsFromGuidsInPlace(files, fullPaths: true);

            Dictionary<string, string> fileNamesMaps = files.ToDictionary(t => Path.GetFileName(t));

            MSBuildSolutionTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "MSBuild Solution", MSBuildSolutionTemplateName);
            SDKProjectFileTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "SDK Project", SDKProjectFileTemplateName);
            PlatformPropsTemplatePath = GetExpectedTemplatesPath(fileNamesMaps, "Platform Props", PlatformPropsTemplateName);

            // Get specific platforms
            Dictionary<string, string> platformTemplates = new Dictionary<string, string>();
            Dictionary<BuildTargetGroup, FileInfo> metaFileTemplates = new Dictionary<BuildTargetGroup, FileInfo>();

            HashSet<string> toRemove = new HashSet<string>();
            foreach (KeyValuePair<string, string> pair in fileNamesMaps)
            {
                if (Regex.IsMatch(pair.Key, SpecifcPlatformPropsTemplateRegex))
                {
                    platformTemplates.Add(pair.Key, pair.Value);
                    toRemove.Add(pair.Key);
                }
                else
                {
                    Match match = Regex.Match(pair.Key, PluginMetaFileTemplateRegex);
                    if (match.Success)
                    {
                        string value = match.Groups[1].Captures[0].Value;
                        FileInfo fileInfo = new FileInfo(pair.Value);
                        if (Equals(value, "Editor"))
                        {
                            metaFileTemplates.Add(BuildTargetGroup.Unknown, fileInfo);
                        }
                        else if (Enum.TryParse(value, out BuildTargetGroup buildTargetGroup))
                        {
                            metaFileTemplates.Add(buildTargetGroup, fileInfo);
                        }
                        else
                        {
                            Debug.LogError($"Matched meta template but failed to parse it: {pair.Key}");
                        }

                        toRemove.Add(pair.Key);
                    }
                }
            }

            foreach (string item in toRemove)
            {
                fileNamesMaps.Remove(item);
            }

            PlatformTemplates = new ReadOnlyDictionary<string, string>(platformTemplates);
            PluginMetaTemplatePaths = new ReadOnlyDictionary<BuildTargetGroup, FileInfo>(metaFileTemplates);

            OtherFiles = new ReadOnlyCollection<string>(fileNamesMaps.Values.ToList());
        }

        /// <summary>
        /// Gets the correct platform template file path.
        /// </summary>
        /// <param name="platform">The platform of the requested template.</param>
        /// <param name="configuration">The configuration of the requested template.</param>
        /// <returns>The absolute file path for the platform template to use.</returns>
        public string GetTemplateFilePathForPlatform(string platform, string configuration)
        {
            if (PlatformTemplates.TryGetValue($"{platform}.{configuration}.Template.props.template", out string templatePath)
                || PlatformTemplates.TryGetValue($"{platform}.Configuration.Template.props.template", out templatePath))
            {
                return templatePath;
            }
            else
            {
                return PlatformPropsTemplatePath;
            }
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
