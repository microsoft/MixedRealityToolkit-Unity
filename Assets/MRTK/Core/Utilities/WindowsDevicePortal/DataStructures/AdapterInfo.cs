// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class AdapterInfo
    {
        public string Description;
        public string HardwareAddress;
        public int Index;
        public string Name;
        public string Type;
        public DHCPInfo DHCP;
        public IpAddressInfo[] Gateways;
        public IpAddressInfo[] IpAddresses;
    }
}
