
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI.BoundingBoxTypes;

namespace Microsoft.MixedReality.Toolkit.UI
{
    class BoundingBoxTarget
    {

        public BoundingBoxTarget(GameObject target)
        {
            this.targetObject = target;
        }

        private GameObject targetObject;

        public GameObject Target
        {
            get
            {
                //if (targetObject == null)
                //{
                //    targetObject = gameObject;
                //}

                return targetObject;
            }
        }

        /// <summary>
        /// The collider reference tracking the bounds utilized by this component during runtime
        /// </summary>
        public BoxCollider TargetBounds { get; private set; }

        public bool HasChanged()
        {
            return Target.transform.hasChanged;
        }

        public void ClearChanged()
        {
            Target.transform.hasChanged = false;
        }


        public void TransformTarget(HandleType transformType)
        {
            if (transformType != HandleType.None)
            {
                Vector3 prevGrabPoint = currentGrabPoint;
                currentGrabPoint = (currentPointer.Rotation * grabPointInPointer) + currentPointer.Position;

                if (transformType == HandleType.Rotation)
                {
                    Vector3 prevDir = Vector3.ProjectOnPlane(prevGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Vector3 currentDir = Vector3.ProjectOnPlane(currentGrabPoint - rigRoot.transform.position, currentRotationAxis).normalized;
                    Quaternion q = Quaternion.FromToRotation(prevDir, currentDir);
                    q.ToAngleAxis(out float angle, out Vector3 axis);

                    Target.transform.RotateAround(rigRoot.transform.position, axis, angle);
                }
                else if (transformType == HandleType.Scale)
                {
                    float initialDist = Vector3.Dot(initialGrabPoint - oppositeCorner, diagonalDir);
                    float currentDist = Vector3.Dot(currentGrabPoint - oppositeCorner, diagonalDir);
                    float scaleFactor = 1 + (currentDist - initialDist) / initialDist;

                    Vector3 newScale = initialScaleOnGrabStart * scaleFactor;
                    Vector3 clampedScale = newScale;
                    if (scaleHandler != null)
                    {
                        clampedScale = scaleHandler.ClampScale(newScale);
                        if (clampedScale != newScale)
                        {
                            scaleFactor = clampedScale[0] / initialScaleOnGrabStart[0];
                        }
                    }

                    Target.transform.localScale = clampedScale;
                    Target.transform.position = initialPositionOnGrabStart * scaleFactor + (1 - scaleFactor) * oppositeCorner;
                }
            }
        }
    }
}
