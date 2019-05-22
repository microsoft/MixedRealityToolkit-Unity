// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// TODO
    /// </summary>
    public class HandConstraintPalmUp : HandConstraint
    {
        [Header("Palm Up")]
        [SerializeField]
        [Tooltip("TODO")]
        private float jointFacingThreshold = 0.3f;

        protected override bool IsHandActive(IMixedRealityHand hand)
        {
            if (!base.IsHandActive(hand))
            {
                return false;
            }

            MixedRealityPose pose;

            if (hand.TryGetJoint(TrackedHandJoint.Palm, out pose))
            {
                return Vector3.Dot(pose.Up, CameraCache.Main.transform.forward) > jointFacingThreshold;
            }

            return true;
        }
    }
}
