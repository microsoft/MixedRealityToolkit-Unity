// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Enum describing the status of a device's connection.
    /// </summary>
    [Serializable]
    public enum ConnectStatus : byte
    {
        /// <summary>
        /// The sharing service has not connected.
        /// </summary>
        NotConnected = 1,

        /// <summary>
        /// The sharing service is attempting to connect to the server.
        /// </summary>
        AttemptingToConnect = 2,

        /// <summary>
        /// The sharing service has connected to the server. It may be attempting to a lobby.
        /// </summary>
        ConnectedToServer = 4,

        /// <summary>
        /// The sharing service has joined a lobby. It may be attempting to connect to a room.
        /// </summary>
        ConnectedToLobby = 8,

        /// <summary>
        /// The sharing service has joined a room.
        /// </summary>
        FullyConnected = 16,
    }
}