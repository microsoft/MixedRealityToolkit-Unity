// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    /// <summary>
    /// Scriptable object that contains reusable configuration values for spatial mesh visualization.
    /// </summary>
    public class GenericSpatialMeshVisualizerConfig : BaseSpatialMeshVisualizerConfig
    {
        [SerializeField]
        [Tooltip("The shape of observation volume")]
        private GenericObserverVolumeType observerVolumeType = GenericObserverVolumeType.AxisAlignedCube;

        /// <summary>
        /// The shape (ex: axis aligned cube) of the observation volume.
        /// </summary>
        public GenericObserverVolumeType ObserverVolumeType => observerVolumeType;
    }
}

