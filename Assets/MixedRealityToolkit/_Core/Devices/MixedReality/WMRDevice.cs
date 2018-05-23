// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WMRDevice : IMixedRealityDevice
    {
        /// <summary>
        /// Dictionary to capture all active controllers detected
        /// </summary>
        private readonly Dictionary<uint, IMixedRealityController> activeControllers = new Dictionary<uint, IMixedRealityController>();

        /// <summary>
        /// Input System reference
        /// </summary>
        private IMixedRealityInputSystem inputSystem;

        /// <summary>
        /// Public accessor for the currently attached controllers for a Windows Mixed Reality Device
        /// </summary>
        /// <returns></returns>
        public IMixedRealityController[] GetActiveControllers()
        {
            return activeControllers.ExportDictionaryValuesAsArray();
        }

        #region Device Initialization


        // TODO To be determined if needed
        /// <summary>
        /// Public Contructor
        /// </summary>
        public WMRDevice()
        {
            // TODO - Discover available controllers?
            // ForEach then initialise each
        }

        /// <summary>
        /// The initialize function is used to setup the device once created.
        /// This method is called once all managers have been registered in the Mixed Reality Manager.
        /// </summary>
        public void Initialize()
        {
            inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();

            InitializeSources();
        }

        /// <summary>
        /// The internal initialize function is used to register for controller events.
        /// </summary>
        private void InitializeSources()
        {
            //Tried but it fails to build and causes errors in Unity :S (recognized in the Player project)
            //var spatialManager = Windows.UI.Input.Spatial.SpatialInteractionManager.GetForCurrentView();
            //spatialManager.SourceDetected += spatialManager_SourceDetected;
            //spatialManager.SourcePressed += SpatialManager_SourcePressed;

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceUpdated += InteractionManager_InteractionSourceUpdated;
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


        //TODO - kept for reference - clean later.
        //private void SpatialManager_SourcePressed(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        //private void spatialManager_SourceDetected(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        #region Mixed Reality controller handlers

        // TODO (understatement)
        // 1 - Test
        // 2 - Refactor for multi-controller types (possibly as part of the inputsource interface)

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

            //TODO - Controller Type Detection?
            //Define new Controller
            var detectedController = new WindowsMixedRealityController(
                interactionSourceState.source.id,
                interactionSourceState.source.handedness == InteractionSourceHandedness.Left ? Handedness.Left : Handedness.Right
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

            inputSystem.RaiseSourceLost(controller.InputSource);
            activeControllers.Remove(interactionSourceState.source.id);
        }

        /// <summary>
        /// Register a new controller in the Active Store
        /// </summary>
        /// <param name="interactionSourceState">Source State provided by the SDK to add</param>
        private void InteractionSourceDetected(InteractionSourceState interactionSourceState)
        {
            var controller = GetOrAddWindowsMixedRealityController(interactionSourceState);
            if (controller == null) { return; }

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            controller.UpdateInputSource(interactionSourceState);
            inputSystem.RaiseSourceDetected(controller.InputSource);
        }

        // TODO - Need to resolve InputAction
        /// <summary>
        /// React to Input "Press" events and update source data
        /// </summary>
        /// <param name="interactionSourcePressType">Type of press event received</param>
        /// <param name="controller">Source controller to update</param>
        /// <returns></returns>
        //private InputAction PressInteractionSource(InteractionSourcePressType interactionSourcePressType, IMixedRealityController controller)
        //{
        //    DeviceInputType pressedInput;

        //    switch (interactionSourcePressType)
        //    {
        //        case InteractionSourcePressType.None:
        //            pressedInput = DeviceInputType.None;
        //            break;
        //        case InteractionSourcePressType.Select:
        //            pressedInput = DeviceInputType.Select;
        //            break;
        //        case InteractionSourcePressType.Menu:
        //            pressedInput = DeviceInputType.Menu;
        //            break;
        //        case InteractionSourcePressType.Grasp:
        //            pressedInput = DeviceInputType.GripPress;
        //            break;
        //        case InteractionSourcePressType.Touchpad:
        //            pressedInput = DeviceInputType.TouchpadPress;
        //            break;
        //        case InteractionSourcePressType.Thumbstick:
        //            pressedInput = DeviceInputType.ThumbStickPress;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //    if (controller.Interactions.ContainsKey(pressedInput)) controller.Interactions[pressedInput].SetValue(true);
        //    return (InputType)Enum.Parse(typeof(InputType), pressedInput.ToString());
        //}

        // TODO - Need to resolve InputAction
        /// <summary>
        /// React to Input "Release" events and update source data
        /// </summary>
        /// <param name="interactionSourcePressType">Type of release event received</param>
        /// <param name="controller">Source controller to update</param>
        /// <returns></returns>
        //private InputAction ReleaseInteractionSource(InteractionSourcePressType interactionSourcePressType, IMixedRealityController controller)
        //{
        //    DeviceInputType releasedInput;

        //    switch (interactionSourcePressType)
        //    {
        //        case InteractionSourcePressType.None:
        //            releasedInput = DeviceInputType.None;
        //            break;
        //        case InteractionSourcePressType.Select:
        //            releasedInput = DeviceInputType.Select;
        //            break;
        //        case InteractionSourcePressType.Menu:
        //            releasedInput = DeviceInputType.Menu;
        //            break;
        //        case InteractionSourcePressType.Grasp:
        //            releasedInput = DeviceInputType.GripPress;
        //            break;
        //        case InteractionSourcePressType.Touchpad:
        //            releasedInput = DeviceInputType.TouchpadPress;
        //            break;
        //        case InteractionSourcePressType.Thumbstick:
        //            releasedInput = DeviceInputType.ThumbStickPress;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //    if (controller.Interactions.ContainsKey(releasedInput)) controller.Interactions[releasedInput].SetValue(false);
        //    return (InputType)Enum.Parse(typeof(InputType), releasedInput.ToString());
        //}
        #endregion

        #region InteractionManager Events

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
            var controller = GetOrAddWindowsMixedRealityController(args.state);

            if (controller == null) { return; }

            controller.UpdateInputSource(args.state);

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.SpatialPointer].Changed)
            {
                // TODO - Need to resolve InputAction
                //inputSystem.Raise6DofInputChanged(controller.InputSource, controller.ControllerHandedness, InputType.SpatialPointer, controller.Interactions[DeviceInputType.SpatialPointer].GetValue<Tuple<Vector3, Quaternion>>());
            }

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.SpatialGrip].Changed)
            {
                // TODO - Need to resolve InputAction
                //inputSystem.Raise6DofInputChanged(controller.InputSource, controller.ControllerHandedness, InputType.SpatialGrip, controller.Interactions[DeviceInputType.SpatialGrip].GetValue<Tuple<Vector3, Quaternion>>());
            }

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.TouchpadTouch].Changed)
            {
                if (controller.Interactions[DeviceInputType.TouchpadTouch].GetValue<bool>())
                {
                    // TODO - Need to resolve InputAction
                    //inputSystem.RaiseOnInputDown(controller.InputSource, controller.ControllerHandedness, InputType.TouchpadTouch);
                }
                else
                {
                    // TODO - Need to resolve InputAction
                    //inputSystem.RaiseOnInputUp(controller.InputSource, controller.ControllerHandedness, InputType.TouchpadTouch);
                }
            }

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.Touchpad].Changed)
            {
                // TODO - Need to resolve InputAction
                //inputSystem.Raise2DoFInputChanged(controller.InputSource, controller.ControllerHandedness, InputType.Touchpad, controller.Interactions[DeviceInputType.Touchpad].GetValue<Vector2>());
            }

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.ThumbStick].Changed)
            {
                // TODO - Need to resolve InputAction
                //inputSystem.Raise2DoFInputChanged(controller.InputSource, controller.ControllerHandedness, InputType.ThumbStick, controller.Interactions[DeviceInputType.ThumbStick].GetValue<Vector2>());
            }

            if (controller.Interactions.ContainsKey(DeviceInputType.SpatialPointer) && controller.Interactions[DeviceInputType.Trigger].Changed)
            {
                // TODO - Need to resolve InputAction
                //inputSystem.RaiseOnInputPressed(controller.InputSource, controller.ControllerHandedness, InputType.Select, controller.Interactions[DeviceInputType.Trigger].GetValue<float>());
            }
        }

        /// <summary>
        /// SDK Interaction Source Pressed Event handler
        /// </summary>
        /// <param name="args">SDK source pressed event arguments</param>
        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            var controller = GetOrAddWindowsMixedRealityController(args.state);
            if (controller == null) { return; }

            // TODO - Need to resolve InputAction
            //inputSystem.RaiseOnInputDown(controller.InputSource, controller.ControllerHandedness, PressInteractionSource(args.pressType, controller));
        }

        /// <summary>
        /// SDK Interaction Source Released Event handler
        /// </summary>
        /// <param name="args">SDK source released event arguments</param>
        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            var controller = GetOrAddWindowsMixedRealityController(args.state);
            if (controller == null) { return; }

            // TODO - Need to resolve InputAction
            //inputSystem.RaiseOnInputUp(controller.InputSource, controller.ControllerHandedness, ReleaseInteractionSource(args.pressType, controller));
        }

        /// <summary>
        /// SDK Interaction Source Lost Event handler
        /// </summary>
        /// <param name="args">SDK source updated event arguments</param>
        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveWindowsMixedRealityController(args.state);
        }

        #endregion InteractionManager Events

        #region Runtime

        //TODO - runtime needs more thought

        public void Enable()
        {
            InitializeSources();
        }

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

        public void Destroy()
        {
            //Destroy Stuff
        }

        #endregion
    }
}