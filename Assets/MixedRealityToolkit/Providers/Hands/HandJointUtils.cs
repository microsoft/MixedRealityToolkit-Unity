// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class HandJointUtils
    {
        /// <summary>
        /// Tries to get the the pose of the requested joint for the first controller with the specified handedness.
        /// </summary>
        /// <param name="joint">The requested joint</param>
        /// <param name="handedness">The specific hand of interest. This should be either Handedness.Left or Handedness.Right</param>
        /// <param name="pose">The output pose data</param>
        public static bool TryGetJointPose(TrackedHandJoint joint, Handedness handedness, out MixedRealityPose pose)
        {
            IMixedRealityHand hand = FindHand(handedness);
            if (hand != null)
            {
                return hand.TryGetJoint(joint, out pose);
            }

            pose = MixedRealityPose.ZeroIdentity;
            return false;
        }

        /// <summary>
        /// Find the first detected hand controller with matching handedness.
        /// </summary>
        /// <remarks>
        /// The given handeness should be either Handedness.Left or Handedness.Right.
        /// </remarks>
        public static IMixedRealityHand FindHand(Handedness handedness)
        {
            foreach (var detectedController in MixedRealityToolkit.InputSystem.DetectedControllers)
            {
                var hand = detectedController as IMixedRealityHand;
                if (hand != null)
                {
                    if (detectedController.ControllerHandedness == handedness)
                    {
                        return hand;
                    }
                }
            }
            return null;
        }
    }
}
