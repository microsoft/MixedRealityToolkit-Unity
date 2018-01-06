// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// 
    /// </summary>
    public class GenericInputSources : Singleton<GenericInputSources>
    {
        protected const string XboxController = "Xbox Controller";
        protected const string XboxOneForWindows = "Xbox One For Windows";
        protected const string XboxBluetoothGamePad = "Xbox Bluetooth Gamepad";
        protected const string XboxWirelessController = "Xbox Wireless Controller";
        protected const string MotionControllerLeft = "Spatial Controller - Left";
        protected const string MotionControllerRight = "Spatial Controller - Right";

        [SerializeField]
        [Tooltip("Time in seconds to determine if an Input Device has been connected or disconnected")]
        protected float DeviceRefreshInterval = 3.0f;
        protected string[] LastDeviceList;

        private float deviceRefreshTimer;

        protected readonly Dictionary<string, InputSource> inputSources = new Dictionary<string, InputSource>(0);

        #region Input Source Implementation

        protected class InputSource : IInputSource
        {
            public InputSource(uint sourceId, string name)
            {
                SourceId = sourceId;
                Name = name;
            }

            public uint SourceId { get; private set; }
            public string Name { get; private set; }

            public SupportedInputInfo SupportedInputInfo { get; private set; }

            public SupportedInputInfo GetSupportedInputInfo()
            {
                return SupportedInputInfo;
            }

            public bool SupportsInputInfo(SupportedInputInfo inputInfo)
            {
                return (GetSupportedInputInfo() & inputInfo) == inputInfo;
            }
        }

        #endregion

        protected virtual void Update()
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
                    if (!joystickNames[i].Equals(LastDeviceList[i]))
                    {
                        InputSource source;
                        if (inputSources.TryGetValue(joystickNames[i], out source))
                        {
                            InputManager.Instance.RaiseSourceLost(source);
                            inputSources.Remove(joystickNames[i]);
                        }
                    }
                }
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                if (joystickNames[i].Contains(MotionControllerLeft) ||
                    joystickNames[i].Contains(MotionControllerRight))
                {
                    // If we don't have any matching joystick types, continue.
                    // Skip any WMR motion controllers connected.  They're handled in the InteractionInputSources.
                    continue;
                }

                if (joystickNames[i].Contains(XboxController) ||
                    joystickNames[i].Contains(XboxOneForWindows) ||
                    joystickNames[i].Contains(XboxBluetoothGamePad) ||
                    joystickNames[i].Contains(XboxWirelessController))
                {
                    var inputSource = new InputSource(InputManager.GenerateNewSourceId(), joystickNames[i]);
                    inputSources.Add(joystickNames[i], inputSource);
                    InputManager.Instance.RaiseSourceDetected(inputSource);
                }
                else
                {
                    Debug.LogWarningFormat("Unimplemented Controller Type: {0}", joystickNames[i]);
                }
            }

            LastDeviceList = joystickNames;
        }
    }
}
