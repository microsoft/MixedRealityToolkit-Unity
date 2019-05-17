// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// Localization mechanism based on ArUco marker detection
    /// </summary>
    internal class ArUcoMarkerLocalizationMechanism : BroadcasterCoordinateSpatialLocalizationMechanism
    {
        private ISpatialCoordinateService spatialCoordinateService = null;

        [Tooltip("The reference to Aruco marker detector.")]
        [SerializeField]
        private SpectatorViewPluginArUcoMarkerDetector arucoMarkerDetector = null;

        /// <inheritdoc/>
        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        private void Awake()
        {
            DebugLog("Awake", Guid.Empty);
            spatialCoordinateService = new MarkerDetectorCoordinateService(arucoMarkerDetector, debugLogging);
        }
    }
}
