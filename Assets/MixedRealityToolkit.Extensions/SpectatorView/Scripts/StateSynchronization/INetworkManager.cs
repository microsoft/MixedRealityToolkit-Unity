using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.SpectatorView
{
    public interface INetworkManager : ICommandRegistry
    {
        /// <summary>
        /// Gets whether or not a network connection to the device is established.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets whether or not a network connection to the device is pending.
        /// </summary>
        bool IsConnecting { get; }

        /// <summary>
        /// Gets the local IP address reported by the socket used to connect to the device.
        /// </summary>
        string ConnectedIPAddress { get; }

        void ConnectTo(string targetIpString);
        void Disconnect();
        void Broadcast(byte[] data);
    }
}