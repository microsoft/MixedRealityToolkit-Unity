// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// A marker detection based implementation of <see cref="ISpatialCoordinateService"/>.
    /// </summary>
    public class MarkerDetectorCoordinateService : SpatialCoordinateServiceBase<int>
    {
        private class SpatialCoordinate : SpatialCoordinateUnityBase<int>
        {
            private Marker marker;

            public Marker Marker
            {
                get => marker;
                set
                {
                    if (value != null && value.Id != Id)
                    {
                        throw new InvalidOperationException("Setting a marker with the wrong id.");
                    }

                    if (marker != value)
                    {
                        marker = value;

                        if (marker != null)
                        {
                            SetCoordinateWorldTransform(marker.Position, marker.Rotation);
                        }
                    }
                }
            }

            public override LocatedState State => marker == null ? LocatedState.Resolved : LocatedState.Tracking;

            public SpatialCoordinate(Marker marker)
                : base(marker?.Id ?? throw new ArgumentNullException(nameof(marker)))
            {
                Marker = marker;
            }
        }

        private readonly IMarkerDetector markerDetector;
        private readonly bool debugLogging;

        public MarkerDetectorCoordinateService(IMarkerDetector markerDetector, bool debugLogging)
        {
            this.markerDetector = markerDetector;
            this.markerDetector.MarkersUpdated += OnMarkersUpdated;
            this.debugLogging = debugLogging;
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                UnityEngine.Debug.Log($"MarkerDetector: {message}");
            }
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            DebugLog("Disposed");
            markerDetector.MarkersUpdated -= OnMarkersUpdated;
        }

        private void OnMarkersUpdated(Dictionary<int, Marker> markers)
        {
            DebugLog("Markers Updated");
            if (IsDisposed)
            {
                DebugLog("But we are disposed");
                return;
            }

            HashSet<SpatialCoordinate> newCoordinates = new HashSet<SpatialCoordinate>();
            HashSet<int> seenIds = new HashSet<int>();

            foreach (KeyValuePair<int, Marker> pair in markers)
            {
                DebugLog($"Got Marker Id: {pair.Key}");
                seenIds.Add(pair.Key);
                if (knownCoordinates.TryGetValue(pair.Key, out ISpatialCoordinate spatialCoordinate))
                {
                    DebugLog("We have a coordinate for it, updating marker instance.");
                    ((SpatialCoordinate)spatialCoordinate).Marker = pair.Value;
                }
                else
                {
                    DebugLog("New coordinate");
                    newCoordinates.Add(new SpatialCoordinate(pair.Value));
                }
            }

            DebugLog("Processed all markers");
            foreach (var pair in knownCoordinates)
            {
                if (!seenIds.Remove(pair.Key)) // Shrink as we go
                {
                    DebugLog($"Marker {pair.Key} not seen this time.");
                    ((SpatialCoordinate)pair.Value).Marker = null;
                }
            }

            // TODO main thread and locking above
            foreach (SpatialCoordinate newCoordinate in newCoordinates)
            {
                OnNewCoordinate(newCoordinate.Id, newCoordinate);
            }
        }

        protected override bool TryParse(string id, out int result) => int.TryParse(id, out result);

        protected override async Task OnDiscoverCoordinatesAsync(CancellationToken cancellationToken, int[] idsToLocate = null)
        {
#if UNITY_EDITOR
            if (idsToLocate.Length != 1)
            {
                DebugLog("Running the MarkerDetectorCoordinateService in the editor only supports one coordinate id");
                return;
            }

            var coordinate = new SpatialCoordinate(new Marker(idsToLocate[0], UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity));
            DebugLog("Created artificial coordinate at origin for debugging in the editor");
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).IgnoreCancellation();
            OnNewCoordinate(coordinate.Id, coordinate);
#else
            DebugLog("Starting detection");
            markerDetector.StartDetecting();
            try
            {
                if (idsToLocate != null && idsToLocate.Length > 0)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        bool allFound = true;
                        foreach (int id in idsToLocate)
                        {
                            if (!knownCoordinates.TryGetValue(id, out ISpatialCoordinate coordinate) || coordinate.State == LocatedState.Unresolved)
                            {
                                DebugLog($"Marker with id: {id} wasn't found yet.");
                                allFound = false;
                                break;
                            }
                        }

                        if (allFound)
                        {
                            DebugLog("Found all markers");
                            break;
                        }

                        DebugLog("Waiting another second");
                        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).IgnoreCancellation();
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        DebugLog("We have been told to stop");
                    }
                }
                else
                {
                    DebugLog("Just waiting to be told to stop.");
                    await Task.Delay(-1, cancellationToken).IgnoreCancellation();
                    DebugLog("Told to stop.");
                }
            }
            finally
            {
                markerDetector.StopDetecting();
                DebugLog("Stopped detection");
            }
#endif
        }
    }
}
