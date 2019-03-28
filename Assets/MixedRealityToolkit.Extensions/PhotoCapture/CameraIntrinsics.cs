// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture
{
    /// <summary>
    /// Contains information on camera intrinsic parameters.
    /// Note: This class wraps logic in Windows.Media.Devices.Core.CameraIntrinsics for use in Unity.
    /// </summary>
    public class CameraIntrinsics
    {
        /// <summary>
        /// Gets the focal length of the camera.
        /// </summary>
        public Vector2 FocalLength { get; private set; }

        /// <summary>
        /// Gets the image height of the camera, in pixels.
        /// </summary>
        public uint ImageHeight { get; private set; }

        /// <summary>
        /// Gets the image width of the camera, in pixels.
        /// </summary>
        public uint ImageWidth { get; private set; }

        /// <summary>
        /// Gets the principal point of the camera.
        /// </summary>
        public Vector2 PrincipalPoint { get; private set; }

        /// <summary>
        /// Gets the radial distortion coefficient of the camera.
        /// </summary>
        public Vector3 RadialDistortion { get; private set; }

        /// <summary>
        /// Gets the tangential distortion coefficient of the camera.
        /// </summary>
        public Vector2 TangentialDistortion { get; private set; }

        /// <summary>
        ///     Gets a matrix that transforms a 3D point to video frame pixel coordinates without
        ///     compensating for the distortion model of the camera.The 2D point resulting from
        ///     this transformation will not accurately map to the pixel coordinate in a video
        ///     frame unless the app applies its own distortion compensation.This is useful
        ///     for apps that choose to implement GPU-based distortion compensation instead of
        ///     using UndistortPoint, which uses the CPU to compute the distortion compensation.
        /// </summary>
        public Matrix4x4 UndistortedProjectionTransform { get; private set; }

        ./// <summary>
        /// CameraIntrinsics constructor
        /// </summary>
        /// <param name="focalLength">focal length for the camera</param>
        /// <param name="imageWidth">image width in pixels</param>
        /// <param name="imageHeight">image height in pixels</param>
        /// <param name="principalPoint">principal point for the camera </param>
        /// <param name="radialDistortion">radial distortion for the camera</param>
        /// <param name="tangentialDistortion">tangential distortion for the camera</param>
        /// <param name="undistortedProjectionTransform">Undistorted projection transform for the camera</param>
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
