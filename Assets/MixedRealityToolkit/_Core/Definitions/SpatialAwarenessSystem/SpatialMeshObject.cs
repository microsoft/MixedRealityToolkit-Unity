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
        /// <param name="renderer"></param>
        /// <param name="filter"></param>
        /// <param name="collider"></param>
        public SpatialMeshObject(int id, GameObject gameObject, MeshRenderer renderer, MeshFilter filter, MeshCollider collider)
        {
            Id = id;
            GameObject = gameObject;
            Renderer = renderer;
            Filter = filter;
            Collider = collider;
        }

        /// <summary>
        /// The id of the spatial mesh object.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// The <see cref="GameObject"/> reference of the Spatial Mesh Object.
        /// </summary>
        public GameObject GameObject { get; }

        /// <summary>
        /// The <see cref="MeshRenderer"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshRenderer Renderer { get; }

        /// <summary>
        /// The <see cref="MeshFilter"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshFilter Filter { get; }

        /// <summary>
        /// The <see cref="MeshCollider"/> reference for the Spatial Mesh Object.
        /// </summary>
        public MeshCollider Collider { get; }
    }
}