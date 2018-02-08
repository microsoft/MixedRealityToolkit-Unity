// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace MixedRealityToolkit.Build.DataStructures
{
    [Serializable]
    public class DHCPInfo
    {
        public int LeaseExpires;
        public int LeaseObtained;
        public IpAddressInfo Address;
    }
}