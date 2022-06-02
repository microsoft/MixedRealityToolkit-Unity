// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

#if XR_MANAGEMENT_ENABLED
using UnityEngine.XR.Management;
#endif // XR_MANAGEMENT_ENABLED

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// Manages XR SDK devices.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone | SupportedPlatforms.WindowsUniversal,
        "XR SDK Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class XRSDKDeviceManager : BaseInputDeviceManager, IMixedRealityCapabilityCheck
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public XRSDKDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public virtual bool CheckCapability(MixedRealityCapability capability)
        {
            // The XR SDK platform supports motion controllers.
            return (capability == MixedRealityCapability.MotionController);
        }

        protected static readonly Dictionary<InputDevice, GenericXRSDKController> ActiveControllers = new Dictionary<InputDevice, GenericXRSDKController>();

        private readonly List<InputDevice> inputDevices = new List<InputDevice>();
        private readonly List<InputDevice> inputDevicesSubset = new List<InputDevice>();
        private readonly List<InputDevice> lastInputDevices = new List<InputDevice>();

        protected virtual List<InputDeviceCharacteristics> DesiredInputCharacteristics { get; set; } = new List<InputDeviceCharacteristics>()
        {
            InputDeviceCharacteristics.Controller,
            InputDeviceCharacteristics.HandTracking
        };

#if XR_MANAGEMENT_ENABLED
        /// <summary>
        /// Checks if the active loader has a specific name. Used in cases where the loader class is internal, like WindowsMRLoader.
        /// </summary>
        /// <param name="loaderName">The string name to compare against the active loader.</param>
        /// <returns>True if the active loader has the same name as the parameter.</returns>
        [Obsolete("Use XRSDKLoaderHelpers instead.")]
        protected virtual bool IsLoaderActive(string loaderName) => LoaderHelpers.IsLoaderActive(loaderName) ?? false;

        /// <summary>
        /// Checks if the active loader is of a specific type. Used in cases where the loader class is accessible, like OculusLoader.
        /// </summary>
        /// <typeparam name="T">The loader class type to check against the active loader.</typeparam>
        /// <returns>True if the active loader is of the specified type.</returns>
        [Obsolete("Use XRSDKLoaderHelpers instead.")]
        protected virtual bool IsLoaderActive<T>() where T : XRLoader => LoaderHelpers.IsLoaderActive<T>() ?? false;
