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
        private IMixedRealityController[] activeControllers = new IMixedRealityController[0];

        /// <inheritdoc />
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers;
        }

        #region BaseInputDeviceManager Implementation

        public BaseInputSimulationService(
            IMixedRealityServiceRegistrar registrar,
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(registrar, inputSystem, name, priority, profile)
        {}

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

            var inputSource = InputSystem?.RequestNewGenericInputSource($"{handedness} Hand", pointers, InputSourceType.Hand);
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
            if (controller == null)
            {
                Debug.LogError($"Failed to create {controllerType} controller");
                return null;
            }

            if (!controller.SetupConfiguration(controllerType, InputSourceType.Hand))
            {
                // Controller failed to be setup correctly.
                Debug.LogError($"Failed to Setup {controllerType} controller");
                // Return null so we don't raise the source detected.
                return null;
            }

            for (int i = 0; i < controller.InputSource?.Pointers?.Length; i++)
            {
                controller.InputSource.Pointers[i].Controller = controller;
            }

            InputSystem?.RaiseSourceDetected(controller.InputSource, controller);

            trackedHands.Add(handedness, controller);
            UpdateActiveControllers();

            return controller;
        }

        protected void RemoveHandDevice(Handedness handedness)
        {
            var controller = GetHandDevice(handedness);
            if (controller != null)
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);

                trackedHands.Remove(handedness);
                UpdateActiveControllers();
            }
        }

        protected void RemoveAllHandDevices()
        {
            foreach (var controller in trackedHands.Values)
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);
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
