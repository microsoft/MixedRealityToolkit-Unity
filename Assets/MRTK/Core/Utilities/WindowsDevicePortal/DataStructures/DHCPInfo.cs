// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.WindowsDevicePortal
{
    [Serializable]
    public class DHCPInfo
    {
        public int LeaseExpires;
        public int LeaseObtained;
        public IpAddressInfo Address;
    }
}