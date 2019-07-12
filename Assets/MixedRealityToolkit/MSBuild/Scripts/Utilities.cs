// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.MSBuild
{
    /// <summary>
    /// Helper Utilities methods used by other classes.
    /// </summary>
    public static class Utilities
    {
        private const string AssetsFolderName = "Assets";
        private const string PackagesFolderName = "Packages";
        private const string MSBuildFolderName = "MSBuild";

        public static readonly string MSBuildOutputFolder = GetNormalizedPath(Application.dataPath.Replace("Assets", MSBuildFolderName), true);
        public const string PackagesCopyFolderName = "PackagesCopy";

        private static string AssetsPath;

        static Utilities()
        {
            AssetsPath = Path.GetFullPath(Application.dataPath);
        }

        /// <summary>
        /// Converts an assets relative path to an absolute path.
        /// </summary>
        public static string GetFullPathFromAssetsRelative(string assetsRelativePath)
        {
            if (assetsRelativePath.StartsWith(AssetsFolderName))
            {
                return Path.GetFullPath(assetsRelativePath.Replace(AssetsFolderName, AssetsPath));
            }

            throw new InvalidOperationException("Not a path known to be relative to the project's Asset folder.");
        }

        /// <summary>
        /// Converts a Packages relative path to an absolute path using PackagesCopy directory instead.
        /// </summary>
        public static string GetFullPathFromPackagesRelative(string path)
        {
            if (path.StartsWith(PackagesFolderName))
            {
                return Path.GetFullPath(MSBuildOutputFolder + path.Replace(PackagesFolderName, PackagesCopyFolderName));
            }

            throw new InvalidOperationException("Not a path known to be relative to project's Package folder.");
        }

        /// <summary>
        /// Gets a full path from one of the two known relative paths (Assets, Packages). Packages is converted to use PackagesCopy.
        /// </summary>
        public static string GetFullPathFromKnownRelative(string path)
        {
            if (path.StartsWith(AssetsFolderName))
            {
                return GetFullPathFromAssetsRelative(path);
            }
            else if (path.StartsWith(PackagesFolderName))
            {
                return GetFullPathFromPackagesRelative(path);
            }

            throw new InvalidOperationException("Not a known path relative to project's folders.");
        }

        /// <summary>
        /// Get a path relative to the assets folder from the absolute path.
        /// </summary>
        public static string GetAssetsRelativePathFrom(string absolutePath)
        {
            absolutePath = Path.GetFullPath(absolutePath);

            if (!absolutePath.Contains(AssetsPath))
            {
                throw new ArgumentException(nameof(absolutePath), $"Absolute path '{absolutePath}' is not a Unity Assets relative path ('{AssetsPath}')");
            }

            return absolutePath.Replace(AssetsPath, AssetsFolderName);
        }

        /// <summary>
        /// Gets a relative path between two absolute paths.
        /// </summary>
        public static string GetRelativePath(string thisAbsolute, string thatAbsolute)
        {
            thisAbsolute = Path.GetFullPath(thisAbsolute);
            thatAbsolute = Path.GetFullPath(thatAbsolute);

            return new Uri(thisAbsolute).MakeRelativeUri(new Uri(thatAbsolute)).OriginalString;
        }

        /// <summary>
        /// Gets a relative path between two knwon relative paths (inside Assets or Pacakges)
        /// </summary>
        public static string GetRelativePathForKnownFolders(string thisKnownFolder, string thatKnownFolder)
        {
            return GetRelativePath(GetFullPathFromKnownRelative(thisKnownFolder), GetFullPathFromKnownRelative(thatKnownFolder));
        }

        /// <summary>
        /// Copies the source directory to the output directory creating all the directories first then copying.
        /// </summary>
        public static void CopyDirectory(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
            }
        }

        /// <summary>
        /// Gets a normalized path (with back slashes only), and optionally can make full path.
        /// </summary>
        public static string GetNormalizedPath(string path, bool makeFullPath = false)
        {
            return makeFullPath
                ? Path.GetFullPath(path)
                : path.Replace('/', '\\');
        }

        /// <summary>
        /// Deletes a directory then waits for it to be flushed in the system as deleted before creating. Sometimes deleting and creating to quickly will result in an exception.
        /// </summary>
        public static void EnsureCleanDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                DeleteDirectory(path, true);
            }

            if (!TryIOWithRetries(() => Directory.CreateDirectory(path), 5, TimeSpan.FromMilliseconds(100)))
            {
                throw new InvalidOperationException($"Failed to create the directory at '{path}'.");
            }
        }

        /// <summary>
        /// Helper to perform an IO operation with retries.
        /// </summary>
        public static bool TryIOWithRetries(Action operation, int numRetries, TimeSpan sleepBetweenRetrie, bool throwOnLastRetry = false)
        {
            do
            {
                try
                {
                    operation();
                    return true;
                }
                catch (UnauthorizedAccessException)
                {
                    if (throwOnLastRetry && numRetries == 0)
                    {
                        throw;
                    }
                }
                catch (IOException)
                {
                    if (throwOnLastRetry && numRetries == 0)
                    {
                        throw;
                    }
                }

                Thread.Sleep(sleepBetweenRetrie);
                numRetries--;
            } while (numRetries >= 0);

            return false;
        }

        /// <summary>
        /// Delete directory helper that also waits for delete to completely propogate through the system.
        /// </summary>
        public static void DeleteDirectory(string targetDir, bool waitForDirectoryDelete = false)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal | FileAttributes.Hidden);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir, waitForDirectoryDelete);
            }

            TryIOWithRetries(() => Directory.Delete(targetDir, false), 2, TimeSpan.FromMilliseconds(100), true);

            if (waitForDirectoryDelete)
            {
#if UNITY_EDITOR // Just in case make sure this is forced to be Editor only
                // Sometimes the delete isn't committed fast enough, lets spin and wait for this to happen
                for (int i = 0; i < 10 && Directory.Exists(targetDir); i++)
                {
                    Thread.Sleep(100);
                }
#endif
            }
        }

        /// <summary>
        /// Helper to replace tokens in text using StringBuilder.
        /// </summary>
        public static string ReplaceTokens(string text, Dictionary<string, string> tokens)
        {
            StringBuilder builder = new StringBuilder(text);

            foreach (KeyValuePair<string, string> token in tokens)
            {
                if (!string.IsNullOrEmpty(token.Key))
                {
                    builder.Replace(token.Key, token.Value);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Helper to fetch an XML based template.
        /// </summary>
        public static bool TryGetXMLTemplate(string text, string templateName, out string template)
        {
            string regex = $"(<!--{templateName}_TEMPLATE_START-->.*<!--{templateName}_TEMPLATE_END-->)";
            Match result = Regex.Match(text, regex, RegexOptions.Singleline);

            if (result.Success && result.Groups[1].Success && result.Groups[1].Captures.Count > 0)
            {
                template = result.Groups[1].Captures[0].Value;
                return true;
            }

            template = null;
            return false;
        }

        /// <summary>
        /// Helper to fetch a normal text file based template ('prefixed with #')
        /// </summary>
        public static bool TryGetTextTemplate(string text, string templateName, out string template)
        {
            string regex = $"^(\\s*#{templateName}_TEMPLATE .*)$";
            Match result = Regex.Match(text, regex, RegexOptions.Multiline);

            if (result.Success && result.Groups[1].Success && result.Groups[1].Captures.Count > 0)
            {
                template = result.Groups[1].Captures[0].Value;
                return true;
            }

            template = null;
            return false;
        }

        /// <summary>
        /// Given a list of Asset guids converts them to asset paths in place.
        /// </summary>
        public static void GetPathsFromGuidsInPlace(string[] guids, bool fullPaths = false)
        {
            for (int i = 0; i < guids.Length; i++)
            {
                guids[i] = AssetDatabase.GUIDToAssetPath(guids[i]);

                if (fullPaths)
                {
                    guids[i] = GetFullPathFromAssetsRelative(guids[i]);
                }
            }
        }

        /// <summary>
        /// Helper to see if the specified BuildTarget is installed in the editor.
        /// </summary>
        public static bool IsPlatformInstalled(BuildTarget buildTarget)
        {
#if UNITY_EDITOR
            Type moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
            MethodInfo isPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo getTargetStringFromBuildTarget = moduleManager.GetMethod("GetTargetStringFromBuildTarget", BindingFlags.Static | BindingFlags.NonPublic);

            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { buildTarget }) });
#else
            throw new PlatformNotSupportedException($"{nameof(IsPlatformInstalled)} is only supported in the editor.");
#endif
        }

        /// <summary>
        /// Gets a <see cref="BuildTargetGroup"/> for a specified <see cref="BuildTarget"/>.
        /// </summary>
        public static BuildTargetGroup GetBuildTargetGroup(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.NoTarget:
                    return BuildTargetGroup.Unknown;
                default:
                    throw new PlatformNotSupportedException($"Don't currently support {buildTarget}");
            }
        }

        public static IEnumerable<Version> GetUWPSDKs()
        {
            // May also be of interest to GetReferences
#if UNITY_EDITOR
            Type uwpReferences = Type.GetType("UnityEditor.Scripting.Compilers.UWPReferences, UnityEditor.dll");
            MethodInfo getInstalledSDKS = uwpReferences.GetMethod("GetInstalledSDKs", BindingFlags.Static | BindingFlags.Public);
            MethodInfo sdkVersionToString = uwpReferences.GetMethod("SdkVersionToString", BindingFlags.Static | BindingFlags.NonPublic);

            IEnumerable<object> uwpSDKS = (IEnumerable<object>)getInstalledSDKS.Invoke(null, new object[0]);

            return uwpSDKS.Select(t => (Version)t.GetType().GetField("Version", BindingFlags.Instance | BindingFlags.Public).GetValue(t));
#else
            throw new PlatformNotSupportedException($"{nameof(GetUWPSDKs)} is only supported in the editor.");
#endif
        }
    }
}
#endif