// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public static class OnLoadUtilities
    {
        private const string ShaderSentinelFile = "MRTK.Shaders.Sentinel";
        private const string ShaderImportDestination = "MRTK/Shaders";

        static OnLoadUtilities()
        {
            ImportShaderFiles();
            EnsureShaders();
        }

        /// <summary>
        /// Ensures that MRTK shader files are present in the Assets folder tree as they
        /// need to be modifiable to support the Universal Render Pipeline
        /// </summary>
        private static void EnsureShaders()
        {
            if (!AssetsContainsShaders())
            {
                ImportShaderFiles();
            }
        }

        /// <summary>
        /// Checks to see if the Assets folder tree contains the MRTK shaders.
        /// </summary>
        /// <returns>True if the shader sentinel file is found, otherwise false.</returns>
        private static bool AssetsContainsShaders()
        {
            string assetRoot = Application.dataPath;
            DirectoryInfo di = new DirectoryInfo(assetRoot);
            return (di.GetFiles(ShaderSentinelFile, SearchOption.AllDirectories).Length > 0);
        }

        /// <summary>
        /// Finds the shader folder within the package cache.
        /// </summary>
        /// <returns>
        /// DirectoryInfo object representing the shader folder in the package cache.
        /// If not found, returns null.</returns>
        private static DirectoryInfo FindShaderFolderInPackage()
        {
            DirectoryInfo di = new DirectoryInfo(Path.GetFullPath(Path.Combine("Library", "PackageCache")));
            FileInfo[] files = di.GetFiles(ShaderSentinelFile, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                return new DirectoryInfo(files[0].DirectoryName);
            }

            return null;
        }

        /// <summary>
        /// Copies the shader files from the package cache to the Assets folder tree.
        /// </summary>
        private static void ImportShaderFiles()
        {
            DirectoryInfo source = FindShaderFolderInPackage();
            DirectoryInfo destination = new DirectoryInfo(Path.Combine(Application.dataPath, ShaderImportDestination));

            if (!destination.Exists)
            {
                destination.Create();
            }

            FileInfo[] sourceFiles = source.GetFiles();
            foreach (FileInfo fi in sourceFiles)
            {
                fi.CopyTo(Path.Combine(destination.FullName, fi.Name));
            }
        }
    }
}
