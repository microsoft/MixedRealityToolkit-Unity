// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.


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
    /// Hand visualizer that uses a rigged mesh/armature to render high-quality hand meshes.
    /// Not recommended for AR platforms like HoloLens, both for performance and design reasons. 
    /// </summary>
    /// <remarks>
    /// For augmented reality platforms such as HoloLens, we recommend not using any hand visualizations,
    /// as the conflict between the user's real hand and the slightly delayed holographic visualization
    /// can be more distracting than it's worth. However, for opaque platforms, this is a great solution.
    /// </remarks>
    [AddComponentMenu("Scripts/Microsoft/MRTK/Hands/Rigged Hand Mesh Visualizer")]
    public class RiggedHandMeshVisualizer : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The XRNode on which this hand is located.")]
        private XRNode handNode = XRNode.LeftHand;

        /// <summary> The XRNode on which this hand is located. </summary>
        public XRNode HandNode { get => handNode; set => handNode = value; }

        [SerializeField]
        [Range(0.8f, 1.2f)]
        [Tooltip("The overall hand mesh will be scaled by this amount to fit the user's real hand size.")]
        // Will be automatically calculated in the future.
        private float handScale = 1.0f;

        [SerializeField]
        [Tooltip("The transform of the wrist joint.")]
        private Transform wrist;

        /// <summary>
        /// Used to track whether the hand material has the appropriate property
        /// </summary>
        [SerializeField]
        [HideInInspector]
        private bool showMissingPropertyWarning = true;

        [SerializeField]
        [Tooltip("Hand material to use for hand tracking hand mesh.")]
        [ShowInfoIf(UnityEditor.MessageType.Warning, "provided material is missing property " + pinchAmountMaterialProperty, "showMissingPropertyWarning")]
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

        // The property block used to modify the pinch amount property on the material
        private MaterialPropertyBlock propertyBlock = null;

        /// <summary>
        /// Property name for modifying the mesh's appearance based on pinch strength
        /// </summary>
        private const string pinchAmountMaterialProperty = "_PinchAmount";

        // Caching local references 
        private HandsAggregatorSubsystem handsSubsystem;
        private XRBaseController controller;

        // The actual, physical, rigged joints that drive the skinned mesh.
        // Otherwise referred to as "armature". Must be in OpenXR order.
        private readonly Transform[] riggedVisualJointsArray = new Transform[(int)TrackedHandJoint.TotalJoints];

        // Record the initial, intrinsic lengths of the tip bones of the skinned mesh.
        // Used later to adjust the length of the skinned mesh's fingertips for improved accuracy.
        private readonly float[] initialTipLengths = new float[(int)TrackedHandJoint.TotalJoints];

        protected virtual void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();

            // Start the depth-first-traversal at the wrist index.
            int index = (int)TrackedHandJoint.Wrist;

            // This performs a depth-first-traversal of the armature. Ensure
            // the provided armature's bones/joints are in OpenXR order.
            foreach (Transform child in wrist.GetComponentsInChildren<Transform>())
            {
                // The "leaf joints" are excluded
                if (child.name.Contains("end")) { continue; }

                // Measure the intrinsic length of the fingertip bone. Used later to
                // adjust the length of the skinned mesh's fingertips for improved accuracy.
                if (child.name.Contains("tip"))
                {
                    // World-space, absolute, length of the tip-to-tip_end bone.
                    initialTipLengths[index] = child.GetChild(0).localPosition.z * child.lossyScale.z;
                }

                riggedVisualJointsArray[index++] = child;
            }
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

            // Check if the handRenderer's material has the pinchAmountMaterialProperty
            if (!handRenderer.sharedMaterial.HasProperty(pinchAmountMaterialProperty))
            {
                Debug.LogWarning(String.Format("The property {0} for reacting to pinch strength was not found. A material with this property is required to visualize pinch strength.", pinchAmountMaterialProperty));
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
            
            transform.localScale = new Vector3(handNode == XRNode.LeftHand ? -handScale : handScale, handScale, handScale);

            RenderMesh(joints);
        }

        private void RenderMesh(IReadOnlyList<HandJointPose> joints)
        {
            // Enable the hand mesh after once we have joint data
            handRenderer.enabled = true;

            for (int i = 0; i < joints.Count; i++)
            {
                HandJointPose jointPose = joints[i];

                // The actual, physical, rigged joint on the armature.
                // This actually corresponds to the "base" of the bone;
                // as an example, riggedVisualJointsArray[IndexMetacarpal] actually
                // corresponds to a transform that is located at the wrist joint,
                // but points towards the metacarpal joint location.
                // This discrepancy is because OpenXR uses joint locations/rotations,
                // whereas armatures/Unity/Blender use *bones*.
                Transform jointTransform = riggedVisualJointsArray[i];

                if (jointTransform != null)
                {
                    switch ((TrackedHandJoint)i)
                    {
                        case TrackedHandJoint.Palm:
                            // Don't track the palm. The hand mesh shouldn't have a "palm bone".
                            break;
                        case TrackedHandJoint.Wrist:
                            // Set the wrist directly from the joint data.
                            jointTransform.position = jointPose.Position;
                            jointTransform.rotation = jointPose.Rotation;
                            break;
                        case TrackedHandJoint.ThumbTip:
                        case TrackedHandJoint.IndexTip:
                        case TrackedHandJoint.MiddleTip:
                        case TrackedHandJoint.RingTip:
                        case TrackedHandJoint.LittleTip:
                            // The tip bone uses the joint rotation directly.
                            jointTransform.rotation = joints[i-1].Rotation;
                            break;
                        case TrackedHandJoint.ThumbMetacarpal:
                        case TrackedHandJoint.IndexMetacarpal:
                        case TrackedHandJoint.MiddleMetacarpal:
                        case TrackedHandJoint.RingMetacarpal:
                        case TrackedHandJoint.LittleMetacarpal:
                            // Special case metacarpals, because Wrist is not always i-1.
                            // This is the same "simple IK" as the default case, but with special index logic.
                            jointTransform.rotation = Quaternion.LookRotation(jointPose.Position - joints[(int)TrackedHandJoint.Wrist].Position, jointPose.Up);
                            break;
                        default:
                            // For all other bones, do a simple "IK" from the rigged joint to the joint data's position.
                            jointTransform.rotation = Quaternion.LookRotation(jointPose.Position - jointTransform.position, joints[i-1].Up);
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
                controller = GetComponentInParent<XRBaseController>();
            }

            if (controller == null || handRenderer == null) { return; }

            // Update the hand material
            float pinchAmount = Mathf.Pow(controller.selectInteractionState.value, 2.0f);
            handRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(pinchAmountMaterialProperty, pinchAmount);
            handRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
