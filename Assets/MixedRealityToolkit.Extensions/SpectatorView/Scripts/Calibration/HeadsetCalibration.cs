// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System.Text;
using System;
using System.Collections.Concurrent;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Called when HeadsetCalibration has a new qr code/aruco marker payload
    /// </summary>
    /// <param name="data">byte data to send over the network</param>
    public delegate void HeadsetCalibrationDataUpdatedHandler(byte[] data);

    public class HeadsetCalibration : MonoBehaviour
    {
        /// <summary>
        /// Check to show debug visuals for the detected markers.
        /// </summary>
        [Tooltip("Check to show debug visuals for the detected markers.")]
        [SerializeField]
        protected bool showDebugVisuals = true;

        /// <summary>
        /// QR Code Marker Detector in scene
        /// </summary>
        [Tooltip("QR Code Marker Detector in scene")]
        [SerializeField]
        protected QRCodeMarkerDetector qrCodeMarkerDetector;

        /// <summary>
        /// Debug Visual Helper in scene that will place game objects on qr code markers in the scene.
        /// </summary>
        [Tooltip("Debug Visual Helper in scene that will place game objects on qr code markers in the scene.")]
        [SerializeField]
        protected DebugVisualHelper qrCodeDebugVisualHelper;

        /// <summary>
        /// Debug Visual Helper in scene that will place game objects on aruco markers in the scene.
        /// </summary>
        [Tooltip("Debug Visual Helper in scene that will place game objects on aruco markers in the scene.")]
        [SerializeField]
        protected DebugVisualHelper arucoDebugVisualHelper;

        private bool markersUpdated = false;
        private Dictionary<int, Marker> qrCodeMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, GameObject> qrCodeDebugVisuals = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> arucoDebugVisuals = new Dictionary<int, GameObject>();
        private readonly float markerPaddingRatio = 34f / (300f - (2f * 34f)); // padding pixels / marker width in pixels
        private Dictionary<int, MarkerPair> markerPairs = new Dictionary<int, MarkerPair>();
        private ConcurrentQueue<HeadsetCalibrationData> sendQueue;

        /// <inheritdoc />
        public event HeadsetCalibrationDataUpdatedHandler Updated;

        /// <summary>
        /// Call to signal to the HeadsetCalibration class that it should create a new qr code/aruco marker payload
        /// </summary>
        public void UpdateHeadsetCalibrationData()
        {
            Debug.Log("Updating headset calibration data");
            var data = new HeadsetCalibrationData();
            data.timestamp = Time.time;
            data.headsetData.position = Camera.main.transform.position;
            data.headsetData.rotation = Camera.main.transform.rotation;
            data.markers = new List<MarkerPair>();
            foreach (var qrCodePair in qrCodeMarkers)
            {
                if (markerPairs.ContainsKey(qrCodePair.Key))
                {
                    var markerPair = markerPairs[qrCodePair.Key];
                    data.markers.Add(markerPair);
                }
            }

            sendQueue.Enqueue(data);
        }

        private void Start()
        {
            sendQueue = new ConcurrentQueue<HeadsetCalibrationData>();

            qrCodeMarkerDetector.MarkersUpdated += OnQRCodesMarkersUpdated;
            qrCodeMarkerDetector.StartDetecting();
        }

        private void Update()
        {
            if (markersUpdated)
            {
                markersUpdated = false;
                ProcessQRCodeUpdate();
            }

            while (sendQueue.Count > 0)
            {
                if (sendQueue.TryDequeue(out var data))
                {
                    SendHeadsetCalibrationDataPayload(data);
                }
            }
        }

        private void OnQRCodesMarkersUpdated(Dictionary<int, Marker> markers)
        {
            MergeDictionaries(qrCodeMarkers, markers);
            markersUpdated = true;
        }

        private void ProcessQRCodeUpdate()
        {
            HashSet<int> updatedMarkerIds = new HashSet<int>();

            foreach (var marker in qrCodeMarkers)
            {
                updatedMarkerIds.Add(marker.Key);
                float size = 0;
                if (qrCodeMarkerDetector.TryGetMarkerSize(marker.Key, out size))
                {
                    var qrCodePosition = marker.Value.Position;
                    var qrCodeRotation = marker.Value.Rotation;

                    if (showDebugVisuals)
                    {
                        GameObject qrCodeDebugVisual = null;
                        qrCodeDebugVisuals.TryGetValue(marker.Key, out qrCodeDebugVisual);
                        qrCodeDebugVisualHelper.CreateOrUpdateVisual(ref qrCodeDebugVisual, qrCodePosition, qrCodeRotation, size * Vector3.one);
                        qrCodeDebugVisuals[marker.Key] = qrCodeDebugVisual;
                    }

                    var originToQRCode = Matrix4x4.TRS(qrCodePosition, qrCodeRotation, Vector3.one);
                    var arucoPosition = originToQRCode.MultiplyPoint(new Vector3(-1.0f * ((2.0f * (size * markerPaddingRatio)) + (size)), 0, 0));
                    // We assume that the aruco marker has the same orientation as the qr code marker because they are on the same plane/2d calibration board.
                    var arucoRotation = marker.Value.Rotation;

                    if (showDebugVisuals)
                    {
                        GameObject arucoDebugVisual = null;
                        arucoDebugVisuals.TryGetValue(marker.Key, out arucoDebugVisual);
                        arucoDebugVisualHelper.CreateOrUpdateVisual(ref arucoDebugVisual, arucoPosition, arucoRotation, size * Vector3.one);
                        arucoDebugVisuals[marker.Key] = arucoDebugVisual;
                    }

                    var markerPair = new MarkerPair();
                    markerPair.id = marker.Key;
                    markerPair.qrCodeMarkerCorners = CalculateMarkerCorners(qrCodePosition, qrCodeRotation, size);
                    markerPair.arucoMarkerCorners = CalculateMarkerCorners(arucoPosition, arucoRotation, size);

                    lock (markerPairs)
                    {
                        markerPairs[marker.Key] = markerPair;
                    }
                }
            }

            RemoveUnobservedItemsAndDestroy(qrCodeDebugVisuals, updatedMarkerIds);
            RemoveUnobservedItemsAndDestroy(arucoDebugVisuals, updatedMarkerIds);
        }

        private void SendHeadsetCalibrationDataPayload(HeadsetCalibrationData data)
        {
            byte[] payload = data.Serialize();
            Updated?.Invoke(payload);
        }

        private static void MergeDictionaries(Dictionary<int, Marker> dictionary, Dictionary<int, Marker> update)
        {
            HashSet<int> observedMarkers = new HashSet<int>();
            foreach(var markerUpdate in update)
            {
                dictionary[markerUpdate.Key] = markerUpdate.Value;
                observedMarkers.Add(markerUpdate.Key);
            }

            RemoveUnobservedItems(dictionary, observedMarkers);
        }

        private static void RemoveUnobservedItems<TKey, TValue>(Dictionary<TKey, TValue> items, HashSet<TKey> itemsToKeep)
        {
            List<TKey> keysToRemove = new List<TKey>();
            foreach (var pair in items)
            {
                if (!itemsToKeep.Contains(pair.Key))
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                items.Remove(key);
            }
        }

        private static void RemoveUnobservedItemsAndDestroy<TKey>(Dictionary<TKey, GameObject> items, HashSet<TKey> itemsToKeep)
        {
            List<TKey> keysToRemove = new List<TKey>();
            foreach (var pair in items)
            {
                if (!itemsToKeep.Contains(pair.Key))
                {
                    keysToRemove.Add(pair.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                Debug.Log($"Destroying debug visual for marker id:{key}");
                var visual = items[key];
                items.Remove(key);
                Destroy(visual);
            }
        }

        private static Vector4 GetPosition(Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        private static Quaternion GetRotation(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        private static MarkerCorners CalculateMarkerCorners(Vector3 topLeftPosition, Quaternion topLeftOrientation, float size)
        {
            var corners = new MarkerCorners();
            corners.topLeft = topLeftPosition;
            var originToTopLeftCorner = Matrix4x4.TRS(topLeftPosition, topLeftOrientation, Vector3.one);
            corners.topRight = originToTopLeftCorner.MultiplyPoint(new Vector3(-size, 0, 0));
            corners.bottomLeft = originToTopLeftCorner.MultiplyPoint(new Vector3(0, -size, 0));
            corners.bottomRight = originToTopLeftCorner.MultiplyPoint(new Vector3(-size, -size, 0));
            corners.orientation = topLeftOrientation;
            return corners;
        }
    }
}
