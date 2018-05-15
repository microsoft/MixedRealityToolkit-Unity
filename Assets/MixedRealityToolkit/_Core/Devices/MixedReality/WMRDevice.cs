// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions;
using Microsoft.MixedReality.Toolkit.Internal.Extensions;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WMRDevice : IMixedRealityDevice
    {
        private readonly Dictionary<uint, IMixedRealityInputSource> activeControllers = new Dictionary<uint, IMixedRealityInputSource>();
        Vector3 controllerPosition, pointerPosition, gripPosition = Vector3.zero;
        Quaternion controllerRotation, pointerRotation, gripRotation = Quaternion.identity;
        private IMixedRealityInputSystem inputSystem;


        public IMixedRealityInputSource[] GetActiveControllers()
        {
            return activeControllers.ExportDictionaryValuesAsArray();
        }

        public WMRDevice()
        {

        }

        public void Initialize()
        {
            inputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();

            InitializeSources();
        }

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
                SourceDetected(states[i]);
            }
        }

        //TODO - kept for reference - clean later.
        //private void SpatialManager_SourcePressed(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        //private void spatialManager_SourceDetected(Windows.UI.Input.Spatial.SpatialInteractionManager sender, Windows.UI.Input.Spatial.SpatialInteractionSourceEventArgs args)
        //{
        //    throw new NotImplementedException();
        //}

        #region New Handlers

        // TODO (understatement)
        // 1 - Add InteractionDefinitions for Position, Pointer, Grip and Buttons
        // 2 - Check logic for retrieving an Interaction
        // 3 - Update the Update logic to refresh data

        private IMixedRealityInputSource GetOrAddWindowsMixedRealityController(InteractionSourceState interactionSourceState)
        {
            //If a device is already registered with the ID provided, just return it.
            if (activeControllers.ContainsKey(interactionSourceState.source.id))
            {
                return activeControllers[interactionSourceState.source.id];
            }

            bool controllerTracked = ReadMixedRealityControllerValues(ref interactionSourceState);

            // Define Controller capabilities
            var controllerCapabilities = new List<InputType>();

            // Add the Controller trigger
            controllerCapabilities.Add(InputType.Trigger);

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            if (interactionSourceState.source.supportsGrasp)
            {
                controllerCapabilities.Add(InputType.Grip);
            }

            // If the controller has a menu button, add it to the controller capabilities
            if (interactionSourceState.source.supportsMenu)
            {
                controllerCapabilities.Add(InputType.Menu);
            }

            // If the controller has a Thumbstick, add it to the controller capabilities
            if (interactionSourceState.source.supportsThumbstick)
            {
                controllerCapabilities.Add(InputType.ThumbStick);
                controllerCapabilities.Add(InputType.ThumbStickPress);
            }

            // If the controller has a Touchpad, add it to the controller capabilities
            if (interactionSourceState.source.supportsTouchpad)
            {
                controllerCapabilities.Add(InputType.Touchpad);
                controllerCapabilities.Add(InputType.TouchpadTouch);
                controllerCapabilities.Add(InputType.TouchpadPress);
            }

            var DetectedController = new WindowsMixedRealityController()
            {
                SourceId = interactionSourceState.source.id,
                Handedness = interactionSourceState.source.handedness == InteractionSourceHandedness.Left ? Handedness.Left : Handedness.Right,
                InputSourceState = controllerTracked ? InputSourceState.Tracked : InputSourceState.NotTracked,
                Capabilities = controllerCapabilities.ToArray()
            };

            activeControllers.Add(interactionSourceState.source.id, DetectedController);

            return DetectedController;
        }

        private bool ReadMixedRealityControllerValues(ref InteractionSourceState interactionSourceState)
        {
            // Get Controller start position
            bool controllerTracked = interactionSourceState.sourcePose.TryGetPosition(out controllerPosition);
            Debug.Log($"Controller Position on Read [{controllerPosition}]");
            // Get Controller start rotation
            interactionSourceState.sourcePose.TryGetRotation(out controllerRotation);

            // Get Pointer start position
            interactionSourceState.sourcePose.TryGetPosition(out pointerPosition, InteractionSourceNode.Pointer);

            // Get Pointer start rotation
            interactionSourceState.sourcePose.TryGetRotation(out pointerRotation, InteractionSourceNode.Pointer);

            // Get Grip start position
            interactionSourceState.sourcePose.TryGetPosition(out gripPosition, InteractionSourceNode.Grip);

            // Get Grip start rotation
            interactionSourceState.sourcePose.TryGetRotation(out gripRotation, InteractionSourceNode.Grip);
            return controllerTracked;
        }

        private void RemoveWindowsMixedRealityController(WindowsMixedRealityController interactionSource)
        {
            if (interactionSource.SourceId == 0) { return; }

            inputSystem.RaiseSourceLost(interactionSource);
            activeControllers.Remove(interactionSource.SourceId);
        }

        //private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
        //{
        //    Debug.Log($"Source [{obj.pressType.ToString()}] Pressed");
        //    switch (obj.pressType)
        //    {
        //        case InteractionSourcePressType.Select:
        //            SubmitSelectAction();
        //            break;
        //        case InteractionSourcePressType.Menu:
        //            SubmitMenuAction();
        //            break;
        //        case InteractionSourcePressType.Grasp:
        //            SubmitGraspAction();
        //            break;
        //        case InteractionSourcePressType.Touchpad:
        //            SubmitTouchpadAction();
        //            break;
        //        case InteractionSourcePressType.Thumbstick:
        //            SubmitThumbstickAction();
        //            break;
        //        case InteractionSourcePressType.None:
        //        default:
        //            break;
        //    }
        //}

        //private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        //{
        //    // Get Controller position
        //    bool controllerTracked = obj.state.sourcePose.TryGetPosition(out controllerPosition);

        //    // Get Controller rotation
        //    obj.state.sourcePose.TryGetRotation(out controllerRotation);

        //    var controller = activeControllers[obj.state.source.id];
        //    controller.Position = controllerPosition;
        //    controller.Rotation = controllerRotation;
        //    controller.ControllerState = controllerTracked ? ControllerState.Tracked : ControllerState.NotTracked;

        //    //Debug.Log($"Source [{obj.state.properties.sourceLossRisk.ToString()}] Updated");
        //    //Debug.Log($"Source [{obj.state.sourcePose.positionAccuracy.ToString()}] Updated");
        //}

        //private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        //{
        //    Debug.Log($"Source [{obj.pressType.ToString()}] Released");
        //    switch (obj.pressType)
        //    {
        //        case InteractionSourcePressType.Select:
        //            SubmitSelectReleaseAction();
        //            break;
        //        case InteractionSourcePressType.Menu:
        //            SubmitMenuReleaseAction();
        //            break;
        //        case InteractionSourcePressType.Grasp:
        //            SubmitGraspReleaseAction();
        //            break;
        //        case InteractionSourcePressType.Touchpad:
        //            SubmitTouchpadReleaseAction();
        //            break;
        //        case InteractionSourcePressType.Thumbstick:
        //            SubmitThumbstickReleaseAction();
        //            break;
        //        case InteractionSourcePressType.None:
        //        default:
        //            break;
        //    }
        //}

        //private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        //{
        //    Debug.Log($"Source [{obj.state.ToString()}] Lost");
        //}

        #endregion

        #region Input System events

        /// <summary>
        /// Updates the source information.
        /// </summary>
        /// <param name="interactionSourceState">Interaction source to use to update the source information.</param>
        /// <param name="sourceData">GenericInputPointingSource structure to update.</param>
        private void UpdateInteractionSource(InteractionSourceState interactionSourceState, IMixedRealityInputSource sourceData)
        {
            Debug.Assert(interactionSourceState.source.id == sourceData.SourceId, "An UpdateSourceState call happened with mismatched source ID.");
            //TODO - Do we need Kind?
            //Debug.Assert(interactionSourceState.source.kind == sourceData.Source.kind, "An UpdateSourceState call happened with mismatched source kind.");

            bool controllerTracked = ReadMixedRealityControllerValues(ref interactionSourceState);

            //Update Controller
            sourceData.Controller.Update(controllerPosition, controllerRotation);
            sourceData.ControllerPosition = controllerPosition; sourceData.ControllerRotation = controllerRotation;
            Debug.Log($"Controller Position after Read [{controllerPosition}]");

            //Update Pointer
            //TODO - Review, as we no longer have a MixedRealityCameraParent, this will cause issue.
            if (CameraCache.Main.transform.parent != null)
            {
                pointerPosition = CameraCache.Main.transform.parent.TransformPoint(pointerPosition);
                pointerRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(pointerRotation.eulerAngles);
            }
            sourceData.Pointer.Update(pointerPosition, pointerRotation);


            //Update Grip
            //TODO - Review, as we no longer have a MixedRealityCameraParent, this will cause issue.
            if (CameraCache.Main.transform.parent != null)
            {
                gripPosition = CameraCache.Main.transform.parent.TransformPoint(gripPosition);
                gripRotation.eulerAngles = CameraCache.Main.transform.parent.TransformDirection(gripRotation.eulerAngles);
            }
            sourceData.Grip.Update(gripPosition, gripRotation);

            // Update Touchpad
            if (interactionSourceState.touchpadTouched)
            {
            }
        }

        #endregion

        #region InteractionManager Events
        //private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        //{
        //    activeControllers.Add(obj.state.source.id, GetWindowsControllerProperties(ref obj));

        //    Debug.Log($"Source [{obj.state.ToString()}] Detected");
        //    Debug.Log($"handedness: [{obj.state.source.handedness}]");
        //    Debug.Log($"id: [{obj.state.source.id}]");
        //    Debug.Log($"kind: [{obj.state.source.kind}]");
        //    Debug.Log($"productId: [{obj.state.source.productId}]");
        //    Debug.Log($"productVersion: [{obj.state.source.productVersion}]");
        //    Debug.Log($"vendorId: [{obj.state.source.vendorId}]");
        //}

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs args)
        {
            SourceDetected(args.state);
        }

        private void SourceDetected(InteractionSourceState state)
        {
            var inputSource = GetOrAddWindowsMixedRealityController(state);
            if (inputSource.SourceId == 0) { return; }

            // NOTE: We update the source state data, in case an app wants to query it on source detected.
            UpdateInteractionSource(state, inputSource);
            inputSystem.RaiseSourceDetected(inputSource);
        }

        private void InteractionManager_InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args)
        {
            var inputSource = GetOrAddWindowsMixedRealityController(args.state);

            if (inputSource.SourceId == 0) { return; }

            UpdateInteractionSource(args.state, inputSource);

            //TODO - Needs serious thought!
            if (inputSource.Controller.Changed)
            {
                inputSystem.RaiseSourcePositionChanged(inputSource, inputSource.Handedness, inputSource.Pointer.Value.Item1, inputSource.Grip.Value.Item1);
                inputSystem.RaiseSourceRotationChanged(inputSource, inputSource.Handedness, inputSource.Pointer.Value.Item2, inputSource.Grip.Value.Item2);
            }
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs args)
        {
            var inputSource = GetOrAddWindowsMixedRealityController(args.state);
            if (inputSource.SourceId == 0) { return; }
            InputType inputType;
            switch (args.pressType)
            {
                case InteractionSourcePressType.None:
                    inputType = InputType.None;
                    break;
                case InteractionSourcePressType.Select:
                    inputType = InputType.Select;
                    break;
                case InteractionSourcePressType.Menu:
                    inputType = InputType.Menu;
                    break;
                case InteractionSourcePressType.Grasp:
                    inputType = InputType.Grip;
                    break;
                case InteractionSourcePressType.Touchpad:
                    inputType = InputType.Touchpad;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    inputType = InputType.ThumbStick;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            inputSystem.RaiseOnInputDown(inputSource, (Handedness)args.state.source.handedness, inputType);
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs args)
        {
            var inputSource = GetOrAddWindowsMixedRealityController(args.state);
            if (inputSource.SourceId == 0) { return; }

            InputType inputType;
            switch (args.pressType)
            {
                case InteractionSourcePressType.None:
                    inputType = InputType.None;
                    break;
                case InteractionSourcePressType.Select:
                    inputType = InputType.Select;
                    break;
                case InteractionSourcePressType.Menu:
                    inputType = InputType.Menu;
                    break;
                case InteractionSourcePressType.Grasp:
                    inputType = InputType.Grip;
                    break;
                case InteractionSourcePressType.Touchpad:
                    inputType = InputType.Touchpad;
                    break;
                case InteractionSourcePressType.Thumbstick:
                    inputType = InputType.ThumbStick;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            inputSystem.RaiseOnInputUp(inputSource, (Handedness)args.state.source.handedness, inputType);
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs args)
        {
            RemoveInteractionInputSource(GetOrAddWindowsMixedRealityController(args.state));
        }

        #endregion InteractionManager Events


        #region NewMethods?
        private void SubmitSelectAction()
        {

        }

        private void SubmitMenuAction()
        {

        }

        private void SubmitGraspAction()
        {

        }

        private void SubmitTouchpadAction()
        {

        }

        private void SubmitThumbstickAction()
        {

        }

        private void SubmitSelectReleaseAction()
        {

        }

        private void SubmitMenuReleaseAction()
        {

        }

        private void SubmitGraspReleaseAction()
        {

        }

        private void SubmitTouchpadReleaseAction()
        {

        }

        private void SubmitThumbstickReleaseAction()
        {

        }

        private void UpdateLocation()
        {

        }

        private void UpdateRotation()
        {

        }
        #endregion

        #region Runtime
        public void Enable()
        {
            // The first time we call OnEnable we skip this.
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
                RemoveInteractionInputSource(GetOrAddWindowsMixedRealityController(states[i]));
            }
        }

        public void Destroy()
        {
            //Destroy Stuff
        }

        private void RemoveInteractionInputSource(IMixedRealityInputSource interactionSource)
        {
            if (interactionSource.SourceId == 0) { return; }

            inputSystem.RaiseSourceLost(interactionSource);
            activeControllers.Remove(interactionSource.SourceId);
        }
        #endregion
    }
}