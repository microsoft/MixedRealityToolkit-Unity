// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{
    /// <summary>
    /// This glTF test loading script will load a glTF model from the streaming assets folder.
    /// </summary>
    /// <remarks>
    /// This scene needs to be opened before building to the device so the appropriate assets are copied.
    /// </remarks>
    public class TestGltfLoading : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The relative asset path to the glTF asset in the Streaming Assets folder.")]
        private string uri = "\\GltfModels\\Lantern\\glTF\\Lantern.gltf";

        private void OnValidate()
        {
            var path = $"{Application.streamingAssetsPath}{uri}";
            path = path.Replace("/", "\\");

            if (!File.Exists(path) &&
                Application.isEditor)
            {
                DirectoryCopy($"{Application.dataPath}\\MixedRealityToolkit.Examples\\Demos\\Gltf\\Models",
                              $"{Application.streamingAssetsPath}\\GltfModels");
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh(UnityEditor.ImportAssetOptions.ForceUpdate);
#endif
            }
        }

        private async void Start()
        {
            var path = $"{Application.streamingAssetsPath}{uri}";
            path = path.Replace("/", "\\");

            if (!File.Exists(path))
            {
                Debug.LogError($"Unable to find the glTF object at {path}");
            }

            GltfObject gltfObject = null;

            try
            {
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
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