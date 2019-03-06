// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/material.normalTextureInfo.schema.json
    /// </summary>
    [Serializable]
    public class GltfNormalTextureInfo : GltfTextureInfo
    {
        /// <summary>
        /// The scalar multiplier applied to each normal vector of the texture.
        /// This value is ignored if normalTexture is not specified.
        /// This value is linear.
        /// </summary>
        public double scale = 1d;
    }
}