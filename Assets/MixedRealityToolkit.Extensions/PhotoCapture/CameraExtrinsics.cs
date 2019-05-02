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
        public Matrix4x4 ViewFromWorld = Matrix4x4.identity;

        public override string ToString()
        {
            var position = ViewFromWorld.GetColumn(3);
            var rotation = Quaternion.LookRotation(ViewFromWorld.GetColumn(2), ViewFromWorld.GetColumn(1));
            return $"Position: {position.ToString()}, Rotation: {rotation.ToString()}";
        }
    }
}
