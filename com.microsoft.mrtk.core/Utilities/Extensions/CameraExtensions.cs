// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Extension methods for the Unity's Camera class
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Get the horizontal FOV from the stereo camera in radians
        /// </summary>
        public static float GetHorizontalFieldOfViewRadians(this Camera camera)
        {
            return 2f * Mathf.Atan(Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * camera.aspect);
        }

        /// <summary>
        /// Get the horizontal FOV from the stereo camera in degrees
        /// </summary>
        public static float GetHorizontalFieldOfViewDegrees(this Camera camera)
        {
            return camera.GetHorizontalFieldOfViewRadians() * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Test if a point will be rendered on the screen in either eye.
        /// </summary>
        /// <param name="camera">The camera to check the point against.</param>
        /// <param name="position">The 3D position to test.</param>
        public static bool IsInFOV(this Camera camera, Vector3 position)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(position);

            return screenPoint.z >= camera.nearClipPlane && screenPoint.z <= camera.farClipPlane
                && screenPoint.x >= 0 && screenPoint.x <= 1
                && screenPoint.y >= 0 && screenPoint.y <= 1;
        }


        /// <summary>
        /// Test if a point in the cone inscribed into the view frustum of the given camera.
        /// </summary>
        /// <remarks>
        /// The cone is inscribed to a radius equal to the vertical height of the camera's field
        /// of view.  By default, the cone's tip is severed by an amount defined by the camera's
        /// far and near clip planes.
        /// </remarks>
        /// <param name="camera">The camera to check the point against.</param>
        /// <param name="point">Point to test</param>
        /// <param name="coneAngleBufferDegrees">Degrees to expand the cone radius by.</param>
        /// <returns><see langword="true"/> if the given point is in the cone inscribed into the view frustum of the given camera, otherwise <see langword="false"/>.</returns>
        public static bool IsInFOVCone(
            this Camera camera,
            Vector3 point,
            float coneAngleBufferDegrees = 0)
        {
            return MathUtilities.IsInFOVCone(camera.transform,
                point,
                camera.fieldOfView + coneAngleBufferDegrees,
                camera.nearClipPlane,
                camera.farClipPlane
                );
        }

        /// <summary>
        /// Gets the frustum size at a given distance from the camera.
        /// </summary>
        /// <param name="camera">The camera to get the frustum size for</param>
        /// <param name="distanceFromCamera">The distance from the camera to get the frustum size at</param>
        public static Vector2 GetFrustumSizeForDistance(this Camera camera, float distanceFromCamera)
        {
            Vector2 frustumSize = new Vector2
            {
                y = 2.0f * distanceFromCamera * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad)
            };
            frustumSize.x = frustumSize.y * camera.aspect;

            return frustumSize;
        }

        /// <summary>
        /// Gets the distance to the camera that a specific frustum height would be at.
        /// </summary>
        /// <param name="camera">The camera to get the distance from</param>
        /// <param name="frustumHeight">The frustum height</param>
        public static float GetDistanceForFrustumHeight(this Camera camera, float frustumHeight)
        {
            return frustumHeight * 0.5f / Mathf.Max(0.00001f, Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
        }
    }
}