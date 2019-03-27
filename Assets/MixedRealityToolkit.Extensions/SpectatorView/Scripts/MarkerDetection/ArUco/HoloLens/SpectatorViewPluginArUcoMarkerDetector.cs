// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.SpectatorView.MarkerDetection
{
    public class SpectatorViewPluginArUcoMarkerDetector : MonoBehaviour,
        IMarkerDetector
    {
        public event MarkersUpdatedHandler MarkersUpdated;

        [SerializeField]
        private float _markerSize = 0.03f;

        private HoloLensCamera _holoLensCamera;
        private SpectatorViewPluginAPI _api;
        private bool _detecting = false;
        private Dictionary<int, List<Marker>> _markerObservations;
        private int _requiredObservations = 5;

        private void Start()
        {
#if UNITY_WSA
            _api = new SpectatorViewPluginAPI();
            if (!_api.Initialize(_markerSize))
            {
                Debug.LogError("Issue loading SpectatorViewPlugin dll");
            }

            _markerObservations = new Dictionary<int, List<Marker>>();
#endif
        }

        private void Update()
        {
            if (_detecting)
            {
                if(_holoLensCamera.State == CameraState.Ready &&
                    !_holoLensCamera.TakeSingle())
                {
                    Debug.LogError("Failed to take photo with HoloLensCamera, Camera State: " + _holoLensCamera.State.ToString());
                }
            }
        }

        public void StartDetecting()
        {
#if UNITY_WSA
            if (!_detecting)
            {
                _detecting = true;
                Debug.Log("Starting ArUco marker detection");
                SetupCamera();
            }
#else
            Debug.LogError("Capturing is not supported on this platform");
#endif
        }

        public void StopDetecting()
        {
            _detecting = false;
#if UNITY_WSA
            Debug.Log("Stopping ArUco marker detection");
            CleanUpCamera();
#else
            Debug.LogError("Capturing is not supported on this platform");
#endif
        }

        public void SetMarkerSize(float size)
        {
            _markerSize = size;
        }

        private async void SetupCamera()
        {
            Debug.Log("Setting up HoloLensCamera");
            if (_holoLensCamera == null)
                _holoLensCamera = new HoloLensCamera(CaptureMode.SingleLowLatency, PixelFormat.BGRA8);

            _holoLensCamera.OnCameraInitialized += CameraInitialized;
            _holoLensCamera.OnCameraStarted += CameraStarted;
            _holoLensCamera.OnFrameCaptured += FrameCaptured;

            await _holoLensCamera.Initialize();
        }

        private void CleanUpCamera()
        {
            Debug.Log("Cleaning up HoloLensCamera");
            if (_holoLensCamera != null)
            {
                _holoLensCamera.Dispose();
                _holoLensCamera.OnCameraInitialized -= CameraInitialized;
                _holoLensCamera.OnCameraStarted -= CameraStarted;
                _holoLensCamera.OnFrameCaptured -= FrameCaptured;
                _holoLensCamera = null;
            }
        }

        private void CameraInitialized(HoloLensCamera sender, bool initializeSuccessful)
        {
            Debug.Log("HoloLensCamera initialized");
            StreamDescription streamDesc = sender.StreamSelector.Select(StreamCompare.EqualTo, 1280, 720).StreamDescriptions[0];
            sender.Start(streamDesc);
        }

        private void CameraStarted(HoloLensCamera sender, bool startSuccessful)
        {
            if (startSuccessful)
            {
                Debug.Log("HoloLensCamera successfully started");
            }
            else
            {
                Debug.LogError("Error: HoloLensCamera failed to start");
            }
        }

        private void FrameCaptured(HoloLensCamera sender, CameraFrame frame)
        {
#if UNITY_WSA
            Debug.Log("Image obtained from HoloLens");
            if (_api != null &&
                _api.IsInitialized)
            {
                var pixelFormat = frame.PixelFormat;
                var imageWidth = frame.Resolution.Width;
                var imageHeight = frame.Resolution.Height;
                var imageData = frame.PixelData;

                var intrinsics = frame.Intrinsics;
                var extrinsics = frame.Extrinsics;

                var dictionary = _api.ProcessImage(imageData, imageWidth, imageHeight, pixelFormat, intrinsics, extrinsics);
                if (_markerObservations != null)
                {
                    foreach (var markerPair in dictionary)
                    {
                        if (!_markerObservations.ContainsKey(markerPair.Key))
                        {
                            _markerObservations.Add(markerPair.Key, new List<Marker>());
                        }

                        _markerObservations[markerPair.Key].Add(markerPair.Value);
                    }

                    var validMarkers = new Dictionary<int, Marker>();
                    foreach (var observationPair in _markerObservations)
                    {
                        if (observationPair.Value.Count >= _requiredObservations)
                        {
                            var averageMarker = CalcAverageMarker(observationPair.Value);
                            validMarkers[averageMarker.Id] = averageMarker;
                            observationPair.Value.Clear();
                        }
                    }

                    MarkersUpdated?.Invoke(validMarkers);
                }
            }
#endif
        }

        private Marker CalcAverageMarker(List<Marker> markers)
        {
            var count = (float)markers.Count;
            var averagePos = Vector3.zero;
            int id = -1;
            List<Quaternion> rotations = new List<Quaternion>();
            foreach (var marker in markers)
            {
                averagePos += marker.Position / count;
                rotations.Add(marker.Rotation);
                id = marker.Id;
            }

            var averageRot = CalcAverageQuaternion(rotations.ToArray());
            return new Marker(id, averagePos, averageRot);
        }

        private Quaternion CalcAverageQuaternion(Quaternion[] quaternions)
        {
            Quaternion mean = quaternions[0];
            var text = "Quaternions: ";
            for (int i = 1; i < quaternions.Length; i++)
            {
                text += "{" + quaternions[i].x + ", " + quaternions[i].y + ", " + quaternions[i].z + ", " + quaternions[i].w + "}";
                float weight = 1.0f / (i + 1);
                mean = Quaternion.Slerp(mean, quaternions[i], weight);
            }
            Debug.Log(text);
            Debug.Log("Mean Quaternion: " + mean.x + ", " + mean.y + ", " + mean.z + ", " + mean.w);
            return mean;
        }
    }
}