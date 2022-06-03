// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Environment
{
    [AddComponentMenu("Scripts/Microsoft/MRTK/Environment/Generic Spatial Mesh Visualizer")]
    public class GenericSpatialMeshVisualizer : BaseSpatialMeshVisualizer
    {

        [SerializeField]
        [Tooltip("Configuration for the spatial mesh visualizer.")]
        protected GenericSpatialMeshVisualizerConfig config = null;

        /// <summary>
        /// Configuration for the spatial mesh visualizer.
        /// </summary>
        public override BaseSpatialMeshVisualizerConfig Config
        {
            get => config;
            set => config = (GenericSpatialMeshVisualizerConfig)value;
        }

        /// <summary>
        /// The shape (ex: axis aligned cube) of the observation volume.
        /// </summary>
        public GenericObserverVolumeType ObserverVolumeType { get; set; } = GenericObserverVolumeType.AxisAlignedCube;

        /// <summary>
        /// Applies config data from stored config scriptableObject to local variables
        /// </summary>
        protected override void ReadConfig()
        {
            base.ReadConfig();

            if (config == null)
            {
                Debug.LogError($"Spatial mesh visualization requires a configuration profile to run properly.");
                return;
            }

            ObserverVolumeType = config.ObserverVolumeType;
        }

        private static readonly ProfilerMarker ConfigureObserverVolumePerfMarker =
            new ProfilerMarker("[MRTK] GenericSpatialMeshVisualizer.ConfigureObserverVolume");

        private Vector3 oldObserverOrigin = Vector3.zero;
        private Vector3 oldObservationExtents = Vector3.zero;
        private GenericObserverVolumeType oldObserverVolumeType = GenericObserverVolumeType.None;

        /// <summary>
        /// Applies the configured observation extents.
        /// </summary>
        protected override void ConfigureObserverVolume()
        {
            if (meshSubsystem == null
                || (oldObserverOrigin == ObserverOrigin
                && oldObservationExtents == ObservationExtents
                && oldObserverVolumeType == ObserverVolumeType))
            {
                return;
            }

            using (ConfigureObserverVolumePerfMarker.Auto())
            {
                // Update the observer
                switch (ObserverVolumeType)
                {
                    case GenericObserverVolumeType.AxisAlignedCube:
                        meshSubsystem.SetBoundingVolume(ObserverOrigin, ObservationExtents);
                        break;

                    default:
                        Debug.LogError($"Unsupported ObserverVolumeType value {ObserverVolumeType}");
                        break;
                }
            }
        }
    }
}
