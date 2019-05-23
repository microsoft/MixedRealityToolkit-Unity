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

        public static RuntimeRenderSettings Lerp(RuntimeRenderSettings from, RuntimeRenderSettings to, float t)
        {
            bool halfway = t > 0.5f;

            to.AmbientEquatorColor          = Color.Lerp(from.AmbientEquatorColor, to.AmbientEquatorColor, t);
            to.AmbientGroundColor           = Color.Lerp(from.AmbientGroundColor, to.AmbientGroundColor, t);
            to.AmbientIntensity             = Mathf.Lerp(from.AmbientIntensity, to.AmbientIntensity, t);
            to.AmbientLight                 = Color.Lerp(from.AmbientLight, to.AmbientLight, t);
            to.AmbientMode                  = halfway ? from.AmbientMode : to.AmbientMode;
            to.AmbientSkyColor              = Color.Lerp(from.AmbientSkyColor, to.AmbientSkyColor, t);
            to.CustomReflection             = halfway ? from.CustomReflection : to.CustomReflection;
            to.DefaultReflectionMode        = halfway ? from.DefaultReflectionMode : to.DefaultReflectionMode;
            to.DefaultReflectionResolution  = halfway ? from.DefaultReflectionResolution : to.DefaultReflectionResolution;
            to.Fog                          = halfway ? from.Fog : to.Fog;
            to.FogColor                     = Color.Lerp(from.FogColor, to.FogColor, t);
            to.FogDensity                   = Mathf.Lerp(from.FogDensity, to.FogDensity, t);
            to.FogMode                      = halfway ? from.FogMode : to.FogMode;
            to.LinearFogEnd                 = Mathf.Lerp(from.LinearFogEnd, to.LinearFogEnd, t);
            to.LinearFogStart               = Mathf.Lerp(from.LinearFogStart, to.LinearFogStart, t);
            to.ReflectionBounces            = halfway ? from.ReflectionBounces : to.ReflectionBounces;
            to.ReflectionIntensity          = Mathf.Lerp(from.ReflectionIntensity, to.ReflectionIntensity, t);
            to.SkyboxMaterial               = halfway ? from.SkyboxMaterial : to.SkyboxMaterial;
            to.SubtractiveShadowColor       = Color.Lerp(from.SubtractiveShadowColor, to.SubtractiveShadowColor, t);
            to.UseRadianceAmbientProbe      = halfway ? from.UseRadianceAmbientProbe : to.UseRadianceAmbientProbe;

            return to;
        }
    }
}