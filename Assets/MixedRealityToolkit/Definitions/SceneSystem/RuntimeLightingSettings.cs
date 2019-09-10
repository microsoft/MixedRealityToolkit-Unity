// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// A struct that mimics the lighting settings stored in a scene.
    /// Used to store, retrieve and interpolate lighting settings.
    /// Omits any editor-only settings.
    /// </summary>
    [Serializable]
    public struct RuntimeLightingSettings
    {
        public float BounceScale;
        public float IndirectOutputScale;
        public float AlbedoBoost;
        public MixedLightingMode EnvironmentLightingMode;
        public bool EnableBakedLightmaps;
        public bool EnabledRealtimeLightmaps;

        /// <summary>
        /// Lerps between two settings
        /// </summary>
        /// <param name="t">Value from 0 to 1</param>
        public static RuntimeLightingSettings Lerp(RuntimeLightingSettings from, RuntimeLightingSettings to, float t)
        {
            bool notStarted             = t <= 0;
            to.AlbedoBoost              = Mathf.Lerp(from.AlbedoBoost, to.AlbedoBoost, t);
            to.BounceScale              = Mathf.Lerp(from.BounceScale, to.BounceScale, t);
            to.EnableBakedLightmaps     = notStarted ? from.EnableBakedLightmaps : to.EnableBakedLightmaps;
            to.EnabledRealtimeLightmaps = notStarted ? from.EnabledRealtimeLightmaps : to.EnabledRealtimeLightmaps;
            to.EnvironmentLightingMode  = notStarted ? from.EnvironmentLightingMode : to.EnvironmentLightingMode;
            to.IndirectOutputScale      = Mathf.Lerp(from.IndirectOutputScale, to.IndirectOutputScale, t);
            return to;
        }

        /// <summary>
        /// Sets continuous settings to 'black' without changing any discrete features.
        /// </summary>
        public static RuntimeLightingSettings Black(RuntimeLightingSettings source)
        {
            source.AlbedoBoost          = 0;
            source.BounceScale          = 0;
            source.IndirectOutputScale  = 0;
            return source;
        }
    }
}