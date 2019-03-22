// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public partial class SpatialAwarenessPlanarObject : BaseSpatialAwarenessObject
    {
        // todo
        public BoxCollider Collider { get; set;  }

        /// <summary>
        /// constructor
        /// </summary>
        public SpatialAwarenessPlanarObject()
        {
            //empty for now
        }

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessPlanarObject"/>.
        /// </summary>
        /// <param name="size"></param> todo: add comments
        /// <param name="layer"></param>
        /// <param name="name"></param>
        /// <param name="planeId"></param>
        /// <returns>
        /// SpatialAwarenessPlanarObject containing the fields that describe the plane.
        /// </returns>
        public static SpatialAwarenessPlanarObject CreateSpatialObject(Vector3 size, int layer, string name, int planeId)
        {
            SpatialAwarenessPlanarObject newMesh = new SpatialAwarenessPlanarObject();
            
            newMesh.Id = planeId;
            newMesh.GameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newMesh.GameObject.layer = layer;
            newMesh.GameObject.transform.localScale = size;

            newMesh.Filter = newMesh.GameObject.GetComponent<MeshFilter>();
            newMesh.Renderer = newMesh.GameObject.GetComponent<MeshRenderer>();
            newMesh.Collider = newMesh.GameObject.GetComponent<BoxCollider>();

            return newMesh;
        }
    }
}