using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection;
using System;
using UnityEngine;

namespace Assets.MRTK.MixedRealityToolkit.Extensions.SpectatorView.Scripts.MarkerDetection.ArUco
{
    internal class ArUconMarkerLocalizationMechanism : HostCoordinateLocalizationMechanism
    {
        private ISpatialCoordinateService spatialCoordinateService = null;

        [SerializeField]
        private SpectatorViewPluginArUcoMarkerDetector arucoMarkerDetector = null;

        protected override ISpatialCoordinateService SpatialCoordinateService => spatialCoordinateService;

        private void Awake()
        {
            DebugLog("Awake", Guid.Empty);
            spatialCoordinateService = new MarkerDetectorCoordinateService(arucoMarkerDetector, debugLogging);
        }
    }
}
