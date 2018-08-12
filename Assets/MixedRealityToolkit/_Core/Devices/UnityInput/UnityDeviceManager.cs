// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput
{
    public class UnityDeviceManager : BaseDeviceManager
    {
        public UnityDeviceManager(string name, uint priority) : base(name, priority) { }

        private readonly Dictionary<string, GenericUnityController> activeControllers = new Dictionary<string, GenericUnityController>();

        private float deviceRefreshTimer;
        protected float DeviceRefreshInterval = 3.0f;
        protected string[] LastDeviceList;

        public override void Enable()
        {
            RefreshDevices();
        }

        public override void Update()
        {
            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {
                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }

            foreach (KeyValuePair<string, GenericUnityController> controller in activeControllers)
            {
                controller.Value?.UpdateController();
            }
        }

        public override void Disable()
        {
            foreach (var genericOpenVRController in activeControllers)
            {
                if (genericOpenVRController.Value != null)
                {
                    InputSystem?.RaiseSourceLost(genericOpenVRController.Value.InputSource, genericOpenVRController.Value);
                }
            }

            activeControllers.Clear();
        }

        protected virtual void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            if (LastDeviceList != null && joystickNames.Length == LastDeviceList.Length)
            {
                for (int i = 0; i < LastDeviceList.Length; i++)
                {
                    if (joystickNames[i].Equals(LastDeviceList[i])) { continue; }

                    if (activeControllers.ContainsKey(LastDeviceList[i]))
                    {
                        var controller = GetOrAddController(LastDeviceList[i]);

                        if (controller != null)
                        {
                            InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                        }
                        activeControllers.Remove(LastDeviceList[i]);
                    }
                }
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]))
                {
                    continue;
                }

                if (!activeControllers.ContainsKey(joystickNames[i]))
                {
                    var controller = GetOrAddController(joystickNames[i]);

                    if (controller != null)
                    {
                        InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                    }
                }
            }

            LastDeviceList = joystickNames;
        }

        protected virtual GenericUnityController GetOrAddController(string joystickName)
        {
            if (activeControllers.ContainsKey(joystickName))
            {
                var controller = activeControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            Type controllerType;

            switch (GetCurrentControllerType(joystickName))
            {
                default:
                    return null;
                case SupportedControllerType.GenericUnityDevice:
                    controllerType = typeof(GenericUnityController);
                    break;
                case SupportedControllerType.XboxController:
                    controllerType = typeof(XboxUnityController);
                    break;
            }

            var inputSource = InputSystem?.RequestNewGenericInputSource($"{controllerType.Name} Controller");
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, Handedness.None, inputSource, null) as GenericUnityController;
            if (detectedController == null) { Debug.LogError($"Failed to create {controllerType.Name} controller"); }
            detectedController?.SetupConfiguration(controllerType);

            Debug.Log($"Found controller: {joystickName}");
            activeControllers.Add(joystickName, detectedController);
            return detectedController;
        }

        protected virtual SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName) ||
                joystickName.Contains("OpenVR") ||
                joystickName.Contains("Spatial"))
            {
                return SupportedControllerType.None;
            }

            if (joystickName.Contains("Xbox Controller") ||
                joystickName.Contains("Xbox One For Windows") ||
                joystickName.Contains("Xbox Bluetooth Gamepad") ||
                joystickName.Contains("Xbox Wireless Controller"))
            {
                return SupportedControllerType.XboxController;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");
            return SupportedControllerType.GenericUnityDevice;
        }
    }
}
