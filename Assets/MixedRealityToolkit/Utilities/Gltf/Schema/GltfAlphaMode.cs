// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The alpha rendering mode of the material.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/material.schema.json
    /// </summary>
    public enum GltfAlphaMode
    {
        OPAQUE,
        MASK,
        BLEND
    }
}