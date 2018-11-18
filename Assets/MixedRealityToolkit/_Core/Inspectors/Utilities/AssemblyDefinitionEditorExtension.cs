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
        private const string DLL = ".dll";
        private const string META = ".meta";
        private const string JSON = ".json";
        private const string ASMDEF = ".asmdef";

        #region Serialzied Data Objects

        [Serializable]
        internal class AsmDefSourceFiles
        {
            public string[] Files = null;
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

            public AsmDefSourceFiles Source { get; set; } = new AsmDefSourceFiles();

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

        #endregion Serialzied Data Objects

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

            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var directoryPath = new FileInfo(assetPath).Directory?.FullName;
            var assemblyDefinitionText = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath).text;
            var scriptAssemblyData = CustomScriptAssemblyData.FromJson(assemblyDefinitionText);
            var fromAssemblyName = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(scriptAssemblyData.name);

            Debug.Assert(!string.IsNullOrEmpty(scriptAssemblyData.name));
            Debug.Assert(fromAssemblyName == assetPath, "Failed to get the proper assembly name!");

            if (CompilationPipeline.GetAssemblies(AssembliesType.Editor).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath) ||
                CompilationPipeline.GetAssemblies(AssembliesType.Player).ReplaceSourceWithAssembly(ref scriptAssemblyData, directoryPath))
            {
                File.WriteAllText(assetPath, CustomScriptAssemblyData.ToJson(scriptAssemblyData));
                File.WriteAllText($"{Path.GetFullPath(assetPath).Hide()}{JSON}", JsonUtility.ToJson(scriptAssemblyData.Source, true));
                AssetDatabase.ImportAsset(assetPath);
            }
            else
            {
                Debug.LogError("Failed to replace source code with assembly!");
            }
        }

        private static bool ReplaceSourceWithAssembly(this Assembly[] assemblies, ref CustomScriptAssemblyData assemblyData, string directoryPath)
        {
            foreach (Assembly assembly in assemblies)
            {
                if (assembly.name != assemblyData.name) { continue; }

                Debug.Assert(assembly.sourceFiles != null);
                Debug.Assert(assembly.sourceFiles.Length > 0);

                assemblyData.Source.Files = assembly.sourceFiles;

                foreach (string sourceFile in assembly.sourceFiles)
                {
                    var fullPath = Path.GetFullPath(sourceFile);
                    var newPath = fullPath.Hide();

                    File.Move(fullPath, newPath);
                    File.Move($"{fullPath}{META}", $"{newPath}{META}");
                }

                var assemblyPath = $"{directoryPath}\\{assembly.name}{DLL}";

                File.Copy(assembly.outputPath, assemblyPath);

                try
                {
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }

                var importedAssembly = (PluginImporter)AssetImporter.GetAtPath(assemblyPath.GetUnityProjectRelativePath());

                if (importedAssembly == null)
                {
                    Debug.LogError("Failed to get plugin importer!");
                    return true;
                }

                importedAssembly.SetCompatibleWithAnyPlatform(assemblyData.includePlatforms.Length == 0);

                for (int i = 0; i < assemblyData.includePlatforms?.Length; i++)
                {
                    importedAssembly.SetCompatibleWithAnyPlatform(assemblyData.includePlatforms[i].Contains("Editor"));
                    importedAssembly.SetCompatibleWithPlatform(assemblyData.includePlatforms[i], true);
                }
                
                for (int i = 0; i < assemblyData.excludePlatforms?.Length; i++)
                {
                    importedAssembly.SetExcludeEditorFromAnyPlatform(assemblyData.excludePlatforms[i].Contains("Editor"));
                    importedAssembly.SetExcludeFromAnyPlatform(assemblyData.excludePlatforms[i], true);
                }

                importedAssembly.SaveAndReimport();
                Selection.activeObject = importedAssembly;

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
                assemblyDefinitionPath = assetPath.FindSiblingFileByExtension(ASMDEF);
            }
            else if (assetPath.Contains(ASMDEF))
            {
                assemblyDefinitionPath = assetPath;
                builtAssemblyPath = assetPath.FindSiblingFileByExtension(DLL);
            }

            Debug.Assert(!string.IsNullOrEmpty(builtAssemblyPath), "No Assembly found for this Assembly Definition!");
            Debug.Assert(!string.IsNullOrEmpty(assemblyDefinitionPath), "No Assembly Definition found for this Assembly!");
            var assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);
            Debug.Assert(assemblyDefinitionAsset != null, $"Failed to load assembly def asset at {assemblyDefinitionPath}");
            var assemblyDefinitionText = assemblyDefinitionAsset.text;
            var scriptAssemblyData = CustomScriptAssemblyData.FromJson(assemblyDefinitionText);
            var assemblySourcePath = $"{Path.GetFullPath(assemblyDefinitionPath).Hide()}{JSON}";
            Debug.Assert(File.Exists(assemblySourcePath), "Fatal Error: Missing meta data to re-import source files. You'll need to manually do it by removing the '.' in front of each file.");
            string sourceFilesText = File.ReadAllText(assemblySourcePath);
            scriptAssemblyData.Source = JsonUtility.FromJson<AsmDefSourceFiles>(sourceFilesText);

            Debug.Assert(scriptAssemblyData != null);
            Debug.Assert(scriptAssemblyData.Source?.Files != null, "Fatal Error: Missing meta data to re-import source files. You'll need to manually do it by removing the '.' in front of each file.");

            foreach (var sourceFile in scriptAssemblyData.Source.Files)
            {
                var fullHiddenPath = Path.GetFullPath(sourceFile).Hide();

                if (!File.Exists(fullHiddenPath))
                {
                    Debug.LogError($"Failed to find source file for {scriptAssemblyData.name}.{Path.GetFileNameWithoutExtension(sourceFile)}");
                    continue;
                }

                var sourcePath = fullHiddenPath.UnHide();

                File.Move(fullHiddenPath, sourcePath);
                File.Move($"{fullHiddenPath}{META}", $"{sourcePath}{META}");
            }

            File.Delete(builtAssemblyPath);
            File.Delete($"{builtAssemblyPath}{META}");

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        private static string Hide(this string path)
        {
            var index = path.LastIndexOf("\\", StringComparison.Ordinal);

            if (index == 0)
            {
                index = path.LastIndexOf("/", StringComparison.Ordinal);
            }

            if (index == 0)
            {
                return $".{path}";
            }

            return path.Insert(index + 1, ".");
        }

        private static string UnHide(this string path)
        {
            string fileName = Path.GetFileName(path);

            if (string.IsNullOrEmpty(fileName)) { return path; }
            if (!fileName.Contains(".")) { return path; }

            if (fileName.IndexOf(".", StringComparison.Ordinal) == 0)
            {
                fileName = fileName.TrimStart('.');
            }

            return path.Replace($"\\.{fileName}", $"\\{fileName}");
        }

        private static string GetUnityProjectRelativePath(this string path)
        {
            return path.Replace(Path.GetFullPath(Application.dataPath), "Assets").Replace("\\", "/");
        }

        private static string[] GetAssetPathSiblings(this string assetPath)
        {
            Debug.Assert(!string.IsNullOrEmpty(assetPath), $"Invalid asset path {assetPath}");
            string directoryPath = new FileInfo(assetPath).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath), $"Invalid root path {directoryPath}");
            return Directory.GetFiles(directoryPath);
        }

        private static string FindSiblingFileByExtension(this string path, string extension)
        {
            var directoryPath = new FileInfo(path).Directory?.FullName;
            Debug.Assert(!string.IsNullOrEmpty(directoryPath));
            var files = Directory.GetFiles(directoryPath);

            for (var i = 0; i < files.Length; i++)
            {
                if (Path.GetExtension(files[i]) == extension)
                {
                    return files[i].GetUnityProjectRelativePath();
                }
            }

            return string.Empty;
        }

    }
}
