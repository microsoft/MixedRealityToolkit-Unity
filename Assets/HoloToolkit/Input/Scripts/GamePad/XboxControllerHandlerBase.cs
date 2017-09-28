// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerHandlerBase : MonoBehaviour, IXboxControllerHandler
    {
        [SerializeField]
        [Tooltip("Is Gaze required for controller input?")]
        protected bool IsGlobalListener = true;

        protected string GamePadName;

        protected virtual void Start()
        {
            if (IsGlobalListener)
            {
                InputManager.Instance.AddGlobalListener(gameObject);
            }
        }

        public virtual void OnGamePadDetected(GamePadEventData eventData)
        {
            GamePadName = eventData.GamePadName;
        }

        public virtual void OnGamePadLost(GamePadEventData eventData)
        {
            GamePadName = string.Empty;
        }

        public virtual void OnXboxAxisUpdate(XboxControllerEventData eventData)
        {
        }

        protected static bool OnButton_Up(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
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

        protected static bool OnButton_Pressed(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
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

        protected static bool OnButton_Down(XboxControllerMappingTypes buttonType, XboxControllerEventData eventData)
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
    }
}
