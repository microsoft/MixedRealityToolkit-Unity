// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    public class SpatialAwarenessSceneObject : BaseSpatialAwarenessObject
    {
        public SpatialAwarenessSceneObject(
            int id,
            SpatialAwarenessSurfaceTypes surfaceType,
            Vector3 position,
            Quaternion rotation,
            List<Quad> quads,
            List<MeshData> meshData
            )
        {
            Id = id;
            SurfaceType = surfaceType;

            Position = position;
            Rotation = rotation;

            Quads = quads;
            Meshes = meshData;
        }

        public SpatialAwarenessSceneObject()
        {
        }

        public Vector3 Position
        {
            get;
            private set;
        }

        public Quaternion Rotation
        {
            get;
            private set;
        }

        public List<Quad> Quads
        {
            get;
            private set;
        }

        public List<MeshData> Meshes
        {
            get;
            private set;
        }

        public class MeshData
        {
            public int Id { get; set; }
            public int[] Indices { get; set; }
            public Vector3[] Vertices { get; set; }
            public Vector2[] UVs { get; set; }
            public GameObject GameObject { get; set; }
        }

        public class Quad
        {
            public int Id { get; set; }
            public Vector2 Extents { get; set; }
            public byte[] OcclusionMask { get; set; }
            public GameObject GameObject { get; set; }
        }

        public SpatialAwarenessSurfaceTypes SurfaceType
        {
            get;
            private set;
        }
    }
}