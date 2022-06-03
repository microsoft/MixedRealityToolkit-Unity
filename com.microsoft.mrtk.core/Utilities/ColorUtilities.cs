// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit
{
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

        public static Gradient GradientLerpNoAlpha(Gradient a, Gradient b, float t)
        {
            return GradientLerp(a, b, t, true, false);
        }

        public static Gradient GradientLerpNoColor(Gradient a, Gradient b, float t)
        {
            return GradientLerp(a, b, t, false, true);
        }

        // Caching the key times to not create a new HashSet every time this is called.
        static HashSet<float> cachedKeyTimes = new HashSet<float>();
        static Gradient GradientLerp(Gradient a, Gradient b, float t, bool noAlpha, bool noColor)
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