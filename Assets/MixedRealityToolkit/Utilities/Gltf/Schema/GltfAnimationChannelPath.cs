// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The name of the node's TRS property to modify, or the weights of the Morph Target it instantiates.
    /// For the translation property, the values that are provided by the sampler are the translation along the x, y, and z axes.
    /// For the rotation property, the values are a quaternion in the order (x, y, z, w), where w is the scalar.
    /// For the scale property, the values are the scaling factors along the x, y, and z axes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.channel.target.schema.json
    /// </summary>
    public enum GltfAnimationChannelPath
    {
        translation,
        rotation,
        scale,
        weights
    }
}