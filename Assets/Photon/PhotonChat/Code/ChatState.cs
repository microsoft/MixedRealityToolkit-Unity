// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

namespace Photon.Chat
{
    /// <summary>Possible states for a Chat Client.</summary>
    public enum ChatState
    {
        /// <summary>Peer is created but not used yet.</summary>
        Uninitialized,
        /// <summary>Connecting to name server.</summary>
        ConnectingToNameServer,
        /// <summary>Connected to name server.</summary>
        ConnectedToNameServer,
        /// <summary>Authenticating on current server.</summary>
        Authenticating,
        /// <summary>Finished authentication on current server.</summary>
        Authenticated,
        /// <summary>Disconnecting from name server. This is usually a transition from name server to frontend server.</summary>
        DisconnectingFromNameServer,
        /// <summary>Connecting to frontend server.</summary>
        ConnectingToFrontEnd,
        /// <summary>Connected to frontend server.</summary>
        ConnectedToFrontEnd,
        /// <summary>Disconnecting from frontend server.</summary>
        DisconnectingFromFrontEnd,
        /// <summary>Currently not used.</summary>
        QueuedComingFromFrontEnd,
        /// <summary>The client disconnects (from any server).</summary>
        Disconnecting,
        /// <summary>The client is no longer connected (to any server).</summary>
        Disconnected,
    }
}