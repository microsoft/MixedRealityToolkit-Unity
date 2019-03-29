// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.MRTK.MixedRealityToolkit.Extensions.SpectatorView.Scripts.MarkerDetection
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
                        IsLocated = marker != null;
                    }
                }
            }

            public new bool IsLocated
            {
                get => base.IsLocated;
                set => base.IsLocated = value;
            }

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
            markerDetector.MarkersUpdated -= OnMarkersUpdated;
            IsTracking = false;
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

        /// <summary>
        /// Returns an existingly tracked <see cref="ISpatialCoordinate"/> or a new one that will be used when the marker with the id is seen.
        /// </summary>
        public override Task<ISpatialCoordinate> TryGetCoordinateByIdAsync(string markerId, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            ISpatialCoordinate toReturn = null;
            if (int.TryParse(markerId, out int id))
            {
                if (!knownCoordinates.TryGetValue(id, out toReturn))
                {
                    SpatialCoordinate spatialCoordinate = new SpatialCoordinate(id);
                    OnNewCoordinate(id, spatialCoordinate);
                    toReturn = spatialCoordinate;
                }
            }

            return Task.FromResult(toReturn);
        }

        protected override async Task RunTrackingAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            markerDetector.StartDetecting();

            await Task.Delay(-1, cancellationToken).IgnoreCancellation();

            markerDetector.StopDetecting();
        }
    }
}
