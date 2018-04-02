using System;

namespace MixedRealityToolkit.Build.WindowsDevicePortal.DataStructures
{
    [Serializable]
    public class DHCPInfo
    {
        public int LeaseExpires;
        public int LeaseObtained;
        public IpAddressInfo Address;
    }
}