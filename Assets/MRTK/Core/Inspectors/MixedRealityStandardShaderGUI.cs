// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom shader inspector for the "Mixed Reality Toolkit/Standard" shader.
    /// </summary>
    public class MixedRealityStandardShaderGUI : MixedRealityShaderGUI
    {
        protected enum AlbedoAlphaMode
        {
            Transparency,
            Metallic,
            Smoothness
        }

        protected static class Styles
        {
            public static string primaryMapsTitle = "Main Maps";
            public static string renderingOptionsTitle = "Rendering Options";
            public static string advancedOptionsTitle = "Advanced Options";
            public static string fluentOptionsTitle = "Fluent Options";
            public static string stencilComparisonName = "_StencilComparison";
            public static string stencilOperationName = "_StencilOperation";
            public static string disableAlbedoMapName = "_DISABLE_ALBEDO_MAP";
            public static string albedoMapAlphaMetallicName = "_METALLIC_TEXTURE_ALBEDO_CHANNEL_A";
            public static string albedoMapAlphaSmoothnessName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A";
            public static string propertiesComponentHelp = "Use the {0} component(s) to control {1} properties.";
            public static readonly string[] albedoAlphaModeNames = Enum.GetNames(typeof(AlbedoAlphaMode));
            public static GUIContent albedo = new GUIContent("Albedo", "Albedo (RGB) and Transparency (Alpha)");
            public static GUIContent albedoAssignedAtRuntime = new GUIContent("Assigned at Runtime", "As an optimization albedo operations are disabled when no albedo texture is specified. If a albedo texture will be specified at runtime enable this option.");
            public static GUIContent alphaCutoff = new GUIContent("Alpha Cutoff", "Threshold for Alpha Cutoff");
            public static GUIContent metallic = new GUIContent("Metallic", "Metallic Value");
            public static GUIContent smoothness = new GUIContent("Smoothness", "Smoothness Value");
            public static GUIContent enableChannelMap = new GUIContent("Channel Map", "Enable Channel Map, a Channel Packing Texture That Follows Unity's Standard Channel Setup");
            public static GUIContent channelMap = new GUIContent("Channel Map", "Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)");
            public static GUIContent enableNormalMap = new GUIContent("Normal Map", "Enable Normal Map");
            public static GUIContent normalMap = new GUIContent("Normal Map");
            public static GUIContent normalMapScale = new GUIContent("Scale", "Scales the Normal Map Normal");
            public static GUIContent enableEmission = new GUIContent("Emission", "Enable Emission");
            public static GUIContent emissiveColor = new GUIContent("Color");
            public static GUIContent enableTriplanarMapping = new GUIContent("Triplanar Mapping", "Enable Triplanar Mapping, a technique which programmatically generates UV coordinates");
            public static GUIContent enableSSAA = new GUIContent("Super Sample Anti-Aliasing", "Enable Super Sample Anti-Aliasing, a technique improves texture clarity at long distances");
            public static GUIContent mipmapBias = new GUIContent("Mipmap Bias", "Degree to bias the mip map. A larger negative value reduces aliasing and improves clarity, but may decrease performance");
            public static GUIContent enableLocalSpaceTriplanarMapping = new GUIContent("Local Space", "If True Triplanar Mapping is Calculated in Local Space");
            public static GUIContent triplanarMappingBlendSharpness = new GUIContent("Blend Sharpness", "The Power of the Blend with the Normal");
            public static GUIContent directionalLight = new GUIContent("Directional Light", "Affected by One Unity Directional Light");
            public static GUIContent specularHighlights = new GUIContent("Specular Highlights", "Calculate Specular Highlights");
            public static GUIContent sphericalHarmonics = new GUIContent("Spherical Harmonics", "Read From Spherical Harmonics Data for Ambient Light");
            public static GUIContent reflections = new GUIContent("Reflections", "Calculate Glossy Reflections");
            public static GUIContent refraction = new GUIContent("Refraction", "Calculate Refraction");
            public static GUIContent refractiveIndex = new GUIContent("Refractive Index", "Ratio of Indices of Refraction at the Surface Interface");
            public static GUIContent rimLight = new GUIContent("Rim Light", "Enable Rim (Fresnel) Lighting");
            public static GUIContent rimColor = new GUIContent("Color", "Rim Highlight Color");
            public static GUIContent rimPower = new GUIContent("Power", "Rim Highlight Saturation");
            public static GUIContent vertexColors = new GUIContent("Vertex Colors", "Enable Vertex Color Tinting");
            public static GUIContent vertexExtrusion = new GUIContent("Vertex Extrusion", "Enable Vertex Extrusion Along the Vertex Normal");
            public static GUIContent vertexExtrusionValue = new GUIContent("Extrusion Value", "How Far to Extrude the Vertex Along the Vertex Normal");
            public static GUIContent vertexExtrusionSmoothNormals = new GUIContent("Use Smooth Normals", "Should Vertex Extrusion use the Smooth Normals in UV3, or Default Normals");
            public static GUIContent blendedClippingWidth = new GUIContent("Blended Clipping Width", "The Width of the Clipping Primitive Clip Fade Region on Non-Cutout Materials");
            public static GUIContent clippingBorder = new GUIContent("Clipping Border", "Enable a Border Along the Clipping Primitive's Edge");
            public static GUIContent clippingBorderWidth = new GUIContent("Width", "Width of the Clipping Border");
            public static GUIContent clippingBorderColor = new GUIContent("Color", "Interpolated Color of the Clipping Border");
            public static GUIContent nearPlaneFade = new GUIContent("Near Fade", "Objects Disappear (Turn to Black/Transparent) as the Camera (or Hover/Proximity Light) Nears Them");
            public static GUIContent nearLightFade = new GUIContent("Use Light", "A Hover or Proximity Light (Rather Than the Camera) Determines Near Fade Distance");
            public static GUIContent fadeBeginDistance = new GUIContent("Fade Begin", "Distance From Camera (or Hover/Proximity Light) to Begin Fade In");
            public static GUIContent fadeCompleteDistance = new GUIContent("Fade Complete", "Distance From Camera (or Hover/Proximity Light) When Fade is Fully In");
            public static GUIContent fadeMinValue = new GUIContent("Fade Min Value", "Clamps the Fade Amount to a Minimum Value");
            public static GUIContent hoverLight = new GUIContent("Hover Light", "Enable utilization of Hover Light(s)");
            public static GUIContent enableHoverColorOverride = new GUIContent("Override Color", "Override Global Hover Light Color for this Material");
            public static GUIContent hoverColorOverride = new GUIContent("Color", "Override Hover Light Color");
            public static GUIContent proximityLight = new GUIContent("Proximity Light", "Enable utilization of Proximity Light(s)");
            public static GUIContent enableProximityLightColorOverride = new GUIContent("Override Color", "Override Global Proximity Light Color for this Material");
            public static GUIContent proximityLightCenterColorOverride = new GUIContent("Center Color", "The Override Color of the ProximityLight Gradient at the Center (RGB) and (A) is Gradient Extent");
            public static GUIContent proximityLightMiddleColorOverride = new GUIContent("Middle Color", "The Override Color of the ProximityLight Gradient at the Middle (RGB) and (A) is Gradient Extent");
            public static GUIContent proximityLightOuterColorOverride = new GUIContent("Outer Color", "The Override Color of the ProximityLight Gradient at the Outer Edge (RGB) and (A) is Gradient Extent");
            public static GUIContent proximityLightSubtractive = new GUIContent("Subtractive", "Proximity Lights Remove Light from a Surface, Used to Mimic a Shadow");
            public static GUIContent proximityLightTwoSided = new GUIContent("Two Sided", "Proximity Lights Apply to Both Sides of a Surface");
            public static GUIContent fluentLightIntensity = new GUIContent("Light Intensity", "Intensity Scaler for All Hover and Proximity Lights");
            public static GUIContent roundCorners = new GUIContent("Round Corners", "(Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static GUIContent roundCornerRadius = new GUIContent("Unit Radius", "Rounded Rectangle Corner Unit Sphere Radius");
            public static GUIContent roundCornersRadius = new GUIContent("Corners Radius", "UpLeft-UpRight-BottomRight-BottomLeft");
            public static GUIContent roundCornerMargin = new GUIContent("Margin %", "Distance From Geometry Edge");
            public static GUIContent independentCorners = new GUIContent("Independent Corners", "Manage each corner separately");
            public static GUIContent borderLight = new GUIContent("Border Light", "Enable Border Lighting (Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static GUIContent borderLightUsesHoverColor = new GUIContent("Use Hover Color", "Border Color Comes From Hover Light Color Override");
            public static GUIContent borderLightReplacesAlbedo = new GUIContent("Replace Albedo", "Border Light Replaces Albedo (Replacement Rather Than Additive)");
            public static GUIContent borderLightOpaque = new GUIContent("Opaque Borders", "Borders Override Alpha Value to Appear Opaque");
            public static GUIContent borderWidth = new GUIContent("Width %", "Uniform Width Along Border as a % of the Smallest XYZ Dimension");
            public static GUIContent borderMinValue = new GUIContent("Brightness", "Brightness Scaler");
            public static GUIContent edgeSmoothingValue = new GUIContent("Edge Smoothing Value", "Smooths Edges When Round Corners and Transparency Is Enabled");
            public static GUIContent borderLightOpaqueAlpha = new GUIContent("Alpha", "Alpha value of \"opaque\" borders.");
            public static GUIContent innerGlow = new GUIContent("Inner Glow", "Enable Inner Glow (Assumes UVs Specify Borders of Surface, Works Best on Unity Cube, Quad, and Plane)");
            public static GUIContent innerGlowColor = new GUIContent("Color", "Inner Glow Color (RGB) and Intensity (A)");
            public static GUIContent innerGlowPower = new GUIContent("Power", "Power Exponent to Control Glow");
            public static GUIContent iridescence = new GUIContent("Iridescence", "Simulated Iridescence via Albedo Changes with the Angle of Observation)");
            public static GUIContent iridescentSpectrumMap = new GUIContent("Spectrum Map", "Spectrum of Colors to Apply (Usually a Texture with ROYGBIV from Left to Right)");
            public static GUIContent iridescenceIntensity = new GUIContent("Intensity", "Intensity of Iridescence");
            public static GUIContent iridescenceThreshold = new GUIContent("Threshold", "Threshold Window to Sample From the Spectrum Map");
            public static GUIContent iridescenceAngle = new GUIContent("Angle", "Surface Angle");
            public static GUIContent environmentColoring = new GUIContent("Environment Coloring", "Change Color Based on View");
            public static GUIContent environmentColorThreshold = new GUIContent("Threshold", "Threshold When Environment Coloring Should Appear Based on Surface Normal");
            public static GUIContent environmentColorIntensity = new GUIContent("Intensity", "Intensity (or Brightness) of the Environment Coloring");
            public static GUIContent environmentColorX = new GUIContent("X-Axis Color", "Color Along the World Space X-Axis");
            public static GUIContent environmentColorY = new GUIContent("Y-Axis Color", "Color Along the World Space Y-Axis");
            public static GUIContent environmentColorZ = new GUIContent("Z-Axis Color", "Color Along the World Space Z-Axis");
            public static GUIContent stencil = new GUIContent("Enable Stencil Testing", "Enabled Stencil Testing Operations");
            public static GUIContent stencilReference = new GUIContent("Stencil Reference", "Value to Compared Against (if Comparison is Anything but Always) and/or the Value to be Written to the Buffer (if Either Pass, Fail or ZFail is Set to Replace)");
            public static GUIContent stencilComparison = new GUIContent("Stencil Comparison", "Function to Compare the Reference Value to");
            public static GUIContent stencilOperation = new GUIContent("Stencil Operation", "What to do When the Stencil Test Passes");
            public static GUIContent ignoreZScale = new GUIContent("Ignore Z Scale", "For Features That Use Object Scale (Round Corners, Border Light, etc.), Ignore the Z Scale of the Object");
        }

        protected MaterialProperty albedoMap;
        protected MaterialProperty albedoColor;
        protected MaterialProperty albedoAlphaMode;
        protected MaterialProperty albedoAssignedAtRuntime;
        protected MaterialProperty alphaCutoff;
        protected MaterialProperty enableChannelMap;
        protected MaterialProperty channelMap;
        protected MaterialProperty enableNormalMap;
        protected MaterialProperty normalMap;
        protected MaterialProperty normalMapScale;
        protected MaterialProperty enableEmission;
        protected MaterialProperty emissiveColor;
        protected MaterialProperty enableTriplanarMapping;
        protected MaterialProperty enableSSAA;
        protected MaterialProperty mipmapBias;
        protected MaterialProperty enableLocalSpaceTriplanarMapping;
        protected MaterialProperty triplanarMappingBlendSharpness;
        protected MaterialProperty metallic;
        protected MaterialProperty smoothness;
        protected MaterialProperty directionalLight;
        protected MaterialProperty specularHighlights;
        protected MaterialProperty sphericalHarmonics;
        protected MaterialProperty reflections;
        protected MaterialProperty refraction;
        protected MaterialProperty refractiveIndex;
        protected MaterialProperty rimLight;
        protected MaterialProperty rimColor;
        protected MaterialProperty rimPower;
        protected MaterialProperty vertexColors;
        protected MaterialProperty vertexExtrusion;
        protected MaterialProperty vertexExtrusionValue;
        protected MaterialProperty vertexExtrusionSmoothNormals;
        protected MaterialProperty blendedClippingWidth;
        protected MaterialProperty clippingBorder;
        protected MaterialProperty clippingBorderWidth;
        protected MaterialProperty clippingBorderColor;
        protected MaterialProperty nearPlaneFade;
        protected MaterialProperty nearLightFade;
        protected MaterialProperty fadeBeginDistance;
        protected MaterialProperty fadeCompleteDistance;
        protected MaterialProperty fadeMinValue;
        protected MaterialProperty hoverLight;
        protected MaterialProperty enableHoverColorOverride;
        protected MaterialProperty hoverColorOverride;
        protected MaterialProperty proximityLight;
        protected MaterialProperty enableProximityLightColorOverride;
        protected MaterialProperty proximityLightCenterColorOverride;
        protected MaterialProperty proximityLightMiddleColorOverride;
        protected MaterialProperty proximityLightOuterColorOverride;
        protected MaterialProperty proximityLightSubtractive;
        protected MaterialProperty proximityLightTwoSided;
        protected MaterialProperty fluentLightIntensity;
        protected MaterialProperty roundCorners;
        protected MaterialProperty roundCornerRadius;
        protected MaterialProperty roundCornerMargin;
        protected MaterialProperty independentCorners;
        protected MaterialProperty roundCornersRadius;
        protected MaterialProperty borderLight;
        protected MaterialProperty borderLightUsesHoverColor;
        protected MaterialProperty borderLightReplacesAlbedo;
        protected MaterialProperty borderLightOpaque;
        protected MaterialProperty borderWidth;
        protected MaterialProperty borderMinValue;
        protected MaterialProperty edgeSmoothingValue;
        protected MaterialProperty borderLightOpaqueAlpha;
        protected MaterialProperty innerGlow;
        protected MaterialProperty innerGlowColor;
        protected MaterialProperty innerGlowPower;
        protected MaterialProperty iridescence;
        protected MaterialProperty iridescentSpectrumMap;
        protected MaterialProperty iridescenceIntensity;
        protected MaterialProperty iridescenceThreshold;
        protected MaterialProperty iridescenceAngle;
        protected MaterialProperty environmentColoring;
        protected MaterialProperty environmentColorThreshold;
        protected MaterialProperty environmentColorIntensity;
        protected MaterialProperty environmentColorX;
        protected MaterialProperty environmentColorY;
        protected MaterialProperty environmentColorZ;
        protected MaterialProperty stencil;
        protected MaterialProperty stencilReference;
        protected MaterialProperty stencilComparison;
        protected MaterialProperty stencilOperation;
        protected MaterialProperty ignoreZScale;

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            albedoAlphaMode = FindProperty("_AlbedoAlphaMode", props);
            albedoAssignedAtRuntime = FindProperty("_AlbedoAssignedAtRuntime", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            metallic = FindProperty("_Metallic", props);
            smoothness = FindProperty("_Smoothness", props);
            enableChannelMap = FindProperty("_EnableChannelMap", props);
            channelMap = FindProperty("_ChannelMap", props);
            enableNormalMap = FindProperty("_EnableNormalMap", props);
            normalMap = FindProperty("_NormalMap", props);
            normalMapScale = FindProperty("_NormalMapScale", props);
            enableEmission = FindProperty("_EnableEmission", props);
            emissiveColor = FindProperty("_EmissiveColor", props);
            enableTriplanarMapping = FindProperty("_EnableTriplanarMapping", props);
            enableSSAA = FindProperty("_EnableSSAA", props);
            mipmapBias = FindProperty("_MipmapBias", props);
            enableLocalSpaceTriplanarMapping = FindProperty("_EnableLocalSpaceTriplanarMapping", props);
            triplanarMappingBlendSharpness = FindProperty("_TriplanarMappingBlendSharpness", props);
            directionalLight = FindProperty("_DirectionalLight", props);
            specularHighlights = FindProperty("_SpecularHighlights", props);
            sphericalHarmonics = FindProperty("_SphericalHarmonics", props);
            reflections = FindProperty("_Reflections", props);
            refraction = FindProperty("_Refraction", props);
            refractiveIndex = FindProperty("_RefractiveIndex", props);
            rimLight = FindProperty("_RimLight", props);
            rimColor = FindProperty("_RimColor", props);
            rimPower = FindProperty("_RimPower", props);
            vertexColors = FindProperty("_VertexColors", props);
            vertexExtrusion = FindProperty("_VertexExtrusion", props);
            vertexExtrusionValue = FindProperty("_VertexExtrusionValue", props);
            vertexExtrusionSmoothNormals = FindProperty("_VertexExtrusionSmoothNormals", props);
            blendedClippingWidth = FindProperty("_BlendedClippingWidth", props);
            clippingBorder = FindProperty("_ClippingBorder", props);
            clippingBorderWidth = FindProperty("_ClippingBorderWidth", props);
            clippingBorderColor = FindProperty("_ClippingBorderColor", props);
            nearPlaneFade = FindProperty("_NearPlaneFade", props);
            nearLightFade = FindProperty("_NearLightFade", props);
            fadeBeginDistance = FindProperty("_FadeBeginDistance", props);
            fadeCompleteDistance = FindProperty("_FadeCompleteDistance", props);
            fadeMinValue = FindProperty("_FadeMinValue", props);
            hoverLight = FindProperty("_HoverLight", props);
            enableHoverColorOverride = FindProperty("_EnableHoverColorOverride", props);
            hoverColorOverride = FindProperty("_HoverColorOverride", props);
            proximityLight = FindProperty("_ProximityLight", props);
            enableProximityLightColorOverride = FindProperty("_EnableProximityLightColorOverride", props);
            proximityLightCenterColorOverride = FindProperty("_ProximityLightCenterColorOverride", props);
            proximityLightMiddleColorOverride = FindProperty("_ProximityLightMiddleColorOverride", props);
            proximityLightOuterColorOverride = FindProperty("_ProximityLightOuterColorOverride", props);
            proximityLightSubtractive = FindProperty("_ProximityLightSubtractive", props);
            proximityLightTwoSided = FindProperty("_ProximityLightTwoSided", props);
            fluentLightIntensity = FindProperty("_FluentLightIntensity", props);
            roundCorners = FindProperty("_RoundCorners", props);
            roundCornerRadius = FindProperty("_RoundCornerRadius", props);
            roundCornersRadius = FindProperty("_RoundCornersRadius", props);
            roundCornerMargin = FindProperty("_RoundCornerMargin", props);
            independentCorners = FindProperty("_IndependentCorners", props);
            borderLight = FindProperty("_BorderLight", props);
            borderLightUsesHoverColor = FindProperty("_BorderLightUsesHoverColor", props);
            borderLightReplacesAlbedo = FindProperty("_BorderLightReplacesAlbedo", props);
            borderLightOpaque = FindProperty("_BorderLightOpaque", props);
            borderWidth = FindProperty("_BorderWidth", props);
            borderMinValue = FindProperty("_BorderMinValue", props);
            edgeSmoothingValue = FindProperty("_EdgeSmoothingValue", props);
            borderLightOpaqueAlpha = FindProperty("_BorderLightOpaqueAlpha", props);
            innerGlow = FindProperty("_InnerGlow", props);
            innerGlowColor = FindProperty("_InnerGlowColor", props);
            innerGlowPower = FindProperty("_InnerGlowPower", props);
            iridescence = FindProperty("_Iridescence", props);
            iridescentSpectrumMap = FindProperty("_IridescentSpectrumMap", props);
            iridescenceIntensity = FindProperty("_IridescenceIntensity", props);
            iridescenceThreshold = FindProperty("_IridescenceThreshold", props);
            iridescenceAngle = FindProperty("_IridescenceAngle", props);
            environmentColoring = FindProperty("_EnvironmentColoring", props);
            environmentColorThreshold = FindProperty("_EnvironmentColorThreshold", props);
            environmentColorIntensity = FindProperty("_EnvironmentColorIntensity", props);
            environmentColorX = FindProperty("_EnvironmentColorX", props);
            environmentColorY = FindProperty("_EnvironmentColorY", props);
            environmentColorZ = FindProperty("_EnvironmentColorZ", props);
            stencil = FindProperty("_Stencil", props);
            stencilReference = FindProperty("_StencilReference", props);
            stencilComparison = FindProperty(Styles.stencilComparisonName, props);
            stencilOperation = FindProperty(Styles.stencilOperationName, props);
            ignoreZScale = FindProperty("_IgnoreZScale", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            base.OnGUI(materialEditor, props);

            MainMapOptions(materialEditor, material);
            RenderingOptions(materialEditor, material);
            FluentOptions(materialEditor, material);
            AdvancedOptions(materialEditor, material);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            // Cache old shader properties with potentially different names than the new shader.
            float? smoothness = GetFloatProperty(material, "_Glossiness");
            float? diffuse = GetFloatProperty(material, "_UseDiffuse");
            float? specularHighlights = GetFloatProperty(material, "_SpecularHighlights");
            float? normalMap = null;
            Texture normalMapTexture = material.GetTexture("_BumpMap");
            float? normalMapScale = GetFloatProperty(material, "_BumpScale");
            float? emission = null;
            Color? emissionColor = GetColorProperty(material, "_EmissionColor");
            float? reflections = null;
            float? rimLighting = null;
            Vector4? textureScaleOffset = null;
            float? cullMode = GetFloatProperty(material, "_Cull");

            if (oldShader)
            {
                if (oldShader.name.Contains("Standard"))
                {
                    normalMap = material.IsKeywordEnabled("_NORMALMAP") ? 1.0f : 0.0f;
                    emission = material.IsKeywordEnabled("_EMISSION") ? 1.0f : 0.0f;
                    reflections = GetFloatProperty(material, "_GlossyReflections");
                }
                else if (oldShader.name.Contains("Fast Configurable"))
                {
                    normalMap = material.IsKeywordEnabled("_USEBUMPMAP_ON") ? 1.0f : 0.0f;
                    emission = GetFloatProperty(material, "_UseEmissionColor");
                    reflections = GetFloatProperty(material, "_UseReflections");
                    rimLighting = GetFloatProperty(material, "_UseRimLighting");
                    textureScaleOffset = GetVectorProperty(material, "_TextureScaleOffset");
                }
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            // Apply old shader properties to the new shader.
            SetShaderFeatureActive(material, null, "_Smoothness", smoothness);
            SetShaderFeatureActive(material, "_DIRECTIONAL_LIGHT", "_DirectionalLight", diffuse);
            SetShaderFeatureActive(material, "_SPECULAR_HIGHLIGHTS", "_SpecularHighlights", specularHighlights);
            SetShaderFeatureActive(material, "_NORMAL_MAP", "_EnableNormalMap", normalMap);

            if (normalMapTexture)
            {
                material.SetTexture("_NormalMap", normalMapTexture);
            }

            SetShaderFeatureActive(material, null, "_NormalMapScale", normalMapScale);
            SetShaderFeatureActive(material, "_EMISSION", "_EnableEmission", emission);
            SetColorProperty(material, "_EmissiveColor", emissionColor);
            SetShaderFeatureActive(material, "_REFLECTIONS", "_Reflections", reflections);
            SetShaderFeatureActive(material, "_RIM_LIGHT", "_RimLight", rimLighting);
            SetVectorProperty(material, "_MainTex_ST", textureScaleOffset);
            SetShaderFeatureActive(material, null, "_CullMode", cullMode);

            // Setup the rendering mode based on the old shader.
            if (oldShader == null || !oldShader.name.Contains(LegacyShadersPath))
            {
                SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat(BaseStyles.renderingModeName), CustomRenderingMode.Opaque, -1);
            }
            else
            {
                RenderingMode mode = RenderingMode.Opaque;

                if (oldShader.name.Contains(TransparentCutoutShadersPath))
                {
                    mode = RenderingMode.Cutout;
                }
                else if (oldShader.name.Contains(TransparentShadersPath))
                {
                    mode = RenderingMode.Fade;
                }

                material.SetFloat(BaseStyles.renderingModeName, (float)mode);

                MaterialChanged(material);
            }
        }

        protected override void MaterialChanged(Material material)
        {
            SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode, albedoAssignedAtRuntime);

            base.MaterialChanged(material);
        }

        protected void MainMapOptions(MaterialEditor materialEditor, Material material)
        {
            GUILayout.Label(Styles.primaryMapsTitle, EditorStyles.boldLabel);

            materialEditor.TexturePropertySingleLine(Styles.albedo, albedoMap, albedoColor);

            if (albedoMap.textureValue == null)
            {
                materialEditor.ShaderProperty(albedoAssignedAtRuntime, Styles.albedoAssignedAtRuntime, 2);
            }

            materialEditor.ShaderProperty(enableChannelMap, Styles.enableChannelMap);

            if (PropertyEnabled(enableChannelMap))
            {
                EditorGUI.indentLevel += 2;
                materialEditor.TexturePropertySingleLine(Styles.channelMap, channelMap);
                GUILayout.Box("Metallic (Red), Occlusion (Green), Emission (Blue), Smoothness (Alpha)", EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
                EditorGUI.indentLevel -= 2;
            }

            if (!PropertyEnabled(enableChannelMap))
            {
                EditorGUI.indentLevel += 2;

                materialEditor.ShaderProperty(albedoAlphaMode, albedoAlphaMode.displayName);

                if ((RenderingMode)renderingMode.floatValue == RenderingMode.Cutout ||
                    (RenderingMode)renderingMode.floatValue == RenderingMode.Custom)
                {
                    materialEditor.ShaderProperty(alphaCutoff, Styles.alphaCutoff.text);
                }

                if ((AlbedoAlphaMode)albedoAlphaMode.floatValue != AlbedoAlphaMode.Metallic)
                {
                    materialEditor.ShaderProperty(metallic, Styles.metallic);
                }

                if ((AlbedoAlphaMode)albedoAlphaMode.floatValue != AlbedoAlphaMode.Smoothness)
                {
                    materialEditor.ShaderProperty(smoothness, Styles.smoothness);
                }

                SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode, albedoAssignedAtRuntime);

                EditorGUI.indentLevel -= 2;
            }

            if (PropertyEnabled(directionalLight) ||
                PropertyEnabled(reflections) ||
                PropertyEnabled(rimLight) ||
                PropertyEnabled(environmentColoring))
            {
                materialEditor.ShaderProperty(enableNormalMap, Styles.enableNormalMap);

                if (PropertyEnabled(enableNormalMap))
                {
                    EditorGUI.indentLevel += 2;
                    materialEditor.TexturePropertySingleLine(Styles.normalMap, normalMap, normalMap.textureValue != null ? normalMapScale : null);
                    EditorGUI.indentLevel -= 2;
                }
            }

            materialEditor.ShaderProperty(enableEmission, Styles.enableEmission);

            if (PropertyEnabled(enableEmission))
            {
                materialEditor.ShaderProperty(emissiveColor, Styles.emissiveColor, 2);
            }

            GUI.enabled = !PropertyEnabled(enableSSAA);
            materialEditor.ShaderProperty(enableTriplanarMapping, Styles.enableTriplanarMapping);

            if (PropertyEnabled(enableTriplanarMapping))
            {
                materialEditor.ShaderProperty(enableLocalSpaceTriplanarMapping, Styles.enableLocalSpaceTriplanarMapping, 2);
                materialEditor.ShaderProperty(triplanarMappingBlendSharpness, Styles.triplanarMappingBlendSharpness, 2);
            }
            GUI.enabled = true;

            GUI.enabled = !PropertyEnabled(enableTriplanarMapping);
            // SSAA implementation based off this article: https://medium.com/@bgolus/sharper-mipmapping-using-shader-based-supersampling-ed7aadb47bec
            materialEditor.ShaderProperty(enableSSAA, Styles.enableSSAA);

            if (PropertyEnabled(enableSSAA))
            {
                materialEditor.ShaderProperty(mipmapBias, Styles.mipmapBias, 2);
            }
            GUI.enabled = true;

            EditorGUILayout.Space();
            materialEditor.TextureScaleOffsetProperty(albedoMap);
        }

        protected void RenderingOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.renderingOptionsTitle, EditorStyles.boldLabel);

            materialEditor.ShaderProperty(directionalLight, Styles.directionalLight);

            if (PropertyEnabled(directionalLight))
            {
                materialEditor.ShaderProperty(specularHighlights, Styles.specularHighlights, 2);
            }

            materialEditor.ShaderProperty(sphericalHarmonics, Styles.sphericalHarmonics);

            materialEditor.ShaderProperty(reflections, Styles.reflections);

            if (PropertyEnabled(reflections))
            {
                materialEditor.ShaderProperty(refraction, Styles.refraction, 2);

                if (PropertyEnabled(refraction))
                {
                    materialEditor.ShaderProperty(refractiveIndex, Styles.refractiveIndex, 4);
                }
            }

            materialEditor.ShaderProperty(rimLight, Styles.rimLight);

            if (PropertyEnabled(rimLight))
            {
                materialEditor.ShaderProperty(rimColor, Styles.rimColor, 2);
                materialEditor.ShaderProperty(rimPower, Styles.rimPower, 2);
            }

            materialEditor.ShaderProperty(vertexColors, Styles.vertexColors);

            materialEditor.ShaderProperty(vertexExtrusion, Styles.vertexExtrusion);

            if (PropertyEnabled(vertexExtrusion))
            {
                materialEditor.ShaderProperty(vertexExtrusionValue, Styles.vertexExtrusionValue, 2);
                materialEditor.ShaderProperty(vertexExtrusionSmoothNormals, Styles.vertexExtrusionSmoothNormals, 2);
            }

            if ((RenderingMode)renderingMode.floatValue != RenderingMode.Opaque &&
                (RenderingMode)renderingMode.floatValue != RenderingMode.Cutout)
            {
                materialEditor.ShaderProperty(blendedClippingWidth, Styles.blendedClippingWidth);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ClippingPrimitive), "other clipping"), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(clippingBorder, Styles.clippingBorder);

            if (PropertyEnabled(clippingBorder))
            {
                materialEditor.ShaderProperty(clippingBorderWidth, Styles.clippingBorderWidth, 2);
                materialEditor.ShaderProperty(clippingBorderColor, Styles.clippingBorderColor, 2);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ClippingPrimitive), "other clipping"), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(nearPlaneFade, Styles.nearPlaneFade);

            if (PropertyEnabled(nearPlaneFade))
            {
                materialEditor.ShaderProperty(nearLightFade, Styles.nearLightFade, 2);
                materialEditor.ShaderProperty(fadeBeginDistance, Styles.fadeBeginDistance, 2);
                materialEditor.ShaderProperty(fadeCompleteDistance, Styles.fadeCompleteDistance, 2);
                materialEditor.ShaderProperty(fadeMinValue, Styles.fadeMinValue, 2);
            }
        }

        protected void FluentOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.fluentOptionsTitle, EditorStyles.boldLabel);
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            CustomRenderingMode customMode = (CustomRenderingMode)customRenderingMode.floatValue;

            materialEditor.ShaderProperty(hoverLight, Styles.hoverLight);

            if (PropertyEnabled(hoverLight))
            {
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(HoverLight), Styles.hoverLight.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());

                materialEditor.ShaderProperty(enableHoverColorOverride, Styles.enableHoverColorOverride, 2);

                if (PropertyEnabled(enableHoverColorOverride))
                {
                    materialEditor.ShaderProperty(hoverColorOverride, Styles.hoverColorOverride, 4);
                }
            }

            materialEditor.ShaderProperty(proximityLight, Styles.proximityLight);

            if (PropertyEnabled(proximityLight))
            {
                materialEditor.ShaderProperty(enableProximityLightColorOverride, Styles.enableProximityLightColorOverride, 2);

                if (PropertyEnabled(enableProximityLightColorOverride))
                {
                    materialEditor.ShaderProperty(proximityLightCenterColorOverride, Styles.proximityLightCenterColorOverride, 4);
                    materialEditor.ShaderProperty(proximityLightMiddleColorOverride, Styles.proximityLightMiddleColorOverride, 4);
                    materialEditor.ShaderProperty(proximityLightOuterColorOverride, Styles.proximityLightOuterColorOverride, 4);
                }

                materialEditor.ShaderProperty(proximityLightSubtractive, Styles.proximityLightSubtractive, 2);
                materialEditor.ShaderProperty(proximityLightTwoSided, Styles.proximityLightTwoSided, 2);
                GUILayout.Box(string.Format(Styles.propertiesComponentHelp, nameof(ProximityLight), Styles.proximityLight.text), EditorStyles.helpBox, Array.Empty<GUILayoutOption>());
            }

            materialEditor.ShaderProperty(borderLight, Styles.borderLight);

            if (PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(borderWidth, Styles.borderWidth, 2);

                materialEditor.ShaderProperty(borderMinValue, Styles.borderMinValue, 2);

                materialEditor.ShaderProperty(borderLightReplacesAlbedo, Styles.borderLightReplacesAlbedo, 2);

                if (PropertyEnabled(hoverLight) && PropertyEnabled(enableHoverColorOverride))
                {
                    materialEditor.ShaderProperty(borderLightUsesHoverColor, Styles.borderLightUsesHoverColor, 2);
                }

                if (mode == RenderingMode.Cutout || mode == RenderingMode.Fade || mode == RenderingMode.Transparent ||
                    (mode == RenderingMode.Custom && customMode == CustomRenderingMode.Cutout) ||
                    (mode == RenderingMode.Custom && customMode == CustomRenderingMode.Fade))
                {
                    materialEditor.ShaderProperty(borderLightOpaque, Styles.borderLightOpaque, 2);

                    if (PropertyEnabled(borderLightOpaque))
                    {
                        materialEditor.ShaderProperty(borderLightOpaqueAlpha, Styles.borderLightOpaqueAlpha, 4);
                    }
                }
            }

            if (PropertyEnabled(hoverLight) || PropertyEnabled(proximityLight) || PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(fluentLightIntensity, Styles.fluentLightIntensity);
            }

            materialEditor.ShaderProperty(roundCorners, Styles.roundCorners);

            if (PropertyEnabled(roundCorners))
            {
                materialEditor.ShaderProperty(independentCorners, Styles.independentCorners, 2);
                if (PropertyEnabled(independentCorners))
                {
                    materialEditor.ShaderProperty(roundCornersRadius, Styles.roundCornersRadius, 2);
                }
                else
                {
                    materialEditor.ShaderProperty(roundCornerRadius, Styles.roundCornerRadius, 2);
                }

                materialEditor.ShaderProperty(roundCornerMargin, Styles.roundCornerMargin, 2);
            }

            if (PropertyEnabled(roundCorners) || PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(edgeSmoothingValue, Styles.edgeSmoothingValue);
            }

            materialEditor.ShaderProperty(innerGlow, Styles.innerGlow);

            if (PropertyEnabled(innerGlow))
            {
                materialEditor.ShaderProperty(innerGlowColor, Styles.innerGlowColor, 2);
                materialEditor.ShaderProperty(innerGlowPower, Styles.innerGlowPower, 2);
            }

            materialEditor.ShaderProperty(iridescence, Styles.iridescence);

            if (PropertyEnabled(iridescence))
            {
                EditorGUI.indentLevel += 2;
                materialEditor.TexturePropertySingleLine(Styles.iridescentSpectrumMap, iridescentSpectrumMap);
                EditorGUI.indentLevel -= 2;
                materialEditor.ShaderProperty(iridescenceIntensity, Styles.iridescenceIntensity, 2);
                materialEditor.ShaderProperty(iridescenceThreshold, Styles.iridescenceThreshold, 2);
                materialEditor.ShaderProperty(iridescenceAngle, Styles.iridescenceAngle, 2);
            }

            materialEditor.ShaderProperty(environmentColoring, Styles.environmentColoring);

            if (PropertyEnabled(environmentColoring))
            {
                materialEditor.ShaderProperty(environmentColorThreshold, Styles.environmentColorThreshold, 2);
                materialEditor.ShaderProperty(environmentColorIntensity, Styles.environmentColorIntensity, 2);
                materialEditor.ShaderProperty(environmentColorX, Styles.environmentColorX, 2);
                materialEditor.ShaderProperty(environmentColorY, Styles.environmentColorY, 2);
                materialEditor.ShaderProperty(environmentColorZ, Styles.environmentColorZ, 2);
            }
        }

        protected void AdvancedOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.advancedOptionsTitle, EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(renderQueueOverride, BaseStyles.renderQueueOverride);

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            // Show the RenderQueueField but do not allow users to directly manipulate it. That is done via the renderQueueOverride.
            GUI.enabled = false;
            materialEditor.RenderQueueField();

            // Enable instancing to disable batching. Static and dynamic batching will normalize the object scale, which breaks 
            // features which utilize object scale.
            GUI.enabled = !ScaleRequired();

            if (!GUI.enabled && !material.enableInstancing)
            {
                material.enableInstancing = true;
            }

            materialEditor.EnableInstancingField();

            GUI.enabled = true;

            materialEditor.ShaderProperty(stencil, Styles.stencil);

            if (PropertyEnabled(stencil))
            {
                materialEditor.ShaderProperty(stencilReference, Styles.stencilReference, 2);
                materialEditor.ShaderProperty(stencilComparison, Styles.stencilComparison, 2);
                materialEditor.ShaderProperty(stencilOperation, Styles.stencilOperation, 2);
            }
            else
            {
                // When stencil is disable, revert to the default stencil operations. Note, when tested on D3D11 hardware the stencil state 
                // is still set even when the CompareFunction.Disabled is selected, but this does not seem to affect performance.
                material.SetInt(Styles.stencilComparisonName, (int)CompareFunction.Disabled);
                material.SetInt(Styles.stencilOperationName, (int)StencilOp.Keep);
            }

            if (ScaleRequired())
            {
                materialEditor.ShaderProperty(ignoreZScale, Styles.ignoreZScale);
            }
        }

        protected bool ScaleRequired()
        {
            return PropertyEnabled(vertexExtrusion) ||
                   PropertyEnabled(roundCorners) ||
                   PropertyEnabled(borderLight) ||
                   (PropertyEnabled(enableTriplanarMapping) && PropertyEnabled(enableLocalSpaceTriplanarMapping));
        }

        protected static void SetupMaterialWithAlbedo(Material material, MaterialProperty albedoMap, MaterialProperty albedoAlphaMode, MaterialProperty albedoAssignedAtRuntime)
        {
            if (albedoMap.textureValue || PropertyEnabled(albedoAssignedAtRuntime))
            {
                material.DisableKeyword(Styles.disableAlbedoMapName);
            }
            else
            {
                material.EnableKeyword(Styles.disableAlbedoMapName);
            }

            switch ((AlbedoAlphaMode)albedoAlphaMode.floatValue)
            {
                case AlbedoAlphaMode.Transparency:
                {
                    material.DisableKeyword(Styles.albedoMapAlphaMetallicName);
                    material.DisableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    break;
                }

                case AlbedoAlphaMode.Metallic:
                {
                    material.EnableKeyword(Styles.albedoMapAlphaMetallicName);
                    material.DisableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    break;
                }

                case AlbedoAlphaMode.Smoothness:
                {
                    material.DisableKeyword(Styles.albedoMapAlphaMetallicName);
                    material.EnableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    break;
                }
            }
        }

