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
    }
}