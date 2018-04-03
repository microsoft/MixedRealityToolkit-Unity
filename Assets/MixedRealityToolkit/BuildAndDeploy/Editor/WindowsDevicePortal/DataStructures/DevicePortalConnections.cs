using System;
using System.Collections.Generic;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
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
