// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.StateSynchronization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    /// <summary>
    /// Component that connects to the HoloLens application on the holographic camera rig for synchronizing camera poses and receiving calibration data.
    /// </summary>
    [RequireComponent(typeof(TCPConnectionManager))]
    public class HolographicCameraNetworkManager : MonoBehaviour
    {
        private TCPConnectionManager connectionManager;
        private const float trackingStalledReceiveDelay = 1.0f;
        private float lastReceivedPoseTime = -1;

        [SerializeField]
        private CompositionManager compositionManager = null;

        [SerializeField]
        [Tooltip("The port that the " + nameof(HolographicCamera.TCPNetworkListener) + " listens for connections on.")]
        private int remotePort = 7502;

        private SocketEndpoint currentConnection;
        private string holoLensName;
        private string holoLensIPAddress;
        private bool hasTracking;
        private bool isAnchorLocated;

        /// <summary>
        /// Gets the name of the HoloLens running on the holographic camera rig.
        /// </summary>
        public string HoloLensName => holoLensName;

        /// <summary>
        /// Gets the IP address reported by the HoloLens running on the holographic camera rig.
        /// </summary>
        public string HoloLensIPAddress => holoLensIPAddress;

        /// <summary>
        /// Gets the local IP address reported by the socket used to connect to the holographic camera rig.
        /// </summary>
        public string ConnectedIPAddress => currentConnection?.Address;

        /// <summary>
        /// Gets the last-reported tracking status of the HoloLens running on the holographic camera rig.
        /// </summary>
        public bool HasTracking => hasTracking;

        /// <summary>
        /// Gets the last-reported status of whether or not the WorldAnchor used for spatial position sharing is located
        /// on the holographic camera rig.
        /// </summary>
        public bool IsAnchorLocated => isAnchorLocated;

        private void Awake()
        {
            connectionManager = GetComponent<TCPConnectionManager>();
            connectionManager.OnConnected += ConnectionManager_OnConnected;
            connectionManager.OnDisconnected += ConnectionManager_OnDisconnected;
            connectionManager.OnReceive += ConnectionManager_OnReceive;
        }

        private void OnDestroy()
        {
            connectionManager.DisconnectAll();

            connectionManager.OnConnected -= ConnectionManager_OnConnected;
            connectionManager.OnDisconnected -= ConnectionManager_OnDisconnected;
            connectionManager.OnReceive -= ConnectionManager_OnReceive;
        }

        private void ConnectionManager_OnConnected(SocketEndpoint endpoint)
        {
            currentConnection = endpoint;
            lastReceivedPoseTime = Time.time;
            compositionManager.ResetPoseSynchronization();
        }

        private void ConnectionManager_OnDisconnected(SocketEndpoint endpoint)
        {
            if (currentConnection == endpoint)
            {
                currentConnection = null;
            }
        }

        /// <summary>
        /// Gets whether or not a network connection to the holographic camera is established.
        /// </summary>
        public bool IsConnected => connectionManager != null && connectionManager.HasConnections;

        /// <summary>
        /// Gets whether or not a network connection to the holographic camera is pending.
        /// </summary>
        public bool IsConnecting => connectionManager != null && connectionManager.IsConnecting && !connectionManager.HasConnections;

        /// <summary>
        /// Gets whether or not the receipt of new poses from the camera has stalled for an unexpectedly-large time.
        /// </summary>
        public bool IsTrackingStalled => IsConnected && (Time.time - lastReceivedPoseTime) > trackingStalledReceiveDelay;

        /// <summary>
        /// Connects to the holographic camera rig with the provided remote IP address.
        /// </summary>
        /// <param name="remoteAddress">The IP address of the holographic camera rig's HoloLens.</param>
        public void ConnectTo(string remoteAddress)
        {
            connectionManager.ConnectTo(remoteAddress, remotePort);
        }

        /// <summary>
        /// Disconnects the network connection to the holographic camera rig.
        /// </summary>
        public void Disconnect()
        {
            connectionManager.DisconnectAll();
        }

        private void ConnectionManager_OnReceive(IncomingMessage data)
        {
            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();

                switch (command)
                {
                    case "Camera":
                        {
                            lastReceivedPoseTime = Time.time;
                            float timestamp = reader.ReadSingle();
                            Vector3 cameraPosition = reader.ReadVector3();
                            Quaternion cameraRotation = reader.ReadQuaternion();

                            compositionManager.AddCameraPose(cameraPosition, cameraRotation, timestamp);
                        }
                        break;
                    case "CalibrationData":
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
                    case "DeviceInfo":
                        {
                            holoLensName = reader.ReadString();
                            holoLensIPAddress = reader.ReadString();
                        }
                        break;
                    case "Status":
                        {
                            hasTracking = reader.ReadBoolean();
                            isAnchorLocated = reader.ReadBoolean();
                        }
                        break;
                }
            }
        }
    }
}