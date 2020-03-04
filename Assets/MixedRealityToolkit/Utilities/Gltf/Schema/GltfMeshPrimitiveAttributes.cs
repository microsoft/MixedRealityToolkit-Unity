// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Common mesh primitive attributes.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/mesh.primitive.schema.json
    /// </summary>
    public class GltfMeshPrimitiveAttributes
    {
        public static string POSITION = "POSITION";
        public static string NORMAL = "NORMAL";
        public static string TANGENT = "TANGENT";
        public static string TEXCOORD_0 = "TEXCOORD_0";
        public static string TEXCOORD_1 = "TEXCOORD_1";
        public static string TEXCOORD_2 = "TEXCOORD_2";
        public static string TEXCOORD_3 = "TEXCOORD_3";
        public static string COLOR_0 = "COLOR_0";
        public static string JOINTS_0 = "JOINTS_0";
        public static string WEIGHTS_0 = "WEIGHTS_0";
    }
}