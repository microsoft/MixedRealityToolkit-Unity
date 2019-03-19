// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Boundary;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// The EdgeUtilities class provides functionality for working with <see cref="Microsoft.MixedReality.Toolkit.Boundary.Edge"/> objects.
    /// </summary>
    public static class EdgeUtilities
    {
        /// <summary>
        /// A value that should be larger than the maximum boundary width.
        /// </summary>
        /// <remarks>
        /// This value is used to ensure that line segments are created 
        /// that will intersect with a piece of the room boundary.
        /// </remarks>
        internal static readonly float maxWidth = 10000f;

        /// <summary>
        /// A value representing an invalid point.
        /// </summary>
        public static readonly Vector2 InvalidPoint = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        /// <summary>
        /// Determines if the specified point is within the provided geometry.
        /// </summary>
        /// <param name="geometryEdges">The geometry for which we are checking the point.</param>
        /// <param name="point">The point being checked.</param>
        /// <returns>
        /// True if the point falls within the geometry, false otherwise.
        /// </returns>
        public static bool IsInsideBoundary(Edge[] geometryEdges, Vector2 point)
        {
            if (geometryEdges.Length == 0)
            {
                return false;
            }

            // Check if a ray to the right (X+) intersects with an 
            // odd number of edges (inside) or an even number of edges (outside)
            var rightEdge = new Edge(point, new Vector2(maxWidth, point.y));

            int intersections = 0;
            for (int i = 0; i < geometryEdges.Length; i++)
            {
                if (IsValidPoint(GetIntersectionPoint(geometryEdges[i], rightEdge)))
                {
                    ++intersections;
                }
            }

            return (intersections & 1) == 1;
        }

        /// <summary>
        /// Checks to see if a point is valid.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if the point is valid, false otherwise.</returns>
        /// <remarks>
        /// A point is considered invalid if any one of it's coordinate values are infinite or not a number.
        /// </remarks>
        public static bool IsValidPoint(Vector2 point)
        {
            return (!float.IsInfinity(point.x) && !float.IsInfinity(point.y) &&
                    !float.IsNaN(point.x) && !float.IsNaN(point.y));
        }

        /// <summary>
        /// Value calculated by GetIntersectionPoint()
        /// </summary>
        /// <remarks>
        /// This is to save multiple allocations when GetIntersectionPoint is called repeatedly.
        /// </remarks>
        private static Vector2 intersectionPoint = Vector2.zero;

        /// <summary>
        /// Returns the point at which two <see cref="Microsoft.MixedReality.Toolkit.Boundary.Edge"/> values intersect.
        /// </summary>
        /// <param name="edgeA">The first edge</param>
        /// <param name="edgeB">The second edge</param>
        /// <returns>
        /// A Vector2 representing the point at which the two edges intersect, InscribedRectangleDescription.InvalidPoint otherwise.
        /// </returns>
        public static Vector2 GetIntersectionPoint(Edge edgeA, Edge edgeB)
        {
            float sA_x = edgeA.PointB.x - edgeA.PointA.x;
            float sA_y = edgeA.PointB.y - edgeA.PointA.y;
            float sB_x = edgeB.PointB.x - edgeB.PointA.x;
            float sB_y = edgeB.PointB.y - edgeB.PointA.y;

            float s = (-sA_y * (edgeA.PointA.x - edgeB.PointA.x) + sA_x * (edgeA.PointA.y - edgeB.PointA.y)) / (-sB_x * sA_y + sA_x * sB_y);
            float t = (sB_x * (edgeA.PointA.y - edgeB.PointA.y) - sB_y * (edgeA.PointA.x - edgeB.PointA.x)) / (-sB_x * sA_y + sA_x * sB_y);

            if ((s >= 0) && (s <= 1) && (t >= 0) && (t <= 1))
            {
                // Collision detected
                intersectionPoint.x = edgeA.PointA.x + (t * sA_x);
                intersectionPoint.y = edgeA.PointA.y + (t * sA_y);
                return intersectionPoint;
            }

            return InvalidPoint;
        }
    }
}