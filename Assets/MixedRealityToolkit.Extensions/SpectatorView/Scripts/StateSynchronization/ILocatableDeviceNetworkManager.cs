using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface ILocatableDeviceNetworkManager
    {
        bool IsConnected { get; }
        bool HasTracking { get; }
        bool IsSharedSpatialCoordinateLocated { get; }
        bool IsLocatingSharedSpatialCoordinate { get; }
        bool IsTrackingStalled { get; }
        string DeviceIPAddress { get; }
        string ConnectedIPAddress { get; }
        string DeviceName { get; }
        bool IsConnecting { get; }

        void Disconnect();
        void ConnectTo(string targetIpString);
        void SendLocateSharedSpatialCoordinateCommand();
    }
}