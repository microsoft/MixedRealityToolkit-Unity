// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;

namespace Microsoft.MixedReality.Toolkit.Input
{

    public class SimulatedHandPose
    {
        private static readonly int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        // TODO Right now animation is just lerping joint offsets
        // For better transitions, esp. when wrist is rotating, use hierarchical bone rotations!
        private Vector3[] jointOffsets;

        public SimulatedHandPose()
        {
            jointOffsets = new Vector3[jointCount];
            SetZero();
        }

        public SimulatedHandPose(Vector3[] _jointOffsets)
        {
            jointOffsets = new Vector3[jointCount];
            Array.Copy(_jointOffsets, jointOffsets, jointCount);
        }

        public void ComputeJointPositions(Handedness handedness, Quaternion rotation, Vector3 position, Vector3[] jointsOut)
        {
            // Initialize from local offsets
            for (int i = 0; i < jointCount; i++)
            {
                jointsOut[i] = -jointOffsets[i];
            }

            // Pose offset are for right hand, flip if left hand is needed
            if (handedness == Handedness.Left)
            {
                for (int i = 0; i < jointCount; i++)
                {
                    jointsOut[i].x = -jointsOut[i].x;
                }
            }

            // Apply camera transform
            for (int i = 0; i < jointCount; i++)
            {
                jointsOut[i] = Camera.main.transform.TransformDirection(jointsOut[i]);
            }

            for (int i = 0; i < jointCount; i++)
            {
                jointsOut[i] = position + rotation * jointsOut[i];
            }
        }

        public void ParseFromJointPositions(Vector3[] joints, Handedness handedness, Quaternion rotation, Vector3 position)
        {
            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = Quaternion.Inverse(rotation) * (joints[i] - position);
            }

            // To camera space
            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = Camera.main.transform.InverseTransformDirection(jointOffsets[i]);
            }

