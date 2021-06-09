// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

#if !UNITY_2019_1_OR_NEWER
using System.Collections.Generic;
using System.Linq;
#endif // !UNITY_2019_1_OR_NEWER

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public static class EditorProjectUtilities
    {
        private const string SessionStateKey = "EditorProjectUtilitiesSessionStateKey";

        /// <summary>
        /// Static constructor that allows for executing code on project load.
        /// </summary>
        static EditorProjectUtilities()
        {
            // This InitializeOnLoad handler only runs once at editor launch in order to adjust for Unity version
            // differences. These don't need to (and should not be) run on an ongoing basis. This uses the
            // volatile SessionState which is clear when Unity launches to ensure that this only runs the
            // expensive work (UpdateAsmDef) once.
            if (!SessionState.GetBool(SessionStateKey, false))
            {
                SessionState.SetBool(SessionStateKey, true);
                CheckMinimumEditorVersion();
                ApplyARFoundationUWPCompileFix();
                MixedRealityToolkitPreserveSettings.EnsureLinkXml();
            }
        }

        /// <summary>
        /// Checks that a supported version of Unity is being used with this project.
        /// </summary>
        /// <remarks>
        /// This method displays a message to the user allowing them to continue or to exit the editor.
        /// </remarks>
        public static void CheckMinimumEditorVersion()
        {
#if !UNITY_2018_4_OR_NEWER && !UNITY_2019_1_OR_NEWER
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
                "The Mixed Reality Toolkit requires Unity 2018.4 or newer.\n\nUsing an older version of Unity may result in compile errors or incorrect behavior.",
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
            return FindRelativeDirectory(Application.dataPath, packageDirectory, out path);
        }

        /// <summary>
        /// Finds the path of a directory relative to the project folder.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        internal static bool FindRelativeDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            string absolutePath;
            if (FindDirectory(directoryPathToSearch, directoryName, out absolutePath))
            {
                path = MixedRealityToolkitFiles.GetAssetDatabasePath(absolutePath);
                return true;
            }

            path = string.Empty;
            return false;
        }

        /// <summary>
        /// Finds the absolute path of a directory.
        /// </summary>
        /// <param name="directoryPathToSearch">
        /// The subtree's root path to search in.
        /// </param>
        /// <param name="directoryName">
        /// The name of the directory to search for.
        /// </param>
        internal static bool FindDirectory(string directoryPathToSearch, string directoryName, out string path)
        {
            path = string.Empty;

            var directories = Directory.GetDirectories(directoryPathToSearch);

            for (int i = 0; i < directories.Length; i++)
            {
                var name = Path.GetFileName(directories[i]);

                if (name != null && name.Equals(directoryName))
                {
                    path = directories[i];
                    return true;
                }

                if (FindDirectory(directories[i], directoryName, out path))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// On Unity 2018, the .NET backend has been deprecated. AR Foundation components utilize functionality that is not present in the .NET assemblies
        /// for the Universal Windows Platform. This method modifies the AR Foundation assembly definition files to exclude building on the Windows Universal
        /// build target.
        /// </summary>
        /// <remarks>
        /// This method only executes on Unity 2018.x.
        /// </remarks>
        private static void ApplyARFoundationUWPCompileFix()
        {
#if !UNITY_2019_1_OR_NEWER
            bool reloadLocked = EditorAssemblyReloadManager.LockReloadAssemblies;
            if (reloadLocked)
            {
                EditorAssemblyReloadManager.LockReloadAssemblies = false;
            }

            DirectoryInfo packageCache = FileUtilities.GetPackageCache();

            if (packageCache.Exists)
            {
                string uwpPlatformName = "WSA";

                FileInfo arFoundation = GetPackageCacheAssemblyDefinitionFile(
                    packageCache,
                    "com.unity.xr.arfoundation@*",
                    "Unity.XR.ARFoundation.asmdef");
                if (arFoundation != null)
                {
                    bool changed = false;

                    AssemblyDefinition asmDef = AssemblyDefinition.Load(arFoundation.FullName);

                    if (asmDef.IncludePlatforms.Contains(uwpPlatformName))
                    {
                        Debug.Log($"Removing Universal Windows Platform from the {arFoundation.FullName} included platforms list.");
                        List<string> list = new List<string>(asmDef.IncludePlatforms);
                        list.Remove(uwpPlatformName);
                        asmDef.IncludePlatforms = list.ToArray();
                        changed = true;
                    }
                    else if (!asmDef.ExcludePlatforms.Contains(uwpPlatformName))
                    {
                        Debug.Log($"Adding Universal Windows Platform to the {arFoundation.FullName} excluded platforms list.");
                        List<string> list = new List<string>(asmDef.ExcludePlatforms);
                        list.Add(uwpPlatformName);
                        asmDef.ExcludePlatforms = list.ToArray();
                        changed = true;
                    }

                    if (changed)
                    {
                        asmDef.Save(arFoundation.FullName);
                    }
                }

                FileInfo arSubsystems = GetPackageCacheAssemblyDefinitionFile(
                    packageCache,
                    "com.unity.xr.arsubsystems@*",
                    "Unity.XR.ARSubsystems.asmdef");
                if (arSubsystems != null)
                {
                    bool changed = false;

                    AssemblyDefinition asmDef = AssemblyDefinition.Load(arSubsystems.FullName);

                    if (asmDef.IncludePlatforms.Contains(uwpPlatformName))
                    {
                        Debug.Log($"Removing Universal Windows Platform from the {arSubsystems.FullName} included platforms list.");
                        List<string> list = new List<string>(asmDef.IncludePlatforms);
                        list.Remove(uwpPlatformName);
                        asmDef.IncludePlatforms = list.ToArray();
                        changed = true;
                    }
                    else if (!asmDef.ExcludePlatforms.Contains(uwpPlatformName))
                    {
                        Debug.Log($"Adding Universal Windows Platform to the {arSubsystems.FullName} excluded platforms list.");
                        List<string> list = new List<string>(asmDef.ExcludePlatforms);
                        list.Add(uwpPlatformName);
                        asmDef.ExcludePlatforms = list.ToArray();
                        changed = true;
                    }

                    if (changed)
                    {
                        asmDef.Save(arSubsystems.FullName);
                    }
                }
            }

            if (reloadLocked)
            {
                EditorAssemblyReloadManager.LockReloadAssemblies = true;
            }

#endif // !UNITY_2019_OR_NEWER
        }

        /// <summary>
        /// Gets the assembly definition file that best matches the folder name pattern and the file names.
        /// </summary>
        /// <param name="root"><see href="https://docs.microsoft.com/dotnet/api/system.io.directoryinfo"/>DirectoryInfo</see> that describes the package cache root folder.</param>
        /// <param name="folderName">The name of the folder in which to find the requested file. A wildcard ('*') can be specified to match a partial name.</param>
        /// <param name="fileName">The name of the assembly definition file.</param>
        /// <returns>
        /// A <see href="https://docs.microsoft.com/dotnet/api/system.io.fileinfo"/>FileInfo</see> object that describes the assembly definition file or null.
        /// </returns>
        private static FileInfo GetPackageCacheAssemblyDefinitionFile(
            DirectoryInfo root,
            string folderName,
            string fileName)
        {
            DirectoryInfo[] folders = root.GetDirectories(folderName);
            if (folders.Length == 0)
            {
                return null;
            }
            if (folders.Length > 1)
            {
                Debug.LogWarning($"Too many instances of the {folderName} pattern, using the first one found.");
            }

            folders = folders[0].GetDirectories("Runtime");
            if (folders.Length == 0)
            {
                return null;
            }

            FileInfo[] files = folders[0].GetFiles(fileName);
            if (files.Length == 0)
            {
                return null;
            }

            return files[0];
        }
    }
}
