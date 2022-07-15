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
            originalTransform = new Pose(transform.localPosition, transform.localRotation);
        }

        private void OnEnable()
        {
            Application.onBeforeRender += OnBeforeRender;
        }

        private void OnDisable()
        {
            Application.onBeforeRender -= OnBeforeRender;
            transform.localPosition = originalTransform.position;
            transform.localRotation = originalTransform.rotation;
        }

        private void OnBeforeRender()
        {
            // If the position/pose action on the controller is unbound, we need
            // to polyfill with a fake pointing pose. (For now, until we have
            // universal hand interaction profiles.)
            if (controller?.positionAction.action?.activeControl?.device == null)
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

                // Tick the hand ray generator function.
                HandJointPose palm = joints[(int)TrackedHandJoint.Palm];
                handRay.Update(palm.Position, -palm.Up, CameraCache.Main.transform, controller.HandNode.ToHandedness());
                
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