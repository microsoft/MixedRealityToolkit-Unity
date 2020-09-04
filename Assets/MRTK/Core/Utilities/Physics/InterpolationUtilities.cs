// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Physics
{
    /// <summary>
    /// Static class containing interpolation-related utility functions.
    /// </summary>
    public static class InterpolationUtilities
    {
        #region Exponential Decay

        public static float ExpDecay(float from, float to, float hLife, float dTime)
        {
            return Mathf.Lerp(from, to, ExpCoefficient(hLife, dTime));
        }

        public static Vector2 ExpDecay(Vector2 from, Vector2 to, float hLife, float dTime)
        {
            return Vector2.Lerp(from, to, ExpCoefficient(hLife, dTime));
        }

        public static Vector3 ExpDecay(Vector3 from, Vector3 to, float hLife, float dTime)
        {
            return Vector3.Lerp(from, to, ExpCoefficient(hLife, dTime));
        }

        public static Quaternion ExpDecay(Quaternion from, Quaternion to, float hLife, float dTime)
        {
            return Quaternion.Slerp(from, to, ExpCoefficient(hLife, dTime));
        }

        public static Color ExpDecay(Color from, Color to, float hLife, float dTime)
        {
            return Color.Lerp(from, to, ExpCoefficient(hLife, dTime));
        }


        /// <summary>
        /// Computes an exponential coefficient following the given formula: 1 - .5^(dTime/hLife)
        /// </summary>
        public static float ExpCoefficient(float hLife, float dTime)
        {
            if (hLife == 0)
            {
                return 1;
            }

            return 1.0f - Mathf.Pow(0.5f, dTime / hLife);
        }

        #endregion
    }
}