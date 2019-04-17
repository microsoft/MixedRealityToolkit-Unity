// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Linq;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class SimulatedHandUtils
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        /// <summary>
        /// Compute the rotation of each joint, with the forward vector of the rotation pointing along the joint bone, 
        /// and the up vector pointing up.
        /// 
        /// The rotation of the base joints (thumb base, pinky base, etc) as well as the wrist joint is set to 
        /// point in the direction of palm forward.
        /// 
        /// Assumption: the position of each joint has been copied from handData joint positions
        /// </summary>
        ///
        /// Notes:
        ///  - GetPalmUpVector and GetPalmForwardVector appear to be flipped.  GetPalmForwardVector appears
        ///    to return the vector that extends perpendicular from the palm, which might be thought of as 'up'
        public static void CalculateJointRotations(Handedness handedness, Vector3[] jointPositions, Quaternion[] jointOrientationsOut)
        {
            const int numFingers = 5;
            int[] jointsPerFinger = { 4, 5, 5, 5, 5 }; // thumb, index, middle, right, pinky

            for (int fingerIndex = 0; fingerIndex < numFingers; fingerIndex++)
            {
                int jointsCurrentFinger = jointsPerFinger[fingerIndex];
                int lowIndex = (int)TrackedHandJoint.ThumbMetacarpalJoint + jointsPerFinger.Take(fingerIndex).Sum();
                int highIndex = lowIndex + jointsCurrentFinger - 1;

                for (int jointStartidx = lowIndex; jointStartidx <= highIndex; jointStartidx++)
                {
                    // If we are at the lowIndex (metacarpals) use the wrist as the previous joint.
                    int jointEndidx = jointStartidx == lowIndex ? (int)TrackedHandJoint.Wrist : jointStartidx - 1;
                    Vector3 boneForward = jointPositions[jointStartidx] - jointPositions[jointEndidx];
                    Vector3 boneUp = Vector3.Cross(boneForward, GetPalmRightVector(handedness, jointPositions));
                    if (boneForward.magnitude > float.Epsilon && boneUp.magnitude > float.Epsilon)
                    {
                        Quaternion jointRotation = Quaternion.LookRotation(boneForward, boneUp);
                        // If we are the thumb, set the up vector to be from pinky to index (right hand) or index to pinky (left hand).
                        if (fingerIndex == 0)
                        {
                            // Rotate the thumb by 90 degrees (-90 if left hand) about thumb forward vector.
                            Quaternion rotateThumb90 = Quaternion.AngleAxis(handedness == Handedness.Left ? -90 : 90, boneForward);
                            jointRotation = rotateThumb90 * jointRotation;
                        }
                        jointOrientationsOut[jointStartidx] = jointRotation;
                    }
                    else
                    {
                        jointOrientationsOut[jointStartidx] = Quaternion.identity;
                    }
                }
            }
            jointOrientationsOut[(int)TrackedHandJoint.Palm] = Quaternion.LookRotation(GetPalmForwardVector(jointPositions), GetPalmUpVector(handedness, jointPositions));
        }

        /// <summary>
        /// Gets vector corresponding to +z.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPalmForwardVector(Vector3[] jointPositions)
        {
            Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
            Vector3 thumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];

            Vector3 thumbMetaCarpalToIndex = indexBase - thumbMetaCarpal;
            return thumbMetaCarpalToIndex.normalized;
        }

        /// <summary>
        /// Gets the vector corresponding to +y.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetPalmUpVector(Handedness handedness, Vector3[] jointPositions)
        {
            Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
            Vector3 pinkyBase = jointPositions[(int)TrackedHandJoint.PinkyKnuckle];
            Vector3 ThumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];

            Vector3 ThumbMetaCarpalToPinky = pinkyBase - ThumbMetaCarpal;
            Vector3 ThumbMetaCarpalToIndex = indexBase - ThumbMetaCarpal;
            if (handedness == Handedness.Left)
            {
                return Vector3.Cross(ThumbMetaCarpalToPinky, ThumbMetaCarpalToIndex).normalized;
            }
            else
            {
                return Vector3.Cross(ThumbMetaCarpalToIndex, ThumbMetaCarpalToPinky).normalized;
            }
        }


        public static Vector3 GetPalmRightVector(Handedness handedness, Vector3[] jointPositions)
        {
            Vector3 indexBase = jointPositions[(int)TrackedHandJoint.IndexKnuckle];
            Vector3 pinkyBase = jointPositions[(int)TrackedHandJoint.PinkyKnuckle];
            Vector3 thumbMetaCarpal = jointPositions[(int)TrackedHandJoint.ThumbMetacarpalJoint];

            Vector3 thumbMetaCarpalToPinky = pinkyBase - thumbMetaCarpal;
            Vector3 thumbMetaCarpalToIndex = indexBase - thumbMetaCarpal;
            Vector3 thumbMetaCarpalUp = Vector3.zero;
            if (handedness == Handedness.Left)
            {
                thumbMetaCarpalUp = Vector3.Cross(thumbMetaCarpalToPinky, thumbMetaCarpalToIndex).normalized;
            }
            else
            {
                thumbMetaCarpalUp = Vector3.Cross(thumbMetaCarpalToIndex, thumbMetaCarpalToPinky).normalized;
            }

            return Vector3.Cross(thumbMetaCarpalUp, thumbMetaCarpalToIndex).normalized;
        }
    }
}