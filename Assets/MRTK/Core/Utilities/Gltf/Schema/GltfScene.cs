// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The indices of each root node.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/scene.schema.json
    /// </summary>
    [Serializable]
    public class GltfScene : GltfChildOfRootProperty
    {
        /// <summary>
        /// Indices of each root node.
        /// </summary>
        public int[] nodes;
    }
}