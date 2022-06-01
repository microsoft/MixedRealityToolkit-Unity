// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic hand joint visualizer that draws an instanced mesh on each hand joint.
    /// </summary>
    [AddComponentMenu("MRTK/Input/Hand Visualizer")]
    public class HandVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        [SerializeField]
        [Tooltip("Joint visualization mesh.")]
        private Mesh jointMesh;

        /// <summary> Joint visualization mesh. </summary>
        public Mesh JointMesh { get => jointMesh; set => jointMesh = value; }

        [SerializeField]
        [Tooltip("Joint visualization material.")]
        private Material jointMaterial;

        /// <summary> Joint visualization material. </summary>
        public Material JointMaterial { get => jointMaterial; set => jointMaterial = value; }

        private HandsAggregatorSubsystem handsSubsystem;

        // Transformation matrix for each joint.
        private List<Matrix4x4> jointMatrices = new List<Matrix4x4>();


        #region RiggedHand variables
        // This bool is used to track whether or not we are recieving hand mesh data from the platform itself
        // If we aren't we will use our own rigged hand visualizer to render the hand mesh
        //private bool receivingPlatformHandMesh => handMeshFilter != null;

        /// <summary>
        /// Wrist Transform
        /// </summary>
        [SerializeField]
        private Transform Wrist;
        /// <summary>
        /// Palm transform
        /// </summary>
        [SerializeField]
        private Transform Palm;
        /// <summary>
        /// Thumb metacarpal transform  (thumb root)
        /// </summary>
        [SerializeField]
        private Transform ThumbRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool ThumbRootIsMetacarpal = true;

        /// <summary>
        /// Index metacarpal transform (index finger root)
        /// </summary>
        [SerializeField]
        private Transform IndexRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool IndexRootIsMetacarpal = true;

        /// <summary>
        /// Middle metacarpal transform (middle finger root)
        /// </summary>
        [SerializeField]
        private Transform MiddleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool MiddleRootIsMetacarpal = true;

        /// <summary>
        /// Ring metacarpal transform (ring finger root)
        /// </summary>
        [SerializeField]
        private Transform RingRoot;

        [Tooltip("Ring finger node is metacarpal joint.")]
        [SerializeField]
        private bool RingRootIsMetacarpal = true;

        /// <summary>
        /// Little metacarpal transform (Little finger root)
        /// </summary>
        [SerializeField]
        private Transform LittleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool LittleRootIsMetacarpal = true;

        //[Tooltip("Hands are typically rigged in 3D packages with the palm transform near the wrist. Uncheck this if your model's palm transform is at the center of the palm similar to Leap API hands.")]
        [SerializeField]
        private bool ModelPalmAtLeapWrist = true;

        //[Tooltip("Allows the mesh to be stretched to align with finger joint positions.")]
        [SerializeField]
        private bool DeformPosition = true;

        //[Tooltip("Because bones only exist at their roots in model rigs, the length " +
        //  "of the last fingertip bone is lost when placing bones at positions in the " +
        //  "tracked hand. " +
        //  "This option scales the last bone along its X axis (length axis) to match " +
        //  "its bone length to the tracked bone length.")]
        [SerializeField]
        private bool ScaleLastFingerBone = true;

        [Tooltip("If non-zero, this vector and the modelPalmFacing vector " +
        "will be used to re-orient the Transform bones in the hand rig, to " +
        "compensate for bone axis discrepancies between Leap Bones and model " +
        "bones.")]
        [SerializeField]
        private Vector3 ModelFingerPointing;

        [Tooltip("If non-zero, this vector and the modelFingerPointing vector " +
          "will be used to re-orient the Transform bones in the hand rig, to " +
          "compensate for bone axis discrepancies between Leap Bones and model " +
          "bones.")]
        [SerializeField]
        private Vector3 ModelPalmFacing;

        [SerializeField]
        [Tooltip("Hand material to use for hand tracking hand mesh.")]
        private Material handMaterial = null;

        /// <summary>
        /// The property block which is used to modify the press intensity property on the material
        /// </summary>
        private MaterialPropertyBlock propertyBlock = null;

        /// <summary>
        /// Property name for modifying the mesh's appearance based on pinch strength
        /// </summary>
        private const string pinchStrengthMaterialProperty = "_PressIntensity";

        /// <summary>
        /// Precalculated values for LeapMotion testhand fingertip lengths
        /// </summary>
        private const float thumbFingerTipLength = 0.02167f;
        private const float indexingerTipLength = 0.01582f;
        private const float middleFingerTipLength = 0.0174f;
        private const float ringFingerTipLength = 0.0173f;
        private const float LittleFingerTipLength = 0.01596f;

        /// <summary>
        /// Precalculated fingertip lengths used for scaling the fingertips of the skinnedmesh
        /// to match with tracked hand fingertip size 
        /// </summary>
        private Dictionary<HandJoint, float> fingerTipLengths = new Dictionary<HandJoint, float>()
        {
            {HandJoint.ThumbTip, thumbFingerTipLength },
            {HandJoint.IndexTip, indexingerTipLength },
            {HandJoint.MiddleTip, middleFingerTipLength },
            {HandJoint.RingTip, ringFingerTipLength },
            {HandJoint.LittleTip, LittleFingerTipLength }
        };

        /// <summary>
        /// Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector.
        /// </summary>
        public Quaternion UserBoneRotation
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
        private Quaternion Reorientation
        {
            get
            {
                return Quaternion.Inverse(Quaternion.LookRotation(ModelFingerPointing, -ModelPalmFacing));
            }
        }

        private Transform RetrieveChild(HandJoint parentJoint)
        {
            Transform parentJointTransform = riggedVisualJointsArray[(int)parentJoint];
            if (parentJointTransform != null && parentJointTransform.childCount > 0)
            {
                return parentJointTransform.GetChild(0);
            }
            return null;
        }

        public bool enableHandMesh;

        [SerializeField]
        [Tooltip("Renderer of the hand mesh")]
        private SkinnedMeshRenderer handRenderer = null;

        /// <summary>
        /// Renderer of the hand mesh.
        /// </summary>
        public SkinnedMeshRenderer HandRenderer => handRenderer;

        protected readonly Transform[] riggedVisualJointsArray = new Transform[HandTracker.JointCount];

        /// <summary>
        /// flag checking that the handRenderer was initialized with its own material
        /// </summary>
        private bool handRendererInitialized = false;

        ///// <summary>
        ///// Flag to only show a only show a warning once
        ///// </summary>
        private bool displayedMaterialPropertyWarning = false;
        #endregion

        // Initializing rigged visuals stuff
        protected virtual void Start()
        {
            // Ensure hand is not visible until we can update position first time.
            HandRenderer.enabled = enableHandMesh;

            // Initialize joint dictionary with their corresponding joint transforms
            riggedVisualJointsArray[(int)HandJoint.Wrist] = Wrist;
            riggedVisualJointsArray[(int)HandJoint.Palm] = Palm;

            // Thumb riggedVisualJointsArray, first node is user assigned, note that there are only 4 riggedVisualJointsArray in the thumb
            if (ThumbRoot)
            {
                if (ThumbRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)HandJoint.ThumbMetacarpal] = ThumbRoot;
                    riggedVisualJointsArray[(int)HandJoint.ThumbProximal] = RetrieveChild(HandJoint.ThumbMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)HandJoint.ThumbProximal] = ThumbRoot;
                }
                riggedVisualJointsArray[(int)HandJoint.ThumbDistal] = RetrieveChild(HandJoint.ThumbProximal);
                riggedVisualJointsArray[(int)HandJoint.ThumbTip] = RetrieveChild(HandJoint.ThumbDistal);
            }
            // Look up index finger riggedVisualJointsArray below the index finger root joint
            if (IndexRoot)
            {
                if (IndexRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)HandJoint.IndexMetacarpal] = IndexRoot;
                    riggedVisualJointsArray[(int)HandJoint.IndexProximal] = RetrieveChild(HandJoint.IndexMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)HandJoint.IndexProximal] = IndexRoot;
                }
                riggedVisualJointsArray[(int)HandJoint.IndexIntermediate] = RetrieveChild(HandJoint.IndexProximal);
                riggedVisualJointsArray[(int)HandJoint.IndexDistal] = RetrieveChild(HandJoint.IndexIntermediate);
                riggedVisualJointsArray[(int)HandJoint.IndexTip] = RetrieveChild(HandJoint.IndexDistal);
            }

            // Look up middle finger riggedVisualJointsArray below the middle finger root joint
            if (MiddleRoot)
            {
                if (MiddleRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)HandJoint.MiddleMetacarpal] = MiddleRoot;
                    riggedVisualJointsArray[(int)HandJoint.MiddleProximal] = RetrieveChild(HandJoint.MiddleMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)HandJoint.MiddleProximal] = MiddleRoot;
                }
                riggedVisualJointsArray[(int)HandJoint.MiddleIntermediate] = RetrieveChild(HandJoint.MiddleProximal);
                riggedVisualJointsArray[(int)HandJoint.MiddleDistal] = RetrieveChild(HandJoint.MiddleIntermediate);
                riggedVisualJointsArray[(int)HandJoint.MiddleTip] = RetrieveChild(HandJoint.MiddleDistal);
            }

            // Look up ring finger riggedVisualJointsArray below the ring finger root joint
            if (RingRoot)
            {
                if (RingRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)HandJoint.RingMetacarpal] = RingRoot;
                    riggedVisualJointsArray[(int)HandJoint.RingProximal] = RetrieveChild(HandJoint.RingMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)HandJoint.RingProximal] = RingRoot;
                }
                riggedVisualJointsArray[(int)HandJoint.RingIntermediate] = RetrieveChild(HandJoint.RingProximal);
                riggedVisualJointsArray[(int)HandJoint.RingDistal] = RetrieveChild(HandJoint.RingIntermediate);
                riggedVisualJointsArray[(int)HandJoint.RingTip] = RetrieveChild(HandJoint.RingDistal);
            }

            // Look up Little riggedVisualJointsArray below the Little root joint
            if (LittleRoot)
            {
                if (LittleRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)HandJoint.LittleMetacarpal] = LittleRoot;
                    riggedVisualJointsArray[(int)HandJoint.LittleProximal] = RetrieveChild(HandJoint.LittleMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)HandJoint.LittleProximal] = LittleRoot;
                }
                riggedVisualJointsArray[(int)HandJoint.LittleIntermediate] = RetrieveChild(HandJoint.LittleProximal);
                riggedVisualJointsArray[(int)HandJoint.LittleDistal] = RetrieveChild(HandJoint.LittleIntermediate);
                riggedVisualJointsArray[(int)HandJoint.LittleTip] = RetrieveChild(HandJoint.LittleDistal);
            }

            // Give the hand mesh its own material to avoid modifying both hand materials when making property changes
            var handMaterialInstance = new Material(handMaterial);
            handRenderer.sharedMaterial = handMaterialInstance;
            handRendererInitialized = true;
        }

        protected void OnEnable()
        {
            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

            if (handsSubsystem == null)
            {
                StartCoroutine(EnableWhenSubsystemAvailable());
            }
            else
            {
                for (int i = 0; i < HandTracker.JointCount; i++)
                {
                    jointMatrices.Add(new Matrix4x4());
                }
            }
        }

        /// <summary>
        /// Coroutine to wait until subsystem becomes available.
        /// </summary>
        private IEnumerator EnableWhenSubsystemAvailable()
        {
            yield return new WaitUntil(() => XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>() != null);
            OnEnable();
        }

        private void LateUpdate()
        {
            if (handsSubsystem == null)
            {
                return;
            }

            // Query all joints in the hand.
            if (!handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                HandRenderer.enabled = false;
                return;
            }

            RenderJoints(joints);
            RenderMesh(joints);
        }

        private void RenderJoints(IReadOnlyList<HandJointPose> joints)
        {
            for (int i = 0; i < joints.Count; i++)
            {
                // Skip joints with uninitialized quaternions.
                // This is temporary; eventually the HandsSubsystem will
                // be robust enough to never give us broken joints.
                if (joints[i].Rotation.Equals(new Quaternion(0, 0, 0, 0)))
                {
                    continue;
                }

                // Fill the matrices list with TRSs from the joint poses.
                jointMatrices[i] = Matrix4x4.TRS(joints[i].Position, joints[i].Rotation.normalized, Vector3.one * joints[i].Radius);
            }

            // Draw the joints.
            Graphics.DrawMeshInstanced(jointMesh, 0, jointMaterial, jointMatrices);
        }

        private void RenderMesh(IReadOnlyList<HandJointPose> joints)
        {
            HandRenderer.enabled = enableHandMesh;

            for (int i = 0; i < joints.Count; i++)
            {
                HandJoint handJoint = (HandJoint)i;
                HandJointPose handJointPose = joints[i];
                Transform jointTransform = riggedVisualJointsArray[i];

                if (jointTransform != null)
                {
                    if (handJoint == HandJoint.Palm)
                    {
                        if (ModelPalmAtLeapWrist)
                        {
                            Palm.position = joints[(int)HandJoint.Wrist].Position;


                            jointMatrices[i] = Matrix4x4.TRS(joints[(int)HandJoint.Wrist].Position, joints[(int)HandJoint.Wrist].Rotation.normalized, Vector3.one * joints[i].Radius * 3.0f);
                        }
                        else
                        {
                            Palm.position = handJointPose.Position;
                        }
                        Palm.rotation = handJointPose.Rotation * UserBoneRotation;
                    }
                    if (handJoint == HandJoint.Wrist)
                    {
                        if (!ModelPalmAtLeapWrist)
                        {
                          Wrist.position = handJointPose.Position;
                        }
                    }
                    else
                    {
                        // Finger riggedVisualJointsArray
                        jointTransform.rotation = handJointPose.Rotation * Reorientation;

                        if (DeformPosition)
                        {
                            jointTransform.position = handJointPose.Position;
                        }

                        if (ScaleLastFingerBone &&
                            (handJoint == HandJoint.ThumbDistal ||
                            handJoint == HandJoint.IndexDistal ||
                            handJoint == HandJoint.MiddleDistal ||
                            handJoint == HandJoint.RingDistal ||
                            handJoint == HandJoint.LittleDistal))
                        {
                            ScaleFingerTip(joints, jointTransform, handJoint + 1, jointTransform.position);
                        }
                    }
                }
            }

            // Update the hand material based on selectedness value
            UpdateHandMaterial();
        }

        private ArticulatedHandController controller;

        private void UpdateHandMaterial()
        {
            if (controller == null)
            {
                controller = GetComponent<ArticulatedHandController>();
            }

            float selectedness = controller.selectInteractionState.value;

            // Update the hand material
            float pinchStrength = Mathf.Pow(selectedness, 2.0f);

            if (handMaterial != null && handRendererInitialized)
            {
                // This can be propertyBlock.HasFloat(), but that is only in unity 2021+
                if (handRenderer.sharedMaterial.HasProperty(pinchStrengthMaterialProperty))
                {
                    // Set the property on the handRenderer
                    handRenderer.GetPropertyBlock(propertyBlock);
                    propertyBlock.SetFloat(pinchStrengthMaterialProperty, pinchStrength);
                    handRenderer.SetPropertyBlock(propertyBlock);
                }
                // Only show this warning once
                else if (!displayedMaterialPropertyWarning)
                {
                    Debug.LogWarning(String.Format("The property {0} for reacting to pinch strength was not found. A material with this property is required to visualize pinch strength.", pinchStrengthMaterialProperty));
                    displayedMaterialPropertyWarning = true;
                }
            }
        }

        // Private methods for finger tip scaling
        private void ScaleFingerTip(IReadOnlyList<HandJointPose> joints, Transform jointTransform, HandJoint fingerTipJoint, Vector3 boneRootPos)
        {
            // Set fingertip base bone scale to match the bone length to the fingertip.
            // This will only scale correctly if the model was constructed to match
            // the standard "test" edit-time hand model from the LeapMotion TestHandFactory.
            var boneTipPos = joints[(int)fingerTipJoint].Position;
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
