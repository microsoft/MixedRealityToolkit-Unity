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
        Extensions,
        Tools,
        // This module only exists for testing purposes, and is used in edit mode tests in conjunction
        // with MixedRealityToolkitFiles to ensure that this class is able to reason over MRTK
        // files that are placed outside of the root asset folder.
        AdhocTesting,
    }

    /// <summary>
    /// API for working with MixedRealityToolkit folders contained in the project.
    /// </summary>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFiles
    {
        /// <summary>
        /// This controls the behavior of MapRelativePathToAbsolutePath.
        /// </summary>
        private enum SearchType
        {
            /// <summary>
            /// This indicates
            /// </summary>
            File,
            Folder,
        }

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
                    string folder = ResolveFullAssetsPath(asset);
                    TryRegisterModuleFolder(folder);
                }

                foreach (string asset in deletedAssets.Concat(movedFromAssetPaths))
                {
                    string folder = ResolveFullAssetsPath(asset);
                    TryUnregisterModuleFolder(folder);
                }
            }
        }

        private readonly static Dictionary<MixedRealityToolkitModuleType, HashSet<string>> mrtkFolders =
            new Dictionary<MixedRealityToolkitModuleType, HashSet<string>>();
        private readonly static Task searchForFoldersTask;

        /// <summary>
        /// Resolves the given asset to its full path if and only if the asset belongs to the
        /// Assets folder (i.e. it is prefixed with "Assets/..."
        /// </summary>
        /// <remarks>
        /// If not associated with the Assets folder, will return the path unchanged.
        /// </remarks>
        private static string ResolveFullAssetsPath(string path)
        {
            if (path.StartsWith("Assets"))
            {
                // asset.Substring(6) represents the characters after the "Assets" string.
                return Application.dataPath + path.Substring(6);
            }
            return path;
        }

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

        /// <summary>
        /// Directory levels to search for MRTK folders below the root directory.
        /// </summary>
        /// <remarks>
        /// E.g. with level 3 and folders ROOT/A/B/C/D would seach A and B and C, but not D.
        /// </remarks>
        public const int DirectorySearchDepth = 3;

        static MixedRealityToolkitFiles()
        {
            string path = Application.dataPath;
            searchForFoldersTask = Task.Run(() => SearchForFoldersAsync(path));
        }

        private static void SearchForFoldersAsync(string rootPath)
        {
            Stack<IEnumerator<string>> dirIters = new Stack<IEnumerator<string>>(DirectorySearchDepth);

            dirIters.Push(Directory.EnumerateDirectories(rootPath).GetEnumerator());

            while (dirIters.Count > 0)
            {
                IEnumerator<string> iter = dirIters.Peek();
                if (iter.MoveNext())
                {
                    TryRegisterModuleFolder(iter.Current);

                    if (dirIters.Count < DirectorySearchDepth)
                    {
                        dirIters.Push(Directory.EnumerateDirectories(iter.Current).GetEnumerator());
                    }
                }
                else
                {
                    dirIters.Pop();
                }
            }
        }

        private static bool TryRegisterModuleFolder(string folder)
        {
            return TryRegisterModuleFolder(folder, out MixedRealityToolkitModuleType module);
        }

        private static bool TryRegisterModuleFolder(string folder, out MixedRealityToolkitModuleType module)
        {
            string normalizedFolder = NormalizeSeparators(folder);
            if (FindMatchingModule(normalizedFolder, out module))
            {
                if (!mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
                {
                    modFolders = new HashSet<string>();
                    mrtkFolders.Add(module, modFolders);
                }
                modFolders.Add(normalizedFolder);
                return true;
            }

            return false;
        }

        private static bool TryUnregisterModuleFolder(string folder)
        {
            string normalizedFolder = NormalizeSeparators(folder);
            bool found = false;
            foreach (var modFolders in mrtkFolders)
            {
                if (modFolders.Value.Remove(normalizedFolder))
                {
                    if (modFolders.Value.Count == 0)
                    {
                        mrtkFolders.Remove(modFolders.Key);
                    }
                    found = true;
                }
            }

            return found;
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
            string absolutePath = MapRelativeFilePathToAbsolutePath(module, mrtkPathToFile);
            return absolutePath != null ? GetAssetDatabasePath(absolutePath) : null;
        }

        /// <summary>
        /// Maps a single relative path file to MRTK folders to its absolute path, if found. Otherwise returns null.
        /// </summary>
        /// <remarks>
        /// For example, this will map "Inspectors\Data\EditorWindowOptions.json" to its full path like
        /// "c:\project\Assets\Libs\MRTK\MixedRealityToolkit\Inspectors\Data\EditorWindowOptions.json".
        /// This assumes that the passed in mrtkPathToFile is found under the "MixedRealityToolkit" folder
        /// (instead of the MixedRealityToolkit.SDK, or any of the other folders).
        /// </remarks>
        public static string MapRelativeFilePathToAbsolutePath(string mrtkPathToFile)
        {
            return MapRelativeFilePathToAbsolutePath(MixedRealityToolkitModuleType.Core, mrtkPathToFile);
        }

        /// <summary>
        /// Overload of MapRelativeFilePathToAbsolutePath which provides the ability to specify the module that the
        /// file belongs to.
        /// </summary>
        /// <remarks>
        /// When searching for a resource that lives in the MixedRealityToolkit.SDK folder, this could be invoked
        /// in this way:
        /// MapRelativeFilePathToAbsolutePath(MixedRealityToolkitModuleType.SDK, mrtkPathToFile)
        /// </remarks>
        public static string MapRelativeFilePathToAbsolutePath(MixedRealityToolkitModuleType module, string mrtkPathToFile)
        {
            return MapRelativePathToAbsolutePath(SearchType.File, module, mrtkPathToFile);
        }

        /// <summary>
        /// Similar to MapRelativeFilePathToAbsolutePath, except this checks for the existence of a folder instead of file.
        /// </summary>
        public static string MapRelativeFolderPathToAbsolutePath(MixedRealityToolkitModuleType module, string mrtkPathToFolder)
        {
            return MapRelativePathToAbsolutePath(SearchType.Folder, module, mrtkPathToFolder);
        }

        public static string MapModulePath(MixedRealityToolkitModuleType module)
        {
            return GetAssetDatabasePath(MapRelativeFolderPathToAbsolutePath(module, ""));
        }

        /// <summary>
        /// Maps a single relative path (file or folder) in MRTK folders to its absolute path, if found.
        /// Otherwise returns null.
        /// </summary>
        private static string MapRelativePathToAbsolutePath(SearchType searchType, MixedRealityToolkitModuleType module, string mrtkPath)
        {
            if (!AreFoldersAvailable)
            {
                Debug.LogError("Failed to locate MixedRealityToolkit folders in the project.");
                return null;
            }

            if (mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
            {
                string path = modFolders
                    .Select(t => Path.Combine(t, mrtkPath))
                    .FirstOrDefault(t => searchType == SearchType.File ? File.Exists(t) : Directory.Exists(t));
                return path;
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
            { "Tests", MixedRealityToolkitModuleType.Tests },
            { "Extensions", MixedRealityToolkitModuleType.Extensions },
            { "Tools", MixedRealityToolkitModuleType.Tools },

            // This module only exists for testing purposes, and is used in edit mode tests in conjunction
            // with MixedRealityToolkitFiles to ensure that this class is able to reason over MRTK
            // files that are placed outside of the root asset folder.
            { "AdhocTesting", MixedRealityToolkitModuleType.AdhocTesting },
        };

        public static bool FindMatchingModule(string path, out MixedRealityToolkitModuleType result)
        {
            // Matches an optional module suffix, e.g. ".Services"
            const string modulePattern = @"(\.(?<module>[a-zA-Z]+))?";
            // Matches a version string, e.g. "2.0.0-20190611.2"
            const string versionPattern = @"(?<version>[.\-0-9]+)";
            // Matches the naming pattern in the MRTK repository
            // e.g. "MixedRealityToolkit.Services"
            const string mrtkPattern = @"^MixedRealityToolkit" + modulePattern + @"$";
            // Matches "Microsoft.MixedReality.Toolkit", followed by optional module name, followed by version number
            // e.g.: "Microsoft.MixedReality.Toolkit.Services.2.0.0-20190611.2"
            // This alternate path is used if above isn't found. This is to work around long paths issue with NuGetForUnity
            // https://github.com/GlitchEnzo/NuGetForUnity/issues/246
            const string nugetParentPattern = @"^Microsoft\.MixedReality\.Toolkit" + modulePattern + @"\." + versionPattern + @"$";

            if (path.Length > 0)
            {
                var dirInfo = new DirectoryInfo(path);
                if (TryMatchFolderPattern(dirInfo.Name, mrtkPattern, out result))
                {
                    return true;
                }
                else if (dirInfo.Name == "MRTK"
                    && dirInfo.Parent != null
                    && TryMatchFolderPattern(dirInfo.Parent.Name, nugetParentPattern, out result))
                {
                    return true;
                }
            }

            result = MixedRealityToolkitModuleType.Core;
            return false;
        }

        private static bool TryMatchFolderPattern(string name, string pattern, out MixedRealityToolkitModuleType result)
        {
            var folderMatches = System.Text.RegularExpressions.Regex.Matches(name, pattern);
            if (folderMatches.Count == 1)
            {
                var moduleName = folderMatches[0].Groups["module"].Value;
                if (moduleNameMap.TryGetValue(moduleName, out result))
                {
                    return true;
                }
            }

            result = MixedRealityToolkitModuleType.Core;
            return false;
        }

        /// <summary>
        /// This function is only exposed for testing purposes, and can change/be removed at any time.
        /// </summary>
        /// <remarks>
        /// Syncronously refreshes the MRTK folder database.
        /// </remarks>
        public static void RefreshFolders()
        {
            SearchForFoldersAsync(Application.dataPath);
        }
    }
}
