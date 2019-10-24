// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public class EditorProjectUtilities
    {
        /// <summary>
        /// Static constructor that allows for executing code on project load.
        /// </summary>
        static EditorProjectUtilities()
        {
            CheckMinimumEditorVersion();
            ApplyARFoundationUWPCompileFix();
        }

        /// <summary>
        /// Checks that a supported version of Unity is being used with this project.
        /// </summary>
        /// <remarks>
        /// This method displays a message to the user allowing them to continue or to exit the editor.
        /// </remarks>
        public static void CheckMinimumEditorVersion()
        {
#if !UNITY_2018_3_OR_NEWER
            DisplayIncorrectEditorVersionDialog();
#endif
        }

        /// <summary>
        /// Displays a message indicating that a project was loaded in an unsupported version of Unity and allows the user
        /// to continue or exit.
        /// </summary>
        private static void DisplayIncorrectEditorVersionDialog()
        {
            if (!EditorUtility.DisplayDialog(
                "Mixed Reality Toolkit",
                "The Mixed Reality Toolkit requires Unity 2018.3 or newer.\n\nUsing an older version of Unity may result in compile errors or incorrect behavior.",
                "Continue", "Close Editor"))
            {
                EditorApplication.Exit(0);
            }
        }

        /// <summary>
        /// Finds the path of a directory relative to the project directory.
        /// </summary>
        /// <param name="packageDirectory">The name of the directory to search for.</param>
        /// <param name="path">The output parameter in which the fully qualified path is returned.</param>
        /// <returns>True if the directory could be found, false otherwise.</returns>
        public static bool FindRelativeDirectory(string packageDirectory, out string path)
        {
            return MixedRealityEditorSettings.FindRelativeDirectory(UnityEngine.Application.dataPath, packageDirectory, out path);
        }

        /// <summary>
        /// On Unity 2018, the .NET backend has been deprecated. AR Foundation components utilize functionality that is not present in the .NET assemblies
        /// for the Universal Windows Platform. This method modifies the AR Foundation assembly definition files to exclude building on the Windows Universal
        /// build target.
        /// </summary>
        /// <remarks>
        /// This method only executes when the Unity version is 2018.x and IL2CPP is not the selected backend.
        /// </remarks>
        private static void ApplyARFoundationUWPCompileFix()
        {
#if UNITY_2018 && !ENABLE_IL2CPP //&& UNITY_WSA
            // WSA

            DirectoryInfo packageCache = GetPackageCache();

            if (packageCache.Exists)
            {
                // Get the AR Foundation assembly definition.            
                FileInfo arFoundation = GetPackageCacheAssemblyDefinitionFile(
                    packageCache,
                    "com.unity.xr.arfoundation@*",
                    "Unity.XR.ARFoundation.asmdef");
                if (arFoundation != null)
                {
                    Debug.Log($"Updating {arFoundation.FullName} to enable Universal Windows Platform .NET backend builds");
                    // todo
                }

                // Get the AR Foundation assembly definition.            
                FileInfo arSubsystems = GetPackageCacheAssemblyDefinitionFile(
                    packageCache,
                    "com.unity.xr.arsubsystems@*",
                    "Unity.XR.ARSubsystems.asmdef");
                if (arSubsystems != null)
                {
                    Debug.Log($"Updating {arSubsystems.FullName} to enable Universal Windows Platform .NET backend builds");
                    // todo
                }
            }
#endif
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        private static DirectoryInfo GetPackageCache()
        {
            string packageCacheFolderName = @"Library\PackageCache";

            DirectoryInfo projectRoot = new DirectoryInfo(UnityEngine.Application.dataPath).Parent;
            return new DirectoryInfo(Path.Combine(projectRoot.FullName, packageCacheFolderName));
        }

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="root"></param>
        /// <param name="folderName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static FileInfo GetPackageCacheAssemblyDefinitionFile(
            DirectoryInfo root,
            string folderName,
            string fileName)
        {
            DirectoryInfo[] folders = root.GetDirectories(folderName);
            if (folders.Length == 0) { return null; }
            if (folders.Length > 1) 
            {
                Debug.LogWarning("Too many instances of the requested folder name, using the first one found.");
            }

            folders = folders[0].GetDirectories("Runtime");
            if (folders.Length == 0) 
            {
                Debug.Log("Failed to locate the Runtime folder.");
                return null; 
            }

            FileInfo[] files = folders[0].GetFiles(fileName);
            if (files.Length == 0)
            {
                Debug.Log(@"Failed to locate {fileName}.");
                return null;
            }

            return files[0];
        }
    }
}
