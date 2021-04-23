// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public interface IMixedRealitySpatialAwarenessObserver : IMixedRealityDataProvider, IMixedRealityEventSource
    {
        /// <summary>
        /// Indicates the developer's intended startup behavior.
        /// </summary>
        AutoStartBehavior StartupBehavior { get; set; }

        /// <summary>
        /// Get or sets the default Unity Physics Layer on which to set the spatial object.
        /// </summary>
        int DefaultPhysicsLayer { get; }

        /// <summary>
        /// Is the observer running (actively accumulating spatial data)?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Should the observer remain stationary in the scene?
        /// </summary>
        /// <remarks>
        /// <para>Set IsStationaryObserver to false to move the volume with the user. 
        /// If set to true, the origin will be 0,0,0 or the last known location.</para>
        /// </remarks>
        bool IsStationaryObserver { get; set; }

        /// <summary>
        /// Gets or sets the type of volume the observer should operate in.
        /// </summary>
        VolumeType ObserverVolumeType { get; set; }

        /// <summary>
        /// Gets or sets the extents (1/2 size) of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        /// <remarks>
        /// <para>When used when <see cref="ObserverVolumeType"/> is set to <see cref="Microsoft.MixedReality.Toolkit.Utilities.VolumeType.Sphere"/>.
        /// The X value of the extents will be used as the radius.</para>
        /// </remarks>
        Vector3 ObservationExtents { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the volume in world space.
        /// </summary>
        /// <remarks>
        /// This is only used when <see cref="ObserverVolumeType"/> is set to <see cref="Microsoft.MixedReality.Toolkit.Utilities.VolumeType.UserAlignedCube"/>
        /// </remarks>
        Quaternion ObserverRotation { get; set; }

        /// <summary>
        /// Gets or sets the origin, in world space, of the observer.
        /// </summary>
        /// <remarks>
        /// <para>Moving the observer origin allows the spatial awareness system to locate and discard meshes as the user
        /// navigates the environment.</para>
        /// </remarks>
        Vector3 ObserverOrigin { get; set; }

        /// <summary>
        /// Gets or sets the frequency, in seconds, at which the spatial observer should update.
        /// </summary>
        float UpdateInterval { get; set; }

        /// <summary>
        /// Start | resume the observer.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stop | pause the observer
        /// </summary>
        void Suspend();

        /// <summary>
        /// Clears the observer's collection of observations.
        /// </summary>
        /// <remarks>
        /// If the observer is currently running, calling ClearObservations will suspend it.
        /// </remarks>
        void ClearObservations();
    }
}