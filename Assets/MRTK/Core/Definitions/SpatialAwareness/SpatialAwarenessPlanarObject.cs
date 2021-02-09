// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public partial class SpatialAwarenessPlanarObject : BaseSpatialAwarenessObject
    {
        /// <summary>
        /// The BoxCollider associated with this plane's GameObject.
        /// </summary>
        public BoxCollider Collider { get; set; }

        private SpatialAwarenessSurfaceTypes planeType = SpatialAwarenessSurfaceTypes.Unknown;

        /// <summary>
        /// The type of surface (ex: wall) represented by this object. 
        /// </summary>
        public SpatialAwarenessSurfaceTypes SurfaceType
        {
            get => planeType;
            set => planeType = value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        private SpatialAwarenessPlanarObject() : base() { }

        /// <summary>
        /// Creates a <see cref="SpatialAwarenessPlanarObject"/>.
        /// </summary>
        /// <returns>
        /// SpatialAwarenessPlanarObject containing the fields that describe the plane.
        /// </returns>
        public static SpatialAwarenessPlanarObject CreateSpatialObject(
            GameObject planeObject,
            int layer, 
            string name, 
            int planeId,
            SpatialAwarenessSurfaceTypes surfaceType = SpatialAwarenessSurfaceTypes.Unknown)
        {
            SpatialAwarenessPlanarObject newPlane = new SpatialAwarenessPlanarObject();
            newPlane.SurfaceType = surfaceType;

            newPlane.Id = planeId;
            newPlane.GameObject = planeObject;
            newPlane.GameObject.name = name;
            newPlane.GameObject.layer = layer;

            newPlane.Filter = newPlane.GameObject.GetComponent<MeshFilter>();
            newPlane.Renderer = newPlane.GameObject.GetComponent<MeshRenderer>();
            newPlane.Collider = newPlane.GameObject.GetComponent<BoxCollider>();

            return newPlane;
        }
    }
}