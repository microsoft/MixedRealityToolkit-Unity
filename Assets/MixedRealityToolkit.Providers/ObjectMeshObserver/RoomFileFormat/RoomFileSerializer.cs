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
        /// Serializes a list of Mesh objects to the specified writer.
        /// Optionally transforms the vertices into the supplied secondarySpace.
        /// </summary>
        /// <param name="writer">The writer to which to write the serialized objects.</param>
        /// <param name="meshes">List of MeshFilter objects to be serialized.</param>
        /// <param name="secondarySpace">New space to transform the vertices into.</param>
        public static void Serialize(BinaryWriter writer, IList<MeshFilter> meshes, Transform secondarySpace = null)
        {
            if (writer == null)
            {
                // todo: error
                return;
            }

            foreach (MeshFilter meshFilter in meshes)
            {
                WriteMesh(writer, meshFilter.sharedMesh, meshFilter.transform, secondarySpace);
            }

            writer.Flush();
        }

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
                // todo: error
                return meshes;
            }

            while (reader.BaseStream.Length - reader.BaseStream.Position >= HeaderSize)
            {
                meshes.Add(ReadMesh(reader));
            }

            return meshes;
        }

        /// <summary>
        /// Writes a Mesh object to the data stream.
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="mesh">The Mesh object to be written.</param>
        /// <param name="transform">If provided, will transform all vertices into world space before writing.</param>
        /// <param name="secondarySpace">Secondary space to transform the vertices into.</param>
        private static void WriteMesh(BinaryWriter writer, Mesh mesh, Transform transform = null, Transform secondarySpace = null)
        {
            // Write the mesh data.
            WriteFileHeader(writer, mesh.vertexCount, mesh.triangles.Length);
            WriteVertices(writer, mesh.vertices, transform, secondarySpace);
            WriteTriangleIndicies(writer, mesh.triangles);
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
        /// Writes the file header to the data stream.
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="vertexCount">Count of vertices in the mesh.</param>
        /// <param name="triangleIndexCount">Count of triangle indices in the mesh.</param>
        private static void WriteFileHeader(BinaryWriter writer, int vertexCount, int triangleIndexCount)
        {
            writer.Write(vertexCount);
            writer.Write(triangleIndexCount);
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
        /// Writes a mesh's vertices to the data stream.
        /// </summary>
        /// <param name="reader">BinaryReader representing the data stream.</param>
        /// <param name="vertices">Array of Vector3 structures representing each vertex.</param>
        /// <param name="transform">If provided, will convert all vertices into world space before writing.</param>
        /// <param name="secondarySpace">If provided, will convert the vertices local to this space.</param>
        private static void WriteVertices(BinaryWriter writer, Vector3[] vertices, Transform transform = null, Transform secondarySpace = null)
        {
            if (transform != null)
            {
                for (int v = 0, vLength = vertices.Length; v < vLength; ++v)
                {
                    Vector3 vertex = transform.TransformPoint(vertices[v]);
                    if (secondarySpace != null)
                    {
                        vertex = secondarySpace.InverseTransformPoint(vertex);
                    }
                    writer.Write(vertex.x);
                    writer.Write(vertex.y);
                    writer.Write(vertex.z);
                }
            }
            else
            {
                foreach (Vector3 vertex in vertices)
                {
                    writer.Write(vertex.x);
                    writer.Write(vertex.y);
                    writer.Write(vertex.z);
                }
            }
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
        /// Writes the vertex indices that represent a mesh's triangles to the data stream
        /// </summary>
        /// <param name="writer">BinaryWriter representing the data stream.</param>
        /// <param name="triangleIndices">Array of integers that describe how the vertex indices form triangles.</param>
        private static void WriteTriangleIndicies(BinaryWriter writer, int[] triangleIndices)
        {
            foreach (int index in triangleIndices)
            {
                writer.Write(index);
            }
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