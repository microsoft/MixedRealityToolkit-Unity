// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Shape of an articulated hand defined by joint poses.
    /// </summary>
    public class ArticulatedHandPose
    {
        private static readonly TrackedHandJoint[] Joints = Enum.GetValues(typeof(TrackedHandJoint)) as TrackedHandJoint[];

        /// <summary>
        /// Represents the maximum number of tracked hand joints.
        /// </summary>
        public static int JointCount { get; } = Joints.Length;

        /// <summary>
        /// Joint poses are stored as right-hand poses in camera space.
        /// Output poses are computed in world space, and mirroring on the x axis for the left hand.
        /// </summary>
        private readonly MixedRealityPose[] localJointPoses;

        public ArticulatedHandPose()
        {
            localJointPoses = new MixedRealityPose[JointCount];
            SetZero();
        }

        public ArticulatedHandPose(MixedRealityPose[] localJointPoses)
        {
            this.localJointPoses = new MixedRealityPose[JointCount];
            Array.Copy(localJointPoses, this.localJointPoses, JointCount);
        }

        public MixedRealityPose GetLocalJointPose(TrackedHandJoint joint, Handedness handedness)
        {
            MixedRealityPose pose = localJointPoses[(int)joint];

            // Pose offset are for right hand, mirror on X axis if left hand is needed
            if (handedness == Handedness.Left)
            {
                pose = new MixedRealityPose(
                    new Vector3(-pose.Position.x, pose.Position.y, pose.Position.z),
                    new Quaternion(pose.Rotation.x, -pose.Rotation.y, -pose.Rotation.z, pose.Rotation.w));
            }

            return pose;
        }

        /// <summary>
        /// Compute world space poses from camera-space joint data.
        /// </summary>
        /// <param name="handedness">Handedness of the resulting pose</param>
        /// <param name="rotation">Rotational offset of the resulting pose</param>
        /// <param name="position">Translational offset of the resulting pose</param>
        /// <param name="jointsOut">Output array of joint poses</param>
        public void ComputeJointPoses(
            Handedness handedness,
            Quaternion rotation,
            Vector3 position,
            MixedRealityPose[] jointsOut)
        {
            for (int i = 0; i < JointCount; i++)
            {
                // Initialize from local offsets
                MixedRealityPose pose = GetLocalJointPose(Joints[i], handedness);
                Vector3 p = pose.Position;
                Quaternion r = pose.Rotation;

                // Apply external transform
                p = position + rotation * p;
                r = rotation * r;

                jointsOut[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Take world space joint poses from any hand and convert into right-hand, camera-space poses.
        /// </summary>
        /// <param name="joints">Input joint poses</param>
        /// <param name="handedness">Handedness of the input data</param>
        /// <param name="rotation">Rotational offset of the input data</param>
        /// <param name="position">Translational offset of the input data</param>
        public void ParseFromJointPoses(
            MixedRealityPose[] joints,
            Handedness handedness,
            Quaternion rotation,
            Vector3 position)
        {
            Quaternion invRotation = Quaternion.Inverse(rotation);
            Quaternion invCameraRotation = Quaternion.Inverse(CameraCache.Main.transform.rotation);

            for (int i = 0; i < JointCount; i++)
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

        /// <summary>
        /// Set all poses to zero.
        /// </summary>
        public void SetZero()
        {
            for (int i = 0; i < JointCount; i++)
            {
                localJointPoses[i] = MixedRealityPose.ZeroIdentity;
            }
        }

        /// <summary>
        /// Copy data from another articulated hand pose.
        /// </summary>
        public void Copy(ArticulatedHandPose other)
        {
            Array.Copy(other.localJointPoses, localJointPoses, JointCount);
        }

        /// <summary>
        /// Blend between two hand poses.
        /// </summary>
        public void InterpolateOffsets(ArticulatedHandPose poseA, ArticulatedHandPose poseB, float value)
        {
            for (int i = 0; i < JointCount; i++)
            {
                var p = Vector3.Lerp(poseA.localJointPoses[i].Position, poseB.localJointPoses[i].Position, value);
                var r = Quaternion.Slerp(poseA.localJointPoses[i].Rotation, poseB.localJointPoses[i].Rotation, value);
                localJointPoses[i] = new MixedRealityPose(p, r);
            }
        }

        /// <summary>
        /// Supported hand gestures.
        /// </summary>
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
            /// Index finger and Thumb touching, grab point does not move
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
            /// <summary>
            /// Relaxed hand pose, grab point does not move
            /// </summary>
            OpenSteadyGrabPoint,
            /// <summary>
            /// Hand facing upwards, Index and Thumb stretched out to start a teleport
            /// </summary>
            TeleportStart,
            /// <summary>
            /// Hand facing upwards, Index curled in to finish a teleport
            /// </summary>
            TeleportEnd,
        }

        [Obsolete("Use SimulatedArticulatedHandPoses class or other custom class")]
        private static readonly Dictionary<GestureId, ArticulatedHandPose> handPoses = new Dictionary<GestureId, ArticulatedHandPose>();

        /// <summary>
        /// Get pose data for a supported gesture.
        /// </summary>
        [Obsolete("Use SimulatedArticulatedHandPoses.GetGesturePose() or other custom class")]
        public static ArticulatedHandPose GetGesturePose(GestureId gesture)
        {
            if (handPoses.TryGetValue(gesture, out ArticulatedHandPose pose))
            {
                return pose;
            }
            return null;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Load pose data from files.
        /// </summary>
        [Obsolete("Use SimulatedArticulatedHandPoses or other custom class")]
        public static void LoadGesturePoses()
        {
            string[] gestureNames = Enum.GetNames(typeof(GestureId));
            for (int i = 0; i < gestureNames.Length; ++i)
            {
                string gestureFileName = string.Format("ArticulatedHandPose_{0}", gestureNames[i]);
                string[] gestureGuids = AssetDatabase.FindAssets(gestureFileName);
                string gesturePath = string.Empty;
                foreach (string guid in gestureGuids)
                {
                    string tempPath = AssetDatabase.GUIDToAssetPath(guid);
                    if (tempPath.Contains("InputSimulation")
                        && tempPath.Contains("ArticulatedHandPoses")
                        && tempPath.Contains(gestureFileName + ".json"))
                    {
                        gesturePath = tempPath;
                        break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(gesturePath))
                {
                    LoadGesturePose((GestureId)i, gesturePath);
                }
            }
        }

        [Obsolete("Use SimulatedArticulatedHandPoses class or other custom class")]
        private static ArticulatedHandPose LoadGesturePose(GestureId gesture, string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var pose = new ArticulatedHandPose();
                pose.FromJson(File.ReadAllText(filePath));
                handPoses.Add(gesture, pose);
                return pose;
            }
            return null;
        }

        [Obsolete("Use SimulatedArticulatedHandPoses class or other custom class")]
        public static void ResetGesturePoses()
        {
            handPoses.Clear();
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
                get
                {
                    int nameIndex = Array.FindIndex(jointNames, IsJointName);
                    if (nameIndex < 0)
                    {
                        Debug.LogError($"Joint name {joint} not in TrackedHandJoint enum");
                        return TrackedHandJoint.None;
                    }
                    return (TrackedHandJoint)nameIndex;
                }
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
            public ArticulatedHandPoseItem[] items = null;

            public void FromJointPoses(MixedRealityPose[] jointPoses)
            {
                items = new ArticulatedHandPoseItem[JointCount];
                for (int i = 0; i < JointCount; ++i)
                {
                    items[i].JointIndex = (TrackedHandJoint)i;
                    items[i].pose = jointPoses[i];
                }
            }

            public void ToJointPoses(MixedRealityPose[] jointPoses)
            {
                for (int i = 0; i < JointCount; ++i)
                {
                    jointPoses[i] = MixedRealityPose.ZeroIdentity;
                }
                foreach (var item in items)
                {
                    jointPoses[(int)item.JointIndex] = item.pose;
                }
            }
        }

        /// <summary>
        /// Serialize pose data to JSON format.
        /// </summary>
        public string ToJson()
        {
            var dict = new ArticulatedHandPoseDictionary();
            dict.FromJointPoses(localJointPoses);
            return JsonUtility.ToJson(dict, true);
        }

        /// <summary>
        /// Deserialize pose data from JSON format.
        /// </summary>
        public void FromJson(string json)
        {
            var dict = JsonUtility.FromJson<ArticulatedHandPoseDictionary>(json);
            dict.ToJointPoses(localJointPoses);
        }
    }
}
