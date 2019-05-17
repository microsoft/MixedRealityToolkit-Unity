// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Experimental.SpatialAlignment.Common;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// This class observes changes and updates content on a spectator device.
    /// </summary>
    public class StateSynchronizationObserver : Singleton<StateSynchronizationObserver>
    {
        [Tooltip("Toggle to enable troubleshooting logging.")]
        [SerializeField]
        private bool debugLogging = false;

        /// <summary>
        /// Network connection manager that facilitates sending data between devices.
        /// </summary>
        [Tooltip("Network connection manager that facilitates sending data between devices.")]
        [SerializeField]
        protected TCPConnectionManager connectionManager;

        /// <summary>
        /// Port used for sending data.
        /// </summary>
        [Tooltip("Port used for sending data.")]
        [SerializeField]
        protected int port = 7410;

        [Tooltip("The prefab visual that will represent the coordinate used to synchronize.")]
        [SerializeField]
        private GameObject anchorPrefab = null;

        private SocketEndpoint currentConnection = null;
        private double[] averageTimePerFeature;
        private const float heartbeatTimeInterval = 0.1f;
        private float timeSinceLastHeartbeat = 0.0f;
        private HologramSynchronizer hologramSynchronizer = new HologramSynchronizer();

        private static readonly byte[] heartbeatMessage = GenerateHeartbeatMessage();

        private ConnectedObserver connectedObserver = null;

        /// <summary>
        /// The spatial localization method to be used by the observer.
        /// </summary>
        internal SpatialLocalizationMechanismBase SpatialLocalizationMechanism { get; set; }

        protected override void Awake()
        {
            base.Awake();

            // Ensure that runInBackground is set to true so that the app continues to send network
            // messages even if it loses focus
            Application.runInBackground = true;

            if (connectionManager != null)
            {
                connectionManager.OnConnected += OnConnected;
                connectionManager.OnReceive += OnReceive;
                connectionManager.OnDisconnected += OnDisconnected;

                // Start listening to incoming connections as well.
                connectionManager.StartListening(port);
            }
            else
            {
                Debug.LogError("Connection manager not specified for Observer.");
            }
        }

        protected void Update()
        {
            CheckAndSendHeartbeat();
            hologramSynchronizer.UpdateHolograms();
        }

        private void DebugLog(string message)
        {
            if (debugLogging)
            {
                string connectedState = currentConnection != null ? $"Connected - {currentConnection.Address}" : "Not Connected";
                Debug.Log($"StateSynchronizationObserver - {connectedState}: {message}");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            DebugLog("Disposing observer.");
            connectedObserver?.Dispose();
            connectedObserver = null;

            if (connectionManager != null)
            {
                connectionManager.OnConnected -= OnConnected;
                connectionManager.OnReceive -= OnReceive;
                connectionManager.OnDisconnected -= OnDisconnected;
                connectionManager.StopListening();
                connectionManager.DisconnectAll();
            }
        }

        private void OnConnected(SocketEndpoint endpoint)
        {
            if (currentConnection != null || connectedObserver != null)
            {
                Debug.LogError("We just connected, yet the current observer or connection weren't null");

                DebugLog("Disposing observer.");
                connectedObserver?.Dispose();
                connectedObserver = null;

                currentConnection = null;
            }

            currentConnection = endpoint;
            Debug.Log("Observer Connected!");

            connectedObserver = new ConnectedObserver(Role.Observer, endpoint, () => Instantiate(anchorPrefab), debugLogging);
            DebugLog("Created ConnectedObserver.");
            if (StateSynchronizationSceneManager.IsInitialized)
            {
                StateSynchronizationSceneManager.Instance.MarkSceneDirty();
            }

            hologramSynchronizer.Reset(currentConnection);

            if (SpatialLocalizationMechanism != null)
            {
                DebugLog("Spatially localizing observer");
                connectedObserver.LocalizeAsync(SpatialLocalizationMechanism).FireAndForget();
            }
        }

        private void OnDisconnected(SocketEndpoint endpoint)
        {
            DebugLog("Disconnected, disposing observer");
            connectedObserver?.Dispose();
            connectedObserver = null;

            currentConnection = null;
        }

        /// <summary>
        /// Connects to the provided ip address
        /// </summary>
        /// <param name="address">ip address</param>
        public void ConnectTo(string address)
        {
            if (connectionManager != null)
            {
                connectionManager.ConnectTo(address, port);
            }
        }

        /// <summary>
        /// Disconnects any network connections
        /// </summary>
        public void Disconnect()
        {
            if (connectionManager != null)
            {
                connectionManager.DisconnectAll();
            }
        }

        private void OnReceive(IncomingMessage data)
        {
            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();
                switch (command)
                {
                    case "Camera":
                        {
                            float timeStamp = reader.ReadSingle();
                            hologramSynchronizer.RegisterCameraUpdate(timeStamp);
                            transform.position = reader.ReadVector3();
                            transform.rotation = reader.ReadQuaternion();
                        }
                        break;
                    case "SYNC":
                        {
                            float timeStamp = reader.ReadSingle();
                            int bytesLeft = data.Size - (int)reader.BaseStream.Position;
                            hologramSynchronizer.RegisterFrameData(reader.ReadBytes(bytesLeft), timeStamp);
                        }
                        break;
                    case "Perf":
                        {
                            int featureCount = reader.ReadInt32();

                            if (averageTimePerFeature == null)
                            {
                                averageTimePerFeature = new double[featureCount];
                            }

                            for (int i = 0; i < featureCount; i++)
                            {
                                averageTimePerFeature[i] = reader.ReadSingle();
                            }
                        }
                        break;

                    case ConnectedObserver.SpatialLocalizationMessageHeader:
                        DebugLog("Passing message to observer");
                        connectedObserver.ReceiveMessage(reader);
                        break;
                }
            }
        }

        /// <summary>
        /// Returns true if a network connection has been established, otherwise false.
        /// </summary>
        /// <returns>True if a network connection has been established, otherwise false.</returns>
        public bool IsConnected()
        {
            return currentConnection != null && currentConnection.IsConnected;
        }

        /// <summary>
        /// Returns true if the class is ready for usage.
        /// </summary>
        /// <returns>Returns true if the class is ready for usage.</returns>
        public bool IsReady()
        {
            return connectionManager != null && currentConnection != null;
        }

        /// <summary>
        /// Sends data to other connected devices
        /// </summary>
        /// <param name="data">payload to send to other devices</param>
        public void SendComponentMessage(byte[] data)
        {
            if (currentConnection != null)
            {
                currentConnection.Send(data);
            }
        }

        internal int PerformanceFeatureCount
        {
            get { return averageTimePerFeature?.Length ?? 0; }
        }

        internal IReadOnlyList<double> AverageTimePerFeature
        {
            get { return averageTimePerFeature; }
        }

        private void CheckAndSendHeartbeat()
        {
            if (connectionManager != null &&
                connectionManager.HasConnections)
            {
                timeSinceLastHeartbeat += Time.deltaTime;
                if (timeSinceLastHeartbeat > heartbeatTimeInterval)
                {
                    timeSinceLastHeartbeat = 0.0f;
                    connectionManager.Broadcast(heartbeatMessage);
                }
            }
        }

        private static byte[] GenerateHeartbeatMessage()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                // It doesn't matter what the content of this message is, it just can't conflict with other commands
                // sent in this channel and read by the Broadcaster.
                writer.Write("♥");
                writer.Flush();

                return stream.ToArray();
            }
        }
    }
}
