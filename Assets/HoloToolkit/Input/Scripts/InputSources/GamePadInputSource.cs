// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.EventSystems;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadInputSource : BaseInputSource
    {
        protected const string XboxController = "Xbox Controller";
        protected const string XboxOneForWindows = "Xbox One For Windows";
        protected const string XboxBluetoothGamePad = "Xbox Bluetooth Gamepad";
        protected const string XboxWirelessController = "Xbox Wireless Controller";
        protected const string MotionControllerLeft = "Spatial Controller - Left";
        protected const string MotionControllerRight = "Spatial Controller - Right";

        [SerializeField]
        [Tooltip("Time in seconds to determine if an Input Device has been connected or disconnected")]
        protected float DeviceRefreshInterval = 3.0f;
        protected int LastDeviceUpdateCount;
        protected string[] LastDeviceList;

        protected StandaloneInputModule InputModule;
        protected const string DefaultHorizontalAxis = "Horizontal";
        protected const string DefaultVerticalAxis = "Vertical";
        protected const string DefaultSubmitButton = "Submit";
        protected const string DefaultCancelButton = "Cancel";
        protected const bool DefaultForceActiveState = false;

        protected string PreviousHorizontalAxis;
        protected string PreviousVerticalAxis;
        protected string PreviousSubmitButton;
        protected string PreviousCancelButton;
        protected bool PreviousForceActiveState;

        private float deviceRefreshTimer;

        #region Unity methods

        protected virtual void Awake()
        {
            InputModule = FindObjectOfType<StandaloneInputModule>();

            if (InputModule == null)
            {
                Debug.LogError("Missing the Standalone Input Module for GamePad Input Source!\n" +
                               "Ensure you have an Event System in your scene.");
            }
        }

        protected virtual void Update()
        {
            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {
                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }
        }

        #endregion // Unity methods

        protected virtual void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                Debug.LogWarningFormat("Joystick \"{0}\" has not been setup with the input manager.  Create a new class that inherits from \"GamePadInputSource\" and implement it.", joystickNames[i]);
            }
        }

        #region Base Input Source Methods

        public override SupportedInputInfo GetSupportedInputInfo()
        {
            return SupportedInputInfo.Thumbstick;
        }

        #endregion // Base Input Source Methods
    }
}
