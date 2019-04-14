// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    public class HandRay
    {
        public Ray Ray
        {
            get
            {
                ray.origin = stabilizedRay.StabilizedPosition;
                ray.direction = stabilizedRay.StabilizedDirection;
                return ray;
            }
        }

        public bool ShouldShowRay
        {
            get
            {
                if(headForward.magnitude < Mathf.Epsilon)
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

        // Constants from Shell Implementation of hand ray
        private const float DynamicPivotBaseY = -0.1f, DynamicPivotMultiplierY = 0.65f, DynamicPivotMinY = -0.6f, DynamicPivotMaxY = -0.2f;
        private const float DynamicPivotBaseX = 0.03f, DynamicPivotMultiplierX = 0.65f, DynamicPivotMinX = 0.08f, DynamicPivotMaxX = 0.15f;
        private const float HeadToPivotOffsetZ = 0.08f;
        private readonly float CursorBeamBackwardTolerance = 0.5f;
        private readonly float CursorBeamUpTolerance = 0.8f;


        // Smoothing factor for ray stabilization
        private const float StabilizedRayHalfLife = 0.01f;

        private StabilizedRay stabilizedRay = new StabilizedRay(StabilizedRayHalfLife);
        private Vector3 palmNormal;
        private Vector3 headForward;

        #region Public Methods

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

            float xBase = DynamicPivotBaseX;
            float xMultiplier = DynamicPivotMultiplierX;
            float xMin = DynamicPivotMinX;
            float xMax = DynamicPivotMaxX;
            if (sourceHandedness == Handedness.Left)
            {
                xBase = -xBase;
                float tmp = xMin;
                xMin = -xMax;
                xMax = tmp;
            }
            float relativePivotX = xBase + xMultiplier * handPositionHeadSpace.x;
            relativePivotX = Mathf.Clamp(relativePivotX, xMin, xMax);

            Vector3 relativePivot = new Vector3(
                    relativePivotX,
                    relativePivotY,
                    HeadToPivotOffsetZ
                );

            Quaternion headRotationFlat = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y, 0);
            return headTransform.position + headRotationFlat * relativePivot;
        }
    }

    /// <summary>
    /// A ray whose position and direction are stabilized in a way similar to how gaze stabilization
    /// works in HoloLens.
    /// 
    /// The ray uses Anatolie Gavrulic's "DynamicExpDecay" filter to stabilize the ray
    /// this filter adjusts its smoothing factor based on the velocity of the filtered object
    /// 
    /// The formula is
    ///  Y_smooted += ∆𝑌_𝑟
    ///  where
    /// 〖∆𝑌_𝑟=∆𝑌∗〖0.5〗^(∆𝑌/〖Halflife〗)
    /// 
    /// In code, LERP(signal, oldValue, POW(0.5, ABS(signal – oldValue) / hl)
    /// </summary>
    internal class StabilizedRay
    {
        public float HalfLifePosition { get; } = 0.1f;

        public float HalfLifeDirection { get; } = 0.1f;

        public Vector3 StabilizedPosition { get; private set; }

        public Vector3 StabilizedDirection { get; private set; }

        public Quaternion StabilizedRotation => Mathf.Abs(StabilizedDirection.magnitude) < 1e-5f ?
            Quaternion.identity : Quaternion.LookRotation(StabilizedDirection);

        private bool isInitialized = false;

        /// <summary>
        /// HalfLife closer to zero means lerp closer to one
        /// </summary>
        /// <param name="halfLife"></param>
        public StabilizedRay(float halfLife)
        {
            HalfLifePosition = halfLife;
            HalfLifeDirection = halfLife;
        }

        public StabilizedRay(float halfLifePos, float halfLifeDir)
        {
            HalfLifePosition = halfLifePos;
            HalfLifeDirection = halfLifeDir;
        }

        public void AddSample(Ray ray)
        {
            if (!isInitialized)
            {
                StabilizedPosition = ray.origin;
                StabilizedDirection = ray.direction.normalized;
                isInitialized = true;
            }
            else
            {
                StabilizedPosition = DynamicExpDecay(StabilizedPosition, ray.origin, HalfLifePosition);
                StabilizedDirection = DynamicExpDecay(StabilizedDirection, ray.direction.normalized, HalfLifeDirection);
            }
        }


        public static float DynamicExpCoefficient(float hLife, float delta)
        {
            if (hLife == 0)
            {
                return 1;
            }

            return 1.0f - Mathf.Pow(0.5f, delta / hLife);
        }

        public static Vector3 DynamicExpDecay(Vector3 from, Vector3 to, float hLife)
        {
            return Vector3.Lerp(from, to, DynamicExpCoefficient(hLife, Vector3.Distance(to, from)));
        }
    }
}