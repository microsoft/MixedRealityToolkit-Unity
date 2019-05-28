// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// SpatialLocalizer that is based on a marker detector.
    /// </summary>
    internal class MarkerDetectorSpatialLocalizer : SpatialLocalizerBase
    {
        [Tooltip("The reference to an IMarkerDetector GameObject")]
        [SerializeField]
        private MonoBehaviour MarkerDetector = null;
        private IMarkerDetector markerDetector = null;

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<IMarkerDetector>(MarkerDetector);
        }
#endif

        private void Awake()
        {
            DebugLog("Awake", Guid.Empty);
            markerDetector = MarkerDetector as IMarkerDetector;
            if (markerDetector == null)
            {
                Debug.LogWarning("Marker detector not appropriately set for MarkerDetectorSpatialLocalizer");
            }

            spatialCoordinateService = new MarkerDetectorCoordinateService(markerDetector, debugLogging);
        }

        /// <inheritdoc/>
        protected override async Task<ISpatialCoordinate> GetHostCoordinateAsync(Guid token)
        {
            DebugLog("Getting host coordinate", token);

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                TaskCompletionSource<ISpatialCoordinate> coordinateTCS = new TaskCompletionSource<ISpatialCoordinate>();
                void coordinateDiscovered(ISpatialCoordinate coord)
                {
                    DebugLog("Coordinate found", token);
                    coordinateTCS.SetResult(coord);
                    cts.Cancel();
                }

                SpatialCoordinateService.CoordinatedDiscovered += coordinateDiscovered;
                try
                {
                    DebugLog("Starting to look for coordinates", token);
                    await SpatialCoordinateService.TryDiscoverCoordinatesAsync(cts.Token);
                    DebugLog("Stopped looking for coordinates", token);


                    DebugLog("Awaiting found coordiante", token);
                    // Don't necessarily need to await here
                    return await coordinateTCS.Task;
                }
                finally
                {
                    DebugLog("Unsubscribing from coordinate discovered", token);
                    SpatialCoordinateService.CoordinatedDiscovered -= coordinateDiscovered;
                }
            }
        }
    }
}
