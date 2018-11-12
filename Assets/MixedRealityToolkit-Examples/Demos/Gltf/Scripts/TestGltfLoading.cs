// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Serialization;
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
                uri = $"{Application.dataPath}\\MixedRealityToolkit-Examples\\Demos\\Gltf\\Models\\Lantern\\glTF\\Lantern.gltf";
                uri = uri.Replace("/", "\\");
                Debug.Assert(File.Exists(uri));
            }
        }

        private async void Start()
        {
            await new WaitForSeconds(5f);

            var gltfObject = await GltfSerializationUtility.GetGltfObjectFromPathAsync(uri);

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
    }
}