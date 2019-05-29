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
    public class HolographicCameraNetworkManager : NetworkManager<HolographicCameraNetworkManager>, ICommandHandler
    {
        public const string CameraCommand = "Camera";
        public const string CalibrationDataCommand = "CalibrationData";

        [SerializeField]
        private CompositionManager compositionManager = null;

        [SerializeField]
        [Tooltip("The port that the " + nameof(HolographicCamera.TCPNetworkListener) + " listens for connections on.")]
        private int remotePort = 7502;

        protected override int RemotePort => remotePort;

        protected override void Awake()
        {
            base.Awake();

            RegisterCommandHandler(CameraCommand, this);
            RegisterCommandHandler(CalibrationDataCommand, this);
        }

        protected override void OnConnected(SocketEndpoint endpoint)
        {
            base.OnConnected(endpoint);

            compositionManager.ResetOnNewCameraConnection();
        }

        public void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            switch (command)
            {
                case CameraCommand:
                    {
                        ILocatableDevice device = GetComponent<ILocatableDevice>();
                        if (device != null)
                        {
                            device.NotifyTrackingUpdated();
                        }

                        float timestamp = reader.ReadSingle();
                        Vector3 cameraPosition = reader.ReadVector3();
                        Quaternion cameraRotation = reader.ReadQuaternion();

                        compositionManager.AddCameraPose(cameraPosition, cameraRotation, timestamp);
                    }
                    break;
                case CalibrationDataCommand:
                    {
                        int calibrationDataPayloadLength = reader.ReadInt32();
                        byte[] calibrationDataPayload = reader.ReadBytes(calibrationDataPayloadLength);

                        CalculatedCameraCalibration calibration;
                        if (CalculatedCameraCalibration.TryDeserialize(calibrationDataPayload, out calibration))
                        {
                            compositionManager.EnableHolographicCamera(transform, new CalibrationData(calibration.Intrinsics, calibration.Extrinsics));
                        }
                        else
                        {
                            Debug.LogError("Received a CalibrationData packet from the HoloLens that could not be understood.");
                        }
                    }
                    break;
            }
        }

        void ICommandHandler.OnConnected(SocketEndpoint endpoint)
        {
        }

        void ICommandHandler.OnDisconnected(SocketEndpoint endpoint)
        {
        }
    }
}