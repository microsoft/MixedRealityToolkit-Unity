// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Joints and matrices defining a skin.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/skin.schema.json
    /// </summary>
    [Serializable]
    public class GltfSkin : GltfChildOfRootProperty
    {
        /// <summary>
        /// The index of the accessor containing the floating-point 4x4 inverse-bind matrices.
        /// The default is that each matrix is a 4x4 Identity matrix, which implies that inverse-bind
        /// matrices were pre-applied.
        /// </summary>
        public int inverseBindMatrices = -1;

        /// <summary>
        /// The index of the node used as a skeleton root.
        /// When undefined, joints transforms resolve to scene root.
        /// </summary>
        public int skeleton = -1;

        /// <summary>
        /// Indices of skeleton nodes, used as joints in this skin.  The array length must be the
        /// same as the `count` property of the `inverseBindMatrices` accessor (when defined).
        /// </summary>
        public int[] joints;
    }
}