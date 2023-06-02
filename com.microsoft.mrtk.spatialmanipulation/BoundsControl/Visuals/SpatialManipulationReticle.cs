using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    public class SpatialManipulationReticle : MonoBehaviour
    {

        [SerializeField]
        [Tooltip("The root of the reticle visuals")]
        private SpatialManipulationReticleType reticleType;

        private Quaternion worldRotationCache;

        // Update is called once per frame
        public void UpdateReticle(Vector3 reticleNormal, Transform hitTargetTransform)
        {
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
                Vector3 adjustedCursorPos = transform.position - contextTransform.position;

                switch (reticleType)
                {
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
            worldRotationCache = transform.rotation;
        }

        public void UpdateRotation()
        {
            if (worldRotationCache != null)
            {
                transform.rotation = worldRotationCache;
            }
        }

        /// <summary>
        /// Gets three axes where the forward is as close to the provided normal as
        /// possible but where the axes are aligned to the TargetObject's transform
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
