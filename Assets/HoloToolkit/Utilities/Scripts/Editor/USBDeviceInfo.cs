// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity
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

        [SerializeField]
        public int VendorId { get; private set; }

        [SerializeField]
        public string Udid { get; private set; }

        [SerializeField]
        public int ProductId { get; private set; }

        [SerializeField]
        public string Name { get; private set; }

        [SerializeField]
        public int Revision { get; private set; }
    }
}