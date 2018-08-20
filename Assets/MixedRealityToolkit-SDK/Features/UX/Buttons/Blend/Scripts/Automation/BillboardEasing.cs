// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Blend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blend.Automation
{
    public enum BillboardAxis { Y, Free}

    /// <summary>
    /// adds some easing to billboarding using BlendQuaternion
    /// </summary>
    [RequireComponent(typeof(BlendQuaternion))]
    public class BillboardEasing : MonoBehaviour
    {
        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        [Tooltip("Specifies the axis about which the object will rotate.")]
        public BillboardAxis PivotAxis = BillboardAxis.Y;

        [Tooltip("Specifies the target we will orient to. If no Target is specified the main camera will be used.")]
        public Transform TargetTransform;

        private void OnEnable()
        {
            if (TargetTransform == null)
            {
                TargetTransform = Camera.main.transform;
            }

            Update();
        }

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void Update()
        {
            if (TargetTransform == null)
            {
                return;
            }

            // Get a Vector that points from the target to the main camera.
            Vector3 directionToTarget = TargetTransform.position - transform.position;

            // Adjust for the pivot axis.
            switch (PivotAxis)
            {
                case BillboardAxis.Y:
                    directionToTarget.y = 0.0f;
                    break;

                case BillboardAxis.Free:
                default:
                    // No changes needed.
                    break;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }

            // Calculate and apply the rotation required to reorient the object
            ApplyRotation(Quaternion.LookRotation(-directionToTarget));
        }

        protected virtual void ApplyRotation(Quaternion lookRotation)
        {
            BlendQuaternion quaternion = GetComponent<BlendQuaternion>();

            if (quaternion != null)
            {
                quaternion.TargetValue = lookRotation;

                if (quaternion.LerpType == LerpTypes.Timed)
                {
                    quaternion.Play();
                }
            }
        }
    }
}
