// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Input.UnityInput;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.OpenVR.Input
{
    /// <summary>
    /// Manages Open VR devices using Unity's input system.
    /// </summary>
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        SupportedPlatforms.WindowsStandalone | SupportedPlatforms.MacStandalone | SupportedPlatforms.LinuxStandalone,
        "OpenVR Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.LegacyXR)]
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
        [Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        public OpenVRDeviceManager(
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
        public OpenVRDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = DefaultPriority,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // The OpenVR platform supports motion controllers.
            return (capability == MixedRealityCapability.MotionController);
        }

        #region Controller Utilities

        private static readonly ProfilerMarker GetOrAddControllerPerfMarker = new ProfilerMarker("[MRTK] OpenVRDeviceManager.GetOrAddController");

        /// <inheritdoc />
        protected override GenericJoystickController GetOrAddController(string joystickName)
        {
            using (GetOrAddControllerPerfMarker.Auto())
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
                    case SupportedControllerType.HPMotionController:
                        controllerType = typeof(HPMotionController);
                        break;
                    default:
                        return null;
                }

                IMixedRealityPointer[] pointers = RequestPointers(currentControllerType, controllingHand);
                IMixedRealityInputSource inputSource = Service?.RequestNewGenericInputSource($"{currentControllerType} Controller {controllingHand}", pointers, InputSourceType.Controller);
                GenericOpenVRController detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericOpenVRController;

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

                ActiveControllers.Add(joystickName, detectedController);

                return detectedController;
            }
        }

        private static readonly ProfilerMarker RemoveControllerPerfMarker = new ProfilerMarker("[MRTK] OpenVRDeviceManager.RemoveController");

        /// <inheritdoc />
        protected override void RemoveController(string joystickName)
        {
            using (RemoveControllerPerfMarker.Auto())
            {
                var controller = GetOrAddController(joystickName);

                if (controller != null)
                {
                    RecyclePointers(controller.InputSource);

                    if (controller.Visualizer != null &&
                        controller.Visualizer.GameObjectProxy != null)
                    {
                        controller.Visualizer.GameObjectProxy.SetActive(false);
                    }
                }

                base.RemoveController(joystickName);
            }
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

            if (joystickName.Contains("Knuckles"))
            {
                return SupportedControllerType.ViveKnuckles;
            }

            if (joystickName.Contains("WindowsMR"))
            {
                // Working around the fact that HP controllers identify as a WindowsMR controller, but have a specific PID we can check
                // https://github.com/microsoft/MixedRealityToolkit-Unity/pull/8794#discussion_r523313899
                if (joystickName.Contains("0x066A"))
                {
                    return SupportedControllerType.HPMotionController;
                }
                else
                {
                    return SupportedControllerType.WindowsMixedReality;
                }
            }

            Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericOpenVR;
        }

        #endregion Controller Utilities
    }
}
