// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace MixedRealityToolkit.InputModule.GamePad
{
    /// <summary>
    /// Data class that carries the input data for the event handler.
    /// </summary>
    public struct XboxControllerData
    {
        public string GamePadName { get; set; }

        public float XboxLeftStickHorizontalAxis { get; set; }
        public float XboxLeftStickVerticalAxis { get; set; }
        public float XboxRightStickHorizontalAxis { get; set; }
        public float XboxRightStickVerticalAxis { get; set; }
        public float XboxDpadHorizontalAxis { get; set; }
        public float XboxDpadVerticalAxis { get; set; }
        public float XboxLeftTriggerAxis { get; set; }
        public float XboxRightTriggerAxis { get; set; }
        public float XboxSharedTriggerAxis { get; set; }

        public bool XboxA_Pressed { get; set; }
        public bool XboxB_Pressed { get; set; }
        public bool XboxX_Pressed { get; set; }
        public bool XboxY_Pressed { get; set; }
        public bool XboxLeftBumper_Pressed { get; set; }
        public bool XboxRightBumper_Pressed { get; set; }
        public bool XboxLeftStick_Pressed { get; set; }
        public bool XboxRightStick_Pressed { get; set; }
        public bool XboxView_Pressed { get; set; }
        public bool XboxMenu_Pressed { get; set; }

        public bool XboxA_Up { get; set; }
        public bool XboxB_Up { get; set; }
        public bool XboxX_Up { get; set; }
        public bool XboxY_Up { get; set; }
        public bool XboxLeftBumper_Up { get; set; }
        public bool XboxRightBumper_Up { get; set; }
        public bool XboxLeftStick_Up { get; set; }
        public bool XboxRightStick_Up { get; set; }
        public bool XboxView_Up { get; set; }
        public bool XboxMenu_Up { get; set; }

        public bool XboxA_Down { get; set; }
        public bool XboxB_Down { get; set; }
        public bool XboxX_Down { get; set; }
        public bool XboxY_Down { get; set; }
        public bool XboxLeftBumper_Down { get; set; }
        public bool XboxRightBumper_Down { get; set; }
        public bool XboxLeftStick_Down { get; set; }
        public bool XboxRightStick_Down { get; set; }
        public bool XboxView_Down { get; set; }
        public bool XboxMenu_Down { get; set; }
    }
}