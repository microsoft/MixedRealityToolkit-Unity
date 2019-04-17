// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// API for working with MixedRealityToolkit folders contained in the project.
    /// </summary>
    [InitializeOnLoad]
    public static class MixedRealityToolkitFiles
    {
        /// <summary>
        /// In order to subscribe for a <see cref="OnPostprocessAllAssets(string[], string[], string[], string[])"/> callback, 
        /// the class declaring the method must derive from AssetPostprocessor. So this class is nested privately as to prevent instnatiation of it.
        /// </summary>
        private class AssetPostprocessor : UnityEditor.AssetPostprocessor
        {
            public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                searchForFoldersTask.Wait();

                foreach (string asset in importedAssets.Concat(movedAssets))
                {
                    string folder = asset.Replace("Assets", Application.dataPath);
                    if (folder.EndsWith(MixedRealityToolkitDirectory))
                    {
                        mrtkFolders.Add(NormalizeSeparators(folder));
                    }
                }

                foreach (string asset in deletedAssets.Concat(movedFromAssetPaths))
                {
                    string folder = asset.Replace("Assets", Application.dataPath);
                    if (folder.EndsWith(MixedRealityToolkitDirectory))
                    {
                        folder = NormalizeSeparators(folder);
                        if (mrtkFolders.Contains(folder) && !Directory.Exists(folder))
                        {
                            // The contains check in the if statement is faster than Directory.Exists so that's why it's used
                            // Otherwise, it isn't necessary, as the statement below doesn't throw if item wasn't found
                            mrtkFolders.Remove(folder);
                        }
                    }
                }
            }
        }

        private const string MixedRealityToolkitDirectory = "MixedRealityToolkit";

        private readonly static HashSet<string> mrtkFolders = new HashSet<string>();
        private readonly static Task searchForFoldersTask;

        /// <summary>
        /// Returns a collection of MRTK directories found in the project.
        /// </summary>
        public static IEnumerable<string> MRTKDirectories { get; } = mrtkFolders;

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
            IEnumerable<string> directories = Directory.GetDirectories(rootPath, MixedRealityToolkitDirectory, SearchOption.AllDirectories)
                .Select(NormalizeSeparators);

            foreach (string s in directories)
            {
                mrtkFolders.Add(s);
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
            if (!AreFoldersAvailable)
            {
                Debug.LogError("Failed to locate MixedRealityToolkit folders in the project.");
                return null;
            }

            return mrtkFolders
                .Select(t => Path.Combine(t, mrtkRelativeFolder))
                .Where(Directory.Exists)
                .SelectMany(t => Directory.GetFiles(t))
                .Select(GetAssetDatabasePath)
                .ToArray();
        }

        /// <summary>
        /// Maps a single relative path file to a concrete path from one of the MRTK folders, if found. Otherwise returns null.
        /// </summary>
        /// <param name="mrtkPathToFile">The MRTK folder relative path to the file.</param>
        /// <returns>The project relative path to the file.</returns>
        public static string MapRelativeFilePath(string mrtkPathToFile)
        {
            if (!AreFoldersAvailable)
            {
                Debug.LogError("Failed to locate MixedRealityToolkit folders in the project.");
                return null;
            }

            string path = mrtkFolders
                .Select(t => Path.Combine(t, mrtkPathToFile))
                .FirstOrDefault(t => File.Exists(t));

            return path != null ? GetAssetDatabasePath(path) : null;
        }
    }
}
