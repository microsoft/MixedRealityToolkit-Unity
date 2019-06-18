using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public abstract class NetworkManager<TService> : CommandRegistry<TService>, INetworkManager where TService : Singleton<TService>
    {
        private float lastReceivedUpdate;

        [SerializeField]
        protected TCPConnectionManager connectionManager = null;

        private SocketEndpoint currentConnection;

        /// <inheritdoc />
        public string ConnectedIPAddress => currentConnection?.Address;

        /// <inheritdoc />
        public bool IsConnected => connectionManager != null && connectionManager.HasConnections;

        /// <inheritdoc />
        public bool IsConnecting => connectionManager != null && connectionManager.IsConnecting && !connectionManager.HasConnections;

        /// <inheritdoc />
        public TimeSpan TimeSinceLastUpdate => TimeSpan.FromSeconds(Time.time - lastReceivedUpdate);

        /// <summary>
        /// Gets the port used to connect to the remote device.
        /// </summary>
        protected abstract int RemotePort { get; }

        /// <summary>
        /// Connects to the holographic camera rig with the provided remote IP address.
        /// </summary>
        /// <param name="remoteAddress">The IP address of the holographic camera rig's HoloLens.</param>
        public void ConnectTo(string remoteAddress)
        {
            connectionManager.ConnectTo(remoteAddress, RemotePort);
        }

        /// <summary>
        /// Sends data to other connected devices
        /// </summary>
        /// <param name="data">payload to send to other devices</param>
        public void Broadcast(byte[] data)
        {
            if (currentConnection != null)
            {
                currentConnection.Send(data);
            }
        }

        /// <summary>
        /// Disconnects the network connection to the holographic camera rig.
        /// </summary>
        public void Disconnect()
        {
            connectionManager.DisconnectAll();
        }

        protected override void Awake()
        {
            base.Awake();

            connectionManager.OnConnected += OnConnected;
            connectionManager.OnDisconnected += OnDisconnected;
            connectionManager.OnReceive += OnReceive;
        }

        protected virtual void Start()
        {
            if (SpatialCoordinateSystemManager.IsInitialized)
            {
                SpatialCoordinateSystemManager.Instance.RegisterNetworkManager(this);
            }
            else
            {
                Debug.LogError("Attempted to register NetworkManager with the SpatialCoordinateSystemManager but no SpatialCoordinateSystemManager is initialized");
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            connectionManager.StopListening();
            connectionManager.DisconnectAll();

            connectionManager.OnConnected -= OnConnected;
            connectionManager.OnDisconnected -= OnDisconnected;
            connectionManager.OnReceive -= OnReceive;

            if (SpatialCoordinateSystemManager.IsInitialized)
            {
                SpatialCoordinateSystemManager.Instance.UnregisterNetworkManager(this);
            }
        }

        protected virtual void OnConnected(SocketEndpoint endpoint)
        {
            currentConnection = endpoint;

            NotifyConnected(endpoint);
        }

        protected virtual void OnDisconnected(SocketEndpoint endpoint)
        {
            if (currentConnection == endpoint)
            {
                currentConnection = null;
            }

            NotifyDisconnected(endpoint);
        }

        protected void OnReceive(IncomingMessage data)
        {
            lastReceivedUpdate = Time.time;

            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();

                NotifyCommand(data.Endpoint, command, reader, data.Size - (int)stream.Position);
            }
        }
    }
}