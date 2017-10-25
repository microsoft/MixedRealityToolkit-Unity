// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class MotionControllerInputSource : GamePadInputSource
{
    [Serializable]
    private class MappingEntry
    {
        public MotionControllerMappingTypes Type = MotionControllerMappingTypes.None;
        public string Value = string.Empty;
    }

    private const string MotionControllerRight = "";
    private const string MotionControllerLeft = "";

    private readonly Dictionary<uint, MotionControllerData> gamePadInputDatas = new Dictionary<uint, MotionControllerData>(0);

    private MotionControllerData controllerData;



    public MotionControllerMappingTypes HorizontalAxis { get { return horizontalAxis; } }
    public MotionControllerMappingTypes VerticalAxis { get { return verticalAxis; } }
    public MotionControllerMappingTypes CancelButton { get { return cancelButton; } }
    public MotionControllerMappingTypes SubmitButton { get { return submitButton; } }

    [SerializeField]
    private MotionControllerMappingTypes horizontalAxis = MotionControllerMappingTypes.RightTouchPadHorizontal;

    [SerializeField]
    private MotionControllerMappingTypes verticalAxis = MotionControllerMappingTypes.RightTouchPadVertical;

    [SerializeField]
    private MotionControllerMappingTypes submitButton = MotionControllerMappingTypes.RightTriggerPressed;

    [SerializeField]
    private MotionControllerMappingTypes cancelButton = MotionControllerMappingTypes.RightMenu;

    [SerializeField]
    private MappingEntry[] mapping;

    protected override void Awake()
    {
        base.Awake();

        if (mapping != null)
        {
            for (var i = 0; i < Enum.GetNames(typeof(MotionControllerMappingTypes)).Length; i++)
            {
                MotionControllerMapping.SetMapping((MotionControllerMappingTypes)i, mapping[i].Value);
            }
        }

        PreviousForceActiveState = InputModule.forceModuleActive;

        if (horizontalAxis != MotionControllerMappingTypes.None)
        {
            PreviousHorizontalAxis = InputModule.horizontalAxis;
        }

        if (verticalAxis != MotionControllerMappingTypes.None)
        {
            PreviousVerticalAxis = InputModule.verticalAxis;
        }

        if (submitButton != MotionControllerMappingTypes.None)
        {
            PreviousSubmitButton = InputModule.submitButton;
        }

        if (cancelButton != MotionControllerMappingTypes.None)
        {
            PreviousCancelButton = InputModule.cancelButton;
        }
    }

    protected override void Update()
    {
        base.Update();

        controllerData.LeftStickHorizontalAxis = Input.GetAxis(MotionControllerMapping.LeftStickHorizontal);
        controllerData.LeftStickVerticalAxis = Input.GetAxis(MotionControllerMapping.LeftStickVertical);
        controllerData.LeftTouchPadHorizontalAxis = Input.GetAxis(MotionControllerMapping.LeftTouchPadHorizontal);
        controllerData.LeftTouchPadVerticalAxis = Input.GetAxis(MotionControllerMapping.LeftTouchPadVertical);
        controllerData.RightStickHorizontalAxis = Input.GetAxis(MotionControllerMapping.RightStickHorizontal);
        controllerData.RightStickVerticalAxis = Input.GetAxis(MotionControllerMapping.RightStickVertical);
        controllerData.RightTouchPadHorizontalAxis = Input.GetAxis(MotionControllerMapping.RightTouchPadHorizontal);
        controllerData.RightTouchPadVerticalAxis = Input.GetAxis(MotionControllerMapping.RightTouchPadVertical);
        controllerData.LeftTriggerAxis = Input.GetAxis(MotionControllerMapping.LeftTrigger);
        controllerData.RightTriggerAxis = Input.GetAxis(MotionControllerMapping.RightTrigger);

        controllerData.LeftTrigger_PartiallyPressed_Down = Input.GetButtonDown(MotionControllerMapping.LeftTriggerPartiallyPressed);
        controllerData.RightTrigger_PartiallyPressed_Down = Input.GetButtonDown(MotionControllerMapping.RightTriggerPartiallyPressed);
        controllerData.LeftMenu_Down = Input.GetButtonDown(MotionControllerMapping.LeftMenu);
        controllerData.RightMenu_Down = Input.GetButtonDown(MotionControllerMapping.RightMenu);
        controllerData.LeftGrip_Down = Input.GetButtonDown(MotionControllerMapping.LeftGrip);
        controllerData.RightGrip_Down = Input.GetButtonDown(MotionControllerMapping.RightGrip);
        controllerData.LeftStick_Down = Input.GetButtonDown(MotionControllerMapping.LeftStickClick);
        controllerData.RightStick_Down = Input.GetButtonDown(MotionControllerMapping.RightStickClick);
        controllerData.LeftTouchPadTouch_Down = Input.GetButtonDown(MotionControllerMapping.LeftTouchPadTouched);
        controllerData.RightTouchPadTouch_Down = Input.GetButtonDown(MotionControllerMapping.RightTouchPadTouched);
        controllerData.LeftTouchPad_Down = Input.GetButtonDown(MotionControllerMapping.LeftTouchPadPressed);
        controllerData.RightTouchPad_Down = Input.GetButtonDown(MotionControllerMapping.RightTouchPadPressed);

        controllerData.LeftTrigger_Pressed = !(controllerData.LeftTriggerAxis < 1.0f);
        controllerData.RightTrigger_Pressed = !(controllerData.RightTriggerAxis < 1.0f);
        controllerData.LeftTrigger_PartiallyPressed = Input.GetButton(MotionControllerMapping.LeftTriggerPartiallyPressed);
        controllerData.RightTrigger_PartiallyPressed = Input.GetButton(MotionControllerMapping.RightTriggerPartiallyPressed);
        controllerData.LeftMenu_Pressed = Input.GetButton(MotionControllerMapping.LeftMenu);
        controllerData.RightMenu_Pressed = Input.GetButton(MotionControllerMapping.RightMenu);
        controllerData.LeftGrip_Pressed = Input.GetButton(MotionControllerMapping.LeftGrip);
        controllerData.RightGrip_Pressed = Input.GetButton(MotionControllerMapping.RightGrip);
        controllerData.LeftStick_Pressed = Input.GetButton(MotionControllerMapping.LeftStickClick);
        controllerData.RightStick_Pressed = Input.GetButton(MotionControllerMapping.RightStickClick);
        controllerData.LeftTouchPad_Touched = Input.GetButton(MotionControllerMapping.LeftTouchPadTouched);
        controllerData.RightTouchPad_Touched = Input.GetButton(MotionControllerMapping.RightTouchPadTouched);
        controllerData.LeftTouchPad_Pressed = Input.GetButton(MotionControllerMapping.LeftTouchPadPressed);
        controllerData.RightTouchPad_Pressed = Input.GetButton(MotionControllerMapping.RightTouchPadPressed);

        controllerData.LeftTrigger_PartiallyPressed_Up = Input.GetButtonUp(MotionControllerMapping.LeftTriggerPartiallyPressed);
        controllerData.RightTrigger_PartiallyPressed_Up = Input.GetButtonUp(MotionControllerMapping.RightTriggerPartiallyPressed);
        controllerData.LeftMenu_Up = Input.GetButtonUp(MotionControllerMapping.LeftMenu);
        controllerData.RightMenu_Up = Input.GetButtonUp(MotionControllerMapping.RightMenu);
        controllerData.LeftGrip_Up = Input.GetButtonUp(MotionControllerMapping.LeftGrip);
        controllerData.RightGrip_Up = Input.GetButtonUp(MotionControllerMapping.RightGrip);
        controllerData.LeftStick_Up = Input.GetButtonUp(MotionControllerMapping.LeftStickClick);
        controllerData.RightStick_Up = Input.GetButtonUp(MotionControllerMapping.RightStickClick);
        controllerData.LeftTouchPadTouch_Up = Input.GetButtonUp(MotionControllerMapping.LeftTouchPadTouched);
        controllerData.RightTouchPadTouch_Up = Input.GetButtonUp(MotionControllerMapping.RightTouchPadTouched);
        controllerData.LeftTouchPad_Up = Input.GetButtonUp(MotionControllerMapping.LeftTouchPadPressed);
        controllerData.RightTouchPad_Up = Input.GetButtonUp(MotionControllerMapping.RightTouchPadPressed);

        InputManager.Instance.RaiseMotionControllerInputUpdate(this, SourceId, controllerData);
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
                InputModule.forceModuleActive = PreviousForceActiveState;
                InputModule.verticalAxis = PreviousVerticalAxis;
                InputModule.horizontalAxis = PreviousHorizontalAxis;
                InputModule.submitButton = PreviousSubmitButton;
                InputModule.cancelButton = PreviousCancelButton;

                InputManager.Instance.RaiseGamePadLost(this, gamePadInputSource.Key, LastDeviceList[gamePadInputSource.Key]);
            }

            gamePadInputDatas.Clear();
        }

        for (var i = 0; i < joystickNames.Length; i++)
        {
            if (string.IsNullOrEmpty(joystickNames[i]) || gamePadInputDatas.ContainsKey((uint)i)) { continue; }

            if (joystickNames[i].Contains(MotionControllerRight) ||
                joystickNames[i].Contains(MotionControllerLeft))
            {
                SourceId = (uint)i;
                controllerData = new MotionControllerData();
                gamePadInputDatas.Add(SourceId, controllerData);

                // Setup the Input Module to use our custom axis settings.
                InputModule.forceModuleActive = true;
                InputModule.verticalAxis = MotionControllerMapping.GetMapping(verticalAxis);
                InputModule.horizontalAxis = MotionControllerMapping.GetMapping(horizontalAxis);
                InputModule.submitButton = MotionControllerMapping.GetMapping(submitButton);
                InputModule.cancelButton = MotionControllerMapping.GetMapping(cancelButton);

                InputManager.Instance.RaiseGamePadDetected(this, SourceId, joystickNames[i]);
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
