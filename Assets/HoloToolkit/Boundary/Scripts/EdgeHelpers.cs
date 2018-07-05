// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.Boundary;
using System;
using UnityEngine;

namespace HoloToolkit.Unity.Boundary
{
    public static class EdgeHelpers
    {
        // A value that is larger than the widest possible room. We use this
        // to create line segments that are "guaranteed" to hit a piece of the
        // room boundary
        private const float largeValue = 10000;
        private const float smallValue = -largeValue;

        // Sentinel value
        public static readonly Vector2 InvalidPoint = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        /// <summary>
        /// Helper to know when a point is invalid.
        /// </summary>
        public static bool IsValidPoint(Vector2 point)
        {
            return !float.IsNegativeInfinity(point.x) && !float.IsNegativeInfinity(point.y);
        }

        /// <summary>
        /// Returns true if the given point is within the boundary.
        /// </summary>
        public static bool IsInside(Edge[] edges, Vector2 point)
        {
            // Check if a ray to the right (X+) intersects with an odd number of edges (inside) or an even number of edges (outside)
            Vector2 farPoint = point;
            farPoint.x = largeValue;
            var rightEdge = new Edge(point, farPoint);
            int intersections = 0;
            foreach (var edge in edges)
            {
                if (IsValidPoint(GetIntersection(edge, rightEdge)))
                {
                    ++intersections;
                }
            }
            return (intersections & 1) == 1;
        }

        /// <summary>
        /// Gets the point where two edges intersect. Value is InvalidPoint if they do not.
        /// </summary>
        public static Vector2 GetIntersection(Edge edge1, Edge edge2)
        {
            float s1_x = edge1.Bx - edge1.Ax;
            float s1_y = edge1.By - edge1.Ay;
            float s2_x = edge2.Bx - edge2.Ax;
            float s2_y = edge2.By - edge2.Ay;

            float s, t;
            s = (-s1_y * (edge1.Ax - edge2.Ax) + s1_x * (edge1.Ay - edge2.Ay)) / (-s2_x * s1_y + s1_x * s2_y);
            t = (s2_x * (edge1.Ay - edge2.Ay) - s2_y * (edge1.Ax - edge2.Ax)) / (-s2_x * s1_y + s1_x * s2_y);

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                // Collision detected
                return new Vector2(edge1.Ax + (t * s1_x), edge1.Ay + (t * s1_y));
            }

            return InvalidPoint;
        }
    }
}
