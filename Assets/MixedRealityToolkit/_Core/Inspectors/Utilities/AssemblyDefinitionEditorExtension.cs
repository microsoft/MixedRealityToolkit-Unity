// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Utilities
{
    public static class AssemblyDefinitionEditorExtension
    {
        private const string ASMDEF = ".asmdef";
        private const string DLL = ".dll";
        private const string META = ".meta";

        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Source with Assembly", true, 99)]
        public static bool ReplaceWithAssemblyValidation()
        {
            if (Selection.activeObject == null) { return false; }
            return !AssetDatabase.GetAssetPath(Selection.activeObject).GetAssetPathSiblings().Any(path => path.Contains(DLL));
        }

        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Source with Assembly", false, 99)]
        public static void ReplaceWithAssembly()
        {
            Debug.Assert(Selection.activeObject != null);

            var success = false;
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var directoryPath = new FileInfo(assetPath).Directory?.FullName;
            var assemblyDefinitionText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath).text;
            var scriptAssemblyData = CustomScriptAssemblyData.FromJson(assemblyDefinitionText);
            var fromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(scriptAssemblyData.name);

            Debug.Assert(!string.IsNullOrEmpty(scriptAssemblyData.name));
            Debug.Assert(fromAssemblyName == assetPath, "Failed to get the proper assembly name!");

            success |= CompilationPipeline.GetAssemblies(AssembliesType.Editor).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath);
            success |= CompilationPipeline.GetAssemblies(AssembliesType.Player).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath);

            if (success)
            {
                File.WriteAllText(assetPath, CustomScriptAssemblyData.ToJson(scriptAssemblyData));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            Debug.Assert(success, "Failed to replace source code with assembly!");
        }

        private static bool ReplaceSourceWithAssembly(this Assembly[] assemblies, ref CustomScriptAssemblyData assemblyData, string directoryPath)
        {
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.name == assemblyData.name) { continue; }

                Debug.Assert(assembly.sourceFiles != null);
                Debug.Assert(assembly.sourceFiles.Length > 0);
                assemblyData.sourceFiles = assembly.sourceFiles;

                foreach (string sourceFile in assembly.sourceFiles)
                {
                    string fullPath = Path.GetFullPath(sourceFile);
                    string newPath = fullPath.Insert(fullPath.LastIndexOf("\\", StringComparison.Ordinal) + 1, ".");
                    File.Move(fullPath, newPath);
                    File.Move($"{fullPath}{META}", $"{newPath}{META}");
                }

                File.Copy(assembly.outputPath, $"{directoryPath}\\{assembly.name}{DLL}");

                return true;
            }

            return false;
        }

        [MenuItem("CONTEXT/PluginImporter/Replace Assembly with Source", true, 99)]
        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Assembly with Source", true, 99)]
        public static bool ReplaceWithSourceValidation()
        {
            if (Selection.activeObject == null) { return false; }

            string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            return assetPath.GetAssetPathSiblings().Any(path => assetPath.Contains(DLL) && path.Contains(ASMDEF) || assetPath.Contains(ASMDEF) && path.Contains(DLL));
        }

        [MenuItem("CONTEXT/PluginImporter/Replace Assembly with Source", false, 99)]
        [MenuItem("CONTEXT/AssemblyDefinitionImporter/Replace Assembly with Source", false, 99)]
        public static void ReplaceWithSource()
        {
            Debug.Assert(Selection.activeObject != null);

            var builtAssemblyPath = string.Empty;
            var assemblyDefinitionPath = string.Empty;
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (assetPath.Contains(DLL))
            {
                builtAssemblyPath = assetPath;
                assemblyDefinitionPath = assetPath.FindFileByExtension(ASMDEF);
            }
            else if (assetPath.Contains(ASMDEF))
            {
                assemblyDefinitionPath = assetPath;
                builtAssemblyPath = assetPath.FindFileByExtension(DLL);
            }

            Debug.Assert(!string.IsNullOrEmpty(builtAssemblyPath), "No Assembly found for this Assembly Definition!");
            Debug.Assert(!string.IsNullOrEmpty(assemblyDefinitionPath), "No Assembly Definition found for this Assembly!");

            var assemblyDefinitionText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath).text;
            var scriptAssemblyData = CustomScriptAssemblyData.FromJson(assemblyDefinitionText);

            foreach (var sourceFile in scriptAssemblyData.sourceFiles)
            {
                string hiddenPath = sourceFile.Insert(sourceFile.LastIndexOf("\\", StringComparison.Ordinal) + 1, ".");

                if (!File.Exists(hiddenPath))
                {
                    Debug.LogError($"Failed to find source file for {scriptAssemblyData.name}.{Path.GetFileName(sourceFile)}");
                    continue;
                }

                string fullHiddenPath = Path.GetFullPath(hiddenPath);
                string fullSourcePath = Path.GetFullPath(sourceFile);

                File.Move(fullHiddenPath, fullSourcePath);
                File.Move($"{fullHiddenPath}{META}", $"{fullSourcePath}{META}");
            }

            File.Delete(builtAssemblyPath);
            File.Delete($"{builtAssemblyPath}{META}");
        }

        private static string FindFileByExtension(this string path, string extension)
        {
            var directoryPath = new FileInfo(path).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath));
            var files = Directory.GetFiles(directoryPath);

            for (var i = 0; i < files.Length; i++)
            {
                if (files[i].Contains(extension))
                {
                    return files[i].Replace(Path.GetFullPath(Application.dataPath), "Assets").Replace("\\", "/");
                }
            }

            return string.Empty;
        }

        private static string[] GetAssetPathSiblings(this string assetPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(assetPath), $"Invalid asset path {assetPath}");
            string directoryPath = new FileInfo(assetPath).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath), $"Invalid root path {directoryPath}");
            return Directory.GetFiles(directoryPath);
        }

        [Serializable]
        internal class CustomScriptAssemblyData
        {
            public string name = null;
            public string[] references = null;
            public string[] optionalUnityReferences = null;
            public string[] includePlatforms = null;
            public string[] excludePlatforms = null;
            public bool allowUnsafeCode = false;
            public string[] sourceFiles = null;

            public static CustomScriptAssemblyData FromJson(string json)
            {
                var scriptAssemblyData = JsonUtility.FromJson<CustomScriptAssemblyData>(json);
                if (scriptAssemblyData == null) { throw new Exception("Json file does not contain an assembly definition"); }
                if (string.IsNullOrEmpty(scriptAssemblyData.name)) { throw new Exception("Required property 'name' not set"); }
                if (scriptAssemblyData.excludePlatforms != null && scriptAssemblyData.excludePlatforms.Length > 0 &&
                   (scriptAssemblyData.includePlatforms != null && scriptAssemblyData.includePlatforms.Length > 0))
                {
                    throw new Exception("Both 'excludePlatforms' and 'includePlatforms' are set.");
                }

                return scriptAssemblyData;
            }

            public static string ToJson(CustomScriptAssemblyData data)
            {
                return JsonUtility.ToJson(data, true);
            }
        }
    }
}
