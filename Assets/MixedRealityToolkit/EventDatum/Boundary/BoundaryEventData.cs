// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace Microsoft.MixedReality.Toolkit.Boundary
{
    /// <summary>
    /// The data describing the boundary system event.
    /// </summary>
    public class BoundaryEventData : GenericBaseEventData
    {
        /// <summary>
        /// Is the floor being visualized by the boundary system.
        /// </summary>
        public bool IsFloorVisualized { get; private set; }

        /// <summary>
        /// Is the play area being visualized by the boundary system.
        /// </summary>
        public bool IsPlayAreaVisualized { get; private set; }

        /// <summary>
        /// Is the tracked area being visualized by the boundary system.
        /// </summary>
        public bool IsTrackedAreaVisualized { get; private set; }

        /// <summary>
        /// Are the boundary walls being visualized by the boundary system.
        /// </summary>
        public bool AreBoundaryWallsVisualized { get; private set; }

        /// <summary>
        /// Is the ceiling being visualized by the boundary system.
        /// </summary>
        /// <remarks>
        /// The boundary system defines the ceiling as a plane set at <see cref="Microsoft.MixedReality.Toolkit.Boundary.IMixedRealityBoundarySystem.BoundaryHeight"/> above the floor.
        /// </remarks>
        public bool IsCeilingVisualized { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventSystem"></param>
        public BoundaryEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(
            IMixedRealityBoundarySystem boundarySystem, 
            bool isFloorVisualized,
            bool isPlayAreaVisualized,
            bool isTrackedAreaVisualized,
            bool areBoundaryWallsVisualized,
            bool isCeilingVisualized)
        {
            base.BaseInitialize(boundarySystem);
            IsFloorVisualized = isFloorVisualized;
            IsPlayAreaVisualized = isPlayAreaVisualized;
            IsTrackedAreaVisualized = isTrackedAreaVisualized;
            AreBoundaryWallsVisualized = areBoundaryWallsVisualized;
            IsCeilingVisualized = isCeilingVisualized;
        }
    }
}
