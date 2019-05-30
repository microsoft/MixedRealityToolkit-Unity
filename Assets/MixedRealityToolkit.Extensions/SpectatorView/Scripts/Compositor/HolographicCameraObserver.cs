// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Component that connects to the HoloLens application on the holographic camera rig for synchronizing camera poses and receiving calibration data.
    /// </summary>
    public class HolographicCameraObserver : NetworkManager<HolographicCameraObserver>
    {
        public const string CameraCommand = "Camera";
        public const string CalibrationDataCommand = "CalibrationData";

        [SerializeField]
        private CompositionManager compositionManager = null;

        [SerializeField]
        private LocatableDeviceObserver appDeviceObserver = null;

        [SerializeField]
        [Tooltip("The port that the " + nameof(HolographicCamera.HolographicCameraBroadcaster) + " listens for connections on.")]
        private int remotePort = 7502;

        protected override int RemotePort => remotePort;

        private GameObject sharedSpatialCoordinateProxy;

        protected override void Awake()
        {
            base.Awake();

            RegisterCommandHandler(CameraCommand, HandleCameraCommand);
            RegisterCommandHandler(CalibrationDataCommand, HandleCalibrationDataCommand);
        }

        protected override void OnConnected(SocketEndpoint endpoint)
        {
            base.OnConnected(endpoint);

            compositionManager.ResetOnNewCameraConnection();
        }

        private void Update()
        {
            if (appDeviceObserver != null && sharedSpatialCoordinateProxy != null)
            {
                sharedSpatialCoordinateProxy.transform.position = appDeviceObserver.SharedSpatialCoordinateWorldPosition;
                sharedSpatialCoordinateProxy.transform.rotation = appDeviceObserver.SharedSpatialCoordinateWorldRotation;
            }
        }

        public void HandleCameraCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            float timestamp = reader.ReadSingle();
            Vector3 cameraPosition = reader.ReadVector3();
            Quaternion cameraRotation = reader.ReadQuaternion();

            compositionManager.AddCameraPose(cameraPosition, cameraRotation, timestamp);
        }

        public void HandleCalibrationDataCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            int calibrationDataPayloadLength = reader.ReadInt32();
            byte[] calibrationDataPayload = reader.ReadBytes(calibrationDataPayloadLength);

            CalculatedCameraCalibration calibration;
            if (CalculatedCameraCalibration.TryDeserialize(calibrationDataPayload, out calibration))
            {
                if (sharedSpatialCoordinateProxy == null)
                {
                    sharedSpatialCoordinateProxy = new GameObject("App HMD Shared Spatial Coordinate");
                    sharedSpatialCoordinateProxy.transform.SetParent(transform, worldPositionStays: true);
                    if (appDeviceObserver != null)
                    {
                        sharedSpatialCoordinateProxy.transform.position = appDeviceObserver.SharedSpatialCoordinateWorldPosition;
                        sharedSpatialCoordinateProxy.transform.rotation = appDeviceObserver.SharedSpatialCoordinateWorldRotation;
                    }
                }
                compositionManager.EnableHolographicCamera(sharedSpatialCoordinateProxy.transform, new CalibrationData(calibration.Intrinsics, calibration.Extrinsics));
            }
            else
            {
                Debug.LogError("Received a CalibrationData packet from the HoloLens that could not be understood.");
            }
        }
    }
}