// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// A set of primitives to be rendered. A node can contain one or more meshes.
    /// A node's transform places the mesh in the scene.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.schema.json
    /// </summary>
    [Serializable]
    public class GltfMesh : GltfChildOfRootProperty
    {
        /// <summary>
        /// An array of primitives, each defining geometry to be rendered with
        /// a material.
        /// <minItems>1</minItems>
        /// </summary>
        public GltfMeshPrimitive[] primitives;

        /// <summary>
        /// Array of weights to be applied to the Morph Targets.
        /// <minItems>0</minItems>
        /// </summary>
        public double[] weights;

        /// <summary>
        /// Unity Mesh wrapper for the GltfMesh
        /// </summary>
        public Mesh Mesh { get; internal set; }
    }
}