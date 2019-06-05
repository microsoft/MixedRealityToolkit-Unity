// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Specifies if the attribute is a scalar, vector, or matrix.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/accessor.schema.json
    /// </summary>
    public enum GltfAccessorAttributeType
    {
        SCALAR,
        VEC2,
        VEC3,
        VEC4,
        MAT2,
        MAT3,
        MAT4
    }
}