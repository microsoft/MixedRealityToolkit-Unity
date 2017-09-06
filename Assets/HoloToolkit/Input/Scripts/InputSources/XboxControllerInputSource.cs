// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Xbox Controller support.
    /// <remarks>Only supports one connected device at a time.</remarks>
    /// <remarks>Make sure to enable the <see cref="HumanInterfaceDevice"/> capability before using.</remarks>
    /// </summary>
    public class XboxControllerInputSource : GamePadInputSource
    {
        private uint sourceId;
        private readonly Dictionary<uint, XboxControllerData> gamePadInputDatas = new Dictionary<uint, XboxControllerData>(0);
        private XboxControllerData controllerData;

        protected override void Update()
        {
            base.Update();

            if (gamePadInputDatas.Count != 1) { return; }

            controllerData.XboxLeftStickHorizontalAxis = Input.GetAxis("XBOX_LEFT_STICK_HORIZONTAL");
            controllerData.XboxLeftStickVerticalAxis = Input.GetAxis("XBOX_LEFT_STICK_VERTICAL");
            controllerData.XboxRightStickHorizontalAxis = Input.GetAxis("XBOX_RIGHT_STICK_HORIZONTAL");
            controllerData.XboxRightStickVerticalAxis = Input.GetAxis("XBOX_RIGHT_STICK_VERTICAL");
            controllerData.XboxDpadHorizontalAxis = Input.GetAxis("XBOX_DPAD_HORIZONTAL");
            controllerData.XboxDpadVerticalAxis = Input.GetAxis("XBOX_DPAD_VERTICAL");
            controllerData.XboxLeftTriggerAxis = Input.GetAxis("XBOX_LEFT_TRIGGER");
            controllerData.XboxRightTriggerAxis = Input.GetAxis("XBOX_RIGHT_TRIGGER");
            controllerData.XboxSharedTriggerAxis = Input.GetAxis("XBOX_TRIGGER_SHARED");

            controllerData.XboxA = Input.GetButton("XBOX_A");
            controllerData.XboxB = Input.GetButton("XBOX_B");
            controllerData.XboxX = Input.GetButton("XBOX_X");
            controllerData.XboxY = Input.GetButton("XBOX_Y");
            controllerData.XboxLeftBumper = Input.GetButton("XBOX_LEFT_BUMPER");
            controllerData.XboxRightBumper = Input.GetButton("XBOX_RIGHT_BUMPER");
            controllerData.XboxLeftStickClick = Input.GetButton("XBOX_LEFT_STICK_CLICK");
            controllerData.XboxRightStickClick = Input.GetButton("XBOX_RIGHT_STICK_CLICK");
            controllerData.XboxView = Input.GetButton("XBOX_VIEW");
            controllerData.XboxMenu = Input.GetButton("XBOX_MENU");

            InputManager.Instance.RaiseXboxInputUpdate(this, sourceId, controllerData);
        }

        protected override void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            if (LastDeviceList != null && (joystickNames.Length != LastDeviceUpdateCount || string.IsNullOrEmpty(joystickNames[0])))
            {
                foreach (var gamePadInputSource in gamePadInputDatas)
                {
                    InputManager.Instance.RaiseGamePadLost(this, gamePadInputSource.Key, LastDeviceList[gamePadInputSource.Key]);
                }

                gamePadInputDatas.Clear();
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]) || gamePadInputDatas.ContainsKey((uint)i)) { continue; }

                if (joystickNames[i].Contains("Xbox Bluetooth Gamepad"))
                {
                    sourceId = (uint)i;
                    controllerData = new XboxControllerData();
                    gamePadInputDatas.Add(sourceId, controllerData);
                    InputManager.Instance.RaiseGamePadDetected(this, sourceId, joystickNames[i]);
                }
                else
                {
                    Debug.LogWarning("Unimplemented Controller type Detected: " + joystickNames[i]);
                }
            }

            LastDeviceList = joystickNames;
            LastDeviceUpdateCount = joystickNames.Length;
        }
    }
}
