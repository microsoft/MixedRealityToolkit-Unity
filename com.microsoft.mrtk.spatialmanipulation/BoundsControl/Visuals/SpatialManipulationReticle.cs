// Copyright(c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// A reticle used to visualize spatial manipulation capabilities when hovering over a bounding box handle.
    /// The reticle is oriented in relation to the bounding box, to indicate the direction for rotation or scaling.
    /// </summary>
    public class SpatialManipulationReticle : MonoBehaviour, IVariableReticle
    {
        /// <summary>
        /// The type of the reticle visuals. Scale or Rotate.
        /// </summary>
        [field: SerializeField, Tooltip("The type of the reticle visuals. Scale or Rotate.")]
        public SpatialManipulationReticleType ReticleType { get; set; }

        private Transform contextTransform;
        private Quaternion worldRotationCache;

        /// <summary>
        /// Called by once per frame by <see cref="MRTKRayReticleVisual"/> from its UpdateReticle.
        /// Rotates the cursor reticle based on the hovered or selected handle's position relative to the box visuals. 
        /// </summary>
        public void UpdateVisuals(VariableReticleUpdateArgs args)
        {
            if (args.Interactor is XRRayInteractor rayInteractor)
            {
                if (args.ReticleNormal != Vector3.zero)
                {
                    transform.SetPositionAndRotation(args.ReticlePosition, Quaternion.LookRotation(args.ReticleNormal, Vector3.up));
                }
                else
                {
                    transform.position = args.ReticlePosition;
                }

                if (rayInteractor.interactablesSelected.Count > 0)
                {
                    FixedRotateReticle();
                }
                else if (rayInteractor.interactablesHovered.Count > 0)
                {
                    RotateReticle(args.ReticleNormal, rayInteractor.interactablesHovered[0].transform);
                }
            }
        }

        /// <summary>
        /// Rotates the cursor reticle based on the hovered or selected handle's position relative to the box visuals. 
        /// </summary>
        private void RotateReticle(Vector3 reticleNormal, Transform hitTargetTransform)
        {
            if (hitTargetTransform == null)
            {
                return;
            }

            Vector3 right = Vector3.zero;
            Vector3 up = Vector3.zero;
            Vector3 forward = Vector3.zero;
            GetCursorTargetAxes(reticleNormal, ref right, ref up, ref forward, hitTargetTransform.parent);

            // Get the cursor position, relative to the handles container
            Vector3 adjustedCursorPos = transform.position - hitTargetTransform.parent.position;

            switch (ReticleType)
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
            
            // Cache the world rotation 
            worldRotationCache = transform.rotation;
        }

        /// <summary>
        /// Rotates the cursor reticle based on the last stored value to maintain a fixed rotation. 
        /// </summary>
        private void FixedRotateReticle()
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

    /// <summary>
    /// The type of manipulation being visualized: rotation or scaling.
    /// </summary>
    public enum SpatialManipulationReticleType
    {
        /// <summary>
        /// No valid type has been set.
        /// </summary>
        Unknown,

        /// <summary>
        /// The reticle indicates direction for one of the scaling handles on the corner of the bounding box.
        /// </summary>
        Scale,

        /// <summary>
        /// The reticle indicates direction for one of the rotation handles on the side of the bounding box.
        /// </summary>
        Rotate
    }
}
