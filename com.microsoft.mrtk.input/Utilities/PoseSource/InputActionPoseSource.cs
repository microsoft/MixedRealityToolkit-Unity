// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A pose source which gets the pose composed of a tracked position and rotation input action.
    /// </summary>
    [Serializable]
    public class InputActionPoseSource : IPoseSource
    {
        [SerializeField]
        [Tooltip("The input action property used when obtaining the tracking information for the current pose.")]
        InputActionProperty trackingStateActionProperty;

        [SerializeField]
        [Tooltip("The input action property used when obtaining the position information for the current pose.")]
        InputActionProperty positionActionProperty;

        [SerializeField]
        [Tooltip("The input action property used when obtaining the rotation information for the current pose.")]
        InputActionProperty rotationActionProperty;

        /// <summary>
        /// Tries to get the pose in world space composed of the provided input action properties when the position and rotation are tracked.
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            InputAction trackingStateAction = trackingStateActionProperty.action;
            InputAction positionAction = positionActionProperty.action;
            InputAction rotationAction = rotationActionProperty.action;

            if (trackingStateAction.HasAnyControls()
                && positionAction.HasAnyControls()
                && rotationAction.HasAnyControls()
                && ((InputTrackingState)trackingStateAction.ReadValue<int>() & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0)
            {
                // Transform the pose into worldspace, as input actions are returned
                // in floor-offset-relative coordinates.
                pose = PlayspaceUtilities.TransformPose(
                    new Pose(
                        positionAction.ReadValue<Vector3>(),
                        rotationAction.ReadValue<Quaternion>()));
                return true;
            }
            else
            {
                pose = Pose.identity;
                return false;
            }
        }
    }
}
