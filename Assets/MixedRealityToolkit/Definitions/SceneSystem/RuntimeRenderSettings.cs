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
        public Color AmbientLight;
        public float AmbientIntensity;
        public int AmbientMode;
        public Color SubtractiveShadowColor;
        public Material SkyboxMaterial;
        public DefaultReflectionMode DefaultReflectionMode;
        public int DefaultReflectionResolution;
        public int ReflectionBounces;
        public float ReflectionIntensity;
        public Cubemap CustomReflection;
        public bool UseRadianceAmbientProbe;

        /// <summary>
        /// Lerps between two settings
        /// </summary>
        /// <param name="t">Value from 0 to 1</param>
        public static RuntimeRenderSettings Lerp(RuntimeRenderSettings from, RuntimeRenderSettings to, float t)
        {
            bool notStarted                 = t <= 0;
            to.AmbientEquatorColor          = Color.Lerp(from.AmbientEquatorColor, to.AmbientEquatorColor, t);
            to.AmbientGroundColor           = Color.Lerp(from.AmbientGroundColor, to.AmbientGroundColor, t);
            to.AmbientIntensity             = Mathf.Lerp(from.AmbientIntensity, to.AmbientIntensity, t);
            to.AmbientLight                 = Color.Lerp(from.AmbientLight, to.AmbientLight, t);
            to.AmbientMode                  = notStarted ? from.AmbientMode : to.AmbientMode;
            to.AmbientSkyColor              = Color.Lerp(from.AmbientSkyColor, to.AmbientSkyColor, t);
            to.CustomReflection             = notStarted ? from.CustomReflection : to.CustomReflection;
            to.DefaultReflectionMode        = notStarted ? from.DefaultReflectionMode : to.DefaultReflectionMode;
            to.DefaultReflectionResolution  = notStarted ? from.DefaultReflectionResolution : to.DefaultReflectionResolution;
            to.Fog                          = notStarted ? from.Fog : to.Fog;
            to.FogColor                     = Color.Lerp(from.FogColor, to.FogColor, t);
            to.FogDensity                   = Mathf.Lerp(from.FogDensity, to.FogDensity, t);
            to.FogMode                      = notStarted ? from.FogMode : to.FogMode;
            to.LinearFogEnd                 = Mathf.Lerp(from.LinearFogEnd, to.LinearFogEnd, t);
            to.LinearFogStart               = Mathf.Lerp(from.LinearFogStart, to.LinearFogStart, t);
            to.ReflectionBounces            = notStarted ? from.ReflectionBounces : to.ReflectionBounces;
            to.ReflectionIntensity          = Mathf.Lerp(from.ReflectionIntensity, to.ReflectionIntensity, t);
            to.SkyboxMaterial               = notStarted ? from.SkyboxMaterial : to.SkyboxMaterial;
            to.SubtractiveShadowColor       = Color.Lerp(from.SubtractiveShadowColor, to.SubtractiveShadowColor, t);
            to.UseRadianceAmbientProbe      = notStarted ? from.UseRadianceAmbientProbe : to.UseRadianceAmbientProbe;
            return to;
        }


        /// <summary>
        /// Sets continuous settings to 'black' without changing any discrete features.
        /// </summary>
        public static RuntimeRenderSettings Black(RuntimeRenderSettings source)
        {
            source.AmbientEquatorColor      = Color.clear;
            source.AmbientGroundColor       = Color.clear;
            source.AmbientIntensity         = 0;
            source.AmbientLight             = Color.clear;
            source.AmbientSkyColor          = Color.clear;
            source.FogColor                 = Color.clear;
            source.FogDensity               = 0;
            source.LinearFogEnd             = 0;
            source.LinearFogStart           = 0;
            source.ReflectionIntensity      = 0;
            source.SubtractiveShadowColor   = Color.clear;
            return source;
        }
    }
}