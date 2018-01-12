// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace MixedRealityToolkit.Build
{
    /// <summary>
    /// This class is designed to post process the UWP Assembly-CSharp projects to ensure that the defaults and defines are set correctly.
    /// </summary>
    public static class UwpProjectPostProcess
    {
        private static readonly Regex PlatformRegex = new Regex(@"'\$\(Configuration\)\|\$\(Platform\)' == '(?<Configuration>.*)\|(?<Platform>.*)'");

        /// <summary>
        /// Executes the Post Processes on the C# Projects generated as part of the UWP build.
        /// </summary>
        /// <param name="buildRootPath">The root path of the UWP build output.</param>
        public static void Execute(string buildRootPath)
        {
            UpdateProjectFile(Path.Combine(buildRootPath, @"GeneratedProjects\UWP\Assembly-CSharp\Assembly-CSharp.csproj"));
            UpdateProjectFile(Path.Combine(buildRootPath, @"GeneratedProjects\UWP\Assembly-CSharp-firstpass\Assembly-CSharp-firstpass.csproj"));
        }

        /// <summary>
        /// Updates the project file to ensure that certain requirements are met.
        /// </summary>
        /// <param name="filename">The filename of the project to update.</param>
        /// <remarks>This is manually parsing the Unity generated MSBuild projects, which means it will be fragile to changes.</remarks>
        private static void UpdateProjectFile(string filename)
        {
            if (!File.Exists(filename))
            {
                UnityEngine.Debug.LogWarningFormat("Unable to find file \"{0}\", double check that the build succeeded and that the C# Projects are set to be generated.", filename);
                return;
            }

            var projectDocument = new XmlDocument();
            projectDocument.Load(filename);

            if (projectDocument.DocumentElement == null)
            {
                UnityEngine.Debug.LogWarningFormat("Unable to load file \"{0}\", double check that the build succeeded and that the C# Projects are set to be generated.", filename);
                return;
            }

            if (projectDocument.DocumentElement.Name != "Project")
            {
                UnityEngine.Debug.LogWarningFormat("The loaded project \"{0}\", does not appear to be a MSBuild Project file.", filename);
                return;
            }

            foreach (XmlNode node in projectDocument.DocumentElement.ChildNodes)
            {
                // Everything we are looking for is inside a PropertyGroup...
                if (node.Name != "PropertyGroup" || node.Attributes == null)
                {
                    continue;
                }

                if (node.Attributes.Count == 0 && node["Configuration"] != null && node["Platform"] != null)
                {
                    // Update the defaults to Release and x86 so that we can run NuGet restore.
                    node["Configuration"].InnerText = "Release";
                    node["Platform"].InnerText = "x86";
                }
                else if (node.Attributes["Condition"] != null)
                {
                    // Update the DefineConstants to include the configuration allowing us to conditionally compile code based on the configuration.
                    Match match = PlatformRegex.Match(node.Attributes["Condition"].InnerText);

                    if (match.Success)
                    {
                        UpdateDefineConstants(node["DefineConstants"], match.Groups["Configuration"].Value, match.Groups["Platform"].Value);
                    }
                }
            }

            WriteXmlDocumentToFile(projectDocument, filename);
        }

        private static void UpdateDefineConstants(XmlNode defineConstants, string configuration, string platform)
        {
            if (defineConstants == null)
            {
                return;
            }

            IEnumerable<string> symbols = defineConstants.InnerText.Split(';').Except(new[]
            {
                string.Empty,
                BuildSLNUtilities.BuildSymbolDebug,
                BuildSLNUtilities.BuildSymbolRelease,
                BuildSLNUtilities.BuildSymbolMaster
            }).Union(new[] { configuration.ToUpperInvariant() });

            defineConstants.InnerText = string.Join(";", symbols.ToArray());
            //UnityEngine.Debug.LogFormat("Updating defines for Configuration|Platform: {0}|{1} => {2}", configuration, platform, defineConstants.InnerText);
        }

        private static void WriteXmlDocumentToFile(XmlNode document, string fullPath)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = File.Open(fullPath, FileMode.Create);

                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    CloseOutput = true
                };

                using (XmlWriter writer = XmlWriter.Create(fileStream, settings))
                {
                    fileStream = null;
                    document.WriteTo(writer);
                }
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
            }
        }
    }
}
