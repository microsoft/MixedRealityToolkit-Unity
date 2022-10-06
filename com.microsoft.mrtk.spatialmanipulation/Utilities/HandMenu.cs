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

        /// <summary>
        /// This is automatically set if a Canvas-based menu is used.
        /// If a non-canvas object is used, this should be set to approximate
        /// 2D footprint of the menu object.
        /// </summary>
        public Rect MenuSize { get => menuSize; set => menuSize = value; }

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

        #region Hand-tuned constants

        // The range to inverse-lerp across when damping the movement of the menu
        // based on the opposite hand's proximity.
        private static readonly float minOppositeHandDistance = 0.05f;
        private static readonly float maxOppositeHandDistance = 0.25f;

        // When the menu is less-activated than this threshold, it'll be hidden.
        private static readonly float minActivationToHide = 0.1f;

        // How "activated" must a hand be to consider it available for the menu to attach to?
        private static readonly float minActivationThreshold = 0.01f;

        // A non-framerate-independent filtering to reduce jitter.
        private static readonly float perFrameFiltering = 0.4f;

        private static readonly float minHandNearDamping = 0.4f;
        private static readonly float maxHandNearDamping = 0.9f;

        #endregion Hand-tuned constants

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
                Padding = padding
            };

            rightHand = new TargetedHand()
            {
                Menu = this,
                Hand = Handedness.Right,
                PositionAction = rightHandPosition.action,
                RotationAction = rightHandRotation.action,
                Padding = padding
            };

            leftHand.Opposite = rightHand;
            rightHand.Opposite = leftHand;
        }

        private void Update()
        {
            Pose leftPose = default, rightPose = default;
            bool gotLeftAttach = (target & Handedness.Left) != 0 && leftHand.TryGetAttachPose(out leftPose);
            bool gotRightAttach = (target & Handedness.Right) != 0 && rightHand.TryGetAttachPose(out rightPose);

            // Construct bitflag for which hand targets are actually available.
            Handedness available = (((gotLeftAttach && leftHand.Activation > minActivationThreshold) ? Handedness.Left : Handedness.None) |
                                   ((gotRightAttach && rightHand.Activation > minActivationThreshold) ? Handedness.Right : Handedness.None)) & target;

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

            // No target? Disable the menu.
            if (currentTarget == null)
            {
                if (visibleRoot.activeSelf)
                {
                    visibleRoot.SetActive(false); // Disable the menu.
                }

                // Return early, nothing else to do.
                return;
            }
            else // Otherwise, make sure it's visible.
            {
                if (visibleRoot.activeSelf == false)
                {
                    visibleRoot.SetActive(true);
                }
            }

            // If we've switched hands, ensure the menu's pivots and anchors are correctly set.
            if (lastTarget != currentTarget && rootRectTransform != null)
            {
                rootRectTransform.pivot = new Vector2(currentTarget.Hand == Handedness.Right ? 1 : 0, 0.5f);
                rootRectTransform.anchoredPosition = new Vector2(0,0);
            }

            // If the menu is sufficiently "deactivated", just disable it.
            visibleRoot.SetActive(currentTarget.Activation > minActivationToHide);

            // "Genie effect".
            transform.localScale = Vector3.one * currentTarget.Activation;
            
            // Apply pose to the menu. Dampen the movement when the opposite hand is nearby for precision.
            Pose attach = currentTarget == leftHand ? leftPose : rightPose;

            // Depending on how close the other hand is, we damp more or less.
            float dampingFactor = Mathf.Lerp(minHandNearDamping, maxHandNearDamping, Mathf.InverseLerp(maxOppositeHandDistance, minOppositeHandDistance, currentTarget.OppositeHandDistance));
            transform.position = Vector3.Lerp(attach.position, transform.position, dampingFactor);
            transform.rotation = Quaternion.Slerp(attach.rotation, transform.rotation, dampingFactor);
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

            #region Hand-tuned constants

            // Above this activation threshold, the menu will be considered "locked"
            // and secondary activation factors like gaze and hand-flatness will be ignored.
            private readonly float lockThreshold = 0.9f;

            // Below this activation threshold, if the menu is locked, it will be unlocked
            // and secondary activation factors like gaze and hand-flatness will be considered.
            private readonly float unlockThreshold = 0.2f;

            // The range (in degrees) between the Menu.MinimumViewAngle and the angle at which
            // the menu will be completely deactivated by gaze.
            private readonly float gazeLerpRange = 10f;

            // We don't know exactly how large the user's hand is,
            // so we'll make an educated guess when we're attaching a hand menu
            // to a motion controller.
            private readonly float controllerBasedHandRadius = 0.04f;

            // Minimum flatness value. Below this, the menu won't be able to activate.
            private readonly float minFlatness = 0.75f;

            // Maximum flatness value. Above this, the menu will be fully activated.
            private readonly float maxFlatness = 0.9f;

            // An additional small # of degrees to bias the rotation of the menu towards
            // the user for easier use.
            private readonly float LookTowardsUserDegrees = 15;

            #endregion

            /// <summary>
            /// Tries to get the attach pose from the hand joints on the given <paramref name="hand"/>.
            /// </summary>
            public bool TryGetAttachPose(out Pose pose)
            {
                pose = default;

                // Short-circuits.
                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, Hand.ToXRNode().Value, out _))
                {
                    return TryGetJointAttach(out pose);
                }
                else
                {
                    return TryGetControllerAttach(out pose);
                }
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

                // Making an assumption here; if we're getting joints on the left hand, we probably aren't in "motion controller"
                // mode, and we can use joints for getting the opposite hand's position.
                Pose? oppositeHandPose = null;

                Debug.Assert(Opposite != null, "Opposite hand is null!");
                Debug.Assert(XRSubsystemHelpers.HandsAggregator != null, "Hands aggregator is null!");
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
                float tempActivation = Mathf.Clamp01(Vector3.Dot(anchor.rotation * Vector3.forward, (anchor.position - Camera.main.transform.position).normalized));

                // Save us some work if the hand is *really* not ready yet.
                if (tempActivation < 0.01f) { return false; }

                // Compute the "flatness" of the hand, but only when we haven't "locked on" to the hand.
                if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, Hand.ToXRNode().Value, out HandJointPose indexTipPose) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.RingTip, Hand.ToXRNode().Value, out HandJointPose ringTipPose) &&
                    XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.MiddleProximal, Hand.ToXRNode().Value, out HandJointPose middleProximalPose) &&
                    Locked == false)
                {
                    Vector3 fingerNormal = Vector3.Cross(indexTipPose.Position - middleProximalPose.Position,
                                                    ringTipPose.Position - indexTipPose.Position).normalized;
                    fingerNormal *= (Hand == Handedness.Right) ? 1.0f : -1.0f;
                    float flatness = (Vector3.Dot(fingerNormal, palm.Up) + 1.0f) / 2.0f;
                    tempActivation *= Mathf.InverseLerp(minFlatness, maxFlatness, flatness);
                }

                tempActivation = Mathf.Clamp01(Menu.ActivationCurve.Evaluate(tempActivation));

                // Reduce activation for out-of-view hands.
                tempActivation *= FOVFactor(anchor);

                // Do a very small amount of per-frame filtering to reduce jitter.
                Activation = Mathf.Lerp(Activation, tempActivation, HandMenu.perFrameFiltering);

                if (Activation <= HandMenu.minActivationThreshold) { return false; }

                // The offset vector (direction from the hand in which the menu will be positioned)
                // is the averaged away-from-pinky vector of the proximal and metacarpal joints.
                Vector3 offsetVector = (-proximal.Right + -metacarpal.Right).normalized * (Hand == Handedness.Left ? 1 : -1);

                // Finally, compute the actual attach pose.
                pose = GetAttach(anchor, oppositeHandPose, offsetVector, Padding);
                return true;
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
                if (!PoseFromActions(PositionAction, RotationAction, out Pose gripPose)) { return false; }

                // The controller-based anchor point is calculated from the grip pose.
                // Imagine the hand menu extending out from the bottom of a controller,
                // with the vertical axis of the menu aligned with the grip pose's Z axis.
                Vector3 offsetVector = gripPose.right * (Hand == Handedness.Right ? 1 : -1);
                Pose anchor = new Pose(
                    gripPose.position,
                    Quaternion.LookRotation(
                        gripPose.up,
                        gripPose.forward
                    )
                );

                // Making an assumption here; if we're reading input actions from this hand,
                // we can probably read them from the other hand. (i.e., if one hand is a motion controller,
                // both are probably motion controllers!)
                Pose? oppositeHandPose = null;
                if (PoseFromActions(Opposite.PositionAction, Opposite.RotationAction, out Pose oppositeGripPose))
                {
                    oppositeHandPose = oppositeGripPose;
                }

                // Compute the dot product between the anchor (hand) rotation and
                // the user's view direction. The activation amount is then computed
                // from this dot product as evaluated by the activation curve.
                float tempActivation = Mathf.Clamp01(
                    Menu.ActivationCurve.Evaluate(
                        Vector3.Dot(
                            anchor.rotation * Vector3.forward,
                            (anchor.position - Camera.main.transform.position).normalized
                        )
                    )
                );

                // Reduce activation for out-of-view controllers.
                tempActivation *= FOVFactor(anchor);

                // Do a very small amount of per-frame filtering to reduce jitter.
                Activation = Mathf.Lerp(Activation, tempActivation, HandMenu.perFrameFiltering);

                if (Activation <= HandMenu.minActivationThreshold) { return false; }

                // Use controllerBasedHandRadius to estimate the average hand size when we don't
                // have any hand joints to go off of.
                pose = GetAttach(anchor, oppositeHandPose, offsetVector, Padding + controllerBasedHandRadius);
                return true;
            }

            // Returns a 0..1 range, where 0.0 is completely out of view, and 1.0 is completely in view.
            // If the hand menu is locked, this will always return 1.0.
            private float FOVFactor(Pose anchor)
            {
                if (!Locked)
                {
                    // Compute the view angle to determine whether the hand is
                    // within the "FOV" restriction specified by the user (minimumViewAngle)
                    float viewAngle = Vector3.Angle(
                        Camera.main.transform.forward,
                        anchor.position - Camera.main.transform.position
                    );
                    Debug.Log(viewAngle);
                    // Multiply the activation by this gaze angle, lerped across gazeLerpRange for smoothness.
                    return Mathf.InverseLerp(Menu.MinimumViewAngle + gazeLerpRange, Menu.MinimumViewAngle, viewAngle);
                }
                else
                {
                    // If the hand menu is in "locked mode", gaze direction can't deactivate it.
                    return 1.0f;
                }
            }

            // Computes the hand menu attach pose from a hand/controller agnostic
            // set of poses and information.
            private Pose GetAttach(Pose anchor, Pose? oppositeHandPose,
                                    Vector3 offsetVector, float padding)
            {
                OppositeHandDistance = 1.0f;

                Pose attach = new Pose(
                    // Compute the attach pose. The position is the anchor position offset
                    // along the offset vector, by the padding amount.
                    anchor.position + offsetVector * padding,
                    // The rotation is the anchor's rotation, mixed with a 0-90 rotation blended by
                    // the activation amount. This results in a procedural "flipping" effect as the user
                    // flips their hand over.
                    anchor.rotation * Quaternion.Euler(-LookTowardsUserDegrees, (-90 * (1.0f - Activation) + LookTowardsUserDegrees) * (Hand == Handedness.Left ? 1 : -1) , 0)
                );

                // If we have a pose for the opposite hand, compute the distance
                // between the opposite hand and the center of the hand menu. This will
                // be used to dampen the motion of the hand menu as the opposite hand approaches.
                if (oppositeHandPose.HasValue)
                {
                    Vector3 menuCenter = Menu.VisibleRoot.transform.position + Menu.VisibleRoot.transform.right.normalized * (Menu.MenuSize.width / 2) * (Hand == Handedness.Left ? 1 : -1);
                    OppositeHandDistance = (oppositeHandPose.Value.position - menuCenter).magnitude;
                }

                // Update locking state.
                if (Activation >= lockThreshold)
                {
                    Locked = true; // We're active! Hand flatness and gaze won't be able to deactivate, only the hand rotation.
                }
                else if (Activation <= unlockThreshold)
                {
                    Locked = false; // We're inactive! Hand flatness and gaze can deactivate.
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