// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Hardware;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    [InitializeOnLoad]
    public class USBDeviceListener
    {
        public static USBDeviceInfo[] USBDevices;

        public delegate void OnUsbDevicesChanged(UsbDevice[] usbDevices);

        public static event OnUsbDevicesChanged UsbDevicesChanged;

        private static readonly List<USBDeviceInfo> USBDevicesList = new List<USBDeviceInfo>(0);

        static USBDeviceListener()
        {
            UnityEditor.Hardware.Usb.DevicesChanged += NotifyUsbDevicesChanged;
        }

        private static void NotifyUsbDevicesChanged(UsbDevice[] devices)
        {
            UsbDevicesChanged?.Invoke(devices);

            USBDevicesList.Clear();

            foreach (UsbDevice device in devices)
            {
                USBDevicesList.Add(new USBDeviceInfo(device.vendorId, device.udid, device.productId, device.name, device.revision));
            }

            USBDevices = USBDevicesList.ToArray();
        }
    }
}