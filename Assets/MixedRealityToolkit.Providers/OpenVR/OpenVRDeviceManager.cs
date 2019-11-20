// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone,
        "OpenVR Device Manager")]
    public class OpenVRDeviceManager : UnityJoystickManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenVRDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // The OpenVR platform supports motion controllers.
            return (capability == MixedRealityCapability.MotionController);
        }

        #region Controller Utilities

        /// <inheritdoc />
        protected override GenericJoystickController GetOrAddController(string joystickName)
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

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            var pointers = RequestPointers(currentControllerType, controllingHand);
            var inputSource = inputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers, InputSourceType.Controller);
            var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericOpenVRController;

            if (detectedController == null)
            {
                Debug.LogError($"Failed to create {controllerType.Name} controller");
                return null;
            }

            if (!detectedController.SetupConfiguration(controllerType))
            {
                // Controller failed to be set up correctly.
                Debug.LogError($"Failed to set up {controllerType.Name} controller");
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
                return 0;
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
