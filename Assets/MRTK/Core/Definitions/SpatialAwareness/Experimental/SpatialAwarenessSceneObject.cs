// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// Object encapsulating the components of a spatial awareness scene object.
    /// </summary>
    public class SpatialAwarenessSceneObject : BaseSpatialAwarenessObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        private SpatialAwarenessSceneObject() : base() { }

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessSceneObject"/>.
        /// </summary>
        /// <returns>
        /// A SpatialAwarenessSceneObject containing the fields that describe the scene object.
        /// </returns>
        public static SpatialAwarenessSceneObject Create(
            int id,
            SpatialAwarenessSurfaceTypes surfaceType,
            Vector3 position,
            Quaternion rotation,
            List<QuadData> quads,
            List<MeshData> meshData
            )
        {
            SpatialAwarenessSceneObject newObject = new SpatialAwarenessSceneObject
            {
                Id = id
            };

            newObject.SurfaceType = surfaceType;
            newObject.Position = position;
            newObject.Rotation = rotation;
            newObject.Quads = quads;
            newObject.Meshes = meshData;

            return newObject;
        }

        /// <summary>
        /// The world position for the scene object.
        /// </summary>
        public Vector3 Position
        {
            get;
            private set;
        }

        /// <summary>
        /// The world rotation for the scene object.
        /// </summary>
        public Quaternion Rotation
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of quads associated with the scene object.
        /// </summary>
        public List<QuadData> Quads
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of meshes associated with the scene object.
        /// </summary>
        public List<MeshData> Meshes
        {
            get;
            private set;
        }

        /// <summary>
        /// The surface type of the scene object.
        /// </summary>
        public SpatialAwarenessSurfaceTypes SurfaceType
        {
            get;
            private set;
        }

        /// <summary>
        /// Object encapsulating data of a mesh.
        /// </summary>
        public class MeshData
        {
            /// <summary>
            /// Id of the mesh.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Indices of the mesh.
            /// </summary>
            public int[] Indices { get; set; }

            /// <summary>
            /// Vertices of the mesh.
            /// </summary>
            public Vector3[] Vertices { get; set; }

            /// <summary>
            /// UVs of the mesh.
            /// </summary>
            public Vector2[] UVs { get; set; }

            /// <summary>
            /// The gameObject associated with the mesh.
            /// </summary>
            public GameObject GameObject { get; set; }
        }

        /// <summary>
        /// Object encapsulating data of a quad.
        /// </summary>
        public class QuadData
        {
            /// <summary>
            /// Id of the quad.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Extents of the quad.
            /// </summary>
            public Vector2 Extents { get; set; }

            /// <summary>
            /// The occlusion mask of the quad.
            /// </summary>
            public byte[] OcclusionMask { get; set; }

            /// <summary>
            /// The gameObject associated with the quad.
            /// </summary>
            public GameObject GameObject { get; set; }
        }
    }
}