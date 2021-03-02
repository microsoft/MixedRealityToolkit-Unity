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
        [MenuItem("Mixed Reality Toolkit/Utilities/Check for Shader Updates")]
        private static void EnsureShaders()
        {
            DirectoryInfo packageShaderFolder = FindShaderFolderInPackage();

            if (!AssetsContainsShaders(packageShaderFolder))
            {
                ImportShaderFiles(packageShaderFolder);
            }
        }

        /// <summary>
        /// Checks to see if the Assets or Packages (if embedded) folder trees contains the MRTK shaders.
        /// </summary>
        /// <returns>True if the shader sentinel file is found, otherwise false.</returns>
        private static bool AssetsContainsShaders(DirectoryInfo packageShaderFolder)
        {
            string sentinelPath = AssetDatabase.GUIDToAssetPath(ShaderSentinelGuid);

            // If we do not find the sentinel, we need to import the shaders.
            if (string.IsNullOrWhiteSpace(sentinelPath))
            { 
                return false; 
            }

            // Getting here indicates that the project's Assets folder contains the shader sentinel.

            // If the package shader folder does not exist, there is nothing for us to do.
            if ((packageShaderFolder == null) || !packageShaderFolder.Exists)
            {
                return true;
            }

            // Get the versions of the sentinel files,
            int packageVer = ReadSentinelVersion(Path.Combine(packageShaderFolder.FullName, ShaderSentinelFile));
            int assetVer = ReadSentinelVersion(sentinelPath);

            // No need to copy if the versions are the same.
            if (packageVer == assetVer) 
            { 
                return true; 
            }

            string message = (packageVer < assetVer) ?
                "The MRTK shaders older than those in your project, do you wish to overwrite the existing shaders?" :
                "Updated MRTK shaders are available, do you wish to overwrite the existing shaders?";

            bool dialogResponse = EditorUtility.DisplayDialog(
                "Mixed Reality Toolkit Standard Assets",
                message + 
                "\n\nNOTE: Overwriting will lose any customizations and may require reconfiguring the render pipeline.",
                "Yes",
                "No");

            // Return the inverse of the dialog result. Result of true means we want to overwrite, this method returns false
            // to cause an overwrite.
            return (!dialogResponse);
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
        private static void ImportShaderFiles(DirectoryInfo packageShaderFolder)
        {
            if (packageShaderFolder == null)
            {
                Debug.LogError("Unable to locate the shader source folder in the package");
                return;
            }

            DirectoryInfo destination = new DirectoryInfo(Path.Combine(Application.dataPath, ShaderImportDestination));
            if (!destination.Exists)
            {
                destination.Create();
            }

            FileInfo[] sourceFiles = packageShaderFolder.GetFiles();
            foreach (FileInfo fi in sourceFiles)
            {
                fi.CopyTo(Path.Combine(destination.FullName, fi.Name), true);
            }
        }

        /// <summary>
        /// Reads the version number out of the shader 
        /// </summary>
        /// <param name="sentinelPath"></param>
        /// <returns></returns>
        private static int ReadSentinelVersion(string sentinelPath)
        {
            using (FileStream fs = new FileStream(sentinelPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string token = "ver:";

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line.StartsWith(token))
                        {
                            line = line.Substring(token.Length).Trim();
                            if (!int.TryParse(line, out int ver))
                            {
                                break;
                            }
                            return ver;
                        }
                    }
                }
            }

            return -1;
        }
    }
}
