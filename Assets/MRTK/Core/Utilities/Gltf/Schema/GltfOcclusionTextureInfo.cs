// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/material.occlusionTextureInfo.schema.json
    /// </summary>
    [Serializable]
    public class GltfOcclusionTextureInfo : GltfTextureInfo
    {
        /// <summary>
        /// A scalar multiplier controlling the amount of occlusion applied.
        /// A value of 0.0 means no occlusion.
        /// A value of 1.0 means full occlusion.
        /// This value is ignored if the corresponding texture is not specified.
        /// This value is linear.
        /// <minimum>0.0</minimum>
        /// <maximum>1.0</maximum>
        /// </summary>
        public double strength = 1d;
    }
}