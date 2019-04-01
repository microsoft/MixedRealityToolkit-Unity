// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialAwareness
{
    public abstract class BaseSpatialAwarenessObserverProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("How should the observer behave at startup?")]
        private AutoStartBehavior startupBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// Indicates if the observer is to start immediately or wait for manual startup.
        /// </summary>
        public AutoStartBehavior StartupBehavior => startupBehavior;

        [SerializeField]
        [Tooltip("Should the spatial observer remain in a fixed location?")]
        private bool isStationaryObserver = false;

        /// <summary>
        /// Indicates whether or not the spatial observer is to remain in a fixed location.
        /// </summary>
        public bool IsStationaryObserver => isStationaryObserver;

        [SerializeField]
        [Tooltip("The dimensions of the spatial observer volume, in meters.")]
        private Vector3 observationExtents = Vector3.one * 3;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("The shape of observation volume")]
        private VolumeType observerVolumeType = VolumeType.AxisAlignedCube;

        /// <summary>
        /// The shape (ex: axis aligned cube) of the observation volume.
        /// </summary>
        public VolumeType ObserverVolumeType => observerVolumeType;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// The frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval => updateInterval;
    }
}