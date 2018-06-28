// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Devices;
using Microsoft.MixedReality.Toolkit.Internal.Definitions.Utilities;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Utilities;
using System;
using UnityEngine;

#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

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
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public WindowsMixedRealityController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

#if UNITY_WSA

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Controller.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; private set; }

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerData = new MixedRealityPose(Vector3.zero, Quaternion.identity);

        private Vector3 currentGripPosition = Vector3.zero;
        private Quaternion currentGripRotation = Quaternion.identity;
        private MixedRealityPose currentGripData = new MixedRealityPose(Vector3.zero, Quaternion.identity);

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
            var lastState = TrackingState;

            switch (interactionSourceState.sourcePose.positionAccuracy)
            {
                case InteractionSourcePositionAccuracy.None:
                    TrackingState = TrackingState.NotTracked;
                    break;
                case InteractionSourcePositionAccuracy.Approximate:
                    TrackingState = TrackingState.Approximate;
                    break;
                case InteractionSourcePositionAccuracy.High:
                    TrackingState = TrackingState.Tracked;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (lastState != TrackingState)
            {
                InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdatePointerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            interactionSourceState.sourcePose.TryGetPosition(out currentPointerPosition, InteractionSourceNode.Pointer);
            interactionSourceState.sourcePose.TryGetRotation(out currentPointerRotation, InteractionSourceNode.Pointer);

            if (CameraCache.Main.transform.parent != null)
            {
                currentPointerData.Position = CameraCache.Main.transform.parent.TransformPoint(currentPointerPosition);
                currentPointerData.Rotation = Quaternion.Euler(CameraCache.Main.transform.parent.TransformDirection(currentPointerRotation.eulerAngles));
            }

            //Update the interaction data source
            interactionMapping.SetPoseValue(currentPointerData);

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                //Raise input system Event if it enabled
                InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentPointerData);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        private void UpdateGripData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
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
                        interactionMapping.SetPoseValue(currentGripData);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.Raise6DofInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentGripData);
                        }
                    }
                    break;
                case DeviceInputType.GripPress:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.grasped);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (interactionSourceState.grasped)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
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
        private void UpdateTouchPadData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.touchpadTouched);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (interactionSourceState.touchpadTouched)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.TouchpadPress:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.touchpadPressed);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (interactionSourceState.touchpadPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Touchpad:
                    {
                        //Update the interaction data source
                        interactionMapping.SetVector2Value(interactionSourceState.touchpadPosition);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
                        }
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
        private void UpdateThumbStickData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.thumbstickPressed);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (interactionSourceState.thumbstickPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.ThumbStick:
                    {
                        //Update the interaction data source
                        interactionMapping.SetVector2Value(interactionSourceState.thumbstickPosition);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.Raise2DoFInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.thumbstickPosition);
                        }
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
        private void UpdateTriggerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                case DeviceInputType.Select:
                    {
                        //Update the interaction data source
                        interactionMapping.SetBoolValue(interactionSourceState.selectPressed);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            if (interactionSourceState.selectPressed)
                            {
                                InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                            else
                            {
                                InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                            }
                        }
                        break;
                    }
                case DeviceInputType.Trigger:
                    {
                        //Update the interaction data source
                        interactionMapping.SetFloatValue(interactionSourceState.selectPressedAmount);

                        // If our value changed raise it.
                        if (interactionMapping.Changed)
                        {
                            //Raise input system Event if it enabled
                            InputSystem?.RaiseOnInputPressed(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.selectPressedAmount);
                        }
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
        private void UpdateMenuData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            //Update the interaction data source
            interactionMapping.SetBoolValue(interactionSourceState.menuPressed);

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                //Raise input system Event if it enabled
                if (interactionSourceState.menuPressed)
                {
                    InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Update data functions

#endif // UNITY_WSA
    }
}