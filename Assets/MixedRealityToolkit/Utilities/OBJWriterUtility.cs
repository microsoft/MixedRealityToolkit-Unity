// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    public static class OBJWriterUtility
    {
        public static IEnumerator<string> CreateOBJFile(GameObject target, bool includeChildren)
        {
            StringBuilder objData = new StringBuilder();

            objData.Append($"# {target.name}").NewLine();
            var dt = DateTime.Now;
            objData.Append($"# {dt.ToLongDateString()} - {dt.ToLongTimeString()}").NewLine().NewLine();

            Stack<Transform> processStack = new Stack<Transform>();
            processStack.Push(target.transform);

            try
            {
                int startVertexIndex = 0;
                while (processStack.Count != 0)
                {
                    var current = processStack.Pop();

                    MeshFilter meshFilter = current.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        WriteMesh(meshFilter, objData, ref startVertexIndex);
                    }

                    if (includeChildren)
                    {
                        for (int i = 0; i < current.childCount; i++)
                        {
                            processStack.Push(current.GetChild(i));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Test" + ex.Message);
            }

            yield return objData.ToString();
        }

        private static void WriteMesh(MeshFilter meshFilter, 
            StringBuilder output,
            ref int currentVertexIndex)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (!mesh)
            {
                return;
            }

            var transform = meshFilter.transform;
            int numVertices = 0;
            //Material[] mats = meshFilter.renderer.sharedMaterials;

            output.Append("g ").Append(transform.name).NewLine();

            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 v = transform.TransformPoint(vertex);
                numVertices++;
                output.Append($"v {v.x} {v.y} {-v.z}\n");
            }
            output.NewLine();

            foreach (Vector3 normal in mesh.normals)
            {
                Vector3 vn = transform.localRotation * normal;
                output.Append($"vn {-vn.x} {-vn.y} {vn.z}\n");
            }

            output.NewLine();
            foreach (Vector3 uv in mesh.uv)
            {
                output.Append($"vt {uv.x} {uv.y}\n");
            }

            for (int idx = 0; idx < mesh.subMeshCount; idx++)
            {
                output.NewLine();
                //sb.Append("usemtl ").Append(mats[material].name).Append("\n");
                //sb.Append("usemap ").Append(mats[material].name).Append("\n");

                int[] triangles = mesh.GetTriangles(idx);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    output.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + currentVertexIndex, triangles[i + 1] + 1 + currentVertexIndex, triangles[i + 2] + 1 + currentVertexIndex));
                }
            }

            currentVertexIndex += numVertices;
        }
    }
}
