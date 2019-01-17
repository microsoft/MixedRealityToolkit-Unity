// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Devices;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem.Observers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{

    public interface IMixedRealitySpatialAwarenessMeshSystem : IMixedRealitySpatialAwarenessSystem
    {
        #region Mesh Handling

        /// <summary>
        /// Get or sets the desired Unity Physics Layer on which to set the spatial mesh.
        /// </summary>
        int MeshPhysicsLayer { get; set; }

        /// <summary>
        /// Gets the bit mask that corresponds to the value specified in <see cref="MeshPhysicsLayer"/>.
        /// </summary>
        int MeshPhysicsLayerMask { get; }

        //todo remove Mesh Prefix (down)


        /// <summary>
        /// Gets or sets the level of detail, as a MixedRealitySpatialAwarenessMeshLevelOfDetail value, for the returned spatial mesh.
        /// Setting this value to Custom, implies that the developer is specifying a custom value for MeshTrianglesPerCubicMeter. 
        /// </summary>
        /// <remarks>
        /// Specifying any other value will cause <see cref="MeshTrianglesPerCubicMeter"/> to be overwritten.
        /// </remarks>
        SpatialAwarenessMeshLevelOfDetail MeshLevelOfDetail { get; set; }

        /// <summary>
        /// Gets or sets the level of detail, in triangles per cubic meter, for the returned spatial mesh.
        /// </summary>
        /// <remarks>
        /// When specifying Coarse or Fine for the <see cref="MeshLevelOfDetail"/>, this value will be automatically overwritten with system default values.
        /// </remarks>
        int MeshTrianglesPerCubicMeter { get; set; }


        /// <summary>
        /// Gets or sets a value indicating if the spatial awareness system to generate normal for the returned meshes
        /// as some platforms may not support returning normal along with the spatial mesh. 
        /// </summary>
        bool MeshRecalculateNormals { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how the mesh subsystem is to display surface meshes within the application.
        /// </summary>
        /// <remarks>
        /// Applications that wish to process the <see cref="Mesh"/>es should set this value to None.
        /// </remarks>
        SpatialObjectDisplayOptions MeshDisplayOption { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when displaying <see cref="Mesh"/>es.
        /// </summary>
        Material MeshVisibleMaterial { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Material"/> to be used when spatial <see cref="Mesh"/>es should occlude other objects.
        /// </summary>
        Material MeshOcclusionMaterial { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="GameObject"/>s being managed by the spatial awareness mesh subsystem.
        /// </summary>
        IReadOnlyDictionary<int, SpatialAwarenessMeshObject> Meshes { get; }

        #region Mesh Events

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshAdded"/> method to indicate a mesh has been added.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshAdded(IMixedRealitySpatialAwarenessMeshObserver observer, int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been updated.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <param name="meshObject">The mesh <see cref="GameObject"/>.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshUpdated(IMixedRealitySpatialAwarenessMeshObserver observer, int meshId, GameObject meshObject);

        /// <summary>
        /// The spatial awareness system will call the <see cref="IMixedRealitySpatialAwarenessMeshHandler.OnMeshUpdated"/> method to indicate an existing mesh has been removed.
        /// </summary>
        /// <param name="observer">The observer raising the event.</param>
        /// <param name="meshId">Value identifying the mesh.</param>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialObserver"/> interface, and not by application code.
        /// </remarks>
        void RaiseMeshRemoved(IMixedRealitySpatialAwarenessMeshObserver observer, int meshId);

        #endregion Mesh Events

        #endregion Mesh Handling
    }
}