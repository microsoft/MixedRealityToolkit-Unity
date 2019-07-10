// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema.Extensions;
using Microsoft.MixedReality.Toolkit.Utilities.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    [Serializable]
    public class GltfObject : GltfProperty
    {
        #region Serialized Fields

        /// <summary>
        /// Names of glTF extensions used somewhere in this asset.
        /// </summary>
        [JSONArray(minItems:1)]
        public string[] extensionsUsed;

        /// <summary>
        /// Names of glTF extensions required to properly load this asset.
        /// </summary>
        [JSONArray(minItems:1)]
        public string[] extensionsRequired;

        /// <summary>
        /// An array of accessors. An accessor is a typed view into a bufferView.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfAccessor[] accessors;

        /// <summary>
        /// An array of keyframe animations.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfAnimation[] animations;

        /// <summary>
        /// Metadata about the glTF asset.
        /// </summary>
        public GltfAssetInfo asset;

        /// <summary>
        /// An array of buffers. A buffer points to binary geometry, animation, or skins.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfBuffer[] buffers;

        /// <summary>
        /// An array of bufferViews.
        /// A bufferView is a view into a buffer generally representing a subset of the buffer.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfBufferView[] bufferViews;

        /// <summary>
        /// An array of cameras. A camera defines a projection matrix.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfCamera[] cameras;

        /// <summary>
        /// An array of images. An image defines data used to create a texture.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfImage[] images;

        /// <summary>
        /// An array of materials. A material defines the appearance of a primitive.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfMaterial[] materials;

        /// <summary>
        /// An array of meshes. A mesh is a set of primitives to be rendered.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfMesh[] meshes;

        /// <summary>
        /// An array of nodes.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfNode[] nodes;

        /// <summary>
        /// An array of samplers. A sampler contains properties for texture filtering and wrapping modes.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfSampler[] samplers;

        /// <summary>
        /// The index of the default scene.
        /// </summary>
        public int scene;

        /// <summary>
        /// An array of scenes.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfScene[] scenes;

        /// <summary>
        /// An array of skins. A skin is defined by joints and matrices.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfSkin[] skins;

        /// <summary>
        /// An array of textures.
        /// </summary>
        [JSONArray(minItems:1)]
        public GltfTexture[] textures;

        #endregion Serialized Fields

        /// <summary>
        /// The name of the gltf Object.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The absolute path to the glTF Object on disk.
        /// </summary>
        public string Uri { get; internal set; }

        /// <summary>
        /// The <see href="https://docs.unity3d.com/ScriptReference/GameObject.html">GameObject</see> reference for the gltf Object.
        /// </summary>
        public GameObject GameObjectReference { get; internal set; }

        /// <summary>
        /// The list of registered glTF extensions found for this object.
        /// </summary>
        public List<GltfExtension> RegisteredExtensions { get; internal set; } = new List<GltfExtension>();

        /// <summary>
        /// Flag for setting object load behavior.
        /// Importers should run on the main thread; all other loading scenarios should likely use the background thread
        /// </summary>
        internal bool UseBackgroundThread { get; set; } = true;
    }
}
