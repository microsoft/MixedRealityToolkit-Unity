// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
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
        /// Dictionary to capture all active hands detected
        /// </summary>
        private readonly Dictionary<Handedness, SimulatedHand> trackedHands = new Dictionary<Handedness, SimulatedHand>();

        /// <summary>
        /// Active controllers
        /// </summary>
        private IMixedRealityController[] activeControllers = System.Array.Empty<IMixedRealityController>();

        /// <inheritdoc />
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers;
        }

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

        // Register input sources for hands based on hand data
        protected void UpdateHandDevice(HandSimulationMode simulationMode, Handedness handedness, SimulatedHandData handData)
        {
            if (handData != null && handData.IsTracked)
            {
                SimulatedHand controller = GetOrAddHandDevice(handedness, simulationMode);
                controller.UpdateState(handData);
            }
            else
            {
                RemoveHandDevice(handedness);
            }
        }

        public SimulatedHand GetHandDevice(Handedness handedness)
        {
            if (trackedHands.TryGetValue(handedness, out SimulatedHand controller))
            {
                return controller;
            }
            return null;
        }

        protected SimulatedHand GetOrAddHandDevice(Handedness handedness, HandSimulationMode simulationMode)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                if (controller.SimulationMode == simulationMode)
                {
                    return controller;
                }
                else
                {
                    // Remove and recreate hand device if simulation mode doesn't match
                    RemoveHandDevice(handedness);
                }
            }

            SupportedControllerType st = simulationMode == HandSimulationMode.Gestures ? SupportedControllerType.GGVHand : SupportedControllerType.ArticulatedHand;
            IMixedRealityPointer[] pointers = RequestPointers(st, handedness);

            var inputSource = Service?.RequestNewGenericInputSource($"{handedness} Hand", pointers, InputSourceType.Hand);
            switch (simulationMode)
            {
                case HandSimulationMode.Articulated:
                    controller = new SimulatedArticulatedHand(TrackingState.Tracked, handedness, inputSource);
                    break;
                case HandSimulationMode.Gestures:
                    controller = new SimulatedGestureHand(TrackingState.Tracked, handedness, inputSource);
                    break;
                default:
                    controller = null;
                    break;
            }

            System.Type controllerType = simulationMode == HandSimulationMode.Gestures ? typeof(SimulatedGestureHand) : typeof(SimulatedArticulatedHand);
            
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

            trackedHands.Add(handedness, controller);
            UpdateActiveControllers();

            return controller;
        }

        protected void RemoveHandDevice(Handedness handedness)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                Service?.RaiseSourceLost(controller.InputSource, controller);

                RecyclePointers(controller.InputSource);

                trackedHands.Remove(handedness);
                UpdateActiveControllers();
            }
        }

        protected void RemoveAllHandDevices()
        {
            foreach (var controller in trackedHands.Values)
            {
                Service?.RaiseSourceLost(controller.InputSource, controller);

                RecyclePointers(controller.InputSource);
            }

            trackedHands.Clear();
            UpdateActiveControllers();
        }

        private void UpdateActiveControllers()
        {
            activeControllers = trackedHands.Values.ToArray<IMixedRealityController>();
        }
    }
}
