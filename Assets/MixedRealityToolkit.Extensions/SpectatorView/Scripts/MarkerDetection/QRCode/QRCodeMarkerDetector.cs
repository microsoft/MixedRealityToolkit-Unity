// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

// The process for distributing QRCodesTrackerPlugin.dll is not yet defined. This code will remain unusable
// to the general public until said distribution story is determiend. However, this file has been added to enable
// public facing development.
// #define QRCODESTRACKER_BINARY_AVAILABLE

#if QRCODESTRACKER_BINARY_AVAILABLE
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.QRCodesTracker;
#endif

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using System.Collections.Generic;
using System;

#if UNITY_WSA && WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
#endif

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection
{
    /// <summary>
    /// QR code detector that implements <see cref="Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection.IVariableSizeMarkerDetector"/>
    /// </summary>
    public class QRCodeMarkerDetector : MonoBehaviour,
        IMarkerDetector
    {
#if WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
        private QRCodesManager _qrCodesManager;
        private Dictionary<Guid, SpatialCoordinateSystem> _markerCoordinateSystems = new Dictionary<Guid, SpatialCoordinateSystem>();
#endif
        private class QRCodeInfo
        {
            public Guid QRCodeId;
            public int MarkerId;
            public float MarkerSize;
            public QRCodeInfo(Guid qrCodeId, int markerId, float markerSize)
            {
                this.QRCodeId = qrCodeId;
                this.MarkerId = markerId;
                this.MarkerSize = markerSize;
            }
        }

        private bool _processMarkers = false;
        private Dictionary<Guid, QRCodeInfo> _cachedMarkers = new Dictionary<Guid, QRCodeInfo>();
        private Dictionary<int, List<Marker>> _markerObservations = new Dictionary<int, List<Marker>>();
        private readonly string _qrCodeNamePrefix = "sv";

#if WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
        private bool _tracking = false;
#endif

#pragma warning disable 67
        /// <inheritdoc />
        public event MarkersUpdatedHandler MarkersUpdated;
#pragma warning restore 67

        /// <inheritdoc />
        public void SetMarkerSize(float markerSize){}

        /// <inheritdoc />
        public void StartDetecting()
        {
#if WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
            _tracking = true;
#else
            Debug.LogError("Current platform does not support qr code marker detector");
#endif
        }

        /// <inheritdoc />
        public void StopDetecting()
        {
#if WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
            _tracking = false;
            _processMarkers = false;
#else
            Debug.LogError("Current platform does not support qr code marker detector");
#endif
        }

        /// <inheritdoc />
        public bool TryGetMarkerSize(int markerId, out float size)
        {
            lock(_cachedMarkers)
            {
                foreach (var marker in _cachedMarkers)
                {
                    if (marker.Value.MarkerId == markerId)
                    {
                        size = marker.Value.MarkerSize;
                        return true;
                    }
                }
            }

            size = 0.0f;
            return false;
        }

#if WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE
        protected void Start()
        {
            if (_qrCodesManager == null)
            {
                _qrCodesManager = QRCodesManager.FindOrCreateQRCodesManager(gameObject);
                _qrCodesManager.QRCodeAdded += QRCodeAdded;
                _qrCodesManager.QRCodeRemoved += QRCodeRemoved;
                _qrCodesManager.QRCodeUpdated += QRCodeUpdated;
            }

            var result = _qrCodesManager.StartQRTracking();
            Debug.Log("Started qr tracker: " + result.ToString());
        }

        protected void Update()
        {
            if (_tracking &&
                _processMarkers)
            {
                ProcessMarkerUpdates();
            }
        }

        private void QRCodeAdded(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            int markerId;
            lock (_cachedMarkers)
            {
                if (!_cachedMarkers.ContainsKey(e.Data.Id) &&
                    TryGetMarkerId(e.Data.Code, out markerId))
                {
                    _cachedMarkers.Add(e.Data.Id, new QRCodeInfo(e.Data.Id, markerId, e.Data.PhysicalSizeMeters));
                    _processMarkers = true;
                }
            }
        }

        private void QRCodeUpdated(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            int markerId;
            lock (_cachedMarkers)
            {
                if (!_cachedMarkers.ContainsKey(e.Data.Id) &&
                    TryGetMarkerId(e.Data.Code, out markerId))
                {
                    _cachedMarkers.Add(e.Data.Id, new QRCodeInfo(e.Data.Id, markerId, e.Data.PhysicalSizeMeters));
                    _processMarkers = true;
                }
            }
        }

        private void QRCodeRemoved(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            lock(_cachedMarkers)
            {
                if (_cachedMarkers.ContainsKey(e.Data.Id))
                {
                    _cachedMarkers.Remove(e.Data.Id);
                    _processMarkers = true;
                    _markerCoordinateSystems.Remove(e.Data.Id);
                }
            }
        }

        private void ProcessMarkerUpdates()
        {
            lock (_cachedMarkers)
            {
                foreach (var markerPair in _cachedMarkers)
                {
                    if (!_markerCoordinateSystems.ContainsKey(markerPair.Key))
                    {
                        var coordinateSystem = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(markerPair.Key);
                        if (coordinateSystem != null)
                        {
                            _markerCoordinateSystems.Add(markerPair.Key, coordinateSystem);
                        }
                    }
                }
            }

            bool locatedAllMarkers = true;
            var markerDictionary = new Dictionary<int, Marker>();
            foreach (var coordinatePair in _markerCoordinateSystems)
            {
                int markerId = -1;
                lock (_cachedMarkers)
                {
                    if (_cachedMarkers.ContainsKey(coordinatePair.Key))
                    {
                        markerId = _cachedMarkers[coordinatePair.Key].MarkerId;
                    }
                }

                Matrix4x4 location;
                if (markerId >= 0 &&
                    _qrCodesManager.TryGetLocationForQRCode(coordinatePair.Key, out location))
                {
                    var translation = location.GetColumn(3);
                    // The obtained QRCode orientation will reflect a positive y axis down the QRCode.
                    // Spectator view marker detectors should return a positive y axis up the marker,
                    // so, we rotate the marker orientation 180 degrees around its z axis.
                    var rotation = Quaternion.LookRotation(location.GetColumn(2), location.GetColumn(1)) * Quaternion.Euler(0, 0, 180);
                    var marker = new Marker(markerId, translation, rotation);
                    markerDictionary.Add(markerId, marker);
                }
                else
                {
                    Debug.Log($"Failed to locate marker:{coordinatePair.Key}, {markerId}");
                    locatedAllMarkers = false;
                }
            }

            if (markerDictionary.Count > 0 || locatedAllMarkers)
            {
                MarkersUpdated?.Invoke(markerDictionary);
            }

            // Stop processing markers once all markers have been located
            _processMarkers = !locatedAllMarkers;
        }
#endif // WINDOWS_UWP && QRCODESTRACKER_BINARY_AVAILABLE

        private bool TryGetMarkerId(string qrCode, out int markerId)
        {
            markerId = -1;
            if (qrCode != null &&
                qrCode.Trim().StartsWith(_qrCodeNamePrefix))
            {
                var qrCodeId = qrCode.Trim().Replace(_qrCodeNamePrefix, "");
                if (Int32.TryParse(qrCodeId, out markerId))
                {
                    return true;
                }
            }

            Debug.Log("Unable to obtain markerId for QR code: " + qrCode);
            markerId = -1;
            return false;
        }
    }
}
