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

        public static RuntimeLightingSettings Lerp(RuntimeLightingSettings from, RuntimeLightingSettings to, float t)
        {
            bool firstHalf = t < 0.5f;

            to.AlbedoBoost = Mathf.Lerp(from.AlbedoBoost, to.AlbedoBoost, t);
            to.BounceScale = Mathf.Lerp(from.BounceScale, to.BounceScale, t);
            to.EnableBakedLightmaps = firstHalf ? from.EnableBakedLightmaps : to.EnableBakedLightmaps;
            to.EnabledRealtimeLightmaps = firstHalf ? from.EnabledRealtimeLightmaps : to.EnabledRealtimeLightmaps;
            to.EnvironmentLightingMode = firstHalf ? from.EnvironmentLightingMode : to.EnvironmentLightingMode;
            to.IndirectOutputScale = Mathf.Lerp(from.IndirectOutputScale, to.IndirectOutputScale, t);

            return to;
        }
    }
}