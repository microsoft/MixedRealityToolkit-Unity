// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    internal class WindowsMixedRealityDeviceManager : IMixedRealityDevice
    {
        public WindowsMixedRealityDeviceManager(string name, uint priority)
        {
            Name = name;
            Priority = priority;
        }

        //Ignore - test only
        private bool experimental = false;

        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <summary>
        /// Input System reference
        /// </summary>
        /// <remarks>Ensure all uses of this reference are covered by a NULL check (inputsystem?) as it is not guaranteed there will be one at runtime</remarks>
        private IMixedRealityInputSystem inputSystem;

        #region IMixedRealityManager Interface

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public uint Priority { get; }

        /// <inheritdoc/>
        public void Initialize()
        {
            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem)
            {
                inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
            }

            InitializeSources();
        }

        /// <inheritdoc/>
        public void Reset() { }

        /// <inheritdoc/>
        public void Enable()
        {
            InitializeSources();
        }

        /// <inheritdoc/>
        public void Update() { }

        /// <inheritdoc/>
        public void Disable()
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
        public void Destroy()
        {
            //Destroy Stuff
        }

        #endregion IMixedRealityManager Interface

        #region IMixedRealityDevice Interface

        /// <inheritdoc/>
        public IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.ExportDictionaryValuesAsArray();
        }

        #endregion IMixedRealityDevice Interface

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
        private IMixedRealityController GetOrAddWindowsMixedRealityController(InteractionSourceState interactionSourceState)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSourceState.source.id))
            {
                return activeControllers[interactionSourceState.source.id];
            }

            var controllingHand = interactionSourceState.source.handedness == InteractionSourceHandedness.Left ? Handedness.Left : Handedness.Right;
            IMixedRealityInputSource controllerInputSource = inputSystem?.RequestNewGenericInputSource($"Mixed Reality Controller {controllingHand}");

            //TODO - Controller Type Detection?
            //Define new Controller
            var detectedController = new WindowsMixedRealityController(
                ControllerState.NotTracked,
                controllingHand,
                controllerInputSource,
                new Dictionary<DeviceInputType, InteractionMapping>()
                );

            detectedController.SetupInputSource(interactionSourceState);

            activeControllers.Add(interactionSourceState.source.id, detectedController);

            return detectedController;
        }

        /// <summary>
        /// Remove the selected controller from the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to remove</param>
        private void RemoveWindowsMixedRealityController(InteractionSourceState interactionSourceState)
        {
            var controller = GetOrAddWindowsMixedRealityController(interactionSourceState);
            if (controller == null) { return; }

            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem) inputSystem?.RaiseSourceLost(controller.InputSource);
            activeControllers.Remove(interactionSourceState.source.id);
        }

        /// <summary>
        /// Register a new controller in the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to add</param>
        private void InteractionSourceDetected(InteractionSourceState interactionSourceState)
        {
            var controller = GetOrAddWindowsMixedRealityController(interactionSourceState);
            if (controller?.InputSource == null) { return; }

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            controller.UpdateInputSource(interactionSourceState);
            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem) inputSystem?.RaiseSourceDetected(controller.InputSource);
        }

        /// <summary>
        /// Update the controller data from the Interaction Source update event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        private void InteractionSourceUpdated(InteractionSourceState state)
        {
            var controller = GetOrAddWindowsMixedRealityController(state);

            if (controller == null) { return; }

            controller.UpdateInputSource(state);

            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem)
            {
                if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.SpatialPointer))
                {
                    inputSystem?.Raise6DofInputChanged(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.SpatialPointer].InputAction, controller.Interactions[DeviceInputType.SpatialPointer].GetTransform());
                }

                if (controller.Interactions.ContainsKey(DeviceInputType.SpatialGrip) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.SpatialGrip))
                {
                    inputSystem?.Raise6DofInputChanged(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.SpatialGrip].InputAction, controller.Interactions[DeviceInputType.SpatialGrip].GetTransform());
                }

                if (controller.Interactions.ContainsKey(DeviceInputType.TouchpadTouch) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.TouchpadTouch))
                {
                    if (controller.Interactions[DeviceInputType.TouchpadTouch].GetBooleanValue())
                    {
                        inputSystem?.RaiseOnInputDown(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.TouchpadTouch].InputAction);
                    }
                    else
                    {
                        inputSystem?.RaiseOnInputUp(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.TouchpadTouch].InputAction);
                    }
                }

                if (controller.Interactions.ContainsKey(DeviceInputType.Touchpad) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.Touchpad))
                {
                    inputSystem?.Raise2DoFInputChanged(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.Touchpad].InputAction, controller.Interactions[DeviceInputType.Touchpad].GetVector2Value());
                }

                if (controller.Interactions.ContainsKey(DeviceInputType.ThumbStick) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.ThumbStick))
                {
                    inputSystem?.Raise2DoFInputChanged(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.ThumbStick].InputAction, controller.Interactions[DeviceInputType.ThumbStick].GetVector2Value());
                }

                if (controller.Interactions.ContainsKey(DeviceInputType.Trigger) && controller.Interactions.GetDictionaryValueChanged(DeviceInputType.Trigger))
                {
                    inputSystem?.RaiseOnInputPressed(controller.InputSource, controller.ControllerHandedness, controller.Interactions[DeviceInputType.Select].InputAction, controller.Interactions[DeviceInputType.Trigger].GetFloatValue());
                }
            }
        }

        /// <summary>
        /// Update the controller data from the Interaction Source Pressed event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pressType"></param>
        private void InteractionSourcePressed(InteractionSourceState state, InteractionSourcePressType pressType)
        {
            var controller = GetOrAddWindowsMixedRealityController(state);
            if (controller == null) { return; }

            var inputAction = PressInteractionSource(pressType, controller);


            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem) inputSystem?.RaiseOnInputDown(controller.InputSource, controller.ControllerHandedness, inputAction);
        }

        /// <summary>
        /// Update the controller data from the Interaction Source Released event and raise input events on change
        /// </summary>
        /// <param name="state"></param>
        /// <param name="pressType"></param>
        private void InteractionSourceReleased(InteractionSourceState state, InteractionSourcePressType pressType)
        {
            var controller = GetOrAddWindowsMixedRealityController(state);
            if (controller == null) { return; }

            var inputAction = ReleaseInteractionSource(pressType, controller);


            if (MixedRealityManager.Instance.ActiveProfile.EnableInputSystem) inputSystem?.RaiseOnInputUp(controller.InputSource, controller.ControllerHandedness, inputAction);
        }

        /// <summary>
        /// React to Input "Press" events and update source data
        /// </summary>
        /// <param name="interactionSourcePressType">Type of press event received</param>
        /// <param name="controller">Source controller to update</param>
        /// <returns></returns>
        private InputAction PressInteractionSource(InteractionSourcePressType interactionSourcePressType, IMixedRealityController controller)
        {
            DeviceInputType pressedInput;

            switch (interactionSourcePressType)
            {
                case InteractionSourcePressType.None:
                    pressedInput = DeviceInputType.None;
                    break;
                case InteractionSourcePressType.Select:
                    pressedInput = DeviceInputType.Select;
                    break;
                case InteractionSourcePressType.Menu:
                    pressedInput = DeviceInputType.Menu;
                    break;
                case InteractionSourcePressType.Grasp:
                    pressedInput = DeviceInputType.GripPress;
                    break;
                case InteractionSourcePressType.Touchpad:
                    pressedInput = DeviceInputType.TouchpadPress;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    pressedInput = DeviceInputType.ThumbStickPress;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (controller.Interactions.ContainsKey(pressedInput))
            {
                controller.Interactions.SetDictionaryValue(pressedInput, true);
                return controller.Interactions[pressedInput].InputAction;
            }

            // if no mapping found, no action can take place
            return null;
        }

        /// <summary>
        /// React to Input "Release" events and update source data
        /// </summary>
        /// <param name="interactionSourcePressType">Type of release event received</param>
        /// <param name="controller">Source controller to update</param>
        /// <returns></returns>
        private InputAction ReleaseInteractionSource(InteractionSourcePressType interactionSourcePressType, IMixedRealityController controller)
        {
            DeviceInputType releasedInput;

            switch (interactionSourcePressType)
            {
                case InteractionSourcePressType.None:
                    releasedInput = DeviceInputType.None;
                    break;
                case InteractionSourcePressType.Select:
                    releasedInput = DeviceInputType.Select;
                    break;
                case InteractionSourcePressType.Menu:
                    releasedInput = DeviceInputType.Menu;
                    break;
                case InteractionSourcePressType.Grasp:
                    releasedInput = DeviceInputType.GripPress;
                    break;
                case InteractionSourcePressType.Touchpad:
                    releasedInput = DeviceInputType.TouchpadPress;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    releasedInput = DeviceInputType.ThumbStickPress;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (controller.Interactions.ContainsKey(releasedInput))
            {
                controller.Interactions.SetDictionaryValue(releasedInput, false);
                return controller.Interactions[releasedInput].InputAction;
            }

            // if no mapping found, no action can take place
            return null;
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
