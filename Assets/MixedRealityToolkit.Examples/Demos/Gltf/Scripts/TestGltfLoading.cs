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
    [AddComponentMenu("Scripts/MRTK/Examples/TestGltfLoading")]
    public class TestGltfLoading : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The relative asset path to the glTF asset in the Streaming Assets folder.")]
        private string uri = "\\GltfModels\\Lantern\\glTF\\Lantern.gltf";

        public string Uri
        {
            get => uri;
        }

        [SerializeField]
        [Tooltip("Scale factor to apply on load")]
        private float ScaleFactor = 1.0f;

        [SerializeField]
        public GameObject DebugText;

        private async void Start()
        {
            var path = $"{Application.streamingAssetsPath}{uri}";
            path = path.Replace("/", "\\");

            if (!File.Exists(path))
            {
                Debug.LogError($"Unable to find the glTF object at {path}");
                DebugText.SetActive(true);
                return;
            }

            DebugText.SetActive(false);

            GltfObject gltfObject = null;

            try
            {
                gltfObject = await GltfUtility.ImportGltfObjectFromPathAsync(path);

                // Put object in front of user
                gltfObject.GameObjectReference.transform.position = new Vector3(0.0f, 0.0f, 1.0f);

                gltfObject.GameObjectReference.transform.localScale *= this.ScaleFactor;
            }
            catch (Exception e)
            {
                Debug.LogError($"TestGltfLoading start failed - {e.Message}\n{e.StackTrace}");
            }

            if (gltfObject != null)
            {
                Debug.Log("Import successful");
            }
        }
        
    }
}