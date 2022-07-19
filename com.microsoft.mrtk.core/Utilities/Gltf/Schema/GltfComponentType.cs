// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/accessor.schema.json:componentType <para/>
    /// The datatype of components in the attribute.  All valid values correspond to WebGL enums.
    /// The corresponding typed arrays are 'Int8Array', 'Uint8Array', 'Int16Array', 'Uint16Array', 'Uint32Array', and 'Float32Array', respectively.
    /// 5125 (UNSIGNED_INT) is only allowed when the accessor contains indices, i.e., the accessor is only referenced by 'primitive.indices'.
    /// </summary>
    public enum GltfComponentType
    {
        Byte = 5120,
        UnsignedByte = 5121,
        Short = 5122,
        UnsignedShort = 5123,
        UnsignedInt = 5125,
        Float = 5126
    }
}