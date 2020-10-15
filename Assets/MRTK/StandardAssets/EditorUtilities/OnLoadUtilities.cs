// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public static class OnLoadUtilities
    {
        private const string SessionStateKey = "StandardAssetsOnLoadUtilitiesSessionStateKey";
        
        private const string ShaderSentinelFile = "MRTK.Shaders.Sentinel";
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
        /// Checks to see if the Assets or Pacakges (if embedded) folder trees contains the MRTK shaders.
        /// </summary>
        /// <returns>True if the shader sentinel file is found, otherwise false.</returns>
        private static bool AssetsContainsShaders()
        {
            List<string> searchFolders = new List<string>
            {
                Application.dataPath,
                Path.GetFullPath("Packages")
            };

            foreach (string folder in searchFolders)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                if (di.GetFiles(ShaderSentinelFile, SearchOption.AllDirectories).Length > 0)
                {
                    return true;
                }
            }

            return false;
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
            if (!di.Exists) { return null; }

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
