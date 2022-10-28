// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HandRay : IHandRay
    {
        /// <summary>
        /// Constructs the hand ray generator with an optional
        /// customized half-life value for the smoothing/filtering function.
        /// A smaller half-life results in less smoothing.
        /// </summary>
        public HandRay(float halfLife = 0.01f)
        {
            stabilizedRayHalfLife = halfLife;
            stabilizedRay = new StabilizedRay(stabilizedRayHalfLife);
        }

        /// <inheritdoc />
        public Ray Ray
        {
            get
            {
                ray.origin = stabilizedRay.StabilizedPosition;
                ray.direction = stabilizedRay.StabilizedDirection;
                return ray;
            }
        }

        /// <inheritdoc />
        public bool ShouldShowRay
        {
            get
            {
                if (headForward.magnitude < Mathf.Epsilon)
                {
                    return false;
                }
                bool valid = true;
                if (CursorBeamBackwardTolerance >= 0)
                {
                    Vector3 cameraBackward = -headForward;
                    if (Vector3.Dot(palmNormal.normalized, cameraBackward) > CursorBeamBackwardTolerance)
                    {
                        valid = false;
                    }
                }
                if (valid && CursorBeamUpTolerance >= 0)
                {
                    if (Vector3.Dot(palmNormal, Vector3.up) > CursorBeamUpTolerance)
                    {
                        valid = false;
                    }
                }

                return valid;
            }
        }

        private Ray ray = new Ray();

        // Constants from Shell Implementation of hand ray.
        private const float DynamicPivotBaseY = -0.1f, DynamicPivotMultiplierY = 0.65f, DynamicPivotMinY = -0.6f, DynamicPivotMaxY = -0.2f;
        private const float DynamicPivotBaseX = 0.03f, DynamicPivotMultiplierX = 0.65f, DynamicPivotMinX = 0.08f, DynamicPivotMaxX = 0.15f;
        private const float HeadToPivotOffsetZ = 0.08f;
        private const float CursorBeamBackwardTolerance = 0.5f;
        private const float CursorBeamUpTolerance = 0.8f;

        // Smoothing factor for ray stabilization.
        private readonly float stabilizedRayHalfLife = 0.01f;

        private readonly StabilizedRay stabilizedRay;
        private Vector3 palmNormal;
        private Vector3 headForward;

        #region Public Methods

        /// <inheritdoc />
        public void Update(Vector3 handPosition, Vector3 palmNormal, Transform headTransform, Handedness sourceHandedness)
        {
            Vector3 rayPivotPoint = ComputeRayPivotPosition(handPosition, headTransform, sourceHandedness);
            Vector3 measuredRayPosition = handPosition;
            Vector3 measuredDirection = measuredRayPosition - rayPivotPoint;
            this.palmNormal = palmNormal;
            this.headForward = headTransform.forward;

            stabilizedRay.AddSample(new Ray(measuredRayPosition, measuredDirection));
        }

        #endregion

        private Vector3 ComputeRayPivotPosition(Vector3 handPosition, Transform headTransform, Handedness sourceHandedness)
        {
            Vector3 handPositionHeadSpace = headTransform.InverseTransformPoint(handPosition);
            float relativePivotY = DynamicPivotBaseY + Mathf.Min(DynamicPivotMultiplierY * handPositionHeadSpace.y, 0);
            relativePivotY = Mathf.Clamp(relativePivotY, DynamicPivotMinY, DynamicPivotMaxY);

            float xBase = sourceHandedness == Handedness.Right ? DynamicPivotBaseX : -DynamicPivotBaseX;
            float xMultiplier = DynamicPivotMultiplierX;
            float xMin = sourceHandedness == Handedness.Right ? DynamicPivotMinX : -DynamicPivotMaxX;
            float xMax = sourceHandedness == Handedness.Right ? DynamicPivotMaxX : -DynamicPivotMinX;
            float relativePivotX = xBase + xMultiplier * handPositionHeadSpace.x;
            relativePivotX = Mathf.Clamp(relativePivotX, xMin, xMax);

            Vector3 relativePivot = new Vector3(
                    relativePivotX,
                    relativePivotY,
                    HeadToPivotOffsetZ
                );

            return headTransform.position + headTransform.TransformVector(relativePivot);
        }
    }
}
