// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A set of utilities for working with files.
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Locates the files that match the specified name within the Assets folder structure.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        public static FileInfo[] FindFilesInAssets(string fileName)
        {
            // FindAssets doesn't take a file extension
            string[] assetGuids = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(fileName));

            List<FileInfo> fileInfos = new List<FileInfo>();
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                // Since this is an asset search without extension, some filenames may contain parts of other filenames.
                // Therefore, double check that the path actually contains the filename with extension.
                if (assetPath.Contains(fileName))
                {
                    fileInfos.Add(new FileInfo(assetPath));
                }
            }

            return fileInfos.ToArray();
        }

        /// <summary>
        /// Locates the files that match the specified name within the package cache folder structure.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        public static FileInfo[] FindFilesInPackageCache(string fileName)
        {
            DirectoryInfo root = GetPackageCache();
            return FindFiles(fileName, root);
        }

        /// <summary>
        /// Gets the package cache folder of this project.
        /// </summary>
        /// <returns>
        /// A DirectoryInfo object that describes the package cache folder.
        /// </returns>
        public static DirectoryInfo GetPackageCache()
        {
            string packageCacheFolderName = Path.Combine("Library", "PackageCache");

            DirectoryInfo projectRoot = new DirectoryInfo(Application.dataPath).Parent;
            return new DirectoryInfo(Path.Combine(projectRoot.FullName, packageCacheFolderName));
        }

        /// <summary>
        /// Finds all files matching the specified name.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <param name="root">The folder in which to perform the search.</param>
        /// <returns>Array of FileInfo objects containing the search results.</returns>
        private static FileInfo[] FindFiles(string fileName, DirectoryInfo root)
        {
            return root.GetFiles(fileName, SearchOption.AllDirectories);
        }
    }
}
