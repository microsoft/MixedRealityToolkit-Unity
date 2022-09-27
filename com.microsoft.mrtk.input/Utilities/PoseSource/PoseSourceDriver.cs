// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Uses a specified <see cref="IPoseSource"/> to drive the pose of the object this is placed on.
    /// </summary>
    /// <remarks>Similar in purpose to <see cref="UnityEngine.InputSystem.XR.TrackedPoseDriver"/>.</remarks>
    [AddComponentMenu("MRTK/Input/Pose Source Driver")]
    internal class PoseSourceDriver : MonoBehaviour
    {
        [SerializeReference, InterfaceSelector]
        private IPoseSource poseSource;

        protected void Update()
        {
            if (poseSource.TryGetPose(out Pose pose))
            {
                transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
        }
    }
}
