// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// A node in the node hierarchy.
    /// When the node contains `skin`, all `mesh.primitives` must contain `JOINT`
    /// and `WEIGHT` attributes.  A node can have either a `matrix` or any combination
    /// of `translation`/`rotation`/`scale` (TRS) properties.
    /// TRS properties are converted to matrices and postmultiplied in
    /// the `T * R * S` order to compose the transformation matrix;
    /// first the scale is applied to the vertices, then the rotation, and then
    /// the translation. If none are provided, the transform is the Identity.
    /// When a node is targeted for animation
    /// (referenced by an animation.channel.target), only TRS properties may be present;
    /// `matrix` will not be present.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/node.schema.json
    /// </summary>
    [Serializable]
    public class GltfNode : GltfChildOfRootProperty
    {
        /// <summary>
        /// If true, extracts transform, rotation, scale values from the Matrix4x4. Otherwise uses the Transform, Rotate, Scale directly as specified by the node.
        /// </summary>
        public bool useTRS;

        /// <summary>
        /// The index of the camera referenced by this node.
        /// </summary>
        public int camera = -1;

        /// <summary>
        /// The indices of this node's children.
        /// </summary>
        public int[] children;

        /// <summary>
        /// The index of the skin referenced by this node.
        /// </summary>
        public int skin = -1;

        /// <summary>
        /// A floating-point 4x4 transformation matrix stored in column-major order.
        /// </summary>
        public double[] matrix = { 1d, 0d, 0d, 0d, 0d, 1d, 0d, 0d, 0d, 0d, 1d, 0d, 0d, 0d, 0d, 1d };

        public Matrix4x4 Matrix { get; set; }

        /// <summary>
        /// The index of the mesh in this node.
        /// </summary>
        public int mesh = -1;

        /// <summary>
        /// The node's unit quaternion rotation in the order (x, y, z, w),
        /// where w is the scalar.
        /// </summary>
        public float[] rotation = { 0f, 0f, 0f, 1f };

        /// <summary>
        /// The node's non-uniform scale.
        /// </summary>
        public float[] scale = { 1f, 1f, 1f };

        /// <summary>
        /// The node's translation.
        /// </summary>
        public float[] translation = { 0f, 0f, 0f };

        /// <summary>
        /// The weights of the instantiated Morph Target.
        /// Number of elements must match number of Morph Targets of used mesh.
        /// </summary>
        public double[] weights;
    }
}