#endif // XR_MANAGEMENT_ENABLED

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] XRSDKDeviceManager.Update");

        public override void Enable()
        {
            base.Enable();

            inputDevices.Clear();
            foreach (InputDeviceCharacteristics inputDeviceCharacteristics in DesiredInputCharacteristics)
            {
                InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, inputDevicesSubset);
                foreach (InputDevice device in inputDevicesSubset)
                {
                    if (!inputDevices.Contains(device))
                    {
                        inputDevices.Add(device);
                    }
                }
            }

            InputDevices.deviceConnected += InputDevices_deviceConnected;
            InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
        }

        /// <inheritdoc/>
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                if (!IsEnabled)
                {
                    return;
                }

                base.Update();

                if (XRSubsystemHelpers.InputSubsystem == null || !XRSubsystemHelpers.InputSubsystem.running)
                {
                    return;
                }

                foreach (InputDevice device in inputDevices)
                {
                    if (device.isValid)
                    {
                        GenericXRSDKController controller = GetOrAddController(device);

                        if (controller != null && lastInputDevices.Contains(device))
                        {
                            // Remove devices from our previously tracked list as we update them.
                            // This will allow us to remove all stale devices that were tracked
                            // last frame but not this one.
                            lastInputDevices.Remove(device);
                            controller.UpdateController(device);
                        }
                    }
                }

                foreach (InputDevice device in lastInputDevices)
                {
                    RemoveController(device);
                }

                lastInputDevices.Clear();
                lastInputDevices.AddRange(inputDevices);
            }
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            InputDevices.deviceConnected -= InputDevices_deviceConnected;
            InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;

            var controllersCopy = ActiveControllers.ToReadOnlyCollection();
            foreach (var controller in controllersCopy)
            {
                RemoveController(controller.Key);
            }

            base.Disable();
        }

        private void InputDevices_deviceConnected(InputDevice obj)
        {
            bool characteristicsMatch = (obj.characteristics & (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HandTracking)) > 0;
            if (characteristicsMatch && !inputDevices.Contains(obj))
            {
                inputDevices.Add(obj);
            }
        }

        private void InputDevices_deviceDisconnected(InputDevice obj)
        {
            inputDevices.Remove(obj);
        }

        #region Controller Utilities

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] XRSDKDeviceManager.GetOrAddController");

        /// <summary>
        /// Gets or adds a controller using the InputDevice name provided.
        /// </summary>
        /// <param name="inputDevice">The InputDevice from XR SDK.</param>
        /// <returns>The controller reference.</returns>
        protected virtual GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                // If a device is already registered with the ID provided, just return it.
                if (ActiveControllers.ContainsKey(inputDevice))
                {
                    var controller = ActiveControllers[inputDevice];
                    Debug.Assert(controller != null);
                    return controller;
                }

                Handedness controllingHand;

                if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Left))
                {
                    controllingHand = Handedness.Left;
                }
                else if (inputDevice.characteristics.IsMaskSet(InputDeviceCharacteristics.Right))
                {
                    controllingHand = Handedness.Right;
                }
                else
                {
                    controllingHand = Handedness.None;
                }

                SupportedControllerType currentControllerType = GetCurrentControllerType(inputDevice);
                Type controllerType = GetControllerType(currentControllerType);

                if (controllerType == null)
                {
                    return null;
                }

                InputSourceType inputSourceType = GetInputSourceType(currentControllerType);

                IMixedRealityPointer[] pointers = RequestPointers(currentControllerType, controllingHand);
                IMixedRealityInputSource inputSource = Service?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers, inputSourceType);
                GenericXRSDKController detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericXRSDKController;

                if (detectedController == null || !detectedController.Enabled)
                {
                    // Controller failed to be set up correctly.
                    Debug.LogError($"Failed to create {controllerType.Name} controller");

                    // Return null so we don't raise the source detected.
                    return null;
                }

                for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
                {
                    detectedController.InputSource.Pointers[i].Controller = detectedController;
                }

                ActiveControllers.Add(inputDevice, detectedController);

                Service?.RaiseSourceDetected(detectedController.InputSource, detectedController);

                return detectedController;
            }
        }

        private static readonly ProfilerMarker RemoveControllerPerfMarker = new ProfilerMarker("[MRTK] XRSDKDeviceManager.RemoveController");

        /// <summary>
        /// Gets the current controller type for the InputDevice name provided.
        /// </summary>
        /// <param name="inputDevice">The InputDevice from XR SDK.</param>
        protected virtual void RemoveController(InputDevice inputDevice)
        {
            using (RemoveControllerPerfMarker.Auto())
            {
                if (ActiveControllers.TryGetValue(inputDevice, out GenericXRSDKController controller))
                {
                    if (controller != null)
                    {
                        RemoveControllerFromScene(controller);
                    }

                    ActiveControllers.Remove(inputDevice);
                }
            }
        }

        /// <summary>
        /// Removes the controller from the scene and handles any additional cleanup
        /// </summary>
        protected void RemoveControllerFromScene(GenericXRSDKController controller)
        {
            Service?.RaiseSourceLost(controller.InputSource, controller);

            RecyclePointers(controller.InputSource);

            var visualizer = controller.Visualizer;

            if (!visualizer.IsNull() &&
                visualizer.GameObjectProxy != null)
            {
                visualizer.GameObjectProxy.SetActive(false);
            }
        }

        /// <summary>
        /// Gets the concrete type of the detected controller, based on the <see cref="Toolkit.Input.SupportedControllerType"/> and defined per-platform.
        /// </summary>
        /// <param name="supportedControllerType">The current controller type.</param>
        /// <returns>The concrete type of the currently detected controller.</returns>
        protected virtual Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            return typeof(GenericXRSDKController);
        }

        /// <summary>
        /// Returns the <see cref="Toolkit.Input.InputSourceType"/> of the currently detected controller, based on the <see cref="Toolkit.Input.SupportedControllerType"/>.
        /// </summary>
        /// <param name="supportedControllerType">The current controller type.</param>
        /// <returns>The enum value of the currently detected controller's InputSource type.</returns>
        protected virtual InputSourceType GetInputSourceType(SupportedControllerType supportedControllerType)
        {
            return InputSourceType.Controller;
        }

        /// <summary>
        /// Gets the current controller type for the InputDevice name provided.
        /// </summary>
        /// <param name="inputDevice">The InputDevice from XR SDK.</param>
        /// <returns>The supported controller type.</returns>
        protected virtual SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            Debug.Log($"{inputDevice.name} does not have a defined controller type, falling back to generic controller type");
            return SupportedControllerType.GenericUnity;
        }

        #endregion Controller Utilities
    }
}
