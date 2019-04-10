// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// Camera extrinsics helper class
    /// </summary>
    [Serializable]
    public class CameraExtrinsics
    {
        /// <summary>
        /// Camera's view from world matrix
        /// </summary>
        public Matrix4x4 ViewFromWorld;
    }
}
