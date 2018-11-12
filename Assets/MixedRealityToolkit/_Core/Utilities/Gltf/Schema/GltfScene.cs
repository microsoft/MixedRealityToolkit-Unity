// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.Gltf.Schema
{
    /// <summary>
    /// The indices of each root node.
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