// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{

    public class MotionControllerHandlerBase : GamePadHandlerBase, IMotionControllerHandler
    {
        public virtual void OnMotionControllerInputUpdate(MotionControllerEventData eventData)
        {
        }

        protected static bool OnButton_Up(MotionControllerMappingTypes buttonType, MotionControllerEventData eventData)
        {
            switch (buttonType)
            {
                case MotionControllerMappingTypes.None:
                    return false;
                case MotionControllerMappingTypes.LeftTriggerPartiallyPressed:
                    return eventData.LeftTrigger_PartiallyPressed_Up;
                case MotionControllerMappingTypes.RightTriggerPartiallyPressed:
                    return eventData.RightTrigger_PartiallyPressed_Up;
                case MotionControllerMappingTypes.LeftMenu:
                    return eventData.LeftMenu_Up;
                case MotionControllerMappingTypes.RightMenu:
                    return eventData.RightMenu_Up;
                case MotionControllerMappingTypes.LeftGrip:
                    return eventData.LeftGrip_Up;
                case MotionControllerMappingTypes.RightGrip:
                    return eventData.RightGrip_Up;
                case MotionControllerMappingTypes.LeftStickClick:
                    return eventData.LeftStick_Up;
                case MotionControllerMappingTypes.RightStickClick:
                    return eventData.RightStick_Up;
                case MotionControllerMappingTypes.LeftTouchPadTouched:
                    return eventData.LeftTouchPadTouch_Up;
                case MotionControllerMappingTypes.RightTouchPadTouched:
                    return eventData.RightTouchPadTouch_Up;
                case MotionControllerMappingTypes.LeftTouchPadPressed:
                    return eventData.LeftTouchPad_Up;
                case MotionControllerMappingTypes.RightTouchPadPressed:
                    return eventData.RightTouchPad_Up;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }

        protected static bool OnButton_Pressed(MotionControllerMappingTypes buttonType, MotionControllerEventData eventData)
        {
            switch (buttonType)
            {
                case MotionControllerMappingTypes.None:
                    return false;
                case MotionControllerMappingTypes.LeftTriggerPressed:
                    return eventData.LeftTrigger_Pressed;
                case MotionControllerMappingTypes.LeftTriggerPartiallyPressed:
                    return eventData.LeftTrigger_PartiallyPressed;
                case MotionControllerMappingTypes.RightTriggerPressed:
                    return eventData.RightTrigger_Pressed;
                case MotionControllerMappingTypes.RightTriggerPartiallyPressed:
                    return eventData.RightTrigger_PartiallyPressed;
                case MotionControllerMappingTypes.LeftMenu:
                    return eventData.LeftMenu_Pressed;
                case MotionControllerMappingTypes.RightMenu:
                    return eventData.RightMenu_Pressed;
                case MotionControllerMappingTypes.LeftGrip:
                    return eventData.LeftGrip_Pressed;
                case MotionControllerMappingTypes.RightGrip:
                    return eventData.RightGrip_Pressed;
                case MotionControllerMappingTypes.LeftStickClick:
                    return eventData.LeftStick_Pressed;
                case MotionControllerMappingTypes.RightStickClick:
                    return eventData.RightStick_Pressed;
                case MotionControllerMappingTypes.LeftTouchPadTouched:
                    return eventData.LeftTouchPad_Touched;
                case MotionControllerMappingTypes.RightTouchPadTouched:
                    return eventData.RightTouchPad_Touched;
                case MotionControllerMappingTypes.LeftTouchPadPressed:
                    return eventData.LeftTouchPad_Pressed;
                case MotionControllerMappingTypes.RightTouchPadPressed:
                    return eventData.RightTouchPad_Pressed;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }

        protected static bool OnButton_Down(MotionControllerMappingTypes buttonType, MotionControllerEventData eventData)
        {
            switch (buttonType)
            {
                case MotionControllerMappingTypes.None:
                    return false;
                case MotionControllerMappingTypes.LeftTriggerPartiallyPressed:
                    return eventData.LeftTrigger_PartiallyPressed_Down;
                case MotionControllerMappingTypes.RightTriggerPartiallyPressed:
                    return eventData.RightTrigger_PartiallyPressed_Down;
                case MotionControllerMappingTypes.LeftMenu:
                    return eventData.LeftMenu_Down;
                case MotionControllerMappingTypes.RightMenu:
                    return eventData.RightMenu_Down;
                case MotionControllerMappingTypes.LeftGrip:
                    return eventData.LeftGrip_Down;
                case MotionControllerMappingTypes.RightGrip:
                    return eventData.RightGrip_Down;
                case MotionControllerMappingTypes.LeftStickClick:
                    return eventData.LeftStick_Down;
                case MotionControllerMappingTypes.RightStickClick:
                    return eventData.RightStick_Down;
                case MotionControllerMappingTypes.LeftTouchPadTouched:
                    return eventData.LeftTouchPadTouch_Down;
                case MotionControllerMappingTypes.RightTouchPadTouched:
                    return eventData.RightTouchPadTouch_Down;
                case MotionControllerMappingTypes.LeftTouchPadPressed:
                    return eventData.LeftTouchPad_Down;
                case MotionControllerMappingTypes.RightTouchPadPressed:
                    return eventData.RightTouchPad_Down;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }

        protected static float OnAxis(MotionControllerMappingTypes axisType, MotionControllerEventData eventData)
        {
            switch (axisType)
            {
                case MotionControllerMappingTypes.None:
                    return 0f;
                case MotionControllerMappingTypes.LeftStickHorizontal:
                    return eventData.LeftStickHorizontalAxis;
                case MotionControllerMappingTypes.LeftStickVertical:
                    return eventData.LeftStickVerticalAxis;
                case MotionControllerMappingTypes.LeftTouchPadHorizontal:
                    return eventData.LeftTouchPadHorizontalAxis;
                case MotionControllerMappingTypes.LeftTouchPadVertical:
                    return eventData.LeftTouchPadVerticalAxis;
                case MotionControllerMappingTypes.RightStickHorizontal:
                    return eventData.RightStickHorizontalAxis;
                case MotionControllerMappingTypes.RightStickVertical:
                    return eventData.RightStickVerticalAxis;
                case MotionControllerMappingTypes.RightTouchPadHorizontal:
                    return eventData.RightTouchPadHorizontalAxis;
                case MotionControllerMappingTypes.RightTouchPadVertical:
                    return eventData.RightTouchPadVerticalAxis;
                case MotionControllerMappingTypes.LeftTrigger:
                    return eventData.LeftTriggerAxis;
                case MotionControllerMappingTypes.RightTrigger:
                    return eventData.RightTriggerAxis;
                default:
                    throw new ArgumentOutOfRangeException("axisType", axisType, null);
            }
        }
    }
}
