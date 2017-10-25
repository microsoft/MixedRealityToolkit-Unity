// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{

    public class MotionControllerEventData : GamePadEventData
    {
        public float LeftStickHorizontalAxis { get; private set; }
        public float LeftStickVerticalAxis { get; private set; }
        public float LeftTouchPadHorizontalAxis { get; private set; }
        public float LeftTouchPadVerticalAxis { get; private set; }
        public float RightStickHorizontalAxis { get; private set; }
        public float RightStickVerticalAxis { get; private set; }
        public float RightTouchPadHorizontalAxis { get; private set; }
        public float RightTouchPadVerticalAxis { get; private set; }
        public float LeftTriggerAxis { get; private set; }
        public float RightTriggerAxis { get; private set; }

        public bool LeftTrigger_Pressed { get; private set; }
        public bool RightTrigger_Pressed { get; private set; }
        public bool LeftTrigger_PartiallyPressed { get; private set; }
        public bool RightTrigger_PartiallyPressed { get; private set; }
        public bool LeftMenu_Pressed { get; private set; }
        public bool RightMenu_Pressed { get; private set; }
        public bool LeftGrip_Pressed { get; private set; }
        public bool RightGrip_Pressed { get; private set; }
        public bool LeftStick_Pressed { get; private set; }
        public bool RightStick_Pressed { get; private set; }
        public bool LeftTouchPad_Touched { get; private set; }
        public bool RightTouchPad_Touched { get; private set; }
        public bool LeftTouchPad_Pressed { get; private set; }
        public bool RightTouchPad_Pressed { get; private set; }

        public bool LeftTrigger_PartiallyPressed_Up { get; private set; }
        public bool RightTrigger_PartiallyPressed_Up { get; private set; }
        public bool LeftMenu_Up { get; private set; }
        public bool RightMenu_Up { get; private set; }
        public bool LeftGrip_Up { get; private set; }
        public bool RightGrip_Up { get; private set; }
        public bool LeftStick_Up { get; private set; }
        public bool RightStick_Up { get; private set; }
        public bool LeftTouchPadTouch_Up { get; private set; }
        public bool RightTouchPadTouch_Up { get; private set; }
        public bool LeftTouchPad_Up { get; private set; }
        public bool RightTouchPad_Up { get; private set; }

        public bool LeftTrigger_PartiallyPressed_Down { get; private set; }
        public bool RightTrigger_PartiallyPressed_Down { get; private set; }
        public bool LeftMenu_Down { get; private set; }
        public bool RightMenu_Down { get; private set; }
        public bool LeftGrip_Down { get; private set; }
        public bool RightGrip_Down { get; private set; }
        public bool LeftStick_Down { get; private set; }
        public bool RightStick_Down { get; private set; }
        public bool LeftTouchPadTouch_Down { get; private set; }
        public bool RightTouchPadTouch_Down { get; private set; }
        public bool LeftTouchPad_Down { get; private set; }
        public bool RightTouchPad_Down { get; private set; }

        public MotionControllerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, MotionControllerData inputData, object tag = null)
        {
            BaseInitialize(inputSource, sourceId, tag);

            LeftStickHorizontalAxis = inputData.LeftStickHorizontalAxis;
            LeftStickVerticalAxis = inputData.LeftStickVerticalAxis;
            LeftTouchPadHorizontalAxis = inputData.LeftTouchPadHorizontalAxis;
            LeftTouchPadVerticalAxis = inputData.LeftTouchPadVerticalAxis;
            RightStickHorizontalAxis = inputData.RightStickHorizontalAxis;
            RightStickVerticalAxis = inputData.RightStickVerticalAxis;
            RightTouchPadHorizontalAxis = inputData.RightTouchPadHorizontalAxis;
            RightTouchPadVerticalAxis = inputData.RightTouchPadVerticalAxis;
            LeftTriggerAxis = inputData.LeftTriggerAxis;
            RightTriggerAxis = inputData.RightTriggerAxis;

            LeftTrigger_Pressed = inputData.LeftTrigger_Pressed;
            RightTrigger_Pressed = inputData.RightTrigger_Pressed;
            LeftTrigger_PartiallyPressed = inputData.LeftTrigger_PartiallyPressed;
            RightTrigger_PartiallyPressed = inputData.RightTrigger_PartiallyPressed;
            LeftMenu_Pressed = inputData.LeftMenu_Pressed;
            RightMenu_Pressed = inputData.RightMenu_Pressed;
            LeftGrip_Pressed = inputData.LeftGrip_Pressed;
            RightGrip_Pressed = inputData.RightGrip_Pressed;
            LeftStick_Pressed = inputData.LeftStick_Pressed;
            RightStick_Pressed = inputData.RightStick_Pressed;
            LeftTouchPad_Touched = inputData.LeftTouchPad_Touched;
            RightTouchPad_Touched = inputData.RightTouchPad_Touched;
            LeftTouchPad_Pressed = inputData.LeftTouchPad_Pressed;
            RightTouchPad_Pressed = inputData.RightTouchPad_Pressed;

            LeftTrigger_PartiallyPressed_Up = inputData.LeftTrigger_PartiallyPressed_Up;
            RightTrigger_PartiallyPressed_Up = inputData.RightTrigger_PartiallyPressed_Up;
            LeftMenu_Up = inputData.LeftMenu_Up;
            RightMenu_Up = inputData.RightMenu_Up;
            LeftGrip_Up = inputData.LeftGrip_Up;
            RightGrip_Up = inputData.RightGrip_Up;
            LeftStick_Up = inputData.LeftStick_Up;
            RightStick_Up = inputData.RightStick_Up;
            LeftTouchPadTouch_Up = inputData.LeftTouchPadTouch_Up;
            RightTouchPadTouch_Up = inputData.RightTouchPadTouch_Up;
            LeftTouchPad_Up = inputData.LeftTouchPad_Up;
            RightTouchPad_Up = inputData.RightTouchPad_Up;

            LeftTrigger_PartiallyPressed_Down = inputData.LeftTrigger_PartiallyPressed_Down;
            RightTrigger_PartiallyPressed_Down = inputData.RightTrigger_PartiallyPressed_Down;
            LeftMenu_Down = inputData.LeftMenu_Down;
            RightMenu_Down = inputData.RightMenu_Down;
            LeftGrip_Down = inputData.LeftGrip_Down;
            RightGrip_Down = inputData.RightGrip_Down;
            LeftStick_Down = inputData.LeftStick_Down;
            RightStick_Down = inputData.RightStick_Down;
            LeftTouchPadTouch_Down = inputData.LeftTouchPadTouch_Down;
            RightTouchPadTouch_Down = inputData.RightTouchPadTouch_Down;
            LeftTouchPad_Down = inputData.LeftTouchPad_Down;
            RightTouchPad_Down = inputData.RightTouchPad_Down;
        }
    }
}
