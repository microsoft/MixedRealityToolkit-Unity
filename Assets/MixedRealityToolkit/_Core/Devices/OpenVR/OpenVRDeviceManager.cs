// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
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
            InputTracking.nodeAdded += InputTracking_nodeAdded;
            InputTracking.nodeRemoved += InputTracking_nodeRemoved;
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

        #region Unity InteractionManager Events

        private void InputTracking_nodeAdded(XRNodeState obj)
        {
            if (IsNodeTypeSupported(obj))
            {
                GetOrAddController(obj);
            }
        }

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
                var controller = GetOrAddController(obj);
                InputSystem?.RaiseSourceLost(controller?.InputSource, controller);
            }
        }

        private void InputTracking_nodeRemoved(XRNodeState obj)
        {
            activeControllers.Remove(obj.nodeType);
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
                    switch (controllers[i])
                    {
                        case "OpenVR Controller - Left":
                        case "OpenVR Controller - Right":
                            return SupportedControllerType.GenericOpenVR;
                        case "OpenVR Controller(Oculus Rift CV1 (Left Controller)) - Left":
                        case "OpenVR Controller(Oculus Rift CV1 (Right Controller)) - Right":
                            return SupportedControllerType.OculusTouch;
                        case "OpenVR Controller(Oculus remote)":                                // TODO: Yet to test
                            return SupportedControllerType.OculusRemote;
                        case "Vive Wand - Left":                                                // TODO: Yet to test
                        case "Vive Wand - Right":                                               // TODO: Yet to test
                            return SupportedControllerType.ViveWand;
                        case "Valve Knuckles - Left":                                            // TODO: Yet to test
                        case "Valve Knuckles - Right":                                            // TODO: Yet to test
                            return SupportedControllerType.ValveKnuckles;
                        case "Oculus Remote":
                            return SupportedControllerType.OculusRemote;
                    }
                }

                return SupportedControllerType.None;
            }
        }

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="xrNodeState"></param>
        /// <returns>New or Existing Controller Input Source</returns>
        private GenericOpenVRController GetOrAddController(XRNodeState xrNodeState)
        {
            //If a device is already registered with the ID provided, just return it.
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

            IMixedRealityInputSource inputSource = InputSystem?.RequestNewGenericInputSource($"{CurrentControllerType} Controller {controllingHand}");

            GenericOpenVRController detectedController = null;

            // Initialize the controller base on the detected type
            switch (CurrentControllerType)
            {
                case SupportedControllerType.GenericOpenVR:
                    detectedController = new GenericOpenVRController(TrackingState.NotTracked, controllingHand, inputSource);
                    detectedController.SetupConfiguration(typeof(GenericOpenVRController));
                    break;
                case SupportedControllerType.ViveWand:
                    detectedController = new ViveWandController(TrackingState.NotTracked, controllingHand, inputSource);
                    detectedController.SetupConfiguration(typeof(ViveWandController));
                    break;
                case SupportedControllerType.ValveKnuckles:
                    detectedController = new ValveKnucklesController(TrackingState.NotTracked, controllingHand, inputSource);
                    detectedController.SetupConfiguration(typeof(ValveKnucklesController));
                    break;
                case SupportedControllerType.OculusTouch:
                    detectedController = new OculusTouchController(TrackingState.NotTracked, controllingHand, inputSource);
                    detectedController.SetupConfiguration(typeof(OculusTouchController));
                    break;
                case SupportedControllerType.OculusRemote:
                    detectedController = new OculusRemoteController(TrackingState.NotTracked, controllingHand, inputSource);
                    detectedController.SetupConfiguration(typeof(OculusTouchController));
                    break;
            }

            Debug.Assert(detectedController != null);
            detectedController.UpdateController(xrNodeState);
            activeControllers.Add(xrNodeState.nodeType, detectedController);
            return detectedController;
        }

        #endregion Controller Utilities
    }
}