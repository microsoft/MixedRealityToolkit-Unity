// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using Microsoft.MixedReality.Toolkit.XRSDK.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.XRSDK
{
    /// <summary>
    /// Manages Open VR Devices using unity's input system.
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
        public bool CheckCapability(MixedRealityCapability capability)
        {
            // The OpenVR platform supports motion controllers.
            return (capability == MixedRealityCapability.MotionController);
        }

        protected static readonly Dictionary<string, GenericXRSDKController> ActiveControllers = new Dictionary<string, GenericXRSDKController>();

        private InputDevice leftInputDevice;
        private InputDevice rightInputDevice;
        private bool wasLeftInputDeviceValid = false;
        private bool wasRightInputDeviceValid = false;

        public override void Update()
        {
            base.Update();

            if (XRSDKSubsystemHelpers.InputSubsystem == null || !XRSDKSubsystemHelpers.InputSubsystem.running)
            {
                Debug.Log($"Input system null {XRSDKSubsystemHelpers.InputSubsystem == null}");
                return;
            }

            if (!leftInputDevice.isValid)
            {
                if (wasLeftInputDeviceValid)
                {
                    GenericXRSDKController controller = GetOrAddController(leftInputDevice);

                    if (controller != null)
                    {
                        CoreServices.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                    }

                    RemoveController(leftInputDevice);
                    wasLeftInputDeviceValid = false;
                }

                leftInputDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

                if (leftInputDevice.isValid)
                {
                    wasLeftInputDeviceValid = true;
                    GenericXRSDKController controller = GetOrAddController(leftInputDevice);

                    if (controller != null)
                    {
                        CoreServices.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                    }
                }
            }
            else
            {
                GetOrAddController(leftInputDevice)?.UpdateController(leftInputDevice);
            }

            if (!rightInputDevice.isValid)
            {
                if (wasRightInputDeviceValid)
                {
                    GenericXRSDKController controller = GetOrAddController(rightInputDevice);

                    if (controller != null)
                    {
                        CoreServices.InputSystem?.RaiseSourceLost(controller.InputSource, controller);
                    }

                    RemoveController(rightInputDevice);
                    wasRightInputDeviceValid = false;
                }

                rightInputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

                if (rightInputDevice.isValid)
                {
                    wasRightInputDeviceValid = true;
                    GenericXRSDKController controller = GetOrAddController(rightInputDevice);

                    if (controller != null)
                    {
                        CoreServices.InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
                    }
                }
            }
            else
            {
                GetOrAddController(rightInputDevice)?.UpdateController(rightInputDevice);
            }
        }

        #region Controller Utilities

        /// <summary>
        /// Gets or adds a controller using the joystick name provided.
        /// </summary>
        /// <param name="joystickName">The name of the joystick from Unity's <see href="https://docs.unity3d.com/ScriptReference/Input.GetJoystickNames.html">Input.GetJoystickNames</see></param>
        /// <returns>The controller reference.</returns>
        protected virtual GenericXRSDKController GetOrAddController(InputDevice inputDevice)
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
                //case SupportedControllerType.GenericOpenVR:
                //    controllerType = typeof(GenericOpenVRController);
                //    break;
                //case SupportedControllerType.ViveWand:
                //    controllerType = typeof(ViveWandController);
                //    break;
                //case SupportedControllerType.ViveKnuckles:
                //    controllerType = typeof(ViveKnucklesController);
                //    break;
                //case SupportedControllerType.OculusTouch:
                //    controllerType = typeof(OculusTouchController);
                //    break;
                //case SupportedControllerType.OculusRemote:
                //    controllerType = typeof(OculusRemoteController);
                //    break;
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
        protected virtual void RemoveController(InputDevice inputDevice)
        {
            GenericXRSDKController controller = GetOrAddController(inputDevice);

            if (controller != null)
            {
                foreach (IMixedRealityPointer pointer in controller.InputSource.Pointers)
                {
                    if (pointer != null)
                    {
                        pointer.Controller = null;
                    }
                }

                if (controller.Visualizer != null &&
                    controller.Visualizer.GameObjectProxy != null)
                {
                    controller.Visualizer.GameObjectProxy.SetActive(false);
                }

                ActiveControllers.Remove(inputDevice.name);
            }
        }

        /// <inheritdoc />
        protected virtual SupportedControllerType GetCurrentControllerType(InputDevice inputDevice)
        {
            Debug.Log(inputDevice.name);

            // 1118 is valid for Windows Mixed Reality controllers
            if (inputDevice.manufacturer.Contains("1118"))
            {
                return SupportedControllerType.WindowsMixedReality;
            }

            if (inputDevice.characteristics.HasFlag(InputDeviceCharacteristics.HandTracking))
            {
                return SupportedControllerType.ArticulatedHand;
            }

            //if (string.IsNullOrEmpty(joystickName) || !joystickName.Contains("OpenVR"))
            //{
            //    return 0;
            //}

            //if (joystickName.Contains("Oculus Rift CV1"))
            //{
            //    return SupportedControllerType.OculusTouch;
            //}

            //if (joystickName.Contains("Oculus remote"))
            //{
            //    return SupportedControllerType.OculusRemote;
            //}

            //if (joystickName.Contains("Vive Wand"))
            //{
            //    return SupportedControllerType.ViveWand;
            //}

            //if (joystickName.Contains("Vive Knuckles"))
            //{
            //    return SupportedControllerType.ViveKnuckles;
            //}

            //if (joystickName.Contains("WindowsMR"))
            //{
            //    return SupportedControllerType.WindowsMixedReality;
            //}

            //Debug.Log($"{joystickName} does not have a defined controller type, falling back to generic controller type");

            return SupportedControllerType.GenericOpenVR;
        }

        #endregion Controller Utilities
    }
}
