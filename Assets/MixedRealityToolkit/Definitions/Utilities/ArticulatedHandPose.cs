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
            LoadGesturePose(GestureId.Flat, "Assets/MixedRealityToolkit.Services/InputSimulation/ArticulatedHandPoses/ArticulatedHandPose_Flat.json");
        }

        private static ArticulatedHandPose LoadGesturePose(GestureId gesture, string filePath)
        {
            if (filePath.Length > 0)
            {
                // pose = (ArticulatedHandPose)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_TimelineJumpFwd.png"), typeof(ArticulatedHandPose));
                // pose = (ArticulatedHandPose)AssetDatabase.LoadAssetAtPath<ArticulatedHandPose>(File.ReadAllText(poseFilePath));
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

        public string GenerateInitializerCode()
        {
            string[] names = Enum.GetNames(typeof(TrackedHandJoint));
            string s = "";
            s += "new MixedRealityPose[]\n";
            s += "{\n";
            for (int i = 0; i < jointCount; ++i)
            {
                Vector3 p = localJointPoses[i].Position;
                Quaternion r = localJointPoses[i].Rotation;
                s += $"    new MixedRealityPose(new Vector3({p.x:F5}f, {p.y:F5}f, {p.z:F5}f), new Quaternion({r.x:F5}f, {r.y:F5}f, {r.z:F5}f, {r.w:F5}f)), // {names[i]}\n";
            }
            s += "}\n";

            return s;
        }

        // static private SimulatedHandPose HandOpened = new SimulatedHandPose(new Vector3[]
        // {
        //     // Right palm is duplicate of right thumb metacarpal and right pinky metacarpal
        //     new Vector3(0.0f,0.0f,0.0f),        // None
        //     new Vector3(-0.036f,0.165f,0.061f),  // Wrist
        //     new Vector3(-0.036f,0.10f,0.051f),  // Palm
        //     new Vector3(-0.020f,0.159f,0.061f), // ThumbMetacarpal
        //     new Vector3(0.018f,0.126f,0.047f),
        //     new Vector3(0.044f,0.107f,0.041f),
        //     new Vector3(0.063f,0.097f,0.040f), // ThumbTip
        //     new Vector3(-0.027f,0.159f,0.061f), // IndexMetacarpal
        //     new Vector3(-0.009f,0.075f,0.033f), 
        //     new Vector3(-0.005f,0.036f,0.017f),
        //     new Vector3(-0.002f,0.015f,0.007f),
        //     new Vector3(0.000f,0.000f,0.000f), // IndexTip
        //     new Vector3(-0.035f,0.159f,0.061f), // MiddleMetacarpal
        //     new Vector3(-0.032f,0.073f,0.032f),
        //     new Vector3(-0.017f,0.077f,-0.002f),
        //     new Vector3(-0.017f,0.104f,-0.001f),
        //     new Vector3(-0.021f,0.119f,0.008f),
        //     new Vector3(-0.043f,0.159f,0.061f), // RingMetacarpal
        //     new Vector3(-0.055f,0.078f,0.032f),
        //     new Vector3(-0.041f,0.080f,0.001f),
        //     new Vector3(-0.037f,0.106f,0.003f),
        //     new Vector3(-0.038f,0.121f,0.012f),
        //     new Vector3(-0.050f,0.159f,0.061f), // PinkyMetacarpal
        //     new Vector3(-0.074f,0.087f,0.031f), // PinkyProximal
        //     new Vector3(-0.061f,0.084f,0.006f), // PinkyIntermediate
        //     new Vector3(-0.054f,0.101f,0.005f), // PinkyDistal
        //     new Vector3(-0.054f,0.116f,0.013f), // PinkyTip
        // });

        static private ArticulatedHandPose HandFlat = new ArticulatedHandPose(new MixedRealityPose[]
        {
            new MixedRealityPose(new Vector3(0.18872f, -0.15009f, -0.36633f), new Quaternion(0.00000f, 0.00000f, 0.00000f, 0.00000f)), // None
            new MixedRealityPose(new Vector3(0.11895f, -0.00955f, -0.04882f), new Quaternion(0.18012f, 0.45945f, 0.20984f, -0.84385f)), // Wrist
            new MixedRealityPose(new Vector3(0.08976f, 0.01252f, -0.01992f), new Quaternion(0.18012f, 0.45945f, 0.20984f, -0.84385f)), // Palm
            new MixedRealityPose(new Vector3(0.09393f, -0.00662f, -0.05127f), new Quaternion(0.50505f, 0.44138f, -0.49259f, -0.55502f)), // ThumbMetacarpalJoint
            new MixedRealityPose(new Vector3(0.05312f, -0.00142f, -0.04708f), new Quaternion(-0.33131f, -0.24515f, 0.58184f, 0.70120f)), // ThumbProximalJoint
            new MixedRealityPose(new Vector3(0.02995f, 0.00427f, -0.02616f), new Quaternion(-0.27801f, -0.22496f, 0.60180f, 0.71424f)), // ThumbDistalJoint
            new MixedRealityPose(new Vector3(0.01862f, 0.00644f, -0.01333f), new Quaternion(-0.27801f, -0.22496f, 0.60180f, 0.71424f)), // ThumbTip
            new MixedRealityPose(new Vector3(0.09991f, -0.00383f, -0.04370f), new Quaternion(0.15775f, 0.57478f, 0.28421f, -0.75098f)), // IndexMetacarpal
            new MixedRealityPose(new Vector3(0.05414f, 0.02951f, -0.02658f), new Quaternion(0.05536f, -0.47983f, -0.00242f, 0.87562f)), // IndexKnuckle
            new MixedRealityPose(new Vector3(0.02434f, 0.02616f, -0.00766f), new Quaternion(0.40229f, -0.40146f, 0.28802f, 0.77079f)), // IndexMiddleJoint
            new MixedRealityPose(new Vector3(0.01572f, 0.00721f, 0.00022f), new Quaternion(0.61683f, -0.29131f, 0.32876f, 0.65341f)), // IndexDistalJoint
            new MixedRealityPose(new Vector3(0.01609f, -0.00803f, 0.00128f), new Quaternion(0.61683f, -0.29131f, 0.32876f, 0.65341f)), // IndexTip
            new MixedRealityPose(new Vector3(0.10500f, -0.00374f, -0.03830f), new Quaternion(0.25380f, 0.44550f, 0.18895f, -0.83750f)), // MiddleMetacarpal
            new MixedRealityPose(new Vector3(0.06771f, 0.03029f, -0.01111f), new Quaternion(0.08102f, -0.43972f, -0.05162f, 0.89300f)), // MiddleKnuckle
            new MixedRealityPose(new Vector3(0.03464f, 0.02615f, 0.01389f), new Quaternion(0.51721f, -0.40289f, 0.19070f, 0.73071f)), // MiddleMiddleJoint
            new MixedRealityPose(new Vector3(0.02484f, 0.00339f, 0.01741f), new Quaternion(0.78471f, -0.26305f, 0.25014f, 0.50315f)), // MiddleDistalJoint
            new MixedRealityPose(new Vector3(0.02690f, -0.01164f, 0.01139f), new Quaternion(0.78471f, -0.26305f, 0.25014f, 0.50315f)), // MiddleTip
            new MixedRealityPose(new Vector3(0.11127f, -0.00368f, -0.03065f), new Quaternion(0.23752f, 0.36351f, 0.09240f, -0.89605f)), // RingMetacarpal
            new MixedRealityPose(new Vector3(0.07842f, 0.02297f, 0.00303f), new Quaternion(0.10007f, -0.41463f, -0.08395f, 0.90058f)), // RingKnuckle
            new MixedRealityPose(new Vector3(0.05159f, 0.01908f, 0.02537f), new Quaternion(0.68355f, -0.37598f, 0.16828f, 0.60265f)), // RingMiddleJoint
            new MixedRealityPose(new Vector3(0.04688f, -0.00098f, 0.02079f), new Quaternion(0.88073f, -0.23392f, 0.22665f, 0.34449f)), // RingDistalJoint
            new MixedRealityPose(new Vector3(0.05116f, -0.01384f, 0.00888f), new Quaternion(0.88073f, -0.23392f, 0.22665f, 0.34449f)), // RingTip
            new MixedRealityPose(new Vector3(0.11647f, -0.00659f, -0.02443f), new Quaternion(0.21259f, 0.27599f, -0.00653f, -0.93733f)), // PinkyMetacarpal
            new MixedRealityPose(new Vector3(0.08950f, 0.01389f, 0.01484f), new Quaternion(0.06588f, -0.39462f, -0.14201f, 0.90542f)), // PinkyKnuckle
            new MixedRealityPose(new Vector3(0.07198f, 0.01372f, 0.03108f), new Quaternion(0.56426f, -0.38403f, 0.00787f, 0.73086f)), // PinkyMiddleJoint
            new MixedRealityPose(new Vector3(0.06240f, -0.00069f, 0.03227f), new Quaternion(0.81600f, -0.32333f, -0.01271f, 0.47941f)), // PinkyDistalJoint
            new MixedRealityPose(new Vector3(0.05779f, -0.01145f, 0.02476f), new Quaternion(0.81600f, -0.32333f, -0.01271f, 0.47941f)), // PinkyTip
        });

        // static private SimulatedHandPose HandPinch = new SimulatedHandPose(new Vector3[]
        // {
        //     // Right palm is duplicate of right thumb metacarpal and right pinky metacarpal
        //     new Vector3(0.0f,0.0f,0.0f),        // None
        //     new Vector3(-0.042f,0.111f,0.060f), // Wrist
        //     new Vector3(-0.042f,0.051f,0.060f), // Palm
        //     new Vector3(-0.032f,0.091f,0.060f), // ThumbMetacarpal
        //     new Vector3(-0.013f,0.052f,0.044f),
        //     new Vector3(0.002f,0.026f,0.030f),
        //     new Vector3(0.007f,0.007f,0.017f),  // ThumbTip
        //     new Vector3(-0.038f,0.091f,0.060f), // IndexMetacarpal
        //     new Vector3(-0.029f,0.008f,0.050f),
        //     new Vector3(-0.009f,-0.016f,0.025f),
        //     new Vector3(-0.002f,-0.011f,0.008f),
        //     new Vector3(0.000f,0.000f,0.000f),
        //     new Vector3(-0.042f,0.091f,0.060f), // MiddleMetacarpal
        //     new Vector3(-0.050f,0.004f,0.046f),
        //     new Vector3(-0.026f,0.004f,0.014f),
        //     new Vector3(-0.028f,0.031f,0.014f),
        //     new Vector3(-0.034f,0.048f,0.020f),
        //     new Vector3(-0.048f,0.091f,0.060f), // RingMetacarpal
        //     new Vector3(-0.071f,0.008f,0.041f),
        //     new Vector3(-0.048f,0.009f,0.012f),
        //     new Vector3(-0.046f,0.036f,0.014f),
        //     new Vector3(-0.050f,0.052f,0.022f),
        //     new Vector3(-0.052f,0.091f,0.060f), // PinkyMetacarpal
        //     new Vector3(-0.088f,0.014f,0.034f),
        //     new Vector3(-0.067f,0.012f,0.013f),
        //     new Vector3(-0.061f,0.031f,0.014f),
        //     new Vector3(-0.062f,0.046f,0.021f),
        // });

        // static private SimulatedHandPose HandPinchSteadyWrist = new SimulatedHandPose(new Vector3[]
        // {
        //     new Vector3(0.0f, 0.0f, 0.0f), // None
        //     new Vector3(-0.03600f, 0.16500f, 0.06100f), // Wrist
        //     new Vector3(-0.03600f, 0.10500f, 0.06100f), // Palm
        //     new Vector3(-0.02600f, 0.14500f, 0.06100f), // ThumbMetacarpalJoint
        //     new Vector3(-0.00700f, 0.10600f, 0.04500f), // ThumbProximalJoint
        //     new Vector3(0.00800f, 0.08000f, 0.03100f), // ThumbDistalJoint
        //     new Vector3(0.01300f, 0.06100f, 0.01800f), // ThumbTip
        //     new Vector3(-0.03200f, 0.14500f, 0.06100f), // IndexMetacarpal
        //     new Vector3(-0.02300f, 0.06200f, 0.05100f), // IndexKnuckle
        //     new Vector3(-0.00300f, 0.03800f, 0.02600f), // IndexMiddleJoint
        //     new Vector3(0.00400f, 0.04300f, 0.00900f), // IndexDistalJoint
        //     new Vector3(0.00600f, 0.05400f, 0.00100f), // IndexTip
        //     new Vector3(-0.03600f, 0.14500f, 0.06100f), // MiddleMetacarpal
        //     new Vector3(-0.04400f, 0.05800f, 0.04700f), // MiddleKnuckle
        //     new Vector3(-0.02000f, 0.05800f, 0.01500f), // MiddleMiddleJoint
        //     new Vector3(-0.02200f, 0.08500f, 0.01500f), // MiddleDistalJoint
        //     new Vector3(-0.02800f, 0.10200f, 0.02100f), // MiddleTip
        //     new Vector3(-0.04200f, 0.14500f, 0.06100f), // RingMetacarpal
        //     new Vector3(-0.06500f, 0.06200f, 0.04200f), // RingKnuckle
        //     new Vector3(-0.04200f, 0.06300f, 0.01300f), // RingMiddleJoint
        //     new Vector3(-0.04000f, 0.09000f, 0.01500f), // RingDistalJoint
        //     new Vector3(-0.04400f, 0.10600f, 0.02300f), // RingTip
        //     new Vector3(-0.04600f, 0.14500f, 0.06100f), // PinkyMetacarpal
        //     new Vector3(-0.08200f, 0.06800f, 0.03500f), // PinkyKnuckle
        //     new Vector3(-0.06100f, 0.06600f, 0.01400f), // PinkyMiddleJoint
        //     new Vector3(-0.05500f, 0.08500f, 0.01500f), // PinkyDistalJoint
        //     new Vector3(-0.05600f, 0.10000f, 0.02200f), // PinkyTip
        // });

        // static private SimulatedHandPose HandPoke = new SimulatedHandPose(new Vector3[]
        // {
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // None
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // Wrist
        //     new Vector3(-0.03259f, 0.03268f, 0.02552f), // Palm
        //     new Vector3(-0.03259f, 0.03268f, 0.02552f), // ThumbMetacarpalJoint
        //     new Vector3(-0.00118f, 0.05920f, 0.03279f), // ThumbProximalJoint
        //     new Vector3(0.00171f, 0.05718f, 0.00273f), // ThumbDistalJoint
        //     new Vector3(-0.00877f, 0.05977f, -0.00905f), // ThumbTip
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // IndexMetacarpal
        //     new Vector3(-0.00508f, 0.01676f, 0.02067f), // IndexKnuckle
        //     new Vector3(0.00320f, -0.00908f, -0.00469f), // IndexMiddleJoint
        //     new Vector3(0.00987f, -0.01695f, -0.02251f), // IndexDistalJoint
        //     new Vector3(0.01363f, -0.02002f, -0.03255f), // IndexTip
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // MiddleMetacarpal
        //     new Vector3(-0.02474f, 0.01422f, 0.01270f), // MiddleKnuckle
        //     new Vector3(-0.00556f, 0.03787f, -0.01700f), // MiddleMiddleJoint
        //     new Vector3(-0.00648f, 0.06095f, -0.00658f), // MiddleDistalJoint
        //     new Vector3(-0.01209f, 0.06180f, 0.00499f), // MiddleTip
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // RingMetacarpal
        //     new Vector3(-0.04422f, 0.01955f, 0.00797f), // RingKnuckle
        //     new Vector3(-0.02535f, 0.04385f, -0.01667f), // RingMiddleJoint
        //     new Vector3(-0.02465f, 0.06521f, -0.00440f), // RingDistalJoint
        //     new Vector3(-0.02953f, 0.06711f, 0.00726f), // RingTip
        //     new Vector3(-0.06209f, 0.07595f, 0.07231f), // PinkyMetacarpal
        //     new Vector3(-0.06218f, 0.02740f, 0.00434f), // PinkyKnuckle
        //     new Vector3(-0.04335f, 0.04456f, -0.01437f), // PinkyMiddleJoint
        //     new Vector3(-0.03864f, 0.06018f, -0.00822f), // PinkyDistalJoint
        //     new Vector3(-0.04340f, 0.06202f, 0.00248f), // PinkyTip
        // });

        // static private SimulatedHandPose HandGrab = new SimulatedHandPose(new Vector3[]
        // {
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // None
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // Wrist
        //     new Vector3(-0.02248f, -0.03254f, 0.01987f), // Palm
        //     new Vector3(-0.02248f, -0.03254f, 0.01987f), // ThumbMetacarpalJoint
        //     new Vector3(0.01379f, -0.00633f, 0.02738f), // ThumbProximalJoint
        //     new Vector3(0.02485f, -0.02278f, 0.00441f), // ThumbDistalJoint
        //     new Vector3(0.01847f, -0.02790f, -0.00937f), // ThumbTip
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // IndexMetacarpal
        //     new Vector3(0.00565f, -0.04883f, 0.01896f), // IndexKnuckle
        //     new Vector3(0.01153f, -0.02915f, -0.01358f), // IndexMiddleJoint
        //     new Vector3(0.00547f, -0.00864f, -0.01074f), // IndexDistalJoint
        //     new Vector3(0.00098f, -0.00265f, -0.00172f), // IndexTip
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // MiddleMetacarpal
        //     new Vector3(-0.01343f, -0.05324f, 0.01296f), // MiddleKnuckle
        //     new Vector3(-0.00060f, -0.03063f, -0.02099f), // MiddleMiddleJoint
        //     new Vector3(-0.00567f, -0.00696f, -0.01334f), // MiddleDistalJoint
        //     new Vector3(-0.01080f, -0.00381f, -0.00193f), // MiddleTip
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // RingMetacarpal
        //     new Vector3(-0.03363f, -0.05134f, 0.00849f), // RingKnuckle
        //     new Vector3(-0.02000f, -0.02888f, -0.02123f), // RingMiddleJoint
        //     new Vector3(-0.02304f, -0.00675f, -0.01062f), // RingDistalJoint
        //     new Vector3(-0.02754f, -0.00310f, 0.00082f), // RingTip
        //     new Vector3(-0.05340f, 0.02059f, 0.05460f), // PinkyMetacarpal
        //     new Vector3(-0.05225f, -0.04629f, 0.00384f), // PinkyKnuckle
        //     new Vector3(-0.03698f, -0.03051f, -0.01900f), // PinkyMiddleJoint
        //     new Vector3(-0.03617f, -0.01485f, -0.01130f), // PinkyDistalJoint
        //     new Vector3(-0.03902f, -0.00873f, -0.00159f), // PinkyTip
        // });

        // static private SimulatedHandPose HandThumbsUp = new SimulatedHandPose(new Vector3[]
        // {
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // None
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // Wrist
        //     new Vector3(0.00054f, 0.01716f, -0.03472f), // Palm
        //     new Vector3(0.00054f, 0.01716f, -0.03472f), // ThumbMetacarpalJoint
        //     new Vector3(-0.03106f, -0.02225f, -0.00120f), // ThumbProximalJoint
        //     new Vector3(-0.02123f, -0.04992f, 0.00199f), // ThumbDistalJoint
        //     new Vector3(-0.01793f, -0.06510f, 0.00358f), // ThumbTip
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // IndexMetacarpal
        //     new Vector3(0.01284f, -0.01185f, -0.03795f), // IndexKnuckle
        //     new Vector3(0.03290f, -0.00608f, -0.00720f), // IndexMiddleJoint
        //     new Vector3(0.01647f, 0.00258f, 0.00238f), // IndexDistalJoint
        //     new Vector3(0.00559f, 0.00502f, 0.00017f), // IndexTip
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // MiddleMetacarpal
        //     new Vector3(0.01777f, 0.00604f, -0.04572f), // MiddleKnuckle
        //     new Vector3(0.03731f, 0.00686f, -0.00898f), // MiddleMiddleJoint
        //     new Vector3(0.01713f, 0.01536f, 0.00219f), // MiddleDistalJoint
        //     new Vector3(0.00542f, 0.01846f, -0.00091f), // MiddleTip
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // RingMetacarpal
        //     new Vector3(0.01836f, 0.02624f, -0.04858f), // RingKnuckle
        //     new Vector3(0.03987f, 0.02388f, -0.01663f), // RingMiddleJoint
        //     new Vector3(0.02182f, 0.02969f, -0.00200f), // RingDistalJoint
        //     new Vector3(0.00978f, 0.03257f, -0.00310f), // RingTip
        //     new Vector3(-0.05754f, 0.04969f, -0.01566f), // PinkyMetacarpal
        //     new Vector3(0.01784f, 0.04561f, -0.04763f), // PinkyKnuckle
        //     new Vector3(0.03771f, 0.03986f, -0.02511f), // PinkyMiddleJoint
        //     new Vector3(0.02634f, 0.04067f, -0.01262f), // PinkyDistalJoint
        //     new Vector3(0.01505f, 0.04267f, -0.01241f), // PinkyTip
        // });

        // static private SimulatedHandPose HandVictory = new SimulatedHandPose(new Vector3[]
        // {
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // None
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // Wrist
        //     new Vector3(-0.05800f, 0.07626f, 0.00397f), // Palm
        //     new Vector3(-0.05800f, 0.07626f, 0.00397f), // ThumbMetacarpalJoint
        //     new Vector3(-0.03418f, 0.10243f, -0.00761f), // ThumbProximalJoint
        //     new Vector3(-0.03570f, 0.08857f, -0.03347f), // ThumbDistalJoint
        //     new Vector3(-0.04603f, 0.08584f, -0.04472f), // ThumbTip
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // IndexMetacarpal
        //     new Vector3(-0.03200f, 0.05950f, 0.00836f), // IndexKnuckle
        //     new Vector3(-0.01535f, 0.02702f, 0.00475f), // IndexMiddleJoint
        //     new Vector3(-0.00577f, 0.00904f, 0.00218f), // IndexDistalJoint
        //     new Vector3(-0.00046f, -0.00064f, 0.00053f), // IndexTip
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // MiddleMetacarpal
        //     new Vector3(-0.05017f, 0.05484f, 0.00171f), // MiddleKnuckle
        //     new Vector3(-0.04307f, 0.01754f, -0.01398f), // MiddleMiddleJoint
        //     new Vector3(-0.03889f, -0.00439f, -0.02321f), // MiddleDistalJoint
        //     new Vector3(-0.03677f, -0.01553f, -0.02789f), // MiddleTip
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // RingMetacarpal
        //     new Vector3(-0.06885f, 0.05706f, -0.00568f), // RingKnuckle
        //     new Vector3(-0.05015f, 0.04311f, -0.03622f), // RingMiddleJoint
        //     new Vector3(-0.04307f, 0.06351f, -0.04646f), // RingDistalJoint
        //     new Vector3(-0.04840f, 0.07039f, -0.03760f), // RingTip
        //     new Vector3(-0.08835f, 0.13334f, 0.02745f), // PinkyMetacarpal
        //     new Vector3(-0.08544f, 0.06253f, -0.01371f), // PinkyKnuckle
        //     new Vector3(-0.06657f, 0.07318f, -0.03519f), // PinkyMiddleJoint
        //     new Vector3(-0.06393f, 0.08967f, -0.03293f), // PinkyDistalJoint
        //     new Vector3(-0.06748f, 0.09752f, -0.02542f), // PinkyTip
        // });
    }
}