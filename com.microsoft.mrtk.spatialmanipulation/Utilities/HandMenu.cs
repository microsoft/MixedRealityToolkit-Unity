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
        private Handedness target;

        /// <summary>
        /// A bitflag representing the hands that should be used for this menu.
        /// If both hands are selected, the menu will be visible on either.
        /// </summary>
        public Handedness Target { get => target; set => target = value; }

        [SerializeField]
        [Tooltip("The mapping between the dot product and the activation amount.")]
        private AnimationCurve activationCurve = AnimationCurve.Linear(0, 0, 1, 1.4f);

        /// <summary>
        /// The mapping between the dot product and the activation amount.
        /// </summary>
        public AnimationCurve ActivationCurve { get => activationCurve; set => activationCurve = value; }

        [SerializeField]
        [Tooltip("The minimum view angle (angle between the hand and the user's view direction) " +
                 "at which the menu can activate.")]
        private float minimumViewAngle = 45f;

        /// <summary>
        /// The minimum view angle (angle between the hand and the user's view direction)
        /// at which the menu can activate.
        /// </summary>
        public float MinimumViewAngle { get => minimumViewAngle; set => minimumViewAngle = value; }

        [SerializeField]
        [Tooltip("The input action representing the left hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty leftHandPosition = new InputActionProperty(
            new InputAction(binding: "<XRController>{LeftHand}/devicePosition")
        );

        [SerializeField]
        [Tooltip("The input action representing the left hand's rotation. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty leftHandRotation = new InputActionProperty(
            new InputAction(binding: "<XRController>{LeftHand}/deviceRotation")
        );

        [SerializeField]
        [Tooltip("The input action representing the right hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty rightHandPosition = new InputActionProperty(
            new InputAction(binding: "<XRController>{RightHand}/devicePosition")
        );

        [SerializeField]
        [Tooltip("The input action representing the left hand's rotation. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty rightHandRotation = new InputActionProperty(
            new InputAction(binding: "<XRController>{RightHand}/deviceRotation")
        );

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

        #region Private Fields

        // The rect transform located at the visibleRoot (if one exists)
        private RectTransform rootRectTransform = null;

        // The hand target we're currently targeting.
        private TargetedHand currentTarget = null;

        private TargetedHand leftHand = null;

        private TargetedHand rightHand = null;

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

            leftHand = new TargetedHand()
            {
                Menu = this,
                Hand = Handedness.Left,
                PositionAction = leftHandPosition.action,
                RotationAction = leftHandRotation.action,
                Opposite = rightHand,
                Padding = padding
            };

            rightHand = new TargetedHand()
            {
                Menu = this,
                Hand = Handedness.Right,
                PositionAction = rightHandPosition.action,
                RotationAction = leftHandPosition.action,
                Opposite = leftHand,
                Padding = padding
            };
        }

        private void Update()
        {
            Pose leftPose = default, rightPose = default;
            bool gotLeftAttach = (target & Handedness.Left) != 0 && leftHand.TryGetAttachPose(out leftPose);
            bool gotRightAttach = (target & Handedness.Right) != 0 && rightHand.TryGetAttachPose(out rightPose);

            // Construct bitflag for which hand targets are actually available.
            Handedness available = (((gotLeftAttach && leftHand.Activation > 0) ? Handedness.Left : Handedness.None) |
                                   ((gotRightAttach && rightHand.Activation > 0) ? Handedness.Right : Handedness.None)) & target;

            TargetedHand lastTarget = currentTarget;

            if (available == Handedness.Both)
            {
                // Tiebreak! If both hands are valid targets, then we either stay with our existing target
                // or tiebreak to the right hand.
                // Tiebreak to the right hand iff we weren't targeting anything last frame.
                if (lastTarget == null)
                {
                    currentTarget = rightHand;
                }
                else
                {
                    currentTarget = lastTarget;
                }
            }
            else if (available == Handedness.Left)
            {
                currentTarget = leftHand;
            }
            else if (available == Handedness.Right)
            {
                currentTarget = rightHand;
            }
            else
            {
                currentTarget = null;
            }

            // Flag if we've switched to a new hand this frame,
            // and the menu is sufficiently "behind" the hand.
            // This is to avoid the menu "lerping" between hands
            // when very small (causing distracting visuals.)
            bool snapToNewHand = lastTarget != currentTarget && currentTarget.Activation < 0.5f;

            // No target? Disable the menu.
            if (currentTarget == null && visibleRoot.activeSelf)
            {
                visibleRoot.SetActive(false); // Disable the menu.
            }
            else // Otherwise, make sure it's visible.
            {
                if (visibleRoot.activeSelf == false)
                {
                    visibleRoot.SetActive(true);
                    // If we've just set the menu active,
                    // make sure we snap to the hand immediately.
                    snapToNewHand = true;
                }
            }

            // If we've switched hands, ensure the menu's pivots and anchors are correctly set.
            if (lastTarget != currentTarget && rootRectTransform != null)
            {
                rootRectTransform.pivot = new Vector2(currentTarget.Hand == Handedness.Right ? 1 : 0, 0.5f);
                rootRectTransform.anchoredPosition = new Vector2(0,0);
            }

            // If the menu is sufficiently "deactivated", just disable it.
            visibleRoot.SetActive(currentTarget.Activation > 0.15f);

            // "Genie effect".
            transform.localScale = Vector3.one * currentTarget.Activation;

            Pose attach = currentTarget == leftHand ? leftPose : rightPose;
            
            if (snapToNewHand)
            {
                transform.position = attach.position;
                transform.rotation = attach.rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, attach.position, Mathf.Lerp(0.1f, 0.5f, currentTarget.OppositeHandDistance));
                transform.rotation = Quaternion.Slerp(transform.rotation, attach.rotation, Mathf.Lerp(0.1f, 0.5f, currentTarget.OppositeHandDistance));
            }
        }

        private class TargetedHand
        {
            public HandMenu Menu { get; set; }
            public Handedness Hand { get; set; }

            public float Activation { get; set; }

            public bool Locked {
                get;
                set;
            }

            public TargetedHand Opposite;

            public float OppositeHandDistance { get; private set; }
            public float Padding;
            public InputAction PositionAction;
            public InputAction RotationAction;

            /// <summary>
            /// Tries to get the attach pose from the hand joints on the given <paramref name="hand"/>.
            /// </summary>
            public bool TryGetAttachPose(out Pose pose)
            {
                pose = default;

                // Short-circuits.
                return !TryGetJointAttach(out pose) || TryGetControllerAttach(out pose);
            }

            private bool TryGetJointAttach(out Pose pose)
            {
                pose = default;

                if (XRSubsystemHelpers.HandsAggregator == null) { return false; }

                bool gotJoints = true;
                gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleMetacarpal,
                                                                            Hand.ToXRNode().Value,
                                                                            out var metacarpal);
                gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleProximal,
                                                                            Hand.ToXRNode().Value,
                                                                            out var proximal);
                gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm,
                                                                            Hand.ToXRNode().Value,
                                                                            out var palm);
                if (!gotJoints) { return false; }

                // The anchor's position is halfway between the proximal and metacarpal,
                // and the rotation is facing upwards out of the palm and up along the fingers.
                Pose anchor = new Pose(
                    (proximal.Position + metacarpal.Position) * 0.5f,
                    Quaternion.LookRotation(palm.Up, palm.Forward)
                );

                // The offset vector (direction from the hand in which the menu will be positioned)
                // is the averaged away-from-pinky vector of the proximal and metacarpal joints.
                Vector3 offsetVector = (-proximal.Right + -metacarpal.Right).normalized * (Hand == Handedness.Left ? 1 : -1);

                // Making an assumption here; if we're getting joints on the left hand, we probably aren't in "motion controller"
                // mode, and we can use joints for getting the opposite hand's position.
                Pose? oppositeHandPose = null;
                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip,
                                                                   Opposite.Hand.ToXRNode().Value,
                                                                   out var oppositeIndexTip))
                {
                    oppositeHandPose = oppositeIndexTip;
                }

                // Compute the dot product between the anchor (hand) rotation and
                // the user's view direction. The activation amount is then computed
                // from this dot product, multiplied by the flatness of the hand, as
                // evaluated by the activation curve.
                Activation = Mathf.Clamp01(Vector3.Dot(anchor.rotation * Vector3.forward, CameraCache.Main.transform.forward));

                // Compute the "flatness" of the hand, but only when we haven't "locked on" to the hand.
                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, Hand.ToXRNode().Value, out HandJointPose indexTipPose) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.RingTip, Hand.ToXRNode().Value, out HandJointPose ringTipPose) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.MiddleKnuckle, Hand.ToXRNode().Value, out HandJointPose middleKnucklePose) &&
                    Locked == false)
                {
                    Vector3 fingerNormal = Vector3.Cross(indexTipPose.Position - middleKnucklePose.Position,
                                                    ringTipPose.Position - indexTipPose.Position).normalized;
                    fingerNormal *= (Hand == Handedness.Right) ? 1.0f : -1.0f;
                    float flatness = (Vector3.Dot(fingerNormal, palm.Up) + 1.0f) / 2.0f;
                    Activation *= Mathf.InverseLerp(0.5f, 1.0f, flatness);
                }

                if (!Locked)
                {
                    // Compute the view angle to determine whether the hand is
                    // within the "FOV" restriction specified by the user (minimumViewAngle)
                    float viewAngle = Vector3.Angle(
                        CameraCache.Main.transform.forward,
                        anchor.position - CameraCache.Main.transform.position
                    );

                    Activation *= Mathf.InverseLerp(25, 0, viewAngle);
                }

                Activation = Mathf.Clamp01(Menu.activationCurve.Evaluate(Activation));

                if (Activation >= 1.0f)
                {
                    Locked = true; // We're active! Hand flatness and gaze won't be able to deactivate, only the hand rotation.
                }

                // Finally, compute the actual attach pose.
                return GetAttach(anchor, oppositeHandPose, offsetVector, Padding, Activation);
            }

            /// <summary>
            /// Computes the attach pose from the controller-based <paramref name="gripPose"/>.
            /// </summary>
            /// <param name="gripPose">The grip pose to compute the attach pose for.</param>
            /// <param name="activation">The dot product of the hand's view direction and the user's view direction.</param>
            /// <param name="oppositeHandDistance">The distance between <paramref name="hand"/> and the opposite hand.</param>
            private bool TryGetControllerAttach(out Pose pose)
            {
                pose = default;
                if (!PoseFromActions(RotationAction, PositionAction, out Pose gripPose)) { return false;}

                // The controller-based anchor point is calculated from the grip pose.
                // Imagine the hand menu extending out from the bottom of a controller,
                // with the vertical axis of the menu aligned with the grip pose's Z axis.
                Vector3 offsetVector = -gripPose.up;
                Pose anchor = new Pose(
                    gripPose.position,
                    Quaternion.LookRotation(
                        -gripPose.right * (hand == XRNode.LeftHand ? 1 : -1),
                        gripPose.forward
                    )
                );

                // We don't know exactly how large the user's hand is,
                // so we'll make an educated guess.
                const float averageHandRadius = 0.04f;

                // Compute the dot product between the anchor (hand) rotation and
                // the user's view direction. The activation amount is then computed
                // from this dot product as evaluated by the activation curve.
                activation = Mathf.Clamp01(
                    activationCurve.Evaluate(
                        Vector3.Dot(
                            anchor.rotation * Vector3.forward,
                            CameraCache.Main.transform.forward
                        )
                    )
                );

                if (activation <= 0) { return default; }

                return GetAttach(hand, anchor, oppositeGripPose,
                                offsetVector, Padding + averageHandRadius,
                                activation, out oppositeHandDistance);
            }

            // Computes the hand menu attach pose from a hand/controller agnostic
            // set of poses and information.
            private Pose GetAttach(Pose anchor, Pose? opposite,
                                    Vector3 offsetVector, float padding,
                                    float activation)
            {
                OppositeHandDistance = 1.0f;

                // Compute the attach pose. The position is the anchor position offset
                // along the offset vector, by the padding amount.
                attach.position = anchor.position + offsetVector * padding;

                // The rotation is the anchor's rotation, mixed with a 0-90 rotation blended by
                // the activation amount. This results in a procedural "flipping" effect as the user
                // flips their hand over.
                attach.rotation = anchor.rotation * Quaternion.Euler(0, -90 * (1.0f - activation) * (hand == XRNode.LeftHand ? 1 : -1), 0);

                // If we have a pose for the opposite hand, compute the distance
                // between the opposite hand and the center of the hand menu. This will
                // be used to dampen the motion of the hand menu as the opposite hand approaches.
                if (opposite.HasValue)
                {
                    Vector3 menuCenter = attach.position + offsetVector * (menuSize.width * 0.5f);
                    OppositeHandDistance = Mathf.Clamp01(
                        Mathf.InverseLerp(0.05f, 0.2f, (opposite.Value.position - menuCenter).magnitude)
                    );
                }

                return attach;
            }

            /// <summary>
            /// Convert a pair (position, rotation) of input actions into a Pose.
            /// Performs the relevant tracking state checks; returns false if the device isn't tracked.
            /// </summary>
            private bool PoseFromActions(InputAction positionAction, InputAction rotationAction, out Pose pose)
            {
                pose = default;
                if (positionAction == null || rotationAction == null) { return false; }
                
                if (!positionAction.enabled) { positionAction.Enable(); }
                if (!rotationAction.enabled) { rotationAction.Enable(); }

                // If the device isn't tracked, no pose is available.
                TrackedDevice device = positionAction.activeControl?.device as TrackedDevice;
                if (device == null ||
                    ((InputTrackingState)device.trackingState.ReadValue() &
                        (InputTrackingState.Position | InputTrackingState.Rotation)) !=
                        (InputTrackingState.Position | InputTrackingState.Rotation))
                {
                    return false;
                }

                pose = PlayspaceUtilities.TransformPose(
                    new Pose(
                        positionAction.ReadValue<Vector3>(),
                        rotationAction.ReadValue<Quaternion>()
                    )
                );

                return true;
            }

            /// <summary>
            /// Convert a pair (position, rotation) of input actions into a Pose.
            /// Performs the relevant tracking state checks; returns null if the device isn't tracked.
            /// </summary>
            private bool PoseFromActions(InputActionProperty positionAction, InputActionProperty rotationAction, out Pose pose)
            {
                return PoseFromActions(positionAction.action, rotationAction.action, out pose);
            }
        }
        
    }
}