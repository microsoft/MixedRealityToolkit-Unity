// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.RiggedHandVisualizer
{
    /// <summary>
    /// Hand visualizer that controls a hierarchy of transforms to be used by a SkinnedMeshRenderer
    /// Implementation is derived from LeapMotion RiggedHand and RiggedFinger and has visual parity
    /// </summary>
    public class RiggedHandVisualizer : MonoBehaviour, IMixedRealityControllerVisualizer, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
    {
        /// <inheritdoc/>
        public GameObject GameObjectProxy => gameObject;

        /// <inheritdoc/>
        public IMixedRealityController Controller { get; set; }

        /// <summary>
        /// Wrist Transform
        /// </summary>
        public Transform Wrist;
        /// <summary>
        /// Palm transform
        /// </summary>
        public Transform Palm;
        /// <summary>
        /// Thumb metacarpal transform  (thumb root)
        /// </summary>
        public Transform ThumbRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool ThumbRootIsMetacarpal = true;

        /// <summary>
        /// Index metacarpal transform (index finger root)
        /// </summary>
        public Transform IndexRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool IndexRootIsMetacarpal = true;

        /// <summary>
        /// Middle metacarpal transform (middle finger root)
        /// </summary>
        public Transform MiddleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool MiddleRootIsMetacarpal = true;

        /// <summary>
        /// Ring metacarpal transform (ring finger root)
        /// </summary>
        public Transform RingRoot;

        [Tooltip("Ring finger node is metacarpal joint.")]
        public bool RingRootIsMetacarpal = true;

        /// <summary>
        /// Pinky metacarpal transform (pinky finger root)
        /// </summary>
        public Transform PinkyRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        public bool PinkyRootIsMetacarpal = true;

        [Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this if your model's palm transform is at the center of the palm similar to Leap API hands.")]
        public bool ModelPalmAtLeapWrist = true;

        [Tooltip("Allows the mesh to be stretched to align with finger joint positions. Only set to true when mesh is not visible.")]
        public bool DeformPosition = true;

        [Tooltip("Because bones only exist at their roots in model rigs, the length " +
          "of the last fingertip bone is lost when placing bones at positions in the " +
          "tracked hand. " +
          "This option scales the last bone along its X axis (length axis) to match " +
          "its bone length to the tracked bone length. This option only has an " +
          "effect if Deform Positions In Fingers is enabled.")]
        public bool ScaleLastFingerBone = true;

        [Tooltip("If non-zero, this vector and the modelPalmFacing vector " +
        "will be used to re-orient the Transform bones in the hand rig, to " +
        "compensate for bone axis discrepancies between Leap Bones and model " +
        "bones.")]
        public Vector3 ModelFingerPointing = new Vector3(0, 0, 0);

        [Tooltip("If non-zero, this vector and the modelFingerPointing vector " +
          "will be used to re-orient the Transform bones in the hand rig, to " +
          "compensate for bone axis discrepancies between Leap Bones and model " +
          "bones.")]
        public Vector3 ModelPalmFacing = new Vector3(0, 0, 0);

        /// <summary>
        /// Precalculated values for LeapMotion testhand fingertip lengths
        /// </summary>
        private const float thumbFingerTipLength = 0.02167f;
        private const float indexingerTipLength = 0.01582f;
        private const float middleFingerTipLength = 0.0174f;
        private const float ringFingerTipLength = 0.0173f;
        private const float pinkyFingerTipLength = 0.01596f;

        /// <summary>
        /// Precalculated fingertip lengths used for scaling the fingertips of the skinnedmesh
        /// to match with tracked hand fingertip size 
        /// </summary>
        private Dictionary<TrackedHandJoint, float> fingerTipLengths = new Dictionary<TrackedHandJoint, float>()
        {
            {TrackedHandJoint.ThumbTip, thumbFingerTipLength },
            {TrackedHandJoint.IndexTip, indexingerTipLength },
            {TrackedHandJoint.MiddleTip, middleFingerTipLength },
            {TrackedHandJoint.RingTip, ringFingerTipLength },
            {TrackedHandJoint.PinkyTip, pinkyFingerTipLength }
        };       

        /// <summary>
        /// Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector.
        /// </summary>
        private Quaternion userBoneRotation
        {
            get
            {
                if (ModelFingerPointing == Vector3.zero || ModelPalmFacing == Vector3.zero)
                {
                    return Quaternion.identity;
                }
                return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
            }
        }

        private Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
        }

        private Dictionary<TrackedHandJoint, Transform> joints = new Dictionary<TrackedHandJoint, Transform>();

        private void Start()
        {
            // Initialize joint dictionary with their corresponding joint transforms
            joints[TrackedHandJoint.Wrist] = Wrist;
            joints[TrackedHandJoint.Palm] = Palm;

            // Thumb joints, first node is user assigned, note that there are only 4 joints in the thumb
            if (ThumbRoot)
            {
                if (ThumbRootIsMetacarpal)
                {
                    joints[TrackedHandJoint.ThumbMetacarpalJoint] = ThumbRoot;
                    joints[TrackedHandJoint.ThumbProximalJoint] = RetrieveChild(TrackedHandJoint.ThumbMetacarpalJoint);
                }
                else
                {
                    joints[TrackedHandJoint.ThumbProximalJoint] = ThumbRoot;
                }                
                joints[TrackedHandJoint.ThumbDistalJoint] = RetrieveChild(TrackedHandJoint.ThumbProximalJoint);
                joints[TrackedHandJoint.ThumbTip] = RetrieveChild(TrackedHandJoint.ThumbDistalJoint);
            }
            // Look up index finger joints below the index finger root joint
            if (IndexRoot)
            {
                if (IndexRootIsMetacarpal)
                {
                    joints[TrackedHandJoint.IndexMetacarpal] = IndexRoot;
                    joints[TrackedHandJoint.IndexKnuckle] = RetrieveChild(TrackedHandJoint.IndexMetacarpal);
                }
                else
                {
                    joints[TrackedHandJoint.IndexKnuckle] = IndexRoot;
                }
                joints[TrackedHandJoint.IndexMiddleJoint] = RetrieveChild(TrackedHandJoint.IndexKnuckle);
                joints[TrackedHandJoint.IndexDistalJoint] = RetrieveChild(TrackedHandJoint.IndexMiddleJoint);
                joints[TrackedHandJoint.IndexTip] = RetrieveChild(TrackedHandJoint.IndexDistalJoint);
            }

            // Look up middle finger joints below the middle finger root joint
            if (MiddleRoot)
            {
                if (MiddleRootIsMetacarpal)
                {
                    joints[TrackedHandJoint.MiddleMetacarpal] = MiddleRoot;
                    joints[TrackedHandJoint.MiddleKnuckle] = RetrieveChild(TrackedHandJoint.MiddleMetacarpal);
                }
                else
                {
                    joints[TrackedHandJoint.MiddleKnuckle] = MiddleRoot;
                }
                joints[TrackedHandJoint.MiddleMiddleJoint] = RetrieveChild(TrackedHandJoint.MiddleKnuckle);
                joints[TrackedHandJoint.MiddleDistalJoint] = RetrieveChild(TrackedHandJoint.MiddleMiddleJoint);
                joints[TrackedHandJoint.MiddleTip] = RetrieveChild(TrackedHandJoint.MiddleDistalJoint);
            }

            // Look up ring finger joints below the ring finger root joint
            if (RingRoot)
            {
                if (RingRootIsMetacarpal)
                {
                    joints[TrackedHandJoint.RingMetacarpal] = RingRoot;
                    joints[TrackedHandJoint.RingKnuckle] = RetrieveChild(TrackedHandJoint.RingMetacarpal);
                }
                else
                {
                    joints[TrackedHandJoint.RingKnuckle] = RingRoot;
                }
                joints[TrackedHandJoint.RingMiddleJoint] = RetrieveChild(TrackedHandJoint.RingKnuckle);
                joints[TrackedHandJoint.RingDistalJoint] = RetrieveChild(TrackedHandJoint.RingMiddleJoint);
                joints[TrackedHandJoint.RingTip] = RetrieveChild(TrackedHandJoint.RingDistalJoint);
            }

            // Look up pinky joints below the pinky root joint
            if (PinkyRoot)
            {
                if (PinkyRootIsMetacarpal)
                {
                    joints[TrackedHandJoint.PinkyMetacarpal] = PinkyRoot;
                    joints[TrackedHandJoint.PinkyKnuckle] = RetrieveChild(TrackedHandJoint.PinkyMetacarpal);
                }
                else
                {
                    joints[TrackedHandJoint.PinkyKnuckle] = PinkyRoot;
                }
                joints[TrackedHandJoint.PinkyMiddleJoint] = RetrieveChild(TrackedHandJoint.PinkyKnuckle);
                joints[TrackedHandJoint.PinkyDistalJoint] = RetrieveChild(TrackedHandJoint.PinkyMiddleJoint);
                joints[TrackedHandJoint.PinkyTip] = RetrieveChild(TrackedHandJoint.PinkyDistalJoint);
            }
        }

        private Transform RetrieveChild(TrackedHandJoint parentJoint)
        {
            if (joints[parentJoint] != null && joints[parentJoint].childCount > 0)
            {
                return joints[parentJoint].GetChild(0);
            }
            return null;
        }

        private float Distance(TrackedHandJoint joint1, TrackedHandJoint joint2)
        {
            if (joints[joint1] != null && joints[joint2] != null)
            {
                return Vector3.Distance(joints[joint1].position, joints[joint2].position);
            }
            return 0;
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
        
        /// <inheritdoc/>
        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }
        
        /// <inheritdoc/>
        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (Controller?.InputSource.SourceId == eventData.SourceId)
            {
                Destroy(gameObject);
            }
        }

        /// <inheritdoc/>
        void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            var inputSystem = CoreServices.InputSystem;

            if (eventData.InputSource.SourceId != Controller.InputSource.SourceId)
            {
                return;
            }
            Debug.Assert(eventData.Handedness == Controller.ControllerHandedness);

            Transform jointTransform;
            // Apply updated TrackedHandJoint pose data to the assigned transforms
            foreach (TrackedHandJoint handJoint in eventData.InputData.Keys)
            {
                if (joints.TryGetValue(handJoint, out jointTransform))
                {
                    if (jointTransform != null)
                    {
                        if (handJoint == TrackedHandJoint.Palm)
                        {
                            if (ModelPalmAtLeapWrist)
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
                            if (!ModelPalmAtLeapWrist)
                            {
                                Wrist.position = eventData.InputData[TrackedHandJoint.Wrist].Position;
                                Wrist.position = eventData.InputData[TrackedHandJoint.Wrist].Position;
                            }
                        }
                        else
                        {
                            // Finger joints
                            jointTransform.rotation = eventData.InputData[handJoint].Rotation * Reorientation();

                            if (DeformPosition)
                            {
                                jointTransform.position = eventData.InputData[handJoint].Position;

                                if (ScaleLastFingerBone &&
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
            var lengthComponentIdx = GetLargestComponentIndex(ModelFingerPointing);
            newScale[lengthComponentIdx] = boneVec.magnitude / fingerTipLengths[fingerTipJoint];
            jointTransform.transform.localScale = newScale;
        }

        private int GetLargestComponentIndex(Vector3 pointingVector)
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
