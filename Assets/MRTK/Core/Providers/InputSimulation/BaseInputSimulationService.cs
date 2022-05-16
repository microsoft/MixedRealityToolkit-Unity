// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Base class for services that create simulated input devices.
    /// </summary>
    public abstract class BaseInputSimulationService : BaseInputDeviceManager
    {
        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<Handedness, BaseController> trackedControllers = new Dictionary<Handedness, BaseController>();

        /// <summary>
        /// Active controllers
        /// </summary>
        private IMixedRealityController[] activeControllers = Array.Empty<IMixedRealityController>();

        /// <inheritdoc />
        public override IMixedRealityController[] GetActiveControllers() => activeControllers;

        #region BaseInputDeviceManager Implementation

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="registrar">The <see cref="IMixedRealityServiceRegistrar"/> instance that loaded the data provider.</param>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        [System.Obsolete("This constructor is obsolete (registrar parameter is no longer required) and will be removed in a future version of the Microsoft Mixed Reality Toolkit.")]
        protected BaseInputSimulationService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : this(inputSystem, name, priority, profile)
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
        protected BaseInputSimulationService(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile)
        { }

        #endregion BaseInputDeviceManager Implementation

        // Register input sources for controllers based on controller data
        protected void UpdateControllerDevice(ControllerSimulationMode simulationMode, Handedness handedness, object controllerData)
        {
            if (controllerData != null)
            {
                if (controllerData is SimulatedHandData handData && handData.IsTracked)
                {

                    SimulatedHand hand = GetOrAddControllerDevice(handedness, simulationMode) as SimulatedHand;
                    hand.UpdateState(handData);
                    return;

                }
                else if (controllerData is SimulatedMotionControllerData motionControllerData && motionControllerData.IsTracked)
                {
                    SimulatedMotionController motionController = GetOrAddControllerDevice(handedness, simulationMode) as SimulatedMotionController;
                    motionController.UpdateState(motionControllerData);
                    return;
                }
            }

            RemoveControllerDevice(handedness);
        }

        public BaseController GetControllerDevice(Handedness handedness)
        {
            if (trackedControllers.TryGetValue(handedness, out BaseController controller))
            {
                return controller;
            }
            return null;
        }

        protected BaseController GetOrAddControllerDevice(Handedness handedness, ControllerSimulationMode simulationMode)
        {
            var controller = GetControllerDevice(handedness);
            if (controller != null)
            {
                if (controller is SimulatedHand hand && hand.SimulationMode == simulationMode)
                {
                    return controller;
                }
                else if (controller is SimulatedMotionController && simulationMode == ControllerSimulationMode.MotionController)
                {
                    return controller;
                }
                else
                {
                    // Remove and recreate controller device if simulation mode doesn't match
                    RemoveControllerDevice(handedness);
                }
            }

            DebugUtilities.LogVerboseFormat("GetOrAddControllerDevice: Adding a new simulated controller of handedness {0} and simulation mode {1}", handedness, simulationMode);
            System.Type controllerType = null;

            SupportedControllerType st;
            IMixedRealityInputSource inputSource;
            switch (simulationMode)
            {
                case ControllerSimulationMode.HandGestures:
                    st = SupportedControllerType.GGVHand;
                    inputSource = Service?.RequestNewGenericInputSource($"Simulated GGV {handedness} Hand", RequestPointers(st, handedness), InputSourceType.Hand);
                    controller = new SimulatedGestureHand(TrackingState.Tracked, handedness, inputSource);
                    controllerType = typeof(SimulatedGestureHand);
                    break;
                case ControllerSimulationMode.ArticulatedHand:
                    st = SupportedControllerType.ArticulatedHand;
                    inputSource = Service?.RequestNewGenericInputSource($"Simulated Articulated {handedness} Hand", RequestPointers(st, handedness), InputSourceType.Hand);
                    controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);
                    controllerType = typeof(SimulatedArticulatedHand);
                    break;
                case ControllerSimulationMode.MotionController:
                    st = SupportedControllerType.WindowsMixedReality;
                    inputSource = Service?.RequestNewGenericInputSource($"Simulated {handedness} MotionController", RequestPointers(st, handedness), InputSourceType.Controller);
                    controller = new SimulatedMotionController(TrackingState.Tracked, handedness, inputSource);
                    controllerType = typeof(SimulatedMotionController);
                    break;
                default:
                    controller = null;
                    break;
            }

            if (controller == null || !controller.Enabled)
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to create {controllerType} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            Service?.RaiseSourceDetected(controller.InputSource, controller);

            trackedControllers.Add(handedness, controller);
            UpdateActiveControllers();

            return controller;
        }

        protected void RemoveControllerDevice(Handedness handedness)
        {
            var controller = GetControllerDevice(handedness);
            if (controller != null)
            {
                DebugUtilities.LogVerboseFormat("RemoveHandDevice: Removing simulated controller of handedness", handedness);

                Service?.RaiseSourceLost(controller.InputSource, controller);

                RecyclePointers(controller.InputSource);

                trackedControllers.Remove(handedness);
                UpdateActiveControllers();
            }
        }

        protected void RemoveAllControllerDevices()
        {
            foreach (var controller in trackedControllers.Values)
            {
                Service?.RaiseSourceLost(controller.InputSource, controller);

                RecyclePointers(controller.InputSource);
            }

            trackedControllers.Clear();
            UpdateActiveControllers();
        }

        private void UpdateActiveControllers()
        {
            activeControllers = trackedControllers.Values.ToArray<IMixedRealityController>();
        }

        #region Obsolete Methods

        // Register input sources for hands based on hand data
        [Obsolete("Use UpdateControllerDevice instead.")]
        protected void UpdateHandDevice(ControllerSimulationMode simulationMode, Handedness handedness, SimulatedHandData handData)
        {
            UpdateControllerDevice(simulationMode, handedness, handData);
        }

        [Obsolete("Use GetControllerDevice instead.")]
        public SimulatedHand GetHandDevice(Handedness handedness)
        {
            return GetControllerDevice(handedness) as SimulatedHand;
        }

        [Obsolete("Use GetOrAddControllerDevice instead.")]
        protected SimulatedHand GetOrAddHandDevice(Handedness handedness, ControllerSimulationMode simulationMode)
        {
            return GetOrAddControllerDevice(handedness, simulationMode) as SimulatedHand;
        }

        [Obsolete("Use RemoveControllerDevice instead.")]
        protected void RemoveHandDevice(Handedness handedness)
        {
            RemoveControllerDevice(handedness);
        }

        [Obsolete("Use RemoveAllControllerDevices instead.")]
        protected void RemoveAllHandDevices()
        {
            RemoveAllControllerDevices();
        }

        #endregion
    }
}
