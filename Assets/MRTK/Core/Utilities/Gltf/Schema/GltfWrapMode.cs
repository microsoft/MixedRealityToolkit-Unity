// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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