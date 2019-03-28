// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Flag that signals the priority for a new network payload
    /// </summary>
    public enum NetworkPriority
    {
        Default,
        Critical
    }

    /// <summary>
    /// Delegate called when new data has been received through a <see cref="INetworkingService"/>
    /// </summary>
    public delegate void DataHandler(string playerId, byte[] payload);

    /// <summary>
    /// Interface implemented by classes that setup network pipes
    /// </summary>
    public interface INetworkingService
    {
        /// <summary>
        /// Sends data over the network
        /// </summary>
        /// <param name="data">Data payload to send over the network</param>
        /// <param name="priority">Priority of the data being sent over the network</param>
        bool SendData(byte[] data, NetworkPriority priority = NetworkPriority.Default);

        /// <summary>
        /// Event called when new data has been received
        /// </summary>
        event DataHandler DataReceived;
    }
}
