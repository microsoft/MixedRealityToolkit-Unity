// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Devices.UnityInput;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    public class OpenVRDeviceManager : UnityDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public OpenVRDeviceManager(string name, uint priority) : base(name, priority) { }

        #region Controller Utilities

        /// <inheritdoc />
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
                    controllerType = typeof(GenericOpenVRController);
                    break;
                case SupportedControllerType.ViveWand:
                    controllerType = typeof(ViveWandController);
                    break;
                case SupportedControllerType.ViveKnuckles:
                    controllerType = typeof(ViveKnucklesController);
                    break;
                case SupportedControllerType.OculusTouch:
                    controllerType = typeof(OculusTouchController);
                    break;
                case SupportedControllerType.OculusRemote:
                    controllerType = typeof(OculusRemoteController);
                    break;
                case SupportedControllerType.WindowsMixedReality:
                    controllerType = typeof(WindowsMixedRealityOpenVRMotionController);
                    break;
                default:
                    return null;
            }

            var pointers = RequestPointers(controllerType, controllingHand);
            var inputSource = InputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers);
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericOpenVRController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to Setup {controllerType.Name} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            ActiveControllers.Add(joystickName, detectedController);
            return detectedController;
        }

        /// <inheritdoc />
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