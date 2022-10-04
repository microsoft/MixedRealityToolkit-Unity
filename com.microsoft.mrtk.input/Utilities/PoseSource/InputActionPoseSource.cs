// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]

    /// <summary>
    /// A pose source which gets the pose composed of a tracked position and rotation input action
    /// </summary>
    public class InputActionPoseSource : IPoseSource
    {
        [SerializeField]
        InputActionProperty trackingStateActionProperty;

        [SerializeField]
        InputActionProperty positionActionProperty;

        [SerializeField]
        InputActionProperty rotationActionProperty;

        /// <summary>
        /// Tries to get the pose in worldspace composed of the provided input action properties when the position and rotation are tracked
        /// </summary>
        public bool TryGetPose(out Pose pose)
        {
            InputAction trackingStateAction = trackingStateActionProperty.action;
            InputAction positionAction = positionActionProperty.action;
            InputAction rotationAction = rotationActionProperty.action;

            if (trackingStateAction != null && trackingStateAction.controls.Count != 0
                && positionAction != null && positionAction.controls.Count != 0
                && rotationAction != null && rotationAction.controls.Count != 0
                && ((InputTrackingState)trackingStateAction.ReadValue<int>() & (InputTrackingState.Position | InputTrackingState.Rotation)) != 0)
            {
                // Transform the pose into worldspace, as input actions are returned in rig space
                pose.position = PlayspaceUtilities.ReferenceTransform.TransformPoint(positionAction.ReadValue<Vector3>());
                pose.rotation = PlayspaceUtilities.ReferenceTransform.rotation * rotationAction.ReadValue<Quaternion>();

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
