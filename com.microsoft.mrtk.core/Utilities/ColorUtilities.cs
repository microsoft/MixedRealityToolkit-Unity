// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
    /// <summary>
    /// Utilities for manipulating and interpolating between colors and gradients.
    /// </summary>
    public static class ColorUtilities
    {
        /// <summary>
        /// Linearly interpolates between gradients a and b by t
        /// </summary>
        /// <remarks>Taken from https://forum.unity.com/threads/lerp-from-one-gradient-to-another.342561/ </remarks>
        /// <returns>The linearly interpolated gradient</returns>
        public static Gradient GradientLerp(Gradient a, Gradient b, float t)
        {
            return GradientLerp(a, b, t, false, false);
        }

        /// <summary>
        /// Linearly interpolate between two gradients, without interpolating alpha.
        /// </summary>
        /// <remarks>Taken from https://forum.unity.com/threads/lerp-from-one-gradient-to-another.342561/ </remarks>
        /// <returns>The linearly interpolated gradient</returns>
        public static Gradient GradientLerpNoAlpha(Gradient a, Gradient b, float t)
        {
            return GradientLerp(a, b, t, true, false);
        }

        /// <summary>
        /// Linearly interpolate between two gradients using only the alpha channel.
        /// </summary>
        /// <remarks>Taken from https://forum.unity.com/threads/lerp-from-one-gradient-to-another.342561/ </remarks>
        /// <returns>The linearly interpolated gradient</returns>
        public static Gradient GradientLerpNoColor(Gradient a, Gradient b, float t)
        {
            return GradientLerp(a, b, t, false, true);
        }

        /// <summary>
        /// Compresses the gradient to the interval (p1, p2). The colors at the start and end of the result are the same as the gradient a.
        /// </summary>
        /// <param name="a">The gradient we are trying to compress.</param>
        /// <param name="p1">The starting position of the compressed gradient, where 0 &lt;= p1 &lt; p2 &lt;= 1.</param>
        /// <param name="p2">The ending position of the compressed gradient, where 0 &lt;= p1 &lt; p2 &lt;= 1.</param>
        /// <returns>A new gradient with the gradient a compressed into the interval (p1, p2).</returns>
        public static Gradient GradientCompress(Gradient a, float p1, float p2)
        {
            if (p2 <= p1)
            {
                Debug.LogError("Trying to compress the gradient with an invalid range");
                return a;
            }

            float compressionRatio = p2 - p1;
            if (p1 == 0.0f && compressionRatio == 1.0f)
            {
                return a;
            }

            // This call will alloc because .colorKeys and .alphaKeys creates copies of the underlying arrays
            GradientColorKey[] clrs = a.colorKeys;
            GradientAlphaKey[] alphas = a.alphaKeys;

            for (int i = 0; i < clrs.Length; i++)
            {
                var newTime = p1 + compressionRatio * clrs[i].time;
                clrs[i].time = newTime;
            }

            for (int i = 0; i < alphas.Length; i++)
            {
                var newTime = p1 + compressionRatio * alphas[i].time;
                alphas[i].time = newTime;
            }

            var g = new Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }

        // Caching the key times to not create a new HashSet every time this is called.
        private static HashSet<float> cachedKeyTimes = new HashSet<float>();
        private static Gradient GradientLerp(Gradient a, Gradient b, float t, bool noAlpha, bool noColor)
        {
            if (t == 0.0f)
            {
                return a;
            }

            if (t == 1.0f)
            {
                return b;
            }

            // List of all the unique key times
            cachedKeyTimes.Clear();

            if (!noColor)
            {
                for (int i = 0; i < a.colorKeys.Length; i++)
                {
                    float k = a.colorKeys[i].time;
                    if (!cachedKeyTimes.Contains(k))
                        cachedKeyTimes.Add(k);
                }

                for (int i = 0; i < b.colorKeys.Length; i++)
                {
                    float k = b.colorKeys[i].time;
                    if (!cachedKeyTimes.Contains(k))
                        cachedKeyTimes.Add(k);
                }
            }

            if (!noAlpha)
            {
                for (int i = 0; i < a.alphaKeys.Length; i++)
                {
                    float k = a.alphaKeys[i].time;
                    if (!cachedKeyTimes.Contains(k))
                        cachedKeyTimes.Add(k);
                }

                for (int i = 0; i < b.alphaKeys.Length; i++)
                {
                    float k = b.alphaKeys[i].time;
                    if (!cachedKeyTimes.Contains(k))
                        cachedKeyTimes.Add(k);
                }
            }

            GradientColorKey[] clrs = new GradientColorKey[cachedKeyTimes.Count];
            GradientAlphaKey[] alphas = new GradientAlphaKey[cachedKeyTimes.Count];
            int gradientIdx = 0;

            // Pick colors of both gradients at key times and lerp them
            foreach (float time in cachedKeyTimes)
            {
                var clr = Color.Lerp(a.Evaluate(time), b.Evaluate(time), t);
                clrs[gradientIdx] = new GradientColorKey(clr, time);
                alphas[gradientIdx] = new GradientAlphaKey(clr.a, time);
                gradientIdx++;
            }

            var g = new Gradient();
            g.SetKeys(clrs, alphas);

            return g;
        }
    }
}
