// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
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
    public class StabilizedRay
    {
        /// <summary>
        /// Half life used for position decay calculations.
        /// </summary>
        public float HalfLifePosition { get; } = 0.1f;

        /// <summary>
        /// Half life used for velocity decay calculations.
        /// </summary>
        public float HalfLifeDirection { get; } = 0.1f;

        /// <summary>
        /// Computed Stabilized position.
        /// </summary>
        public Vector3 StabilizedPosition { get; private set; }

        /// <summary>
        /// Computed stabilized direction.
        /// </summary>
        public Vector3 StabilizedDirection { get; private set; }

        private bool isInitialized = false;

        /// <summary>
        /// HalfLife closer to zero means lerp closer to one.
        /// </summary>
        public StabilizedRay(float halfLife)
        {
            HalfLifePosition = halfLife;
            HalfLifeDirection = halfLife;
        }

        /// <summary>
        /// Add sample to ray stabilizer.
        /// </summary>
        /// <param name="ray">New Sample used to update stabilized ray.</param>
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

        /// <summary>
        /// Compute dynamic exponential coefficient.
        /// </summary>
        /// <param name="hLife">Half life</param>
        /// <param name="delta">Distance delta</param>
        /// <returns>The dynamic exponential coefficient.</returns>
        public static float DynamicExpCoefficient(float hLife, float delta)
        {
            if (hLife == 0)
            {
                return 1;
            }

            return 1.0f - Mathf.Pow(0.5f, delta / hLife);
        }

        /// <summary>
        /// Compute stabilized vector3 given a previously stabilized value, and a new sample, given a half life.
        /// </summary>
        /// <param name="from">Previous stabilized Vector3.</param>
        /// <param name="to">New Vector3 sample.</param>
        /// <param name="hLife">Half life used for stabilization.</param>
        /// <returns>Stabilized Vector 3.</returns>
        public static Vector3 DynamicExpDecay(Vector3 from, Vector3 to, float hLife)
        {
            return Vector3.Lerp(from, to, DynamicExpCoefficient(hLife, Vector3.Distance(to, from)));
        }
    }
}