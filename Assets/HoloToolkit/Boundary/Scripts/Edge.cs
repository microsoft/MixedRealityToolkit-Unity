// Copyright(c) Microsoft Corporation.All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.Boundary
{
    /// <summary>
    /// Helper struct to hold an edge.
    /// </summary>
    public struct Edge
    {
        public float Ax;
        public float Ay;
        public float Bx;
        public float By;

        public Edge(float ax, float ay, float bx, float by)
        {
            Ax = ax;
            Ay = ay;
            Bx = bx;
            By = by;
        }

        public Edge(Vector2 pointA, Vector2 pointB)
        {
            Ax = pointA.x;
            Bx = pointB.x;
            Ay = pointA.y;
            By = pointB.y;
        }
    }
}