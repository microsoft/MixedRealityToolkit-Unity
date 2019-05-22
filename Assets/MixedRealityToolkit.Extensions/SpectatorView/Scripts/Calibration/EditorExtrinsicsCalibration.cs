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

        [Header("Physical Calibration Board Parameters")]
        /// <summary>
        /// The number of ArUco markers on the calibration board.
        /// </summary>
        [Tooltip("The number of ArUco markers on the calibration board.")]
        [SerializeField]
        protected int expectedNumberOfMarkers = 18;

        [Header("HoloLens Parameters")]
        /// <summary>
        /// Used to setup a network connection.
        /// </summary>
        [Tooltip("Used to setup a network connection.")]
        [SerializeField]
        protected MonoBehaviour MatchMakingService;
        protected IMatchMakingService matchMakingService = null;

        /// <summary>
        /// Used to send/receive data related to the calibration process.
        /// </summary>
        [Tooltip("Used to send/receive data related to the calibration process.")]
        [SerializeField]
        protected MonoBehaviour NetworkingService;
        protected INetworkingService networkingService = null;

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

        private CalculatedCameraIntrinsics dslrIntrinsics;
        private HeadsetCalibrationData headsetData = null;
        private List<CalculatedCameraExtrinsics> cameraExtrinsics;
        private CalculatedCameraExtrinsics globalExtrinsics;
        private List<GameObject> parentVisuals = new List<GameObject>();

#if UNITY_EDITOR
        private void OnValidate()
        {
            FieldHelper.ValidateType<INetworkingService>(NetworkingService);
            FieldHelper.ValidateType<IMatchMakingService>(MatchMakingService);
        }

        private void Start()
        {
            CalibrationDataHelper.Initialize();
            dslrIntrinsics = CalibrationDataHelper.LoadCameraIntrinsics(cameraIntrinsicsPath);
            if (dslrIntrinsics == null)
            {
                throw new Exception("Failed to load the camera intrinsics file.");
            }
            else
            {
                Debug.Log($"Successfully loaded the provided camera intrinsics file: {dslrIntrinsics}");
            }

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

            var arucoDatasetFileNames = CalibrationDataHelper.GetArUcoDatasetFileNames();
            foreach (var fileName in arucoDatasetFileNames)
            {
                var dslrTexture = CalibrationDataHelper.LoadDSLRArUcoImage(fileName);
                var headsetData = CalibrationDataHelper.LoadHeadsetData(fileName);

                if (dslrTexture == null ||
                    headsetData == null)
                {
                    Debug.LogWarning($"Failed to locate dataset: {fileName}");
                }
                else if (!ProcessArUcoData(headsetData, dslrTexture))
                {
                    Debug.LogWarning($"Failed to process dataset: {fileName}");
                }
                else
                {
                    CalibrationDataHelper.SaveDSLRArUcoDetectedImage(dslrTexture, fileName);
                    CreateVisual(headsetData, fileName);
                }
            }
        }

        private void OnHeadsetCalibrationUpdated(byte[] data)
        {
            if(HeadsetCalibrationData.TryDeserialize(data, out var headsetCalibrationData))
            {
                this.headsetData = headsetCalibrationData;
            }
        }

        private void Update()
        {
            if (feedImage != null &&
                feedImage.texture == null)
            {
                feedImage.texture = CompositorWrapper.Instance.GetVideoCameraFeed();
            }

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

            if (headsetData != null)
            {
                if (headsetData.markers.Count != expectedNumberOfMarkers)
                {
                    Debug.Log("Headset has not yet detected all of the markers on the calibration board, dropping payload from headset.");
                }
                else
                {
                    var dslrTexture = CompositorWrapper.Instance.GetVideoCameraTexture();
                    var fileName = CalibrationDataHelper.GetUniqueFileName();
                    CalibrationDataHelper.SaveDSLRArUcoImage(dslrTexture, fileName);
                    CalibrationDataHelper.SaveHeadsetData(headsetData, fileName);

                    if (ProcessArUcoData(headsetData, dslrTexture))
                    {
                        CalibrationDataHelper.SaveDSLRArUcoDetectedImage(dslrTexture, fileName);
                        CreateVisual(headsetData, fileName);
                    }
                }

                headsetData = null;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log("Starting Individual Camera Extrinsics calculations.");
                cameraExtrinsics = CalibrationAPI.Instance.CalculateIndividualArUcoExtrinsics(dslrIntrinsics, parentVisuals.Count);
                if (cameraExtrinsics != null)
                {
                    foreach (var extrinsic in cameraExtrinsics)
                    {
                        Debug.Log($"Calculated extrinsics: {extrinsic}");
                    }
                    CreateExtrinsicsVisual(cameraExtrinsics);
                }

                Debug.Log("Starting the Global Camera Extrinsics calculation.");
                globalExtrinsics = CalibrationAPI.Instance.CalculateGlobalArUcoExtrinsics(dslrIntrinsics);
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
                headsetData = headsetCalibrationData;
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

            if (!CalibrationAPI.Instance.ProcessArUcoData(headsetData, pixels, imageWidth, imageHeight))
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

        private void CreateVisual(HeadsetCalibrationData data, string fileName)
        {
            var parent = new GameObject();
            parent.name = $"Dataset {fileName}";

            for (int i = 0; i < data.markers.Count; i++)
            {
                GameObject temp = null;
                var corners = data.markers[i].arucoMarkerCorners;
                float dist = Vector3.Distance(corners.topLeft, corners.topRight);
                markerVisualHelper.CreateOrUpdateVisual(ref temp, corners.topLeft, corners.orientation, dist * Vector3.one);
                temp.name = $"Marker {fileName}.{data.markers[i].id}";
                temp.transform.parent = parent.transform;
            }

            GameObject camera = null;
            cameraVisualHelper.CreateOrUpdateVisual(ref camera, data.headsetData.position, data.headsetData.rotation);
            camera.name = $"HoloLens {fileName}";
            camera.transform.parent = parent.transform;

            parentVisuals.Add(parent);
        }

        private void CreateExtrinsicsVisual(List<CalculatedCameraExtrinsics> extrinsics)
        {
            if (extrinsics.Count < parentVisuals.Count)
            {
                Debug.LogWarning("Extrinsics count should be at least as large as the parent visuals count, visuals not created");
            }

            for (int i = 0; i < parentVisuals.Count; i++)
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
