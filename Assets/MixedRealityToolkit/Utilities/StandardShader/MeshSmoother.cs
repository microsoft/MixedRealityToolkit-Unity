// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// TODO
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class MeshSmoother : MonoBehaviour
    {
        [Tooltip("TODO")]
        [SerializeField]
        private bool smoothNormals = false;

        private MeshFilter meshFilter = null;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();

            if (smoothNormals)
            {
                SmoothNormals();
            }
        }

        public void SmoothNormals()
        {
            var mesh = meshFilter.mesh;
            mesh.SetUVs(2, CalculateSmoothNormals(mesh.vertices, mesh.normals));
        }

        private static List<Vector3> CalculateSmoothNormals(Vector3[] verticies, Vector3[] normals)
        {
            // Group all vertices that share the same location in space.
            var groupedVerticies = new Dictionary<Vector3, List<KeyValuePair<int, Vector3>>>();

            for (int i = 0; i < verticies.Length; ++i)
            {
                var vertex = verticies[i];
                List<KeyValuePair<int, Vector3>> group;

                if (!groupedVerticies.TryGetValue(vertex, out group))
                {
                    group = new List<KeyValuePair<int, Vector3>>();
                    groupedVerticies[vertex] = group;
                }

                group.Add(new KeyValuePair<int, Vector3>(i, vertex));
            }

            var smoothNormals = new List<Vector3>(normals);

            // If none of the vertices can be grouped, no smoothing can be applied, simply return the default normals.
            if (groupedVerticies.Count == verticies.Length)
            {
                return smoothNormals;
            }

            // Average the normals of each group.
            foreach (var group in groupedVerticies)
            {
                var smoothingGroup = group.Value;

                // No need to smooth a group of one.
                if (smoothingGroup.Count != 1)
                {
                    var smoothedNormal = Vector3.zero;

                    foreach (var vertex in smoothingGroup)
                    {
                        smoothedNormal += normals[vertex.Key];
                    }

                    smoothedNormal.Normalize();

                    foreach (var vertex in smoothingGroup)
                    {
                        smoothNormals[vertex.Key] = smoothedNormal;
                    }
                }
            }

            return smoothNormals;
        }
    }
}
