// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// A set of utilities for working with files.
    /// </summary>
    public static class FileUtilities
    {
        /// <summary>
        /// Locates the files that match the specified name within the Assets folder (application data path) structure.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        public static FileInfo[] FindFilesInAssets(string fileName)
        {
            DirectoryInfo root = new DirectoryInfo(Application.dataPath);
            return FindFiles(fileName, root);
        }

        /// <summary>
        /// Locates the files that match the specified name within the Library folder structure.
        /// </summary>
        /// <param name="fileName">The name of the file to locate (ex: "TestFile.asmdef")</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        public static FileInfo[] FindFilesInLibrary(string fileName)
        {
            // todo
            return new FileInfo[0];
        }

        /// <summary>
        /// Finds files in the specified folder structure.
        /// </summary>
        /// <param name="fileName">The name of the file to find.</param>
        /// <param name="rootFolder">The folder in which to search for files.</param>
        /// <returns>Array of FileInfo objects representing the located file(s).</returns>
        private static FileInfo[] FindFiles(
            string fileName,
            DirectoryInfo rootFolder)
        {
            if (string.IsNullOrWhiteSpace(fileName)) { return new FileInfo[0]; }

            DirectoryInfo assets = new DirectoryInfo(Application.dataPath);
            return assets.GetFiles(fileName, SearchOption.AllDirectories);
        }
    }
}
