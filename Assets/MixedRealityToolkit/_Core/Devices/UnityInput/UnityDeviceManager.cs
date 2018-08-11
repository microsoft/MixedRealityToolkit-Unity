// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput
{
    public class UnityDeviceManager : BaseDeviceManager
    {
        public UnityDeviceManager(string name, uint priority) : base(name, priority) { }

        private readonly Dictionary<string, GenericUnityController> activeControllers = new Dictionary<string, GenericUnityController>();

        private float deviceRefreshTimer;
        protected float DeviceRefreshInterval = 3.0f;
        protected string[] LastDeviceList;
        protected const string XboxController = "Xbox Controller";
        protected const string XboxOneForWindows = "Xbox One For Windows";
        protected const string XboxBluetoothGamePad = "Xbox Bluetooth Gamepad";
        protected const string XboxWirelessController = "Xbox Wireless Controller";

        public override void Enable()
        {
            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {
                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }
        }

        protected virtual void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            if (LastDeviceList != null && joystickNames.Length == LastDeviceList.Length)
            {
                for (int i = 0; i < LastDeviceList.Length; i++)
                {
                    if (joystickNames[i].Equals(LastDeviceList[i])) { continue; }

                    foreach (var controller in activeControllers)
                    {
                        // TODO get the controller and remove it
                    }
                }
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                // TODO get controller type and call the method to create.
            }

            LastDeviceList = joystickNames;
        }
    }
}
