// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WindowsMixedRealityDeviceManager : BaseDeviceManager
    {
        public WindowsMixedRealityDeviceManager(string name, uint priority) : base(name, priority) { }

        //Ignore - test only
        private bool experimental = false;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        #region IMixedRealityDeviceManager Interface

        /// <inheritdoc/>
        public override void Enable() => InitializeSources();

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
                RemoveWindowsMixedRealityController(states[i]);
            }
        }

        /// <inheritdoc/>
        public override IMixedRealityController[] GetActiveControllers()
        {
            var controllers = new IMixedRealityController[activeControllers.Count];
            int i = 0;
            foreach (var activeController in activeControllers.Values)
            {
                controllers[i] = activeController;
                i++;
            }

            return controllers;
        }

        #endregion IMixedRealityDeviceManager Interface

        #region Device Initialization

        /// <summary>
        /// The internal initialize function is used to register for controller events.
        /// </summary>
        private void InitializeSources()
        {
            #region Experimental_WSA native device input
            if (experimental)
            {
#if WINDOWS_UWP
                //TODO - kept for reference - clean later.
                var spatialManager = Windows.UI.Input.Spatial.SpatialInteractionManager.GetForCurrentView();
                spatialManager.SourceDetected += spatialManager_SourceDetected;
                spatialManager.SourceUpdated += SpatialManager_SourceUpdated;
                spatialManager.SourcePressed += SpatialManager_SourcePressed;
                spatialManager.SourceReleased += SpatialManager_SourceReleased;
                spatialManager.SourceLost += SpatialManager_SourceLost;
                spatialManager.InteractionDetected += SpatialManager_InteractionDetected;
#endif //WINDOWS_UWP
            }

            #endregion Experimental_WSA native device input

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;

            InteractionSourceState[] states = InteractionManager.GetCurrentReading();

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            for (var i = 0; i < states.Length; i++)
            {
                InteractionSourceDetected(states[i]);
            }
        }

        #endregion Device Initialization

        #region Mixed Reality controller handlers

        /// <summary>
        /// Retrieve the source controller from the Active Store, or create a new device and register it
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK</param>
        /// <returns>New or Existing Controller Input Source</returns>
        private WindowsMixedRealityController GetOrAddWindowsMixedRealityController(InteractionSourceState interactionSourceState)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSourceState.source.id))
            {
                WindowsMixedRealityController controller = activeControllers[interactionSourceState.source.id] as WindowsMixedRealityController;
                Debug.Assert(controller != null);
                controller.UpdateController(interactionSourceState);
                return controller;
            }

            Handedness controllingHand;
            switch (interactionSourceState.source.handedness)
            {
                case InteractionSourceHandedness.Unknown:
                    controllingHand = Handedness.None;
                    break;
                case InteractionSourceHandedness.Left:
                    controllingHand = Handedness.Left;
                    break;
                case InteractionSourceHandedness.Right:
                    controllingHand = Handedness.Right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IMixedRealityInputSource controllerInputSource = InputSystem?.RequestNewGenericInputSource($"Mixed Reality Controller {controllingHand}");

            //TODO - Controller Type Detection?
            //Define new Controller
            var detectedController = new WindowsMixedRealityController(ControllerState.NotTracked, controllingHand, controllerInputSource);

            //Load the Interaction Mappings for a controller
            detectedController.LoadConfiguration();

            activeControllers.Add(interactionSourceState.source.id, detectedController);

            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveWindowsMixedRealityController(InteractionSourceState interactionSourceState)
        {
            InputSystem?.RaiseSourceLost(GetOrAddWindowsMixedRealityController(interactionSourceState)?.InputSource);
            activeControllers.Remove(interactionSourceState.source.id);
        }

        /// <summary>
        /// Register a new controller in the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to add</param>
        private void InteractionSourceDetected(InteractionSourceState interactionSourceState)
        {
            InputSystem?.RaiseSourceDetected(GetOrAddWindowsMixedRealityController(interactionSourceState)?.InputSource);
        }

        /// <summary>
        /// Update the controller data from the Interaction Source update event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        private void InteractionSourceUpdated(InteractionSourceState state)
        {
            GetOrAddWindowsMixedRealityController(state)?.UpdateController(state);
        }

        /// <summary>
        /// Update the controller data from the Interaction Source Pressed event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pressType"></param>
        private void InteractionSourcePressed(InteractionSourceState state, InteractionSourcePressType pressType)
        {
            //GetOrAddWindowsMixedRealityController(state)?.UpdateController(state);
            //GetOrAddWindowsMixedRealityController(state)?.UpdateControllerPressed(pressType);
        }

        /// <summary>
        /// Update the controller data from the Interaction Source Released event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pressType"></param>
        private void InteractionSourceReleased(InteractionSourceState state, InteractionSourcePressType pressType)
        {
            //GetOrAddWindowsMixedRealityController(state)?.UpdateController(state);
            //GetOrAddWindowsMixedRealityController(state)?.UpdateControllerReleased(pressType);
        }

        #endregion

        #region Unity InteractionManager Events

        /// <summary>
        /// SDK Interaction Source Detected Event handler
        /// </summary>
        /// <param name="args">SDK source detected event arguments</param>
        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            InteractionSourceDetected(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Updated Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            InteractionSourceUpdated(args.state);
        }

        /// <summary>
        /// SDK Interaction Source Pressed Event handler
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            InteractionSourcePressed(args.state, args.pressType);
        }

        /// <summary>
        /// SDK Interaction Source Released Event handler
        /// </summary>
        /// <param name="args">SDK source released event arguments</param>
        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            InteractionSourceReleased(args.state, args.pressType);
        }

        /// <summary>
        /// SDK Interaction Source Lost Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveWindowsMixedRealityController(args.state);
        }

        #endregion Unity InteractionManager Events

        #region Experimental_PLAYER native device input

#if WINDOWS_UWP

        //TODO - kept for reference - clean later.
        private void spatialManager_SourceDetected(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        {
            //InteractionSourceDetected(args.state);
        }

        private void SpatialManager_SourceUpdated(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        {
            //InteractionSourceUpdated(args.state);
        }

        private void SpatialManager_SourcePressed(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        {
            //InteractionSourcePressed(args.state, args.pressType);
        }
        private void SpatialManager_SourceReleased(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        {
            //InteractionSourceReleased(args.state, args.pressType);
        }

        private void SpatialManager_SourceLost(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        {
            //RemoveWindowsMixedRealityController(args.state);
        }

        private void SpatialManager_InteractionDetected(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionDetectedEventArgs args)
        {
            //Not Implemented Yet
        }

#endif // WINDOWS_UWP

        #endregion Experimental_PLAYER native device input
    }
}
