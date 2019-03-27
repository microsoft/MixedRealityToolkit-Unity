// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    public class CameraIntrinsics
    {
        //
        // Summary:
        //     Gets the focal length of the camera.
        //
        // Returns:
        //     The focal length of the camera.
        public Vector2 FocalLength { get; private set; }
        //
        // Summary:
        //     Gets the image height of the camera, in pixels.
        //
        // Returns:
        //     The image height of the camera, in pixels.
        public uint ImageHeight { get; private set; }
        //
        // Summary:
        //     Gets the image width of the camera, in pixels.
        //
        // Returns:
        //     The image width of the camera, in pixels.
        public uint ImageWidth { get; private set; }
        //
        // Summary:
        //     Gets the principal point of the camera.
        //
        // Returns:
        //     The principal point of the camera.
        public Vector2 PrincipalPoint { get; private set; }
        //
        // Summary:
        //     Gets the radial distortion coefficient of the camera.
        //
        // Returns:
        //     The radial distortion coefficient of the camera.
        public Vector3 RadialDistortion { get; private set; }
        //
        // Summary:
        //     Gets the tangential distortion coefficient of the camera.
        //
        // Returns:
        //     The tangential distortion coefficient of the camera.
        public Vector2 TangentialDistortion { get; private set; }
        //
        // Summary:
        //     Gets a matrix that transforms a 3D point to video frame pixel coordinates without
        //     compensating for the distortion model of the camera. The 2D point resulting from
        //     this transformation will not accurately map to the pixel coordinate in a video
        //     frame unless the app applies its own distortion compensation. This is useful
        //     for apps that choose to implement GPU-based distortion compensation instead of
        //     using UndistortPoint, which uses the CPU to compute the distortion compensation.
        //
        // Returns:
        //     Gets a matrix that transforms a 3D point to the video frame pixel coordinates
        //     without compensating for the distortion model of the camera.
        public Matrix4x4 UndistortedProjectionTransform { get; private set; }

        public CameraIntrinsics(
            Vector2 focalLength,
            uint imageWidth,
            uint imageHeight,
            Vector2 principalPoint,
            Vector3 radialDistortion,
            Vector2 tangentialDistortion,
            Matrix4x4 undistortedProjectionTransform)
        {
            FocalLength = focalLength;
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            PrincipalPoint = principalPoint;
            RadialDistortion = radialDistortion;
            TangentialDistortion = tangentialDistortion;
            UndistortedProjectionTransform = undistortedProjectionTransform;
        }
    }
}
