// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput
{
    /// <summary>
    /// Manages devices using unity input system.
    /// </summary>
    public class UnityDeviceManager : BaseDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public UnityDeviceManager(string name, uint priority) : base(name, priority) { }

        private const float DeviceRefreshInterval = 3.0f;

        protected static readonly Dictionary<string, GenericUnityController> ActiveControllers = new Dictionary<string, GenericUnityController>();

        private float deviceRefreshTimer;
        private string[] lastDeviceList;

        /// <inheritdoc />
        public override void Update()
        {
            deviceRefreshTimer += Time.unscaledDeltaTime;

            if (deviceRefreshTimer >= DeviceRefreshInterval)
            {
                deviceRefreshTimer = 0.0f;
                RefreshDevices();
            }

            foreach (var controller in ActiveControllers)
            {
                controller.Value?.UpdateController();
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            foreach (var genericOpenVRController in ActiveControllers)
            {
                if (genericOpenVRController.Value != null)
                {
                    InputSystem?.RaiseSourceLost(genericOpenVRController.Value.InputSource, genericOpenVRController.Value);
                }
            }

            ActiveControllers.Clear();
        }

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return ActiveControllers.Values.ToArray<IMixedRealityController>();
        }

        private void RefreshDevices()
        {
            var joystickNames = Input.GetJoystickNames();

            if (joystickNames.Length <= 0) { return; }

            if (lastDeviceList != null && joystickNames.Length == lastDeviceList.Length)
            {
                for (int i = 0; i < lastDeviceList.Length; i++)
                {
                    if (joystickNames[i].Equals(lastDeviceList[i])) { continue; }

                    if (ActiveControllers.ContainsKey(lastDeviceList[i]))
                    {
                        var controller = GetOrAddController(lastDeviceList[i]);

                        if (controller != null)
                        {
                            InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                        }

                        ActiveControllers.Remove(lastDeviceList[i]);
                    }
                }
            }

            for (var i = 0; i < joystickNames.Length; i++)
            {
                if (string.IsNullOrEmpty(joystickNames[i]))
                {
                    continue;
                }

                if (!ActiveControllers.ContainsKey(joystickNames[i]))
                {
                    var controller = GetOrAddController(joystickNames[i]);

                    if (controller != null)
                    {
                        InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                    }
                }
            }

            lastDeviceList = joystickNames;
        }

        /// <summary>
        /// Gets or adds a controller using the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of they joystick from Unity's <see cref="Input.GetJoystickNames"/></param>
        /// <returns>A new controller reference.</returns>
        protected virtual GenericUnityController GetOrAddController(string joystickName)
        {
            if (ActiveControllers.ContainsKey(joystickName))
            {
                var controller = ActiveControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            Type controllerType;

            switch (GetCurrentControllerType(joystickName))
            {
                default:
                    return null;
                case SupportedControllerType.GenericUnity:
                    controllerType = typeof(GenericUnityController);
                    break;
                case SupportedControllerType.Xbox:
                    controllerType = typeof(XboxController);
                    break;
            }

            var inputSource = InputSystem?.RequestNewGenericInputSource($"{controllerType.Name} Controller");
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, Handedness.None, inputSource, null) as GenericUnityController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            ActiveControllers.Add(joystickName, detectedController);
            return detectedController;
        }

        /// <summary>
        /// Gets the current controller type for the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of they joystick from Unity's <see cref="Input.GetJoystickNames"/></param>
        /// <returns>The supported controller type</returns>
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
                return SupportedControllerType.Xbox;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");
            return SupportedControllerType.GenericUnity;
        }
    }
}
