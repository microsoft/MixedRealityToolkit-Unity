// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using NumericsVector3 = System.Numerics.Vector3;
using NumericsQuaternion = System.Numerics.Quaternion;
using UnityEngine.XR.WSA;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.WorldAnchors;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// A marker detection based implementation of <see cref="ISpatialCoordinateService"/>.
    /// </summary>
    public class MarkerDetectorCoordinateService : SpatialCoordinateServiceBase<int>, ISpatialCoordinatePersistenceStore<int>
    {
        private class SpatialCoordinate : TransformSpatialCoordinate<int>
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
                        gameObject.transform.position = marker.Position;
                        gameObject.transform.rotation = marker.Rotation;
                    }
                }
            }

            internal GameObject GameObject => gameObject;

            public override LocatedState State => marker == null ? base.State : LocatedState.Tracking;

            public SpatialCoordinate(ISpatialCoordinatePersistenceStore<int> persistenceStore, int id)
                : base(persistenceStore, id)
            {
            }

            public SpatialCoordinate(ISpatialCoordinatePersistenceStore<int> persistenceStore, Marker marker)
                : this(persistenceStore, marker?.Id ?? throw new ArgumentNullException(nameof(marker)))
            {
                Marker = marker;
            }

            protected override void OnManagedDispose()
            {
                base.OnManagedDispose();

                GameObject.Destroy(gameObject);
            }
        }

        private static readonly string persistedCoordinateSetKey = $"{nameof(MarkerDetectorCoordinateService)}_PersistedCoordinates";
        private readonly HashSet<int> persistedCoordinates = new HashSet<int>();
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
                    newCoordinates.Add(new SpatialCoordinate(this, pair.Value));
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

            var coordinate = new SpatialCoordinate(this, idsToLocate[0]);
            coordinate.Marker = new Marker(coordinate.Id, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity);
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

        private string GetUniquePersistenceIdentifier(int spatialCoordinateId)
        {
            return $"{nameof(MarkerDetectorCoordinateService)}_{spatialCoordinateId}";
        }

        public bool IsPersisted(int spatialCoordinateId)
        {
            return persistedCoordinates.Contains(spatialCoordinateId);
        }

        public void RestorePersistedCoordinates()
        {
            LoadPersistedCoordinateSet();

            foreach (int spatialCoordinateId in persistedCoordinates)
            {
                SpatialCoordinate coordinate = new SpatialCoordinate(this, spatialCoordinateId);
                WorldAnchorManager.Instance.AttachAnchor(coordinate.GameObject, GetUniquePersistenceIdentifier(spatialCoordinateId));
                OnNewCoordinate(spatialCoordinateId, coordinate);
            }
        }

        public void AddPersistedTransform(GameObject gameObject, int spatialCoordinateId)
        {
            persistedCoordinates.Add(spatialCoordinateId);
            string anchorName = GetUniquePersistenceIdentifier(spatialCoordinateId);
            WorldAnchorManager.Instance.AttachAnchor(gameObject, anchorName);

            SavePersistedCoordinateSet();
        }

        public void RemovePersistedTransform(GameObject gameObject, int spatialCoordinateId)
        {
            persistedCoordinates.Remove(spatialCoordinateId);
            WorldAnchorManager.Instance.RemoveAnchor(gameObject);

            SavePersistedCoordinateSet();
        }

        private void SavePersistedCoordinateSet()
        {
            PlayerPrefs.SetString(persistedCoordinateSetKey, string.Join(",", persistedCoordinates.Select(i => i.ToString())));
            PlayerPrefs.Save();
        }

        private void LoadPersistedCoordinateSet()
        {
            persistedCoordinates.Clear();
            string serializedSet = PlayerPrefs.GetString(persistedCoordinateSetKey, string.Empty);
            foreach (string value in serializedSet.Split(','))
            {
                if (int.TryParse(value, out int spatialCoordinateId))
                {
                    persistedCoordinates.Add(spatialCoordinateId);
                }
            }
        }
    }
}
