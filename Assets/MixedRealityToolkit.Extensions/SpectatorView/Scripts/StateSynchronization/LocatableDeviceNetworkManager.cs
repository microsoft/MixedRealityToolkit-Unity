using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public abstract class LocatableDeviceNetworkManager<TService> : CommandRegistry<TService>, ILocatableDeviceNetworkManager, ICommandHandler where TService : Singleton<TService>
    {
        public const string CreateSharedSpatialCoordinateCommand = "CreateSharedSpatialCoordinate";
        public const string DeviceInfoCommand = "DeviceInfo";
        public const string StatusCommand = "Status";
        public const float arUcoMarkerSizeInMeters = 0.1f;

        [SerializeField]
        protected TCPConnectionManager connectionManager = null;

        private const float trackingStalledReceiveDelay = 1.0f;

        private float lastReceivedPoseTime = -1;
        private SocketEndpoint currentConnection;
        private string deviceName;
        private string deviceIPAddress;
        private bool hasTracking;
        private bool isSharedSpatialCoordinateLocated;
        private bool isLocatingSharedSpatialCoordinate;

        /// <summary>
        /// Gets the name of the HoloLens running on the holographic camera rig.
        /// </summary>
        public string DeviceName => deviceName;

        /// <summary>
        /// Gets the IP address reported by the HoloLens running on the holographic camera rig.
        /// </summary>
        public string DeviceIPAddress => deviceIPAddress;

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
        public bool IsSharedSpatialCoordinateLocated => isSharedSpatialCoordinateLocated;

        /// <summary>
        /// Gets whether or not the HMD is actively attempting to locate the shared spatial coordinate.
        /// </summary>
        public bool IsLocatingSharedSpatialCoordinate => isLocatingSharedSpatialCoordinate;

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

        protected abstract int RemotePort { get; }

        /// <summary>
        /// Connects to the holographic camera rig with the provided remote IP address.
        /// </summary>
        /// <param name="remoteAddress">The IP address of the holographic camera rig's HoloLens.</param>
        public void ConnectTo(string remoteAddress)
        {
            connectionManager.ConnectTo(remoteAddress, RemotePort);
        }

        public void SendLocateSharedSpatialCoordinateCommand()
        {
            if (currentConnection == null)
            {
                Debug.LogError("Can't locate shared spatial coordinate while disconnected");
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write(CreateSharedSpatialCoordinateCommand);
                message.Write(arUcoMarkerSizeInMeters);

                currentConnection.Send(memoryStream.ToArray());
            }
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

            connectionManager = GetComponent<TCPConnectionManager>();
            connectionManager.OnConnected += OnConnected;
            connectionManager.OnDisconnected += OnDisconnected;
            connectionManager.OnReceive += OnReceive;

            RegisterCommandHandler(DeviceInfoCommand, this);
            RegisterCommandHandler(StatusCommand, this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            connectionManager.StopListening();
            connectionManager.DisconnectAll();

            connectionManager.OnConnected -= OnConnected;
            connectionManager.OnDisconnected -= OnDisconnected;
            connectionManager.OnReceive -= OnReceive;
        }

        protected virtual void OnConnected(SocketEndpoint endpoint)
        {
            currentConnection = endpoint;
            lastReceivedPoseTime = Time.time;
        }

        protected virtual void OnDisconnected(SocketEndpoint endpoint)
        {
            if (currentConnection == endpoint)
            {
                currentConnection = null;
            }
        }

        protected void UpdateTrackingStatus()
        {
            lastReceivedPoseTime = Time.time;
        }

        protected void OnReceive(IncomingMessage data)
        {
            using (MemoryStream stream = new MemoryStream(data.Data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                string command = reader.ReadString();

                NotifyCommand(data.Endpoint, command, reader, data.Size - (int)stream.Position);
            }
        }

        void ICommandHandler.OnConnected(SocketEndpoint endpoint)
        {
        }

        void ICommandHandler.OnDisconnected(SocketEndpoint endpoint)
        {
        }

        public virtual void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            switch (command)
            {
                case DeviceInfoCommand:
                    {
                        deviceName = reader.ReadString();
                        deviceIPAddress = reader.ReadString();
                    }
                    break;
                case StatusCommand:
                    {
                        hasTracking = reader.ReadBoolean();
                        isSharedSpatialCoordinateLocated = reader.ReadBoolean();
                        isLocatingSharedSpatialCoordinate = reader.ReadBoolean();
                    }
                    break;
            }
        }
    }
}