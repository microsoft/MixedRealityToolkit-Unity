#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public enum PluginType
    {
        Managed,
        Native
    }

    public class PluginAssemblyInfo : ReferenceInfo
    {
        public PluginType Type { get; }

        public string AssetsRelativePath { get; }

        public bool AutoReferenced { get; private set; }

        /// <summary>
        /// If the plugin has define constraints, then it will only be referenced if the platform/project defines at least one of these constraints.
        /// ! operator means that the specified plugin must not be included
        /// https://docs.unity3d.com/ScriptReference/PluginImporter.DefineConstraints.html
        /// </summary>
        public HashSet<string> DefineConstraints { get; private set; }

        public PluginAssemblyInfo(Guid guid, string assetsRelativePath)
            : this(guid, assetsRelativePath, Utilities.NormalizePath(Utilities.UnityFolderRelativeToAbsolutePath(assetsRelativePath)))
        {

        }
        public PluginAssemblyInfo(Guid guid, string assetsRelativePath, string fullPath)
            : base(guid, new Uri(fullPath), Path.GetFileNameWithoutExtension(fullPath))
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

            Dictionary<BuildTarget, CompilationSettings.CompilationPlatform> inEditorPlatforms = new Dictionary<BuildTarget, CompilationSettings.CompilationPlatform>();
            if (enabledPlatforms.TryGetValue("Editor", out bool platformEnabled) && platformEnabled)
            {
                foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> pair in CompilationSettings.Instance.AvailablePlatforms)
                {
                    inEditorPlatforms.Add(pair.Key, pair.Value);
                }
            }

            Dictionary<BuildTarget, CompilationSettings.CompilationPlatform> playerPlatforms = new Dictionary<BuildTarget, CompilationSettings.CompilationPlatform>();

            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Win", BuildTarget.StandaloneWindows);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Win64", BuildTarget.StandaloneWindows64);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "WindowsStoreApps", BuildTarget.WSAPlayer);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "iOS", BuildTarget.iOS);
            TryAddEnabledPlatform(playerPlatforms, enabledPlatforms, "Android", BuildTarget.Android);

            FilterPlatformsBasedOnDefineConstraints(inEditorPlatforms, true);
            FilterPlatformsBasedOnDefineConstraints(playerPlatforms, false);

            InEditorPlatforms = new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(inEditorPlatforms);
            PlayerPlatforms = new ReadOnlyDictionary<BuildTarget, CompilationSettings.CompilationPlatform>(playerPlatforms);
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

        private bool ContainsDefineHelper(string define, bool inEditor)
        {
            return CompilationSettings.Instance.CommonDefines.Contains(define)
                || (inEditor && CompilationSettings.Instance.DevelopmentBuildAdditionalDefines.Contains(define));
        }

        private bool ContainsDefineHelper(string define, bool inEditor, CompilationSettings.CompilationPlatform platform)
        {
            return ContainsDefineHelper(define, inEditor)
                || platform.CommonPlatformDefines.Contains(define)
                || (inEditor ? platform.AdditionalInEditorDefines.Contains(define) : platform.AdditionalPlayerDefines.Contains(define));
        }

        private void FilterPlatformsBasedOnDefineConstraints(IDictionary<BuildTarget, CompilationSettings.CompilationPlatform> platformDictionary, bool inEditor)
        {
            if (DefineConstraints.Count == 0)
            {
                // No exclusions
                return;
            }

            bool defaultExcludeValue = DefineConstraints.Any(t => !t.StartsWith("!"));
            HashSet<BuildTarget> toExclude = new HashSet<BuildTarget>();
            foreach (KeyValuePair<BuildTarget, CompilationSettings.CompilationPlatform> platformPair in platformDictionary)
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

        private void TryAddEnabledPlatform(Dictionary<BuildTarget, CompilationSettings.CompilationPlatform> playerPlatforms, Dictionary<string, bool> enabledPlatforms, string platformName, BuildTarget platformTarget)
        {
            if (enabledPlatforms.TryGetValue(platformName, out bool platformEnabled) && platformEnabled)
            {
                if (CompilationSettings.Instance.AvailablePlatforms.TryGetValue(platformTarget, out CompilationSettings.CompilationPlatform platform))
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