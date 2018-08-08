// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.OpenVR
{
    public class OpenVRDeviceManager : BaseDeviceManager
    {
        public OpenVRDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<XRNode, GenericOpenVRController> activeControllers = new Dictionary<XRNode, GenericOpenVRController>();

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager
        /// </summary>
        private readonly List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.Values.ToArray<IMixedRealityController>();
        }

        public override void Enable()
        {
            InputTracking.trackingAcquired += InputTracking_trackingAcquired;
            InputTracking.trackingLost += InputTracking_trackingLost;
        }

        public override void Update()
        {
            InputTracking.GetNodeStates(nodeStates);

            for (int i = 0; i < nodeStates.Count; i++)
            {
                if (IsNodeTypeSupported(nodeStates[i]) &&
                    activeControllers.ContainsKey(nodeStates[i].nodeType) &&
                    activeControllers[nodeStates[i].nodeType].Enabled)
                {
                    activeControllers[nodeStates[i].nodeType].UpdateController(nodeStates[i]);
                }
            }
        }

        public override void Disable()
        {
            InputTracking.trackingAcquired -= InputTracking_trackingAcquired;
            InputTracking.trackingLost -= InputTracking_trackingLost;

            foreach (var genericOpenVRController in activeControllers)
            {
                InputSystem?.RaiseSourceLost(genericOpenVRController.Value?.InputSource, genericOpenVRController.Value);
            }

            activeControllers.Clear();
        }

        #region Unity InteractionManager Events

        private void InputTracking_trackingAcquired(XRNodeState obj)
        {
            if (IsNodeTypeSupported(obj))
            {
                if (!activeControllers.ContainsKey(obj.nodeType))
                {
                    var controller = GetOrAddController(obj);
                    InputSystem?.RaiseSourceDetected(controller?.InputSource, controller);
                }
            }
        }

        private void InputTracking_trackingLost(XRNodeState obj)
        {
            if (IsNodeTypeSupported(obj))
            {
                if (activeControllers.ContainsKey(obj.nodeType))
                {
                    var controller = GetOrAddController(obj);
                    InputSystem?.RaiseSourceLost(controller?.InputSource, controller);
                    activeControllers.Remove(obj.nodeType);
                }
            }
        }

        #endregion Unity InteractionManager Events

        #region Controller Utilities

        private bool IsNodeTypeSupported(XRNodeState obj)
        {
            switch (obj.nodeType)
            {
                case XRNode.LeftEye:
                case XRNode.RightEye:
                case XRNode.CenterEye:
                case XRNode.Head:
                case XRNode.TrackingReference:
                case XRNode.HardwareTracker:
                    return false;
                case XRNode.LeftHand:
                case XRNode.RightHand:
                case XRNode.GameController:
                default:
                    return true;
            }
        }

        /// <summary>
        /// Function to guess which type of controller is attached from the Joystick array
        /// </summary>
        /// <remarks>Note, Unity now caches the array between runs, so if you have more than one controller attached, this will fail
        /// TODO: Find a better way?</remarks>
        /// <returns></returns>
        private static SupportedControllerType CurrentControllerType
        {
            get
            {
                var controllers = Input.GetJoystickNames();

                for (int i = 0; i < controllers.Length; i++)
                {
                    if (string.IsNullOrEmpty(controllers[i]))
                    {
                        return SupportedControllerType.None;
                    }

                    if (controllers[i].Contains("Oculus Rift CV1 "))
                    {
                        return SupportedControllerType.OculusTouch;
                    }

                    if (controllers[i].Contains("Oculus remote"))
                    {
                        return SupportedControllerType.OculusRemote;
                    }

                    if (controllers[i].Contains("Vive Wand"))
                    {
                        return SupportedControllerType.ViveWand;
                    }

                    if (controllers[i].Contains("Vive Knuckles"))
                    {
                        return SupportedControllerType.ViveKnuckles;
                    }

                    if (controllers[i].Contains("WindowsMR"))
                    {
                        return SupportedControllerType.WindowsMixedReality;
                    }

                    Debug.Log($"{controllers[i]} does not have a defined controller type, falling back to generic controller type");
                }

                return SupportedControllerType.GenericOpenVR;
            }
        }

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="xrNodeState">OpenVR Node State provided by the SDK</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private GenericOpenVRController GetOrAddController(XRNodeState xrNodeState)
        {
            // If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(xrNodeState.nodeType))
            {
                //TODO - Need logic to determine controller type (if possible)
                var controller = activeControllers[xrNodeState.nodeType];
                Debug.Assert(controller != null);
                return controller;
            }

            Handedness controllingHand;

            switch (xrNodeState.nodeType)
            {
                case XRNode.LeftHand:
                    controllingHand = Handedness.Left;
                    break;
                case XRNode.RightHand:
                    controllingHand = Handedness.Right;
                    break;
                default:
                    controllingHand = Handedness.None;
                    break;
            }

            Type controllerType;

            switch (CurrentControllerType)
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
                    controllerType = typeof(WindowsMixedRealityOpenVRController);
                    break;
                default:
                    Debug.LogError($"Unsupported controller type detected.");
                    return null;
            }

            var pointers = RequestPointers(controllerType, controllingHand);
            var inputSource = InputSystem?.RequestNewGenericInputSource($"{CurrentControllerType} Controller {controllingHand}", pointers);

            GenericOpenVRController detectedController = Activator.CreateInstance(controllerType, TrackingState.NotTracked, controllingHand, inputSource, null) as GenericOpenVRController;

            Debug.Assert(detectedController != null);
            detectedController?.SetupConfiguration(controllerType);

            for (int i = 0; i < detectedController?.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            activeControllers.Add(xrNodeState.nodeType, detectedController);
            return detectedController;
        }

        #endregion Controller Utilities
    }
}