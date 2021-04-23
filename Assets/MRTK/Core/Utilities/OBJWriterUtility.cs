// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Utility for generating and saving OBJ files from GameObjects and their Meshes
    /// </summary>
    public static class OBJWriterUtility
    {
        /// <summary>
        /// Export mesh data of provided GameObject, and children if enabled, to file provided in OBJ format
        /// </summary>
        /// <remarks>
        /// <para>Traversal of GameObject mesh data is done via Coroutine on main Unity thread due to limitations by Unity.
        /// If a file does not exist at given file path, a new one is automatically created
        /// If applicable, children Mesh data will be bundled into same OBJ file.</para>
        /// </remarks>
        public static async Task ExportOBJAsync(GameObject root, string filePath, bool includeChildren = true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new Exception("Invalid file path");
            }

            Debug.Log($"Exporting GameObject {root.name} to {filePath} OBJ file");

            // Await coroutine that must execute on Unity's main thread
            string getObjData = await CreateOBJFileContentAsync(root, includeChildren);

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    await sw.WriteAsync(getObjData);
                }
            }
        }

        /// <summary>
        /// Coroutine async method that generates string in OBJ file format of provided GameObject's Mesh, and possibly children.
        /// </summary>
        /// <param name="target">GameObject to target for pulling MeshFilter data</param>
        /// <param name="includeChildren">Include Mesh data of children GameObjects as sub-meshes in output</param>
        /// <returns>string of all mesh data (no materials) in OBJ file format</returns>
        public static IEnumerator<string> CreateOBJFileContentAsync(GameObject target, bool includeChildren)
        {
            StringBuilder objBuffer = new StringBuilder();

            objBuffer.Append($"# {target.name}").AppendNewLine();
            var dt = DateTime.Now;
            objBuffer.Append($"# {dt.ToString(CultureInfo.InvariantCulture)}").AppendNewLine().AppendNewLine();

            Stack<Transform> processStack = new Stack<Transform>();
            processStack.Push(target.transform);

            // If including sub-meshes, need to track vertex indices in relation to entire file
            int startVertexIndex = 0;

            // DFS processing routine to add Mesh data to OBJ 
            while (processStack.Count != 0)
            {
                var current = processStack.Pop();

                MeshFilter meshFilter = current.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    CreateOBJDataForMesh(meshFilter, objBuffer, ref startVertexIndex);
                }

                if (includeChildren)
                {
                    for (int i = 0; i < current.childCount; i++)
                    {
                        processStack.Push(current.GetChild(i));
                    }
                }

                yield return null;
            }

            yield return objBuffer.ToString();
        }

        private static void CreateOBJDataForMesh(MeshFilter meshFilter, StringBuilder buffer, ref int startVertexIndex)
        {
            Mesh mesh = meshFilter.sharedMesh;
            if (!mesh)
            {
                return;
            }

            var transform = meshFilter.transform;

            buffer.Append("g ").Append(transform.name).AppendNewLine();

            foreach (Vector3 vertex in mesh.vertices)
            {
                Vector3 v = transform.TransformPoint(vertex);
                buffer.Append($"v {v.x} {v.y} {v.z}\n");
            }
            buffer.AppendNewLine();

            foreach (Vector3 normal in mesh.normals)
            {
                Vector3 vn = transform.TransformDirection(normal);
                buffer.Append($"vn {vn.x} {vn.y} {vn.z}\n");
            }

            buffer.AppendNewLine();
            foreach (Vector3 uv in mesh.uv)
            {
                buffer.Append($"vt {uv.x} {uv.y}\n");
            }

            for (int idx = 0; idx < mesh.subMeshCount; idx++)
            {
                buffer.AppendNewLine();

                int[] triangles = mesh.GetTriangles(idx);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    buffer.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                        triangles[i] + 1 + startVertexIndex, triangles[i + 1] + 1 + startVertexIndex, triangles[i + 2] + 1 + startVertexIndex));
                }
            }

            startVertexIndex += mesh.vertexCount;
        }
    }
}
