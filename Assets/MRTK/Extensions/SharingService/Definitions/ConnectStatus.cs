// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{

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
        /// The sharing service has connected to the server and is attempting to join the specified lobby.
        /// </summary>
        ConnectedToServer = 4,
        /// <summary>
        /// The sharing service has joined a lobby and is attempting to join the specified room.
        /// </summary>
        ConnectedToLobby = 8,
        /// <summary>
        /// The sharing service has joined a room.
        /// </summary>
        FullyConnected = 16,
    }
}