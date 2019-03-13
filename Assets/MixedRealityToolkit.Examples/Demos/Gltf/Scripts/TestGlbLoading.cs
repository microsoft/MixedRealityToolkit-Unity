// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Async;
using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Serialization;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.Gltf
{
    public class TestGlbLoading : MonoBehaviour
    {
        [SerializeField]
        private string uri = string.Empty;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(uri))
            {
                uri = "\\MixedRealityToolkit.Examples\\Demos\\Gltf\\Models\\Lantern\\glTF-Binary\\Lantern.glb";
                var path = $"{Application.dataPath}{uri}";
                path = path.Replace("/", "\\");
                Debug.Assert(File.Exists(path));
            }
        }
#endif

        private async void Start()
        {
#if UNITY_EDITOR
            await new WaitForSeconds(5f);

            var gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync($"{Application.dataPath}{uri}");

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
#else
            Debug.Log("TestGlbLoading.cs is not currently supported outside of the editor.");
#endif
        }
    }
}