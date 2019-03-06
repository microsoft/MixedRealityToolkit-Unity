// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Indices of those attributes that deviate from their initialization value.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/accessor.sparse.indices.schema.json
    /// </summary>
    [Serializable]
    public class GltfAccessorSparseIndices : GltfProperty
    {
        /// <summary>
        /// The index of the bufferView with sparse indices.
        /// Referenced bufferView can't have ARRAY_BUFFER or ELEMENT_ARRAY_BUFFER target.
        /// </summary>
        public int bufferView;

        /// <summary>
        /// The offset relative to the start of the bufferView in bytes. Must be aligned.
        /// <minimum>0</minimum>
        /// </summary>
        public int byteOffset;

        /// <summary>
        /// The indices data type. Valid values correspond to WebGL enums:
        /// `5121` (UNSIGNED_BYTE)
        /// `5123` (UNSIGNED_SHORT)
        /// `5125` (UNSIGNED_INT)
        /// </summary>
        public GltfComponentType ComponentType;
    }
}