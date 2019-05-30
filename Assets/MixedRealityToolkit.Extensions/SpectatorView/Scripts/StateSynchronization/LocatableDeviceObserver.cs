using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.MixedReality.Toolkit.Extensions.Experimental.Socketer;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public class LocatableDeviceObserver : MonoBehaviour
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
        private Vector3 sharedSpatialCoordinateWorldPosition;
        private Quaternion sharedSpatialCoordinateWorldRotation;

        /// <summary>
        /// Gets the network manager associated with the device.
        /// </summary>
        public INetworkManager NetworkManager => networkManager;

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        public string DeviceName => deviceName;

        /// <summary>
        /// Gets the IP address reported by the device itself.
        /// </summary>
        public string DeviceIPAddress => deviceIPAddress;

        /// <summary>
        /// Gets the last-reported tracking status of the device.
        /// </summary>
        public bool HasTracking => hasTracking;

        /// <summary>
        /// Gets the last-reported status of whether or not the WorldAnchor used for spatial position sharing is located
        /// on the holographic camera rig.
        /// </summary>
        public bool IsSharedSpatialCoordinateLocated => isSharedSpatialCoordinateLocated;

        /// <summary>
        /// Gets whether or not the device is actively attempting to locate the shared spatial coordinate.
        /// </summary>
        public bool IsLocatingSharedSpatialCoordinate => isLocatingSharedSpatialCoordinate;

        /// <summary>
        /// Gets whether or not the receipt of new poses from the device has stalled for an unexpectedly-large time.
        /// </summary>
        public bool IsTrackingStalled => networkManager.IsConnected && (Time.time - lastReceivedPoseTime) > trackingStalledReceiveDelay;

        /// <summary>
        /// Gets the position of the shared spatial coordinate in the device's world space.
        /// </summary>
        public Vector3 SharedSpatialCoordinateWorldPosition => sharedSpatialCoordinateWorldPosition;

        /// <summary>
        /// Gets the rotation of the shared spatial coordinate in the device's world space.
        /// </summary>
        public Quaternion SharedSpatialCoordinateWorldRotation => sharedSpatialCoordinateWorldRotation;

        /// <summary>
        /// Sends a command to the device to request that the device should locate the shared spatial coordinate.
        /// </summary>
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
            networkManager.Connected += OnConnected;
            networkManager.RegisterCommandHandler(DeviceInfoCommand, HandleDeviceInfoCommand);
            networkManager.RegisterCommandHandler(StatusCommand, HandleStatusCommand);
            networkManager.RegisterCommandHandler(StateSynchronizationObserver.CameraCommand, HandleCameraCommand);
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.Connected -= OnConnected;
                networkManager.UnregisterCommandHandler(DeviceInfoCommand, HandleDeviceInfoCommand);
                networkManager.UnregisterCommandHandler(StatusCommand, HandleStatusCommand);
                networkManager.UnregisterCommandHandler(StateSynchronizationObserver.CameraCommand, HandleCameraCommand);
            }
        }

        private void OnConnected(SocketEndpoint endpoint)
        {
            lastReceivedPoseTime = Time.time;
        }

        private void HandleDeviceInfoCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            deviceName = reader.ReadString();
            deviceIPAddress = reader.ReadString();
        }

        private void HandleStatusCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            hasTracking = reader.ReadBoolean();
            isSharedSpatialCoordinateLocated = reader.ReadBoolean();
            isLocatingSharedSpatialCoordinate = reader.ReadBoolean();
            sharedSpatialCoordinateWorldPosition = reader.ReadVector3();
            sharedSpatialCoordinateWorldRotation = reader.ReadQuaternion();
        }

        private void HandleCameraCommand(SocketEndpoint endpoint, string command, BinaryReader reader, int remainingDataSize)
        {
            lastReceivedPoseTime = Time.time;
        }
    }
}