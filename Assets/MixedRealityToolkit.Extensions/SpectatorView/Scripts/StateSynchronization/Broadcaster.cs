// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// This class observes changes and updates content on a user device.
    /// </summary>
    public class Broadcaster : Singleton<Broadcaster>
    {
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
        public int Port = 7410;

        /// <summary>
        /// Called when a socket end point has connected.
        /// </summary>
        public event Action<SocketEndpoint> Connected;

        /// <summary>
        /// Called when a socket end point has disconnected.
        /// </summary>
        public event Action<SocketEndpoint> Disconnected;

        private const float PerfUpdateTimeSeconds = 1.0f;
        private float timeUntilNextPerfUpdate = PerfUpdateTimeSeconds;

        protected override void Awake()
        {
            base.Awake();

            // Ensure that runInBackground is set to true so that the app continues to send network
            // messages even if it loses focus
            Application.runInBackground = true;
        }

        protected void Start()
        {
            SetupNetworkConnectionManager();
        }

        protected virtual void SetupNetworkConnectionManager()
        {
            connectionManager.OnConnected += NetMgr_OnConnected;
            connectionManager.OnDisconnected += NetMgr_OnDisconnected;
            connectionManager.OnReceive += NetMgr_OnReceive;
            connectionManager.StartListening(Port);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CleanUpNetworkConnectionManager();
        }

        protected virtual void CleanUpNetworkConnectionManager()
        {
            connectionManager.StopListening();
            connectionManager.DisconnectAll();
        }

        protected void NetMgr_OnConnected(SocketEndpoint endpoint)
        {
            Connected?.Invoke(endpoint);
        }

        protected void NetMgr_OnDisconnected(SocketEndpoint endpoint)
        {
            Disconnected?.Invoke(endpoint);
        }

        protected void NetMgr_OnReceive(IncomingMessage data)
        {
            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();
                switch (command)
                {
                    case "SYNC":
                        {
                            reader.ReadSingle(); // float time
                            StateSynchronizationSceneManager.Instance.ReceiveMessage(data.Endpoint, reader);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// True if network connections exist, otherwise false
        /// </summary>
        public bool HasConnections
        {
            get
            {
                return connectionManager != null && connectionManager.HasConnections;
            }
        }

        /// <summary>
        /// Returns how many bytes have been queued to send to other devices
        /// </summary>
        public int OutputBytesQueued
        {
            get
            {
                return connectionManager.OutputBytesQueued;
            }
        }

        private void Update()
        {
            if (connectionManager == null)
            {
                return;
            }

            UpdateExtension();

            if (HasConnections && BroadcasterSettings.IsInitialized && BroadcasterSettings.Instance && BroadcasterSettings.Instance.AutomaticallySynchronizeAllGameObjects)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    foreach (GameObject root in scene.GetRootGameObjects())
                    {
                        ComponentExtensions.EnsureComponent<TransformBroadcaster>(root);
                    }
                }
            }
        }

        /// <summary>
        /// Extension method called on update
        /// </summary>
        protected virtual void UpdateExtension() { }

        /// <summary>
        /// Called after a frame is completed to send state data to socket end points.
        /// </summary>
        public void OnFrameCompleted()
        {
            //Camera update
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter message = new BinaryWriter(memoryStream))
                {
                    message.Write("Camera");
                    Transform camTrans = Camera.main.transform;
                    message.Write(Time.time);
                    message.Write(camTrans.position);
                    message.Write(camTrans.rotation);
                    message.Flush();

                    connectionManager.Broadcast(memoryStream.ToArray());
                }
            }

            //Perf
            timeUntilNextPerfUpdate -= Time.deltaTime;
            if (timeUntilNextPerfUpdate < 0)
            {
                timeUntilNextPerfUpdate = PerfUpdateTimeSeconds;

                using (MemoryStream memoryStream = new MemoryStream())
                using (BinaryWriter message = new BinaryWriter(memoryStream))
                {
                    message.Write("Perf");

                    StateSynchronizationPerformanceMonitor.Instance.WriteMessage(message);
                    message.Flush();
                    connectionManager.Broadcast(memoryStream.ToArray());
                }
            }
        }
    }
}
