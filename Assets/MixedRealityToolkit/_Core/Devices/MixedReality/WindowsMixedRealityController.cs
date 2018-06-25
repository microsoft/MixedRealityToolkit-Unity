// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Microsoft.MixedReality.Toolkit.Internal.Devices.WindowsMixedReality
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    public class WindowsMixedRealityController : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controllerState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public WindowsMixedRealityController(ControllerState controllerState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, IMixedRealityInteractionMapping[] interactions = null)
                : base(controllerState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Controller.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; private set; }

        private Vector3 currentPointerPosition;
        private Quaternion currentPointerRotation;
        private SixDof currentPointerData = new SixDof(Vector3.zero, Quaternion.identity);

        private Vector3 currentGripPosition;
        private Quaternion currentGripRotation;
        private SixDof currentGripData = new SixDof(Vector3.zero, Quaternion.identity);

        /// <inheritdoc/>
        public override void SetupConfiguration()
        {
            if (MixedRealityManager.Instance.ActiveProfile.EnableControllerProfiles)
            {
                MixedRealityControllerMapping controllerMapping = default(MixedRealityControllerMapping);
                var controllerMappings = MixedRealityManager.Instance.ActiveProfile.ControllersProfile.MixedRealityControllerMappingProfiles;

                for (int i = 0; i < controllerMappings?.Length; i++)
                {
                    if (controllerMappings[i].Controller.Type == GetType() && controllerMappings[i].Handedness == ControllerHandedness)
                    {
                        controllerMapping = controllerMappings[i];
                        break;
                    }
                }

                if (controllerMapping.Interactions?.Length > 0)
                {
                    SetupFromMapping(controllerMapping.Interactions);
                    return;
                }
            }

            SetupControllerDefaults();
        }

        #region Setup and Update functions

        /// <summary>
        /// Load the Interaction mappings for this controller from the configured Controller Mapping profile
        /// </summary>
        /// <param name="mappings">Configured mappings from a controller mapping profile</param>
        private void SetupFromMapping(IMixedRealityInteractionMapping[] mappings)
        {
            var interactions = new List<IMixedRealityInteractionMapping>();
            for (int i = 0; i < mappings.Length; i++)
            {
                switch (mappings[i].AxisType)
                {
                    case AxisType.Digital:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.SingleAxis:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.DualAxis:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDofPosition:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.ThreeDofRotation:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.SixDof:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    case AxisType.None:
                    case AxisType.Raw:
                        interactions.Add(new MixedRealityInteractionMapping((uint)i, mappings[i].AxisType, mappings[i].InputType, (InputAction)mappings[i].InputAction));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Interactions = interactions.ToArray();
        }

        /// <summary>
        /// Create Interaction mappings from a device specific default set of action mappings
        /// </summary>
        private void SetupControllerDefaults()
        {
            Interactions = new IMixedRealityInteractionMapping[9];
            Interactions[0] = new MixedRealityInteractionMapping(0, AxisType.Digital, DeviceInputType.Select, new InputAction(1, "Select"));
            Interactions[1] = new MixedRealityInteractionMapping(1, AxisType.SingleAxis, DeviceInputType.Trigger, new InputAction(2, "Trigger Press"));
            Interactions[2] = new MixedRealityInteractionMapping(2, AxisType.Digital, DeviceInputType.GripPress, new InputAction(2, "Grip Press"));
            Interactions[3] = new MixedRealityInteractionMapping(3, AxisType.Digital, DeviceInputType.Menu, new InputAction(3, "Menu Press"));
            Interactions[4] = new MixedRealityInteractionMapping(4, AxisType.DualAxis, DeviceInputType.ThumbStick, ControllerHandedness == Handedness.Left ? new InputAction(4, "Vertical Movement") : new InputAction(4, "Horizontal Movement"));
            Interactions[5] = new MixedRealityInteractionMapping(5, AxisType.Digital, DeviceInputType.ThumbStickPress, new InputAction(5, "Thumbstick Press"));
            Interactions[6] = new MixedRealityInteractionMapping(6, AxisType.DualAxis, DeviceInputType.Touchpad, new InputAction(6, "Touchpad Position"));
            Interactions[7] = new MixedRealityInteractionMapping(7, AxisType.Digital, DeviceInputType.TouchpadTouch, new InputAction(7, "Touchpad Touch"));
            Interactions[8] = new MixedRealityInteractionMapping(8, AxisType.Digital, DeviceInputType.TouchpadPress, new InputAction(8, "Touchpad Press"));
        }

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public void UpdateController(InteractionSourceState interactionSourceState)
        {
            UpdateControllerData(interactionSourceState);

            Debug.Assert(Interactions != null);
            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
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
                        UpdateMenuData(interactionSourceState, Interactions[i]);
                        break;
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

            // Windows Mixed Reality Devices are always tracked during their lifetime.
            ControllerState = ControllerState.Tracked;
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            interactionSourceState.sourcePose.TryGetPosition(out currentPointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out currentPointerRotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                currentPointerData.Position = CameraCache.Main.transform.parent.TransformPoint(currentPointerPosition);
                currentPointerData.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentPointerRotation.eulerAngles));
            }

            //Update the interaction data source
            interactionMapping.SetSixDofValue(currentPointerData);

            //Raise input system Event if it enabled
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
                case DeviceInputType.GripPosition:
                case DeviceInputType.GripRotation:
                    {
                        interactionSourceState.sourcePose.TryGetPosition(out currentGripPosition, InteractionSourceNode.Grip);
                        interactionSourceState.sourcePose.TryGetRotation(out currentGripRotation, InteractionSourceNode.Grip);

                        if (CameraCache.Main.transform.parent != null)
                        {
                            currentGripData.Position = CameraCache.Main.transform.parent.TransformPoint(currentGripPosition);
                            currentGripData.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentGripRotation.eulerAngles));
                        }

                        //Update the interaction data source
                        interactionMapping.SetSixDofValue(currentGripData);

                        //Raise input system Event if it enabled
                        InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, currentGripData);
                    }
                    break;
                case DeviceInputType.GripPress:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.grasped);

                        //Raise input system Event if it enabled
                        if (interactionSourceState.grasped)
                        {
                            InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                        else
                        {
                            InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
                        }
                    }
                    break;
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
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.touchpadTouched);

                        //Raise input system Event if it enabled
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
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.touchpadPressed);

                        //Raise input system Event if it enabled
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
                        //Update the interaction data source
                        interactionMapping.SetVector2Value(interactionSourceState.touchpadPosition);

                        //Raise input system Event if it enabled
                        InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.touchpadPosition);
                        break;
                    }
                default:
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
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.thumbstickPressed);

                        //Raise input system Event if it enabled
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
                        //Update the interaction data source
                        interactionMapping.SetVector2Value(interactionSourceState.thumbstickPosition);

                        //Raise input system Event if it enabled
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
                case DeviceInputType.TriggerPress:
                case DeviceInputType.Select:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.selectPressed);

                        //Raise input system Event if it enabled
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
                        //Update the interaction data source
                        interactionMapping.SetFloatValue(interactionSourceState.selectPressedAmount);

                        //Raise input system Event if it enabled
                        InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.InputAction, interactionSourceState.selectPressedAmount);
                        break;
                    }
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        /// <summary>
        /// Update the Menu button state.
        /// </summary>
        /// <param name="interactionSourceState"></param>
        /// <param name="interactionMapping"></param>
        private void UpdateMenuData(InteractionSourceState interactionSourceState, IMixedRealityInteractionMapping interactionMapping)
        {
            //Update the interaction data source
            interactionMapping.SetBoolValue(interactionSourceState.menuPressed);

            //Raise input system Event if it enabled
            if (interactionSourceState.menuPressed)
            {
                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.InputAction);
            }
            else
            {
                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.InputAction);
            }
        }

        #endregion Update data functions

        #endregion Setup and Update functions
    }
}