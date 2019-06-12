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
                    foreach (MixedRealityToolkitModuleType module in Enum.GetValues(typeof(MixedRealityToolkitModuleType)))
                    {
                        if (folder.EndsWith(MixedRealityToolkitDirectory(module)))
                        {
                            if (!mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
                            {
                                modFolders = new HashSet<string>();
                                mrtkFolders.Add(module, modFolders);
                            }
                            modFolders.Add(NormalizeSeparators(folder));
                        }
                    }
                }

                foreach (string asset in deletedAssets.Concat(movedFromAssetPaths))
                {
                    string folder = asset.Replace("Assets", Application.dataPath);
                    foreach (MixedRealityToolkitModuleType module in Enum.GetValues(typeof(MixedRealityToolkitModuleType)))
                    {
                        if (mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
                        {
                            if (folder.EndsWith(MixedRealityToolkitDirectory(module)))
                            {
                                folder = NormalizeSeparators(folder);
                                if (modFolders.Contains(folder) && !Directory.Exists(folder))
                                {
                                    // The contains check in the if statement is faster than Directory.Exists so that's why it's used
                                    // Otherwise, it isn't necessary, as the statement below doesn't throw if item wasn't found
                                    modFolders.Remove(folder);
                                    if (modFolders.Count == 0)
                                    {
                                        mrtkFolders.Remove(module);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string MixedRealityToolkitDirectory(MixedRealityToolkitModuleType module, string basePath="MixedRealityToolkit")
        {
            switch (module)
            {
                case MixedRealityToolkitModuleType.Core: return basePath;
                case MixedRealityToolkitModuleType.Providers: return basePath + ".Providers";
                case MixedRealityToolkitModuleType.Services: return basePath + ".Services";
                case MixedRealityToolkitModuleType.SDK: return basePath + ".SDK";
                case MixedRealityToolkitModuleType.Examples: return basePath + ".Examples";
                case MixedRealityToolkitModuleType.Tests: return basePath + ".Tests";
            }
            Debug.Assert(false);
            return null;
        }

        // This alternate path is used if above isn't found. This is to work around long paths issue with NuGetForUnity
        // https://github.com/GlitchEnzo/NuGetForUnity/issues/246
        private static string AlternateMixedRealityToolkitDirectory(MixedRealityToolkitModuleType module)
        {
            return MixedRealityToolkitDirectory(module, "MRTK");
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
            foreach (MixedRealityToolkitModuleType module in Enum.GetValues(typeof(MixedRealityToolkitModuleType)))
            {
                IEnumerable<string> directories = Directory.GetDirectories(rootPath, MixedRealityToolkitDirectory(module), SearchOption.AllDirectories);

                if (directories.Count() == 0)
                {
                    directories = Directory.GetDirectories(rootPath, AlternateMixedRealityToolkitDirectory(module), SearchOption.AllDirectories);
                }

                directories = directories.Select(NormalizeSeparators);

                foreach (string s in directories)
                {
                    if (!mrtkFolders.TryGetValue(module, out HashSet<string> modFolders))
                    {
                        modFolders = new HashSet<string>();
                        mrtkFolders.Add(module, modFolders);
                    }
                    modFolders.Add(s);
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
    }
}
