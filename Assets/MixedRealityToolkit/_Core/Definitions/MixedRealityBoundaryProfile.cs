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
    [CreateAssetMenu(menuName = "Mixed Reality Toolkit/Mixed Reality Boundary Profile", fileName = "MixedRealityBoundaryProfile", order = 0)]
    public class MixedRealityBoundaryProfile : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The size of the physical space available for Mixed Reality.")]
        private TrackingSpaceType trackingSpaceType = TrackingSpaceType.RoomScale;

        [SerializeField]
        [Tooltip("The height of the playspace, in meters. This is used to create a three dimensional boundary volume.")]
        private float boundaryHeight = 10.0f;

        [SerializeField]
        [Tooltip("Instruct the platform to render the playspace boundary. Not all platforms will support configuring this option.")]
        private bool enablePlatformBoundaryRendering = true;

        [SerializeField]
        [Tooltip("Creates a rectangle within a non-rectangular playspace boundary.")]
        private bool createInscribedRectangle = false;

        public void ApplySettings()
        {
            // todo
        }
    }
}
