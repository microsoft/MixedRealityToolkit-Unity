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
    internal class HandMenu : MonoBehaviour
    {
        [Flags]
        private enum TargetType
        {
            LeftHand = 1 << 0, RightHand = 1 << 1
        }

        [SerializeField]
        private GameObject root;

        [SerializeField]
        private TargetType target;

        [SerializeField]
        private InputActionProperty leftHand;

        [SerializeField]
        private InputActionProperty rightHand;

        [SerializeField]
        private Rect menuSize;

        [SerializeField]
        private float padding = 0.04f;

        private TargetType currentlyTrackedTarget;

        private RectTransform rootRectTransform = null;

        private void Start()
        {
            if (root != null)
            {
                rootRectTransform = root.GetComponent<RectTransform>();
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

            if ((target & TargetType.LeftHand) != 0)
            {
                leftAttach = GetJointAttach(XRNode.LeftHand, out leftViewDot);
                if (!leftAttach.HasValue)
                {
                    leftAttach = GetControllerAttach(XRNode.LeftHand);
                }
            }

            if ((target & TargetType.RightHand) != 0)
            {
                rightAttach = GetJointAttach(XRNode.RightHand, out rightViewDot);
                if (!rightAttach.HasValue)
                {
                    rightAttach = GetControllerAttach(XRNode.RightHand);
                }
            }

            // todo: tiebreak
            Pose? attach = rightAttach.HasValue ? rightAttach : leftAttach;
            float viewDot = rightAttach.HasValue ? rightViewDot : leftViewDot;

            if (rootRectTransform != null)
            {
                rootRectTransform.pivot = new Vector2(rightAttach.HasValue ? 1 : 0, 0.5f);
                rootRectTransform.anchoredPosition = new Vector2(0,0);
            }

            if (!attach.HasValue)
            {
                // TODO: animate.
                root.SetActive(false);
                return;
            }
            else
            {
                // TODO: animate.
                root.SetActive(true);
            }

            transform.localScale = Vector3.one * viewDot;
            transform.position = Vector3.Lerp(transform.position, attach.Value.position, 0.4f);
            transform.rotation = Quaternion.Slerp(transform.rotation, attach.Value.rotation, 0.4f);
        }

        private Pose? GetJointAttach(XRNode hand, out float viewDot)
        {
            viewDot = 0;
            if (XRSubsystemHelpers.HandsAggregator == null) { return null; }

            bool gotJoints = true;
            gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleMetacarpal,
                                                                        hand,
                                                                        out var metacarpal);
            gotJoints &= XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.LittleProximal,
                                                                        hand,
                                                                        out var proximal);
            if (!gotJoints) { return null; }

            Vector3 offsetVector = (-proximal.Right + -proximal.Right).normalized;

            
            Pose attach = default;
            
            // The rotation that the hand menu would have if it were perfectly flat along the hand.
            Quaternion flatRotation = Quaternion.LookRotation(proximal.Up, (proximal.Position - metacarpal.Position));

            viewDot = Mathf.Clamp01(Vector3.Dot(flatRotation * Vector3.forward, CameraCache.Main.transform.forward) * 1.4f);

            attach.position = ((proximal.Position + metacarpal.Position) * 0.5f) + offsetVector * padding;
            attach.rotation = flatRotation * Quaternion.Euler(0, -90 * (1.0f - viewDot), 0);
            
            return attach;
        }

        private Pose? GetControllerAttach(XRNode hand) => null;
        
    }
}