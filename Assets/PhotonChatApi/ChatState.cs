// ----------------------------------------------------------------------------------------------------------------------
// <summary>The Photon Chat Api enables clients to connect to a chat server and communicate with other clients.</summary>
// <remarks>ChatClient is the main class of this api.</remarks>
// <copyright company="Exit Games GmbH">Photon Chat Api - Copyright (C) 2014 Exit Games GmbH</copyright>
// ----------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.Photon.Chat
{
    /// <summary>Possible states for a LoadBalancingClient.</summary>
    public enum ChatState
    {
        /// <summary>Peer is created but not used yet.</summary>
        Uninitialized,
        /// <summary>Connecting to master (includes connect, authenticate and joining the lobby)</summary>
        ConnectingToNameServer,
        /// <summary>Connected to master server.</summary>
        ConnectedToNameServer,
        /// <summary>Usually when Authenticated, the client will join a game or the lobby (if AutoJoinLobby is true).</summary>
        Authenticating,
        /// <summary>Usually when Authenticated, the client will join a game or the lobby (if AutoJoinLobby is true).</summary>
        Authenticated,
        /// <summary>Transition from master to game server.</summary>
        DisconnectingFromNameServer,
        /// <summary>Transition to gameserver (client will authenticate and join/create game).</summary>
        ConnectingToFrontEnd,
        /// <summary>Connected to gameserver (going to auth and join game).</summary>
        ConnectedToFrontEnd,
        /// <summary>Transition from gameserver to master (after leaving a room/game).</summary>
        DisconnectingFromFrontEnd,
        /// <summary>Currently not used.</summary>
        QueuedComingFromFrontEnd,
        /// <summary>The client disconnects (from any server).</summary>
        Disconnecting,
        /// <summary>The client is no longer connected (to any server). Connect to master to go on.</summary>
        Disconnected,
    }
}