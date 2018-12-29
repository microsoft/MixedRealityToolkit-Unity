// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// A Spatial Mesh Object is the Spatial Awareness System's representation of a spatial object with mesh information.
    /// </summary>
    public struct SpatialMeshObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="gameObject"></param>
        public SpatialMeshObject(int id, GameObject gameObject) : this()
        {
            Id = id;
            GameObject = gameObject;
        }

        /// <summary>
        /// The id of the spatial mesh object.
        /// </summary>
        public int Id { get; internal set; }

        private GameObject gameObject;

        /// <summary>
        /// The <see cref="GameObject"/> reference of the Spatial Mesh Object.
        /// </summary>
        public GameObject GameObject
        {
            get { return gameObject; }
            internal set
            {
                gameObject = value;

                Renderer = gameObject.GetComponent<MeshRenderer>();
                Filter = gameObject.GetComponent<MeshFilter>();
                Collider = gameObject.GetComponent<MeshCollider>();
            }
        }

        public Mesh Mesh
        {
            get { return Filter.sharedMesh; }
            internal set
            {
                // Reset the surface mesh collider to fit the updated mesh. 
                // Unity tribal knowledge indicates that to change the mesh assigned to a
                // mesh collider and mesh filter, the mesh must first be set to null.  Presumably there
                // is a side effect in the setter when setting the shared mesh to null.
                Filter.sharedMesh = null;
                Filter.sharedMesh = value;
                Collider.sharedMesh = null;
                Collider.sharedMesh = Filter.sharedMesh;
            }
        }

        /// <summary>
        /// The <see cref="MeshRenderer"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshRenderer Renderer { get; private set; }

        /// <summary>
        /// The <see cref="MeshFilter"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshFilter Filter { get; private set; }

        /// <summary>
        /// The <see cref="MeshCollider"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshCollider Collider { get; private set; }
    }
}