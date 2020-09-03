// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    /// <summary>
    /// Utility class to store a list of device connection info and track current one in use or selected
    /// </summary>
    [Serializable]
    public class DevicePortalConnections
    {
        /// <summary>
        /// List of device endpoints being tracked including ip address, authorization info, etc.
        /// </summary>
        public List<DeviceInfo> Connections = new List<DeviceInfo>(0);

        /// <summary>
        /// Current or last targeted connection index in connection list
        /// </summary>
        public int CurrentConnectionIndex = 0;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public DevicePortalConnections() { }

        /// <summary>
        /// Initialize
        /// </summary>
        public DevicePortalConnections(DeviceInfo deviceInfo)
        {
            Connections.Add(deviceInfo);
        }
    }
}
