// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The index of the node and TRS property that an animation channel targets.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.channel.target.schema.json
    /// </summary>
    [Serializable]
    public class GltfAnimationChannelTarget : GltfProperty
    {
        /// <summary>
        /// The index of the node to target.
        /// </summary>
        public int node = -1;

        /// <summary>
        /// The name of the node's TRS property to modify.
        /// </summary>
        public GltfAnimationChannelPath path;
    }
}