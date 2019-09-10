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

        /// <summary>
        /// Lerps between two settings
        /// </summary>
        /// <param name="t">Value from 0 to 1</param>
        public static RuntimeSunlightSettings Lerp(RuntimeSunlightSettings from, RuntimeSunlightSettings to, float t)
        {
            bool notStarted     = t <= 0;
            to.Color            = Color.Lerp(from.Color, to.Color, t);
            to.Intensity        = Mathf.Lerp(from.Intensity, to.Intensity, t);
            to.XRotation        = Mathf.Lerp(from.XRotation, to.XRotation, t);
            to.YRotation        = Mathf.Lerp(from.YRotation, to.YRotation, t);
            to.ZRotation        = Mathf.Lerp(from.ZRotation, to.ZRotation, t);
            to.UseSunlight      = notStarted ? from.UseSunlight : to.UseSunlight;
            return to;
        }

        /// <summary>
        /// Sets continuous settings to 'black' without changing any discrete features.
        /// </summary>
        public static RuntimeSunlightSettings Black(RuntimeSunlightSettings source)
        {
            source.Color        = Color.clear;
            source.Intensity    = 0;

            return source;
        }
    }
}