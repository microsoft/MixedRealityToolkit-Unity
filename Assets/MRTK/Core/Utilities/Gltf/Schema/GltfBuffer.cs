// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// A buffer points to binary geometry, animation, or skins.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/buffer.schema.json
    /// </summary>
    [Serializable]
    public class GltfBuffer : GltfChildOfRootProperty
    {
        /// <summary>
        /// The uri of the buffer.
        /// Relative paths are relative to the .gltf file.
        /// Instead of referencing an external file, the uri can also be a data-uri.
        /// </summary>
        public string uri;

        /// <summary>
        /// The length of the buffer in bytes.
        /// <minimum>0</minimum>
        /// </summary>
        public int byteLength = 0;

        public byte[] BufferData { get; internal set; }
    }
}