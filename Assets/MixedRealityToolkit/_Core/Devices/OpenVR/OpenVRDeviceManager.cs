// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
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
        private readonly Dictionary<XRNode, IMixedRealityController> activeControllers = new Dictionary<XRNode, IMixedRealityController>();

        /// <summary>
        /// Tracking states returned from the InputTracking state tracking manager
        /// </summary>
        private List<XRNodeState> nodeStates = new List<XRNodeState>();

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.Values.ToArray();
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
            GenericOpenVRController controller;
            InputTracking.GetNodeStates(nodeStates);
            for (int i = 0; i < nodeStates.Count; i++)
            {
                if (IsNodeTypeSupported(nodeStates[i]) && activeControllers.ContainsKey(nodeStates[i].nodeType))
                {
                    //TODO - Need logic to determine controller type (if possible)
                    controller = activeControllers[nodeStates[i].nodeType] as GenericOpenVRController;
                    if(controller.Enabled) controller.UpdateController(nodeStates[i]);
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
                var controller = GetOrAddController(obj);
                InputSystem?.RaiseSourceDetected(controller?.InputSource, controller);
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
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private IMixedRealityController GetOrAddController(XRNodeState xrNodeState)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(xrNodeState.nodeType))
            {
                //TODO - Need logic to determine controller type (if possible)
                var controller = activeControllers[xrNodeState.nodeType];
                Debug.Assert(controller != null);
                return controller;
            }

            //TODO - hand detection
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

            var inputSource = InputSystem?.RequestNewGenericInputSource($"Generic OpenVR Controller {controllingHand}");
            
            //TODO - Need logic to determine controller type (if possible)
            var detectedController = new GenericOpenVRController(TrackingState.NotTracked, controllingHand, inputSource);
            detectedController.SetupConfiguration(typeof(GenericOpenVRController));
            detectedController.UpdateController(xrNodeState);
            activeControllers.Add(xrNodeState.nodeType, detectedController);

            return detectedController;
        }

        #endregion Controller Utilities
    }
}