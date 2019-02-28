// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Utilities.Gltf.Serialization;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{
    public class TestGltfLoading : MonoBehaviour
    {
        [SerializeField]
        private string uri = string.Empty;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uri))
            {
                uri = "\\MixedRealityToolkit.Examples\\Demos\\Gltf\\Models\\Lantern\\glTF\\Lantern.gltf";
                var path = $"{Application.dataPath}{uri}";
                path = path.Replace("/", "\\");
                Debug.Assert(File.Exists(path));
            }
        }

        private async void Start()
        {
            await new WaitForSeconds(5f);

            var gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync($"{Application.dataPath}{uri}");

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}