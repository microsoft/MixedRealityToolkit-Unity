// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace Microsoft.MixedReality.Toolkit.Core.Utilities.WindowsDevicePortal.DataStructures
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
