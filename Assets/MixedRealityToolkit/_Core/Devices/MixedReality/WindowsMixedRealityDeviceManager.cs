// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System.Collections.Generic;
using System.Linq;
#if UNITY_WSA
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using System;
using UnityEngine.XR.WSA.Input;
using UnityEngine;
#endif // UNITY_WSA

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WindowsMixedRealityDeviceManager : BaseDeviceManager
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="priority"></param>
        public WindowsMixedRealityDeviceManager(string name, uint priority) : base(name, priority) { }

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.Values.ToArray();
        }

#if UNITY_WSA

        #region IMixedRealityDeviceManager Interface

        /// <inheritdoc/>
        public override void Enable()
        {
            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < states.Length; i++)
            {
                var controller = GetOrAddController(states[i]);

                if (controller != null)
                {
                    controller.UpdateController(states[i]);
                    InputSystem.RaiseSourceDetected(controller.InputSource, controller);
                }
            }
        }

        /// <inheritdoc/>
        public override void Disable()
        {
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated -= InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();
            for (var i = 0; i < states.Length; i++)
            {
                RemoveController(states[i]);
            }
        }

        #endregion IMixedRealityDeviceManager Interface

        #region Controller Utilities

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private WindowsMixedRealityController GetOrAddController(InteractionSourceState interactionSourceState)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSourceState.source.id))
            {
                var controller = activeControllers[interactionSourceState.source.id] as WindowsMixedRealityController;
                Debug.Assert(controller != null);
                return controller;
            }

            Handedness controllingHand;
            switch (interactionSourceState.source.handedness)
            {
                default:
                    controllingHand = Handedness.None;
                    break;
                case InteractionSourceHandedness.Left:
                    controllingHand = Handedness.Left;
                    break;
                case InteractionSourceHandedness.Right:
                    controllingHand = Handedness.Right;
                    break;
            }

            IMixedRealityPointer[] pointers = interactionSourceState.source.supportsPointing ? RequestPointers(typeof(WindowsMixedRealityController), controllingHand) : null;
            var inputSource = InputSystem?.RequestNewGenericInputSource($"Mixed Reality Controller {controllingHand}", pointers);
            var detectedController = new WindowsMixedRealityController(TrackingState.NotTracked, controllingHand, inputSource);

            if (!detectedController.SetupConfiguration(typeof(WindowsMixedRealityController)))
            {
                // Controller failed to be setup correctly.
                // Return null so we don't raise the source detected.
                return null;
            }

            Debug.Assert(detectedController != null);

            for (int i = 0; i < detectedController.InputSource?.Pointers?.Length; i++)
            {
                detectedController.InputSource.Pointers[i].Controller = detectedController;
            }

            activeControllers.Add(interactionSourceState.source.id, detectedController);
            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveController(InteractionSourceState interactionSourceState)
        {
            var controller = GetOrAddController(interactionSourceState);

            if (controller != null)
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);
            }

            activeControllers.Remove(interactionSourceState.source.id);
        }

        #endregion Controller Utilities

        #region Unity InteractionManager Events

        /// <summary>
        /// SDK Interaction Source Detected Event handler
        /// </summary>
        /// <param name="args">SDK source detected event arguments</param>
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            var controller = GetOrAddController(args.state);

            if (controller != null)
            {
                InputSystem?.RaiseSourceDetected(controller.InputSource, controller);
            }

            controller?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Updated Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            GetOrAddController(args.state)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Pressed Event handler
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            GetOrAddController(args.state)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Released Event handler
        /// </summary>
        /// <param name="args">SDK source released event arguments</param>
        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            GetOrAddController(args.state)?.UpdateController(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Lost Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveController(args.state);
        }

        #endregion Unity InteractionManager Events

#endif // UNITY_WSA

    }
}