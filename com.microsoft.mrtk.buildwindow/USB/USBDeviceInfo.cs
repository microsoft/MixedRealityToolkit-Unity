// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [Serializable]
    public class USBDeviceInfo
    {
        public USBDeviceInfo(int vendorId, string udid, int productId, string name, int revision)
        {
            VendorId = vendorId;
            Udid = udid;
            ProductId = productId;
            Name = name;
            Revision = revision;
        }

        public int VendorId { get; private set; }

        public string Udid { get; private set; }

        public int ProductId { get; private set; }

        public string Name { get; private set; }

        public int Revision { get; private set; }
    }
}