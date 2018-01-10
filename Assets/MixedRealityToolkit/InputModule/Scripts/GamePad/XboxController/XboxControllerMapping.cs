// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.InputModule.EventData;
using MixedRealityToolkit.Utilities;
using System;

namespace MixedRealityToolkit.InputModule.GamePad
{
    /// <summary>
    /// Defines the controller mapping for the input source.
    /// </summary>
    public static class XboxControllerMapping
    {
        public static string XboxLeftStickHorizontal { get; private set; }
        public static string XboxLeftStickVertical { get; private set; }
        public static string XboxRightStickHorizontal { get; private set; }
        public static string XboxRightStickVertical { get; private set; }
        public static string XboxDpadHorizontal { get; private set; }
        public static string XboxDpadVertical { get; private set; }
        public static string XboxLeftTrigger { get; private set; }
        public static string XboxRightTrigger { get; private set; }
        public static string XboxSharedTrigger { get; private set; }
        public static string XboxA { get; private set; }
        public static string XboxB { get; private set; }
        public static string XboxX { get; private set; }
        public static string XboxY { get; private set; }
        public static string XboxLeftBumper { get; private set; }
        public static string XboxRightBumper { get; private set; }
        public static string XboxLeftStickClick { get; private set; }
        public static string XboxRightStickClick { get; private set; }
        public static string XboxView { get; private set; }
        public static string XboxMenu { get; private set; }

