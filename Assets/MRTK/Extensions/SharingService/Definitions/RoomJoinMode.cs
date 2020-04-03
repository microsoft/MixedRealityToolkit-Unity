// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Defines behavior for joining a room.
    /// </summary>
    public enum RoomJoinMode
    {
        /// <summary>
        /// Default. First, attempt to join. If that fails, attempt to rejoin. If that fails, attempt to create. If that fails, cancel the attempt.
        /// </summary>
        JoinRejoinCreate = 0,

        /// <summary>
        /// First, attempt to join. If that fails, disconnect and attempt to join again. If that fails, cancel the attempt.
        /// Use when you know a room exists.
        /// </summary>
        JoinRejoin = 1,

        /// <summary>
        /// First, attempt to join. If that fails, disconnect and attempt to join with a new ID. If that fails, cancel the attempt. 
        /// Use when you know a room exists and want to join as a new device.
        /// </summary>
        JoinForceRejoin = 2,

        /// <summary>
        /// Attempt to join. If that fails, cancel the attempt. 
        /// Use when you know a room exists and only want to join if you haven't joined before.
        /// </summary>
        JoinOnly = 3,

        /// <summary>
        /// Attemp to re-join. If that fails, cancel the attempt. 
        /// Use when you only want to join a room if it's the one you were previously in.
        /// </summary>
        RejoinOnly = 4,

        /// <summary>
        /// Attempt to create and join. If that fails, cancel the attempt. 
        /// Use when you only want to join a room if you created it.
        /// </summary>
        CreateOnly = 5,
    }
}