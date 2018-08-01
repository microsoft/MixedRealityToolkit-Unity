// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.Boundary
{
    public class InscribedRectangle
    {
        // Total number of starting points randomly generated within the boundary
        private const int randomPointCount = 30;

        // A value that is larger than the widest possible room. We use this
        // to create line segments that are "guaranteed" to hit a piece of the
        // room boundary
        private const float largeValue = 10000;
        private const float smallValue = -largeValue;

        // The total amount of height we want to gain with each binary search
        // change before we decide that it's good enough, in meters
        private const float minimumHeightGain = 0.01f;

        private float DegreesToRadians(double deg) { return (float)(Math.PI / 180 * deg); }

        // Parameters for the inscribed rectangle
        private Vector2 center = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        private float angle;
        private float width;
        private float height;

        /// <summary>
        /// Constructor that takes in a list of points and generates a seed.
        /// </summary>
        public InscribedRectangle(IList<Vector3> points) : this(points, (int)DateTime.UtcNow.Ticks) { }

        /// <summary>
        /// Constructor that takes in a list of points and sets a fixed random seed to get repeatable results.
        /// </summary>
        public InscribedRectangle(IList<Vector3> points, int randomSeed) : this(EdgeHelpers.ConvertVector3ListToEdgeArray(points), randomSeed) { }

        /// <summary>
        /// Constructor that takes in an array of Edges and generates a seed.
        /// </summary>
        public InscribedRectangle(Edge[] edges) : this(edges, (int)DateTime.UtcNow.Ticks) { }

        /// <summary>
        /// Constructor that takes in an array of Edges and sets a fixed random seed to get repeatable results.
        /// </summary>
        public InscribedRectangle(Edge[] edges, int randomSeed)
        {
            FindInscribedRectangle(edges, randomSeed, out center, out angle, out width, out height);
        }

        /// <summary>
        /// Use this to determine if there is a valid inscribed rectangle.
        /// </summary>
        public bool IsRectangleValid
        {
            get { return EdgeHelpers.IsValidPoint(center); }
        }

        /// <summary>
        /// Retrieves the parameters describing the largest inscribed rectangle.
        /// within the bounds
        /// </summary>
        public void GetRectangleParams(out Vector2 centerOut, out float angleOut, out float widthOut, out float heightOut)
        {
            if (!IsRectangleValid)
            {
                throw new InvalidOperationException();
            }

            centerOut = center;
            angleOut = angle;
            widthOut = width;
            heightOut = height;
        }

        /// <summary>
        /// Returns the four points that make up the inscribed rectangle.
        /// </summary>
        public Vector2[] GetRectanglePoints()
        {
            if (!IsRectangleValid)
            {
                throw new InvalidOperationException();
            }

            var points = new Vector2[4];

            float x = width / 2.0f;
            float y = height / 2.0f;
            points = new Vector2[]
            {
                new Vector2(x, y),
                new Vector2(x, -y),
                new Vector2(-x, -y),
                new Vector2(-x, y)
            };

            for (int i = 0; i < points.Length; ++i)
            {
                points[i] = RotatePoint(Vector2.zero, DegreesToRadians(angle), points[i]);
                points[i] += center;
            }

            return points;
        }

        /// <summary>
        /// Returns true if the given point is within the inscribed rectangle.
        /// </summary>
        public bool IsPointInRectangleBounds(Vector2 point)
        {
            if (!IsRectangleValid)
            {
                throw new InvalidOperationException();
            }

            point -= center;
            point = RotatePoint(Vector2.zero, DegreesToRadians(-angle), point);
            return (Math.Abs(point.x) <= (width / 2.0f)) && (Math.Abs(point.y) <= height / 2.0f);
        }

        /// <summary>
        /// Rotate a point about another point by the specified angle in radians.
        /// </summary>
        private Vector2 RotatePoint(Vector2 origin, float angleRad, Vector2 point)
        {
            Vector2 retval = point;
            if (0.0f == angleRad)
            {
                return retval;
            }

            // Translate to origin of rotation
            retval.x -= origin.x;
            retval.y -= origin.y;

            // Rotate the point
            float s = (float)Math.Sin(angleRad);
            float c = (float)Math.Cos(angleRad);
            float rotatedX = retval.x * c - retval.y * s;
            float rotatedY = retval.x * s + retval.y * c;

            // Translate back and return
            retval.x = rotatedX + origin.x;
            retval.y = rotatedY + origin.y;
            return retval;
        }

        /// <summary>
        /// Given a point inside of the boundary, finds the points on the
        /// boundary directly above, below, left and right of the point,
        /// with respect to the given angle.
        /// </summary>
        private bool FindSurroundingCollisionPoints(
            Edge[] edges,
            Vector2 point, float angleRad, out Vector2 topCollisionPoint,
            out Vector2 bottomCollisionPoint, out Vector2 leftCollisionPoint,
            out Vector2 rightCollisionPoint)
        {
            topCollisionPoint = EdgeHelpers.InvalidPoint;
            bottomCollisionPoint = EdgeHelpers.InvalidPoint;
            leftCollisionPoint = EdgeHelpers.InvalidPoint;
            rightCollisionPoint = EdgeHelpers.InvalidPoint;

            bool isInside = EdgeHelpers.IsInside(edges, point);
            if (!isInside)
            {
                return false;
            }

            // Find the top and bottom collision points by creating a large line segment that goes through the point to MAX and MIN values on Y
            var topEndpoint = new Vector2(point.x, largeValue);
            var bottomEndpoint = new Vector2(point.x, smallValue);
            topEndpoint = RotatePoint(point, angleRad, topEndpoint);
            bottomEndpoint = RotatePoint(point, angleRad, bottomEndpoint);
            var verticalLine = new Edge(topEndpoint, bottomEndpoint);
            // Find the left and right collision points by creating a large line segment that goes through the point to MAX and Min values on X
            var rightEndpoint = new Vector2(largeValue, point.y);
            var leftEndpoint = new Vector2(smallValue, point.y);
            rightEndpoint = RotatePoint(point, angleRad, rightEndpoint);
            leftEndpoint = RotatePoint(point, angleRad, leftEndpoint);
            var horizontalLine = new Edge(rightEndpoint, leftEndpoint);

            // Loop the edges and find the nearest intersection point
            foreach (var edge in edges)
            {
                Vector2 verticalIntersection = EdgeHelpers.GetIntersection(edge, verticalLine);
                if (EdgeHelpers.IsValidPoint(verticalIntersection))
                {
                    // Is this intersection above or below the point?
                    bool isAbove = RotatePoint(point, -angleRad, verticalIntersection).y > point.y;
                    if (isAbove)
                    {
                        // If this collision point is closer than the previous one
                        if (!EdgeHelpers.IsValidPoint(topCollisionPoint) ||
                            Vector2.SqrMagnitude(point - verticalIntersection) < Vector2.SqrMagnitude(point - topCollisionPoint))
                        {
                            topCollisionPoint = verticalIntersection;
                        }
                    }
                    else
                    {
                        if (!EdgeHelpers.IsValidPoint(bottomCollisionPoint) ||
                            Vector2.SqrMagnitude(point - verticalIntersection) < Vector2.SqrMagnitude(point - bottomCollisionPoint))
                        {
                            bottomCollisionPoint = verticalIntersection;
                        }
                    }
                }  // If vertical intersection found

                Vector2 horizontalIntersection = EdgeHelpers.GetIntersection(edge, horizontalLine);
                if (EdgeHelpers.IsValidPoint(horizontalIntersection))
                {
                    // Is this intersection left or right of the point?
                    bool isLeft = RotatePoint(point, -angleRad, horizontalIntersection).x < point.x;
                    if (isLeft)
                    {
                        // Is it closer than the previous intersection?
                        if (!EdgeHelpers.IsValidPoint(leftCollisionPoint) ||
                            Vector2.SqrMagnitude(point - horizontalIntersection) < Vector2.SqrMagnitude(point - leftCollisionPoint))
                        {
                            leftCollisionPoint = horizontalIntersection;
                        }
                    }
                    else
                    {
                        // Is it closer than the previous intersection?
                        if (!EdgeHelpers.IsValidPoint(rightCollisionPoint) ||
                            Vector2.SqrMagnitude(point - horizontalIntersection) < Vector2.SqrMagnitude(point - rightCollisionPoint))
                        {
                            rightCollisionPoint = horizontalIntersection;
                        }
                    }
                }
            }

            // Assert that any point inside should have intersection points on all sides with the polygon
            if (!EdgeHelpers.IsValidPoint(topCollisionPoint) || !EdgeHelpers.IsValidPoint(bottomCollisionPoint) ||
                !EdgeHelpers.IsValidPoint(leftCollisionPoint) || !EdgeHelpers.IsValidPoint(rightCollisionPoint))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to fit rectangles using predefined aspect ratios in the space
        /// centered on the given point, and rotated by the given angle.
        /// It will return the maximum rectangle it could fit (its aspect ratio
        /// and dimensions), within a margin of error. Returns false if it could
        /// not fit any rectangles that meet the criteria.
        /// </summary>
        private bool TryFitMaximumRectangleAtAngle(
            Edge[] edges,
            Vector2 center, float angleRad, float minArea,
            out float aspectRatio, out float width, out float height)
        {
            width = 0;
            height = 0;
            aspectRatio = 0;

            float[] aspectRatios = {1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, 5.5f, 6, 6.5f, 7, 7.5f,
                8, 8.5f, 9, 9.5f, 10, 10.5f, 11, 11.5f, 12, 12.5f, 13, 13.5f, 14, 14.5f, 15};

            // Start by calculating max width and height by ray-casting a cross from the point at the given angle
            // and taking the shortest leg of each ray. Width is the longest.
            Vector2 topCollisionPoint;
            Vector2 bottomCollisionPoint;
            Vector2 leftCollisionPoint;
            Vector2 rightCollisionPoint;
            if (!FindSurroundingCollisionPoints(edges, center, angleRad,
                out topCollisionPoint, out bottomCollisionPoint, out leftCollisionPoint, out rightCollisionPoint))
            {
                return false;
            }

            float verticalMinDistanceToEdge =
                Math.Min(Vector2.Distance(center, topCollisionPoint), Vector2.Distance(center, bottomCollisionPoint));
            float horizontalMinDistanceToEdge =
                Math.Min(Vector2.Distance(center, leftCollisionPoint), Vector2.Distance(center, rightCollisionPoint));

            // Width is the largest of the possible dimensions (doubled of course)
            float maxWidth = Math.Max(verticalMinDistanceToEdge, horizontalMinDistanceToEdge) * 2.0f;
            float maxHeight = Math.Min(verticalMinDistanceToEdge, horizontalMinDistanceToEdge) * 2.0f;

            // For each aspect ratio we do a binary search to find the maximum rectangle that fits, though once we start increasing our area by minimumHeightGain we call it good enough
            foreach (var candidateAspectRatio in aspectRatios)
            {
                // The height is limited by the width. If a height would make our width exceed maxWidth, it can't be used
                float searchHeightUpperBound = Math.Max(maxHeight, maxWidth / candidateAspectRatio);

                // Set to the min height that will out perform our previous area at the given aspect ratio. This is 0 the first time.
                // Derived from biggestAreaSoFar=height*(height*aspctRatio)
                float searchHeightLowerBound = (float)Math.Sqrt(Math.Max((width * height), minArea) / candidateAspectRatio);

                // If the lowest value needed to outperform the previous best is greater than our max, this aspect ratio can't outperform what we've already calculated
                if (searchHeightLowerBound > searchHeightUpperBound || searchHeightLowerBound * candidateAspectRatio > maxWidth)
                {
                    continue;
                }

                float currentTestingHeight = Math.Max(searchHeightLowerBound, maxHeight / 2);

                // Do the binary search until continuing to search will not give us a significant win
                do
                {
                    if (WillRectangleFit(edges, center, angleRad, candidateAspectRatio * currentTestingHeight, currentTestingHeight))
                    {
                        // Binary search up-ward

                        // If the rectangle will fit, increase the lower bounds of our binary search
                        searchHeightLowerBound = currentTestingHeight;

                        width = currentTestingHeight * candidateAspectRatio;
                        height = currentTestingHeight;
                        aspectRatio = candidateAspectRatio;
                        currentTestingHeight = (searchHeightUpperBound + currentTestingHeight) / 2;
                    }
                    else
                    {
                        // If the rectangle won't fit, update our upper bound and lower our binary search
                        searchHeightUpperBound = currentTestingHeight;
                        currentTestingHeight = (currentTestingHeight + searchHeightLowerBound) / 2;
                    }
                }
                while ((searchHeightUpperBound - searchHeightLowerBound) > minimumHeightGain);
            }

            return aspectRatio > 0;
        }

        /// <summary>
        /// Returns true if a rectangle centered at the given point, at the
        /// given angle and dimensions, will fit in the polygon.
        /// </summary>
        private bool WillRectangleFit(Edge[] edges, Vector2 center, float angleRad, float width, float height)
        {
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            var topLeft = new Vector2(center.x - halfWidth, center.y + halfHeight);
            var topRight = new Vector2(center.x + halfWidth, center.y + halfHeight);
            var bottomLeft = new Vector2(center.x - halfWidth, center.y - halfHeight);
            var bottomRight = new Vector2(center.x + halfWidth, center.y - halfHeight);

            topLeft = RotatePoint(center, angleRad, topLeft);
            topRight = RotatePoint(center, angleRad, topRight);
            bottomLeft = RotatePoint(center, angleRad, bottomLeft);
            bottomRight = RotatePoint(center, angleRad, bottomRight);

            var top = new Edge(topLeft, topRight);
            var right = new Edge(topRight, bottomRight);
            var bottom = new Edge(bottomLeft, bottomRight);
            var left = new Edge(topLeft, bottomLeft);

            // If any edges of the polygon intersect with any of our edges, it won't fit
            foreach (var edge in edges)
            {
                if (EdgeHelpers.IsValidPoint(EdgeHelpers.GetIntersection(edge, top)) || EdgeHelpers.IsValidPoint(EdgeHelpers.GetIntersection(edge, right)) ||
                    EdgeHelpers.IsValidPoint(EdgeHelpers.GetIntersection(edge, bottom)) || EdgeHelpers.IsValidPoint(EdgeHelpers.GetIntersection(edge, left)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds a large inscribed rectangle. Tries to be maximal but this is
        /// best effort. The algorithm used was inspired by the blog post
        /// https://d3plus.org/blog/behind-the-scenes/2014/07/08/largest-rect/
        /// Random points within the polygon are chosen, and then 2 lines are
        /// drawn through those points. The midpoints of those lines are
        /// used as the center of various rectangles, using a binary search to
        /// vary the size, until the largest fit-able rectangle is found.
        /// This is then repeated for predefined angles (0-180 in steps of 15)
        /// and aspect ratios (1 to 15 in steps of 0.5).
        /// </summary>
        private void FindInscribedRectangle(Edge[] edges, int randomSeed, out Vector2 center, out float angle, out float width, out float height)
        {
            center = EdgeHelpers.InvalidPoint;
            angle = width = height = 0;

            // Find min x, min y, max x, max y and generate random
            // points in this range until we have randomPointCount
            // random starting points
            float minX = largeValue;
            float minY = largeValue;
            float maxX = smallValue;
            float maxY = smallValue;

            foreach (var edge in edges)
            {
                if (edge.Ax < minX || edge.Bx < minX)
                {
                    minX = Math.Min(edge.Ax, edge.Bx);
                }
                if (edge.Ay < minY || edge.By < minY)
                {
                    minY = Math.Min(edge.Ay, edge.By);
                }
                if (edge.Ax > maxX || edge.Bx > maxX)
                {
                    maxX = Math.Max(edge.Ax, edge.Bx);
                }
                if (edge.Ay > maxY || edge.By > maxY)
                {
                    maxY = Math.Max(edge.Ay, edge.By);
                }
            }

            // Generate random points
            Vector2[] startingPoints = new Vector2[randomPointCount];
            {
                var random = new System.Random(randomSeed);

                for (int pointIndex = 0; pointIndex < randomPointCount; ++pointIndex)
                {
                    Vector2 candidatePoint;

                    do
                    {
                        candidatePoint.x = ((float)random.NextDouble() * (maxX - minX)) + minX;
                        candidatePoint.y = ((float)random.NextDouble() * (maxY - minY)) + minY;
                    }
                    while (!EdgeHelpers.IsInside(edges, candidatePoint));

                    startingPoints[pointIndex] = candidatePoint;
                }
            }

            float[] angles = { 0, 15, 30, 45, 60, 75, 90, 105, 120, 135, 150, 165 };

            foreach (var candidateAngle in angles)
            {
                // For each randomly generated starting point
                foreach (var startingPoint in startingPoints)
                {
                    // Find the collision point of a cross through the given point at the given angle
                    // Ignore the return value. A return value of false indicates one of the points is bad.
                    // We will check each point's validity ourselves anyway.
                    Vector2 topCollisionPoint;
                    Vector2 bottomCollisionPoint;
                    Vector2 leftCollisionPoint;
                    Vector2 rightCollisionPoint;
                    FindSurroundingCollisionPoints(edges, startingPoint, DegreesToRadians(candidateAngle),
                        out topCollisionPoint, out bottomCollisionPoint, out leftCollisionPoint, out rightCollisionPoint);

                    // Now calculate the midpoint between top and bottom (the "vertical midpoint") and left and right (the "horizontal midpoint")
                    if (EdgeHelpers.IsValidPoint(topCollisionPoint) && EdgeHelpers.IsValidPoint(bottomCollisionPoint))
                    {
                        var verticalMidpoint = new Vector2((topCollisionPoint.x + bottomCollisionPoint.x) / 2,
                            (topCollisionPoint.y + bottomCollisionPoint.y) / 2);
                        float aspectRatio;
                        float w;
                        float h;
                        if (TryFitMaximumRectangleAtAngle(edges, verticalMidpoint, DegreesToRadians(candidateAngle), width * height, out aspectRatio, out w, out h))
                        {
                            center = verticalMidpoint;
                            angle = candidateAngle;
                            width = w;
                            height = h;
                        }
                    }

                    if (EdgeHelpers.IsValidPoint(leftCollisionPoint) && EdgeHelpers.IsValidPoint(rightCollisionPoint))
                    {
                        var horizontalMidpoint = new Vector2((leftCollisionPoint.x + rightCollisionPoint.x) / 2,
                            (leftCollisionPoint.y + rightCollisionPoint.y) / 2);
                        float aspectRatio;
                        float w;
                        float h;
                        if (TryFitMaximumRectangleAtAngle(edges, horizontalMidpoint, DegreesToRadians(candidateAngle), width * height, out aspectRatio, out w, out h))
                        {
                            center = horizontalMidpoint;
                            angle = candidateAngle;
                            width = w;
                            height = h;
                        }
                    }
                }
            }
        }
    }
}
