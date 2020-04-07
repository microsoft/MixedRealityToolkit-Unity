// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.Extensions.Sharing
{
    /// <summary>
    /// Struct use to describe a device connected in the room.
    /// </summary>
    public struct DeviceInfo
    {
        /// <summary>
        /// The ID of the device.
        /// </summary>
        public short ID;

        /// <summary>
        /// The name of the device.
        /// </summary>
        public string Name;

        /// <summary>
        /// True if this is the local device.
        /// </summary>
        public bool IsLocalDevice;

        /// <summary>
        /// The role of the device.
        /// </summary>
        public AppRole AppRole;

        /// <summary>
        /// The type of device connecting.
        /// </summary>
        public DeviceTypeEnum DeviceType;

        /// <summary>
        /// The connection state of the device.
        /// </summary>
        public DeviceConnectionState ConnectionState;

        /// <summary>
        /// Set of custom properties describing the device.
        /// </summary>
        public IEnumerable<DeviceProp> Props;
    }
}