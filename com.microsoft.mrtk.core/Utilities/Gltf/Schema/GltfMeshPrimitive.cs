// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Geometry to be rendered with the given material.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.primitive.schema.json
    /// </summary>
    [Serializable]
    public class GltfMeshPrimitive : GltfProperty, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The index of the accessor that contains mesh indices.
        /// When this is not defined, the primitives should be rendered without indices
        /// using `drawArrays()`. When defined, the accessor must contain indices:
        /// the `bufferView` referenced by the accessor must have a `target` equal
        /// to 34963 (ELEMENT_ARRAY_BUFFER); a `byteStride` that is tightly packed,
        /// i.e., 0 or the byte size of `componentType` in bytes;
        /// `componentType` must be 5121 (UNSIGNED_BYTE), 5123 (UNSIGNED_SHORT)
        /// or 5125 (UNSIGNED_INT), the latter is only allowed
        /// when `OES_element_index_uint` extension is used; `type` must be `\"SCALAR\"`.
        /// </summary>
        public int indices = -1;

        /// <summary>
        /// The index of the material to apply to this primitive when rendering.
        /// </summary>
        public int material = -1;

        /// <summary>
        /// The type of primitives to render. All valid values correspond to WebGL enums.
        /// </summary>
        public GltfDrawMode Mode { get; set; }

        [SerializeField]
        private string mode = null;

        /// <summary>
        /// An array of Morph Targets, each  Morph Target is a dictionary mapping
        /// attributes (only "POSITION" and "NORMAL" supported) to their deviations
        /// in the Morph Target (index of the accessor containing the attribute
        /// displacements' data).
        /// </summary>
        public List<Dictionary<string, int>> Targets { get; internal set; }

        /// <summary>
        /// A dictionary object, where each key corresponds to mesh attribute semantic
        /// and each value is the index of the accessor containing attribute's data.
        /// </summary>
        public GltfMeshPrimitiveAttributes Attributes { get; internal set; }

        /// <summary>
        /// Unity Mesh wrapper for the GltfMeshPrimitive SubMesh
        /// </summary>
        public Mesh SubMesh { get; internal set; }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (Enum.TryParse(mode, out GltfDrawMode result))
            {
                Mode = result;
            }
            else
            {
                Mode = GltfDrawMode.Triangles;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            mode = Mode.ToString();
        }
    }
}