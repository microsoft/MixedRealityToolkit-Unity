// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.SpatialAwareness
{
    /// <summary>
    /// Configuration profile settings for setting up spatial awareness.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Spatial Awareness Profile", fileName = "MixedRealitySpatialAwarenessProfile", order = 7)]
    public class MixedRealitySpatialAwarenessProfile : ScriptableObject
    {
        [SerializeField]
        private bool autoStart = true;

        /// <summary>
        /// Indicates whether or not the spatial awareness system is to be automatically started.
        /// </summary>
        public bool AutoStart => autoStart;

        [SerializeField]
        private Vector3 observationExtents = Vector3.one * 10;

        /// <summary>
        /// The size of the observation volume.
        /// </summary>
        public Vector3 ObservationExtents => observationExtents;

        [SerializeField]
        private int physicsLayer = 31;

        /// <summary>
        /// The physics layer to which identified meshes and surfaces should be attached.
        /// </summary>
        public int PhysicsLayer => physicsLayer;

        [SerializeField]
        private float updateInterval = 3.5f;

        /// <summary>
        /// The interval, in seconds, between observation updates.
        /// </summary>
        public float UpdateInterval => updateInterval;

        #region Mesh settings

        [SerializeField]
        private int trianglesPerCubicMeter = 500;

        /// <summary>
        /// The number of triangles to calculate per cubic meter. 
        /// </summary>
        public int TrianglesPerCubicMeter => trianglesPerCubicMeter;

        [SerializeField]
        private bool recalculateNormals = true;

        /// <summary>
        /// Indicates whether or not normals should be recalculated when observations are updated.
        /// </summary>
        public bool RecalculateNormals => recalculateNormals;

        #endregion Mesh settings

        #region Surface settings

        [SerializeField]
        private float minimumSurfaceArea = 0.025f;

        /// <summary>
        /// The minimum area, in square meters, threshold before a surface plane will be identified.
        /// </summary>
        public float MinimumSurfaceArea => minimumSurfaceArea;

        #endregion Surface settings

    }
}
