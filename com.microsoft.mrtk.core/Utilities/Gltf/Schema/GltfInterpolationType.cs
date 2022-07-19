// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Interpolation algorithm.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/animation.sampler.schema.json
    /// </summary>
    public enum GltfInterpolationType
    {
        LINEAR,
        STEP,
        CATMULLROMSPLINE,
        CUBICSPLINE
    }
}