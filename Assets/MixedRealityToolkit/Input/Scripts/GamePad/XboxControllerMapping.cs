// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace HoloToolkit.Unity.InputModule
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
                case XboxControllerMappingTypes.XboxLeftStickHorizontal:
                    XboxLeftStickHorizontal = string.IsNullOrEmpty(value) ? "XBOX_LEFT_STICK_HORIZONTAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftStickVertical:
                    XboxLeftStickVertical = string.IsNullOrEmpty(value) ? "XBOX_LEFT_STICK_VERTICAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickHorizontal:
                    XboxRightStickHorizontal = string.IsNullOrEmpty(value) ? "XBOX_RIGHT_STICK_HORIZONTAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickVertical:
                    XboxRightStickVertical = string.IsNullOrEmpty(value) ? "XBOX_RIGHT_STICK_VERTICAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxDpadHorizontal:
                    XboxDpadHorizontal = string.IsNullOrEmpty(value) ? "XBOX_DPAD_HORIZONTAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxDpadVertical:
                    XboxDpadVertical = string.IsNullOrEmpty(value) ? "XBOX_DPAD_VERTICAL" : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftTrigger:
                    XboxLeftTrigger = string.IsNullOrEmpty(value) ? "XBOX_LEFT_TRIGGER" : value;
                    break;
                case XboxControllerMappingTypes.XboxRightTrigger:
                    XboxRightTrigger = string.IsNullOrEmpty(value) ? "XBOX_RIGHT_TRIGGER" : value;
                    break;
                case XboxControllerMappingTypes.XboxSharedTrigger:
                    XboxSharedTrigger = string.IsNullOrEmpty(value) ? "XBOX_SHARED_TRIGGER" : value;
                    break;
                case XboxControllerMappingTypes.XboxA:
                    XboxA = string.IsNullOrEmpty(value) ? "XBOX_A" : value;
                    break;
                case XboxControllerMappingTypes.XboxB:
                    XboxB = string.IsNullOrEmpty(value) ? "XBOX_B" : value;
                    break;
                case XboxControllerMappingTypes.XboxX:
                    XboxX = string.IsNullOrEmpty(value) ? "XBOX_X" : value;
                    break;
                case XboxControllerMappingTypes.XboxY:
                    XboxY = string.IsNullOrEmpty(value) ? "XBOX_Y" : value;
                    break;
                case XboxControllerMappingTypes.XboxView:
                    XboxView = string.IsNullOrEmpty(value) ? "XBOX_VIEW" : value;
                    break;
                case XboxControllerMappingTypes.XboxMenu:
                    XboxMenu = string.IsNullOrEmpty(value) ? "XBOX_MENU" : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftBumper:
                    XboxLeftBumper = string.IsNullOrEmpty(value) ? "XBOX_LEFT_BUMPER" : value;
                    break;
                case XboxControllerMappingTypes.XboxRightBumper:
                    XboxRightBumper = string.IsNullOrEmpty(value) ? "XBOX_RIGHT_BUMPER" : value;
                    break;
                case XboxControllerMappingTypes.XboxLeftStickClick:
                    XboxLeftStickClick = string.IsNullOrEmpty(value) ? "XBOX_LEFT_STICK_CLICK" : value;
                    break;
                case XboxControllerMappingTypes.XboxRightStickClick:
                    XboxRightStickClick = string.IsNullOrEmpty(value) ? "XBOX_RIGHT_STICK_CLICK" : value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}