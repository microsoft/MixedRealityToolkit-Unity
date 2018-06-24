// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    public class WindowsMixedRealityController : BaseController
    {
        #region Private properties

        private bool isControllerTracked;
        private Vector3 currentControllerPosition;
        private Vector3 currentPointerPosition;
        private Vector3 currentGripPosition;
        private Quaternion currentControllerRotation;
        private Quaternion currentPointerRotation;
        private Quaternion currentGripRotation;

        private SixDof currentPointerData = new SixDof(Vector3.zero, Quaternion.identity);
        private SixDof currentGripData = new SixDof(Vector3.zero, Quaternion.identity);


        #endregion Private properties

        public WindowsMixedRealityController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, IMixedRealityInteractionMapping[] interactions = null)
                : base(controllerState, controllerHandedness, inputSource, interactions)
        {
            isControllerTracked = false;
            currentControllerPosition = currentPointerPosition = currentGripPosition = Vector3.zero;
            currentControllerRotation = currentPointerRotation = currentGripRotation = Quaternion.identity;
        }

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Controller.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; private set; }

        #region IMixedRealityController Interface Members

        /// <inheritdoc/>
        public override void LoadConfiguration()
        {
            // TODO - Which method is preferred.  I like the generics verion
            MixedRealityControllerMapping controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping(typeof(WindowsMixedRealityController), ControllerHandedness);
            //MixedRealityControllerMappingProfile controllerMapping = Managers.MixedRealityManager.Instance.ActiveProfile.GetControllerMapping<WindowsMixedRealityController>(ControllerHandedness);

            if (controllerMapping.Interactions?.Length > 0)
            {
                SetupFromMapping(controllerMapping.Interactions);
            }
            else
            {
                SetupWMRControllerDefaults();
            }
        }

        ///// <inheritdoc/>
        //public void UpdateInputSource<T>(T state)
        //{
        //    InteractionSourceState interactionSourceState = CheckIfValidInteractionSourceState(state);
        //    UpdateFromInteractionSource(interactionSourceState);
        //}

        #endregion IMixedRealityInputSource Interface Members

        #region Setup and Update functions

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        private void SetupFromMapping(IMixedRealityInteractionMapping[] mappings)
        {
            var interactions = new List<IMixedRealityInteractionMapping>();
            for (uint i = 0; i < mappings.Length; i++)
            {
                // Add interaction for Mapping
                switch (mappings[i].AxisType)
                {
                    case AxisType.Digital:
                        interactions.Add(new MixedRealityInteractionMapping<bool>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.SingleAxis:
                        interactions.Add(new MixedRealityInteractionMapping<float>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.DualAxis:
                        interactions.Add(new MixedRealityInteractionMapping<Vector2>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDofPosition:
                        interactions.Add(new MixedRealityInteractionMapping<Vector3>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDofRotation:
                        interactions.Add(new MixedRealityInteractionMapping<Quaternion>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.SixDof:
                        interactions.Add(new MixedRealityInteractionMapping<SixDof>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                    case AxisType.None:
                    case AxisType.Raw:
                    default:
                        interactions.Add(new MixedRealityInteractionMapping<object>(i, mappings[i].AxisType, mappings[i].InputType, mappings[i].InputAction));
                        break;
                }
            }
            Interactions = interactions.ToArray();
        }

        /// <summary>
        /// Create Interaction mappings from a device specific default set of action mappings
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void SetupWMRControllerDefaults()
        {
            var interactions = new List<IMixedRealityInteractionMapping>();

            InputAction[] inputActions = Managers.MixedRealityManager.Instance.ActiveProfile.InputActionsProfile.InputActions;
            if (inputActions == null)
            {
                return;
            }
            //Add the Controller Pointer
            interactions.Add(new MixedRealityInteractionMapping<SixDof>(1, AxisType.SixDof, DeviceInputType.SpatialPointer, new InputAction(1, "Select"))); // Note will convert these lookups to indexes

            // Add the Controller trigger
            interactions.Add(new MixedRealityInteractionMapping<float>(2, AxisType.SingleAxis, DeviceInputType.Trigger, new InputAction(1, "Select")));

            // If the controller has a Grip / Grasp button, add it to the controller capabilities
            interactions.Add(new MixedRealityInteractionMapping<SixDof>(3, AxisType.SixDof, DeviceInputType.SpatialGrip, new InputAction(2, "Grip")));

            interactions.Add(new MixedRealityInteractionMapping<bool>(4, AxisType.Digital, DeviceInputType.GripPress, new InputAction(3, "Grab")));

            // If the controller has a menu button, add it to the controller capabilities
            interactions.Add(new MixedRealityInteractionMapping<bool>(5, AxisType.Digital, DeviceInputType.Menu, new InputAction(4, "Menu")));

            // If the controller has a Thumbstick, add it to the controller capabilities
            interactions.Add(new MixedRealityInteractionMapping<Vector2>(6, AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerHandedness == Handedness.Left ? new InputAction(5, "Walk") : new InputAction(6, "Look")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(7, AxisType.Digital, DeviceInputType.ThumbStickPress, new InputAction(7, "Interact")));

            // If the controller has a Touchpad, add it to the controller capabilities
            interactions.Add(new MixedRealityInteractionMapping<Vector2>(8, AxisType.DualAxis, DeviceInputType.Touchpad, new InputAction(8, "Inventory")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(9, AxisType.Digital, DeviceInputType.TouchpadTouch, new InputAction(9, "Pickup")));
            interactions.Add(new MixedRealityInteractionMapping<bool>(10, AxisType.Digital, DeviceInputType.TouchpadPress, new InputAction(9, "Pickup")));

            Interactions = interactions.ToArray();
        }

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(InteractionSourceState interactionSourceState)
        {
            UpdateControllerData(interactionSourceState);

            for (int i = 0; i < Interactions.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialPointer:
                    case DeviceInputType.PointerPosition:
                    case DeviceInputType.PointerRotation:
                        UpdatePointerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerPress:
                    case DeviceInputType.PointerClick:
                        UpdateTriggerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
                    case DeviceInputType.GripPosition:
                    case DeviceInputType.GripRotation:
                    case DeviceInputType.GripPress:
                        UpdateGripData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.ThumbStick:
                    case DeviceInputType.ThumbStickPress:
                        UpdateThumbStickData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Touchpad:
                    case DeviceInputType.TouchpadTouch:
                    case DeviceInputType.TouchpadPress:
                        UpdateTouchPadData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Menu:
                    case DeviceInputType.ButtonPress:
                        {
                            var interaction = Interactions[i] as MixedRealityInteractionMapping<bool>;
                            Debug.Assert(interaction != null);
                            interaction.SetValue(interactionSourceState.menuPressed);
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        private void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            LastSourceStateReading = interactionSourceState;

            // Get Controller start position and tracked state
            isControllerTracked = interactionSourceState.sourcePose.TryGetPosition(out currentControllerPosition);
            ControllerState = isControllerTracked ? ControllerState.Tracked : ControllerState.NotTracked;

            // Get Controller start rotation
            interactionSourceState.sourcePose.TryGetRotation(out currentControllerRotation);
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="MixedRealityInteractionMapping"></param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            interactionSourceState.sourcePose.TryGetPosition(out currentPointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out currentPointerRotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                currentPointerPosition = CameraCache.Main.transform.parent.TransformPoint(currentPointerPosition);
                currentPointerRotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentPointerRotation.eulerAngles));
            }

            var interaction = interactionMapping as MixedRealityInteractionMapping<SixDof>;
            currentPointerData.Position = currentPointerPosition;
            currentPointerData.Rotation = currentPointerRotation;
            Debug.Assert(interaction != null);

            //Update the interaction data source
            interaction.SetValue(currentPointerData);

            //Raise Inputsystem Event if it enabled
            InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, currentPointerData);
        }


        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.SpatialGrip:
                    {
                        interactionSourceState.sourcePose.TryGetPosition(out currentGripPosition, InteractionSourceNode.Grip);
                        interactionSourceState.sourcePose.TryGetRotation(out currentGripRotation, InteractionSourceNode.Grip);

                        if (CameraCache.Main.transform.parent != null)
                        {
                            currentGripPosition = CameraCache.Main.transform.parent.TransformPoint(currentGripPosition);
                            currentGripRotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentGripRotation.eulerAngles));
                        }

                        var interaction = interactionMapping as MixedRealityInteractionMapping<SixDof>;
                        currentGripData.Position = currentGripPosition;
                        currentGripData.Rotation = currentGripRotation;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(currentGripData);

                        //Raise Inputsystem Event if it enabled
                        InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, currentGripData);
                        break;
                    }
                case DeviceInputType.GripPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.grasped);

                        //Raise Inputsystem Event if it enabled
                        if (interactionSourceState.grasped)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Update the Touchpad input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTouchPadData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.touchpadTouched);

                        //Raise Inputsystem Event if it enabled
                        if (interactionSourceState.touchpadTouched)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.touchpadPressed);

                        //Raise Inputsystem Event if it enabled
                        if (interactionSourceState.touchpadPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<Vector2>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.touchpadPosition);
                        
                        //Raise Inputsystem Event if it enabled
                        InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.touchpadPosition);
                        break;
                    }
                default:
                    //Unknown DeviceInputType
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateThumbStickData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);
                        
                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.thumbstickPressed);

                        //Raise Inputsystem Event if it enabled
                        if (interactionSourceState.thumbstickPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<Vector2>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.thumbstickPosition);

                        //Raise Inputsystem Event if it enabled
                        InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.thumbstickPosition);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateTriggerData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.Select:
                case DeviceInputType.TriggerPress:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<bool>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.selectPressed);

                        //Raise Inputsystem Event if it enabled
                        if (interactionSourceState.selectPressed)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        var interaction = interactionMapping as MixedRealityInteractionMapping<float>;
                        Debug.Assert(interaction != null);

                        //Update the interaction data source
                        interaction.SetValue(interactionSourceState.selectPressedAmount);

                        //Raise Inputsystem Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.selectPressedAmount);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        #endregion Update data functions

        #endregion Setup and Update functions

        #region Utilities
        private static InteractionSourceState CheckIfValidInteractionSourceState<T>(T state)
        {
            InteractionSourceState interactionSourceState = (InteractionSourceState)Convert.ChangeType(state, typeof(InteractionSourceState));
            if (interactionSourceState.source.id == 0) { throw new ArgumentOutOfRangeException(nameof(state), "Incorrect state type provided to controller,did you send an InteractionSourceState?"); }

            return interactionSourceState;
        }
        #endregion Utilities
    }
}