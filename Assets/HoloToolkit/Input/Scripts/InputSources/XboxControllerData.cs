// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace HoloToolkit.Unity.InputModule
{
    public struct XboxControllerData
    {
        public float XboxLeftStickHorizontalAxis { get; set; }
        public float XboxLeftStickVerticalAxis { get; set; }
        public float XboxRightStickHorizontalAxis { get; set; }
        public float XboxRightStickVerticalAxis { get; set; }
        public float XboxDpadHorizontalAxis { get; set; }
        public float XboxDpadVerticalAxis { get; set; }
        public float XboxLeftTriggerAxis { get; set; }
        public float XboxRightTriggerAxis { get; set; }
        public float XboxSharedTriggerAxis { get; set; }

        public bool XboxA { get; set; }
        public bool XboxB { get; set; }
        public bool XboxX { get; set; }
        public bool XboxY { get; set; }
        public bool XboxLeftBumper { get; set; }
        public bool XboxRightBumper { get; set; }
        public bool XboxLeftStickClick { get; set; }
        public bool XboxRightStickClick { get; set; }
        public bool XboxView { get; set; }
        public bool XboxMenu { get; set; }
    }
}