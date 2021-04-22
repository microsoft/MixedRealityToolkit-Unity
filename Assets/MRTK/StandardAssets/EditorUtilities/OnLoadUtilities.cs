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
        private const string ShaderSentinelGuid = "05852dd420bb9ec4cb7318bfa529d37c";
        private const string ShaderSentinelFile = "MRTK.Shaders.sentinel";

        private const string ShaderImportDestination = "MRTK/Shaders";

        private const string IgnoreFileName = "IgnoreUpdateCheck.sentinel";

        static OnLoadUtilities()
        {
            EnsureShaders(false);
        }

        /// <summary>
        /// Checks for updated shaders and bypasses the ignore update check.
        /// </summary>
        [MenuItem("Mixed Reality/Toolkit/Utilities/Check for Shader Updates")]
        private static void CheckForShaderUpdates()
        {
            EnsureShaders(true);
        }

        /// <summary>
        /// Ensures that MRTK shader files are present in a writable location. To support the 
        /// Universal Render Pipeline, shader modifications must be persisted.
        /// </summary>
        /// <param name="bypassIgnore">Causes the shader update code to disregard the ignore file.</param>
        private static void EnsureShaders(bool bypassIgnore)
        {
            DirectoryInfo packageShaderFolder = FindShaderFolderInPackage();

            if (bypassIgnore)
            {
                // The customer is manually checking for updates, delete the ignore file
                string sentinelPath = AssetDatabase.GUIDToAssetPath(ShaderSentinelGuid);
                if (!string.IsNullOrWhiteSpace(sentinelPath))
                {
                    FileInfo ignoreFile = new FileInfo(Path.Combine(new FileInfo(sentinelPath).Directory.FullName, IgnoreFileName));
                    if (ignoreFile.Exists)
                    {
                        ignoreFile.Delete();
                    }
                    ignoreFile.Refresh();
                }
            }

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

            // Check for the "ignore this check" file, if present we do NOT import
            FileInfo ignoreFile = new FileInfo(Path.Combine(new FileInfo(sentinelPath).Directory.FullName, IgnoreFileName));
            if (ignoreFile.Exists)
            {
                return true;
            }

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

            int dialogResponse = EditorUtility.DisplayDialogComplex(
                "Mixed Reality Toolkit Standard Assets",
                message +
                "\n\nNOTE: Overwriting will lose any customizations and may require reconfiguring the render pipeline.",
                "Yes",      // returns 0
                "Ignore",   // returns 1 - placed in the cancel slot to force the button order as Yes, No, Ignore
                "No");      // returns 2

            if (dialogResponse == 1)
            {
                // Write an "ignore this check" file to prevent future prompting
                if (!ignoreFile.Directory.Exists)
                {
                    ignoreFile.Directory.Create();
                }
                ignoreFile.Create();
            }
            ignoreFile.Refresh();

            // Return the inverse of the dialog result. Result of true means we want to overwrite, this method returns false
            // to cause an overwrite.
            return (dialogResponse != 0);
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
        /// <param name="sentinelPath">The path to the sentinel file.</param>
        /// <returns>The version number found in the file, or -1.</returns>
        private static int ReadSentinelVersion(string sentinelPath)
        {
            using (FileStream fs = new FileStream(sentinelPath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    const string token = "ver:";

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
