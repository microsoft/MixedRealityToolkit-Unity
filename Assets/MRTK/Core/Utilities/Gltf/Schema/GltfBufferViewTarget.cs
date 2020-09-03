// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// The target that the GPU buffer should be bound to.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/bufferView.schema.json
    /// </summary>
    public enum GltfBufferViewTarget
    {
        None = 0,
        ArrayBuffer = 34962,
        ElementArrayBuffer = 34963,
    }
}