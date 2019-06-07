// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Utility class to give input animation clips a valid target.
    /// </summary>
    /// <remarks>
    /// InputAnimationTarget serves as a binding type for AnimationClip,
    /// with concrete serializable fields for each of the input properties.
    /// The serializable fields are converted into actual input after AnimationClip has been evaluated.
    /// </remarks>
    [Serializable]
    public class InputAnimationTarget : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        protected InputAnimationHandTarget leftHand = new InputAnimationHandTarget();
        [SerializeField]
        [HideInInspector]
        protected InputAnimationHandTarget rightHand = new InputAnimationHandTarget();

        [SerializeField]
        [HideInInspector]
        protected MixedRealityPose cameraPose = MixedRealityPose.ZeroIdentity;

        private IInputSimulationService InputSimService => inputSimService ?? (inputSimService = MixedRealityToolkit.Instance.GetService<IInputSimulationService>());
        private IInputSimulationService inputSimService = null;

        void Update()
        {
            CameraCache.Main.transform.SetPositionAndRotation(cameraPose.Position, cameraPose.Rotation);

            EvaluateHandData(InputSimService.HandDataLeft, Handedness.Left);
            EvaluateHandData(InputSimService.HandDataRight, Handedness.Right);
        }

        private void EvaluateHandData(SimulatedHandData handData, Handedness handedness)
        {
            InputAnimationHandTarget target = (handedness == Handedness.Left ? leftHand : rightHand);

            InputSimService.HandDataLeft.Update(target.isTracked, target.isPinching,
                (MixedRealityPose[] joints) =>
                {
                    foreach (var pair in target)
                    {
                        pair.pose.Rotation.Normalize();
                        joints[(int)pair.joint] = pair.pose;
                    }
                });
        }
    }

    /// <summary>
    /// Animation target for a single hand.
    /// </summary>
    /// <remarks>
    /// Unity animation does not support arrays, so each joint pose has to be stored as a separate field.
    /// For ease of use this class is iterable and will yield all the joint poses.
    /// </remarks>
    [Serializable]
    public struct InputAnimationHandTarget : IEnumerable<InputAnimationJointPosePair>
    {
        private static int jointCount = Enum.GetNames(typeof(TrackedHandJoint)).Length;

        public bool isTracked;
        public bool isPinching;

        public MixedRealityPose jointNone;
        public MixedRealityPose jointWrist;
        public MixedRealityPose jointPalm;
        public MixedRealityPose jointThumbMetacarpalJoint;
        public MixedRealityPose jointThumbProximalJoint;
        public MixedRealityPose jointThumbDistalJoint;
        public MixedRealityPose jointThumbTip;
        public MixedRealityPose jointIndexMetacarpal;
        public MixedRealityPose jointIndexKnuckle;
        public MixedRealityPose jointIndexMiddleJoint;
        public MixedRealityPose jointIndexDistalJoint;
        public MixedRealityPose jointIndexTip;
        public MixedRealityPose jointMiddleMetacarpal;
        public MixedRealityPose jointMiddleKnuckle;
        public MixedRealityPose jointMiddleMiddleJoint;
        public MixedRealityPose jointMiddleDistalJoint;
        public MixedRealityPose jointMiddleTip;
        public MixedRealityPose jointRingMetacarpal;
        public MixedRealityPose jointRingKnuckle;
        public MixedRealityPose jointRingMiddleJoint;
        public MixedRealityPose jointRingDistalJoint;
        public MixedRealityPose jointRingTip;
        public MixedRealityPose jointPinkyMetacarpal;
        public MixedRealityPose jointPinkyKnuckle;
        public MixedRealityPose jointPinkyMiddleJoint;
        public MixedRealityPose jointPinkyDistalJoint;
        public MixedRealityPose jointPinkyTip;

        public IEnumerator<InputAnimationJointPosePair> GetEnumerator()
        {
            yield return new InputAnimationJointPosePair(TrackedHandJoint.None,                 jointNone);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.Wrist,                jointWrist);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.Palm,                 jointPalm);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.ThumbMetacarpalJoint, jointThumbMetacarpalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.ThumbProximalJoint,   jointThumbProximalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.ThumbDistalJoint,     jointThumbDistalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.ThumbTip,             jointThumbTip);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.IndexMetacarpal,      jointIndexMetacarpal);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.IndexKnuckle,         jointIndexKnuckle);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.IndexMiddleJoint,     jointIndexMiddleJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.IndexDistalJoint,     jointIndexDistalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.IndexTip,             jointIndexTip);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.MiddleMetacarpal,     jointMiddleMetacarpal);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.MiddleKnuckle,        jointMiddleKnuckle);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.MiddleMiddleJoint,    jointMiddleMiddleJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.MiddleDistalJoint,    jointMiddleDistalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.MiddleTip,            jointMiddleTip);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.RingMetacarpal,       jointRingMetacarpal);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.RingKnuckle,          jointRingKnuckle);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.RingMiddleJoint,      jointRingMiddleJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.RingDistalJoint,      jointRingDistalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.RingTip,              jointRingTip);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.PinkyMetacarpal,      jointPinkyMetacarpal);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.PinkyKnuckle,         jointPinkyKnuckle);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.PinkyMiddleJoint,     jointPinkyMiddleJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.PinkyDistalJoint,     jointPinkyDistalJoint);
            yield return new InputAnimationJointPosePair(TrackedHandJoint.PinkyTip,             jointPinkyTip);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// Utility struct for iterating over all joint poses in InputAnimationTarget.
    /// </summary>
    public struct InputAnimationJointPosePair
    {
        public TrackedHandJoint joint;
        public MixedRealityPose pose;

        public InputAnimationJointPosePair(TrackedHandJoint joint, MixedRealityPose pose)
        {
            this.joint = joint;
            this.pose = pose;
        }
    }
}