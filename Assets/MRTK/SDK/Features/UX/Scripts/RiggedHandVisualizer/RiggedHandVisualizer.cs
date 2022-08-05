// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Hand visualizer that controls a hierarchy of transforms to be used by a SkinnedMeshRenderer
    /// Implementation is derived from LeapMotion RiggedHand and RiggedFinger and has visual parity
    /// </summary>
    public class RiggedHandVisualizer : BaseHandVisualizer
    {
        // This bool is used to track whether or not we are receiving hand mesh data from the platform itself
        // If we aren't we will use our own rigged hand visualizer to render the hand mesh
        private bool ReceivingPlatformHandMesh => handMeshFilter != null;

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

        [Tooltip("Allows the mesh to be stretched to align with finger joint positions.")]
        public bool DeformPosition = true;

        [Tooltip("Because bones only exist at their roots in model rigs, the length " +
          "of the last fingertip bone is lost when placing bones at positions in the " +
          "tracked hand. " +
          "This option scales the last bone along its X axis (length axis) to match " +
          "its bone length to the tracked bone length.")]
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

        [SerializeField]
        [Tooltip("Renderer of the hand mesh")]
        private SkinnedMeshRenderer handRenderer = null;

        /// <summary>
        /// Renderer of the hand mesh.
        /// </summary>
        public SkinnedMeshRenderer HandRenderer => handRenderer;

        /// <summary>
        /// Caching the hand material from CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile.RiggedHandMeshMaterial
        /// </summary>
        private Material handMaterial = null;

        /// <summary>
        /// Hand material to use for hand tracking hand mesh.
        /// </summary>
        [Obsolete("Use the CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile.RiggedHandMeshMaterial instead")]
        public Material HandMaterial => handMaterial;

        /// <summary>
        /// Property name for modifying the mesh's appearance based on pinch strength
        /// </summary>
        private const string pinchStrengthMaterialProperty = "_PressIntensity";

        /// <summary>
        /// Property name for modifying the mesh's appearance based on pinch strength
        /// </summary>
        public string PinchStrengthMaterialProperty => pinchStrengthMaterialProperty;

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
        private Quaternion UserBoneRotation
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

        protected readonly Transform[] riggedVisualJointsArray = new Transform[ArticulatedHandPose.JointCount];

        /// <summary>
        /// flag checking that the handRenderer was initialized with its own material
        /// </summary>
        private bool handRendererInitialized = false;

        /// <summary>
        /// Flag to only show a only show a warning once
        /// </summary>
        private bool displayedMaterialPropertyWarning = false;

        protected override void Start()
        {
            base.Start();

            // Ensure hand is not visible until we can update position first time.
            HandRenderer.enabled = false;

            // Initialize joint dictionary with their corresponding joint transforms
            riggedVisualJointsArray[(int)TrackedHandJoint.Wrist] = Wrist;
            riggedVisualJointsArray[(int)TrackedHandJoint.Palm] = Palm;

            // Thumb riggedVisualJointsArray, first node is user assigned, note that there are only 4 riggedVisualJointsArray in the thumb
            if (ThumbRoot)
            {
                if (ThumbRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbMetacarpalJoint] = ThumbRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbProximalJoint] = RetrieveChild(TrackedHandJoint.ThumbMetacarpalJoint);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbProximalJoint] = ThumbRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.ThumbDistalJoint] = RetrieveChild(TrackedHandJoint.ThumbProximalJoint);
                riggedVisualJointsArray[(int)TrackedHandJoint.ThumbTip] = RetrieveChild(TrackedHandJoint.ThumbDistalJoint);
            }
            // Look up index finger riggedVisualJointsArray below the index finger root joint
            if (IndexRoot)
            {
                if (IndexRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexMetacarpal] = IndexRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexKnuckle] = RetrieveChild(TrackedHandJoint.IndexMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexKnuckle] = IndexRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexMiddleJoint] = RetrieveChild(TrackedHandJoint.IndexKnuckle);
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexDistalJoint] = RetrieveChild(TrackedHandJoint.IndexMiddleJoint);
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexTip] = RetrieveChild(TrackedHandJoint.IndexDistalJoint);
            }

            // Look up middle finger riggedVisualJointsArray below the middle finger root joint
            if (MiddleRoot)
            {
                if (MiddleRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleMetacarpal] = MiddleRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleKnuckle] = RetrieveChild(TrackedHandJoint.MiddleMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleKnuckle] = MiddleRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleMiddleJoint] = RetrieveChild(TrackedHandJoint.MiddleKnuckle);
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleDistalJoint] = RetrieveChild(TrackedHandJoint.MiddleMiddleJoint);
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleTip] = RetrieveChild(TrackedHandJoint.MiddleDistalJoint);
            }

            // Look up ring finger riggedVisualJointsArray below the ring finger root joint
            if (RingRoot)
            {
                if (RingRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingMetacarpal] = RingRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingKnuckle] = RetrieveChild(TrackedHandJoint.RingMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingKnuckle] = RingRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.RingMiddleJoint] = RetrieveChild(TrackedHandJoint.RingKnuckle);
                riggedVisualJointsArray[(int)TrackedHandJoint.RingDistalJoint] = RetrieveChild(TrackedHandJoint.RingMiddleJoint);
                riggedVisualJointsArray[(int)TrackedHandJoint.RingTip] = RetrieveChild(TrackedHandJoint.RingDistalJoint);
            }

            // Look up pinky riggedVisualJointsArray below the pinky root joint
            if (PinkyRoot)
            {
                if (PinkyRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.PinkyMetacarpal] = PinkyRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.PinkyKnuckle] = RetrieveChild(TrackedHandJoint.PinkyMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.PinkyKnuckle] = PinkyRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.PinkyMiddleJoint] = RetrieveChild(TrackedHandJoint.PinkyKnuckle);
                riggedVisualJointsArray[(int)TrackedHandJoint.PinkyDistalJoint] = RetrieveChild(TrackedHandJoint.PinkyMiddleJoint);
                riggedVisualJointsArray[(int)TrackedHandJoint.PinkyTip] = RetrieveChild(TrackedHandJoint.PinkyDistalJoint);
            }

            // Give the hand mesh its own material to avoid modifying both hand materials when making property changes
            MixedRealityHandTrackingProfile handTrackingProfile = CoreServices.InputSystem?.InputSystemProfile.HandTrackingProfile;

            handMaterial = handTrackingProfile.RiggedHandMeshMaterial;
            Material handMaterialInstance = new Material(handMaterial);
            handRenderer.sharedMaterial = handMaterialInstance;
            handRendererInitialized = true;
        }

        private Transform RetrieveChild(TrackedHandJoint parentJoint)
        {
            Transform parentJointTransform = riggedVisualJointsArray[(int)parentJoint];
            if (parentJointTransform != null && parentJointTransform.childCount > 0)
            {
                return parentJointTransform.GetChild(0);
            }
            return null;
        }

        private static readonly ProfilerMarker UpdateHandJointsPerfMarker = new ProfilerMarker("[MRTK] RiggedHandVisualizer.UpdateHandJoints");

        protected override bool UpdateHandJoints()
        {
            using (UpdateHandJointsPerfMarker.Auto())
            {
                // The base class takes care of updating all of the joint data
                _ = base.UpdateHandJoints();

                // Exit early and disable the rigged hand model if we've gotten a hand mesh from the underlying platform
                if (ReceivingPlatformHandMesh || MixedRealityHand.IsNull())
                {
                    HandRenderer.enabled = false;
                    return false;
                }

                IMixedRealityInputSystem inputSystem = CoreServices.InputSystem;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystem?.InputSystemProfile != null ? inputSystem.InputSystemProfile.HandTrackingProfile : null;

                // Only runs if render hand mesh is true
                bool renderHandmesh = handTrackingProfile != null && handTrackingProfile.EnableHandMeshVisualization && MixedRealityHand.TryGetJoint(TrackedHandJoint.Palm, out _);
                HandRenderer.enabled = renderHandmesh;
                if (renderHandmesh)
                {
                    // Render the rigged hand mesh itself
                    // Apply updated TrackedHandJoint pose data to the assigned transforms

                    // This starts at 1 to skip over TrackedHandJoint.None.
                    for (int i = 1; i < ArticulatedHandPose.JointCount; i++)
                    {
                        TrackedHandJoint handJoint = (TrackedHandJoint)i;
                        // Skip this hand joint if the event data doesn't have an entry for it
                        if (!MixedRealityHand.TryGetJoint(handJoint, out MixedRealityPose handJointPose))
                        {
                            continue;
                        }

                        Transform jointTransform = riggedVisualJointsArray[i];
                        if (jointTransform != null)
                        {
                            if (handJoint == TrackedHandJoint.Palm)
                            {
                                if (ModelPalmAtLeapWrist && MixedRealityHand.TryGetJoint(TrackedHandJoint.Wrist, out MixedRealityPose wristPose))
                                {
                                    Palm.position = wristPose.Position;
                                }
                                else
                                {
                                    Palm.position = handJointPose.Position;
                                }
                                Palm.rotation = handJointPose.Rotation * UserBoneRotation;
                            }
                            else if (handJoint == TrackedHandJoint.Wrist)
                            {
                                if (!ModelPalmAtLeapWrist)
                                {
                                    Wrist.position = handJointPose.Position;
                                }
                            }
                            else
                            {
                                // Finger riggedVisualJointsArray
                                jointTransform.rotation = handJointPose.Rotation * Reorientation();

                                if (DeformPosition)
                                {
                                    jointTransform.position = handJointPose.Position;
                                }

                                if (ScaleLastFingerBone &&
                                    (handJoint == TrackedHandJoint.ThumbDistalJoint ||
                                    handJoint == TrackedHandJoint.IndexDistalJoint ||
                                    handJoint == TrackedHandJoint.MiddleDistalJoint ||
                                    handJoint == TrackedHandJoint.RingDistalJoint ||
                                    handJoint == TrackedHandJoint.PinkyDistalJoint))
                                {
                                    ScaleFingerTip(jointTransform, handJoint + 1, jointTransform.position);
                                }
                            }
                        }
                    }

                    // Update the hand material
                    float pinchStrength = HandPoseUtils.CalculateIndexPinch(Controller.ControllerHandedness);

                    // Hand Curl Properties: 
                    float indexFingerCurl = HandPoseUtils.IndexFingerCurl(Controller.ControllerHandedness);
                    float middleFingerCurl = HandPoseUtils.MiddleFingerCurl(Controller.ControllerHandedness);
                    float ringFingerCurl = HandPoseUtils.RingFingerCurl(Controller.ControllerHandedness);
                    float pinkyFingerCurl = HandPoseUtils.PinkyFingerCurl(Controller.ControllerHandedness);

                    if (handMaterial != null && handRendererInitialized)
                    {
                        float gripStrength = indexFingerCurl + middleFingerCurl + ringFingerCurl + pinkyFingerCurl;
                        gripStrength /= 4.0f;
                        gripStrength = gripStrength > 0.8f ? 1.0f : gripStrength;

                        pinchStrength = Mathf.Pow(Mathf.Max(pinchStrength, gripStrength), 2.0f);

                        if (handRenderer.sharedMaterial.HasProperty(pinchStrengthMaterialProperty))
                        {
                            handRenderer.sharedMaterial.SetFloat(pinchStrengthMaterialProperty, pinchStrength);
                        }
                        // Only show this warning once
                        else if (!displayedMaterialPropertyWarning)
                        {
                            Debug.LogWarning(String.Format("The property {0} for reacting to pinch strength was not found. A material with this property is required to visualize pinch strength.", pinchStrengthMaterialProperty));
                            displayedMaterialPropertyWarning = true;
                        }
                    }
                }

                return true;
            }
        }

        private Quaternion Reorientation()
        {
            return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
        }

        private void ScaleFingerTip(Transform jointTransform, TrackedHandJoint fingerTipJoint, Vector3 boneRootPos)
        {
            // Set fingertip base bone scale to match the bone length to the fingertip.
            // This will only scale correctly if the model was constructed to match
            // the standard "test" edit-time hand model from the LeapMotion TestHandFactory.
            if (!MixedRealityHand.TryGetJoint(fingerTipJoint, out MixedRealityPose pose))
            {
                return;
            }

            var boneTipPos = pose.Position;
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
