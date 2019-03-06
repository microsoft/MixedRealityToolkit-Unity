// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The type of primitives to render.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.primitive.schema.json
    /// </summary>
    public enum GltfDrawMode
    {
        Points = 0,
        Lines = 1,
        LineLoop = 2,
        LineStrip = 3,
        Triangles = 4,
        TriangleStrip = 5,
        TriangleFan = 6
    }
}