// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Camera extension methods to test if colliders are within camera's FOV. Uses
    /// caching to improve performance and ensure values are only computed once per frame
    /// </summary>
    public static class CameraFOVChecker
    {
        // Help to clear caches when new frame runs
        private static int inFOVLastCalculatedFrame = -1;
        // Map from grabbable => is the grabbable in FOV for this frame. Cleared every frame
        private static Dictionary<Tuple<Collider, Camera>, bool> inFOVColliderCache = new Dictionary<Tuple<Collider, Camera>, bool>();
        // List of corners shared across all sphere pointer query instances --
        // used to store list of corners for a bounds. Shared and static
        // to avoid allocating memory each frame
        private static List<Vector3> inFOVBoundsCornerPoints = new List<Vector3>();

        /// <summary>
        /// Returns true if a collider's bounds is within the camera FOV. 
        /// Utilizes a cache to test if this collider has been seen before and returns current frame's calculated result.
        /// NOTE: This is a 'loose' FOV check -- it can return true in cases when the collider is actually not in the FOV
        /// because it does an axis-aligned check when testing for large colliders. So, if the axis aligned bounds are in the bounds of the camera, it will return true.
        /// </summary>
        /// <param name="myCollider">The collider to test</param>
        public static bool IsInFOVCached(this Camera cam, Collider myCollider)
        {
            // if the collider's size is zero, it is not visible. Return false.
            if (myCollider.bounds.size == Vector3.zero || myCollider.transform.localScale == Vector3.zero)
            {
                return false;
            }

            Tuple<Collider, Camera> cameraColliderPair = new Tuple<Collider, Camera>(myCollider, cam);
            bool result;
            if (inFOVLastCalculatedFrame != Time.frameCount)
            {
                inFOVColliderCache.Clear();
                inFOVLastCalculatedFrame = Time.frameCount;
            }
            else if (inFOVColliderCache.TryGetValue(cameraColliderPair, out result))
            {
                return result;
            }

            inFOVBoundsCornerPoints.Clear();
            BoundsExtensions.GetColliderBoundsPoints(myCollider, inFOVBoundsCornerPoints, 0);

            float xMin = float.MaxValue, yMin = float.MaxValue, zMin = float.MaxValue;
            float xMax = float.MinValue, yMax = float.MinValue, zMax = float.MinValue;
            for (int i = 0; i < inFOVBoundsCornerPoints.Count; i++)
            {
                var corner = inFOVBoundsCornerPoints[i];
                Vector3 screenPoint = cam.WorldToViewportPoint(corner);

                bool isInFOV = screenPoint.z >= 0 && screenPoint.z <= cam.farClipPlane
                    && screenPoint.x >= 0 && screenPoint.x <= 1
                    && screenPoint.y >= 0 && screenPoint.y <= 1;

                if (isInFOV)
                {
                    inFOVColliderCache.Add(cameraColliderPair, true);
                    return true;
                }

                // if the point is behind the camera, the x and y viewport positions are negated
                var zViewport = screenPoint.z;
                var xViewport = zViewport >= 0 ? screenPoint.x : -screenPoint.x;
                var yViewport = zViewport >= 0 ? screenPoint.y : -screenPoint.y;
                xMin = Mathf.Min(xMin, xViewport);
                yMin = Mathf.Min(yMin, yViewport);
                zMin = Mathf.Min(zMin, zViewport);
                xMax = Mathf.Max(xMax, xViewport);
                yMax = Mathf.Max(yMax, yViewport);
                zMax = Mathf.Max(zMax, zViewport);
            }

            // Check that collider is visible even if all corners are not visible
            // such as when having a large collider
            result =
                zMax > 0 // Front of collider is in front of the camera.
                && zMin < cam.farClipPlane // Back of collider is not too far away.
                && xMin < 1 // Left edge is not too far to the right.
                && xMax > 0 // Right edge is not too far to the left.
                && yMin < 1 // Bottom edge is not too high.
                && yMax > 0; // Top edge is not too low.

            inFOVColliderCache.Add(cameraColliderPair, result);

            return result;
        }

    }
}
