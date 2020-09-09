// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf.Editor
{
    /// <summary>
    /// Editor script for TestGltfLoading scene
    /// </summary>
    [CustomEditor(typeof(TestGltfLoading))]
    public class TestGltfLoadingEditor : UnityEditor.Editor
    {
        private const string GLTFModelsFolderGUID = "55bea35f05e99164cacb9c7dabdd25ee";

        public override void OnInspectorGUI()
        {
            var testGltfLoading = target as TestGltfLoading;

            base.OnInspectorGUI();

            var path = testGltfLoading.AbsolutePath;
            bool needsCopy = !File.Exists(path) && Application.isEditor;
            if (needsCopy)
            {
                EditorGUILayout.HelpBox("glTF path was not discovered in the streaming assets folder. Please copy over files to test example scene", MessageType.Warning);
            }

            if (GUILayout.Button("Copy models to StreamingAssets"))
            {
                string modelsPath = AssetDatabase.GUIDToAssetPath(GLTFModelsFolderGUID);
                DirectoryCopy(modelsPath, Path.Combine(Application.streamingAssetsPath, "GltfModels"));
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                Debug.Log("Copied glTF model files to Streaming Assets folder");
            }
        }

        private void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            var sourceDirectory = new DirectoryInfo(sourceDirName);

            if (!sourceDirectory.Exists)
            {
                Debug.LogError($"Source directory does not exist or could not be found: {sourceDirName}");
            }

            var subDirectories = sourceDirectory.GetDirectories();

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var files = sourceDirectory.GetFiles();

            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith(".meta")) { continue; }

                string tempPath = Path.Combine(destDirName, file.Name);

                if (!File.Exists(tempPath))
                {
                    file.CopyTo(tempPath, false);
                }
            }

            foreach (var subDirectory in subDirectories)
            {
                string tempPath = Path.Combine(destDirName, subDirectory.Name);
                DirectoryCopy(subDirectory.FullName, tempPath);
            }
        }
    }
}
