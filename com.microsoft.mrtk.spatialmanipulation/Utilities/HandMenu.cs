// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Subsystems;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// A dedicated script for controlling hand menus. Best used in conjuction with
    /// a Canvas-based menu object, this will automatically position the menu
    /// to conform to the edge of a user's hand, with proper spacing/padding as well
    /// as logic for switching between hands.
    /// </summary>
    internal class HandMenu : MonoBehaviour
    {
        [Flags]
        internal enum TargetType
        {
            None = 0, LeftHand = 1 << 0, RightHand = 1 << 1, Both = LeftHand | RightHand
        }

        [SerializeField]
        [Tooltip("The visible root of the hand menu object. This is the object that " +
                 "will be enabled/disabled and positioned to fit the hand.")]
        private GameObject visibleRoot;

        /// <summary>
        /// The visible root of the hand menu object. This is the
        /// object that will be enabled/disabled and positioned to fit the hand.
        /// </summary>
        public GameObject VisibleRoot { get => visibleRoot; set => visibleRoot = value; }

        [SerializeField]
        [Tooltip("A bitflag representing the hands that should be used for this menu. " +
                 "If both hands are selected, the menu will be visible on either.")]
        private TargetType target;

        /// <summary>
        /// A bitflag representing the hands that should be used for this menu.
        /// If both hands are selected, the menu will be visible on either.
        /// </summary>
        public TargetType Target { get => target; set => target = value; }

        [Header("Fallback Bindings")]

        [SerializeField]
        [Tooltip("The input action representing the left hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty leftHandPosition;

        [SerializeField]
        [Tooltip("The input action representing the left hand's rotation. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty leftHandRotation;

        [SerializeField]
        [Tooltip("The input action representing the right hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty rightHandPosition;

        [SerializeField]
        [Tooltip("The input action representing the left hand's rotation. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty rightHandRotation;

        [Header("Menu Positioning")]

        [SerializeField]
        [Tooltip("This is automatically set if a Canvas-based menu is used. " +
                 "If a non-canvas object is used, this should be set to approximate " +
                 "2D footprint of the menu object.")]
        private Rect menuSize;

        [SerializeField]
        [Tooltip("The amount of padding between the edge of the user's hand " +
                 "and the edge of the menu.")]
        private float padding = 0.04f;

        /// <summary>
        /// The amount of padding between the edge of the user's hand and the edge of the menu.
        /// </summary>
        public float Padding { get => padding; set => padding = value; }

        [SerializeField]
        [Tooltip("The minimum view angle (angle between the hand and the user's view direction) " +
                 "at which the menu can activate.")]
        private float minimumViewAngle = 45f;

        /// <summary>
        /// The minimum view angle (angle between the hand and the user's view direction)
        /// at which the menu can activate.
        /// </summary>
        public float MinimumViewAngle { get => minimumViewAngle; set => minimumViewAngle = value; }

        #region Private Fields

        // The rect transform located at the visibleRoot (if one exists)
        private RectTransform rootRectTransform = null;

        // The hand target we're currently targeting.
        private TargetType currentTarget = TargetType.None;

        #endregion Private Fields

        private void Start()
        {
            if (visibleRoot != null)
            {
                rootRectTransform = visibleRoot.GetComponent<RectTransform>();
                if (rootRectTransform != null)
                {
                    menuSize.width = rootRectTransform.sizeDelta.x * rootRectTransform.lossyScale.x;
                }
            }
        }

        private void Update()
        {
            Pose? leftAttach = null;
            Pose? rightAttach = null;

            float leftViewDot = 0;
            float rightViewDot = 0;

            float leftToRightHandDistance = 1.0f;
            float rightToLeftHandDistance = 1.0f;

            // Is the left hand a desired target?
            if ((target & TargetType.LeftHand) != 0)
            {
                // Get either a joint-based attach point or a controller-pose-based attach point,
                // whichever is available.
                leftAttach = GetJointAttach(XRNode.LeftHand, out leftViewDot, out leftToRightHandDistance);
                if (!leftAttach.HasValue)
                {
                    leftAttach = GetControllerAttach(XRNode.LeftHand,
                                                     PoseFromActions(leftHandPosition, leftHandRotation),
                                                     PoseFromActions(rightHandPosition, rightHandRotation),
                                                     out leftViewDot, out leftToRightHandDistance);
                }
            }
            
            // Is the right hand a desired target?
            if ((target & TargetType.RightHand) != 0)
            {
                // Get either a joint-based attach point or a controller-pose-based attach point,
                // whichever is available.
                rightAttach = GetJointAttach(XRNode.RightHand, out rightViewDot, out rightToLeftHandDistance);
                if (!rightAttach.HasValue)
                {
                    rightAttach = GetControllerAttach(XRNode.RightHand,
                                                      PoseFromActions(rightHandPosition, rightHandRotation),
                                                      PoseFromActions(leftHandPosition, leftHandRotation),
                                                      out rightViewDot, out rightToLeftHandDistance);
                }
            }

            // Construct bitflag for which hand targets are actually available.
            TargetType available = (rightAttach.HasValue ? TargetType.RightHand : TargetType.None) |
                                   (leftAttach.HasValue ? TargetType.LeftHand : TargetType.None);

            // Our new current target is the intersection of the
            // available target bitflag and our desired targets bitflag.
            TargetType lastTarget = currentTarget;
            currentTarget = available & target;

            // Tiebreak! If both hands are valid targets, then we either stay with our existing target
            // or tiebreak to the right hand.
            if (currentTarget == TargetType.Both)
            {
                currentTarget = (lastTarget == TargetType.None || lastTarget == TargetType.Both) ? TargetType.RightHand : lastTarget;
            }

            Pose? attach = currentTarget == TargetType.RightHand ? rightAttach : leftAttach;
            float viewDot = currentTarget == TargetType.RightHand ? rightViewDot : leftViewDot;
            float dist = currentTarget == TargetType.RightHand ? rightToLeftHandDistance : leftToRightHandDistance;

            // Flag if we've switched to a new hand this frame,
            // and the menu is sufficiently "behind" the hand.
            // This is to avoid the menu "lerping" between hands
            // when very small (causing distracting visuals.)
            bool snapToNewHand = lastTarget != currentTarget && viewDot < 0.5f;

            // If we've switched hands, ensure the menu's pivots and anchors are correctly set.
            if (lastTarget != currentTarget && rootRectTransform != null)
            {
                rootRectTransform.pivot = new Vector2(currentTarget == TargetType.RightHand ? 1 : 0, 0.5f);
                rootRectTransform.anchoredPosition = new Vector2(0,0);
            }

            // Is there no attach available at all? Disable the menu.
            if (!attach.HasValue)
            {
                visibleRoot.SetActive(false);
                return;
            }
            else // Otherwise, make sure it's visible.
            {
                visibleRoot.SetActive(true);
            }

            // If the menu is sufficiently "deactivated", just disable it.
            visibleRoot.SetActive(viewDot > 0.15f);

            // "Genie effect".
            transform.localScale = Vector3.one * viewDot;
            
            if (snapToNewHand)
            {
                transform.position = attach.Value.position;
                transform.rotation = attach.Value.rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, attach.Value.position, Mathf.Lerp(0.1f, 0.5f, dist));
                transform.rotation = Quaternion.Slerp(transform.rotation, attach.Value.rotation, Mathf.Lerp(0.1f, 0.5f, dist));
            }
        }

        /// <summary>
        /// Computes the attach pose from the hand joints on the given <paramref name="hand"/>.
        /// </summary>
        /// <param name="hand">The hand to compute the attach pose for.</param>
        /// <param name="viewDot">The dot product of the hand's view direction and the user's view direction.</param>
        /// <param name="oppositeHandDistance">The distance between <paramref name="hand"/> and the opposite hand.</param>
        private Pose? GetJointAttach(XRNode hand, out float viewDot, out float oppositeHandDistance)
        {
            viewDot = 0;
            oppositeHandDistance = 1.0f;

            if (XRSubsystemHelpers.HandsAggregator == null) { return null; }

            bool gotJoints = true;
            gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleMetacarpal,
                                                                        hand,
                                                                        out var metacarpal);
            gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleProximal,
                                                                        hand,
                                                                        out var proximal);
            gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm,
                                                                        hand,
                                                                        out var palm);
            if (!gotJoints) { return null; }

            // The anchor's position is halfway between the proximal and metacarpal,
            // and the rotation is facing upwards out of the palm and up along the fingers.
            Pose anchor = new Pose(
                (proximal.Position + metacarpal.Position) * 0.5f,
                Quaternion.LookRotation(palm.Up, palm.Forward)
            );

            // The offset vector (direction from the hand in which the menu will be positioned)
            // is the averaged away-from-pinky vector of the proximal and metacarpal joints.
            Vector3 offsetVector = (-proximal.Right + -metacarpal.Right).normalized * (hand == XRNode.LeftHand ? 1 : -1);

            // Making an assumption here; if we're getting joints on the left hand, we probably aren't in "motion controller"
            // mode, and we can use joints for getting the opposite hand's position.
            Pose? oppositeHandPose = null;
            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip,
                                                               hand == XRNode.LeftHand ? XRNode.RightHand : XRNode.LeftHand,
                                                               out var oppositeIndexTip))
            {
                oppositeHandPose = oppositeIndexTip;
            }

            // Finally, compute the actual attach pose.
            return GetAttach(hand, anchor, oppositeHandPose, offsetVector, padding, out viewDot, out oppositeHandDistance);
        }

        /// <summary>
        /// Computes the attach pose from the controller-based <paramref name="gripPose"/>.
        /// </summary>
        /// <param name="gripPose">The grip pose to compute the attach pose for.</param>
        /// <param name="viewDot">The dot product of the hand's view direction and the user's view direction.</param>
        /// <param name="oppositeHandDistance">The distance between <paramref name="hand"/> and the opposite hand.</param>
        private Pose? GetControllerAttach(XRNode hand, Pose? gripPose, Pose? oppositeGripPose,
                                          out float viewDot,
                                          out float oppositeHandDistance)
        {
            viewDot = 0;
            oppositeHandDistance = 1.0f;

            if (!gripPose.HasValue) { return null;}

            Vector3 offsetVector = gripPose.Value.right * (hand == XRNode.LeftHand ? 1 : -1);

            // We don't know exactly how large the user's hand is,
            // so we'll make an educated guess.
            const float averageHandRadius = 0.03f;

            return GetAttach(hand, gripPose.Value, oppositeGripPose,
                             offsetVector, padding + averageHandRadius,
                             out viewDot, out oppositeHandDistance);
        }

        // Computes the hand menu attach pose from a hand/controller agnostic
        // set of poses and information.
        private Pose? GetAttach(XRNode hand, Pose anchor, Pose? opposite,
                                Vector3 offsetVector, float padding,
                                out float viewDot, out float oppositeHandDistance)
        {
            viewDot = 0;
            oppositeHandDistance = 1.0f;

            // Compute the view angle to determine whether the hand is
            // within the "FOV" restriction specified by the user (minimumViewAngle)
            float viewAngle = Vector3.Angle(
                CameraCache.Main.transform.forward,
                anchor.position - CameraCache.Main.transform.position);
            if (viewAngle > minimumViewAngle) { return null; }

            // Compute the dot product between the anchor (hand) rotation and
            // the user's view direction. Used for overall "activation" amount;
            // 0 is no activation, 1 is fully activated (hand facing user and visible)
            viewDot = Mathf.Clamp01(
                Vector3.Dot(
                    anchor.rotation * Vector3.forward,
                    CameraCache.Main.transform.forward
                ) * 1.4f);

            if (viewDot == 0) { return null; }

            Pose attach = default;

            // Compute the attach pose. The position is the anchor position offset
            // along the offset vector, by the padding amount.
            attach.position = anchor.position + offsetVector * padding;

            // The rotation is the anchor's rotation, mixed with a 0-90 rotation blended by
            // the viewDot. This results in a procedural "flipping" effect as the user
            // flips their hand over.
            attach.rotation = anchor.rotation * Quaternion.Euler(0, -90 * (1.0f - viewDot) * (hand == XRNode.LeftHand ? 1 : -1), 0);

            // If we have a pose for the opposite hand, compute the distance
            // between the opposite hand and the center of the hand menu. This will
            // be used to dampen the motion of the hand menu as the opposite hand approaches.
            if (opposite.HasValue)
            {
                Vector3 menuCenter = attach.position + offsetVector * (menuSize.width * 0.5f);
                oppositeHandDistance = Mathf.Clamp01(
                    Mathf.InverseLerp(0.05f, 0.2f, (opposite.Value.position - menuCenter).magnitude)
                );
            }

            return attach;
        }

        /// <summary>
        /// Convert a pair (position, rotation) of input actions into a Pose.
        /// Performs the relevant tracking state checks; returns null if the device isn't tracked.
        /// </summary>
        private Pose? PoseFromActions(InputAction positionAction, InputAction rotationAction)
        {
            if (positionAction == null || rotationAction == null) { return null; }

            // If the device isn't tracked, no pose is available.
            TrackedDevice device = positionAction.activeControl?.device as TrackedDevice;
            if (device == null ||
                device.trackingState.ReadValue() != (int)(InputTrackingState.Position | InputTrackingState.Rotation))
            {
                return null;
            }

            return PlayspaceUtilities.TransformPose(
                new Pose(
                    positionAction.ReadValue<Vector3>(),
                    rotationAction.ReadValue<Quaternion>()
                )
            );
        }

        /// <summary>
        /// Convert a pair (position, rotation) of input actions into a Pose.
        /// Performs the relevant tracking state checks; returns null if the device isn't tracked.
        /// </summary>
        private Pose? PoseFromActions(InputActionProperty positionAction, InputActionProperty rotationAction)
        {
            return PoseFromActions(positionAction.action, rotationAction.action);
        }
        
    }
}