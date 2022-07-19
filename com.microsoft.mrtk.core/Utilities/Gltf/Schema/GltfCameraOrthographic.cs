// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Gltf.Schema
{
    /// <summary>
    /// An orthographic camera containing properties to create an orthographic
    /// projection matrix.
    /// https://github.com/KhronosGroup/glTF/blob/master/specification/2.0/schema/camera.orthographic.schema.json
    /// </summary>
    [Serializable]
    public class GltfCameraOrthographic : GltfProperty
    {
        /// <summary>
        /// The floating-point horizontal magnification of the view.
        /// </summary>
        public double xMag;

        /// <summary>
        /// The floating-point vertical magnification of the view.
        /// </summary>
        public double yMag;

        /// <summary>
        /// The floating-point distance to the far clipping plane.
        /// </summary>
        public double zFar;

        /// <summary>
        /// The floating-point distance to the near clipping plane.
        /// </summary>
        public double zNear;
    }
}