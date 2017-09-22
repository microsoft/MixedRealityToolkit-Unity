// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Xbox Controller support.
    /// <remarks>Only supports one connected device at a time.</remarks>
    /// <remarks>Make sure to enable the <see cref="HumanInterfaceDevice"/> capability before using.</remarks>
    /// </summary>
    public class XboxControllerInputSource : GamePadInputSource
    {
        [Serializable]
        private class MappingEntry
        {
            public XboxControllerMappingTypes Type = 0;
            public string Value = string.Empty;
        }

        private const string XboxController = "Xbox Controller";
        private const string XboxOneForWindows = "Xbox One For Windows";
        private const string XboxBluetoothGamePad = "Xbox Bluetooth Gamepad";
        private const string XboxWirelessController = "Xbox Wireless Controller";

        private readonly Dictionary<uint, XboxControllerData> gamePadInputDatas = new Dictionary<uint, XboxControllerData>(0);

        private uint sourceId;
        private XboxControllerData controllerData;
        private StandaloneInputModule inputModule;
        private string previousHorizontalAxis;
        private string previousVerticalAxis;
        private string previousSubmitButton;
        private string previousCancelButton;
        private bool previousForceActiveState;

        public XboxControllerMappingTypes HorizontalAxis { get { return horizontalAxis; } }
        public XboxControllerMappingTypes VerticalAxis { get { return verticalAxis; } }
        public XboxControllerMappingTypes CancelButton { get { return cancelButton; } }
        public XboxControllerMappingTypes SubmitButton { get { return submitButton; } }

        [SerializeField]
        private XboxControllerMappingTypes horizontalAxis = XboxControllerMappingTypes.XboxDpadHorizontal;

        [SerializeField]
        private XboxControllerMappingTypes verticalAxis = XboxControllerMappingTypes.XboxDpadVertical;

        [SerializeField]
        private XboxControllerMappingTypes submitButton = XboxControllerMappingTypes.None;

        [SerializeField]
        private XboxControllerMappingTypes cancelButton = XboxControllerMappingTypes.None;

        [SerializeField]
        private MappingEntry[] mapping;

        protected virtual void Awake()
        {
            inputModule = FindObjectOfType<StandaloneInputModule>();

            if (inputModule == null)
            {
                Debug.LogError("Missing Standalone Input Module for Xbox Controller Source!\n" +
                               "Ensure you have an Event System in your scene.");
                return;
            }

            for (var i = 0; i < Enum.GetNames(typeof(XboxControllerMappingTypes)).Length; i++)
            {
                XboxControllerMapping.SetMapping((XboxControllerMappingTypes)i, mapping[i].Value);
            }

            previousForceActiveState = inputModule.forceModuleActive;

            if (horizontalAxis != XboxControllerMappingTypes.None)
            {
                previousHorizontalAxis = inputModule.horizontalAxis;
            }

            if (verticalAxis != XboxControllerMappingTypes.None)
            {
                previousVerticalAxis = inputModule.verticalAxis;
            }

            if (submitButton != XboxControllerMappingTypes.None)
            {
                previousSubmitButton = inputModule.submitButton;
            }

            if (cancelButton != XboxControllerMappingTypes.None)
            {
                previousCancelButton = inputModule.cancelButton;
            }
        }

        protected override void Update()
        {
            base.Update();

            // We will only register the first device we find.  Input is taken from joystick 1.
            if (gamePadInputDatas.Count != 1) { return; }

            controllerData.XboxLeftStickHorizontalAxis = Input.GetAxis(XboxControllerMapping.XboxLeftStickHorizontal);
            controllerData.XboxLeftStickVerticalAxis = Input.GetAxis(XboxControllerMapping.XboxLeftStickVertical);
            controllerData.XboxRightStickHorizontalAxis = Input.GetAxis(XboxControllerMapping.XboxRightStickHorizontal);
            controllerData.XboxRightStickVerticalAxis = Input.GetAxis(XboxControllerMapping.XboxRightStickVertical);
            controllerData.XboxDpadHorizontalAxis = Input.GetAxis(XboxControllerMapping.XboxDpadHorizontal);
            controllerData.XboxDpadVerticalAxis = Input.GetAxis(XboxControllerMapping.XboxDpadVertical);
            controllerData.XboxLeftTriggerAxis = Input.GetAxis(XboxControllerMapping.XboxLeftTrigger);
            controllerData.XboxRightTriggerAxis = Input.GetAxis(XboxControllerMapping.XboxRightTrigger);
            controllerData.XboxSharedTriggerAxis = Input.GetAxis(XboxControllerMapping.XboxSharedTrigger);

            controllerData.XboxA_Down = Input.GetButtonDown(XboxControllerMapping.XboxA);
            controllerData.XboxB_Down = Input.GetButtonDown(XboxControllerMapping.XboxB);
            controllerData.XboxX_Down = Input.GetButtonDown(XboxControllerMapping.XboxX);
            controllerData.XboxY_Down = Input.GetButtonDown(XboxControllerMapping.XboxY);
            controllerData.XboxView_Down = Input.GetButtonDown(XboxControllerMapping.XboxView);
            controllerData.XboxMenu_Down = Input.GetButtonDown(XboxControllerMapping.XboxMenu);
            controllerData.XboxLeftBumper_Down = Input.GetButtonDown(XboxControllerMapping.XboxLeftBumper);
            controllerData.XboxRightBumper_Down = Input.GetButtonDown(XboxControllerMapping.XboxRightBumper);
            controllerData.XboxLeftStick_Down = Input.GetButtonDown(XboxControllerMapping.XboxLeftStickClick);
            controllerData.XboxRightStick_Down = Input.GetButtonDown(XboxControllerMapping.XboxRightStickClick);

            controllerData.XboxA_Pressed = Input.GetButton(XboxControllerMapping.XboxA);
            controllerData.XboxB_Pressed = Input.GetButton(XboxControllerMapping.XboxB);
            controllerData.XboxX_Pressed = Input.GetButton(XboxControllerMapping.XboxX);
            controllerData.XboxY_Pressed = Input.GetButton(XboxControllerMapping.XboxY);
            controllerData.XboxView_Pressed = Input.GetButton(XboxControllerMapping.XboxView);
            controllerData.XboxMenu_Pressed = Input.GetButton(XboxControllerMapping.XboxMenu);
            controllerData.XboxLeftBumper_Pressed = Input.GetButton(XboxControllerMapping.XboxLeftBumper);
            controllerData.XboxRightBumper_Pressed = Input.GetButton(XboxControllerMapping.XboxRightBumper);
            controllerData.XboxLeftStick_Pressed = Input.GetButton(XboxControllerMapping.XboxLeftStickClick);
            controllerData.XboxRightStick_Pressed = Input.GetButton(XboxControllerMapping.XboxRightStickClick);

            controllerData.XboxA_Up = Input.GetButtonUp(XboxControllerMapping.XboxA);
            controllerData.XboxB_Up = Input.GetButtonUp(XboxControllerMapping.XboxB);
            controllerData.XboxX_Up = Input.GetButtonUp(XboxControllerMapping.XboxX);
            controllerData.XboxY_Up = Input.GetButtonUp(XboxControllerMapping.XboxY);
            controllerData.XboxView_Up = Input.GetButtonUp(XboxControllerMapping.XboxView);
            controllerData.XboxMenu_Up = Input.GetButtonUp(XboxControllerMapping.XboxMenu);
            controllerData.XboxLeftBumper_Up = Input.GetButtonUp(XboxControllerMapping.XboxLeftBumper);
            controllerData.XboxRightBumper_Up = Input.GetButtonUp(XboxControllerMapping.XboxRightBumper);
            controllerData.XboxLeftStick_Up = Input.GetButtonUp(XboxControllerMapping.XboxLeftStickClick);
            controllerData.XboxRightStick_Up = Input.GetButtonUp(XboxControllerMapping.XboxRightStickClick);

            InputManager.Instance.RaiseXboxInputUpdate(this, sourceId, controllerData);
        }

        protected override void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            bool devicesChanged = LastDeviceList == null;

            if (LastDeviceList != null && joystickNames.Length == LastDeviceList.Length)
            {
                for (int i = 0; i < LastDeviceList.Length; i++)
                {
                    if (!joystickNames[i].Equals(LastDeviceList[i]))
                    {
                        devicesChanged = true;
                        if (LastDeviceList == null)
                        {
                            LastDeviceList = joystickNames;
                        }
                    }
                }
            }

            if (LastDeviceList != null && devicesChanged)
            {
                foreach (var gamePadInputSource in gamePadInputDatas)
                {
                    // Reset our input module to it's previous state.
                    inputModule.forceModuleActive = previousForceActiveState;
                    inputModule.verticalAxis = previousVerticalAxis;
                    inputModule.horizontalAxis = previousHorizontalAxis;
                    inputModule.submitButton = previousSubmitButton;
                    inputModule.cancelButton = previousCancelButton;

                    InputManager.Instance.RaiseGamePadLost(this, gamePadInputSource.Key, LastDeviceList[gamePadInputSource.Key]);
                }

                gamePadInputDatas.Clear();
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]) || gamePadInputDatas.ContainsKey((uint)i)) { continue; }

                if (joystickNames[i].Contains(XboxController) ||
                    joystickNames[i].Contains(XboxOneForWindows) ||
                    joystickNames[i].Contains(XboxBluetoothGamePad) ||
                    joystickNames[i].Contains(XboxWirelessController))
                {
                    // We will only register the first device we find.  Input is taken from all joysticks.
                    if (gamePadInputDatas.Count != 0) { return; }

                    sourceId = (uint)i;
                    controllerData = new XboxControllerData();
                    gamePadInputDatas.Add(sourceId, controllerData);

                    // Setup the Input Module to use our custom axis settings.
                    inputModule.forceModuleActive = true;
                    inputModule.verticalAxis = XboxControllerMapping.GetMapping(verticalAxis);
                    inputModule.horizontalAxis = XboxControllerMapping.GetMapping(horizontalAxis);
                    inputModule.submitButton = XboxControllerMapping.GetMapping(submitButton);
                    inputModule.cancelButton = XboxControllerMapping.GetMapping(cancelButton);

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
