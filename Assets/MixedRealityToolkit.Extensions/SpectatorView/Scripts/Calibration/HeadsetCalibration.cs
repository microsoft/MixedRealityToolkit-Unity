using System.Collections.Generic;
using UnityEngine;

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.MarkerDetection;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using Microsoft.MixedReality.Toolkit.Extensions.PhotoCapture;
using System.Text;
using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public delegate void HeadsetCalibrationDataUpdatedHandler(byte[] data);

    public class HeadsetCalibration : MonoBehaviour
    {
        /// <summary>
        /// Check to include PV camera images in the calibration headset data payload.
        /// </summary>
        [Tooltip("Check to include PV camera images in the calibration headset data payload.")]
        [SerializeField]
        protected bool sendPVImages = false;

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

        protected HoloLensCamera holoLensCamera;
        private bool markersUpdated = false;
        private Dictionary<int, Marker> qrCodeMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Matrix4x4> qrCodesRelativeToCamera = new Dictionary<int, Matrix4x4>();
        private Dictionary<int, GameObject> qrCodeDebugVisuals = new Dictionary<int, GameObject>();
        private Dictionary<int, GameObject> arucoDebugVisuals = new Dictionary<int, GameObject>();
        private readonly float markerPaddingRatio = 34f / (300f - (2f * 34f)); // padding pixels / marker width in pixels
        private Dictionary<int, MarkerCorners> qrCodeMarkerCorners = new Dictionary<int, MarkerCorners>();
        private Dictionary<int, MarkerCorners> arucoMarkerCorners = new Dictionary<int, MarkerCorners>();
        private HeadsetCalibrationData headsetCalibrationData;

        public event HeadsetCalibrationDataUpdatedHandler Updated;
        public void UpdateHeadsetCalibrationData()
        {
            Debug.Log("Updating headset calibration data");
            headsetCalibrationData = new HeadsetCalibrationData();
            headsetCalibrationData.timestamp = Time.time;
            headsetCalibrationData.headsetData.position = Camera.main.transform.position;
            headsetCalibrationData.headsetData.rotation = Camera.main.transform.rotation;
            headsetCalibrationData.markers = new List<MarkerPair>();
            headsetCalibrationData.imageData = new PVImageData();

            if (sendPVImages)
            {
                Debug.Log("Taking photo with HoloLens PV Camera");
                holoLensCamera.TakeSingle();
            }
            else
            {
                SendHeadsetCalibrationDataPayload();
            }
        }

        private void Start()
        {
            if (sendPVImages)
            {
                SetupCamera();
            }

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
        }

        private void OnDestroy()
        {
            if (sendPVImages)
            {
                CleanUpCamera();
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

            qrCodesRelativeToCamera.Clear();
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

                    lock (qrCodeMarkerCorners)
                    {
                        qrCodeMarkerCorners[marker.Key] = CalculateMarkerCorners(qrCodePosition, qrCodeRotation, size);
                    }

                    var originToQRCode = Matrix4x4.TRS(qrCodePosition, qrCodeRotation, Vector3.one);
                    var arucoPosition = originToQRCode.MultiplyPoint(new Vector3(-1.0f * ((2.0f * (size * markerPaddingRatio)) + (size)), 0, 0));
                    // Assuming that the aruco marker has the same orientation as qr code marker.
                    // Because both the aruco marker and qr code marker are on the same plane/2d calibration board.
                    var arucoRotation = marker.Value.Rotation;

                    if (showDebugVisuals)
                    {
                        GameObject arucoDebugVisual = null;
                        arucoDebugVisuals.TryGetValue(marker.Key, out arucoDebugVisual);
                        arucoDebugVisualHelper.CreateOrUpdateVisual(ref arucoDebugVisual, arucoPosition, arucoRotation, size * Vector3.one);
                        arucoDebugVisuals[marker.Key] = arucoDebugVisual;
                    }

                    lock (arucoMarkerCorners)
                    {
                        arucoMarkerCorners[marker.Key] = CalculateMarkerCorners(arucoPosition, arucoRotation, size);
                    }
                }
            }

            RemoveItemsAndDestroy(qrCodeDebugVisuals, updatedMarkerIds);
            RemoveItemsAndDestroy(arucoDebugVisuals, updatedMarkerIds);
        }

        private async void SetupCamera()
        {
            Debug.Log("Setting up HoloLensCamera to take grayscale images");
            if (holoLensCamera == null)
                holoLensCamera = new HoloLensCamera(CaptureMode.SingleLowLatency, PixelFormat.BGRA8);

            holoLensCamera.OnCameraInitialized += CameraInitialized;
            holoLensCamera.OnCameraStarted += CameraStarted;
            holoLensCamera.OnFrameCaptured += FrameCaptured;

            await holoLensCamera.Initialize();
        }

        private void CleanUpCamera()
        {
            Debug.Log("Cleaning up HoloLensCamera");
            if (holoLensCamera != null)
            {
                holoLensCamera.Dispose();
                holoLensCamera.OnCameraInitialized -= CameraInitialized;
                holoLensCamera.OnCameraStarted -= CameraStarted;
                holoLensCamera.OnFrameCaptured -= FrameCaptured;
                holoLensCamera = null;
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
            Debug.Log("Image obtained from HoloLens PV Camera");
            if (headsetCalibrationData != null)
            {
                headsetCalibrationData.imageData.pixelFormat = frame.PixelFormat;
                headsetCalibrationData.imageData.resolution = frame.Resolution;
                headsetCalibrationData.imageData.intrinsics = frame.Intrinsics;
                headsetCalibrationData.imageData.extrinsics = frame.Extrinsics;

                byte[] data = new byte[frame.PixelData.Length];
                Buffer.BlockCopy(frame.PixelData, 0, data, 0, frame.PixelData.Length * sizeof(byte));
                headsetCalibrationData.imageData.pixelData = data;

                SendHeadsetCalibrationDataPayload();

                Debug.Log($"Frame obtained with resolution {frame.Resolution.Width} x {frame.Resolution.Height} and size {frame.PixelData.Length}");
            }
#endif
        }

        private void SendHeadsetCalibrationDataPayload()
        {
            byte[] payload = null;
            payload = Encoding.ASCII.GetBytes(JsonUtility.ToJson(headsetCalibrationData));

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

            RemoveItems(dictionary, observedMarkers);
        }

        private static void RemoveItems<TKey, TValue>(Dictionary<TKey, TValue> items, HashSet<TKey> itemsToKeep)
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

        private static void RemoveItemsAndDestroy<TKey>(Dictionary<TKey, GameObject> items, HashSet<TKey> itemsToKeep)
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
            return corners;
        }
    }
}
