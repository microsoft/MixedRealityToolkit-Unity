// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.DataProviders.SpatialObservers
{
    /// <summary>
    /// The interface contract for Mixed Reality spatial observers.
    /// </summary>
    public interface IMixedRealitySpatialAwarenessObserver : IMixedRealityDataProvider, IMixedRealityEventSource
    {
        /// <summary>
        /// Gets or sets the size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        Vector3 ObservationExtents { get; }

        /// <summary>
        /// Should the observer remain stationary in the scene?
        /// </summary>
        /// <remarks>
        /// Set IsStationaryObserver set to false, to move the volume with the user.
        /// If set to true, the origin will be 0,0,0 or the last known location.
        /// </remarks>
        bool IsStationaryObserver { get; }

        /// <summary>
        /// Gets or sets the origin of the observer.
        /// </summary>
        /// <remarks>
        /// Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.
        /// </remarks>
        Vector3 ObserverOrigin { get; }

        /// <summary>
        /// Gets for sets the rotation of the observer
        /// </summary>
        Quaternion ObserverOrientation { get; }

        /// <summary>
        /// Gets or sets the frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// Is the observer running (actively accumulating spatial data)?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Start the observer.
        /// </summary>
        void StartObserving();

        /// <summary>
        /// Stop the observer.
        /// </summary>
        void StopObserving();

        /// <summary>
        /// The collection of mesh <see cref="SpatialMeshObject"/>s that have been observed.
        /// </summary>
        IReadOnlyDictionary<int, SpatialMeshObject> SpatialMeshObjects { get; }

        void RaiseMeshAdded(SpatialMeshObject spatialMeshObject);

        void RaiseMeshUpdated(SpatialMeshObject spatialMeshObject);

        void RaiseMeshRemoved(SpatialMeshObject spatialMeshObject);
    }
}
