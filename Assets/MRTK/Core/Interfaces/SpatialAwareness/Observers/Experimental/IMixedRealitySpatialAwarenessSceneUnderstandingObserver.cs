// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    /// <summary>
    /// The interface for defining an <see cref="IMixedRealitySpatialAwarenessObserver"/> which provides scene data.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessSceneUnderstandingObserver : IMixedRealityOnDemandObserver
    {
        /// <summary>
        /// Loads the serialized scene file
        /// </summary>
        void LoadScene(byte[] serializedScene);

        /// <summary>
        /// Save a scene file to the device
        /// </summary>
        void SaveScene(string filename);

        bool TryFindCentermostPlacement(System.Guid quadGuid, Vector2 forSize, out Vector3 bestLocationOnPlane);

        /// <summary>
        /// The set of <see cref="SpatialAwarenessSceneObject"/>s being managed by the observer, keyed by a unique id.
        /// </summary>
        IReadOnlyDictionary<System.Guid, SpatialAwarenessSceneObject> SceneObjects { get; }

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
        /// Use <see cref="PlaneValidationMask"/> for the validation mask on the quad.
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