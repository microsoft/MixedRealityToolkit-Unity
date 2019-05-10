// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView.Compositor
{
    [RequireComponent(typeof(TCPConnectionManager))]
    public class HolographicCameraNetworkManager : MonoBehaviour
    {
        private TCPConnectionManager connectionManager;
        private float lastReceivedPoseTime = -1;
        private readonly Dictionary<string, ICalibrationParser> calibrationParsers = new Dictionary<string, ICalibrationParser>();

        [SerializeField]
        private CompositionManager compositionManager = null;

        [SerializeField]
        private int remotePort = 7502;

        private void Awake()
        {
            connectionManager = GetComponent<TCPConnectionManager>();
            connectionManager.OnConnected += ConnectionManager_OnConnected;
            connectionManager.OnReceive += ConnectionManager_OnReceive;
        }

        private void OnDestroy()
        {
            connectionManager.DisconnectAll();
        }

        private void ConnectionManager_OnConnected(SocketEndpoint endpoint)
        {
            lastReceivedPoseTime = Time.time;
        }

        public void RegisterCalibrationParser(string calibrationType, ICalibrationParser parser)
        {
            calibrationParsers[calibrationType] = parser;
        }

        public bool IsConnected => connectionManager != null && connectionManager.HasConnections;

        public bool IsConnecting => connectionManager != null && connectionManager.IsConnecting && !connectionManager.HasConnections;

        public void ConnectTo(string remoteAddress)
        {
            connectionManager.ConnectTo(remoteAddress, remotePort);
        }

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
                            CalibrationPackage calibrationPackage = JsonUtility.FromJson<CalibrationPackage>(calibrationDataJson);

                            ICalibrationParser parser;
                            if (calibrationParsers.TryGetValue(calibrationPackage.calibrationType, out parser))
                            {
                                ICalibrationData calibrationData;
                                if (parser.TryParse(calibrationPackage.calibrationData, out calibrationData))
                                {
                                    compositionManager.EnableHolographicCamera(transform, calibrationData);
                                }
                                else
                                {
                                    Debug.LogError("Failed to parse the received calibration data");
                                }
                            }
                            else
                            {
                                Debug.LogError("Received calibration data with no registered parser");
                            }
                        }
                        break;
                }
            }
        }

        [Serializable]
        private class CalibrationPackage
        {
            public string calibrationType;
            public string calibrationData;
        }
    }
}