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
        /// Set of custom properties describing the device.
        /// </summary>
        public IEnumerable<DeviceProp> Props;
    }
}