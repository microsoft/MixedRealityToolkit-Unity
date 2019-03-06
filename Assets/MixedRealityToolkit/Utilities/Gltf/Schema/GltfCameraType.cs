// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// Specifies if the camera uses a perspective or orthographic projection.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/camera.schema.json
    /// </summary>
    public enum GltfCameraType
    {
        perspective,
        orthographic
    }
}