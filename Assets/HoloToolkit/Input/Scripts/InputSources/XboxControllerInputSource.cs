// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerInputSource : MonoBehaviour
    {
        [Serializable]
        private class MappingEntry
        {
            public XboxControllerMappingTypes Type = XboxControllerMappingTypes.None;
            public string Value = string.Empty;
        }

        private XboxControllerData controllerData;

        [SerializeField]
        private MappingEntry[] mapping;

        private void Awake()
        {
            if (mapping != null)
            {
                for (var i = 0; i < Enum.GetNames(typeof(XboxControllerMappingTypes)).Length; i++)
                {
                    XboxControllerMapping.SetMapping((XboxControllerMappingTypes)i, mapping[i].Value);
                }
            }
        }

        private void Update()
        {
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
        }
    }
}
