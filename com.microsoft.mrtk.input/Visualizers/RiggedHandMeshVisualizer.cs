// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Basic hand joint visualizer that draws an instanced mesh on each hand joint.
    /// </summary>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Hands/Rigged Hand Mesh Visualizer")]
    public class RiggedHandMeshVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        // Caching local references 
        private HandsAggregatorSubsystem handsSubsystem;
        private ArticulatedHandController controller;

        #region RiggedHand variables
        /// <summary>
        /// Palm transform
        /// </summary>
        [SerializeField]
        private Transform palm;

        /// <summary>
        /// Wrist Transform
        /// </summary>
        [SerializeField]
        private Transform wrist;

        /// <summary>
        /// Thumb metacarpal transform  (thumb root)
        /// </summary>
        [SerializeField]
        private Transform thumbRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool ThumbRootIsMetacarpal = true;

        /// <summary>
        /// Index metacarpal transform (index finger root)
        /// </summary>
        [SerializeField]
        private Transform indexRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool IndexRootIsMetacarpal = false;

        /// <summary>
        /// Middle metacarpal transform (middle finger root)
        /// </summary>
        [SerializeField]
        private Transform middleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool MiddleRootIsMetacarpal = false;

        /// <summary>
        /// Ring metacarpal transform (ring finger root)
        /// </summary>
        [SerializeField]
        private Transform ringRoot;

        [Tooltip("Ring finger node is metacarpal joint.")]
        [SerializeField]
        private bool RingRootIsMetacarpal = false;

        /// <summary>
        /// Little metacarpal transform (Little finger root)
        /// </summary>
        [SerializeField]
        private Transform littleRoot;

        [Tooltip("First finger node is metacarpal joint.")]
        [SerializeField]
        private bool LittleRootIsMetacarpal = false;

        [Tooltip("Allows the mesh to be stretched to align with finger joint positions.")]
        [SerializeField]
        private bool deformPosition = true;

        [Tooltip("If non-zero, this vector and the ModelPalmFacing vector " +
          "will be used to re-orient the rigged hand's bones in the hand rig, to " +
          "compensate for bone axis discrepancies with the data " +
          "provided by the platform.")]
        [SerializeField]
        private Vector3 modelFingerPointing;

        [Tooltip("If non-zero, this vector and the ModelFingerPointing vector " +
          "will be used to re-orient the rigged hand's bones in the hand rig, to " +
          "compensate for bone axis discrepancies with the data " +
          "provided by the platform.")]
        [SerializeField]
        private Vector3 modelPalmFacing;

        /// <summary>
        /// Used to track whether the hand material has the appropriate property
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool showMissingPropertyWarning = true;

        [SerializeField]
        [Tooltip("Hand material to use for hand tracking hand mesh.")]
        [ShowInfoIf(UnityEditor.MessageType.Warning, "provided material is missing property " + pinchStrengthMaterialProperty, "showMissingPropertyWarning")]
        private Material handMaterial = null;

        /// <summary>
        /// Used to track whether the hand renderer was provided
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool showMissingRendererWarning = false;

        [SerializeField]
        [Tooltip("Renderer of the hand mesh")]
        [ShowInfoIf(UnityEditor.MessageType.Warning, "Rigged Mesh Renderer is missing", "showMissingRendererWarning")]
        private SkinnedMeshRenderer handRenderer = null;

        /// <summary>
        /// The property block which is used to modify the press intensity property on the material
        /// </summary>
        private MaterialPropertyBlock propertyBlock = null;

        /// <summary>
        /// Property name for modifying the mesh's appearance based on pinch strength
        /// </summary>
        private const string pinchStrengthMaterialProperty = "_PressIntensity";

        /// <summary>
        /// Rotation derived from the `modelFingerPointing` and
        /// `modelPalmFacing` vectors in the RiggedHand inspector.
        /// </summary>
        private Quaternion userBoneRotation
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
        private Quaternion reorientation
        {
            get
            {
                return Quaternion.Inverse(Quaternion.LookRotation(modelFingerPointing, -modelPalmFacing));
            }
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

        protected readonly Transform[] riggedVisualJointsArray = new Transform[(int)TrackedHandJoint.TotalJoints];
        #endregion

        // Initializing rigged visuals stuff
        protected virtual void Start()
        {
            // Initialize joint dictionary with their corresponding joint transforms
            riggedVisualJointsArray[(int)TrackedHandJoint.Palm] = palm;
            riggedVisualJointsArray[(int)TrackedHandJoint.Wrist] = wrist;

            // Thumb riggedVisualJointsArray, first node is user assigned, note that there are only 4 riggedVisualJointsArray in the thumb
            if (thumbRoot)
            {
                if (ThumbRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbMetacarpal] = thumbRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbProximal] = RetrieveChild(TrackedHandJoint.ThumbMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.ThumbProximal] = thumbRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.ThumbDistal] = RetrieveChild(TrackedHandJoint.ThumbProximal);
                riggedVisualJointsArray[(int)TrackedHandJoint.ThumbTip] = RetrieveChild(TrackedHandJoint.ThumbDistal);
            }
            // Look up index finger riggedVisualJointsArray below the index finger root joint
            if (indexRoot)
            {
                if (IndexRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexMetacarpal] = indexRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexProximal] = RetrieveChild(TrackedHandJoint.IndexMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.IndexProximal] = indexRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexIntermediate] = RetrieveChild(TrackedHandJoint.IndexProximal);
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexDistal] = RetrieveChild(TrackedHandJoint.IndexIntermediate);
                riggedVisualJointsArray[(int)TrackedHandJoint.IndexTip] = RetrieveChild(TrackedHandJoint.IndexDistal);
            }

            // Look up middle finger riggedVisualJointsArray below the middle finger root joint
            if (middleRoot)
            {
                if (MiddleRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleMetacarpal] = middleRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleProximal] = RetrieveChild(TrackedHandJoint.MiddleMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.MiddleProximal] = middleRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleIntermediate] = RetrieveChild(TrackedHandJoint.MiddleProximal);
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleDistal] = RetrieveChild(TrackedHandJoint.MiddleIntermediate);
                riggedVisualJointsArray[(int)TrackedHandJoint.MiddleTip] = RetrieveChild(TrackedHandJoint.MiddleDistal);
            }

            // Look up ring finger riggedVisualJointsArray below the ring finger root joint
            if (ringRoot)
            {
                if (RingRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingMetacarpal] = ringRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingProximal] = RetrieveChild(TrackedHandJoint.RingMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.RingProximal] = ringRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.RingIntermediate] = RetrieveChild(TrackedHandJoint.RingProximal);
                riggedVisualJointsArray[(int)TrackedHandJoint.RingDistal] = RetrieveChild(TrackedHandJoint.RingIntermediate);
                riggedVisualJointsArray[(int)TrackedHandJoint.RingTip] = RetrieveChild(TrackedHandJoint.RingDistal);
            }

            // Look up Little riggedVisualJointsArray below the Little root joint
            if (littleRoot)
            {
                if (LittleRootIsMetacarpal)
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.LittleMetacarpal] = littleRoot;
                    riggedVisualJointsArray[(int)TrackedHandJoint.LittleProximal] = RetrieveChild(TrackedHandJoint.LittleMetacarpal);
                }
                else
                {
                    riggedVisualJointsArray[(int)TrackedHandJoint.LittleProximal] = littleRoot;
                }
                riggedVisualJointsArray[(int)TrackedHandJoint.LittleIntermediate] = RetrieveChild(TrackedHandJoint.LittleProximal);
                riggedVisualJointsArray[(int)TrackedHandJoint.LittleDistal] = RetrieveChild(TrackedHandJoint.LittleIntermediate);
                riggedVisualJointsArray[(int)TrackedHandJoint.LittleTip] = RetrieveChild(TrackedHandJoint.LittleDistal);
            }

            // Give the hand mesh its own material to avoid modifying both hand materials when making property changes
            handRenderer.material = handMaterial;
        }

        protected void OnEnable()
        {
            // Ensure hand is not visible until we can update position first time.
            handRenderer.enabled = false;

            Debug.Assert(handNode == XRNode.LeftHand || handNode == XRNode.RightHand, $"HandVisualizer has an invalid XRNode ({handNode})!");

            handsSubsystem = XRSubsystemHelpers.GetFirstRunningSubsystem<HandsAggregatorSubsystem>();

            if (handsSubsystem == null)
            {
                StartCoroutine(EnableWhenSubsystemAvailable());
            }
        }

        protected void OnDisable()
        {
            // Disable the rigged hand renderer when this component is disabled
            handRenderer.enabled = false;
        }

        protected void OnValidate()
        {
            // Skip these steps if the handRenderer is null
            if (handRenderer == null)
            {
                showMissingRendererWarning = true;
                return;
            }
            else
            {
                showMissingRendererWarning = false;
            }

            // Set the handRenderer's material to the provided material if applicable
            if (handMaterial != null)
            {
                handRenderer.sharedMaterial = handMaterial;
            }

            // Check if the handRenderer's material has the pinchStrengthMaterialProperty
            if (!handRenderer.sharedMaterial.HasProperty(pinchStrengthMaterialProperty))
            {
                Debug.LogWarning(String.Format("The property {0} for reacting to pinch strength was not found. A material with this property is required to visualize pinch strength.", pinchStrengthMaterialProperty));
                showMissingPropertyWarning = true;
            }
            else
            {
                showMissingPropertyWarning = false;
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

        private void Update()
        {
            if (handsSubsystem == null)
            {
                return;
            }

            // Query all joints in the hand.
            if (!handsSubsystem.TryGetEntireHand(handNode, out IReadOnlyList<HandJointPose> joints))
            {
                handRenderer.enabled = false;
                return;
            }

            RenderMesh(joints);
        }

        private void RenderMesh(IReadOnlyList<HandJointPose> joints)
        {
            // Enable the hand mesh after once we have joint data
            handRenderer.enabled = true;

            for (int i = 0; i < joints.Count; i++)
            {
                TrackedHandJoint TrackedHandJoint = (TrackedHandJoint)i;
                HandJointPose HandJointPose = joints[i];
                Transform jointTransform = riggedVisualJointsArray[i];

                if (jointTransform != null)
                {
                    switch (TrackedHandJoint)
                    {
                        case TrackedHandJoint.Palm:
                            // We use the palm's  joint pose solely to control the orientation of the palm transform
                            jointTransform.rotation = HandJointPose.Rotation * userBoneRotation;
                            break;
                        case TrackedHandJoint.Wrist:
                            // We use the wrist's  joint pose solely to control the orientation of the wrist transform
                            jointTransform.position = HandJointPose.Position;
                            break;
                        default:
                            // For the rest of the joints, match the transform to the Joint Pose
                            jointTransform.rotation = HandJointPose.Rotation * reorientation;

                            if (deformPosition)
                            {
                                jointTransform.position = HandJointPose.Position;
                            }
                            break;
                    }
                }
            }

            // Update the hand material based on selectedness value
            UpdateHandMaterial();
        }

        private void UpdateHandMaterial()
        {
            if (controller == null)
            {
                controller = GetComponent<ArticulatedHandController>();
            }

            float selectedness = controller.selectInteractionState.value;

            // Update the hand material
            float pinchStrength = Mathf.Pow(selectedness, 2.0f);


            handRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(pinchStrengthMaterialProperty, pinchStrength);
            handRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
