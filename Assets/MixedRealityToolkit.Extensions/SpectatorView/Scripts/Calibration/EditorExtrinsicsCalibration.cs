// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class EditorExtrinsicsCalibration : MonoBehaviour
    {
        [Header("Camera Intrinsics")]
        /// <summary>
        /// Path to a camera intrinsics file created using chessboard intrinsics calibration.
        /// </summary>
        [Tooltip("Path to a camera intrinsics file created using chessboard intrinsics calibration.")]
        [SerializeField]
        protected string cameraIntrinsicsPath = "";

        [Header("HoloLens Parameters")]
        /// <summary>
        /// Time between headset calibration data requests (in seconds).
        /// </summary>
        [Tooltip("Time between headset calibration data requests (in seconds).")]
        [SerializeField]
        private float timeBetweenRequests = 5.0f;

        /// <summary>
        /// Used to setup a network connection.
        /// </summary>
        [Tooltip("Used to setup a network connection.")]
        [SerializeField]
        private MonoBehaviour MatchMakingService;
        private IMatchMakingService matchMakingService = null;

        /// <summary>
        /// Used to send/receive data related to the calibration process.
        /// </summary>
        [Tooltip("Used to send/receive data related to the calibration process.")]
        [SerializeField]
        private MonoBehaviour NetworkingService;
        private INetworkingService networkingService = null;

        [Header("VR Headset Parameters")]
        /// <summary>
        /// Used to obtain headset information for a vr headset.
        /// </summary>
        [Tooltip("Used to obtain headset information for a vr headset.")]
        [SerializeField]
        protected HeadsetCalibration headsetCalibration = null;

        [Header("UI Parameters")]
        /// <summary>
        /// Image for displaying the dslr camera feed.
        /// </summary>
        [Tooltip("Image for displaying the dslr camera feed.")]
        [SerializeField]
        protected RawImage feedImage;

        /// <summary>
        /// Image for displaying the last processed ArUco marker dataset.
        /// </summary>
        [Tooltip(" Image for displaying the last processed ArUco marker dataset.")]
        [SerializeField]
        protected RawImage lastArUcoImage;

        /// <summary>
        /// Used to draw debug visuals for detected aruco markers.
        /// </summary>
        [Tooltip("Used to draw debug visuals for detected aruco markers.")]
        [SerializeField]
        protected DebugVisualHelper markerVisualHelper;

        /// <summary>
        /// Used to draw debug visuals for camera positions/orientations.
        /// </summary>
        [Tooltip("Used to draw debug visuals for camera positions/orientations.")]
        [SerializeField]
        protected DebugVisualHelper cameraVisualHelper;

        private CalibrationAPI calibration = null;
        private CalculatedCameraIntrinsics dslrIntrinsics;
        private Queue<HeadsetCalibrationData> dataQueue = new Queue<HeadsetCalibrationData>();
        private int nextArUcoImageId = 0;
        private List<CalculatedCameraExtrinsics> cameraExtrinsics;
        private CalculatedCameraExtrinsics globalExtrinsics;
        private List<GameObject> parentVisuals = new List<GameObject>();
        private int expectedMarkers = 18;

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<INetworkingService>(NetworkingService);
            FieldHelper.ValidateType<IMatchMakingService>(MatchMakingService);
        }

        private void Start()
        {
            CalibrationDataHelper.Initialize(out var nextChessboardImageId, out nextArUcoImageId);
            dslrIntrinsics = CalibrationDataHelper.LoadCameraIntrinsics(cameraIntrinsicsPath);
            if (dslrIntrinsics == null)
            {
                throw new Exception("Failed to load the camera intrinsics file.");
            }
            else
            {
                Debug.Log($"Successfully loaded the provided camera intrinsics file: {dslrIntrinsics}");
            }

            calibration = new CalibrationAPI();

            networkingService = NetworkingService as INetworkingService;
            if (networkingService != null)
            {
                networkingService.DataReceived += OnDataReceived;
            }

            matchMakingService = MatchMakingService as IMatchMakingService;
            if (matchMakingService != null)
            {
                matchMakingService.Connect();
            }

            if (headsetCalibration != null)
            {
                headsetCalibration.Updated += OnHeadsetCalibrationUpdated;
            }

            for(int i = 0; i < nextArUcoImageId; i++)
            {
                var dslrTexture = CalibrationDataHelper.LoadDSLRArUcoImage(i);
                var headsetData = CalibrationDataHelper.LoadHeadsetData(i);

                if (dslrTexture == null ||
                    headsetData == null ||
                    !ProcessArUcoData(headsetData, dslrTexture))
                {
                    Debug.LogWarning($"Failed to process dataset: {i}");
                }
                else
                {
                    CalibrationDataHelper.SaveDSLRArUcoDetectedImage(dslrTexture, i);
                    CreateVisual(headsetData, i);
                }
            }
        }

        private void OnHeadsetCalibrationUpdated(byte[] data)
        {
            if(HeadsetCalibrationData.TryDeserialize(data, out var headsetData))
            {
                dataQueue.Enqueue(headsetData);
            }
        }

        private void Update()
        {
            if (feedImage != null &&
                feedImage.texture == null)
                feedImage.texture = CompositorWrapper.GetDSLRFeed();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (networkingService != null &&
                    matchMakingService != null)
                {
                    var request = new HeadsetCalibrationDataRequest();
                    request.timestamp = Time.time;
                    var payload = request.Serialize();

                    if (networkingService.SendData(payload, NetworkPriority.Critical))
                    {
                        Debug.Log($"Sent headset calibration data request to HoloLens at {request.timestamp}");
                    }
                    else
                    {
                        Debug.LogWarning("Failed to send headset calibration data request to HoloLens");
                    }
                }

                if (headsetCalibration != null)
                {
                    Debug.Log("Requesting headset calibration data from VR Headset");
                    headsetCalibration.UpdateHeadsetCalibrationData();
                }
            }

            while (dataQueue.Count > 0)
            {
                var data = dataQueue.Dequeue();

                if (dataQueue.Count == 0)
                {
                    if (data.markers.Count != expectedMarkers)
                    {
                        Debug.Log("Headset has not yet detected all of the markers on the calibration board, dropping payload from headset.");
                        continue;
                    }

                    var dslrTexture = CompositorWrapper.GetDSLRTexture();
                    CalibrationDataHelper.SaveDSLRArUcoImage(dslrTexture, nextArUcoImageId);
                    CalibrationDataHelper.SaveHeadsetData(data, nextArUcoImageId);

                    if (ProcessArUcoData(data, dslrTexture))
                    {
                        CalibrationDataHelper.SaveDSLRArUcoDetectedImage(dslrTexture, nextArUcoImageId);
                        CreateVisual(data, nextArUcoImageId);
                    }

                    nextArUcoImageId++;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Starting Individual Camera Extrinsics calculations.");
                cameraExtrinsics = calibration.CalculateIndividualArUcoExtrinsics(dslrIntrinsics, parentVisuals.Count);
                if (cameraExtrinsics != null)
                {
                    foreach (var extrinsic in cameraExtrinsics)
                    {
                        Debug.Log($"Calculated extrinsics: {extrinsic}");
                    }
                    CreateExtrinsicsVisual(cameraExtrinsics);
                }

                Debug.Log("Starting the Global Camera Extrinsics calculation.");
                globalExtrinsics = calibration.CalculateGlobalArUcoExtrinsics(dslrIntrinsics);
                if (globalExtrinsics != null)
                {
                    var fileName = CalibrationDataHelper.SaveCameraExtrinsics(globalExtrinsics);
                    Debug.Log($"Saved global extrinsics: {fileName}");
                    Debug.Log($"Found global extrinsics: {globalExtrinsics}");
                    var position = globalExtrinsics.ViewFromWorld.GetColumn(3);
                    var rotation = Quaternion.LookRotation(globalExtrinsics.ViewFromWorld.GetColumn(2), globalExtrinsics.ViewFromWorld.GetColumn(1));
                    GameObject camera = null;
                    cameraVisualHelper.CreateOrUpdateVisual(ref camera, position, rotation);
                    camera.name = "Global Extrinsics";
                    GameObject hololens = null;
                    cameraVisualHelper.CreateOrUpdateVisual(ref hololens, Vector3.zero, Quaternion.identity);
                    hololens.name = "Global HoloLens";
                }
            }
        }

        private void OnDataReceived(string playerId, byte[] payload)
        {
            Debug.Log($"Received payload of {payload.Length} bytes");
            HeadsetCalibrationData headsetCalibrationData;
            if (HeadsetCalibrationData.TryDeserialize(payload, out headsetCalibrationData))
            {
                dataQueue.Enqueue(headsetCalibrationData);
            }
        }

        private bool ProcessArUcoData(HeadsetCalibrationData headsetData, Texture2D dslrTexture)
        {
            if (dslrTexture == null ||
                dslrTexture.format != TextureFormat.RGB24)
            {
                return false;
            }

            int imageWidth = dslrTexture.width;
            int imageHeight = dslrTexture.height;
            var unityPixels = dslrTexture.GetRawTextureData<byte>();
            var pixels = unityPixels.ToArray();

            if (!calibration.ProcessArUcoData(headsetData, pixels, imageWidth, imageHeight))
            {
                return false;
            }

            for (int i = 0; i < unityPixels.Length; i++)
            {
                unityPixels[i] = pixels[i];
            }

            dslrTexture.Apply();

            if (lastArUcoImage)
                lastArUcoImage.texture = dslrTexture;

            return true;
        }

        private void CreateVisual(HeadsetCalibrationData data, int index)
        {
            var parent = new GameObject();
            parent.name = $"Dataset {index}";

            for (int i = 0; i < data.markers.Count; i++)
            {
                GameObject temp = null;
                var corners = data.markers[i].arucoMarkerCorners;
                float dist = Vector3.Distance(corners.topLeft, corners.topRight);
                markerVisualHelper.CreateOrUpdateVisual(ref temp, corners.topLeft, corners.orientation, dist * Vector3.one);
                temp.name = $"Marker {index}.{data.markers[i].id}";
                temp.transform.parent = parent.transform;
            }

            GameObject camera = null;
            cameraVisualHelper.CreateOrUpdateVisual(ref camera, data.headsetData.position, data.headsetData.rotation);
            camera.name = $"HoloLens {index}";
            camera.transform.parent = parent.transform;

            parentVisuals.Add(parent);
        }

        private void CreateExtrinsicsVisual(List<CalculatedCameraExtrinsics> extrinsics)
        {
            if (extrinsics.Count < parentVisuals.Count)
            {
                Debug.LogWarning("Extrinsics count should be at least as large as the parent visuals count, visuals not created");
            }

            for(int i = 0; i < parentVisuals.Count; i++)
            {
                var parent = parentVisuals[i];
                GameObject camera = null;
                var extrinsic = extrinsics[i];
                var position = extrinsic.ViewFromWorld.GetColumn(3);
                var rotation = Quaternion.LookRotation(extrinsic.ViewFromWorld.GetColumn(2), extrinsic.ViewFromWorld.GetColumn(1));
                cameraVisualHelper.CreateOrUpdateVisual(ref camera, position, rotation);
                camera.name = "Calculated DSLR";
                camera.transform.parent = parent.transform;
            }
        }
#endif
    }
}
