using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    /// <summary>
    /// Interface to represent the connection status of a device
    /// that supports locating a shared spatial coordinate.
    /// </summary>
    public interface ILocatableDevice
    {
        /// <summary>
        /// Gets the network manager associated with the device.
        /// </summary>
        INetworkManager NetworkManager { get; }

        /// <summary>
        /// Gets the last-reported tracking status of the device.
        /// </summary>
        bool HasTracking { get; }

        /// <summary>
        /// Gets whether or not the receipt of new poses from the device has stalled for an unexpectedly-large time.
        /// </summary>
        bool IsTrackingStalled { get; }

        /// <summary>
        /// Gets the last-reported status of whether or not the WorldAnchor used for spatial position sharing is located
        /// on the holographic camera rig.
        /// </summary>
        bool IsSharedSpatialCoordinateLocated { get; }

        /// <summary>
        /// Gets whether or not the device is actively attempting to locate the shared spatial coordinate.
        /// </summary>
        bool IsLocatingSharedSpatialCoordinate { get; }

        /// <summary>
        /// Gets the name of the device.
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// Gets the IP address reported by the device itself.
        /// </summary>
        string DeviceIPAddress { get; }

        void SendLocateSharedSpatialCoordinateCommand();

        void NotifyTrackingUpdated();
    }
}