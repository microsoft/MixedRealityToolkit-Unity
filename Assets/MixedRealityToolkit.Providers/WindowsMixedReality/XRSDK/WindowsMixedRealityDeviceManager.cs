// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine;
using UnityEngine.XR;

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.WindowsMixedReality;
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

namespace Microsoft.MixedReality.Toolkit.XRSDK.WindowsMixedReality
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.WindowsUniversal,
        "XRSDK Windows Mixed Reality Device Manager")]
    public class WindowsMixedRealityDataProvider : XRSDKDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public WindowsMixedRealityDataProvider(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

#if (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP
        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (WindowsMixedRealityUtilities.UtilitiesProvider == null)
            {
                WindowsMixedRealityUtilities.UtilitiesProvider = new XRSDKWindowsMixedRealityUtilitiesProvider();
            }
    }
#endif // (UNITY_WSA && DOTNETWINRT_PRESENT) || WINDOWS_UWP

#region Controller Utilities

        /// <inheritdoc />
        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            // If a device is already registered with the ID provided, just return it.
            if (ActiveControllers.ContainsKey(inputDevice.name))
            {
                var controller = ActiveControllers[inputDevice.name];
                Debug.Assert(controller != null);
                return controller;
            }

            Handedness controllingHand;

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Left))
            {
                controllingHand = Handedness.Left;
            }
            else if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Right))
            {
                controllingHand = Handedness.Right;
            }
            else
            {
                controllingHand = Handedness.None;
            }

            Debug.Log(controllingHand + " | " + inputDevice.manufacturer + " | " + inputDevice.serialNumber);

            var currentControllerType = GetCurrentControllerType(inputDevice);
            Type controllerType;
            InputSourceType inputSourceType;

            switch (currentControllerType)
            {
                case SupportedControllerType.WindowsMixedReality:
                    controllerType = typeof(WindowsMixedRealityXRSDKMotionController);
                    inputSourceType = InputSourceType.Controller;
                    break;
                case SupportedControllerType.ArticulatedHand:
                    controllerType = typeof(WindowsMixedRealityXRSDKArticulatedHand);
                    inputSourceType = InputSourceType.Hand;
                    break;
                default:
                    return null;
            }

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;
            IMixedRealityPointer[] pointers = RequestPointers(currentControllerType, controllingHand);
            IMixedRealityInputSource inputSource = inputSystem?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers, inputSourceType);

            if (!(Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) is GenericXRSDKController detectedController))
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

            ActiveControllers.Add(inputDevice.name, detectedController);
            return detectedController;
        }

        /// <inheritdoc />
        protected override SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            Debug.Log(inputDevice.name);

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                return SupportedControllerType.ArticulatedHand;
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            Debug.Log($"{inputDevice.name} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericUnity;
        }

#endregion Controller Utilities
    }
}

