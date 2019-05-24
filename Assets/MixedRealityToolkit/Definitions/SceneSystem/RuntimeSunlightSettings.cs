// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// Struct for storing directional sunlight settings stored in a scene.
    /// </summary>
    [Serializable]
    public struct RuntimeSunlightSettings
    {
        public bool UseSunlight;
        public Color Color;
        public float Intensity;
        public float XRotation;
        public float YRotation;
        public float ZRotation;

        public static RuntimeSunlightSettings Lerp(RuntimeSunlightSettings from, RuntimeSunlightSettings to, float t)
        {
            bool firstHalf = t < 0.5f;

            to.Color = Color.Lerp(from.Color, to.Color, t);
            to.Intensity = Mathf.Lerp(from.Intensity, to.Intensity, t);
            to.XRotation = Mathf.Lerp(from.XRotation, to.XRotation, t);
            to.YRotation = Mathf.Lerp(from.YRotation, to.YRotation, t);
            to.ZRotation = Mathf.Lerp(from.ZRotation, to.ZRotation, t);
            to.UseSunlight = firstHalf ? from.UseSunlight : to.UseSunlight;

            return to;
        }
    }
}