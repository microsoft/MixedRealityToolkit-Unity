// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Definitions
{
    /// <summary>
    /// todo
    /// </summary>
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Boundary Profile", fileName = "MixedRealityBoundaryProfile", order = 4)]
    public class MixedRealityBoundaryProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The size of the physical space available for Mixed Reality.")]
        private TrackingSpaceType trackingSpaceType = TrackingSpaceType.RoomScale;

        /// <summary>
        /// The size of the physical space available for MixedReality.
        /// </summary>
        public TrackingSpaceType SpaceType => trackingSpaceType;

        [SerializeField]
        [Tooltip("The approximate height of the playspace, in meters. This is used to create a three dimensional volume for the playspace.")]
        private float boundaryHeight = 10.0f;

        /// <summary>
        /// The approximate height of the playspace, in meters.
        /// </summary>
        /// <remarks>
        /// This is used to create a three dimensional volume for the playspace.
        /// </remarks>
        public float BoundaryHeight => boundaryHeight;

        [SerializeField]
        [Tooltip("Instruct the platform whether or not to render the playspace boundary. Not all platforms will support configuring this option.")]
        private bool enablePlatformBoundaryRendering = true;

        /// <summary>
        /// Instruct the platform whether or not to render the playspace boundary.
        /// </summary>
        /// <remarks>
        /// Not all platforms will support configuring this option. If a platform does not support configuring 
        /// boundary rendering, this property will have no effect.
        /// </remarks>
        public bool EnablePlatformBoundaryRendering => enablePlatformBoundaryRendering;
    }
}
