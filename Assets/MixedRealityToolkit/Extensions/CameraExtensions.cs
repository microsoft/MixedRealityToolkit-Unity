// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
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
        /// Returns if a point will be rendered on the screen in either eye
        /// </summary>
        /// <param name="camera">The camera to check the point against</param>
        public static bool IsInFOV(this Camera camera, Vector3 position)
        {
            Vector3 screenPoint = camera.WorldToViewportPoint(position);

            return screenPoint.z >= camera.nearClipPlane && screenPoint.z <= camera.farClipPlane
                && screenPoint.x >= 0 && screenPoint.x <= 1
                && screenPoint.y >= 0 && screenPoint.y <= 1;
        }


        // Help to clear caches when new frame runs
        static private int lastCalculatedFrame = -1;
        // Map from grabbable => is the grabbable in FOV for this frame. Cleared every frame
        private static Dictionary<Collider, bool> colliderCache = new Dictionary<Collider, bool>();
        // List of corners shared across all sphere pointer query instances --
        // used to store list of corners for a bounds. Shared and static
        // to avoid allocating memory each frame
        private static List<Vector3> corners = new List<Vector3>();

        /// <summary>
        /// Returns true if a collider's bounds is within the camera FOV. 
        /// Utilizes a cache to test if this collider has been seen before and returns current frame's calculated result.
        /// </summary>
        /// <param name="myCollider">The collider to test</param>
        public static bool IsInFOVConeCached(this Camera cam,
            Collider myCollider)
        {
            if (lastCalculatedFrame != Time.frameCount)
            {
                colliderCache.Clear();
                lastCalculatedFrame = Time.frameCount;
            }

            if (colliderCache.TryGetValue(myCollider, out bool result))
            {
                return result;
            }

            corners.Clear();
            BoundsExtensions.GetColliderBoundsPoints(myCollider, corners, 0);

            float xMin = float.MaxValue, yMin = float.MaxValue, zMin = float.MaxValue;
            float xMax = float.MinValue, yMax = float.MinValue, zMax = float.MinValue;
            for (int i = 0; i < corners.Count; i++)
            {
                var corner = corners[i];
                if (cam.IsInFOVCone(corner, 0))
                {
                    colliderCache.Add(myCollider, true);
                    return true;
                }

                xMin = Mathf.Min(xMin, corner.x);
                yMin = Mathf.Min(yMin, corner.y);
                zMin = Mathf.Min(zMin, corner.z);
                xMax = Mathf.Max(xMax, corner.x);
                yMax = Mathf.Max(yMax, corner.y);
                zMax = Mathf.Max(zMax, corner.z);
            }

            // edge case: check if camera is inside the entire bounds of the collider;
            // Consider simplifying to myCollider.bounds.Contains(CameraCache.main.transform.position)
            var cameraPos = cam.transform.position;
            result = xMin <= cameraPos.x && cameraPos.x <= xMax
                && yMin <= cameraPos.y && cameraPos.y <= yMax
                && zMin <= cameraPos.z && cameraPos.z <= zMax;

            colliderCache.Add(myCollider, result);

            return result;
        }

        /// <summary>
        /// Returns true if a point is in the a cone inscribed into the Camera's frustrum, false otherwise
        /// The cone is inscribed to a radius equal to the vertical height of the camera's FOV.
        /// By default, the cone's tip is "chopped off" by an amount defined by the camera's
        /// far and near clip planes.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <param name="coneAngleBufferDegrees">Degrees to expand the cone radius by.</param>
        public static bool IsInFOVCone(this Camera camera,
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