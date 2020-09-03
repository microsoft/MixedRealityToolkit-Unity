// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf
{
    public class GltfAsset : ScriptableObject
    {
        [SerializeField]
        private GameObject model;

        public GameObject Model
        {
            get => model;
            internal set => model = value;
        }

        [SerializeField]
        private GltfObject gltfObject;

        public GltfObject GltfObject
        {
            get => gltfObject;
            internal set => gltfObject = value;
        }
    }
}