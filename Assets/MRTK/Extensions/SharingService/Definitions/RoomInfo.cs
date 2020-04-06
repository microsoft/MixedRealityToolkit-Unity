// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct used to define a room available in the lobby.
    /// </summary>
    public struct RoomInfo
    {
        public static RoomInfo Empty => empty;
        private static RoomInfo empty = new RoomInfo();

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string Name;

        /// <summary>
        /// The number of devices currently in a room.
        /// </summary>
        public byte NumDevices;

        /// <summary>
        /// The max number of devices that can enter the room.
        /// </summary>
        public byte MaxDevices;

        /// <summary>
        /// True if this room can be joined.
        /// </summary>
        public bool IsOpen;

        /// <summary>
        /// Set of custom properties that are visible in the room.
        /// Will be empty until room is joined unless properties are included in LobbyProps on create.
        /// </summary>
        public IEnumerable<RoomProp> RoomProps;
    }
}