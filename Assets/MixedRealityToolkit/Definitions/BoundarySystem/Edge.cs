// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    /// <summary>
    /// The BoundaryEdge structure defines the points of a line segment that are used to
    /// construct a polygonal boundary.
    /// </summary>
    public struct Edge
    {
        /// <summary>
        /// The first point of the edge line segment.
        /// </summary>
        public readonly Vector2 PointA;

        /// <summary>
        /// The second point of the edge line segment.
        /// </summary>
        public readonly Vector2 PointB;

        /// <summary>
        /// Initializes the BoundaryEdge structure.
        /// </summary>
        /// <param name="pointA">The first point of the line segment.</param>
        /// <param name="pointB">The second point of the line segment.</param>
        public Edge(Vector2 pointA, Vector2 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }

        /// <summary>
        /// Initializes the BoundaryEdge structure.
        /// </summary>
        /// <param name="pointA">The first point of the line segment.</param>
        /// <param name="pointB">The second point of the line segment.</param>
        public Edge(Vector3 pointA, Vector3 pointB) :
            // Use the X and Z parameters as our edges are height agnostic.
            this(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z))
        { }
    }
}
