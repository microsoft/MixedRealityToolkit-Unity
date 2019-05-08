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
    public class OpenVRDeviceManager : UnityJoystickManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="inputSystemProfile">The input system configuration profile.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenVRDeviceManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            MixedRealityInputSystemProfile inputSystemProfile,
            string name = null, 
            uint priority = DefaultPriority, 
            BaseMixedRealityProfile profile = null) : base(registrar, inputSystem, inputSystemProfile, name, priority, profile) { }

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

            Type controllerType = GetCurrentControllerType(joystickName);

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            var pointers = RequestPointers(controllerType, controllingHand);
            var inputSource = inputSystem?.RequestNewGenericInputSource($"{controllerType.Name} Controller {controllingHand}", pointers, InputSourceType.Controller);
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
        protected override Type GetCurrentControllerType(string joystickName)
        {
            if (string.IsNullOrEmpty(joystickName) || !joystickName.Contains("OpenVR"))
            {
                return null;
            }

            if (joystickName.Contains("Oculus Rift CV1"))
            {
                return typeof(OculusTouchController);
            }

            if (joystickName.Contains("Oculus remote"))
            {
                return typeof(OculusRemoteController);
            }

            if (joystickName.Contains("Vive Wand"))
            {
                return typeof(ViveWandController);
            }

            if (joystickName.Contains("Vive Knuckles"))
            {
                return typeof(ViveKnucklesController);
            }

            if (joystickName.Contains("WindowsMR"))
            {
                return typeof(WindowsMixedRealityOpenVRMotionController);
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");

            return typeof(GenericOpenVRController);
        }

        #endregion Controller Utilities
    }
}
