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
            public int Id;
            public int[] Indices;
            public Vector3[] Vertices;
            public Vector2[] UVs;
            public GameObject GameObject;
        }

        public class Quad
        {
            public int Id;
            public Vector2 Extents;
            public byte[] OcclusionMask;
            public GameObject GameObject;
        }

        public SpatialAwarenessSurfaceTypes SurfaceType
        {
            get;
            private set;
        }
    }
}