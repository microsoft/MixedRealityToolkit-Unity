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
            System.Guid guid,
            SpatialAwarenessSurfaceTypes surfaceType,
            Vector3 position,
            Quaternion rotation,
            List<Quad> quads,
            List<MeshData> meshDatas
            )
        {
            Guid = guid;
            SurfaceType = surfaceType;

            Position = position;
            Rotation = rotation;

            Quads = quads;
            Meshes = meshDatas;
        }

        public SpatialAwarenessSceneObject()
        {
        }

        public System.Guid Guid
        {
            get;
            private set;
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

        public struct MeshData
        {
            public System.Guid guid;
            public int[] indices;
            public Vector3[] vertices;
            public Vector2[] uvs;
        }

        public struct Quad
        {
            public System.Guid guid;
            public Vector2 extents;
            public byte[] occlusionMask;

            public Quad(System.Guid guid, Vector2 extents, byte[] occlusionMask)
            {
                this.guid = guid;
                this.extents = extents;
                this.occlusionMask = occlusionMask;
            }
        }

        public SpatialAwarenessSurfaceTypes SurfaceType
        {
            get;
            private set;
        }
    }
}