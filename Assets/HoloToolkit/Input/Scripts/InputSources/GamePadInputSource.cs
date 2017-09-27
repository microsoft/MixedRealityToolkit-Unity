// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadInputSource : BaseInputSource
    {
        [SerializeField]
        [Tooltip("Time in seconds to determine if an Input Device has been connected or disconnected")]
        protected float DeviceRefreshInterval = 3.0f;
        protected float DeviceRefreshTimer;
        protected int LastDeviceUpdateCount;
        protected string[] LastDeviceList;

        #region Unity methods

        protected virtual void Update()
        {
            DeviceRefreshTimer += Time.unscaledDeltaTime;

            if (DeviceRefreshTimer >= DeviceRefreshInterval)
            {
                DeviceRefreshTimer = 0.0f;
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

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceInfo sourceKind)
        {
            sourceKind = InteractionSourceInfo.Controller;
            return true;
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            pointingRay = default(Ray);
            return false;
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }

        public override bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
        {
            isPressed = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
        {
            isPressed = false;
            isTouched = false;
            position = Vector2.zero;
            return false;
        }

        public override bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedAmount)
        {
            isPressed = false;
            pressedAmount = 0.0;
            return false;
        }

        public override bool TryGetGrasp(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }

        public override bool TryGetMenu(uint sourceId, out bool isPressed)
        {
            isPressed = false;
            return false;
        }

        #endregion // Base Input Source Methods
    }
}
