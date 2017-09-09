// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.XR.WSA.Input;

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

        #region Unity APIs

        protected override void Start()
        {
            base.Start();
            DeviceRefreshTimer = DeviceRefreshInterval;
        }

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
        }

        #region Required Base Input Overrides

        public override SupportedInputInfo GetSupportedInputInfo(uint sourceId)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetSourceKind(uint sourceId, out InteractionSourceKind sourceKind)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetPointerPosition(uint sourceId, out Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetPointingRay(uint sourceId, out Ray pointingRay)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetGripPosition(uint sourceId, out Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetGripRotation(uint sourceId, out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetThumbstick(uint sourceId, out bool isPressed, out Vector2 position)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetTouchpad(uint sourceId, out bool isPressed, out bool isTouched, out Vector2 position)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetSelect(uint sourceId, out bool isPressed, out double pressedValue)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetGrasp(uint sourceId, out bool isPressed)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetMenu(uint sourceId, out bool isPressed)
        {
            throw new System.NotImplementedException();
        }

        public override bool TryGetPointerRotation(uint sourceId, out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
