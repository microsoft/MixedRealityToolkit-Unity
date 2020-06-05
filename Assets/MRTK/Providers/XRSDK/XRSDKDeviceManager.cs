// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK.Input
{
    /// <summary>
    /// Manages XR SDK devices.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone | SupportedPlatforms.WindowsUniversal,
        "XRSDK Device Manager")]
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
        private readonly List<InputDevice> lastInputDevices = new List<InputDevice>();

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] XRSDKDeviceManager.Update");

        /// <inheritdoc/>
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

                if (XRSDKSubsystemHelpers.InputSubsystem == null || !XRSDKSubsystemHelpers.InputSubsystem.running)
                {
                    return;
                }

                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.HandTracking, inputDevices);
                foreach (InputDevice device in inputDevices)
                {
                    if (device.isValid)
                    {
                        GenericXRSDKController controller = GetOrAddController(device);

                        if (controller == null)
                        {
                            continue;
                        }

                        if (!lastInputDevices.Contains(device))
                        {
                            CoreServices.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                        }
                        else
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
                    GenericXRSDKController controller = GetOrAddController(device);
                    if (controller != null)
                    {
                        CoreServices.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                        RemoveController(device);
                    }
                }

                lastInputDevices.Clear();
                lastInputDevices.AddRange(inputDevices);
            }
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

                SupportedControllerType currentControllerType = GetCurrentControllerType(inputDevice);
                Type controllerType = GetControllerType(currentControllerType);
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
                GenericXRSDKController controller = GetOrAddController(inputDevice);

                if (controller != null)
                {
                    RecyclePointers(controller.InputSource);

                    if (controller.Visualizer != null &&
                        controller.Visualizer.GameObjectProxy != null)
                    {
                        controller.Visualizer.GameObjectProxy.SetActive(false);
                    }

                    ActiveControllers.Remove(inputDevice);
                }
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
