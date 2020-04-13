// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine;
using UInput = UnityEngine.Input;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    /// <summary>
    /// Manages joysticks using unity input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),  // All platforms supported by Unity
        "Unity Joystick Manager")]
    public class UnityJoystickManager : BaseInputDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public UnityJoystickManager(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : this(inputSystem, name, priority, profile) 
        {
            Registrar = registrar;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public UnityJoystickManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        private const float DeviceRefreshInterval = 3.0f;

        protected static readonly Dictionary<string, GenericJoystickController> ActiveControllers = new Dictionary<string, GenericJoystickController>();

        private float deviceRefreshTimer;
        private string[] lastDeviceList;

        private static readonly ProfilerMarker UpdatePerfMarker = new ProfilerMarker("[MRTK] UnityJoystickManager.Update");

        /// <inheritdoc />
        public override void Update()
        {
            using (UpdatePerfMarker.Auto())
            {
                base.Update();

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
        }

        /// <inheritdoc />
        public override void Disable()
        {
            base.Disable();

            IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

            foreach (var genericJoystick in ActiveControllers)
            {
                if (genericJoystick.Value != null)
                {
                    inputSystem?.RaiseSourceLost(genericJoystick.Value.InputSource, genericJoystick.Value);
                }
            }

            ActiveControllers.Clear();
        }

        private static readonly ProfilerMarker GetActiveControllersPerfMarker = new ProfilerMarker("[MRTK] UnityJoystickManager.GetActiveControllers");

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            using (GetActiveControllersPerfMarker.Auto())
            {
                IMixedRealityController[] controllers = ActiveControllers.Values.ToArray<IMixedRealityController>();
                return controllers;
            }
        }

        private static readonly ProfilerMarker RefreshDevicesPerfMarker = new ProfilerMarker("[MRTK] UnityJoystickManager.RefreshDevices");

        private void RefreshDevices()
        {
            using (RefreshDevicesPerfMarker.Auto())
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

                var joystickNames = UInput.GetJoystickNames();

                if (joystickNames.Length <= 0)
                {
                    return;
                }

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
                                inputSystem?.RaiseSourceLost(controller.InputSource, controller);
                            }

                            RemoveController(lastDeviceList[i]);
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
                            inputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                        }
                    }
                }

                lastDeviceList = joystickNames;
            }
        }

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] UnityJoystickManager.GetOrAddController");

        /// <summary>
        /// Gets or adds a controller using the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of the joystick from Unity's <see href="https://docs.unity3d.com/ScriptReference/Input.GetJoystickNames.html">Input.GetJoystickNames</see></param>
        /// <returns>A new controller reference.</returns>
        protected virtual GenericJoystickController GetOrAddController(string joystickName)
        {
            using (GetOrAddControllerPerfMarker.Auto())
            {
                IMixedRealityInputSystem inputSystem = Service as IMixedRealityInputSystem;

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
                        controllerType = typeof(GenericJoystickController);
                        break;
                    case SupportedControllerType.Xbox:
                        controllerType = typeof(XboxController);
                        break;
                }

                var inputSource = inputSystem?.RequestNewGenericInputSource($"{controllerType.Name} Controller", sourceType: InputSourceType.Controller);
                var detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, Handedness.None, inputSource, null) as GenericJoystickController;

                if (detectedController == null || !detectedController.Enabled)
                {
                    // Controller failed to be setup correctly.
                    Debug.LogError($"Failed to create {controllerType.Name} controller");

                    // Return null so we don't raise the source detected.
                    return null;
                }

                ActiveControllers.Add(joystickName, detectedController);

                return detectedController;
            }
        }

        /// <summary>
        /// Removes a controller using the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of the joystick from Unity's <see href="https://docs.unity3d.com/ScriptReference/Input.GetJoystickNames.html">Input.GetJoystickNames</see></param>
        protected virtual void RemoveController(string joystickName)
        {
            ActiveControllers.Remove(joystickName);
        }

        /// <summary>
        /// Gets the current controller type for the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of the joystick from Unity's <see href="https://docs.unity3d.com/ScriptReference/Input.GetJoystickNames.html">Input.GetJoystickNames</see></param>
        /// <returns>The supported controller type</returns>
        protected virtual SupportedControllerType GetCurrentControllerType(string joystickName)
        {
            // todo: this should be using an allow list, not a disallow list
            if (string.IsNullOrEmpty(joystickName) ||
                joystickName.Contains("OpenVR") ||
                joystickName.Contains("Spatial"))
            {
                return 0;
            }

            if (joystickName.ToLower().Contains("xbox"))
            {
                return SupportedControllerType.Xbox;
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");
            return SupportedControllerType.GenericUnity;
        }
    }
}
