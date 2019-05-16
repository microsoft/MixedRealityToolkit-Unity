// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection
{
    /// <summary>
    /// A marker detection based implementation of <see cref="ISpatialCoordinateService"/>.
    /// </summary>
    public class MarkerDetectorCoordinateService : SpatialCoordinateServiceBase<int>
    {
        private class SpatialCoordinate : SpatialCoordinateBase<int>
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
                    }
                }
            }

            public override LocatedState State => marker == null ? LocatedState.Resolved : LocatedState.Tracking;

            public SpatialCoordinate(int id)
                : base(id) { }

            public SpatialCoordinate(Marker marker)
                : base(marker?.Id ?? throw new ArgumentNullException(nameof(marker)))
            {
                Marker = marker;
            }

            public override Vector3 CoordinateToWorldSpace(Vector3 vector) => vector - marker.Position.AsNumericsVector();

            public override Quaternion CoordinateToWorldSpace(Quaternion quaternion) => UnityEngine.Quaternion.Inverse(marker.Rotation).AsNumericsQuaternion() * quaternion;

            public override Vector3 WorldToCoordinateSpace(Vector3 vector) => vector + marker.Position.AsNumericsVector();

            public override Quaternion WorldToCoordinateSpace(Quaternion quaternion) => marker.Rotation.AsNumericsQuaternion() * quaternion;
        }

        private readonly IMarkerDetector markerDetector;

        public MarkerDetectorCoordinateService(IMarkerDetector markerDetector)
        {
            this.markerDetector = markerDetector;
            this.markerDetector.MarkersUpdated += OnMarkersUpdated;
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            markerDetector.MarkersUpdated -= OnMarkersUpdated;
        }

        private void OnMarkersUpdated(Dictionary<int, Marker> markers)
        {
            if (IsDisposed)
            {
                return;
            }

            HashSet<SpatialCoordinate> newCoordinates = new HashSet<SpatialCoordinate>();
            HashSet<int> seenIds = new HashSet<int>();

            foreach (KeyValuePair<int, Marker> pair in markers)
            {
                seenIds.Add(pair.Key);
                if (knownCoordinates.TryGetValue(pair.Key, out ISpatialCoordinate spatialCoordinate))
                {
                    ((SpatialCoordinate)spatialCoordinate).Marker = pair.Value;
                }
                else
                {
                    newCoordinates.Add(new SpatialCoordinate(pair.Value));
                }
            }

            foreach (var pair in knownCoordinates)
            {
                if (!seenIds.Remove(pair.Key)) // Shrink as we go
                {
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
            markerDetector.StartDetecting();

            if (idsToLocate != null)
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool allFound = true;
                    foreach (int id in idsToLocate)
                    {
                        if (!knownCoordinates.TryGetValue(id, out ISpatialCoordinate coordinate) || coordinate.State == LocatedState.Unresolved)
                        {
                            allFound = false;
                            break;
                        }
                    }

                    if (allFound)
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).IgnoreCancellation();
                }
            }
            else
            {
                await Task.Delay(-1, cancellationToken).IgnoreCancellation();
            }

            markerDetector.StopDetecting();
        }
    }
}
