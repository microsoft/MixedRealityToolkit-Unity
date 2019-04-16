// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microsoft.MixedReality.Toolkit.Extensions.Experimental.Sharing
{
    /// <summary>
    /// Delegate that is called when a new player enters a shared application
    /// </summary>
    /// <param name="playerId">Unique player id</param>
    public delegate void PlayerConnectedHandler(string playerId);

    /// <summary>
    /// Delegate that is called when an existing player exits a shared application
    /// </summary>
    /// <param name="playerId">Unique player id</param>
    public delegate void PlayerDisconnectedHandler(string playerId);

    /// <summary>
    /// Interface implemented by classes that want to receive updates when players enter and exit a shared application
    /// </summary>
    public interface IPlayerStateObserver
    {
        /// <summary>
        /// Function that can be called when a new player connects to the shared application
        /// </summary>
        /// <param name="playerId">Unique player id</param>
        void PlayerConnected(string playerId);

        /// <summary>
        /// Function that can be called when an existing player disconnects from the shared application
        /// </summary>
        /// <param name="playerId">Unique player id</param>
        void PlayerDisconnected(string playerId);
    }

    /// <summary>
    /// Interface implemented by classes that manage players entering and exiting a shared application
    /// </summary>
    public interface IPlayerService
    {
        /// <summary>
        /// Event called when a new player connects to the shared application
        /// </summary>
        event PlayerConnectedHandler PlayerConnected;

        /// <summary>
        /// Event called when an existing player disconnects from the shared application
        /// </summary>
        event PlayerDisconnectedHandler PlayerDisconnected;
    }
}

