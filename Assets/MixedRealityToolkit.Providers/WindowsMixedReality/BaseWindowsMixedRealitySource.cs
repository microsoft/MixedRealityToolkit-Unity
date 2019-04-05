// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;

#if UNITY_WSA
using UnityEngine;
using UnityEngine.XR.WSA.Input;
#endif

namespace Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input
{
    /// <summary>
    /// A Windows Mixed Reality Controller Instance.
    /// </summary>
    public abstract class BaseWindowsMixedRealitySource : BaseController
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public BaseWindowsMixedRealitySource(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultLeftHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultRightHandedInteractions => DefaultInteractions;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

#if UNITY_WSA

        /// <summary>
        /// The last updated source state reading for this Windows Mixed Reality Source.
        /// </summary>
        public InteractionSourceState LastSourceStateReading { get; protected set; }

        private Vector3 currentControllerPosition = Vector3.zero;
        private Quaternion currentControllerRotation = Quaternion.identity;
        private MixedRealityPose lastControllerPose = MixedRealityPose.ZeroIdentity;
        private MixedRealityPose currentControllerPose = MixedRealityPose.ZeroIdentity;

        private Vector3 currentPointerPosition = Vector3.zero;
        private Quaternion currentPointerRotation = Quaternion.identity;
        private MixedRealityPose currentPointerPose = MixedRealityPose.ZeroIdentity;

        private Vector3 currentGripPosition = Vector3.zero;
        private Quaternion currentGripRotation = Quaternion.identity;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;

        #region Update data functions

        /// <summary>
        /// Update the controller data from the provided platform state
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        public virtual void UpdateController(InteractionSourceState interactionSourceState)
        {
            if (!Enabled) { return; }

            UpdateControllerData(interactionSourceState);

            if (Interactions == null)
            {
                Debug.LogError($"No interaction configuration for Windows Mixed Reality {ControllerHandedness} Source");
                Enabled = false;
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {
                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.None:
                        break;
                    case DeviceInputType.SpatialPointer:
                        UpdatePointerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.Select:
                    case DeviceInputType.Trigger:
                    case DeviceInputType.TriggerTouch:
                    case DeviceInputType.TriggerPress:
                        UpdateTriggerData(interactionSourceState, Interactions[i]);
                        break;
                    case DeviceInputType.SpatialGrip:
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
                }
            }

            LastSourceStateReading = interactionSourceState;
        }

        /// <summary>
        /// Update the "Controller" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        protected virtual void UpdateControllerData(InteractionSourceState interactionSourceState)
        {
            var lastState = TrackingState;
            var sourceKind = interactionSourceState.source.kind;

            lastControllerPose = currentControllerPose;

            if (sourceKind == InteractionSourceKind.Hand ||
               (sourceKind == InteractionSourceKind.Controller && interactionSourceState.source.supportsPointing))
            {
                // The source is either a hand or a controller that supports pointing.
                // We can now check for position and rotation.
                IsPositionAvailable = interactionSourceState.sourcePose.TryGetPosition(out currentControllerPosition);

                if (IsPositionAvailable)
                {
                    IsPositionApproximate = (interactionSourceState.sourcePose.positionAccuracy == InteractionSourcePositionAccuracy.Approximate);
                }
                else
                {
                    IsPositionApproximate = false;
                }

                IsRotationAvailable = interactionSourceState.sourcePose.TryGetRotation(out currentControllerRotation);

                // Devices are considered tracked if we receive position OR rotation data from the sensors.
                TrackingState = (IsPositionAvailable || IsRotationAvailable) ? TrackingState.Tracked : TrackingState.NotTracked;
            }
            else
            {
                // The input source does not support tracking.
                TrackingState = TrackingState.NotApplicable;
            }

            currentControllerPose.Position = currentControllerPosition;
            currentControllerPose.Rotation = currentControllerRotation;

            // Raise input system events if it is enabled.
            if (lastState != TrackingState)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourceTrackingStateChanged(InputSource, this, TrackingState);
            }

            if (TrackingState == TrackingState.Tracked && lastControllerPose != currentControllerPose)
            {
                if (IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePoseChanged(InputSource, this, currentControllerPose);
                }
                else if (IsPositionAvailable && !IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentControllerPosition);
                }
                else if (!IsPositionAvailable && IsRotationAvailable)
                {
                    MixedRealityToolkit.InputSystem?.RaiseSourceRotationChanged(InputSource, this, currentControllerRotation);
                }
            }
        }

        /// <summary>
        /// Update the "Spatial Pointer" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdatePointerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            if (interactionSourceState.source.supportsPointing)
            {
                interactionSourceState.sourcePose.TryGetPosition(out currentPointerPosition, InteractionSourceNode.Pointer);
                interactionSourceState.sourcePose.TryGetRotation(out currentPointerRotation, InteractionSourceNode.Pointer);

                // We want the controller to follow the Playspace, so fold in the playspace transform here to 
                // put the controller pose into world space.
                var playspace = MixedRealityToolkit.Instance.MixedRealityPlayspace;
                if (playspace != null)
                {
                    currentPointerPose.Position = playspace.TransformPoint(currentPointerPosition);
                    currentPointerPose.Rotation = playspace.rotation * currentPointerRotation;
                }
                else
                {
                    currentPointerPose.Position = currentPointerPosition;
                    currentPointerPose.Rotation = currentPointerRotation;
                }
            }

            // Update the interaction data source
            interactionMapping.PoseData = currentPointerPose;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentPointerPose);
            }
        }

        /// <summary>
        /// Update the "Spatial Grip" input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateGripData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.AxisType)
            {
                case AxisType.SixDof:
                {
                    interactionSourceState.sourcePose.TryGetPosition(out currentGripPosition, InteractionSourceNode.Grip);
                    interactionSourceState.sourcePose.TryGetRotation(out currentGripRotation, InteractionSourceNode.Grip);

                    var playspace = GetPlayspace();
                    if (playspace != null)
                    {
                        currentGripPose.Position = playspace.TransformPoint(currentGripPosition);
                        currentGripPose.Rotation = Quaternion.Euler(playspace.TransformDirection(currentGripRotation.eulerAngles));
                    }
                    else
                    {
                        currentGripPose.Position = currentGripPosition;
                        currentGripPose.Rotation = currentGripRotation;
                    }

                    // Update the interaction data source
                    interactionMapping.PoseData = currentGripPose;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, currentGripPose);
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
        protected virtual void UpdateTouchPadData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TouchpadTouch:
                {
                    // Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.touchpadTouched;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionSourceState.touchpadTouched)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
                case DeviceInputType.TouchpadPress:
                {
                    //Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.touchpadPressed;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionSourceState.touchpadPressed)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
                case DeviceInputType.Touchpad:
                {
                    // Update the interaction data source
                    interactionMapping.Vector2Data = interactionSourceState.touchpadPosition;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.touchpadPosition);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update the Thumbstick input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateThumbStickData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.ThumbStickPress:
                {
                    // Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.thumbstickPressed;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionSourceState.thumbstickPressed)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
                case DeviceInputType.ThumbStick:
                {
                    // Update the interaction data source
                    interactionMapping.Vector2Data = interactionSourceState.thumbstickPosition;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        MixedRealityToolkit.InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.thumbstickPosition);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update the Trigger input from the device
        /// </summary>
        /// <param name="interactionSourceState">The InteractionSourceState retrieved from the platform</param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateTriggerData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            switch (interactionMapping.InputType)
            {
                case DeviceInputType.TriggerPress:
                    // Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.grasped;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionMapping.BoolData)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                case DeviceInputType.Select:
                {
                    // Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.selectPressed;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionSourceState.selectPressed)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
                case DeviceInputType.Trigger:
                {
                    // Update the interaction data source
                    interactionMapping.FloatData = interactionSourceState.selectPressedAmount;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        MixedRealityToolkit.InputSystem?.RaiseFloatInputChanged(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction, interactionSourceState.selectPressedAmount);
                    }
                    break;
                }
                case DeviceInputType.TriggerTouch:
                {
                    // Update the interaction data source
                    interactionMapping.BoolData = interactionSourceState.selectPressedAmount > 0;

                    // If our value changed raise it.
                    if (interactionMapping.Changed)
                    {
                        // Raise input system Event if it enabled
                        if (interactionSourceState.selectPressedAmount > 0)
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                        else
                        {
                            MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Update the Menu button state.
        /// </summary>
        /// <param name="interactionSourceState"></param>
        /// <param name="interactionMapping"></param>
        protected virtual void UpdateMenuData(InteractionSourceState interactionSourceState, MixedRealityInteractionMapping interactionMapping)
        {
            //Update the interaction data source
            interactionMapping.BoolData = interactionSourceState.menuPressed;

            // If our value changed raise it.
            if (interactionMapping.Changed)
            {
                // Raise input system Event if it enabled
                if (interactionSourceState.menuPressed)
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
                else
                {
                    MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, interactionMapping.MixedRealityInputAction);
                }
            }
        }

        #endregion Update data functions

#endif // UNITY_WSA
    }
}