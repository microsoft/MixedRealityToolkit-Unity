// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.Events;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Interfaces.SpatialAwarenessSystem
{
    public interface IMixedRealitySpatialAwarenessSystem : IMixedRealityEventSystem
    {
        /// <summary>
        /// Indicates the developer's intended startup behavior.
        /// </summary>
        AutoStartBehavior StartupBehavior { get; set; }

        /// <summary>
        /// Gets or sets the type of volume the observer should operate in.
        /// </summary>
        VolumeType ObserverVolumeType { get; set; }

        /// <summary>
        /// Gets or sets the extents( 1/2 size) of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        /// <remarks>
        /// Only used when <see cref="ObserverVolumeType"/> is set to <see cref="VolumeType.Cubic"/>
        /// </remarks>
        Vector3 ObservationExtents { get; set; }

        /// <summary>
        /// Gets or sets the radius of the spherical volume, in meters, from which individual observations will be made.
        /// </summary>
        /// <remarks>
        /// Only used when <see cref="ObserverVolumeType"/> is set to <see cref="VolumeType.Spherical"/>
        /// </remarks>
        float ObserverRadius { get; set; }

        /// <summary>
        /// Should the observer remain stationary in the scene?
        /// </summary>
        /// <remarks>
        /// Set IsStationaryObserver set to false, to move the volume with the user. 
        /// If set to true, the origin will be 0,0,0 or the last known location.
        /// </remarks>
        bool IsStationaryObserver { get; set; }

        /// <summary>
        /// Gets or sets the origin of the observer.
        /// </summary>
        /// <remarks>
        /// Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.
        /// </remarks>
        Vector3 ObserverOrigin { get; set; }

        // todo Quaternion ObserverOrientation { get; set }

        /// <summary>
        /// Gets or sets the frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// Indicates the current running state of the spatial awareness observer.
        /// </summary>
        bool IsObserverRunning { get; }

        /// <summary>
        /// Starts / restarts the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to resume.</remarks>
        void ResumeObserver();

        /// <summary>
        /// Stops / pauses the spatial observer.
        /// </summary>
        /// <remarks>This will cause spatial awareness events to be suspended until ResumeObserver is called.</remarks>
        void SuspendObserver();

        /// <summary>
        /// //todo returns all given IMixedRealitySpatialAwarenessObservers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void GetObservers<T>();

        /// <summary>
        /// //todo returns first IMixedRealitySpatialAwarenessObserver
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void GetObserver<T>();

        /// <summary>
        /// //todo returns IMixedRealitySpatialAwarenessObserver matching given id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void GetObserver<T>(int id);

        /// <summary>
        /// Generates a new source identifier for an <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.
        /// </summary>
        /// <returns>The source identifier to be used by the <see cref="IMixedRealitySpatialAwarenessObserver"/> implementation.</returns>
        /// <remarks>
        /// This method is to be called by implementations of the <see cref="IMixedRealitySpatialAwarenessObserver"/> interface, and not by application code.
        /// </remarks>
        uint GenerateNewSourceId();
    }
}
