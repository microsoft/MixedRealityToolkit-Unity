// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// A keyframe animation.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.schema.json
    /// </summary>
    [Serializable]
    public class GltfAnimation : GltfChildOfRootProperty
    {
        /// <summary>
        /// An array of channels, each of which targets an animation's sampler at a
        /// node's property. Different channels of the same animation can't have equal
        /// targets.
        /// </summary>
        public GltfAnimationChannel[] channels;

        /// <summary>
        /// An array of samplers that combines input and output accessors with an
        /// interpolation algorithm to define a keyframe graph (but not its target).
        /// </summary>
        public GltfAnimationSampler[] samplers;
    }
}