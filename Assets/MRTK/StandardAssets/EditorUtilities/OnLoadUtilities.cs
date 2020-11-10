// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    static class OnLoadUtilities
    {
        private const string SessionStateKey = "StandardAssetsOnLoadUtilitiesSessionStateKey";

        private const string ShaderSentinelGuid = "05852dd420bb9ec4cb7318bfa529d37c";
        private const string ShaderSentinelFile = "MRTK.Shaders.sentinel";

        private const string ShaderImportDestination = "MRTK/Shaders";

        static OnLoadUtilities()
        {
            // This InitializeOnLoad handler only runs once at editor launch in order to adjust for Unity version
            // differences. These don't need to (and should not be) run on an ongoing basis. This uses the
            // volatile SessionState which is clear when Unity launches to ensure that this only runs the
            // expensive work (file system i/o) once.
            if (!SessionState.GetBool(SessionStateKey, false))
            {
                SessionState.SetBool(SessionStateKey, true);
                EnsureShaders();
            }
        }

        /// <summary>
        /// Ensures that MRTK shader files are present in a writable location. To support the 
        /// Universal Render Pipeline, shader modifications must be persisted.
        /// </summary>
        private static void EnsureShaders()
        {
            if (!AssetsContainsShaders())
            {
                ImportShaderFiles();
            }
        }

        /// <summary>
        /// Checks to see if the Assets or Packages (if embedded) folder trees contains the MRTK shaders.
        /// </summary>
        /// <returns>True if the shader sentinel file is found, otherwise false.</returns>
        private static bool AssetsContainsShaders()
        {
            return !string.IsNullOrWhiteSpace(AssetDatabase.GUIDToAssetPath(ShaderSentinelGuid));
        }

        /// <summary>
        /// Finds the shader folder within an installed or embedded package.
        /// </summary>
        /// <returns>
        /// DirectoryInfo object representing the shader folder in the package cache.
        /// If not found, returns null.
        /// </returns>
        private static DirectoryInfo FindShaderFolderInPackage()
        {
            List<string> searchPaths = new List<string>
            {
                Path.GetFullPath(Path.Combine("Library", "PackageCache")),
                Path.GetFullPath("Packages")
            };

            foreach (string path in searchPaths)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (!di.Exists) { continue; }

                FileInfo[] files = di.GetFiles(ShaderSentinelFile, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    return new DirectoryInfo(files[0].DirectoryName);
                }
            }

            return null;
        }

        /// <summary>
        /// Copies the shader files from the package cache to the Assets folder tree.
        /// </summary>
        private static void ImportShaderFiles()
        {
            DirectoryInfo source = FindShaderFolderInPackage();
            if (source == null)
            {
                Debug.LogError("Unable to locate the shader source folder in the package");
                return;
            }

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
