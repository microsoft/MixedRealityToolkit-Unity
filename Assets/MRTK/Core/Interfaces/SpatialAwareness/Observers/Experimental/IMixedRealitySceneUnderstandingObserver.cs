// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness
{
    /// <summary>
    /// The interface for defining a spatial awareness observer which provides scene data.
    /// </summary>
    public interface IMixedRealitySceneUnderstandingObserver : IMixedRealityOnDemandObserver
    {
        /// <summary>
        /// Save a scene file to the device
        /// </summary>
        /// <param name="filenamePrefix">Prefix of the name of the saved file</param>
        void SaveScene(string filenamePrefix);

        /// <summary>
        /// Finds best placement position in local space to the quad
        /// </summary>
        /// <param name="quadId">The id of quad that will be used for placement</param>
        /// <param name="objExtents">Total width and height of object to be placed in meters.</param>
        /// <param name="placementPosOnQuad">Base position on plane in local space.</param>
        /// <returns>Returns false if a centermost placement location cannot be found.</returns>
        bool TryFindCentermostPlacement(
            int quadId,
            Vector2 objExtents,
            out Vector3 placementPosOnQuad);

        /// <summary>
        /// The set of SpatialAwarenessSceneObjects being managed by the observer, keyed by a unique id.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessSceneObject> SceneObjects { get; }

        /// <summary>
        /// Surface types to be observed by the observer.
        /// </summary>
        SpatialAwarenessSurfaceTypes SurfaceTypes { get; set; }

        /// <summary>
        /// Number of meshes to generate per frame. Throttled to keep framerate under control.
        /// </summary>
        int InstantiationBatchRate { get; set; }

        /// <summary>
        /// When enabled, generates  data for observed and inferred regions in the scene.
        /// When disabled, generates data only for observed regions in the scene.
        /// </summary>
        bool InferRegions { get; set; }

        /// <summary>
        /// When enabled, the service will provide surface meshes.
        /// </summary>
        bool RequestMeshData { get; set; }

        /// <summary>
        /// When enabled, the service will provide surface planes, represented as a quad.
        /// </summary>
        /// <remarks>
        /// Use PlaneValidationMask for the validation mask on the quad.
        /// </remarks>
        bool RequestPlaneData { get; set; }

        /// <summary>
        /// When enabled, the service will generate texture data for suitable for spatial queries
        /// </summary>
        bool RequestOcclusionMask { get; set; }

        /// <summary>
        /// When enabled, the service will preserve previously observed surfaces when updating.
        /// </summary>
        bool UsePersistentObjects { get; set; }

        /// <summary>
        /// The distance infer surface understanding
        /// </summary>
        float QueryRadius { get; set; }

        /// <summary>
        /// Configures the density of the mesh retrieved from the service
        /// </summary>
        SpatialAwarenessMeshLevelOfDetail WorldMeshLevelOfDetail { get; set; }
    }
}
