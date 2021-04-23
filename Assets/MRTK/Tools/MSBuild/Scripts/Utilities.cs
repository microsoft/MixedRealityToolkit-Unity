// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// Represents where a Unity project reference asset is located.
    /// </summary>
    public enum AssetLocation
    {
        /// <summary>
        /// Inside the Assets folder of the Unity project.
        /// </summary>
        Project,

        /// <summary>
        /// Inside the Packages folder of the Unity project.
        /// </summary>
        Package,

        /// <summary>
        /// Inside the Packages folder shipped with the Unity version (without source).
        /// </summary>
        BuiltInPackage,

        /// <summary>
        /// Inside the Packages folder shipped with the Unity version (with source).
        /// </summary>
        BuiltInPackageWithSource
    }

    /// <summary>
    /// Helper Utilities methods used by other classes.
    /// </summary>
    public static class Utilities
    {
        private const string AssetsFolderName = "Assets";
        private const string PackagesFolderName = "Packages";
        private const string MSBuildFolderName = "MSBuild";
        public const string PackagesCopyFolderName = "PackagesCopy";

        private const string BuiltInPackagesRelativePath = @"Data\Resources\PackageManager\BuiltInPackages";

        public static string ProjectPath { get; } = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsFolderName.Length);
        public static string MSBuildOutputFolder { get; } = GetNormalizedPath(ProjectPath + MSBuildFolderName, true);
        public static string PackagesCopyPath { get; } = Path.Combine(MSBuildOutputFolder, PackagesCopyFolderName);
        public const string MetaFileGuidRegex = @"guid:\s*([0-9a-fA-F]{32})";
        public const string MetaFileIdRegex = @"fileID:\s*(-?[0-9]+)";

        private static readonly string packagesPath;

        public static string AssetPath { get; }

        public static string BuiltInPackagesPath { get; }

        static Utilities()
        {
            AssetPath = Path.GetFullPath(Application.dataPath);
            packagesPath = Path.GetFullPath(ProjectPath + PackagesFolderName);
            BuiltInPackagesPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(EditorApplication.applicationPath), BuiltInPackagesRelativePath));
        }

        /// <summary>
        /// Converts an assets relative path to an absolute path.
        /// </summary>
        public static string GetFullPathFromAssetsRelative(string assetsRelativePath)
        {
            if (assetsRelativePath.StartsWith(AssetsFolderName))
            {
                return Path.GetFullPath(AssetPath + assetsRelativePath.Substring(AssetsFolderName.Length));
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
                return Path.GetFullPath(Path.Combine(MSBuildOutputFolder, PackagesCopyFolderName + path.Substring(PackagesFolderName.Length)));
            }

            throw new InvalidOperationException("Not a path known to be relative to project's Package folder.");
        }

        /// <summary>
        /// Parses a .meta file to extract a guid for the asset.
        /// </summary>
        /// <param name="assetPath">The path to the asset (not the .meta file).</param>
        /// <param name="guid">The guid extracted.</param>
        /// <returns>True if the operation was successful.</returns>
        public static bool TryGetGuidForAsset(FileInfo assetPath, out Guid guid)
        {
            string metaFile = $"{assetPath.FullName}.meta";

            if (!File.Exists(metaFile))
            {
                guid = default;
                return false;
            }

            string guidString = null;
            using (StreamReader reader = new StreamReader(metaFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    Match match = Regex.Match(line, MetaFileGuidRegex);

                    if (match.Success)
                    {
                        guidString = match.Groups[1].Captures[0].Value;
                        break;
                    }
                }
            }

            if (guid != null && Guid.TryParse(guidString, out guid))
            {
                return true;
            }

            guid = default;
            return false;
        }

        /// <summary>
        /// Gets the known <see cref="AssetLocation"/> for the asset file.
        /// </summary>
        /// <param name="assetFile">The asset file.</param>
        /// <returns>The <see cref="AssetLocation"/> if valid; throws an exception otherwise.</returns>
        public static AssetLocation GetAssetLocation(FileInfo assetFile)
        {
            string absolutePath = Path.GetFullPath(assetFile.FullName);

            if (absolutePath.Contains(AssetPath))
            {
                return AssetLocation.Project;
            }
#if UNITY_2019_3_OR_NEWER
            else if (UnityProjectInfo.SpecialPluginNameMappingUnity2019.Keys.Any(s => absolutePath.ToLower().Contains(s.ToLower())))
            {
                return AssetLocation.BuiltInPackageWithSource;
            }
#endif
            else if (absolutePath.Contains(packagesPath) || absolutePath.Contains(PackagesCopyPath))
            {
                return AssetLocation.Package;
            }
            else if (absolutePath.Contains(BuiltInPackagesPath))
            {
                return AssetLocation.BuiltInPackage;
            }
            else
            {
                throw new InvalidDataException($"Unknown asset location for '{absolutePath}'");
            }
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

            if (absolutePath.Contains(AssetPath))
            {
                return absolutePath.Replace(AssetPath, AssetsFolderName);
            }

            throw new ArgumentException(nameof(absolutePath), $"Absolute path '{absolutePath}' is not a Unity Assets relative path ('{AssetPath}')");
        }

        /// <summary>
        /// Get a path relative to the Packages folder from the absolute path, uses PackagesOutput folder.
        /// </summary>
        public static string GetPackagesRelativePathFrom(string absolutePath)
        {
            absolutePath = Path.GetFullPath(absolutePath);

            if (absolutePath.Contains(packagesPath))
            {
                return absolutePath.Replace(packagesPath, PackagesCopyFolderName);
            }
            else if (absolutePath.Contains(PackagesCopyPath))
            {
                return absolutePath.Replace(PackagesCopyPath, PackagesCopyFolderName);
            }

            throw new ArgumentException(nameof(absolutePath), $"Absolute path '{absolutePath}' is not a Unity Project Packages relative path ('{packagesPath}')");
        }

        /// <summary>
        /// Gets a relative path between two absolute paths.
        /// </summary>
        public static string GetRelativePath(string thisAbsolute, string thatAbsolute)
        {
            thisAbsolute = Path.GetFullPath(thisAbsolute);
            thatAbsolute = Path.GetFullPath(thatAbsolute);

            if (!thisAbsolute.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                thisAbsolute += Path.DirectorySeparatorChar;
            }

            return GetNormalizedPath(new Uri(thisAbsolute).MakeRelativeUri(new Uri(thatAbsolute)).OriginalString);
        }

        /// <summary>
        /// Gets a relative path between two known relative paths (inside Assets or Packages)
        /// </summary>
        public static string GetRelativePathForKnownFolders(string thisKnownFolder, string thatKnownFolder)
        {
            return GetRelativePath(GetFullPathFromKnownRelative(thisKnownFolder), GetFullPathFromKnownRelative(thatKnownFolder));
        }

        /// <summary>
        /// Reads until some contents is encountered, or the end of the stream is reached.
        /// </summary>
        /// <param name="reader">The <see cref="System.IO.StreamReader"/> to use for reading.</param>
        /// <param name="contents">The contents to search for in the lines being read.</param>
        /// <returns>The line on which some of the contents was found.</returns>
        public static string ReadUntil(this StreamReader reader, params string[] contents)
        {
            return ReadWhile(reader, line => !contents.Any(c => line.Contains(c)));
        }

        /// <summary>
        /// A helper to check whether a DLL is a managed assembly.
        /// </summary>
        /// <param name="assemblyPath">The path to the assembly.</param>
        /// <remarks>Taken from https://stackoverflow.com/questions/367761/how-to-determine-whether-a-dll-is-a-managed-assembly-or-native-prevent-loading. </remarks>
        /// <returns>True if a managed assembly.</returns>
        public static bool IsManagedAssembly(string assemblyPath)
        {
            using (Stream fileStream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                if (fileStream.Length < 64)
                {
                    return false;
                }

                // PE Header starts @ 0x3C (60). Its a 4 byte header.
                fileStream.Position = 0x3C;
                uint peHeaderPointer = binaryReader.ReadUInt32();
                if (peHeaderPointer == 0)
                {
                    peHeaderPointer = 0x80;
                }

                // Ensure there is at least enough room for the following structures:
                //     24 byte PE Signature & Header
                //     28 byte Standard Fields         (24 bytes for PE32+)
                //     68 byte NT Fields               (88 bytes for PE32+)
                // >= 128 byte Data Dictionary Table
                if (peHeaderPointer > fileStream.Length - 256)
                {
                    return false;
                }

                // Check the PE signature.  Should equal 'PE\0\0'.
                fileStream.Position = peHeaderPointer;
                uint peHeaderSignature = binaryReader.ReadUInt32();
                if (peHeaderSignature != 0x00004550)
                {
                    return false;
                }

                // skip over the PEHeader fields
                fileStream.Position += 20;

                const ushort PE32 = 0x10b;
                const ushort PE32Plus = 0x20b;

                // Read PE magic number from Standard Fields to determine format.
                ushort peFormat = binaryReader.ReadUInt16();
                if (peFormat != PE32 && peFormat != PE32Plus)
                {
                    return false;
                }

                // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
                // When this is non-zero then the file contains CLI data otherwise not.
                ushort dataDictionaryStart = (ushort)(peHeaderPointer + (peFormat == PE32 ? 232 : 248));
                fileStream.Position = dataDictionaryStart;

                uint cliHeaderRva = binaryReader.ReadUInt32();
                if (cliHeaderRva == 0)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Reads while the predicate is satisfied, returns the line on which it failed.
        /// </summary>
        /// <param name="reader">The <see cref="System.IO.StreamReader"/> to use for reading.</param>
        /// <param name="predicate">The predicate that should return false when reading should stop.</param>
        /// <returns>The line on which the predicate returned false.</returns>
        public static string ReadWhile(this StreamReader reader, System.Func<string, bool> predicate)
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!predicate(line))
                {
                    return line;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Copies the source directory to the output directory creating all the directories first then copying.
        /// </summary>
        public static void CopyDirectory(string sourcePath, string destinationPath)
        {
            // Create the root directory itself
            Directory.CreateDirectory(destinationPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            }

            // Copy all the files & Replaces any files with the same name
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
        public static bool TryIOWithRetries(Action operation, int numRetries, TimeSpan sleepBetweenRetries, bool throwOnLastRetry = false)
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

                Thread.Sleep(sleepBetweenRetries);
                numRetries--;
            } while (numRetries >= 0);

            return false;
        }

        /// <summary>
        /// Delete directory helper that also waits for delete to completely propagate through the system.
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

            if (waitForDirectoryDelete && Application.isEditor)
            {
                // Just in case make sure this is forced to be Editor only
                // Sometimes the delete isn't committed fast enough, lets spin and wait for this to happen
                for (int i = 0; i < 10 && Directory.Exists(targetDir); i++)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Helper to replace tokens in text using StringBuilder.
        /// </summary>
        public static string ReplaceTokens(string text, Dictionary<string, string> tokens, bool verifyAllTokensPresent = false)
        {
            if (verifyAllTokensPresent)
            {
                string[] missingTokens = tokens.Keys.Where(t => !text.Contains(t)).ToArray();
                if (missingTokens.Length > 0)
                {
                    throw new InvalidOperationException($"Token replacement failed, found tokens missing from the template: '{string.Join("; ", missingTokens)}'.");
                }
            }

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
        public static bool TryGetTextTemplate(string text, string templateName, out string fullTemplate)
        {
            return TryGetTextTemplate(text, templateName, out fullTemplate, out string _);
        }

        /// <summary>
        /// Helper to fetch a normal text file based template ('prefixed with #')
        /// </summary>
        public static bool TryGetTextTemplate(string text, string templateName, out string fullTemplate, out string templateBody)
        {
            string regex = $"^\\s*#{templateName}_TEMPLATE (.*)$";
            Match result = Regex.Match(text, regex, RegexOptions.Multiline);

            if (result.Success)
            {
                fullTemplate = result.Groups[0].Captures[0].Value.TrimEnd();
                templateBody = result.Groups[1].Captures[0].Value.TrimEnd();
                return true;
            }

            fullTemplate = null;
            templateBody = null;
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
        /// Gets a <see href="https://docs.unity3d.com/ScriptReference/BuildTargetGroup.html">BuildTargetGroup</see> for a specified <see href="https://docs.unity3d.com/ScriptReference/BuildTarget.html">BuildTarget</see>.
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
#if UNITY_EDITOR
            Type uwpReferences = Type.GetType("UnityEditor.Scripting.Compilers.UWPReferences, UnityEditor.dll");
            MethodInfo getInstalledSDKS = uwpReferences.GetMethod("GetInstalledSDKs", BindingFlags.Static | BindingFlags.Public);
            MethodInfo sdkVersionToString = uwpReferences.GetMethod("SdkVersionToString", BindingFlags.Static | BindingFlags.NonPublic);

            IEnumerable<object> uwpSDKS = (IEnumerable<object>)getInstalledSDKS.Invoke(null, Array.Empty<object>());

            return uwpSDKS.Select(t => (Version)t.GetType().GetField("Version", BindingFlags.Instance | BindingFlags.Public).GetValue(t));
#else
            throw new PlatformNotSupportedException($"{nameof(GetUWPSDKs)} is only supported in the editor.");
#endif
        }
    }
}
