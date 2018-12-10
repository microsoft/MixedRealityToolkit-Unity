// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Attributes;
using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.DataProviders.SpatialObservers
{
    // No Scriptable Object Menu constructor attributes here, as this class is meant to be inherited.

    /// <summary>
    /// Base Mixed Reality Observer Profile.
    /// </summary>
    public abstract class BaseMixedRealitySpatialObserverProfile : BaseMixedRealityProfile
    {
        [SerializeField]
        [Tooltip("How should the spatial awareness observer behave at startup?")]
        private AutoStartBehavior startupBehavior = AutoStartBehavior.AutoStart;

        /// <summary>
        /// Indicates if the developer intends for the spatial awareness observer start immediately or wait for manual startup.
        /// </summary>
        public AutoStartBehavior StartupBehavior => startupBehavior;

        [SerializeField]
        [Tooltip("The dimensions of the spatial observer volume, in meters.")]
        private Vector3 observationExtents = Vector3.one * 3;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("Should the spatial observer remain in a fixed location?")]
        private bool isStationaryObserver = false;

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public bool IsStationaryObserver => isStationaryObserver;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// The frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval => updateInterval;

        [PhysicsLayer]
        [SerializeField]
        [Tooltip("Which physics layer should this spatial observer use to create it's objects?")]
        private int physicsLayer = -1;

        /// <summary>
        /// The physics layer that the spatial observer will use for it's spatial objects.
        /// </summary>
        public int PhysicsLayer => physicsLayer;
    }
}