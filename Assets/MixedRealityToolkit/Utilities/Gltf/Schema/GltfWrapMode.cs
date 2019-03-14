// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Texture wrap mode.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/sampler.schema.json
    /// </summary>
    public enum GltfWrapMode
    {
        None = 0,
        ClampToEdge = 33071,
        MirroredRepeat = 33648,
        Repeat = 10497
    }
}