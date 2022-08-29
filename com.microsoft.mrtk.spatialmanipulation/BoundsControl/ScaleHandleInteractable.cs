// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SpatialManipulation
{
    /// <summary>
    /// A specialized bounds handle intended to be a scale affordance, placed
    /// at the corners of a bounding box.
    /// This handle supports occlusion + reorientation to maintain
    /// a particular flattened/occluded visual look + feel.
    /// </summary>
    [AddComponentMenu("MRTK/Spatial Manipulation/Scale Handle Interactable")]
    internal class ScaleHandleInteractable : BoundsHandleInteractable
    {
        #region Private Fields

        private Vector3 originalLocalForward;

        private Vector3 originalLocalUp;

        private Vector3 originalLocalRight;

        #endregion Private Fields

        #region Monobehaviour Methods

        private void OnValidate()
        {
            HandleType = HandleType.Scale;
        }

        protected override void Awake()
        {
            base.Awake();
            OnValidate();

            originalLocalForward = transform.parent.InverseTransformDirection(transform.forward);
            originalLocalUp = transform.parent.InverseTransformDirection(transform.up);
            originalLocalRight = transform.parent.InverseTransformDirection(transform.right);
        }

        protected override void LateUpdate()
        {
            if (!IsOccluded)
            {
                // Take our original axes (in local space) and transform to global coords.
                // We will use these as the "reference axes" (i.e., the known-good axes configuration). This assumes
                // that the handles are set up originally (in a prefab) where the "missing lobe" of a two-lobe handle is on Z.
                Vector3 originalForwardGlobal = transform.parent.TransformVector(originalLocalForward);
                Vector3 originalUpGlobal = transform.parent.TransformVector(originalLocalUp);
                Vector3 originalRightGlobal = transform.parent.TransformVector(originalLocalRight);

                if (IsFlattened)
                {
                    Vector3 globalFlatten = transform.parent.TransformDirection(FlattenVector);
                    float forwardDot = Vector3.Dot(originalForwardGlobal.normalized, globalFlatten);
                    float upDot = Vector3.Dot(originalUpGlobal.normalized, globalFlatten);
                    float rightDot = Vector3.Dot(originalRightGlobal.normalized, globalFlatten);

                    if (Mathf.Abs(rightDot) > 0.01f)
                    {
                        transform.rotation = Quaternion.LookRotation(originalRightGlobal, originalForwardGlobal);
                    }
                    else if (Mathf.Abs(upDot) > 0.01f)
                    {
                        transform.rotation = Quaternion.LookRotation(originalUpGlobal, originalRightGlobal);
                    }
                    else if (Mathf.Abs(forwardDot) > 0.01f)
                    {
                        transform.rotation = Quaternion.LookRotation(originalForwardGlobal, originalUpGlobal);
                    }
                }
                else
                {
                    // Then, we take these axes and project them onto screen coordinates.
                    Vector2 originalForwardProjected = ProjectToScreen(transform.position, originalForwardGlobal);
                    Vector2 originalUpProjected = ProjectToScreen(transform.position, originalUpGlobal);
                    Vector2 originalRightProjected = ProjectToScreen(transform.position, originalRightGlobal);

                    float xyAngle = Vector2.SignedAngle(originalRightProjected, originalUpProjected);
                    float xzAngle = Vector2.SignedAngle(originalRightProjected, originalForwardProjected);
                    float yzAngle = Vector2.SignedAngle(originalUpProjected, originalForwardProjected);

                    if (Mathf.Sign(xyAngle) != Mathf.Sign(xzAngle))
                    {
                        // x is middle
                        transform.rotation = Quaternion.LookRotation(originalRightGlobal, originalForwardGlobal);
                    }
                    else if (Mathf.Sign(-xyAngle) != Mathf.Sign(yzAngle))
                    {
                        // y is middle
                        transform.rotation = Quaternion.LookRotation(originalUpGlobal, originalRightGlobal);
                    }
                    else
                    {
                        // z is middle
                        transform.rotation = Quaternion.LookRotation(originalForwardGlobal, originalUpGlobal);
                    }
                }
            }

            base.LateUpdate();
        }

        #endregion Monobehaviour Methods

        #region Private Helpers

        private Vector2 ProjectToScreen(Vector3 worldPoint, Vector3 worldVector)
        {
            return Camera.main.WorldToScreenPoint(worldPoint + worldVector) - Camera.main.WorldToScreenPoint(worldPoint);
        }

        #endregion Private Helpers
    }
}