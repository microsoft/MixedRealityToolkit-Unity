using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class LocatableDevice : MonoBehaviour, ILocatableDevice, ICommandHandler
    {
        private const float trackingStalledReceiveDelay = 1.0f;

        public const string CreateSharedSpatialCoordinateCommand = "CreateSharedSpatialCoordinate";
        public const string DeviceInfoCommand = "DeviceInfo";
        public const string StatusCommand = "Status";
        public const float arUcoMarkerSizeInMeters = 0.1f;

        private INetworkManager networkManager;
        private float lastReceivedPoseTime = -1;
        private string deviceName;
        private string deviceIPAddress;
        private bool hasTracking;
        private bool isSharedSpatialCoordinateLocated;
        private bool isLocatingSharedSpatialCoordinate;

        /// <inheritdoc />
        public INetworkManager NetworkManager => networkManager;

        /// <inheritdoc />
        public string DeviceName => deviceName;

        /// <inheritdoc />
        public string DeviceIPAddress => deviceIPAddress;

        /// <inheritdoc />
        public bool HasTracking => hasTracking;

        /// <inheritdoc />
        public bool IsSharedSpatialCoordinateLocated => isSharedSpatialCoordinateLocated;

        /// <inheritdoc />
        public bool IsLocatingSharedSpatialCoordinate => isLocatingSharedSpatialCoordinate;

        /// <inheritdoc />
        public bool IsTrackingStalled => networkManager.IsConnected && (Time.time - lastReceivedPoseTime) > trackingStalledReceiveDelay;

        public void SendLocateSharedSpatialCoordinateCommand()
        {
            if (networkManager == null || !networkManager.IsConnected)
            {
                Debug.LogError("Can't locate shared spatial coordinate while disconnected");
                return;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter message = new BinaryWriter(memoryStream))
            {
                message.Write(CreateSharedSpatialCoordinateCommand);
                message.Write(arUcoMarkerSizeInMeters);

                networkManager.Broadcast(memoryStream.ToArray());
            }
        }

        private void Awake()
        {
            networkManager = GetComponent<INetworkManager>();
            networkManager.RegisterCommandHandler(DeviceInfoCommand, this);
            networkManager.RegisterCommandHandler(StatusCommand, this);
        }

        public void OnConnected(SocketEndpoint endpoint)
        {
            lastReceivedPoseTime = Time.time;
        }

        public void OnDisconnected(SocketEndpoint endpoint)
        {
        }

        public void HandleCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
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

        public void NotifyTrackingUpdated()
        {
            lastReceivedPoseTime = Time.time;
        }
    }
}