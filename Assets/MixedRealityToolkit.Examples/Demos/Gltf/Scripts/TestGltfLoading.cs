// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System;
using System.IO;
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
            GltfObject gltfObject = null;

            try
            {
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync($"{Application.dataPath}{uri}");
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
    }
}