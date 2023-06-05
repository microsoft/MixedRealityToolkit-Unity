// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A specilized version of Unity's ActionBasedController that will fallback to other Input System actions for
    /// position and rotation data when ActionBasedController's default track state has no position or rotation data. 
    /// </summary>
    /// <remarks>
    /// This is useful when the controller has multiple active devices backing it, and some controllers are not being
    /// tracked. For example, HoloLens 2 eye gaze might be active but not calibrated, in which case eye gaze tracking
    /// state will have no position and no rotation data. In this case, the controller may want to fallback to head pose.
    /// </remarks>
    [AddComponentMenu("MRTK/Input/XR Controller (Action-based with Fallbacks)")]
    public class ActionBasedControllerWithFallbacks : ActionBasedController
    {
        #region Fallback actions values

        [SerializeField, Tooltip("The fallback Input System action to use for Position Tracking for this GameObject when the default tracking state action has no position data. Must be a Vector3Control Control.")]
        private InputActionProperty fallbackPositionAction;

        /// <summary>
        /// The fallback Input System action to use for Position Tracking for this GameObject when the default tracking
        /// state action has no position data. Must be a Vector3Control Control.
        /// </summary>
        public InputActionProperty FallbackPositionAction => fallbackPositionAction;

        [SerializeField, Tooltip("The fallback Input System action to use for Rotation Tracking for this GameObject when the default tracking state action has no position data. Must be a QuaternionControl Control.")]
        private InputActionProperty fallbackRotationAction;

        /// <summary>
        /// The fallback Input System action to use for Rotation Tracking for this GameObject when the default tracking
        /// state action has no rotation data. Must be a QuaternionControl Control.
        /// </summary>
        public InputActionProperty FallbackRotationAction => fallbackRotationAction;

        [SerializeField, Tooltip("The fallback Input System action to get the Tracking State when updating this GameObject position or rotation. when the default track status action has no position or rotation data. If not specified, this will fallback to the device's tracking state that drives the position or rotation action.Must be a IntegerControl Control.")]
        private InputActionProperty fallbackTrackingStateAction;

        /// <summary>
        /// The fallback Input System action to get the Tracking State when updating this GameObject position or rotation
        /// when the default track status action has no position or rotation data. If not specified, this will fallback
        /// to the device's tracking state that drives the position or rotation action.Must be a IntegerControl Control.
        /// </summary>
        public InputActionProperty FallbackTrackingStateAction => fallbackTrackingStateAction;

        #endregion Fallback action values


        #region ActionBasedController Overrides 
        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            base.UpdateTrackingInput(controllerState);
            if (controllerState == null)
            {
                return;
            }

            var positionAction = fallbackPositionAction.action;
            var hasPositionAction = positionAction != null;

            var rotationAction = fallbackRotationAction.action;
            var hasRotationAction = rotationAction != null;

            var trackingStateAction = fallbackTrackingStateAction.action;
            var hasTrackingStateAction = trackingStateAction != null && trackingStateAction.bindings.Count > 0;

            // If no position data, use position fallback
            if (!controllerState.inputTrackingState.HasFlag(InputTrackingState.Position) && hasPositionAction)
            {
                InputTrackingState fallbackPositionState = InputTrackingState.None;
                if (hasTrackingStateAction)
                {
                    fallbackPositionState = (InputTrackingState)trackingStateAction.ReadValue<int>();
                }
                else if (positionAction.activeControl?.device is TrackedDevice trackedDevice)
                {
                    fallbackPositionState = (InputTrackingState)trackedDevice.trackingState.ReadValue();
                }

                if (fallbackPositionState.HasFlag(InputTrackingState.Position))
                {
                    var position = positionAction.ReadValue<Vector3>();
                    controllerState.position = position;
                    controllerState.inputTrackingState |= InputTrackingState.Position;
                }
            }

            // If no rotation data, use rotation fallback
            if (!controllerState.inputTrackingState.HasFlag(InputTrackingState.Rotation) && hasRotationAction)
            {
                InputTrackingState fallbackRotationState = InputTrackingState.None;
                if (hasTrackingStateAction)
                {
                    fallbackRotationState = (InputTrackingState)trackingStateAction.ReadValue<int>();
                }
                else if (positionAction.activeControl?.device is TrackedDevice trackedDevice)
                {
                    fallbackRotationState = (InputTrackingState)trackedDevice.trackingState.ReadValue();
                }

                if (fallbackRotationState.HasFlag(InputTrackingState.Rotation))
                {
                    var rotation = rotationAction.ReadValue<Quaternion>();
                    controllerState.rotation = rotation;
                    controllerState.inputTrackingState |= InputTrackingState.Rotation;
                }
            }
        }
        #endregion ActionBasedController Overrides 
    }
}
