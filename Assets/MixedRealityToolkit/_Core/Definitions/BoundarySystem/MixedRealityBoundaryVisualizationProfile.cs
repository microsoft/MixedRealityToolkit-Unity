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
        #region Floor settings

        [SerializeField]
        private bool showFloor = true;

        /// <summary>
        /// Should the boundary system display the floor?
        /// </summary>
        public bool ShowFloor => showFloor;

        // todo: consider allowing optional custom prefab

        [SerializeField]
        private Material floorMaterial = null;

        /// <summary>
        /// The material to use for the floor <see cref="GameObject"/> when created by the boundary system.
        /// </summary>
        public Material FloorMaterial => floorMaterial;

        [SerializeField]
        private Vector2 floorScale = new Vector2(10f, 10f);

        /// <summary>
        /// The size at which to display the rectangular floor plane <see cref="GameObject"/>.
        /// </summary>
        public Vector3 FloorScale => floorScale;

        #endregion Floor settings

        #region Play area settings

        [SerializeField]
        private bool showPlayArea = true;

        /// <summary>
        /// Should the boundary system display the play area?
        /// </summary>
        public bool ShowPlayArea => showPlayArea;

        // todo: consider allowing outline vs polygon

        [SerializeField]
        private Material playAreaMaterial = null;

        /// <summary>
        /// The material to use for the rectangular play area <see cref="GameObject"/>.
        /// </summary>
        public Material PlayAreaMaterial => playAreaMaterial;

        #endregion Play area settings

        #region Tracked area settings

        [SerializeField]
        private bool showTrackedArea = true;

        /// <summary>
        /// Should the boundary system display the tracked area?
        /// </summary>
        public bool ShowTrackedArea => showTrackedArea;

        // todo: consider allowing polygon vs outline

        [SerializeField]
        private Material trackedAreaMaterial = null;

        /// <summary>
        /// The material to use for the boundary geometry <see cref="GameObject"/>.
        /// </summary>
        public Material TrackedAreaMaterial => trackedAreaMaterial;

        #endregion Tracked area settings

        #region Boundary wall settings

        [SerializeField]
        private bool showBoundaryWalls = true;

        /// <summary>
        /// Should the boundary system display the boundary geometry walls?
        /// </summary>
        public bool ShowBoundaryWalls => showBoundaryWalls;

        [SerializeField]
        private Material boundaryWallMaterial = null;

        /// <summary>
        /// The material to use for displaying the boundary geometry walls.
        /// </summary>
        public Material BoundaryWallMaterial => boundaryWallMaterial;

        #endregion Boundary wall settings

        [SerializeField]
        private bool showBoundaryCeiling = true;

        /// <summary>
        /// Should the boundary system display the boundary ceiling?
        /// </summary>
        public bool ShowBoundaryCeiling => showBoundaryCeiling;

        [SerializeField]
        private Material boundaryCeilingMaterial = null;

        /// <summary>
        /// The material to use for displaying the boundary ceiling.
        /// </summary>
        public Material BoundaryCeilingMaterial => boundaryCeilingMaterial;

        #region Boundary ceiling settings
        #endregion Boundary ceiling settings
    }
}
