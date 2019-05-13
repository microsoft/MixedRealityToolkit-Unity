// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.StateSynchronization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.WSA;

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
        private const float arUcoMarkerSizeInMeters = 0.1f;
        private float lastReceivedPoseTime = -1;
        private readonly Dictionary<string, ICalibrationParser> calibrationParsers = new Dictionary<string, ICalibrationParser>();

        [SerializeField]
        private CompositionManager compositionManager = null;

        [SerializeField]
        [Tooltip("The port that the " + nameof(HolographicCamera.TCPNetworkListener) + " listens for connections on.")]
        private int remotePort = 7502;

        private SocketEndpoint currentConnection;
        private string holoLensName;
        private string holoLensIPAddress;
        private PositionalLocatorState holoLensTrackingState;
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
        public bool HasTracking => holoLensTrackingState == PositionalLocatorState.Active;

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

        public void RegisterCalibrationParser(string calibrationType, ICalibrationParser parser)
        {
            calibrationParsers[calibrationType] = parser;
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
                            string calibrationDataJson = reader.ReadString();

                            ICalibrationData calibrationData;
                            if (CalibrationPackage.TryParseCalibration(calibrationDataJson, calibrationParsers, out calibrationData))
                            {
                                compositionManager.EnableHolographicCamera(transform, calibrationData);
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
                            holoLensTrackingState = (PositionalLocatorState)reader.ReadByte();
                            isAnchorLocated = reader.ReadBoolean();
                        }
                        break;
                }
            }
        }

        public void SendLocateSharedAnchorCommand()
        {
            if (currentConnection == null)
            {
                Debug.LogError("Can't locate shared anchor while disconnected");
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write("CreateSharedAnchor");
                message.Write(arUcoMarkerSizeInMeters);

                currentConnection.Send(memoryStream.ToArray());
            }
        }

        [Serializable]
        private class CalibrationPackage
        {
            public string calibrationType = null;
            public string calibrationData = null;

            public static bool TryParseCalibration(string calibrationDataJson, IDictionary<string, ICalibrationParser> calibrationParsers, out ICalibrationData calibrationData)
            {
                CalibrationPackage calibrationPackage = JsonUtility.FromJson<CalibrationPackage>(calibrationDataJson);

                ICalibrationParser parser;
                if (calibrationParsers.TryGetValue(calibrationPackage.calibrationType, out parser))
                {
                    if (parser.TryParse(calibrationPackage.calibrationData, out calibrationData))
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Failed to parse the received calibration data");
                        return false;
                    }
                }
                else
                {
                    Debug.LogError("Received calibration data with no registered parser");
                    calibrationData = null;
                    return false;
                }
            }
        }
    }
}