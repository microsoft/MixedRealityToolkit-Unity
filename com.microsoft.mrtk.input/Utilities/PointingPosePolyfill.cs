// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Subsystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// A small PoseDriver-style script that drives the transform of the attached object
    /// according to a synthesized hand ray vector. A temporary polyfill for devices
    /// that are missing a pointing pose when using hand tracking.
    /// </summary>
    internal class PointingPosePolyfill : MonoBehaviour
    {
        private ArticulatedHandController controller;
        private HandRay handRay;
        private HandsAggregatorSubsystem handsAggregator;
        private Pose originalTransform;
        private bool transformDirty = false;

        private void Awake()
        {
            handRay = new HandRay();
            controller = GetComponentInParent<ArticulatedHandController>();
        }

        private void OnEnable()
        {
            originalTransform = new Pose(transform.localPosition, transform.localRotation);
        }

        private void OnDisable()
        {
            transform.localPosition = originalTransform.position;
            transform.localRotation = originalTransform.rotation;
        }

        private void LateUpdate()
        {
            // If the position/pose action on the controller is unbound, we need
            // to polyfill with a fake pointing pose. (For now, until we have
            // universal hand interaction profiles.)
            if (controller != null && (controller.positionAction.action?.controls.Count ?? 0) <= 0)
            {
                if (handsAggregator == null)
                {
                    handsAggregator = HandsUtils.GetSubsystem();
                }

                if (handsAggregator == null || !handsAggregator.TryGetEntireHand(controller.HandNode, out IReadOnlyList<HandJointPose> joints))
                {
                    ResetIfDirty();
                    return;
                }

                // Tick the hand ray generator function. Uses index knuckle for position.
                HandJointPose knuckle = joints[(int)TrackedHandJoint.IndexProximal];
                HandJointPose palm = joints[(int)TrackedHandJoint.Palm];
                handRay.Update(knuckle.Position, -palm.Up, CameraCache.Main.transform, controller.HandNode.ToHandedness());
                
                Ray ray = handRay.Ray;
                transform.SetPositionAndRotation(ray.origin, Quaternion.LookRotation(ray.direction, palm.Up));
                
                // Remember that we modified the transform. If the action becomes bound later on,
                // say, if motion controllers are suddenly connected, we will reset the transform
                // back to where we found it.
                transformDirty = true;
            }
            else
            {   
                ResetIfDirty();
            }
        }

        private void ResetIfDirty()
        {
            // Reset the transform back to where we found it.
            if (transformDirty)
            {
                transform.localPosition = originalTransform.position;
                transform.localRotation = originalTransform.rotation;

                // But don't keep doing it.
                transformDirty = false;
            }
        }
    }
}