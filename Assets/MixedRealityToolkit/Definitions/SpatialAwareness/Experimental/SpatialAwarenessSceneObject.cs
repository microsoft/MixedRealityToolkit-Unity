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
            System.Numerics.Matrix4x4 sceneTransformMatrix,
            List<Quad> quads,
            List<MeshData> meshDatas
            )
        {
            Guid = guid;
            SurfaceType = surfaceType;
            SceneTransformMatrix = sceneTransformMatrix;

            Vector3 position;
            Quaternion rotation;
            GetPosRotFromMatrix4x4(SwapRuntimeAndUnityCoordinateSystem(sceneTransformMatrix), out position, out rotation);
            Position = position;
            Rotation = rotation;

            Quads = quads;
            Meshes = meshDatas;
        }

        public System.Guid Guid
        {
            get;
            private set;
        }

        public System.Numerics.Matrix4x4 SceneTransformMatrix
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

        public byte[] OcclusionMaskBytes
        {
            get;
            private set;
        }

        private static System.Numerics.Matrix4x4 SwapRuntimeAndUnityCoordinateSystem(System.Numerics.Matrix4x4 matrix)
        {
            matrix.M13 = -matrix.M13;
            matrix.M23 = -matrix.M23;
            matrix.M43 = -matrix.M43;

            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M34 = -matrix.M34;

            return matrix;
        }

        private static void SetTransformFromMatrix4x4(Transform unityTransform, System.Numerics.Matrix4x4 transformationMatrix, bool updateLocalTransformOnly = false)
        {
            Vector3 t;
            Quaternion r;

            GetPosRotFromMatrix4x4(transformationMatrix, out t, out r);

            if (updateLocalTransformOnly)
            {
                unityTransform.localPosition = t;
                unityTransform.localRotation = r;
            }
            else
            {
                unityTransform.SetPositionAndRotation(t, r);
            }
        }

        private static void GetPosRotFromMatrix4x4(System.Numerics.Matrix4x4 matrix, out Vector3 translation, out Quaternion rotation)
        {
            System.Numerics.Vector3 t;
            System.Numerics.Vector3 s; // ignored but required for signature
            System.Numerics.Quaternion r;

            System.Numerics.Matrix4x4.Decompose(matrix, out s, out r, out t);

            translation = new Vector3(t.X, t.Y, t.Z);
            rotation = new Quaternion(r.X, r.Y, r.Z, r.W);
        }
    }
}