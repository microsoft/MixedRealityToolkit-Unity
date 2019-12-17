// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Extensions
{
    public static class MeshExtensions
    {
        public static int[] quadTriangles = new int[]
        {
            0, 3, 1,
            0, 2, 3,
            1, 3, 0,
            3, 2, 0
        };

        public static Vector2[] quadUVs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        /// <summary>
        /// Updates the mesh with width and height information from a quad.
        /// </summary>
        /// <param name="widthInMeters">Width of the quad in meters.</param>
        /// <param name="heightInMeters">Width of the quad in meters.</param>
        public static void CreateMeshFromQuad(this Mesh mesh, float widthInMeters, float heightInMeters)
        {
            List<Vector3> vertices = new List<Vector3>()
            {
                new Vector3(-widthInMeters / 2, -heightInMeters / 2, 0),
                new Vector3( widthInMeters / 2, -heightInMeters / 2, 0),
                new Vector3(-widthInMeters / 2,  heightInMeters / 2, 0),
                new Vector3( widthInMeters / 2,  heightInMeters / 2, 0)
            };

            mesh.SetVertices(vertices);
            mesh.SetIndices(quadTriangles, MeshTopology.Triangles, 0);
            mesh.SetUVs(0, new List<Vector2>(quadUVs));
        }

    }
}