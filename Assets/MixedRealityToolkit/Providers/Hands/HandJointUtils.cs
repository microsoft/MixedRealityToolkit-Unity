// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public static class HandJointUtils
    {
        /// <summary>
        /// Tries to get the pose of the requested joint for the first controller with the specified handedness.
        /// </summary>
        /// <param name="joint">The requested joint</param>
        /// <param name="handedness">The specific hand of interest. This should be either Handedness.Left or Handedness.Right</param>
        /// <param name="pose">The output pose data</param>
        public static bool TryGetJointPose(TrackedHandJoint joint, Handedness handedness, out MixedRealityPose pose)
        {
            return TryGetJointPose<IMixedRealityHand>(joint, handedness, out pose);
        }

        /// <summary>
        /// Try to find the first matching hand controller of the given type and return the pose of the requested joint for that hand.
        /// </summary>
        public static bool TryGetJointPose<T>(TrackedHandJoint joint, Handedness handedness, out MixedRealityPose pose) where T : class, IMixedRealityHand
        {
            T hand = FindHand<T>(handedness);
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
        /// The given handedness should be either Handedness.Left or Handedness.Right.
        /// </remarks>
        public static IMixedRealityHand FindHand(Handedness handedness)
        {
            return FindHand<IMixedRealityHand>(handedness);
        }

        /// <summary>
        /// Find the first detected hand controller of the given type with matching handedness.
        /// </summary>
        public static T FindHand<T>(Handedness handedness) where T : class, IMixedRealityHand
        {
            foreach (var detectedController in CoreServices.InputSystem.DetectedControllers)
            {
                var hand = detectedController as T;
                if (hand != null)
                {
                    if (detectedController.ControllerHandedness.IsMatch(handedness))
                    {
                        return hand;
                    }
                }
            }
            return null;
        }
    }
}
