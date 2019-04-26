// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Magnification filter mode.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/sampler.schema.json
    /// </summary>
    public enum GltfMagnificationFilterMode
    {
        None = 0,
        Nearest = 9728,
        Linear = 9729,
    }
}