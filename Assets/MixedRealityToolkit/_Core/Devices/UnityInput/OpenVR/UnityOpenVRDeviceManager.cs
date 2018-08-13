// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput.OpenVR
{
    public class UnityOpenVRDeviceManager : UnityDeviceManager
    {
        public UnityOpenVRDeviceManager(string name, uint priority) : base(name, priority) { }

        #region Controller Utilities

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <returns>New or Existing Controller Input Source</returns>
        protected override GenericUnityController GetOrAddController(string joystickName)
        {
            // If a device is already registered with the ID provided, just return it.
            if (ActiveControllers.ContainsKey(joystickName))
            {
                var controller = ActiveControllers[joystickName];
                Debug.Assert(controller != null);
                return controller;
            }

            Handedness controllingHand;

            if (joystickName.Contains("Left"))
            {
                controllingHand = Handedness.Left;
            }
            else if (joystickName.Contains("Right"))
            {
                controllingHand = Handedness.Right;
            }
            else
            {
                controllingHand = Handedness.None;
            }

            var currentControllerType = GetCurrentControllerType(joystickName);
            Type controllerType;

            switch (currentControllerType)
            {
                case SupportedControllerType.GenericOpenVR:
                    controllerType = typeof(GenericUnityOpenVRController);
                    break;
                case SupportedControllerType.ViveWand:
                    controllerType = typeof(UnityViveWandController);
                    break;
                case SupportedControllerType.ViveKnuckles:
                    controllerType = typeof(UnityViveKnucklesController);
                    break;
                case SupportedControllerType.OculusTouch:
                    controllerType = typeof(UnityOculusTouchController);
                    break;
                case SupportedControllerType.OculusRemote:
                    controllerType = typeof(UnityOculusRemoteController);
                    break;
                case SupportedControllerType.WindowsMixedReality:
                    controllerType = typeof(UnityWindowsMixedRealityUnityOpenVRController);
                    break;
                default:
                    return null;
            }

            var pointers = RequestPointers(controllerType, controllingHand);
            var inputSource = InputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers);
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericUnityOpenVRController;
            if (detectedController == null) { Debug.LogError($"Failed to create {controllerType.Name} controller"); }
            detectedController?.SetupConfiguration(controllerType);

            for (int i = 0; i < detectedController?.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            ActiveControllers.Add(joystickName, detectedController);
            return detectedController;
        }

        protected override SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName) || !joystickName.Contains("OpenVR"))
            {
                return SupportedControllerType.None;
            }

            if (joystickName.Contains("Oculus Rift CV1"))
            {
                return SupportedControllerType.OculusTouch;
            }

            if (joystickName.Contains("Oculus remote"))
            {
                return SupportedControllerType.OculusRemote;
            }

            if (joystickName.Contains("Vive Wand"))
            {
                return SupportedControllerType.ViveWand;
            }

            if (joystickName.Contains("Vive Knuckles"))
            {
                return SupportedControllerType.ViveKnuckles;
            }

            if (joystickName.Contains("WindowsMR"))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericOpenVR;
        }

        #endregion Controller Utilities
    }
}