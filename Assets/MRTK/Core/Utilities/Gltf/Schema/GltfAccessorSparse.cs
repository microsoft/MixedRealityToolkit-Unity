// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Sparse storage of attributes that deviate from their initialization value.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/accessor.sparse.schema.json
    /// </summary>
    [Serializable]
    public class GltfAccessorSparse : GltfProperty
    {
        /// <summary>
        /// Number of entries stored in the sparse array.
        /// <minimum>1</minimum>
        /// </summary>
        public int count;

        /// <summary>
        /// Index array of size `count` that points to those accessor attributes that
        /// deviate from their initialization value. Indices must strictly increase.
        /// </summary>
        public GltfAccessorSparseIndices indicies;

        /// <summary>
        /// "Array of size `count` times number of components, storing the displaced
        /// accessor attributes pointed by `indices`. Substituted values must have
        /// the same `componentType` and number of components as the base accessor.
        /// </summary>
        public GltfAccessorSparseValues values;
    }
}