// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerEventData : SourceStateEventData
    {
        public string GamePadName { get; private set; }

        public float XboxLeftStickHorizontalAxis { get; private set; }
        public float XboxLeftStickVerticalAxis { get; private set; }
        public float XboxRightStickHorizontalAxis { get; private set; }
        public float XboxRightStickVerticalAxis { get; private set; }
        public float XboxDpadHorizontalAxis { get; private set; }
        public float XboxDpadVerticalAxis { get; private set; }
        public float XboxLeftTriggerAxis { get; private set; }
        public float XboxRightTriggerAxis { get; private set; }
        public float XboxSharedTriggerAxis { get; private set; }

        public bool XboxA_Pressed { get; private set; }
        public bool XboxB_Pressed { get; private set; }
        public bool XboxX_Pressed { get; private set; }
        public bool XboxY_Pressed { get; private set; }
        public bool XboxLeftBumper_Pressed { get; private set; }
        public bool XboxRightBumper_Pressed { get; private set; }
        public bool XboxLeftStick_Pressed { get; private set; }
        public bool XboxRightStick_Pressed { get; private set; }
        public bool XboxView_Pressed { get; private set; }
        public bool XboxMenu_Pressed { get; private set; }

        public bool XboxA_Up { get; private set; }
        public bool XboxB_Up { get; private set; }
        public bool XboxX_Up { get; private set; }
        public bool XboxY_Up { get; private set; }
        public bool XboxLeftBumper_Up { get; private set; }
        public bool XboxRightBumper_Up { get; private set; }
        public bool XboxLeftStick_Up { get; private set; }
        public bool XboxRightStick_Up { get; private set; }
        public bool XboxView_Up { get; private set; }
        public bool XboxMenu_Up { get; private set; }

        public bool XboxA_Down { get; private set; }
        public bool XboxB_Down { get; private set; }
        public bool XboxX_Down { get; private set; }
        public bool XboxY_Down { get; private set; }
        public bool XboxLeftBumper_Down { get; private set; }
        public bool XboxRightBumper_Down { get; private set; }
        public bool XboxLeftStick_Down { get; private set; }
        public bool XboxRightStick_Down { get; private set; }
        public bool XboxView_Down { get; private set; }
        public bool XboxMenu_Down { get; private set; }

        public XboxControllerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, XboxControllerData inputData, object tag = null)
        {
            BaseInitialize(inputSource, sourceId, tag);

            GamePadName = inputData.GamePadName;

            XboxLeftStickHorizontalAxis = inputData.XboxLeftStickHorizontalAxis;
            XboxLeftStickVerticalAxis = inputData.XboxLeftStickVerticalAxis;
            XboxRightStickHorizontalAxis = inputData.XboxRightStickHorizontalAxis;
            XboxRightStickVerticalAxis = inputData.XboxRightStickVerticalAxis;
            XboxDpadHorizontalAxis = inputData.XboxDpadHorizontalAxis;
            XboxDpadVerticalAxis = inputData.XboxDpadVerticalAxis;
            XboxLeftTriggerAxis = inputData.XboxLeftTriggerAxis;
            XboxRightTriggerAxis = inputData.XboxRightTriggerAxis;
            XboxSharedTriggerAxis = inputData.XboxSharedTriggerAxis;

            XboxA_Down = inputData.XboxA_Down;
            XboxB_Down = inputData.XboxB_Down;
            XboxX_Down = inputData.XboxX_Down;
            XboxY_Down = inputData.XboxY_Down;
            XboxLeftBumper_Down = inputData.XboxLeftBumper_Down;
            XboxRightBumper_Down = inputData.XboxRightBumper_Down;
            XboxLeftStick_Down = inputData.XboxLeftStick_Down;
            XboxRightStick_Down = inputData.XboxRightStick_Down;
            XboxView_Down = inputData.XboxView_Down;
            XboxMenu_Down = inputData.XboxMenu_Down;

            XboxA_Pressed = inputData.XboxA_Pressed;
            XboxB_Pressed = inputData.XboxB_Pressed;
            XboxX_Pressed = inputData.XboxX_Pressed;
            XboxY_Pressed = inputData.XboxY_Pressed;
            XboxLeftBumper_Pressed = inputData.XboxLeftBumper_Pressed;
            XboxRightBumper_Pressed = inputData.XboxRightBumper_Pressed;
            XboxLeftStick_Pressed = inputData.XboxLeftStick_Pressed;
            XboxRightStick_Pressed = inputData.XboxRightStick_Pressed;
            XboxView_Pressed = inputData.XboxView_Pressed;
            XboxMenu_Pressed = inputData.XboxMenu_Pressed;

            XboxA_Up = inputData.XboxA_Up;
            XboxB_Up = inputData.XboxB_Up;
            XboxX_Up = inputData.XboxX_Up;
            XboxY_Up = inputData.XboxY_Up;
            XboxLeftBumper_Up = inputData.XboxLeftBumper_Up;
            XboxRightBumper_Up = inputData.XboxRightBumper_Up;
            XboxLeftStick_Up = inputData.XboxLeftStick_Up;
            XboxRightStick_Up = inputData.XboxRightStick_Up;
            XboxView_Up = inputData.XboxView_Up;
            XboxMenu_Up = inputData.XboxMenu_Up;
        }
    }
}
