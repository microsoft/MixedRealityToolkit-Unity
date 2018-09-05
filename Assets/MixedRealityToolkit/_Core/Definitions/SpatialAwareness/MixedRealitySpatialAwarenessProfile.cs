// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Definitions.SpatialAwarenessSystem
{
    /// <summary>
    /// Configuration profile settings for setting up the spatial awareness system.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Spatial Awareness Profile", fileName = "MixedRealitySpatialAwarenessProfile", order = (int)CreateProfileMenuItemIndices.SpatialAwareness)]
    public class MixedRealitySpatialAwarenessProfile : ScriptableObject
    {
        #region General settings

        [SerializeField]
        [Tooltip("Should the spatial awareness observer be suspended at startup?")]
        private bool startObserverSuspended = true;

        /// <summary>
        /// Indicates if the developer intends for the spatial awareness observer to not return data until explicitly resumed.
        /// </summary>
        public bool StartObserverSuspended => startObserverSuspended;

        [SerializeField]
        [Tooltip("The dimensions of the spatial observer volume, in meters.")]
        private Vector3 observationExtents = new Vector3(10f, 10f, 10f);

        /// <summary>
        /// The size of the volume, in meters per axis, from which individual observations will be made.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        [Tooltip("How often, in seconds, should the spatial observer update?")]
        private float updateInterval = 3.5f;

        /// <summary>
        /// Rhe frequency, in seconds, at which the spatial observer updates.
        /// </summary>
        public float UpdateInterval => updateInterval;

        #endregion General settings

        #region Mesh settings
        // todo
        #endregion Mesh settings

        #region Surface Finding settings
        // todo
        #endregion Surface Finding settings
    }
}
