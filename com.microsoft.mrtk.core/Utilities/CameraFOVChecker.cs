// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Camera extension methods to test if colliders are within camera's FOV. Uses caching to improve performance and
    /// ensure values are only computed once per frame.
    /// </summary>
    public static class CameraFOVChecker
    {
        private static int inFOVLastCalculatedFrame = -1;

        /// <summary>
        /// Map from object => is the object in the FOV for this frame.
        /// </summary>
        /// <remarks>
        /// This map is cleared every frame.
        /// </remarks>
        private static readonly Dictionary<(Collider, Camera), bool> inFOVColliderCache = new Dictionary<(Collider, Camera), bool>();

        /// <summary>
        /// List of corners shared across all sphere pointer query instances -- used to store list of corners for
        /// a bounds. Shared and static to avoid allocating memory each frame
        /// </summary>
        private static readonly List<Vector3> inFOVBoundsCornerPoints = new List<Vector3>();

        /// <summary>
        /// Test if a collider's bounds is within the camera's field of view.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This function utilizes a cache to test if this collider has been seen before and returns current 
        /// frame's calculated result.
        /// </para>
        /// <para>
        /// This is a loose field of view check, meaning it can return <see langword="true"/> in cases when the 
        /// collider is actually not in the field of view. This is because this function does an axis-aligned
        ///  check when testing for large colliders. So, if the axis aligned bounds are in the bounds of the 
        /// camera, it will return <see langword="true"/>.
        /// </para>
        /// </remarks>
        /// <param name="camera">The camera to test.</param>
        /// <param name="myCollider">The collider to test.</param>
        /// <returns><see langword="true"/> if a collider's bounds is within the camera's field of view, otherwise <see langword="false"/>.</returns>
        public static bool IsInFOVCached(this Camera camera, Collider myCollider)
        {
            // If the collider's size is zero, it is not visible. Return false.
            if (myCollider.bounds.size == Vector3.zero || myCollider.transform.localScale == Vector3.zero)
            {
                return false;
            }

            (Collider, Camera) cameraColliderPair = (myCollider, camera);

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
                Vector3 screenPoint = camera.WorldToViewportPoint(corner);

                bool isInFOV = screenPoint.z >= 0 && screenPoint.z <= camera.farClipPlane
                    && screenPoint.x >= 0 && screenPoint.x <= 1
                    && screenPoint.y >= 0 && screenPoint.y <= 1;

                if (isInFOV)
                {
                    inFOVColliderCache.Add(cameraColliderPair, true);
                    return true;
                }

                // If the point is behind the camera, the x and y viewport positions are negated.
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

            // Check that collider is visible even if all corners are not visible,
            // such as when having a large collider.
            result =
                zMax > 0                    // Front of collider is in front of the camera.
                && zMin < camera.farClipPlane  // Back of collider is not too far away.
                && xMin < 1                 // Left edge is not too far to the right.
                && xMax > 0                 // Right edge is not too far to the left.
                && yMin < 1                 // Bottom edge is not too high.
                && yMax > 0;                // Top edge is not too low.

            inFOVColliderCache.Add(cameraColliderPair, result);

            return result;
        }
    }
}
