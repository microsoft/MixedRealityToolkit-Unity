using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace Assets.MRTK.Tools.Scripts
{
    public class Utilities
    {
        public const string PackagesCopy = nameof(PackagesCopy);

        public static string UnityFolderRelativeToAbsolutePath(string path)
        {
            if (path.StartsWith("Assets"))
            {
                return path.Replace("Assets", Application.dataPath);
            }
            else if (path.StartsWith("Packages"))
            {
                return Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + path.Replace("Packages", PackagesCopy);
            }

            return path;
        }

        public static string NormalizePath(string path)
        {
            return path.Replace('/', '\\');
        }

        public static string MakePathRelativeTo(string @thisAbsolute, string thatAbsolute)
        {
            return new Uri(thisAbsolute).MakeRelativeUri(new Uri(thatAbsolute)).OriginalString;
        }

        public static string MakeAssetRelativePathRelativeTo(string @thisAbsolute, string thatAbsolute)
        {
            return MakePathRelativeTo(UnityFolderRelativeToAbsolutePath(thisAbsolute), UnityFolderRelativeToAbsolutePath(thatAbsolute));
        }

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

            Directory.Delete(targetDir, false);

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
    }
}
