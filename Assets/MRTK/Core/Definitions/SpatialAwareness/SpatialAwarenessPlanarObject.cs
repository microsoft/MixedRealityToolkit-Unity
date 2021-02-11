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
            private set => planeType = value;
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
            Vector3 center,
            Vector3 size,
            Quaternion rotation,
            int layer, 
            string name, 
            int planeId,
            SpatialAwarenessSurfaceTypes surfaceType = SpatialAwarenessSurfaceTypes.Unknown)
        {
            SpatialAwarenessPlanarObject newPlane = new SpatialAwarenessPlanarObject();

            newPlane.Id = planeId;
            newPlane.SurfaceType = surfaceType;


            GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            planeObject.transform.position = center;
            planeObject.transform.rotation = rotation;
            planeObject.transform.localScale = size;
            planeObject.name = name;
            planeObject.layer = layer;

            newPlane.GameObject = planeObject;
            newPlane.Filter = newPlane.GameObject.GetComponent<MeshFilter>();
            newPlane.Renderer = newPlane.GameObject.GetComponent<MeshRenderer>();
            newPlane.Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newPlane.Collider = newPlane.GameObject.GetComponent<BoxCollider>();

            return newPlane;
        }
    }
}