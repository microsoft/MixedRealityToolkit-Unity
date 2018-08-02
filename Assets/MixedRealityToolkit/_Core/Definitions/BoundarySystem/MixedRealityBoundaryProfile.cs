// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions.BoundarySystem
{
    /// <summary>
    /// Configuration profile settings for setting up boundary visualizations.
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Boundary Profile", fileName = "MixedRealityEnvironmentBoundaryProfile", order = 6)]
    public class MixedRealityBoundaryProfile : ScriptableObject
    {
        [SerializeField]
        private bool showPlayArea = true;

        /// <summary>
        /// Indicates whether or not the play area is to be displayed.
        /// </summary>
        /// <remarks>
        /// The size of the play area is determined by the boundary system.
        /// </remarks>
        public bool ShowPlayArea => showPlayArea;

        [SerializeField]
        private Material playAreaMaterial = null;

        /// <summary>
        /// The material to use for the rectangular play area <see cref="GameObject"/>.
        /// </summary>
        public Material PlayAreaMaterial => playAreaMaterial;

        [SerializeField]
        private bool showFloorPlane = true;

        /// <summary>
        /// Indicates whether or not the floor plane is to be displayed.
        /// </summary>
        public bool ShowFloorPlane => showFloorPlane;

        [SerializeField]
        private Material floorPlaneMaterial = null;

        /// <summary>
        /// The material to use for the rectangular floor plane <see cref="GameObject"/>.
        /// </summary>
        public Material FloorPlaneMaterial => floorPlaneMaterial;

        [SerializeField]
        private Vector3 floorPlaneScale = new Vector3(3f, 3f, 1f);

        /// <summary>
        /// The the size at which to display the rectangular floor plane <see cref="GameObject"/>.
        /// </summary>
        public Vector3 FloorPlaneScale => floorPlaneScale;
    }
}
