// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace HoloToolkit.Unity.InputModule
{
    public class GamePadInputSource : BaseInputSource
    {
        [SerializeField]
        [Tooltip("Time in secords to determine if an Input Device has been connected or disconnected")]
        protected float DeviceRefreshInterval = 3.0f;
        protected float DeviceRefreshTimer;
        protected int LastDeviceUpdateCount;
        protected string[] LastDeviceList;

        #region Required Base Input Overrides 

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            return SupportedInputInfo.None;
        }

        public override bool TryGetPosition(uint sourceId, out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public override bool TryGetOrientation(uint sourceId, out Quaternion orientation)
        {
            orientation = Quaternion.identity;
            return false;
        }

        #endregion

        #region Unity APIs

        protected virtual void Update()
        {
            DeviceRefreshTimer += Time.unscaledDeltaTime;

            if (DeviceRefreshTimer >= DeviceRefreshInterval)
            {
                DeviceRefreshTimer = 0.0f;
                RefreshDevices();
            }
        }

        #endregion

        protected virtual void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }


            for (var i = 0; i < joystickNames.Length; i++)
            {
                Debug.LogWarningFormat("Joystick \"{0}\" has not been setup with the input manager.  Create a new class that inherits from \"GamePadInputSource\" and implement it.", joystickNames[i]);
            }
        }
    }
}