#if UNITY_2019_1_OR_NEWER
        [MenuItem("Mixed Reality/Toolkit/Utilities/Upgrade MRTK Standard Shader for Universal Render Pipeline")]
#else
        [MenuItem("Mixed Reality/Toolkit/Utilities/Upgrade MRTK Standard Shader for Lightweight Render Pipeline")]
#endif
        protected static void UpgradeShaderForUniversalRenderPipeline()
        {
            string confirmationMessage;
#if UNITY_2019_1_OR_NEWER
            confirmationMessage = "This will alter the MRTK Standard Shader for use with Unity's Universal Render Pipeline. You cannot undo this action.";
#else
            confirmationMessage = "This will alter the MRTK Standard Shader for use with Unity's Lightweight Render Pipeline. You cannot undo this action.";
#endif

            if (EditorUtility.DisplayDialog("Upgrade MRTK Standard Shader?",
                                            confirmationMessage,
                                            "Ok",
                                            "Cancel"))
            {
                string path = AssetDatabase.GetAssetPath(StandardShaderUtility.MrtkStandardShader);

                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        string upgradedShader = File.ReadAllText(path);

#if UNITY_2019_1_OR_NEWER
                        upgradedShader = upgradedShader.Replace("Tags{ \"RenderType\" = \"Opaque\" \"LightMode\" = \"ForwardBase\" }",
                                                                "Tags{ \"RenderType\" = \"Opaque\" \"LightMode\" = \"UniversalForward\" }");
#else
                        upgradedShader = upgradedShader.Replace("Tags{ \"RenderType\" = \"Opaque\" \"LightMode\" = \"ForwardBase\" }",
                                                                "Tags{ \"RenderType\" = \"Opaque\" \"LightMode\" = \"LightweightForward\" }");
#endif

                        upgradedShader = upgradedShader.Replace("//#define _RENDER_PIPELINE",
                                                                "#define _RENDER_PIPELINE");

                        File.WriteAllText(path, upgradedShader);
                        AssetDatabase.Refresh();

#if UNITY_2019_1_OR_NEWER
                        Debug.LogFormat("Upgraded {0} for use with the Universal Render Pipeline.", path);
#else
                        Debug.LogFormat("Upgraded {0} for use with the Lightweight Render Pipeline.", path);
#endif
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Failed to get asset path to: {0}", StandardShaderUtility.MrtkStandardShaderName);
                }
            }
        }

#if UNITY_2019_1_OR_NEWER
        [MenuItem("Mixed Reality/Toolkit/Utilities/Upgrade MRTK Standard Shader for Universal Render Pipeline", true)]
#else
        [MenuItem("Mixed Reality/Toolkit/Utilities/Upgrade MRTK Standard Shader for Lightweight Render Pipeline", true)]
#endif
        protected static bool UpgradeShaderForUniversalRenderPipelineValidate()
        {
            // If a scriptable render pipeline is not present, no need to upgrade the shader.
            return GraphicsSettings.renderPipelineAsset != null;
        }
    }
}