            // Pose offset are for right hand, flip if left hand is given
            if (handedness == Handedness.Left)
            {
                for (int i = 0; i < jointCount; i++)
                {
                    jointOffsets[i].x = -jointOffsets[i].x;
                }
            }

            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = -jointOffsets[i];
            }
        }

        public void SetZero()
        {
            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = Vector3.zero;
            }
        }

        public void Copy(SimulatedHandPose other)
        {
            Array.Copy(other.jointOffsets, jointOffsets, jointCount);
        }

        public void InterpolateOffsets(SimulatedHandPose poseA, SimulatedHandPose poseB, float value)
        {
            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = Vector3.Lerp(poseA.jointOffsets[i], poseB.jointOffsets[i], value);
            }
        }

        public void TransitionTo(SimulatedHandPose other, float lastAnim, float currentAnim)
        {
            if (currentAnim <= lastAnim)
            {
                return;
            }

            float range = Mathf.Clamp01(1.0f - lastAnim);
            float lerpFactor = range > 0.0f ? (currentAnim - lastAnim) / range : 1.0f;
            for (int i = 0; i < jointCount; i++)
            {
                jointOffsets[i] = Vector3.Lerp(jointOffsets[i], other.jointOffsets[i], lerpFactor);
            }
        }

        public enum GestureId
        {
            None = 0,
            Flat,
            Open,
            Pinch,
            Poke,
            Grab,
            ThumbsUp,
            Victory,
        }

        static public SimulatedHandPose GetGesturePose(GestureId gesture)
        {
            switch (gesture)
            {
                case GestureId.None: return null;
                case GestureId.Flat: return HandFlat;
                case GestureId.Open: return HandOpened;
                case GestureId.Pinch: return HandPinch;
                case GestureId.Poke: return HandPoke;
                case GestureId.Grab: return HandGrab;
                case GestureId.ThumbsUp: return HandThumbsUp;
                case GestureId.Victory: return HandVictory;
            }
            return null;
        }

        public string GenerateInitializerCode()
        {
            string[] names = Enum.GetNames(typeof(TrackedHandJoint));
            string s = "";
            s += "new Vector3[]\n";
            s += "{\n";
            for (int i = 0; i < jointCount; ++i)
            {
                Vector3 v = jointOffsets[i];
                s += $"    new Vector3({v.x:F5}f, {v.y:F5}f, {v.z:F5}f), // {names[i]}\n";
            }
            s += "}\n";

            return s;
        }

        static private SimulatedHandPose HandOpened = new SimulatedHandPose(new Vector3[]
        {
            // Right palm is duplicate of right thumb metacarpal and right pinky metacarpal
            new Vector3(0.0f,0.0f,0.0f),        // None
            new Vector3(-0.036f,0.165f,0.061f),  // Wrist
            new Vector3(-0.036f,0.10f,0.051f),  // Palm
            new Vector3(-0.020f,0.159f,0.061f), // ThumbMetacarpal
            new Vector3(0.018f,0.126f,0.047f),
            new Vector3(0.044f,0.107f,0.041f),
            new Vector3(0.063f,0.097f,0.040f), // ThumbTip
            new Vector3(-0.027f,0.159f,0.061f), // IndexMetacarpal
            new Vector3(-0.009f,0.075f,0.033f), 
            new Vector3(-0.005f,0.036f,0.017f),
            new Vector3(-0.002f,0.015f,0.007f),
            new Vector3(0.000f,0.000f,0.000f), // IndexTip
            new Vector3(-0.035f,0.159f,0.061f), // MiddleMetacarpal
            new Vector3(-0.032f,0.073f,0.032f),
            new Vector3(-0.017f,0.077f,-0.002f),
            new Vector3(-0.017f,0.104f,-0.001f),
            new Vector3(-0.021f,0.119f,0.008f),
            new Vector3(-0.043f,0.159f,0.061f), // RingMetacarpal
            new Vector3(-0.055f,0.078f,0.032f),
            new Vector3(-0.041f,0.080f,0.001f),
            new Vector3(-0.037f,0.106f,0.003f),
            new Vector3(-0.038f,0.121f,0.012f),
            new Vector3(-0.050f,0.159f,0.061f), // PinkyMetacarpal
            new Vector3(-0.074f,0.087f,0.031f), // PinkyProximal
            new Vector3(-0.061f,0.084f,0.006f), // PinkyIntermediate
            new Vector3(-0.054f,0.101f,0.005f), // PinkyDistal
            new Vector3(-0.054f,0.116f,0.013f), // PinkyTip
        });

        static private SimulatedHandPose HandFlat = new SimulatedHandPose(new Vector3[]
        {
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // None
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // Wrist
            new Vector3(-0.04323f, 0.08708f, -0.00732f), // Palm
            new Vector3(-0.04323f, 0.08708f, -0.00732f), // ThumbMetacarpalJoint
            new Vector3(0.00233f, 0.10868f, 0.00007f), // ThumbProximalJoint
            new Vector3(0.02282f, 0.08608f, 0.00143f), // ThumbDistalJoint
            new Vector3(0.03249f, 0.07318f, 0.00075f), // ThumbTip
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // IndexMetacarpal
            new Vector3(-0.01960f, 0.06668f, 0.00068f), // IndexKnuckle
            new Vector3(-0.00923f, 0.02973f, 0.00216f), // IndexMiddleJoint
            new Vector3(-0.00289f, 0.00912f, 0.00130f), // IndexDistalJoint
            new Vector3(0.00075f, -0.00198f, 0.00011f), // IndexTip
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // MiddleMetacarpal
            new Vector3(-0.03941f, 0.06396f, -0.00371f), // MiddleKnuckle
            new Vector3(-0.03483f, 0.02113f, -0.00446f), // MiddleMiddleJoint
            new Vector3(-0.03101f, -0.00381f, -0.00731f), // MiddleDistalJoint
            new Vector3(-0.02906f, -0.01649f, -0.00879f), // MiddleTip
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // RingMetacarpal
            new Vector3(-0.05859f, 0.06795f, -0.01061f), // RingKnuckle
            new Vector3(-0.06179f, 0.02820f, -0.01284f), // RingMiddleJoint
            new Vector3(-0.06302f, 0.00366f, -0.01568f), // RingDistalJoint
            new Vector3(-0.06334f, -0.00900f, -0.01776f), // RingTip
            new Vector3(-0.06300f, 0.15441f, 0.00097f), // PinkyMetacarpal
            new Vector3(-0.07492f, 0.07446f, -0.01953f), // PinkyKnuckle
            new Vector3(-0.08529f, 0.04472f, -0.02228f), // PinkyMiddleJoint
            new Vector3(-0.08974f, 0.02819f, -0.02572f), // PinkyDistalJoint
            new Vector3(-0.09266f, 0.01696f, -0.02819f), // PinkyTip
        });

        static private SimulatedHandPose HandPinch = new SimulatedHandPose(new Vector3[]
        {
            // Right palm is duplicate of right thumb metacarpal and right pinky metacarpal
            new Vector3(0.0f,0.0f,0.0f),        // None
            new Vector3(-0.042f,0.111f,0.060f), // Wrist
            new Vector3(-0.042f,0.051f,0.060f), // Palm
            new Vector3(-0.032f,0.091f,0.060f), // ThumbMetacarpal
            new Vector3(-0.013f,0.052f,0.044f),
            new Vector3(0.002f,0.026f,0.030f),
            new Vector3(0.007f,0.007f,0.017f),  // ThumbTip
            new Vector3(-0.038f,0.091f,0.060f), // IndexMetacarpal
            new Vector3(-0.029f,0.008f,0.050f),
            new Vector3(-0.009f,-0.016f,0.025f),
            new Vector3(-0.002f,-0.011f,0.008f),
            new Vector3(0.000f,0.000f,0.000f),
            new Vector3(-0.042f,0.091f,0.060f), // MiddleMetacarpal
            new Vector3(-0.050f,0.004f,0.046f),
            new Vector3(-0.026f,0.004f,0.014f),
            new Vector3(-0.028f,0.031f,0.014f),
            new Vector3(-0.034f,0.048f,0.020f),
            new Vector3(-0.048f,0.091f,0.060f), // RingMetacarpal
            new Vector3(-0.071f,0.008f,0.041f),
            new Vector3(-0.048f,0.009f,0.012f),
            new Vector3(-0.046f,0.036f,0.014f),
            new Vector3(-0.050f,0.052f,0.022f),
            new Vector3(-0.052f,0.091f,0.060f), // PinkyMetacarpal
            new Vector3(-0.088f,0.014f,0.034f),
            new Vector3(-0.067f,0.012f,0.013f),
            new Vector3(-0.061f,0.031f,0.014f),
            new Vector3(-0.062f,0.046f,0.021f),
        });

        static private SimulatedHandPose HandPoke = new SimulatedHandPose(new Vector3[]
        {
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // None
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // Wrist
            new Vector3(-0.03259f, 0.03268f, 0.02552f), // Palm
            new Vector3(-0.03259f, 0.03268f, 0.02552f), // ThumbMetacarpalJoint
            new Vector3(-0.00118f, 0.05920f, 0.03279f), // ThumbProximalJoint
            new Vector3(0.00171f, 0.05718f, 0.00273f), // ThumbDistalJoint
            new Vector3(-0.00877f, 0.05977f, -0.00905f), // ThumbTip
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // IndexMetacarpal
            new Vector3(-0.00508f, 0.01676f, 0.02067f), // IndexKnuckle
            new Vector3(0.00320f, -0.00908f, -0.00469f), // IndexMiddleJoint
            new Vector3(0.00987f, -0.01695f, -0.02251f), // IndexDistalJoint
            new Vector3(0.01363f, -0.02002f, -0.03255f), // IndexTip
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // MiddleMetacarpal
            new Vector3(-0.02474f, 0.01422f, 0.01270f), // MiddleKnuckle
            new Vector3(-0.00556f, 0.03787f, -0.01700f), // MiddleMiddleJoint
            new Vector3(-0.00648f, 0.06095f, -0.00658f), // MiddleDistalJoint
            new Vector3(-0.01209f, 0.06180f, 0.00499f), // MiddleTip
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // RingMetacarpal
            new Vector3(-0.04422f, 0.01955f, 0.00797f), // RingKnuckle
            new Vector3(-0.02535f, 0.04385f, -0.01667f), // RingMiddleJoint
            new Vector3(-0.02465f, 0.06521f, -0.00440f), // RingDistalJoint
            new Vector3(-0.02953f, 0.06711f, 0.00726f), // RingTip
            new Vector3(-0.06209f, 0.07595f, 0.07231f), // PinkyMetacarpal
            new Vector3(-0.06218f, 0.02740f, 0.00434f), // PinkyKnuckle
            new Vector3(-0.04335f, 0.04456f, -0.01437f), // PinkyMiddleJoint
            new Vector3(-0.03864f, 0.06018f, -0.00822f), // PinkyDistalJoint
            new Vector3(-0.04340f, 0.06202f, 0.00248f), // PinkyTip
        });

        static private SimulatedHandPose HandGrab = new SimulatedHandPose(new Vector3[]
        {
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // None
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // Wrist
            new Vector3(-0.02248f, -0.03254f, 0.01987f), // Palm
            new Vector3(-0.02248f, -0.03254f, 0.01987f), // ThumbMetacarpalJoint
            new Vector3(0.01379f, -0.00633f, 0.02738f), // ThumbProximalJoint
            new Vector3(0.02485f, -0.02278f, 0.00441f), // ThumbDistalJoint
            new Vector3(0.01847f, -0.02790f, -0.00937f), // ThumbTip
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // IndexMetacarpal
            new Vector3(0.00565f, -0.04883f, 0.01896f), // IndexKnuckle
            new Vector3(0.01153f, -0.02915f, -0.01358f), // IndexMiddleJoint
            new Vector3(0.00547f, -0.00864f, -0.01074f), // IndexDistalJoint
            new Vector3(0.00098f, -0.00265f, -0.00172f), // IndexTip
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // MiddleMetacarpal
            new Vector3(-0.01343f, -0.05324f, 0.01296f), // MiddleKnuckle
            new Vector3(-0.00060f, -0.03063f, -0.02099f), // MiddleMiddleJoint
            new Vector3(-0.00567f, -0.00696f, -0.01334f), // MiddleDistalJoint
            new Vector3(-0.01080f, -0.00381f, -0.00193f), // MiddleTip
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // RingMetacarpal
            new Vector3(-0.03363f, -0.05134f, 0.00849f), // RingKnuckle
            new Vector3(-0.02000f, -0.02888f, -0.02123f), // RingMiddleJoint
            new Vector3(-0.02304f, -0.00675f, -0.01062f), // RingDistalJoint
            new Vector3(-0.02754f, -0.00310f, 0.00082f), // RingTip
            new Vector3(-0.05340f, 0.02059f, 0.05460f), // PinkyMetacarpal
            new Vector3(-0.05225f, -0.04629f, 0.00384f), // PinkyKnuckle
            new Vector3(-0.03698f, -0.03051f, -0.01900f), // PinkyMiddleJoint
            new Vector3(-0.03617f, -0.01485f, -0.01130f), // PinkyDistalJoint
            new Vector3(-0.03902f, -0.00873f, -0.00159f), // PinkyTip
        });

        static private SimulatedHandPose HandThumbsUp = new SimulatedHandPose(new Vector3[]
        {
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // None
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // Wrist
            new Vector3(0.00054f, 0.01716f, -0.03472f), // Palm
            new Vector3(0.00054f, 0.01716f, -0.03472f), // ThumbMetacarpalJoint
            new Vector3(-0.03106f, -0.02225f, -0.00120f), // ThumbProximalJoint
            new Vector3(-0.02123f, -0.04992f, 0.00199f), // ThumbDistalJoint
            new Vector3(-0.01793f, -0.06510f, 0.00358f), // ThumbTip
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // IndexMetacarpal
            new Vector3(0.01284f, -0.01185f, -0.03795f), // IndexKnuckle
            new Vector3(0.03290f, -0.00608f, -0.00720f), // IndexMiddleJoint
            new Vector3(0.01647f, 0.00258f, 0.00238f), // IndexDistalJoint
            new Vector3(0.00559f, 0.00502f, 0.00017f), // IndexTip
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // MiddleMetacarpal
            new Vector3(0.01777f, 0.00604f, -0.04572f), // MiddleKnuckle
            new Vector3(0.03731f, 0.00686f, -0.00898f), // MiddleMiddleJoint
            new Vector3(0.01713f, 0.01536f, 0.00219f), // MiddleDistalJoint
            new Vector3(0.00542f, 0.01846f, -0.00091f), // MiddleTip
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // RingMetacarpal
            new Vector3(0.01836f, 0.02624f, -0.04858f), // RingKnuckle
            new Vector3(0.03987f, 0.02388f, -0.01663f), // RingMiddleJoint
            new Vector3(0.02182f, 0.02969f, -0.00200f), // RingDistalJoint
            new Vector3(0.00978f, 0.03257f, -0.00310f), // RingTip
            new Vector3(-0.05754f, 0.04969f, -0.01566f), // PinkyMetacarpal
            new Vector3(0.01784f, 0.04561f, -0.04763f), // PinkyKnuckle
            new Vector3(0.03771f, 0.03986f, -0.02511f), // PinkyMiddleJoint
            new Vector3(0.02634f, 0.04067f, -0.01262f), // PinkyDistalJoint
            new Vector3(0.01505f, 0.04267f, -0.01241f), // PinkyTip
        });

        static private SimulatedHandPose HandVictory = new SimulatedHandPose(new Vector3[]
        {
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // None
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // Wrist
            new Vector3(-0.05800f, 0.07626f, 0.00397f), // Palm
            new Vector3(-0.05800f, 0.07626f, 0.00397f), // ThumbMetacarpalJoint
            new Vector3(-0.03418f, 0.10243f, -0.00761f), // ThumbProximalJoint
            new Vector3(-0.03570f, 0.08857f, -0.03347f), // ThumbDistalJoint
            new Vector3(-0.04603f, 0.08584f, -0.04472f), // ThumbTip
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // IndexMetacarpal
            new Vector3(-0.03200f, 0.05950f, 0.00836f), // IndexKnuckle
            new Vector3(-0.01535f, 0.02702f, 0.00475f), // IndexMiddleJoint
            new Vector3(-0.00577f, 0.00904f, 0.00218f), // IndexDistalJoint
            new Vector3(-0.00046f, -0.00064f, 0.00053f), // IndexTip
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // MiddleMetacarpal
            new Vector3(-0.05017f, 0.05484f, 0.00171f), // MiddleKnuckle
            new Vector3(-0.04307f, 0.01754f, -0.01398f), // MiddleMiddleJoint
            new Vector3(-0.03889f, -0.00439f, -0.02321f), // MiddleDistalJoint
            new Vector3(-0.03677f, -0.01553f, -0.02789f), // MiddleTip
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // RingMetacarpal
            new Vector3(-0.06885f, 0.05706f, -0.00568f), // RingKnuckle
            new Vector3(-0.05015f, 0.04311f, -0.03622f), // RingMiddleJoint
            new Vector3(-0.04307f, 0.06351f, -0.04646f), // RingDistalJoint
            new Vector3(-0.04840f, 0.07039f, -0.03760f), // RingTip
            new Vector3(-0.08835f, 0.13334f, 0.02745f), // PinkyMetacarpal
            new Vector3(-0.08544f, 0.06253f, -0.01371f), // PinkyKnuckle
            new Vector3(-0.06657f, 0.07318f, -0.03519f), // PinkyMiddleJoint
            new Vector3(-0.06393f, 0.08967f, -0.03293f), // PinkyDistalJoint
            new Vector3(-0.06748f, 0.09752f, -0.02542f), // PinkyTip
        });
    }

}