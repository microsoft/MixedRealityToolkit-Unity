// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// Disable "missing XML comment" warning for sample. While nice to have, this documentation is not required for samples.
#pragma warning disable CS1591


using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// This class allows changing the render mode on-the-fly. This is, for example, useful when trying to show a semi-transparent preview of an 
    /// target.
    /// </summary>
    public static class ChangeRenderMode
    {
        /// <summary>
        /// Available blend modes for rendering the material
        /// </summary>
        public enum BlendMode
        {
            /// <summary>
            /// Opaque - Is the default, and suitable for normal solid objects with no transparent areas.
            /// </summary> 
            Opaque,

            /// <summary>
            /// Cutout - Allows you to create a transparent effect that has hard edges between the opaque and transparent areas.
            /// In this mode, there are no semi-transparent areas, the texture is either 100% opaque, or invisible.
            /// This is useful when using transparency to create the shape of materials such as leaves, or cloth with holes and tatters.
            /// </summary>
            Cutout,

            /// <summary>
            /// Fade - Allows the transparency values to entirely fade an object out, including any specular highlights
            /// or reflections it may have.This mode is useful if you want to animate an object fading in or out.
            /// It is not suitable for rendering realistic transparent materials such as clear plastic or glass because the reflections
            /// and highlights will also be faded out.
            /// </summary>
            Fade,

            /// <summary>
            /// Transparent - Suitable for rendering realistic transparent materials such as clear plastic or glass.
            /// In this mode, the material itself will take on transparency values (based on the texture’s alpha channel
            /// and the alpha of the tint colour), however reflections and lighting highlights will remain visible at full clarity
            /// as is the case with real transparent materials.
            /// </summary>
            Transparent
        }
        public static void ChangeRenderModes(Material standardShaderMaterial, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    standardShaderMaterial.SetInt("_ZWrite", 1);
                    standardShaderMaterial.EnableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 2450;
                    break;
                case BlendMode.Fade:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
                case BlendMode.Transparent:
                    standardShaderMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    standardShaderMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    standardShaderMaterial.SetInt("_ZWrite", 0);
                    standardShaderMaterial.DisableKeyword("_ALPHATEST_ON");
                    standardShaderMaterial.DisableKeyword("_ALPHABLEND_ON");
                    standardShaderMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    standardShaderMaterial.renderQueue = 3000;
                    break;
            }
        }
    }
}

#pragma warning restore CS1591
