// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class XboxControllerEventData : GamePadEventData
    {
        public float XboxLeftStickHorizontalAxis { get; private set; }
        public float XboxLeftStickVerticalAxis { get; private set; }
        public float XboxRightStickHorizontalAxis { get; private set; }
        public float XboxRightStickVerticalAxis { get; private set; }
        public float XboxDpadHorizontalAxis { get; private set; }
        public float XboxDpadVerticalAxis { get; private set; }
        public float XboxLeftTriggerAxis { get; private set; }
        public float XboxRightTriggerAxis { get; private set; }
        public float XboxSharedTriggerAxis { get; private set; }

        public bool XboxA { get; private set; }
        public bool XboxB { get; private set; }
        public bool XboxX { get; private set; }
        public bool XboxY { get; private set; }
        public bool XboxLeftBumper { get; private set; }
        public bool XboxRightBumper { get; private set; }
        public bool XboxLeftStickClick { get; private set; }
        public bool XboxRightStickClick { get; private set; }
        public bool XboxView { get; private set; }
        public bool XboxMenu { get; private set; }

        public XboxControllerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public void Initialize(IInputSource inputSource, uint sourceId, XboxControllerData inputData)
        {
            BaseInitialize(inputSource, sourceId);

            XboxLeftStickHorizontalAxis = inputData.XboxLeftStickHorizontalAxis;
            XboxLeftStickVerticalAxis = inputData.XboxLeftStickVerticalAxis;
            XboxRightStickHorizontalAxis = inputData.XboxRightStickHorizontalAxis;
            XboxRightStickVerticalAxis = inputData.XboxRightStickVerticalAxis;
            XboxDpadHorizontalAxis = inputData.XboxDpadHorizontalAxis;
            XboxDpadVerticalAxis = inputData.XboxDpadVerticalAxis;
            XboxLeftTriggerAxis = inputData.XboxLeftTriggerAxis;
            XboxRightTriggerAxis = inputData.XboxRightTriggerAxis;
            XboxSharedTriggerAxis = inputData.XboxSharedTriggerAxis;

            XboxA = inputData.XboxA;
            XboxB = inputData.XboxB;
            XboxX = inputData.XboxX;
            XboxY = inputData.XboxY;
            XboxLeftBumper = inputData.XboxLeftBumper;
            XboxRightBumper = inputData.XboxRightBumper;
            XboxLeftStickClick = inputData.XboxLeftStickClick;
            XboxRightStickClick = inputData.XboxRightStickClick;
            XboxView = inputData.XboxView;
            XboxMenu = inputData.XboxMenu;
        }
    }
}
