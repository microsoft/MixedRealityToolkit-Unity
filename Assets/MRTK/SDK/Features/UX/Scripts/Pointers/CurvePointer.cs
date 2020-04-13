// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Physics;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Extends line pointer to support curves. Useful for teleportation or other situations where multiple raysteps need to be tested along a spline
    /// </summary>
    [AddComponentMenu("Scripts/MRTK/SDK/CurvePointer")]
    public class CurvePointer : LinePointer
    {
        [Range(1, 50)]
        [SerializeField]
        [Tooltip("Number of ray steps to utilize in raycast operation along curve defined in LineBase. This setting has a high performance cost. Values above 20 are not recommended.")]
        protected int LineCastResolution = 10;

        private static readonly ProfilerMarker UpdateRaysPerfMarker = new ProfilerMarker("[MRTK] CurvePointer.UpdateRays");

        /// <inheritdoc />
        protected override void UpdateRays()
        {
            using (UpdateRaysPerfMarker.Auto())
            {
                // Make sure our array will hold
                if (Rays == null || Rays.Length != LineCastResolution)
                {
                    Rays = new RayStep[LineCastResolution];
                }

                float stepSize = 1f / Rays.Length;
                Vector3 lastPoint = LineBase.GetUnClampedPoint(0f);
                for (int i = 0; i < Rays.Length; i++)
                {
                    Vector3 currentPoint = LineBase.GetUnClampedPoint(stepSize * (i + 1));
                    Rays[i].UpdateRayStep(ref lastPoint, ref currentPoint);
                    lastPoint = currentPoint;
                }
            }
        }
    }
}