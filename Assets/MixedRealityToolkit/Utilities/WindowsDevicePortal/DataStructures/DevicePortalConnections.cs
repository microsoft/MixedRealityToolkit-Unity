// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class DevicePortalConnections
    {
        public List<DeviceInfo> Connections = new List<DeviceInfo>(0);

        public DevicePortalConnections() { }

        public DevicePortalConnections(DeviceInfo deviceInfo)
        {
            Connections.Add(deviceInfo);
        }
    }
}
