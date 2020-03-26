// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public partial class SpatialAwarenessPlanarObject : BaseSpatialAwarenessObject
    {
        /// <summary>
        /// The BoxCollider associated with this plane's GameObject.
        /// </summary>
        public BoxCollider Collider { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpatialAwarenessPlanarObject() : base() { }

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessPlanarObject"/>.
        /// </summary>
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