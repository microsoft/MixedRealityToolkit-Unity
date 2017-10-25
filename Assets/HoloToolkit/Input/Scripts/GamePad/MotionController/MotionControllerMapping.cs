// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
{
    public class MotionControllerMapping
    {
        public static string LeftStickHorizontal { get; private set; }
        public static string LeftStickVertical { get; private set; }
        public static string LeftTouchPadHorizontal { get; private set; }
        public static string LeftTouchPadVertical { get; private set; }
        public static string RightStickHorizontal { get; private set; }
        public static string RightStickVertical { get; private set; }
        public static string RightTouchPadHorizontal { get; private set; }
        public static string RightTouchPadVertical { get; private set; }
        public static string LeftTrigger { get; private set; }
        public static string LeftTriggerPartiallyPressed { get; private set; }
        public static string RightTrigger { get; private set; }
        public static string RightTriggerPartiallyPressed { get; private set; }
        public static string LeftMenu { get; private set; }
        public static string RightMenu { get; private set; }
        public static string LeftGrip { get; private set; }
        public static string RightGrip { get; private set; }
        public static string LeftStickClick { get; private set; }
        public static string RightStickClick { get; private set; }
        public static string LeftTouchPadTouched { get; private set; }
        public static string RightTouchPadTouched { get; private set; }
        public static string LeftTouchPadPressed { get; private set; }
        public static string RightTouchPadPressed { get; private set; }

        private const string CONTROLLER_LEFT_STICK_HORIZONTAL = "CONTROLLER_LEFT_STICK_HORIZONTAL";
        private const string CONTROLLER_LEFT_STICK_VERTICAL = "CONTROLLER_LEFT_STICK_VERTICAL";
        private const string CONTROLLER_LEFT_TOUCHPAD_HORIZONTAL = "CONTROLLER_LEFT_TOUCHPAD_HORIZONTAL";
        private const string CONTROLLER_LEFT_TOUCHPAD_VERTICAL = "CONTROLLER_LEFT_TOUCHPAD_VERTICAL";
        private const string CONTROLLER_RIGHT_STICK_HORIZONTAL = "CONTROLLER_RIGHT_STICK_HORIZONTAL";
        private const string CONTROLLER_RIGHT_STICK_VERTICAL = "CONTROLLER_RIGHT_STICK_VERTICAL";
        private const string CONTROLLER_RIGHT_TOUCHPAD_HORIZONTAL = "CONTROLLER_RIGHT_TOUCHPAD_HORIZONTAL";
        private const string CONTROLLER_RIGHT_TOUCHPAD_VERTICAL = "CONTROLLER_RIGHT_TOUCHPAD_VERTICAL";
        private const string CONTROLLER_LEFT_TRIGGER = "CONTROLLER_LEFT_TRIGGER";
        private const string CONTROLLER_LEFT_TRIGGER_PARTIAL_PRESSED = "CONTROLLER_LEFT_TRIGGER_PARTIAL_PRESSED";
        private const string CONTROLLER_RIGHT_TRIGGER = "CONTROLLER_RIGHT_TRIGGER";
        private const string CONTROLLER_RIGHT_TRIGGER_PARTIAL_PRESSED = "CONTROLLER_RIGHT_TRIGGER_PARTIAL_PRESSED";
        private const string CONTROLLER_LEFT_MENU = "CONTROLLER_LEFT_MENU";
        private const string CONTROLLER_RIGHT_MENU = "CONTROLLER_RIGHT_MENU";
        private const string CONTROLLER_LEFT_BUMPER_OR_GRIP = "CONTROLLER_LEFT_BUMPER_OR_GRIP";
        private const string CONTROLLER_RIGHT_BUMPER_OR_GRIP = "CONTROLLER_RIGHT_BUMPER_OR_GRIP";
        private const string CONTROLLER_LEFT_STICK_CLICK = "CONTROLLER_LEFT_STICK_CLICK";
        private const string CONTROLLER_RIGHT_STICK_CLICK = "CONTROLLER_RIGHT_STICK_CLICK";
        private const string CONTROLLER_LEFT_TOUCHPAD_TOUCHED = "CONTROLLER_LEFT_TOUCHPAD_TOUCHED";
        private const string CONTROLLER_RIGHT_TOUCHPAD_TOUCHED = "CONTROLLER_RIGHT_TOUCHPAD_TOUCHED";
        private const string CONTROLLER_LEFT_TOUCHPAD_PRESSED = "CONTROLLER_LEFT_TOUCHPAD_PRESSED";
        private const string CONTROLLER_RIGHT_TOUCHPAD_PRESSED = "CONTROLLER_RIGHT_TOUCHPAD_PRESSED";

        public static string GetMapping(MotionControllerMappingTypes type)
        {
            switch (type)
            {
                case MotionControllerMappingTypes.None:
                    return string.Empty;
                case MotionControllerMappingTypes.LeftStickHorizontal:
                    return LeftStickHorizontal;
                case MotionControllerMappingTypes.LeftStickVertical:
                    return LeftStickVertical;
                case MotionControllerMappingTypes.LeftTouchPadHorizontal:
                    return LeftTouchPadHorizontal;
                case MotionControllerMappingTypes.LeftTouchPadVertical:
                    return LeftTouchPadVertical;
                case MotionControllerMappingTypes.RightStickHorizontal:
                    return RightStickHorizontal;
                case MotionControllerMappingTypes.RightStickVertical:
                    return RightStickVertical;
                case MotionControllerMappingTypes.RightTouchPadHorizontal:
                    return RightTouchPadHorizontal;
                case MotionControllerMappingTypes.RightTouchPadVertical:
                    return RightTouchPadVertical;
                case MotionControllerMappingTypes.LeftTrigger:
                    return LeftTrigger;
                case MotionControllerMappingTypes.LeftTriggerPressed:
                    return LeftTrigger;
                case MotionControllerMappingTypes.LeftTriggerPartiallyPressed:
                    return LeftTriggerPartiallyPressed;
                case MotionControllerMappingTypes.RightTrigger:
                    return RightTrigger;
                case MotionControllerMappingTypes.RightTriggerPressed:
                    return RightTrigger;
                case MotionControllerMappingTypes.RightTriggerPartiallyPressed:
                    return RightTriggerPartiallyPressed;
                case MotionControllerMappingTypes.LeftMenu:
                    return LeftMenu;
                case MotionControllerMappingTypes.RightMenu:
                    return RightMenu;
                case MotionControllerMappingTypes.LeftGrip:
                    return LeftGrip;
                case MotionControllerMappingTypes.RightGrip:
                    return RightGrip;
                case MotionControllerMappingTypes.LeftStickClick:
                    return LeftStickClick;
                case MotionControllerMappingTypes.RightStickClick:
                    return RightStickClick;
                case MotionControllerMappingTypes.LeftTouchPadTouched:
                    return LeftTouchPadTouched;
                case MotionControllerMappingTypes.RightTouchPadTouched:
                    return RightTouchPadTouched;
                case MotionControllerMappingTypes.LeftTouchPadPressed:
                    return LeftTouchPadPressed;
                case MotionControllerMappingTypes.RightTouchPadPressed:
                    return RightTouchPadPressed;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static void SetMapping(MotionControllerMappingTypes type, string value)
        {
            switch (type)
            {
                case MotionControllerMappingTypes.None:
                case MotionControllerMappingTypes.LeftTriggerPressed:
                case MotionControllerMappingTypes.RightTriggerPressed:
                    break;
                case MotionControllerMappingTypes.LeftStickHorizontal:
                    LeftStickHorizontal = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_STICK_HORIZONTAL : value;
                    break;
                case MotionControllerMappingTypes.LeftStickVertical:
                    LeftStickVertical = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_STICK_VERTICAL : value;
                    break;
                case MotionControllerMappingTypes.LeftTouchPadHorizontal:
                    LeftTouchPadHorizontal = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TOUCHPAD_HORIZONTAL : value;
                    break;
                case MotionControllerMappingTypes.LeftTouchPadVertical:
                    LeftTouchPadVertical = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TOUCHPAD_VERTICAL : value;
                    break;
                case MotionControllerMappingTypes.RightStickHorizontal:
                    RightStickHorizontal = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_STICK_HORIZONTAL : value;
                    break;
                case MotionControllerMappingTypes.RightStickVertical:
                    RightStickVertical = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_STICK_VERTICAL : value;
                    break;
                case MotionControllerMappingTypes.RightTouchPadHorizontal:
                    RightTouchPadHorizontal = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TOUCHPAD_HORIZONTAL : value;
                    break;
                case MotionControllerMappingTypes.RightTouchPadVertical:
                    RightTouchPadVertical = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TOUCHPAD_VERTICAL : value;
                    break;
                case MotionControllerMappingTypes.LeftTrigger:
                    LeftTrigger = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TRIGGER : value;
                    break;
                case MotionControllerMappingTypes.LeftTriggerPartiallyPressed:
                    LeftTriggerPartiallyPressed = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TRIGGER_PARTIAL_PRESSED : value;
                    break;
                case MotionControllerMappingTypes.RightTrigger:
                    RightTrigger = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TRIGGER : value;
                    break;
                case MotionControllerMappingTypes.RightTriggerPartiallyPressed:
                    RightTriggerPartiallyPressed = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TRIGGER_PARTIAL_PRESSED : value;
                    break;
                case MotionControllerMappingTypes.LeftMenu:
                    LeftMenu = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_MENU : value;
                    break;
                case MotionControllerMappingTypes.RightMenu:
                    RightMenu = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_MENU : value;
                    break;
                case MotionControllerMappingTypes.LeftGrip:
                    LeftGrip = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_BUMPER_OR_GRIP : value;
                    break;
                case MotionControllerMappingTypes.RightGrip:
                    RightGrip = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_BUMPER_OR_GRIP : value;
                    break;
                case MotionControllerMappingTypes.LeftStickClick:
                    LeftStickClick = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_STICK_CLICK : value;
                    break;
                case MotionControllerMappingTypes.RightStickClick:
                    RightStickClick = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_STICK_CLICK : value;
                    break;
                case MotionControllerMappingTypes.LeftTouchPadTouched:
                    LeftTouchPadTouched = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TOUCHPAD_TOUCHED : value;
                    break;
                case MotionControllerMappingTypes.RightTouchPadTouched:
                    RightTouchPadTouched = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TOUCHPAD_TOUCHED : value;
                    break;
                case MotionControllerMappingTypes.LeftTouchPadPressed:
                    LeftTouchPadPressed = string.IsNullOrEmpty(value) ? CONTROLLER_LEFT_TOUCHPAD_PRESSED : value;
                    break;
                case MotionControllerMappingTypes.RightTouchPadPressed:
                    RightTouchPadPressed = string.IsNullOrEmpty(value) ? CONTROLLER_RIGHT_TOUCHPAD_PRESSED : value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}