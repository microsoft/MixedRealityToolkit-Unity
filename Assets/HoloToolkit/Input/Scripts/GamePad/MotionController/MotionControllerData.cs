// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    public class MotionControllerData
    {
        public float LeftStickHorizontalAxis { get; set; }
        public float LeftStickVerticalAxis { get; set; }
        public float LeftTouchPadHorizontalAxis { get; set; }
        public float LeftTouchPadVerticalAxis { get; set; }
        public float RightStickHorizontalAxis { get; set; }
        public float RightStickVerticalAxis { get; set; }
        public float RightTouchPadHorizontalAxis { get; set; }
        public float RightTouchPadVerticalAxis { get; set; }
        public float LeftTriggerAxis { get; set; }
        public float RightTriggerAxis { get; set; }

        public bool LeftTrigger_Pressed { get; set; }
        public bool RightTrigger_Pressed { get; set; }
        public bool LeftTrigger_PartiallyPressed { get; set; }
        public bool RightTrigger_PartiallyPressed { get; set; }
        public bool LeftMenu_Pressed { get; set; }
        public bool RightMenu_Pressed { get; set; }
        public bool LeftGrip_Pressed { get; set; }
        public bool RightGrip_Pressed { get; set; }
        public bool LeftStick_Pressed { get; set; }
        public bool RightStick_Pressed { get; set; }
        public bool LeftTouchPad_Touched { get; set; }
        public bool RightTouchPad_Touched { get; set; }
        public bool LeftTouchPad_Pressed { get; set; }
        public bool RightTouchPad_Pressed { get; set; }

        public bool LeftTrigger_PartiallyPressed_Up { get; set; }
        public bool RightTrigger_PartiallyPressed_Up { get; set; }
        public bool LeftMenu_Up { get; set; }
        public bool RightMenu_Up { get; set; }
        public bool LeftGrip_Up { get; set; }
        public bool RightGrip_Up { get; set; }
        public bool LeftStick_Up { get; set; }
        public bool RightStick_Up { get; set; }
        public bool LeftTouchPadTouch_Up { get; set; }
        public bool RightTouchPadTouch_Up { get; set; }
        public bool LeftTouchPad_Up { get; set; }
        public bool RightTouchPad_Up { get; set; }

        public bool LeftTrigger_PartiallyPressed_Down { get; set; }
        public bool RightTrigger_PartiallyPressed_Down { get; set; }
        public bool LeftMenu_Down { get; set; }
        public bool RightMenu_Down { get; set; }
        public bool LeftGrip_Down { get; set; }
        public bool RightGrip_Down { get; set; }
        public bool LeftStick_Down { get; set; }
        public bool RightStick_Down { get; set; }
        public bool LeftTouchPadTouch_Down { get; set; }
        public bool RightTouchPadTouch_Down { get; set; }
        public bool LeftTouchPad_Down { get; set; }
        public bool RightTouchPad_Down { get; set; }
    }
}
