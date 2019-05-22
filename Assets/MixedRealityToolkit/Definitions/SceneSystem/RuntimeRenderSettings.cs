// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.SceneSystem
{
    /// <summary>
    /// A struct that mimics the render settings stored in a scene.
    /// Used to store, retrieve and interpolate render settings.
    /// Omits any editor-only settings, as well as some settings that are seldom used.
    /// </summary>
    [Serializable]
    public struct RuntimeRenderSettings
    {
        public bool Fog;
        public Color FogColor;
        public FogMode FogMode;
        public float FogDensity;
        public float LinearFogStart;
        public float LinearFogEnd;
        public Color AmbientSkyColor;
        public Color AmbientEquatorColor;
        public Color AmbientGroundColor;
        public float AmbientIntensity;
        public int AmbientMode;
        public Color SubtractiveShadowColor;
        public Material SkyboxMaterial;
        public DefaultReflectionMode DefaultReflectionMode;
        public int DefaultReflectionResolution;
        public int ReflectionBounces;
        public float ReflectionIntensity;
        public Cubemap CustomReflection;
        public Light Sun;
        public bool UseRadianceAmbientProbe;
    }
}