        public static string GetMapping(XboxControllerMappingTypes type)
        {
            switch (type)
            {
                case XboxControllerMappingTypes.None:
                    return string.Empty;
                case XboxControllerMappingTypes.XboxLeftStickHorizontal:
                    return XboxLeftStickHorizontal;
                case XboxControllerMappingTypes.XboxLeftStickVertical:
                    return XboxLeftStickVertical;
                case XboxControllerMappingTypes.XboxRightStickHorizontal:
                    return XboxRightStickHorizontal;
                case XboxControllerMappingTypes.XboxRightStickVertical:
                    return XboxRightStickVertical;
                case XboxControllerMappingTypes.XboxDpadHorizontal:
                    return XboxDpadHorizontal;
                case XboxControllerMappingTypes.XboxDpadVertical:
                    return XboxDpadVertical;
                case XboxControllerMappingTypes.XboxLeftTrigger:
                    return XboxLeftTrigger;
                case XboxControllerMappingTypes.XboxRightTrigger:
                    return XboxRightTrigger;
                case XboxControllerMappingTypes.XboxSharedTrigger:
                    return XboxSharedTrigger;
                case XboxControllerMappingTypes.XboxA:
                    return XboxA;
                case XboxControllerMappingTypes.XboxB:
                    return XboxB;
                case XboxControllerMappingTypes.XboxX:
                    return XboxX;
                case XboxControllerMappingTypes.XboxY:
                    return XboxY;
                case XboxControllerMappingTypes.XboxView:
                    return XboxView;
                case XboxControllerMappingTypes.XboxMenu:
                    return XboxMenu;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    return XboxLeftBumper;
                case XboxControllerMappingTypes.XboxRightBumper:
                    return XboxRightBumper;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    return XboxLeftStickClick;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    return XboxRightStickClick;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static void SetMapping(XboxControllerMappingTypes type, string value)
        {
            switch (type)
            {
                case XboxControllerMappingTypes.None:
                    break;
                case XboxControllerMappingTypes.XboxLeftStickHorizontal:
                    XboxLeftStickHorizontal = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_STICK_HORIZONTAL : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftStickVertical:
                    XboxLeftStickVertical = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_STICK_VERTICAL : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickHorizontal:
                    XboxRightStickHorizontal = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_HORIZONTAL : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickVertical:
                    XboxRightStickVertical = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_VERTICAL : value;
                    break;
                case XboxControllerMappingTypes.XboxDpadHorizontal:
                    XboxDpadHorizontal = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_DPAD_HORIZONTAL : value;
                    break;
                case XboxControllerMappingTypes.XboxDpadVertical:
                    XboxDpadVertical = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_DPAD_VERTICAL : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftTrigger:
                    XboxLeftTrigger = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_TRIGGER : value;
                    break;
                case XboxControllerMappingTypes.XboxRightTrigger:
                    XboxRightTrigger = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_TRIGGER : value;
                    break;
                case XboxControllerMappingTypes.XboxSharedTrigger:
                    XboxSharedTrigger = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_SHARED_TRIGGER : value;
                    break;
                case XboxControllerMappingTypes.XboxA:
                    XboxA = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_A : value;
                    break;
                case XboxControllerMappingTypes.XboxB:
                    XboxB = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_B : value;
                    break;
                case XboxControllerMappingTypes.XboxX:
                    XboxX = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_X : value;
                    break;
                case XboxControllerMappingTypes.XboxY:
                    XboxY = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.XBOX_Y : value;
                    break;
                case XboxControllerMappingTypes.XboxView:
                    XboxView = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_MENU : value;
                    break;
                case XboxControllerMappingTypes.XboxMenu:
                    XboxMenu = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_MENU : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    XboxLeftBumper = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_BUMPER_OR_GRIP : value;
                    break;
                case XboxControllerMappingTypes.XboxRightBumper:
                    XboxRightBumper = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_BUMPER_OR_GRIP : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    XboxLeftStickClick = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_LEFT_STICK_CLICK : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    XboxRightStickClick = string.IsNullOrEmpty(value) ? InputMappingAxisUtility.CONTROLLER_RIGHT_STICK_CLICK : value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static float GetAxis(XboxControllerMappingTypes axisType, XboxControllerEventData eventData)
        {
            switch (axisType)
            {
                case XboxControllerMappingTypes.XboxLeftStickHorizontal:
                    return eventData.XboxLeftStickHorizontalAxis;
                case XboxControllerMappingTypes.XboxLeftStickVertical:
                    return eventData.XboxLeftStickVerticalAxis;
                case XboxControllerMappingTypes.XboxRightStickHorizontal:
                    return eventData.XboxRightStickHorizontalAxis;
                case XboxControllerMappingTypes.XboxRightStickVertical:
                    return eventData.XboxRightStickVerticalAxis;
                case XboxControllerMappingTypes.XboxDpadHorizontal:
                    return eventData.XboxDpadHorizontalAxis;
                case XboxControllerMappingTypes.XboxDpadVertical:
                    return eventData.XboxDpadVerticalAxis;
                case XboxControllerMappingTypes.XboxLeftTrigger:
                    return eventData.XboxLeftTriggerAxis;
                case XboxControllerMappingTypes.XboxRightTrigger:
                    return eventData.XboxRightTriggerAxis;
                case XboxControllerMappingTypes.XboxSharedTrigger:
                    return eventData.XboxSharedTriggerAxis;
                default:
                    throw new ArgumentOutOfRangeException("axisType", axisType, null);
            }
        }

        public static bool GetButton_Down(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            switch (buttonType)
            {
                case XboxControllerMappingTypes.None:
                    return false;
                case XboxControllerMappingTypes.XboxA:
                    return eventData.XboxA_Down;
                case XboxControllerMappingTypes.XboxB:
                    return eventData.XboxB_Down;
                case XboxControllerMappingTypes.XboxX:
                    return eventData.XboxX_Down;
                case XboxControllerMappingTypes.XboxY:
                    return eventData.XboxY_Down;
                case XboxControllerMappingTypes.XboxView:
                    return eventData.XboxView_Down;
                case XboxControllerMappingTypes.XboxMenu:
                    return eventData.XboxMenu_Down;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    return eventData.XboxLeftBumper_Down;
                case XboxControllerMappingTypes.XboxRightBumper:
                    return eventData.XboxRightBumper_Down;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    return eventData.XboxLeftStick_Down;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    return eventData.XboxRightStick_Down;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }

        public static bool GetButton_Pressed(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            switch (buttonType)
            {
                case XboxControllerMappingTypes.None:
                    return false;
                case XboxControllerMappingTypes.XboxA:
                    return eventData.XboxA_Pressed;
                case XboxControllerMappingTypes.XboxB:
                    return eventData.XboxB_Pressed;
                case XboxControllerMappingTypes.XboxX:
                    return eventData.XboxX_Pressed;
                case XboxControllerMappingTypes.XboxY:
                    return eventData.XboxY_Pressed;
                case XboxControllerMappingTypes.XboxView:
                    return eventData.XboxView_Pressed;
                case XboxControllerMappingTypes.XboxMenu:
                    return eventData.XboxMenu_Pressed;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    return eventData.XboxLeftBumper_Pressed;
                case XboxControllerMappingTypes.XboxRightBumper:
                    return eventData.XboxRightBumper_Pressed;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    return eventData.XboxLeftStick_Pressed;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    return eventData.XboxRightStick_Pressed;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }

        public static bool GetButton_Up(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
        {
            switch (buttonType)
            {
                case XboxControllerMappingTypes.None:
                    return false;
                case XboxControllerMappingTypes.XboxA:
                    return eventData.XboxA_Up;
                case XboxControllerMappingTypes.XboxB:
                    return eventData.XboxB_Up;
                case XboxControllerMappingTypes.XboxX:
                    return eventData.XboxX_Up;
                case XboxControllerMappingTypes.XboxY:
                    return eventData.XboxY_Up;
                case XboxControllerMappingTypes.XboxView:
                    return eventData.XboxView_Up;
                case XboxControllerMappingTypes.XboxMenu:
                    return eventData.XboxMenu_Up;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    return eventData.XboxLeftBumper_Up;
                case XboxControllerMappingTypes.XboxRightBumper:
                    return eventData.XboxRightBumper_Up;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    return eventData.XboxLeftStick_Up;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    return eventData.XboxRightStick_Up;
                default:
                    throw new ArgumentOutOfRangeException("buttonType", buttonType, null);
            }
        }
    }
}
