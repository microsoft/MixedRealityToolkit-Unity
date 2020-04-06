// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct for configuring a connection attempt.
    /// All default values are valid.
    /// </summary>
    public struct ConnectConfig
    {
        /// <summary>
        /// Creates a connect config from room info.
        /// </summary>
        public ConnectConfig(RoomInfo room) : this()
        {
            // We're not setting any room properties because we're not creating a room.
            RoomConfig = new RoomConfig()
            {
                Name = room.Name
            };
            // Join or re-join this room, don't create the room.
            RoomJoinMode = RoomJoinMode.JoinRejoin;
            // The rest of the config will be pulled from the service's profile.
        }

        /// <summary>
        /// Creates a connect config from room name.
        /// </summary>
        public ConnectConfig(string roomName) : this()
        {
            RoomConfig = new RoomConfig()
            {
                Name = roomName
            };
            // Join or re-join this room, don't create the room.
            RoomJoinMode = RoomJoinMode.JoinRejoin;
            // The rest of the config will be pulled from the service's profile.
        }

        /// <summary>
        /// Configuration of the target room.
        /// </summary>
        public RoomConfig RoomConfig;

        /// <summary>
        /// Which room join steps to use.
        /// </summary>
        public RoomJoinMode RoomJoinMode;

        /// <summary>
        /// This device's desired app role.
        /// </summary>
        public AppRole RequestedRole;

        /// <summary>
        /// This device's desired type. If None, value will be set automatically.
        /// </summary>
        public DeviceTypeEnum RequestedDeviceType;

        /// <summary>
        /// This device's desired name. If empty, value will be pulled from profile.
        /// </summary>
        public string RequestedName;

        /// <summary>
        /// This device's desired subscription mode.
        /// </summary>
        public SubscriptionMode SubscriptionMode;

        /// <summary>
        /// Subscription data types. Must not be null if SubscriptionMode is set to Manual.
        /// </summary>
        public short[] SubscriptionTypes;
    }
}