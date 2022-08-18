// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [Serializable]
    public class InputActionPoseSource : IPoseSource
    {
        [SerializeField]
        InputActionProperty trackingStateActionProperty;

        [SerializeField]
        InputActionProperty positionActionProperty;

        [SerializeField]
        InputActionProperty rotationActionProperty;

        public bool TryGetPose(out Pose pose)
        {
            InputAction trackingStateAction = trackingStateActionProperty.action;
            InputAction positionAction = positionActionProperty.action;
            InputAction rotationAction = rotationActionProperty.action;

            if (trackingStateAction != null && trackingStateAction.enabled
                && positionAction != null && positionAction.enabled
                && rotationAction != null && rotationAction.enabled)
            {
                pose.position = positionAction.ReadValue<Vector3>();
                pose.rotation = rotationAction.ReadValue<Quaternion>();

                return true;
            }
            else
            {
                pose.position = Vector3.zero;
                pose.rotation = Quaternion.identity;

                return false;
            }
        }
    }
}
