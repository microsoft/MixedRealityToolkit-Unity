// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialObjectMeshObserver.RoomFile
{
    /// <summary>
    /// Converts a UnityEngine.Mesh object to and from an array of bytes that conform to the .room file format.
    ///    File header: vertex count (32 bit integer), triangle count (32 bit integer)
    ///    Vertex list: vertex.x, vertex.y, vertex.z (all 32 bit float)
    ///    Triangle index list: 32 bit integers
    /// </summary>
    public static class RoomFileSerializer
    {
        /// <summary>
        /// The mesh header consists of two 32 bit integers.
        /// </summary>
        private static int HeaderSize = sizeof(int) * 2;

        /// <summary>
        /// Deserializes a list of Mesh objects from the provided byte array.
        /// </summary>
        /// <param name="reader">The reader from which to deserialize the meshes.</param>
        /// <returns>Collection of Mesh objects.</returns>
        public static IList<Mesh> Deserialize(BinaryReader reader)
        {
            List<Mesh> meshes = new List<Mesh>();

            if (reader == null)
            {
                Debug.LogError("Null reader passed to Deserialize.");
                return meshes;
            }

            while (reader.BaseStream.Length - reader.BaseStream.Position >= HeaderSize)
            {
                meshes.Add(ReadMesh(reader));
            }

            return meshes;
        }

        /// <summary>
        /// Reads a single Mesh object from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <returns>Mesh object read from the stream.</returns>
        private static Mesh ReadMesh(BinaryReader reader)
        {
            int vertexCount = 0;
            int triangleIndexCount = 0;

            // Read the mesh data.
            ReadFileHeader(reader, out vertexCount, out triangleIndexCount);
            Vector3[] vertices = ReadVertices(reader, vertexCount);
            int[] triangleIndices = ReadTriangleIndicies(reader, triangleIndexCount);

            // Create the mesh.
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangleIndices;
            // Reconstruct the normals from the vertices and triangles.
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Reads the file header from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices in the mesh.</param>
        /// <param name="triangleIndexCount">Count of triangle indices in the mesh.</param>
        private static void ReadFileHeader(BinaryReader reader, out int vertexCount, out int triangleIndexCount)
        {
            vertexCount = reader.ReadInt32();
            triangleIndexCount = reader.ReadInt32();
        }

        /// <summary>
        /// Reads a mesh's vertices from the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices to read.</param>
        /// <returns>Array of Vector3 structures representing the mesh's vertices.</returns>
        private static Vector3[] ReadVertices(BinaryReader reader, int vertexCount)
        {
            Vector3[] vertices = new Vector3[vertexCount];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(reader.ReadSingle(),
                                        reader.ReadSingle(),
                                        reader.ReadSingle());
            }

            return vertices;
        }

        /// <summary>
        /// Reads the vertex indices that represent a mesh's triangles from the data stream
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="triangleIndexCount">Count of indices to read.</param>
        /// <returns>Array of integers that describe how the vertex indices form triangles.</returns>
        private static int[] ReadTriangleIndicies(BinaryReader reader, int triangleIndexCount)
        {
            int[] triangleIndices = new int[triangleIndexCount];

            for (int i = 0; i < triangleIndices.Length; i++)
            {
                triangleIndices[i] = reader.ReadInt32();
            }

            return triangleIndices;
        }
    }
}