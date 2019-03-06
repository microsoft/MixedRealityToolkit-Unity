// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Common mesh primitive attributes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.primitive.schema.json
    /// </summary>
    /// <remarks>
    /// Application specific semantics are not supported
    /// </remarks>
    [Serializable]
    public class GltfMeshPrimitiveAttributes
    {
        public int POSITION = -1;
        public int NORMAL = -1;
        public int TANGENT = -1;
        public int TEXCOORD_0 = -1;
        public int TEXCOORD_1 = -1;
        public int TEXCOORD_2 = -1;
        public int TEXCOORD_3 = -1;
        public int COLOR_0 = -1;
        public int JOINTS_0 = -1;
        public int WEIGHTS_0 = -1;
    }
}