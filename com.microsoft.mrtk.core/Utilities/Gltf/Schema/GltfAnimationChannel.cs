﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Targets an animation's sampler at a node's property.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.channel.schema.json
    /// </summary>
    [Serializable]
    public class GltfAnimationChannel : GltfProperty
    {
        /// <summary>
        /// The index of a sampler in this animation used to compute the value for the
        /// target, e.g., a node's translation, rotation, or scale (TRS).
        /// </summary>
        public int sampler;

        /// <summary>
        /// The index of the node and TRS property to target.
        /// </summary>
        public GltfAnimationChannelTarget target;
    }
}