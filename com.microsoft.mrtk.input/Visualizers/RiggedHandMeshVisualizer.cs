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

        [SerializeField]
        [Tooltip("The transform of the wrist joint.")]
        private Transform wrist;

        // Caching local references 
        private HandsAggregatorSubsystem handsSubsystem;
        private ArticulatedHandController controller;

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

        // Initializing rigged visuals stuff
        protected virtual void Start()
        {
            int index = (int)TrackedHandJoint.Wrist;
            foreach (Transform child in wrist.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("end")) { continue; }
                Debug.Log("Adding " + child.name + " as " + (TrackedHandJoint)index);
                riggedVisualJointsArray[index++] = child;
            }

            // Give the hand mesh its own material to avoid modifying both hand materials when making property changes
            // handRenderer.material = handMaterial;
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
                            break;
                        case TrackedHandJoint.Wrist:
                            jointTransform.position = HandJointPose.Position;
                            jointTransform.rotation = HandJointPose.Rotation;
                            break;
                        case TrackedHandJoint.ThumbMetacarpal:
                        case TrackedHandJoint.IndexMetacarpal:
                        case TrackedHandJoint.MiddleMetacarpal:
                        case TrackedHandJoint.RingMetacarpal:
                        case TrackedHandJoint.LittleMetacarpal:
                            // For the rest of the joints, match the transform to the Joint Pose
                            jointTransform.rotation = Quaternion.LookRotation(HandJointPose.Position - joints[(int)TrackedHandJoint.Wrist].Position, HandJointPose.Up);
                            break;
                        case TrackedHandJoint.ThumbProximal:
                        case TrackedHandJoint.IndexProximal:
                        case TrackedHandJoint.MiddleProximal:
                        case TrackedHandJoint.RingProximal:
                        case TrackedHandJoint.LittleProximal:
                            jointTransform.rotation = joints[i-1].Rotation;
                            // jointTransform.localPosition = new Vector3(jointTransform.localPosition.x, jointTransform.localPosition.y, (joints[i-1].Position - joints[(int)TrackedHandJoint.Wrist].Position).magnitude * 0.01f);
                            break;
                        default:
                            jointTransform.rotation = joints[i-1].Rotation;
                            // jointTransform.localPosition = new Vector3(jointTransform.localPosition.x, jointTransform.localPosition.y, (joints[i-1].Position - joints[i-2].Position).magnitude * 0.01f);
                            // if (deformPosition)
                            // {
                                // jointTransform.position = HandJointPose.Position;
                            // }
                            
                            break;
                    }
                }
            }

            // Update the hand material based on selectedness value
            UpdateHandMaterial();
        }

        private void UpdateHandMaterial()
        {
            // if (controller == null)
            // {
            //     controller = GetComponent<ArticulatedHandController>();
            // }

            // float selectedness = controller.selectInteractionState.value;

            // // Update the hand material
            // float pinchStrength = Mathf.Pow(selectedness, 2.0f);


            // handRenderer.GetPropertyBlock(propertyBlock);
            // propertyBlock.SetFloat(pinchStrengthMaterialProperty, pinchStrength);
            // handRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
