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
    public static class CameraFOVChecker
    {
        
        // Help to clear caches when new frame runs
        static private int inFOVConeLastCalculatedFrame = -1;
        // Map from grabbable => is the grabbable in FOV for this frame. Cleared every frame
        private static Dictionary<Collider, bool> inFOVConeColliderCache = new Dictionary<Collider, bool>();
        // List of corners shared across all sphere pointer query instances --
        // used to store list of corners for a bounds. Shared and static
        // to avoid allocating memory each frame
        private static List<Vector3> inFOVConeBoundsCornerPoints = new List<Vector3>();

        /// <summary>
        /// Returns true if a collider's bounds is within the camera FOV. 
        /// Utilizes a cache to test if this collider has been seen before and returns current frame's calculated result.
        /// </summary>
        /// <param name="myCollider">The collider to test</param>
        public static bool IsInFOVConeCached(this Camera cam,
            Collider myCollider)
        {
            if (inFOVConeLastCalculatedFrame != Time.frameCount)
            {
                inFOVConeColliderCache.Clear();
                inFOVConeLastCalculatedFrame = Time.frameCount;
            }

            if (inFOVConeColliderCache.TryGetValue(myCollider, out bool result))
            {
                return result;
            }

            inFOVConeBoundsCornerPoints.Clear();
            BoundsExtensions.GetColliderBoundsPoints(myCollider, inFOVConeBoundsCornerPoints, 0);

            float xMin = float.MaxValue, yMin = float.MaxValue, zMin = float.MaxValue;
            float xMax = float.MinValue, yMax = float.MinValue, zMax = float.MinValue;
            for (int i = 0; i < inFOVConeBoundsCornerPoints.Count; i++)
            {
                var corner = inFOVConeBoundsCornerPoints[i];
                if (cam.IsInFOVCone(corner, 0))
                {
                    inFOVConeColliderCache.Add(myCollider, true);
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

            inFOVConeColliderCache.Add(myCollider, result);

            return result;
        }

    }
}
