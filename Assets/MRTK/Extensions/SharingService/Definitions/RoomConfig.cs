// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct used to create a room.
    /// </summary>
    [Serializable]
    public struct RoomConfig
    {
        public static RoomConfig Default
        { 
            get
            {
                return new RoomConfig()
                {
                    Name = "MRTKRoom",
                    MaxDevices = 4,
                    ExpireTime = 10000,
                    VisibleInLobby = true,
                    RoomProps = new RoomProp[0]
                };
            } 
        }

        /// <summary>
        /// True if config's name is empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Name);

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Name;
        /// <summary>
        /// The max number of devices that can enter the room. Must be greater than 0.
        /// </summary>
        public byte MaxDevices;
        /// <summary>
        /// How long the room will persist after the last device leaves, in millisenconds.
        /// </summary>
        public int ExpireTime;
        /// <summary>
        /// If true, the room will be listed in available rooms by the service.
        /// If false, the room will only be accessible if the name is known.
        /// </summary>
        public bool VisibleInLobby;
        /// <summary>
        /// Set of custom properties that are visible in the room.
        /// </summary>
        public RoomProp[] RoomProps;
        /// <summary>
        /// Set of custom properties that are visible in the lobby.
        /// </summary>
        public string[] LobbyProps;
    }
}