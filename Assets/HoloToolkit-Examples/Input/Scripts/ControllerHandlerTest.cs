// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace HoloToolkit.Unity.InputModule.Tests
{
    public class ControllerHandlerTest : GamePadHandlerBase, IXboxControllerHandler, IMotionControllerHandler
    {
        [SerializeField]
        private float movementSpeedMultiplier = 1f;

        [SerializeField]
        private float rotationSpeedMultiplier = 1f;

        [Header("Xbox Controller Settings")]
        [SerializeField]
        private XboxControllerMappingTypes xboxResetButton = XboxControllerMappingTypes.XboxY;

        [Header("Motion Controller Settings")]
        [SerializeField]
        private MotionControllerMappingTypes motionControllerResetButton = MotionControllerMappingTypes.RightMenu;

        public Text DebugText;
        private Vector3 initialPosition;
        private Vector3 newPosition;
        private Vector3 newRotation;

        protected override void Start()
        {
            base.Start();
            initialPosition = transform.position;
        }

        public override void OnSourceDetected(SourceStateEventData eventData)
        {
            base.OnSourceDetected(eventData);
            Debug.LogFormat("Joystick Id: \"{0}\" Connected", eventData.SourceId);
        }

        public override void OnSourceLost(SourceStateEventData eventData)
        {
            base.OnSourceLost(eventData);
            Debug.LogFormat("Joystick \"{0}\" Disconnected", eventData.SourceId);
            DebugText.text = "No Controller Connected";
        }

        public void OnMotionControllerInputUpdate(MotionControllerEventData eventData)
        {
            // Reset our new vectors
            newPosition = Vector3.zero;
            newRotation = Vector3.zero;

            // Assign new Position Data
            newPosition.x += eventData.LeftStickHorizontalAxis * movementSpeedMultiplier;
            newPosition.z += eventData.LeftStickVerticalAxis * movementSpeedMultiplier;

            transform.position += newPosition;

            // Assign new rotation data
            newRotation.y += eventData.RightStickHorizontalAxis * rotationSpeedMultiplier;

            transform.rotation *= Quaternion.Euler(newRotation);

            if (MotionControllerMapping.GetButton_Up(motionControllerResetButton, eventData))
            {
                transform.position = initialPosition;
            }

            DebugText.text =
                string.Format(
                    "{0}\n" +
                    "LS Horizontal: {1:0.000} Vertical: {2:0.000}\n" +
                    "RS Horizontal: {3:0.000} Vertical: {4:0.000}\n" +
                    "LT Horizontal: {5:0.000} Vertical: {6:0.000}\n" +
                    "RT Horizontal: {7:0.000} Vertical: {8:0.000}\n" +
                    "Left Trigger:  {9:0.000} Right Trigger: {10:0.000}\n" +
                    "Left Trigger Pressed: {11} Right Trigger Pressed: {12} \n" +
                    "Left Trigger Partially Pressed: {13} Right Trigger Partially Pressed: {14} \n" +
                    "Left Menu Pressed: {15} Right Menu Pressed: {16} \n" +
                    "Left Grip Pressed: {17} Right Grip Pressed: {18} \n" +
                    "Left Stick Pressed: {19} Right Stick Pressed: {20} \n" +
                    "Left TouchPad Touched: {21} Right TouchPad Touched: {22} \n" +
                    "Left TouchPad Pressed: {23} Right TouchPad Pressed: {24} \n",
                    GamePadName,
                    eventData.LeftStickHorizontalAxis, eventData.LeftStickVerticalAxis,
                    eventData.RightStickHorizontalAxis, eventData.RightStickVerticalAxis,
                    eventData.LeftTouchPadHorizontalAxis, eventData.LeftTouchPadVerticalAxis,
                    eventData.RightTouchPadHorizontalAxis, eventData.RightTouchPadVerticalAxis,
                    eventData.LeftTriggerAxis, eventData.RightTriggerAxis,
                    eventData.LeftTrigger_Pressed, eventData.RightTrigger_Pressed,
                    eventData.LeftTrigger_PartiallyPressed, eventData.RightTrigger_PartiallyPressed,
                    eventData.LeftMenu_Pressed, eventData.RightMenu_Pressed,
                    eventData.LeftGrip_Pressed, eventData.RightGrip_Pressed,
                    eventData.LeftStick_Pressed, eventData.RightStick_Pressed,
                    eventData.LeftTouchPad_Touched, eventData.RightTouchPad_Touched,
                    eventData.LeftTouchPad_Pressed, eventData.RightTouchPad_Pressed);
        }

        public void OnXboxInputUpdate(XboxControllerEventData eventData)
        {
            // Reset our new vectors
            newPosition = Vector3.zero;
            newRotation = Vector3.zero;

            // Assign new Position Data
            newPosition.x += eventData.XboxLeftStickHorizontalAxis * movementSpeedMultiplier;
            newPosition.z += eventData.XboxLeftStickVerticalAxis * movementSpeedMultiplier;
            newPosition.y += eventData.XboxSharedTriggerAxis * movementSpeedMultiplier;

            transform.position += newPosition;

            // Assign new rotation data
            newRotation.y += eventData.XboxRightStickHorizontalAxis * rotationSpeedMultiplier;

            transform.rotation *= Quaternion.Euler(newRotation);

            if (XboxControllerMapping.GetButton_Up(xboxResetButton, eventData))
            {
                transform.position = initialPosition;
            }

            DebugText.text =
                string.Format(
                    "{19}\n" +
                    "LS Horizontal: {0:0.000} Vertical: {1:0.000}\n" +
                    "RS Horizontal: {2:0.000} Vertical: {3:0.000}\n" +
                    "DP Horizontal: {4:0.000} Vertical: {5:0.000}\n" +
                    "Left Trigger:  {6:0.000} Right Trigger: {7:0.000} Shared Trigger: {8:0.00}\n" +
                    "A: {9} B: {10} X: {11} Y: {12}\n" +
                    "LB: {13} RB: {14} " +
                    "LS: {15} RS: {16}\n" +
                    "View: {17} Menu: {18}\n",
                    eventData.XboxLeftStickHorizontalAxis, eventData.XboxLeftStickVerticalAxis,
                    eventData.XboxRightStickHorizontalAxis, eventData.XboxRightStickVerticalAxis,
                    eventData.XboxDpadHorizontalAxis, eventData.XboxDpadVerticalAxis,
                    eventData.XboxLeftTriggerAxis, eventData.XboxRightTriggerAxis, eventData.XboxSharedTriggerAxis,
                    eventData.XboxA_Pressed, eventData.XboxB_Pressed, eventData.XboxX_Pressed, eventData.XboxY_Pressed,
                    eventData.XboxLeftBumper_Pressed, eventData.XboxRightBumper_Pressed,
                    eventData.XboxLeftStick_Pressed, eventData.XboxRightStick_Pressed,
                    eventData.XboxView_Pressed, eventData.XboxMenu_Pressed,
                    GamePadName);
        }
    }
}
