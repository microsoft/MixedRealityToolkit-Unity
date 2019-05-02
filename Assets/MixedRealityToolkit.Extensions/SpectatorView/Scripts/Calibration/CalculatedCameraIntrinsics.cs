// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    [Serializable]
    public class CalculatedCameraIntrinsics : CameraIntrinsics
    {
        public float ReprojectionError;

        public CalculatedCameraIntrinsics(
            float reprojectionError,
            Vector2 focalLength,
            uint imageWidth,
            uint imageHeight,
            Vector2 principalPoint,
            Vector3 radialDistortion,
            Vector2 tangentialDistortion,
            Matrix4x4 undistortedProjectionTransform) :
            base(
                focalLength,
                imageWidth,
                imageHeight,
                principalPoint,
                radialDistortion,
                tangentialDistortion,
                undistortedProjectionTransform)
        {
            ReprojectionError = reprojectionError;
        }

        public override string ToString()
        {
            return $"reprojection error: {ReprojectionError} {base.ToString()}";
        }
    }
}
