// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
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
    /// Type of Plugin.
    /// </summary>
    public enum PluginType
    {
        /// <summary>
        /// A .NET DLL.
        /// </summary>
        Managed,

        /// <summary>
        /// A native (C++) dll.
        /// </summary>
        Native
    }

    /// <summary>
    /// This is the information for the plugins in the Unity project.
    /// </summary>
    public class PluginAssemblyInfo : ReferenceItemInfo
    {
        /// <summary>
        /// Ges the type of Plugin.
        /// </summary>
        public PluginType Type { get; }

        /// <summary>
        /// Gets the path relative to the assets folder.
        /// </summary>
        public string AssetsRelativePath { get; }

        /// <summary>
        /// Gets whether this plugin is auto referenced.
        /// </summary>
        public bool AutoReferenced { get; private set; }

        /// <summary>
        /// If the plugin has define constraints, then it will only be referenced if the platform/project defines at least one of these constraints.
        /// ! operator means that the specified plugin must not be included
        /// https://docs.unity3d.com/ScriptReference/PluginImporter.DefineConstraints.html
        /// </summary>
        public HashSet<string> DefineConstraints { get; private set; }

        ///// <summary>
        ///// Creates a new instance of the <see cref="PluginAssemblyInfo"/>.
        ///// </summary>
        ///// <param name="availablePlatforms"></param>
        ///// <param name="guid"></param>
        ///// <param name="assetsRelativePath"></param>
        //public PluginAssemblyInfo(IEnumerable<CompilationPlatformInfo> availablePlatforms, Guid guid, string assetsRelativePath)
        //    : this(availablePlatforms, guid, assetsRelativePath, Path.GetFullPath(Utilities.GetFullPathFromKnownRelative(assetsRelativePath)))
        //{

        //}

        /// <summary>
        /// Creates a new instance of the <see cref="PluginAssemblyInfo"/>.
        /// </summary>
        public PluginAssemblyInfo(IEnumerable<CompilationPlatformInfo> availablePlatforms, Guid guid, string assetsRelativePath, string fullPath)
            : base(availablePlatforms, guid, new Uri(fullPath), Path.GetFileNameWithoutExtension(fullPath))
        {
            AssetsRelativePath = assetsRelativePath;
            PluginImporter importer = (PluginImporter)AssetImporter.GetAtPath(AssetsRelativePath);

            Type = importer.isNativePlugin ? PluginType.Native : PluginType.Managed;

            ParseYAMLFile();
        }

        private void ParseYAMLFile()
        {
            Dictionary<string, bool> enabledPlatforms = new Dictionary<string, bool>();
            using (StreamReader reader = new StreamReader(ReferencePath.AbsolutePath + ".meta"))
            {
                DefineConstraints = new HashSet<string>();

                // Parse define constraints
                string defineConstraints = ReadUntil(reader, "defineConstraints:");
                if (!defineConstraints.Contains("[]"))
                {
                    ReadWhile(reader, line =>
                    {
                        line = line.Trim();
                        if (line.StartsWith("-"))
                        {
                            string define = line.Substring(1).Trim();

                            if (define.StartsWith("'") && define.EndsWith("'"))
                            {
                                define = define.Substring(1, define.Length - 2);
                            }

                            DefineConstraints.Add(define);
                            return true;
                        }
                        // else
                        return false;
                    });
                }

                // Read until isExplicitlyReferenced
                string isExplicitlyReferenced = ReadUntil(reader, "isExplicitlyReferenced:");
                AutoReferenced = isExplicitlyReferenced.Split(':')[1].Trim().Equals("0");

                // Read until platform data
                ReadUntil(reader, "platformData:");

                ParsePlatformData(reader, enabledPlatforms);
            }

            Dictionary<BuildTarget, CompilationPlatformInfo> inEditorPlatforms = new Dictionary<BuildTarget, CompilationPlatformInfo>();
            if (enabledPlatforms.TryGetValue("Editor", out bool platformEnabled) && platformEnabled)
            {
                foreach (CompilationPlatformInfo platform in availablePlatforms)
                {
                    inEditorPlatforms.Add(platform.BuildTarget, platform);
                }
            }

            Dictionary<BuildTarget, CompilationPlatformInfo> playerPlatforms = new Dictionary<BuildTarget, CompilationPlatformInfo>();

            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Win", BuildTarget.StandaloneWindows);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Win64", BuildTarget.StandaloneWindows64);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "WindowsStoreApps", BuildTarget.WSAPlayer);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "iOS", BuildTarget.iOS);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Android", BuildTarget.Android);

            FilterPlatformsBasedOnDefineConstraints(inEditorPlatforms, true);
            FilterPlatformsBasedOnDefineConstraints(playerPlatforms, false);

            InEditorPlatforms = new ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo>(inEditorPlatforms);
            PlayerPlatforms = new ReadOnlyDictionary<BuildTarget, CompilationPlatformInfo>(playerPlatforms);
        }

        private void ParsePlatformData(StreamReader reader, Dictionary<string, bool> enabledPlatforms)
        {
            if (ReadUntil(reader, "first:", "userData:").Contains("userData:") || reader.EndOfStream)
            {
                // We reached the end
                return;
            }

            if (reader.ReadLine().Contains("'': Any")) // Try use exclude method
            {
                string settingsLine = ReadUntil(reader, "settings:", "userData:");
                if (settingsLine.Contains("userData:"))
                {
                    return;
                }

                // We are fine to use exclude method if we have a set of settings
                if (!settingsLine.Contains("settings: {}"))
                {
                    ReadWhile(reader, l =>
                    {
                        if (l.Contains("Exclude"))
                        {
                            string[] parts = l.Trim().Replace("Exclude ", string.Empty).Split(':');
                            enabledPlatforms.Add(parts[0], parts[1].Trim() == "0"); // These are exclude, so check for 0 if to include
                            return true;
                        }

                        return false;
                    });

                    return;
                }
            }
            // else fall through to use -first method 

            string line;
            while ((line = ReadUntil(reader, "first:", "userData:")).Contains("first:") && !reader.EndOfStream)
            {
                string[] platformLineParts = reader.ReadLine().Split(':');
                string platform = platformLineParts[1].Trim();

                if (platformLineParts[0].Contains("Facebook"))
                {
                    platform = $"Facebook{platform}";
                }
                string enabledLine = ReadUntil(reader, "enabled:");

                enabledPlatforms.Add(platform, enabledLine.Split(':')[1].Trim() == "1");
            }
        }

        private bool ContainsDefineHelper(string define, bool inEditor, CompilationPlatformInfo platform)
        {
            return platform.CommonPlatformDefines.Contains(define)
                || (inEditor ? platform.AdditionalInEditorDefines.Contains(define) : platform.AdditionalPlayerDefines.Contains(define));
        }

        private void FilterPlatformsBasedOnDefineConstraints(IDictionary<BuildTarget, CompilationPlatformInfo> platformDictionary, bool inEditor)
        {
            if (DefineConstraints.Count == 0)
            {
                // No exclusions
                return;
            }

            bool defaultExcludeValue = DefineConstraints.Any(t => !t.StartsWith("!"));
            HashSet<BuildTarget> toExclude = new HashSet<BuildTarget>();
            foreach (KeyValuePair<BuildTarget, CompilationPlatformInfo> platformPair in platformDictionary)
            {
                // We presume exclude, then check
                bool exclude = defaultExcludeValue;
                foreach (string define in DefineConstraints)
                {
                    // Does this define exclude
                    if (define.StartsWith("!"))
                    {
                        if (ContainsDefineHelper(define.Substring(1), inEditor, platformPair.Value))
                        {
                            exclude = true;
                            break;
                        }
                    }
                    else if (ContainsDefineHelper(define, inEditor, platformPair.Value))
                    {
                        // This platform is supported, but still search for !defineconstraitns that may force exclusion
                        exclude = false;
                    }
                }

                if (exclude)
                {
                    toExclude.Add(platformPair.Key);
                }
            }

            foreach (BuildTarget buildTarget in toExclude)
            {
                platformDictionary.Remove(buildTarget);
            }
        }

        private void TryAddEnabledPlatform(Dictionary<BuildTarget, CompilationPlatformInfo> playerPlatforms, Dictionary<string, bool> enabledPlatforms, string platformName, BuildTarget platformTarget)
        {
            if (enabledPlatforms.TryGetValue(platformName, out bool platformEnabled) && platformEnabled)
            {
                CompilationPlatformInfo platform = availablePlatforms.FirstOrDefault(t => t.BuildTarget == platformTarget);
                if (platform != null)
                {
                    playerPlatforms.Add(platformTarget, platform);
                }
                else
                {
                    Debug.LogError($"Platform '{platformName}' was specified as enabled by '{ReferencePath.AbsolutePath}' plugin, but not available in processed compilation settings.");
                }
            }
        }

        private string ReadUntil(StreamReader reader, params string[] contents)
        {
            return ReadWhile(reader, line => !contents.Any(c => line.Contains(c)));
        }

        private string ReadWhile(StreamReader reader, System.Func<string, bool> predicate)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!predicate(line))
                {
                    return line;
                }
            }

            return string.Empty;
        }
    }
}
#endif