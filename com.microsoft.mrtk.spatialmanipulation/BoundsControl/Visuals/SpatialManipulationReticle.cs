//Copyright(c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    public class SpatialManipulationReticle : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("The type of the reticle visuals. Scale or Rotate.")]
        private SpatialManipulationReticleType reticleType;

        private Quaternion worldRotationCache;

        /// <summary>
        /// Called by once per frame by <see cref="MRTKRayReticleVisual"/> from its UpdateReticle.
        /// Rotates the cursor reticle based on the hovered or selected handle's position relative to the box visuals. 
        /// </summary>
        public void RotateReticle(Vector3 reticleNormal, Transform hitTargetTransform)
        {
            // After hitting a handle, find the box that the handle belongs to
            SqueezableBoxVisuals boxVisuals = hitTargetTransform.gameObject.GetComponentInParent<SqueezableBoxVisuals>(true);
            if (boxVisuals == null)
                return;

            Transform contextTransform = boxVisuals.HandlesContainer;
            if (contextTransform != null)
            {
                Vector3 right = Vector3.zero;
                Vector3 up = Vector3.zero;
                Vector3 forward = Vector3.zero;
                GetCursorTargetAxes(reticleNormal, ref right, ref up, ref forward, contextTransform);

                // Get the cursor position, relative to the handles container
                Vector3 adjustedCursorPos = transform.position - contextTransform.position;

                switch (reticleType)
                {
                    // If it is a scaling reticle, position the arrows diagonally to indicate scaling direction 
                    case SpatialManipulationReticleType.Scale:
                        {
                            if (Vector3.Dot(adjustedCursorPos, up) * Vector3.Dot(adjustedCursorPos, right) > 0) // quadrant 1 and 3
                            {
                                transform.Rotate(new Vector3(0, 0, -45f));
                            }
                            else // quadrant 2 and 4
                            {
                                transform.Rotate(new Vector3(0, 0, 45f));
                            }
                            break;
                        }
                    // If it is a rotate reticle, position the arrows horizontally or vertically
                    case SpatialManipulationReticleType.Rotate:
                        {
                            if (Math.Abs(Vector3.Dot(adjustedCursorPos, right)) <
                                Math.Abs(Vector3.Dot(adjustedCursorPos, up)))
                            {
                                transform.Rotate(new Vector3(0, 0, 90f));
                            }
                            break;
                        }
                    default: break;
                }
            }
            // Cache the world rotation 
            worldRotationCache = transform.rotation;
        }

        /// <summary>
        /// Called by once per frame by <see cref="MRTKRayReticleVisual"/> from its UpdateReticle.
        /// Rotates the cursor reticle based on the last stored value to maintain a fixed rotation. 
        /// </summary>
        public void FixedRotateReticle()
        {
            if (worldRotationCache != null)
            {
                transform.rotation = worldRotationCache;
            }
        }

        /// <summary>
        /// Gets three axes where the forward is as close to the provided normal as
        /// possible but where the axes are aligned to the TargetObject's transform.
        /// </summary>
        private bool GetCursorTargetAxes(Vector3 normal, ref Vector3 right, ref Vector3 up, ref Vector3 forward, Transform contextTransform)
        {
            Vector3 objRight = contextTransform.TransformDirection(Vector3.right);
            Vector3 objUp = contextTransform.TransformDirection(Vector3.up);
            Vector3 objForward = contextTransform.TransformDirection(Vector3.forward);

            float dotRight = Vector3.Dot(normal, objRight);
            float dotUp = Vector3.Dot(normal, objUp);
            float dotForward = Vector3.Dot(normal, objForward);

            if (Math.Abs(dotRight) > Math.Abs(dotUp) &&
                Math.Abs(dotRight) > Math.Abs(dotForward))
            {
                forward = (dotRight > 0 ? objRight : -objRight).normalized;
            }
            else if (Math.Abs(dotUp) > Math.Abs(dotForward))
            {
                forward = (dotUp > 0 ? objUp : -objUp).normalized;
            }
            else
            {
                forward = (dotForward > 0 ? objForward : -objForward).normalized;
            }

            right = Vector3.Cross(Vector3.up, forward).normalized;
            if (right == Vector3.zero)
            {
                right = Vector3.Cross(objForward, forward).normalized;
            }
            up = Vector3.Cross(forward, right).normalized;

            return true;
        }
    }

    public enum SpatialManipulationReticleType
    {
        Scale,
        Rotate
    }
}
