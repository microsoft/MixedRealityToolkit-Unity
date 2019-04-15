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

        private object _contentLock = new object();
        private bool _processMarkers = false;
        private Dictionary<Guid, int> _markerIds = new Dictionary<Guid, int>();
        private Dictionary<int, float> _markerSizes = new Dictionary<int, float>();
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
            lock(_contentLock)
            {
                if (_markerSizes.TryGetValue(markerId, out size))
                {
                    return true;
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
            if (TryGetMarkerId(e.Data.Code, out var markerId))
            {
                lock (_contentLock)
                {
                    _markerIds.Add(e.Data.Id, markerId);
                    _markerSizes.Add(markerId, e.Data.PhysicalSizeMeters);
                    _processMarkers = true;
                }
            }
        }

        private void QRCodeUpdated(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            if (TryGetMarkerId(e.Data.Code, out var markerId))
            {
                lock (_contentLock)
                {
                    _markerIds.Add(e.Data.Id, markerId);
                    _markerSizes.Add(markerId, e.Data.PhysicalSizeMeters);
                    _processMarkers = true;
                }
            }
        }

        private void QRCodeRemoved(object sender, QRCodeEventArgs<QRCodesTrackerPlugin.QRCode> e)
        {
            lock (_contentLock)
            {
                if (_markerIds.TryGetValue(e.Data.Id, out var markerId))
                {
                    _markerSizes.Remove(markerId);
                }

                _markerIds.Remove(e.Data.Id);
                _markerCoordinateSystems.Remove(e.Data.Id);
                _processMarkers = true;
            }
        }

        private void ProcessMarkerUpdates()
        {
            bool locatedAllMarkers = true;
            var markerDictionary = new Dictionary<int, Marker>();
            lock (_contentLock)
            {
                foreach (var markerPair in _markerIds)
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

                foreach (var coordinatePair in _markerCoordinateSystems)
                {
                    if (!_markerIds.TryGetValue(coordinatePair.Key, out var markerId))
                    {
                        Debug.Log($"Failed to locate marker:{coordinatePair.Key}, {markerId}");
                        locatedAllMarkers = false;
                        continue;
                    }

                    if (_qrCodesManager.TryGetLocationForQRCode(coordinatePair.Key, out var location))
                    {
                        var translation = location.GetColumn(3);
                        // The obtained QRCode orientation will reflect a positive y axis down the QRCode.
                        // Spectator view marker detectors should return a positive y axis up the marker,
                        // so, we rotate the marker orientation 180 degrees around its z axis.
                        var rotation = Quaternion.LookRotation(location.GetColumn(2), location.GetColumn(1)) * Quaternion.Euler(0, 0, 180);
                        var marker = new Marker(markerId, translation, rotation);
                        markerDictionary.Add(markerId, marker);
                    }
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
