// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
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
            None = 0, LeftHand = 1 << 0, RightHand = 1 << 1
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

        [SerializeField]
        [Tooltip("The input action representing the left hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty leftHand;

        [SerializeField]
        [Tooltip("The input action representing the right hand's position. " +
                 "This is used if no joint data is available.")]
        private InputActionProperty rightHand;

        [SerializeField]
        [Tooltip("This is automatically set if a Canvas-based menu is used. " +
                 "If a non-canvas object is used, this should be set to approximate " +
                 "2D footprint of the menu object.")]
        private Rect menuSize;

        [SerializeField]
        [Tooltip("The amount of padding between the edge of the user's hand " +
                 "and the edge of the menu.")]
        private float padding = 0.04f;

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

            if ((target & TargetType.LeftHand) != 0)
            {
                leftAttach = GetJointAttach(XRNode.LeftHand, out leftViewDot, out leftToRightHandDistance);
                if (!leftAttach.HasValue)
                {
                    leftAttach = GetControllerAttach(XRNode.LeftHand);
                }
            }

            if ((target & TargetType.RightHand) != 0)
            {
                rightAttach = GetJointAttach(XRNode.RightHand, out rightViewDot, out rightToLeftHandDistance);
                if (!rightAttach.HasValue)
                {
                    rightAttach = GetControllerAttach(XRNode.RightHand);
                }
            }

            // Flag if we've switched to a new hand this frame,
            // and the menu is sufficiently "behind" the hand.
            // This is to avoid the menu "lerping" between hands
            // when very small (causing distracting visuals.)
            bool snapToNewHand = false;

            if (currentTarget == TargetType.None)
            {
                // Tie break!
                if (rightAttach.HasValue && leftAttach.HasValue)
                {
                    currentTarget = TargetType.RightHand;
                    snapToNewHand = rightViewDot < 0.5f;
                }
                else if (rightAttach.HasValue) // Switch from None to Right
                {
                    currentTarget = TargetType.RightHand;
                    snapToNewHand = rightViewDot < 0.5f;
                }
                else if (leftAttach.HasValue) // Switch from None to Left
                {
                    currentTarget = TargetType.LeftHand;
                    snapToNewHand = leftViewDot < 0.5f;
                }
            }
            else
            {
                // Should we cancel? If we have a target, and the current target is no longer valid, cancel.
                if (currentTarget == TargetType.RightHand && !rightAttach.HasValue)
                {
                    currentTarget = TargetType.None;
                }
                else if (currentTarget == TargetType.LeftHand && !leftAttach.HasValue)
                {
                    currentTarget = TargetType.None;
                }
            }

            // todo: tiebreak
            Pose? attach = currentTarget == TargetType.RightHand ? rightAttach : leftAttach;
            float viewDot = currentTarget == TargetType.RightHand ? rightViewDot : leftViewDot;
            float dist = currentTarget == TargetType.RightHand ? rightToLeftHandDistance : leftToRightHandDistance;

            if (rootRectTransform != null)
            {
                rootRectTransform.pivot = new Vector2(currentTarget == TargetType.RightHand ? 1 : 0, 0.5f);
                rootRectTransform.anchoredPosition = new Vector2(0,0);
            }

            if (!attach.HasValue)
            {
                // TODO: animate.
                visibleRoot.SetActive(false);
                return;
            }
            else
            {
                // TODO: animate.
                visibleRoot.SetActive(true);
            }

            visibleRoot.SetActive(viewDot > 0.15f);

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

            // Check if the hand is within the view angle.
            float viewAngle = Vector3.Angle(CameraCache.Main.transform.forward, palm.Position - CameraCache.Main.transform.position);
            if (viewAngle > minimumViewAngle) { return null; }

            Vector3 offsetVector = (-proximal.Right + -metacarpal.Right).normalized * (hand == XRNode.LeftHand ? 1 : -1);
            
            Pose attach = default;
            
            // The rotation that the hand menu would have if it were perfectly flat along the hand.
            Quaternion flatRotation = Quaternion.LookRotation(palm.Up, palm.Forward);

            viewDot = Mathf.Clamp01(Vector3.Dot(flatRotation * Vector3.forward, CameraCache.Main.transform.forward) * 1.4f);

            if (viewDot == 0) { return null; }

            attach.position = ((proximal.Position + metacarpal.Position) * 0.5f) + offsetVector * padding;
            attach.rotation = flatRotation * Quaternion.Euler(0, -90 * (1.0f - viewDot) * (hand == XRNode.LeftHand ? 1 : -1), 0);

            if (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip,
                                                               hand == XRNode.LeftHand ? XRNode.RightHand : XRNode.LeftHand,
                                                               out var oppositeIndexTip))
            {
                oppositeHandDistance = Mathf.Clamp01(Mathf.InverseLerp(0.05f, 0.2f, (oppositeIndexTip.Position - (attach.position + offsetVector * (menuSize.width * 0.5f))).magnitude));
            }
            
            return attach;
        }

        private Pose? GetControllerAttach(XRNode hand) => null;
        
    }
}