// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Experimental.RiggedHandVisualizer
{
    /// <summary>
    /// Hand visualizer that controls a hierarchy of transforms to be used by a SkinnedMeshRenderer
    /// Implementation is derived from LeapMotion RiggedHand and RiggedFinger and has visual parity
    /// </summary>
    public class RiggedHandVisualizer : MonoBehaviour, IMixedRealityControllerVisualizer, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
    {
        public virtual Handedness Handedness { get; set; }

        public GameObject GameObjectProxy => gameObject;

        public IMixedRealityController Controller { get; set; }

        public Transform Wrist;
        public Transform Palm;
        
        // User configurable metacarpal transforms for each finger
        public Transform ThumbMetacarpal;
        public Transform IndexMetacarpal;
        public Transform MiddleMetacarpal;
        public Transform RingMetacarpal;
        public Transform PinkyMetacarpal;

        [Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this if your model's palm transform is at the center of the palm similar to Leap API hands.")]
        public bool modelPalmAtLeapWrist = true;

        [Tooltip("Allows the mesh to be stretched to align with finger joint positions. Only set to true when mesh is not visible.")]
        public bool deformPosition = true;

        [Tooltip("Because bones only exist at their roots in model rigs, the length " +
          "of the last fingertip bone is lost when placing bones at positions in the " +
          "tracked hand. " +
          "This option scales the last bone along its X axis (length axis) to match " +
          "its bone length to the tracked bone length. This option only has an " +
          "effect if Deform Positions In Fingers is enabled.")]
        public bool scaleLastFingerBone = true;

        [Tooltip("If non-zero, this vector and the modelPalmFacing vector " +
        "will be used to re-orient the Transform bones in the hand rig, to " +
        "compensate for bone axis discrepancies between Leap Bones and model " +
        "bones.")]
        public Vector3 modelFingerPointing = new Vector3(0, 0, 0);

        [Tooltip("If non-zero, this vector and the modelFingerPointing vector " +
          "will be used to re-orient the Transform bones in the hand rig, to " +
          "compensate for bone axis discrepancies between Leap Bones and model " +
          "bones.")]
        public Vector3 modelPalmFacing = new Vector3(0, 0, 0);

        private Dictionary<TrackedHandJoint, float> FingerTipLengths = new Dictionary<TrackedHandJoint, float>();

        /// <summary> Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector. </summary>
        public Quaternion userBoneRotation
        {
            get
            {
                if (modelFingerPointing == Vector3.zero || modelPalmFacing == Vector3.zero)
                {
                    return Quaternion.identity;
                }
                return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
            }
        }

        public Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
        }

        private Dictionary<TrackedHandJoint, Transform> joints = new Dictionary<TrackedHandJoint, Transform>();

        private void Start()
        {
            // Initialize dictionary with joint transforms
            joints[TrackedHandJoint.Wrist] = Wrist;
            joints[TrackedHandJoint.Palm] = Palm;

            if (ThumbMetacarpal)
            {
                joints[TrackedHandJoint.ThumbMetacarpalJoint] = ThumbMetacarpal;
                joints[TrackedHandJoint.ThumbProximalJoint] = ThumbMetacarpal.GetChild(0);
                joints[TrackedHandJoint.ThumbDistalJoint] = ThumbMetacarpal.GetChild(0).GetChild(0);
                joints[TrackedHandJoint.ThumbTip] = ThumbMetacarpal.GetChild(0).GetChild(0).GetChild(0);
            }
            if (IndexMetacarpal)
            {
                joints[TrackedHandJoint.IndexMetacarpal] = IndexMetacarpal;
                joints[TrackedHandJoint.IndexKnuckle] = IndexMetacarpal.GetChild(0);
                joints[TrackedHandJoint.IndexMiddleJoint] = IndexMetacarpal.GetChild(0).GetChild(0);
                joints[TrackedHandJoint.IndexDistalJoint] = IndexMetacarpal.GetChild(0).GetChild(0).GetChild(0);
                joints[TrackedHandJoint.IndexTip] = IndexMetacarpal.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            }
            if (MiddleMetacarpal)
            {
                joints[TrackedHandJoint.MiddleMetacarpal] = MiddleMetacarpal;
                joints[TrackedHandJoint.MiddleKnuckle] = MiddleMetacarpal.GetChild(0);
                joints[TrackedHandJoint.MiddleMiddleJoint] = MiddleMetacarpal.GetChild(0).GetChild(0);
                joints[TrackedHandJoint.MiddleDistalJoint] = MiddleMetacarpal.GetChild(0).GetChild(0).GetChild(0);
                joints[TrackedHandJoint.MiddleTip] = MiddleMetacarpal.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            }
            if (RingMetacarpal)
            {
                joints[TrackedHandJoint.RingMetacarpal] = RingMetacarpal;
                joints[TrackedHandJoint.RingKnuckle] = RingMetacarpal.GetChild(0);
                joints[TrackedHandJoint.RingMiddleJoint] = RingMetacarpal.GetChild(0).GetChild(0);
                joints[TrackedHandJoint.RingDistalJoint] = RingMetacarpal.GetChild(0).GetChild(0).GetChild(0);
                joints[TrackedHandJoint.RingTip] = RingMetacarpal.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            }
            if (PinkyMetacarpal)
            {
                joints[TrackedHandJoint.PinkyMetacarpal] = PinkyMetacarpal;
                joints[TrackedHandJoint.PinkyKnuckle] = PinkyMetacarpal.GetChild(0);
                joints[TrackedHandJoint.PinkyMiddleJoint] = PinkyMetacarpal.GetChild(0).GetChild(0);
                joints[TrackedHandJoint.PinkyDistalJoint] = PinkyMetacarpal.GetChild(0).GetChild(0).GetChild(0);
                joints[TrackedHandJoint.PinkyTip] = PinkyMetacarpal.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            }

            // Precalculated values for LeapMotion testhand
            FingerTipLengths[TrackedHandJoint.ThumbTip] = 0.02167f;
            FingerTipLengths[TrackedHandJoint.IndexTip] = 0.01582f;
            FingerTipLengths[TrackedHandJoint.MiddleTip] = 0.0174f;
            FingerTipLengths[TrackedHandJoint.RingTip] = 0.0173f;
            FingerTipLengths[TrackedHandJoint.PinkyTip] = 0.01596f;
        }

        private void OnEnable()
        {
            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
        }

        private void OnDisable()
        {
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (Controller?.InputSource.SourceId == eventData.SourceId)
            {
                Destroy(gameObject);
            }
        }

        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            var inputSystem = CoreServices.InputSystem;

            if (eventData.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }
            Debug.Assert(eventData.Handedness == Controller.ControllerHandedness);

            Transform jointTransform;
            foreach (TrackedHandJoint handJoint in eventData.InputData.Keys)
            {
                if (joints.TryGetValue(handJoint, out jointTransform))
                {
                    if (jointTransform != null)
                    {
                        if (handJoint == TrackedHandJoint.Palm)
                        {
                            if (modelPalmAtLeapWrist)
                            {
                                Palm.position = eventData.InputData[TrackedHandJoint.Wrist].Position;
                            }
                            else
                            {
                                Palm.position = eventData.InputData[TrackedHandJoint.Palm].Position;
                            }
                            Palm.rotation = eventData.InputData[TrackedHandJoint.Palm].Rotation * userBoneRotation;
                        }
                        else if (handJoint == TrackedHandJoint.Wrist)
                        {
                            if (!modelPalmAtLeapWrist)
                            {
                                Wrist.position = eventData.InputData[TrackedHandJoint.Wrist].Position;
                            }
                        }
                        else
                        {
                            // Finger joints
                            jointTransform.rotation = eventData.InputData[handJoint].Rotation * Reorientation();

                            if (deformPosition)
                            {
                                jointTransform.position = eventData.InputData[handJoint].Position;

                                if (scaleLastFingerBone &&
                                    (handJoint == TrackedHandJoint.ThumbDistalJoint ||
                                    handJoint == TrackedHandJoint.IndexDistalJoint ||
                                    handJoint == TrackedHandJoint.MiddleDistalJoint ||
                                    handJoint == TrackedHandJoint.RingDistalJoint ||
                                    handJoint == TrackedHandJoint.PinkyDistalJoint))
                                {
                                    ScaleFingerTip(eventData, jointTransform, handJoint + 1, jointTransform.position);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ScaleFingerTip(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData, Transform jointTransform, TrackedHandJoint fingerTipJoint, Vector3 boneRootPos)
        {
            // Set fingertip base bone scale to match the bone length to the fingertip.
            // This will only scale correctly if the model was constructed to match
            // the standard "test" edit-time hand model from the LeapMotion TestHandFactory.
            var boneTipPos = eventData.InputData[fingerTipJoint].Position;
            var boneVec = boneTipPos - boneRootPos;

            if (transform.lossyScale.x != 0f && transform.lossyScale.x != 1f)
            {
                boneVec /= transform.lossyScale.x;
            }
            var newScale = jointTransform.transform.localScale;
            var lengthComponentIdx = getLargestComponentIndex(modelFingerPointing);
            newScale[lengthComponentIdx] = boneVec.magnitude / FingerTipLengths[fingerTipJoint];
            jointTransform.transform.localScale = newScale;
        }

        private int getLargestComponentIndex(Vector3 pointingVector)
        {
            var largestValue = 0f;
            var largestIdx = 0;
            for (int i = 0; i < 3; i++)
            {
                var testValue = pointingVector[i];
                if (Mathf.Abs(testValue) > largestValue)
                {
                    largestIdx = i;
                    largestValue = Mathf.Abs(testValue);
                }
            }
            return largestIdx;
        }
    }
}
