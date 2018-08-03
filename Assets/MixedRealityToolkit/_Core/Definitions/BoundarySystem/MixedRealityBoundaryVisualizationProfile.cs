// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem
{
    /// <summary>
    /// Configuration profile settings for setting up boundary visualizations.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Boundary Visualization Profile", fileName = "MixedRealityBoundaryVisualizationProfile", order = 6)]
    public class MixedRealityBoundaryVisualizationProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The approximate height of the play space, in meters.")]
        private float boundaryHeight = 3.0f;

        /// <summary>
        /// The approximate height of the play space, in meters.
        /// </summary>
        /// <remarks>
        /// The BoundaryHeight property is used to create a three dimensional volume for the play space.
        /// </remarks>
        public float BoundaryHeight => boundaryHeight;

        [SerializeField]
        private Material playAreaMaterial = null;

        /// <summary>
        /// The material to use for the rectangular play area <see cref="GameObject"/>.
        /// </summary>
        public Material PlayAreaMaterial => playAreaMaterial;

        [SerializeField]
        private Material trackedAreaMaterial = null;

        /// <summary>
        /// The material to use for the boundary geometry <see cref="GameObject"/>.
        /// </summary>
        public Material TrackedAreaMaterial => trackedAreaMaterial;

        [SerializeField]
        private Material floorPlaneMaterial = null;

        /// <summary>
        /// The material to use for the rectangular floor plane <see cref="GameObject"/>.
        /// </summary>
        public Material FloorPlaneMaterial => floorPlaneMaterial;

        [SerializeField]
        private Vector3 floorPlaneScale = new Vector3(5f, 5f, 1f);

        /// <summary>
        /// The the size at which to display the rectangular floor plane <see cref="GameObject"/>.
        /// </summary>
        public Vector3 FloorPlaneScale => floorPlaneScale;
    }
}
