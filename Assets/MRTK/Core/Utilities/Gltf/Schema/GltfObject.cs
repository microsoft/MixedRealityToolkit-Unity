// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema.Extensions;
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
        public string[] extensionsUsed;

        /// <summary>
        /// Names of glTF extensions required to properly load this asset.
        /// </summary>
        public string[] extensionsRequired;

        /// <summary>
        /// An array of accessors. An accessor is a typed view into a bufferView.
        /// </summary>
        public GltfAccessor[] accessors;

        /// <summary>
        /// An array of keyframe animations.
        /// </summary>
        public GltfAnimation[] animations;

        /// <summary>
        /// Metadata about the glTF asset.
        /// </summary>
        public GltfAssetInfo asset;

        /// <summary>
        /// An array of buffers. A buffer points to binary geometry, animation, or skins.
        /// </summary>
        public GltfBuffer[] buffers;

        /// <summary>
        /// An array of bufferViews.
        /// A bufferView is a view into a buffer generally representing a subset of the buffer.
        /// </summary>
        public GltfBufferView[] bufferViews;

        /// <summary>
        /// An array of cameras. A camera defines a projection matrix.
        /// </summary>
        public GltfCamera[] cameras;

        /// <summary>
        /// An array of images. An image defines data used to create a texture.
        /// </summary>
        public GltfImage[] images;

        /// <summary>
        /// An array of materials. A material defines the appearance of a primitive.
        /// </summary>
        public GltfMaterial[] materials;

        /// <summary>
        /// An array of meshes. A mesh is a set of primitives to be rendered.
        /// </summary>
        public GltfMesh[] meshes;

        /// <summary>
        /// An array of nodes.
        /// </summary>
        public GltfNode[] nodes;

        /// <summary>
        /// An array of samplers. A sampler contains properties for texture filtering and wrapping modes.
        /// </summary>
        public GltfSampler[] samplers;

        /// <summary>
        /// The index of the default scene.
        /// </summary>
        public int scene;

        /// <summary>
        /// An array of scenes.
        /// </summary>
        public GltfScene[] scenes;

        /// <summary>
        /// An array of skins. A skin is defined by joints and matrices.
        /// </summary>
        public GltfSkin[] skins;

        /// <summary>
        /// An array of textures.
        /// </summary>
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
        /// The Node Id and corresponding GameObject pairs.
        /// </summary>
        public Dictionary<int, GameObject> NodeGameObjectPairs { get; internal set; } = new Dictionary<int, GameObject>();

        /// <summary>
        /// The list of registered glTF extensions found for this object.
        /// </summary>
        public List<GltfExtension> RegisteredExtensions { get; internal set; } = new List<GltfExtension>();

        /// <summary>
        /// Flag for setting object load behavior.
        /// Importers should run on the main thread; all other loading scenarios should likely use the background thread
        /// </summary>
        internal bool UseBackgroundThread { get; set; } = true;

        /// <summary>
        /// Get an accessor from an accessor index
        /// </summary>
        public GltfAccessor GetAccessor(int index)
        {
            if (index < 0) return null;

            var accessor = accessors[index];
            accessor.BufferView = bufferViews[accessor.bufferView];
            accessor.BufferView.Buffer = buffers[accessor.BufferView.buffer];
            return accessor;
        }
    }
}
