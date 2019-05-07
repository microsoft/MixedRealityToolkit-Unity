// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
#endif

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Shape of an articulated hand defined by joint poses.
    /// </summary>
    public class ArticulatedHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        private MixedRealityPose[] localJointPoses;

        public ArticulatedHandPose()
        {
            localJointPoses = new MixedRealityPose[jointCount];
            SetZero();
        }

        public ArticulatedHandPose(MixedRealityPose[] _localJointPoses)
        {
            localJointPoses = new MixedRealityPose[jointCount];
            Array.Copy(_localJointPoses, localJointPoses, jointCount);
        }

        public void ComputeJointPoses(Handedness handedness, Quaternion rotation, Vector3 position, MixedRealityPose[] jointsOut)
        {
            var cameraRotation = CameraCache.Main.transform.rotation;

            for (int i = 0; i < jointCount; i++)
            {
                // Initialize from local offsets
                Vector3 p = localJointPoses[i].Position;
                Quaternion r = localJointPoses[i].Rotation;

                // Pose offset are for right hand, mirror on X axis if left hand is needed
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                // Apply camera transform
                p = cameraRotation * p;
                r = cameraRotation * r;

                // Apply external transform
                p = position + rotation * p;
                r = rotation * r;

                jointsOut[i] = new MixedRealityPose(p, r);
            }
        }

        public void ParseFromJointPoses(MixedRealityPose[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            var invRotation = Quaternion.Inverse(rotation);
            var invCameraRotation = Quaternion.Inverse(CameraCache.Main.transform.rotation);

            for (int i = 0; i < jointCount; i++)
            {
                Vector3 p = joints[i].Position;
                Quaternion r = joints[i].Rotation;

                // Apply inverse external transform
                p = invRotation * (p - position);
                r = invRotation * r;

                // To camera space
                p = invCameraRotation * p;
                r = invCameraRotation * r;

                // Pose offset are for right hand, mirror on X axis if left hand is given
                if (handedness == Handedness.Left)
                {
                    p.x = -p.x;
                    r.y = -r.y;
                    r.z = -r.z;
                }

                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        public void SetZero()
        {
            for (int i = 0; i < jointCount; i++)
            {
                localJointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
        }

        public void Copy(ArticulatedHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, jointCount);
        }

        public void InterpolateOffsets(ArticulatedHandPose poseA, ArticulatedHandPose poseB, float value)
        {
            for (int i = 0; i < jointCount; i++)
            {
                var p = Vector3.Lerp(poseA.localJointPoses[i].Position, poseB.localJointPoses[i].Position, value);
                var r = Quaternion.Slerp(poseA.localJointPoses[i].Rotation, poseB.localJointPoses[i].Rotation, value);
                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        public void TransitionTo(ArticulatedHandPose other, float lastAnim, float currentAnim)
        {
            if (currentAnim <= lastAnim)
            {
                return;
            }

            float range = Mathf.Clamp01(1.0f - lastAnim);
            float lerpFactor = range > 0.0f ? (currentAnim - lastAnim) / range : 1.0f;
            for (int i = 0; i < jointCount; i++)
            {
                var p = Vector3.Lerp(localJointPoses[i].Position, other.localJointPoses[i].Position, lerpFactor);
                var r = Quaternion.Slerp(localJointPoses[i].Rotation, other.localJointPoses[i].Rotation, lerpFactor);
                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        public enum GestureId
        {
            /// <summary>
            /// Unspecified hand shape
            /// </summary>
            None = 0,
            /// <summary>
            /// Flat hand with fingers spread out
            /// </summary>
            Flat,
            /// <summary>
            /// Relaxed hand pose
            /// </summary>
            Open,
            /// <summary>
            /// Index finger and Thumb touching, index tip does not move
            /// </summary>
            Pinch,
            /// <summary>
            /// Index finger and Thumb touching, wrist does not move
            /// </summary>
            PinchSteadyWrist,
            /// <summary>
            /// Index finger stretched out
            /// </summary>
            Poke,
            /// <summary>
            /// Grab with whole hand, fist shape
            /// </summary>
            Grab,
            /// <summary>
            /// OK sign
            /// </summary>
            ThumbsUp,
            /// <summary>
            /// Victory sign
            /// </summary>
            Victory,
        }

        private static readonly Dictionary<GestureId, ArticulatedHandPose> handPoses = new Dictionary<GestureId, ArticulatedHandPose>();

        public static ArticulatedHandPose GetGesturePose(GestureId gesture)
        {
            if (handPoses.TryGetValue(gesture, out ArticulatedHandPose pose))
            {
                return pose;
            }
            return null;
        }

        #if UNITY_EDITOR
        public static void LoadGesturePoses()
        {
            string basePath = "Assets/MixedRealityToolkit.Services/InputSimulation/ArticulatedHandPoses/";
            LoadGesturePose(GestureId.Flat, basePath + "ArticulatedHandPose_Flat.json");
            LoadGesturePose(GestureId.Open, basePath + "ArticulatedHandPose_Open.json");
            LoadGesturePose(GestureId.Pinch, basePath + "ArticulatedHandPose_Pinch.json");
            LoadGesturePose(GestureId.PinchSteadyWrist, basePath + "ArticulatedHandPose_PinchSteadyWrist.json");
            LoadGesturePose(GestureId.Poke, basePath + "ArticulatedHandPose_Poke.json");
            LoadGesturePose(GestureId.Grab, basePath + "ArticulatedHandPose_Grab.json");
            LoadGesturePose(GestureId.ThumbsUp, basePath + "ArticulatedHandPose_ThumbsUp.json");
            LoadGesturePose(GestureId.Victory, basePath + "ArticulatedHandPose_Victory.json");
        }

        private static ArticulatedHandPose LoadGesturePose(GestureId gesture, string filePath)
        {
            if (filePath.Length > 0)
            {
                var pose = new ArticulatedHandPose();
                pose.FromJson(File.ReadAllText(filePath));
                handPoses.Add(gesture, pose);
                return pose;
            }
            return null;
        }
        #endif

        /// Utility class to serialize hand pose as a dictionary with full joint names
        [Serializable]
        internal struct ArticulatedHandPoseItem
        {
            private static readonly string[] jointNames = Enum.GetNames(typeof(TrackedHandJoint));

            public string joint;
            public MixedRealityPose pose;

            public TrackedHandJoint JointIndex
            {
                get { return (TrackedHandJoint)Array.FindIndex(jointNames, IsJointName); }
                set { joint = jointNames[(int)value]; }
            }

            private bool IsJointName(string s)
            {
                return s == joint;
            }

            public ArticulatedHandPoseItem(TrackedHandJoint joint, MixedRealityPose pose)
            {
                this.joint = jointNames[(int)joint];
                this.pose = pose;
            }
        }

        /// Utility class to serialize hand pose as a dictionary with full joint names
        [Serializable]
        internal class ArticulatedHandPoseDictionary
        {
            private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

            public ArticulatedHandPoseItem[] items = null;

            public void FromJointPoses(MixedRealityPose[] jointPoses)
            {
                items = new ArticulatedHandPoseItem[jointCount];
                for (int i = 0; i < jointCount; ++i)
                {
                    items[i].JointIndex = (TrackedHandJoint)i;
                    items[i].pose = jointPoses[i];
                }
            }

            public void ToJointPoses(MixedRealityPose[] jointPoses)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    jointPoses[i] = MixedRealityPose.ZeroIdentity;
                }
                foreach (var item in items)
                {
                    jointPoses[(int)item.JointIndex] = item.pose;
                }
            }
        }

        public string ToJson()
        {
            var dict = new ArticulatedHandPoseDictionary();
            dict.FromJointPoses(localJointPoses);
            return JsonUtility.ToJson(dict);
        }

        public void FromJson(string json)
        {
            var dict = JsonUtility.FromJson<ArticulatedHandPoseDictionary>(json);
            dict.ToJointPoses(localJointPoses);
        }
    }
}