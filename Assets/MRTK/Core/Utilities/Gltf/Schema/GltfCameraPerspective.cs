// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// A perspective camera containing properties to create a perspective projection
    /// matrix.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/camera.perspective.schema.json
    /// </summary>
    [Serializable]
    public class GltfCameraPerspective : GltfProperty
    {
        /// <summary>
        /// The floating-point aspect ratio of the field of view.
        /// When this is undefined, the aspect ratio of the canvas is used.
        /// <minimum>0.0</minimum>
        /// </summary>
        public double aspectRatio;

        /// <summary>
        /// The floating-point vertical field of view in radians.
        /// <minimum>0.0</minimum>
        /// </summary>
        public double yFov;

        /// <summary>
        /// The floating-point distance to the far clipping plane. When defined,
        /// `zfar` must be greater than `znear`.
        /// If `zfar` is undefined, runtime must use infinite projection matrix.
        /// <minimum>0.0</minimum>
        /// </summary>
        public double zFar;

        /// <summary>
        /// The floating-point distance to the near clipping plane.
        /// <minimum>0.0</minimum>
        /// </summary>
        public double zNear;
    }
}