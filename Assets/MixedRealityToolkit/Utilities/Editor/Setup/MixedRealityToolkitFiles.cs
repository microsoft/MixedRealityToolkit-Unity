// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// Base folder types for modules searched by the MixedRealityToolkitFiles utility.
    /// </summary>
    public enum MixedRealityToolkitModuleType
    {
        Core,
        Generated,
        Providers,
        Services,
        SDK,
        Examples,
        Tests,
    }

    /// <summary>
    /// API for working with MixedRealityToolkit folders contained in the project.
    /// </summary>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFiles
    {
        /// <summary>
        /// In order to subscribe for a <see cref="OnPostprocessAllAssets(string[], string[], string[], string[])"/> callback, 
        /// the class declaring the method must derive from AssetPostprocessor. So this class is nested privately as to prevent instantiation of it.
        /// </summary>
        private class AssetPostprocessor : UnityEditor.AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                searchForFoldersTask.Wait();

                foreach (string asset in importedAssets.Concat(movedAssets))
                {
                    string folder = asset.Replace("Assets", Application.dataPath);
                    RegisterModuleFolder(folder);
                }

                foreach (string asset in deletedAssets.Concat(movedFromAssetPaths))
                {
                    string folder = asset.Replace("Assets", Application.dataPath);
                    UnregisterModuleFolder(folder);
                }
            }
        }

        private readonly static Dictionary<MixedRealityToolkitModuleType, HashSet<string>> mrtkFolders =
            new Dictionary<MixedRealityToolkitModuleType, HashSet<string>>();
        private readonly static Task searchForFoldersTask;

        /// <summary>
        /// Returns a collection of MRTK directories found in the project.
        /// </summary>
        public static IEnumerable<string> MRTKDirectories => GetDirectories(MixedRealityToolkitModuleType.Core);

        public static IEnumerable<string> GetDirectories(MixedRealityToolkitModuleType module)
        {
            if (mrtkFolders.TryGetValue(module, out HashSet<string> folders))
            {
                return folders;
            }
            return null;
        }

        /// <summary>
        /// Are any of the MRTK directories available?
        /// </summary>
        public static bool AreFoldersAvailable
        {
            get
            {
                searchForFoldersTask.Wait();
                return mrtkFolders.Count > 0;
            }
        }

        static MixedRealityToolkitFiles()
        {
            string path = Application.dataPath;
            searchForFoldersTask = Task.Run(() => SearchForFoldersAsync(path));
        }

        private static void SearchForFoldersAsync(string rootPath)
        {
            foreach (string folder in Directory.EnumerateDirectories(rootPath))
            {
                RegisterModuleFolder(folder);
            }
        }

        private static void RegisterModuleFolder(string folder)
        {
            string normalizedFolder = NormalizeSeparators(folder);
            if (FindMatchingModule(normalizedFolder, out MixedRealityToolkitModuleType module))
            {
                if (!mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
                {
                    modFolders = new HashSet<string>();
                    mrtkFolders.Add(module, modFolders);
                }
                modFolders.Add(normalizedFolder);
            }
        }

        private static void UnregisterModuleFolder(string folder)
        {
            string normalizedFolder = NormalizeSeparators(folder);
            foreach (var modFolders in mrtkFolders)
            {
                if (modFolders.Value.Remove(normalizedFolder))
                {
                    if (modFolders.Value.Count == 0)
                    {
                        mrtkFolders.Remove(modFolders.Key);
                    }
                }
            }
        }

        private static string NormalizeSeparators(string path) => path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        private static string FormatSeparatorsForUnity(string path) => path.Replace('\\', '/');

        /// <summary>
        /// Maps an absolute path to be relative to the Project Root path (the Unity folder that contains Assets)
        /// </summary>
        /// <param name="absolutePath">The absolute path to the project/</param>
        /// <returns>The project relative path.</returns>
        /// <remarks>This doesn't produce paths that contain step out '..' relative paths.</remarks>
        public static string GetAssetDatabasePath(string absolutePath) => FormatSeparatorsForUnity(absolutePath).Replace(Application.dataPath, "Assets");

        /// <summary>
        /// Returns files from all folder instances of the MRTK folder relative path.
        /// </summary>
        /// <param name="mrtkRelativeFolder">The MRTK folder relative path to the target folder.</param>
        /// <returns>The array of files.</returns>
        public static string[] GetFiles(string mrtkRelativeFolder)
        {
            return GetFiles(MixedRealityToolkitModuleType.Core, mrtkRelativeFolder);
        }

        /// <summary>
        /// Returns files from all folder instances of the MRTK folder relative path.
        /// </summary>
        /// <param name="mrtkRelativeFolder">The MRTK folder relative path to the target folder.</param>
        /// <returns>The array of files.</returns>
        public static string[] GetFiles(MixedRealityToolkitModuleType module, string mrtkRelativeFolder)
        {
            if (!AreFoldersAvailable)
            {
                Debug.LogError("Failed to locate MixedRealityToolkit folders in the project.");
                return null;
            }

            if (mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
            {
                return modFolders
                    .Select(t => Path.Combine(t, mrtkRelativeFolder))
                    .Where(Directory.Exists)
                    .SelectMany(t => Directory.GetFiles(t))
                    .Select(GetAssetDatabasePath)
                    .ToArray();
            }
            return null;
        }

        /// <summary>
        /// Maps a single relative path file to a concrete path from one of the MRTK folders, if found. Otherwise returns null.
        /// </summary>
        /// <param name="mrtkPathToFile">The MRTK folder relative path to the file.</param>
        /// <returns>The project relative path to the file.</returns>
        public static string MapRelativeFilePath(string mrtkPathToFile)
        {
            return MapRelativeFilePath(MixedRealityToolkitModuleType.Core, mrtkPathToFile);
        }

        /// <summary>
        /// Maps a single relative path file to a concrete path from one of the MRTK folders, if found. Otherwise returns null.
        /// </summary>
        /// <param name="mrtkPathToFile">The MRTK folder relative path to the file.</param>
        /// <returns>The project relative path to the file.</returns>
        public static string MapRelativeFilePath(MixedRealityToolkitModuleType module, string mrtkPathToFile)
        {
            if (!AreFoldersAvailable)
            {
                Debug.LogError("Failed to locate MixedRealityToolkit folders in the project.");
                return null;
            }

            if (mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
            {
                string path = modFolders
                    .Select(t => Path.Combine(t, mrtkPathToFile))
                    .FirstOrDefault(t => File.Exists(t));
                return path != null ? GetAssetDatabasePath(path) : null;
            }
            return null;
        }

        private static readonly Dictionary<string, MixedRealityToolkitModuleType> moduleNameMap = new Dictionary<string, MixedRealityToolkitModuleType>()
        {
            { "", MixedRealityToolkitModuleType.Core },
            { "Generated", MixedRealityToolkitModuleType.Generated },
            { "Providers", MixedRealityToolkitModuleType.Providers },
            { "Services", MixedRealityToolkitModuleType.Services },
            { "SDK", MixedRealityToolkitModuleType.SDK },
            { "Examples", MixedRealityToolkitModuleType.Examples },
            { "Tests", MixedRealityToolkitModuleType.Tests }
        };

        private static bool FindMatchingModule(string folder, out MixedRealityToolkitModuleType result)
        {
            // Matches an optional module suffix, e.g. ".Services"
            const string modulePattern = @"(\.(?<module>\w+))?";
            // Matches a version string, e.g. "2.0.0-20190611.2"
            const string versionPattern = @"(?<version>[.\-0-9]+)";
            // Matches the naming pattern in the MRTK repository
            // e.g. "MixedRealityToolkit.Services"
            const string mrtkPattern = @"MixedRealityToolkit" + modulePattern;
            // Matches "Microsoft.MixedReality.Toolkit", followed by optional module name, followed by version number
            // e.g.: "Microsoft.MixedReality.Toolkit.Services.2.0.0-20190611.2"
            // This alternate path is used if above isn't found. This is to work around long paths issue with NuGetForUnity
            // https://github.com/GlitchEnzo/NuGetForUnity/issues/246
            const string nugetParentPattern = @"Microsoft\.MixedReality\.Toolkit" + modulePattern + @"\." + versionPattern;

            var dirInfo = new DirectoryInfo(folder);

            if (TryMatchFolderPattern(dirInfo.Name, mrtkPattern, out result))
            {
                return true;
            }
            else if (TryMatchFolderPattern(dirInfo.Parent.Name, nugetParentPattern, out result))
            {
                return true;
            }
            else
            {
                result = MixedRealityToolkitModuleType.Core;
                return false;
            }
        }

        private static bool TryMatchFolderPattern(string name, string pattern, out MixedRealityToolkitModuleType result)
        {
            var folderMatches = System.Text.RegularExpressions.Regex.Matches(name, pattern);
            if (folderMatches.Count == 1)
            {
                var moduleName = folderMatches[0].Groups["module"].Value;
                if (moduleNameMap.TryGetValue(moduleName, out result))
                {
                    Debug.Log($"Found module path for {result}: {name}");
                    return true;
                }
                else
                {
                    Debug.LogWarning($"Found MRTK folder with unknown module extension \"{moduleName}\"");
                    return false;
                }
            }

            result = MixedRealityToolkitModuleType.Core;
            return false;
        }
    }
}
