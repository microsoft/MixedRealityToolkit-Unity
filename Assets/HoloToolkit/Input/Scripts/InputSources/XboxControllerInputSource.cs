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

            controllerData.XboxA_Down = Input.GetButtonDown("XBOX_A");
            controllerData.XboxB_Down = Input.GetButtonDown("XBOX_B");
            controllerData.XboxX_Down = Input.GetButtonDown("XBOX_X");
            controllerData.XboxY_Down = Input.GetButtonDown("XBOX_Y");
            controllerData.XboxLeftBumper_Down = Input.GetButtonDown("XBOX_LEFT_BUMPER");
            controllerData.XboxRightBumper_Down = Input.GetButtonDown("XBOX_RIGHT_BUMPER");
            controllerData.XboxLeftStick_Down = Input.GetButtonDown("XBOX_LEFT_STICK_CLICK");
            controllerData.XboxRightStick_Down = Input.GetButtonDown("XBOX_RIGHT_STICK_CLICK");
            controllerData.XboxView_Down = Input.GetButtonDown("XBOX_VIEW");
            controllerData.XboxMenu_Down = Input.GetButtonDown("XBOX_MENU");

            controllerData.XboxA_Pressed = Input.GetButton("XBOX_A");
            controllerData.XboxB_Pressed = Input.GetButton("XBOX_B");
            controllerData.XboxX_Pressed = Input.GetButton("XBOX_X");
            controllerData.XboxY_Pressed = Input.GetButton("XBOX_Y");
            controllerData.XboxLeftBumper_Pressed = Input.GetButton("XBOX_LEFT_BUMPER");
            controllerData.XboxRightBumper_Pressed = Input.GetButton("XBOX_RIGHT_BUMPER");
            controllerData.XboxLeftStick_Pressed = Input.GetButton("XBOX_LEFT_STICK_CLICK");
            controllerData.XboxRightStick_Pressed = Input.GetButton("XBOX_RIGHT_STICK_CLICK");
            controllerData.XboxView_Pressed = Input.GetButton("XBOX_VIEW");
            controllerData.XboxMenu_Pressed = Input.GetButton("XBOX_MENU");

            controllerData.XboxA_Up = Input.GetButtonUp("XBOX_A");
            controllerData.XboxB_Up = Input.GetButtonUp("XBOX_B");
            controllerData.XboxX_Up = Input.GetButtonUp("XBOX_X");
            controllerData.XboxY_Up = Input.GetButtonUp("XBOX_Y");
            controllerData.XboxLeftBumper_Up = Input.GetButtonUp("XBOX_LEFT_BUMPER");
            controllerData.XboxRightBumper_Up = Input.GetButtonUp("XBOX_RIGHT_BUMPER");
            controllerData.XboxLeftStick_Up = Input.GetButtonUp("XBOX_LEFT_STICK_CLICK");
            controllerData.XboxRightStick_Up = Input.GetButtonUp("XBOX_RIGHT_STICK_CLICK");
            controllerData.XboxView_Up = Input.GetButtonUp("XBOX_VIEW");
            controllerData.XboxMenu_Up = Input.GetButtonUp("XBOX_MENU");

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
