// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Common.EditorScript
{
    /// <summary>
    /// A custom shader inspector for the "MixedRealityToolkit/Standard" shader.
    /// </summary>
    public class StandardShaderGUI : ShaderGUI
    {
        public enum RenderingMode
        {
            Opaque,
            TransparentCutout,
            Transparent,
            PremultipliedTransparent,
            Additive,
            Custom
        }

        public enum CustomRenderingMode
        {
            Opaque,
            TransparentCutout,
            Transparent
        }

        public enum AlbedoAlphaMode
        {
            Transparency,
            Metallic,
            Smoothness
        }

        public enum DepthWrite
        {
            Off,
            On
        }

        public static class Styles
        {
            public static string primaryMapsTitle = "Main Maps";
            public static string renderingOptionsTitle = "Rendering Options";
            public static string advancedOptionsTitle = "Advanced Options";
            public static string fluentOptionsTitle = "Fluent Options";
            public static string renderTypeName = "RenderType";
            public static string renderingModeName = "_Mode";
            public static string customRenderingModeName = "_CustomMode";
            public static string sourceBlendName = "_SrcBlend";
            public static string destinationBlendName = "_DstBlend";
            public static string blendOperationName = "_BlendOp";
            public static string depthTestName = "_ZTest";
            public static string depthWriteName = "_ZWrite";
            public static string colorWriteMaskName = "_ColorWriteMask";
            public static string alphaTestOnName = "_ALPHATEST_ON";
            public static string alphaBlendOnName = "_ALPHABLEND_ON";
            public static string disableAlbedoMapName = "_DISABLE_ALBEDO_MAP";
            public static string albedoMapAlphaMetallicName = "_METALLIC_TEXTURE_ALBEDO_CHANNEL_A";
            public static string albedoMapAlphaSmoothnessName = "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A";
            public static readonly string[] renderingModeNames = Enum.GetNames(typeof(RenderingMode));
            public static readonly string[] customRenderingModeNames = Enum.GetNames(typeof(CustomRenderingMode));
            public static readonly string[] albedoAlphaModeNames = Enum.GetNames(typeof(AlbedoAlphaMode));
            public static readonly string[] depthWriteNames = Enum.GetNames(typeof(DepthWrite));
            public static GUIContent sourceBlend = new GUIContent("Source Blend", "Blend Mode of Newly Calculated Color");
            public static GUIContent destinationBlend = new GUIContent("Destination Blend", "Blend Mode of Exisiting Color");
            public static GUIContent blendOperation = new GUIContent("Blend Operation", "Operation for Blending New Color With Exisiting Color");
            public static GUIContent depthTest = new GUIContent("Depth Test", "How Should Depth Testing Be Performed.");
            public static GUIContent depthWrite = new GUIContent("Depth Write", "Controls Whether Pixels From This Object Are Written to the Depth Buffer");
            public static GUIContent colorWriteMask = new GUIContent("Color Write Mask", "Color Channel Writing Mask");
            public static GUIContent cullMode = new GUIContent("Cull Mode", "Triangle Culling Mode");
            public static GUIContent renderQueueOverride = new GUIContent("Render Queue Override", "Manually Override the Render Queue");
            public static GUIContent albedo = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static GUIContent alphaCutoff = new GUIContent("Alpha Cutoff", "Threshold for Alpha Cutoff");
            public static GUIContent metallic = new GUIContent("Metallic", "Metallic Value");
            public static GUIContent smoothness = new GUIContent("Smoothness", "Smoothness Value");
            public static GUIContent enableNormalMap = new GUIContent("Normal Map", "Enable Normal Map");
            public static GUIContent normalMap = new GUIContent("Normal Map");
            public static GUIContent enableEmission = new GUIContent("Emission", "Enable Emission");
            public static GUIContent emissiveColor = new GUIContent("Color");
            public static GUIContent directionalLight = new GUIContent("Directional Light", "Affected by Unity Directional Light");
            public static GUIContent specularHighlights = new GUIContent("Specular Highlights", "Calculate Specular Highlights");
            public static GUIContent reflections = new GUIContent("Reflections", "Calculate Glossy Reflections");
            public static GUIContent refraction = new GUIContent("Refraction", "Calculate Refraction");
            public static GUIContent refractiveIndex = new GUIContent("Refractive Index", "Ratio of Indices of Refraction at the Surface Interface");
            public static GUIContent rimLight = new GUIContent("Rim Light", "Enable Rim/Edge Lighting");
            public static GUIContent rimColor = new GUIContent("Color", "Rim Highlight Color");
            public static GUIContent rimPower = new GUIContent("Power", "Rim Highlight Saturation");
            public static GUIContent clippingPlane = new GUIContent("Clipping Plane", "Enable Clipping Against a Plane");
            public static GUIContent clipPlane = new GUIContent("Plane", "Plane To Clip Against");
            public static GUIContent clippingPlaneBorder = new GUIContent("Border", "Enable a Border Along the Clipping Plane");
            public static GUIContent clippingPlaneBorderWidth = new GUIContent("Width", "Width of the Clipping Plane Border");
            public static GUIContent clippingPlaneBorderColor = new GUIContent("Color", "Interpolated Color of the Clipping Plane Border");
            public static GUIContent nearPlaneFade = new GUIContent("Near Plane Fade", "Objects Disappear (Turn to Black) as the Camera Nears Them");
            public static GUIContent fadeBeginDistance = new GUIContent("Fade Begin", "Distance From Camera to Begin Fade In");
            public static GUIContent fadeCompleteDistance = new GUIContent("Fade Complete", "Distance From Camera When Fade is Fully In");
            public static GUIContent hoverLight = new GUIContent("Hover Light", "Enable utilization of a Hover Light");
            public static GUIContent enableHoverColorOverride = new GUIContent("Override Color", "Override Global Hover Color");
            public static GUIContent hoverLightOpaque = new GUIContent("Hover Light Opaque", "Enable Hover Light on Transparent pixels");
            public static GUIContent enableHoverColorOpaqueOverride = new GUIContent("Override Color", "Override Opaque Hover Color");
            public static GUIContent hoverColorOpaqueOverride = new GUIContent("Color", "Override Hover Color for Trasnparent Pixels");
            public static GUIContent hoverColorOverride = new GUIContent("Color", "Override Hover Color");
            public static GUIContent roundCorners = new GUIContent("Round Corners", "(Assumes UVs Specify Borders)");
            public static GUIContent roundCornerRadius = new GUIContent("Unit Radius", "Rounded Rectangle Corner Unit Sphere Radius");
            public static GUIContent roundCornerMargin = new GUIContent("Margin %", "Distance From Geometry Edge");
            public static GUIContent borderLight = new GUIContent("Border Light", "Enable Border Lighting (Assumes UVs Specify Borders)");
            public static GUIContent borderLightUsesHoverColor = new GUIContent("Use Hover Color", "Border Color Comes From Hover Light");
            public static GUIContent borderLightOpaque = new GUIContent("Opaque Borders", "Borders Override Alpha Value to Appear Opaque");
            public static GUIContent borderWidth = new GUIContent("Width %", "Uniform Width Along Border as a % of the Smallest XYZ Dimension");
            public static GUIContent borderMinValue = new GUIContent("Min Value", "Minimum Border Saturation");
            public static GUIContent edgeSmoothingValue = new GUIContent("Edge Smoothing Value", "Smooths Edges When Round Corners and Transparency Is Enabled");
            public static GUIContent innerGlow = new GUIContent("Inner Glow", "Enable Inner Glow (Assumes UVs Specify Borders)");
            public static GUIContent innerGlowColor = new GUIContent("Color", "Inner Glow Color (RGB) and Intensity (A)");
            public static GUIContent environmentColoring = new GUIContent("Environment Coloring", "Change Color Based on View");
            public static GUIContent environmentColorThreshold = new GUIContent("Threshold", "Threshold When Environment Coloring Should Appear Based on Surface Normal");
            public static GUIContent environmentColorIntensity = new GUIContent("Intensity", "Intensity (or Brightness) of the Environment Coloring");
            public static GUIContent environmentColorX = new GUIContent("X-Axis Color", "Color Along the Worldspace X-Axis");
            public static GUIContent environmentColorY = new GUIContent("Y-Axis Color", "Color Along the Worldspace Y-Axis");
            public static GUIContent environmentColorZ = new GUIContent("Z-Axis Color", "Color Along the Worldspace Z-Axis");
        }

        protected bool initilized;

        protected MaterialProperty renderingMode;
        protected MaterialProperty customRenderingMode;
        protected MaterialProperty sourceBlend;
        protected MaterialProperty destinationBlend;
        protected MaterialProperty blendOperation;
        protected MaterialProperty depthTest;
        protected MaterialProperty depthWrite;
        protected MaterialProperty colorWriteMask;
        protected MaterialProperty cullMode;
        protected MaterialProperty renderQueueOverride;
        protected MaterialProperty albedoMap;
        protected MaterialProperty albedoColor;
        protected MaterialProperty albedoAlphaMode;
        protected MaterialProperty alphaCutoff;
        protected MaterialProperty enableNormalMap;
        protected MaterialProperty normalMap;
        protected MaterialProperty enableEmission;
        protected MaterialProperty emissiveColor;
        protected MaterialProperty metallic;
        protected MaterialProperty smoothness;
        protected MaterialProperty directionalLight;
        protected MaterialProperty specularHighlights;
        protected MaterialProperty reflections;
        protected MaterialProperty refraction;
        protected MaterialProperty refractiveIndex;
        protected MaterialProperty rimLight;
        protected MaterialProperty rimColor;
        protected MaterialProperty rimPower;
        protected MaterialProperty clippingPlane;
        protected MaterialProperty clipPlane;
        protected MaterialProperty clippingPlaneBorder;
        protected MaterialProperty clippingPlaneBorderWidth;
        protected MaterialProperty clippingPlaneBorderColor;
        protected MaterialProperty nearPlaneFade;
        protected MaterialProperty fadeBeginDistance;
        protected MaterialProperty fadeCompleteDistance;
        protected MaterialProperty hoverLight;
        protected MaterialProperty enableHoverColorOverride;
        protected MaterialProperty hoverLightOpaque;
        protected MaterialProperty enableHoverColorOpaqueOverride;
        protected MaterialProperty hoverColorOverrideOpaque;
        protected MaterialProperty hoverColorOverride;
        protected MaterialProperty roundCorners;
        protected MaterialProperty roundCornerRadius;
        protected MaterialProperty roundCornerMargin;
        protected MaterialProperty borderLight;
        protected MaterialProperty borderLightUsesHoverColor;
        protected MaterialProperty borderLightOpaque;
        protected MaterialProperty borderWidth;
        protected MaterialProperty borderMinValue;
        protected MaterialProperty edgeSmoothingValue;
        protected MaterialProperty innerGlow;
        protected MaterialProperty innerGlowColor;
        protected MaterialProperty environmentColoring;
        protected MaterialProperty environmentColorThreshold;
        protected MaterialProperty environmentColorIntensity;
        protected MaterialProperty environmentColorX;
        protected MaterialProperty environmentColorY;
        protected MaterialProperty environmentColorZ;

        protected void FindProperties(MaterialProperty[] props)
        {
            renderingMode = FindProperty(Styles.renderingModeName, props);
            customRenderingMode = FindProperty(Styles.customRenderingModeName, props);
            sourceBlend = FindProperty(Styles.sourceBlendName, props);
            destinationBlend = FindProperty(Styles.destinationBlendName, props);
            blendOperation = FindProperty(Styles.blendOperationName, props);
            depthTest = FindProperty(Styles.depthTestName, props);
            depthWrite = FindProperty(Styles.depthWriteName, props);
            colorWriteMask = FindProperty(Styles.colorWriteMaskName, props);
            cullMode = FindProperty("_CullMode", props);
            renderQueueOverride = FindProperty("_RenderQueueOverride", props);
            albedoMap = FindProperty("_MainTex", props);
            albedoColor = FindProperty("_Color", props);
            albedoAlphaMode = FindProperty("_AlbedoAlphaMode", props);
            alphaCutoff = FindProperty("_Cutoff", props);
            metallic = FindProperty("_Metallic", props);
            smoothness = FindProperty("_Smoothness", props);
            enableNormalMap = FindProperty("_EnableNormalMap", props);
            normalMap = FindProperty("_NormalMap", props);
            enableEmission = FindProperty("_EnableEmission", props);
            emissiveColor = FindProperty("_EmissiveColor", props);
            directionalLight = FindProperty("_DirectionalLight", props);
            specularHighlights = FindProperty("_SpecularHighlights", props);
            reflections = FindProperty("_Reflections", props);
            refraction = FindProperty("_Refraction", props);
            refractiveIndex = FindProperty("_RefractiveIndex", props);
            rimLight = FindProperty("_RimLight", props);
            rimColor = FindProperty("_RimColor", props);
            rimPower = FindProperty("_RimPower", props);
            clippingPlane = FindProperty("_ClippingPlane", props);
            clipPlane = FindProperty("_ClipPlane", props);
            clippingPlaneBorder = FindProperty("_ClippingPlaneBorder", props);
            clippingPlaneBorderWidth = FindProperty("_ClippingPlaneBorderWidth", props);
            clippingPlaneBorderColor = FindProperty("_ClippingPlaneBorderColor", props);
            nearPlaneFade = FindProperty("_NearPlaneFade", props);
            fadeBeginDistance = FindProperty("_FadeBeginDistance", props);
            fadeCompleteDistance = FindProperty("_FadeCompleteDistance", props);
            hoverLight = FindProperty("_HoverLight", props);
            enableHoverColorOverride = FindProperty("_EnableHoverColorOverride", props);
            hoverColorOverride = FindProperty("_HoverColorOverride", props);
            hoverLightOpaque = FindProperty("_HoverLightOpaque", props);
            enableHoverColorOpaqueOverride = FindProperty("_EnableHoverColorOpaqueOverride", props);
            hoverColorOverrideOpaque = FindProperty("_HoverColorOpaqueOverride", props);
            roundCorners = FindProperty("_RoundCorners", props);
            roundCornerRadius = FindProperty("_RoundCornerRadius", props);
            roundCornerMargin = FindProperty("_RoundCornerMargin", props);
            borderLight = FindProperty("_BorderLight", props);
            borderLightUsesHoverColor = FindProperty("_BorderLightUsesHoverColor", props);
            borderLightOpaque = FindProperty("_BorderLightOpaque", props);
            borderWidth = FindProperty("_BorderWidth", props);
            borderMinValue = FindProperty("_BorderMinValue", props);
            edgeSmoothingValue = FindProperty("_EdgeSmoothingValue", props);
            innerGlow = FindProperty("_InnerGlow", props);
            innerGlowColor = FindProperty("_InnerGlowColor", props);
            environmentColoring = FindProperty("_EnvironmentColoring", props);
            environmentColorThreshold = FindProperty("_EnvironmentColorThreshold", props);
            environmentColorIntensity = FindProperty("_EnvironmentColorIntensity", props);
            environmentColorX = FindProperty("_EnvironmentColorX", props);
            environmentColorY = FindProperty("_EnvironmentColorY", props);
            environmentColorZ = FindProperty("_EnvironmentColorZ", props);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            FindProperties(props);
            Initilize(material);

            RenderingModeOptions(materialEditor);
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
            SetFloatProperty(material, null, "_Smoothness", smoothness);
            SetFloatProperty(material, "_DIRECTIONAL_LIGHT", "_DirectionalLight", diffuse);
            SetFloatProperty(material, "_SPECULAR_HIGHLIGHTS", "_SpecularHighlights", specularHighlights);
            SetFloatProperty(material, "_NORMAL_MAP", "_EnableNormalMap", normalMap);

            if (normalMapTexture)
            {
                material.SetTexture("_NormalMap", normalMapTexture);
            }

            SetFloatProperty(material, "_EMISSION", "_EnableEmission", emission);
            SetColorProperty(material, "_EmissiveColor", emissionColor);
            SetFloatProperty(material, "_REFLECTIONS", "_Reflections", reflections);
            SetFloatProperty(material, "_RIM_LIGHT", "_RimLight", rimLighting);
            SetVectorProperty(material, "_MainTex_ST", textureScaleOffset);
            SetFloatProperty(material, null, "_CullMode", cullMode);

            // Setup the rendering mode based on the old shader.
            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialWithRenderingMode(material, (RenderingMode)material.GetFloat(Styles.renderingModeName), CustomRenderingMode.Opaque, -1);
            }
            else
            {
                RenderingMode mode = RenderingMode.Opaque;

                if (oldShader.name.Contains("/Transparent/Cutout/"))
                {
                    mode = RenderingMode.TransparentCutout;
                }
                else if (oldShader.name.Contains("/Transparent/"))
                {
                    mode = RenderingMode.Transparent;
                }

                material.SetFloat(Styles.renderingModeName, (float)mode);

                MaterialChanged(material);
            }
        }

        protected void Initilize(Material material)
        {
            if (!initilized)
            {
                MaterialChanged(material);
                initilized = true;
            }
        }

        protected void MaterialChanged(Material material)
        {
            SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode);
            SetupMaterialWithRenderingMode(material, (RenderingMode)renderingMode.floatValue, (CustomRenderingMode)customRenderingMode.floatValue, (int)renderQueueOverride.floatValue);
        }

        protected void RenderingModeOptions(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = renderingMode.hasMixedValue;
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = (RenderingMode)EditorGUILayout.Popup(renderingMode.displayName, (int)mode, Styles.renderingModeNames);

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(renderingMode.displayName);
                renderingMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                UnityEngine.Object[] targets = renderingMode.targets;

                foreach (UnityEngine.Object target in targets)
                {
                    MaterialChanged((Material)target);
                }
            }

            if ((RenderingMode)renderingMode.floatValue == RenderingMode.Custom)
            {
                EditorGUI.indentLevel += 2;
                customRenderingMode.floatValue = EditorGUILayout.Popup(customRenderingMode.displayName, (int)customRenderingMode.floatValue, Styles.customRenderingModeNames);
                materialEditor.ShaderProperty(sourceBlend, Styles.sourceBlend);
                materialEditor.ShaderProperty(destinationBlend, Styles.destinationBlend);
                materialEditor.ShaderProperty(blendOperation, Styles.blendOperation);
                materialEditor.ShaderProperty(depthTest, Styles.depthTest);
                depthWrite.floatValue = EditorGUILayout.Popup(depthWrite.displayName, (int)depthWrite.floatValue, Styles.depthWriteNames);
                materialEditor.ShaderProperty(colorWriteMask, Styles.colorWriteMask);
                EditorGUI.indentLevel -= 2;
            }

            materialEditor.ShaderProperty(cullMode, Styles.cullMode);
        }

        protected void MainMapOptions(MaterialEditor materialEditor, Material material)
        {
            GUILayout.Label(Styles.primaryMapsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);

            materialEditor.TexturePropertySingleLine(Styles.albedo, albedoMap, albedoColor);

            EditorGUI.indentLevel += 2;

            albedoAlphaMode.floatValue = EditorGUILayout.Popup(albedoAlphaMode.displayName, (int)albedoAlphaMode.floatValue, Styles.albedoAlphaModeNames);

            if ((RenderingMode)renderingMode.floatValue == RenderingMode.TransparentCutout)
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

            SetupMaterialWithAlbedo(material, albedoMap, albedoAlphaMode);

            EditorGUI.indentLevel -= 2;

            if (PropertyEnabled(directionalLight) ||
                PropertyEnabled(reflections) ||
                PropertyEnabled(rimLight) ||
                PropertyEnabled(environmentColoring))
            {
                materialEditor.ShaderProperty(enableNormalMap, Styles.enableNormalMap);

                if (PropertyEnabled(enableNormalMap))
                {
                    materialEditor.TexturePropertySingleLine(Styles.normalMap, normalMap);
                }
            }

            materialEditor.ShaderProperty(enableEmission, Styles.enableEmission);

            if (PropertyEnabled(enableEmission))
            {
                materialEditor.ShaderProperty(emissiveColor, Styles.emissiveColor, 2);
            }

            EditorGUILayout.Space();
            materialEditor.TextureScaleOffsetProperty(albedoMap);
        }

        protected void RenderingOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.renderingOptionsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);

            materialEditor.ShaderProperty(directionalLight, Styles.directionalLight);

            if (PropertyEnabled(directionalLight))
            {
                materialEditor.ShaderProperty(specularHighlights, Styles.specularHighlights, 2);
            }

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

            materialEditor.ShaderProperty(clippingPlane, Styles.clippingPlane);

            if (PropertyEnabled(clippingPlane))
            {
                materialEditor.ShaderProperty(clipPlane, Styles.clipPlane, 2);
                materialEditor.ShaderProperty(clippingPlaneBorder, Styles.clippingPlaneBorder, 2);

                if (PropertyEnabled(clippingPlaneBorder))
                {
                    materialEditor.ShaderProperty(clippingPlaneBorderWidth, Styles.clippingPlaneBorderWidth, 4);
                    materialEditor.ShaderProperty(clippingPlaneBorderColor, Styles.clippingPlaneBorderColor, 4);
                }
            }

            materialEditor.ShaderProperty(nearPlaneFade, Styles.nearPlaneFade);

            if (PropertyEnabled(nearPlaneFade))
            {
                materialEditor.ShaderProperty(fadeBeginDistance, Styles.fadeBeginDistance, 2);
                materialEditor.ShaderProperty(fadeCompleteDistance, Styles.fadeCompleteDistance, 2);
            }
        }

        protected void FluentOptions(MaterialEditor materialEditor, Material material)
        {
            EditorGUILayout.Space();
            GUILayout.Label(Styles.fluentOptionsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            CustomRenderingMode customMode = (CustomRenderingMode)customRenderingMode.floatValue;

            materialEditor.ShaderProperty(hoverLight, Styles.hoverLight);

            if (PropertyEnabled(hoverLight))
            {
                materialEditor.ShaderProperty(enableHoverColorOverride, Styles.enableHoverColorOverride, 2);

                if (PropertyEnabled(enableHoverColorOverride))
                {
                    materialEditor.ShaderProperty(hoverColorOverride, Styles.hoverColorOverride, 4);
                }

                if (mode == RenderingMode.Transparent || (mode == RenderingMode.Custom && customMode == CustomRenderingMode.Transparent))
                {
                    materialEditor.ShaderProperty(hoverLightOpaque, Styles.hoverLightOpaque, 2);
                }

                if (PropertyEnabled(hoverLightOpaque))
                {
                    materialEditor.ShaderProperty(enableHoverColorOpaqueOverride, Styles.enableHoverColorOpaqueOverride, 4);

                    if (PropertyEnabled(enableHoverColorOpaqueOverride))
                    {
                        materialEditor.ShaderProperty(hoverColorOverrideOpaque, Styles.hoverColorOpaqueOverride, 6);
                    }
                }
            }

            materialEditor.ShaderProperty(roundCorners, Styles.roundCorners);

            if (PropertyEnabled(roundCorners))
            {
                materialEditor.ShaderProperty(roundCornerRadius, Styles.roundCornerRadius, 2);
                materialEditor.ShaderProperty(roundCornerMargin, Styles.roundCornerMargin, 2);
            }

            materialEditor.ShaderProperty(borderLight, Styles.borderLight);

            if (PropertyEnabled(borderLight))
            {
                if (PropertyEnabled(hoverLight))
                {
                    materialEditor.ShaderProperty(borderLightUsesHoverColor, Styles.borderLightUsesHoverColor, 2);
                }

                if (mode == RenderingMode.TransparentCutout || mode == RenderingMode.Transparent ||
                    (mode == RenderingMode.Custom && customMode == CustomRenderingMode.TransparentCutout) ||
                    (mode == RenderingMode.Custom && customMode == CustomRenderingMode.Transparent))
                {
                    materialEditor.ShaderProperty(borderLightOpaque, Styles.borderLightOpaque, 2);
                }

                materialEditor.ShaderProperty(borderWidth, Styles.borderWidth, 2);

                if (!PropertyEnabled(borderLightOpaque))
                {
                    materialEditor.ShaderProperty(borderMinValue, Styles.borderMinValue, 2);
                }
            }

            if (PropertyEnabled(roundCorners) || PropertyEnabled(borderLight))
            {
                materialEditor.ShaderProperty(edgeSmoothingValue, Styles.edgeSmoothingValue);
            }

            materialEditor.ShaderProperty(innerGlow, Styles.innerGlow);

            if (PropertyEnabled(innerGlow))
            {
                materialEditor.ShaderProperty(innerGlowColor, Styles.innerGlowColor, 2);
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
            GUILayout.Label(Styles.advancedOptionsTitle, EditorStyles.boldLabel, new GUILayoutOption[0]);

            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(renderQueueOverride, Styles.renderQueueOverride);

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            // Show the RenderQueueField but do not allow users to directly manipulate it. That is done via the renderQueueOverride.
            GUI.enabled = false;
            materialEditor.RenderQueueField();

            // When round corner or border light features are used, enable instancing to disable batching. Static and dynamic 
            // batching will normalize the object scale, which breakes border realted features.
            GUI.enabled = !PropertyEnabled(roundCorners) && !PropertyEnabled(borderLight);

            if (!GUI.enabled && !material.enableInstancing)
            {
                material.enableInstancing = true;
            }

            materialEditor.EnableInstancingField();
        }

        protected static void SetupMaterialWithAlbedo(Material material, MaterialProperty albedoMap, MaterialProperty albedoAlphaMode)
        {
            if (albedoMap.textureValue)
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
                    }
                    break;

                case AlbedoAlphaMode.Metallic:
                    {
                        material.EnableKeyword(Styles.albedoMapAlphaMetallicName);
                        material.DisableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    }
                    break;

                case AlbedoAlphaMode.Smoothness:
                    {
                        material.DisableKeyword(Styles.albedoMapAlphaMetallicName);
                        material.EnableKeyword(Styles.albedoMapAlphaSmoothnessName);
                    }
                    break;
            }
        }

        protected static void SetupMaterialWithRenderingMode(Material material, RenderingMode mode, CustomRenderingMode customMode, int renderQueueOverride)
        {
            switch (mode)
            {
                case RenderingMode.Opaque:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.renderingModeNames[(int)RenderingMode.Opaque]);
                        material.SetInt(Styles.customRenderingModeName, (int)CustomRenderingMode.Opaque);
                        material.SetInt(Styles.sourceBlendName, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(Styles.destinationBlendName, (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt(Styles.blendOperationName, (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt(Styles.depthTestName, (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetInt(Styles.depthWriteName, (int)DepthWrite.On);
                        material.SetInt(Styles.colorWriteMaskName, (int)UnityEngine.Rendering.ColorWriteMask.All);
                        material.DisableKeyword(Styles.alphaTestOnName);
                        material.DisableKeyword(Styles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)UnityEngine.Rendering.RenderQueue.Geometry;
                    }
                    break;

                case RenderingMode.TransparentCutout:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.renderingModeNames[(int)RenderingMode.TransparentCutout]);
                        material.SetInt(Styles.customRenderingModeName, (int)CustomRenderingMode.TransparentCutout);
                        material.SetInt(Styles.sourceBlendName, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(Styles.destinationBlendName, (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt(Styles.blendOperationName, (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt(Styles.depthTestName, (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetInt(Styles.depthWriteName, (int)DepthWrite.On);
                        material.SetInt(Styles.colorWriteMaskName, (int)UnityEngine.Rendering.ColorWriteMask.All);
                        material.EnableKeyword(Styles.alphaTestOnName);
                        material.DisableKeyword(Styles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    }
                    break;

                case RenderingMode.Transparent:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(Styles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(Styles.sourceBlendName, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt(Styles.destinationBlendName, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt(Styles.blendOperationName, (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt(Styles.depthTestName, (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetInt(Styles.depthWriteName, (int)DepthWrite.Off);
                        material.SetInt(Styles.colorWriteMaskName, (int)UnityEngine.Rendering.ColorWriteMask.All);
                        material.DisableKeyword(Styles.alphaTestOnName);
                        material.EnableKeyword(Styles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.PremultipliedTransparent:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(Styles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(Styles.sourceBlendName, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(Styles.destinationBlendName, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt(Styles.blendOperationName, (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt(Styles.depthTestName, (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetInt(Styles.depthWriteName, (int)DepthWrite.Off);
                        material.SetInt(Styles.colorWriteMaskName, (int)UnityEngine.Rendering.ColorWriteMask.All);
                        material.DisableKeyword(Styles.alphaTestOnName);
                        material.EnableKeyword(Styles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Additive:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(Styles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(Styles.sourceBlendName, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(Styles.destinationBlendName, (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt(Styles.blendOperationName, (int)UnityEngine.Rendering.BlendOp.Add);
                        material.SetInt(Styles.depthTestName, (int)UnityEngine.Rendering.CompareFunction.LessEqual);
                        material.SetInt(Styles.depthWriteName, (int)DepthWrite.Off);
                        material.SetInt(Styles.colorWriteMaskName, (int)UnityEngine.Rendering.ColorWriteMask.All);
                        material.DisableKeyword(Styles.alphaTestOnName);
                        material.EnableKeyword(Styles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Custom:
                    {
                        material.SetOverrideTag(Styles.renderTypeName, Styles.customRenderingModeNames[(int)customMode]);
                        // _SrcBlend, _DstBlend, _BlendOp, _ZTest, _ZWrite, _ColorWriteMask are controlled by UI.

                        switch (customMode)
                        {
                            case CustomRenderingMode.Opaque:
                                {
                                    
                                    material.DisableKeyword(Styles.alphaTestOnName);
                                    material.DisableKeyword(Styles.alphaBlendOnName);
                                }
                                break;

                            case CustomRenderingMode.TransparentCutout:
                                {
                                    material.EnableKeyword(Styles.alphaTestOnName);
                                    material.DisableKeyword(Styles.alphaBlendOnName);
                                }
                                break;

                            case CustomRenderingMode.Transparent:
                                {
                                    material.DisableKeyword(Styles.alphaTestOnName);
                                    material.EnableKeyword(Styles.alphaBlendOnName);
                                }
                                break;
                        }

                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : material.renderQueue;
                    }
                    break;
            }
        }

        protected static bool PropertyEnabled(MaterialProperty property)
        {
            return (property.floatValue != 0.0f);
        }

        protected static float? GetFloatProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetFloat(propertyName);
            }

            return null;
        }

        protected static Vector4? GetVectorProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetVector(propertyName);
            }

            return null;
        }

        protected static Color? GetColorProperty(Material material, string propertyName)
        {
            if (material.HasProperty(propertyName))
            {
                return material.GetColor(propertyName);
            }

            return null;
        }

        protected static void SetFloatProperty(Material material, string keywordName, string propertyName, float? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                if (keywordName != null)
                {
                    if (propertyValue.Value != 0.0f)
                    {
                        material.EnableKeyword(keywordName);
                    }
                    else
                    {
                        material.DisableKeyword(keywordName);
                    }
                }

                material.SetFloat(propertyName, propertyValue.Value);
            }
        }

        protected static void SetVectorProperty(Material material, string propertyName, Vector4? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                material.SetVector(propertyName, propertyValue.Value);
            }
        }

        protected static void SetColorProperty(Material material, string propertyName, Color? propertyValue)
        {
            if (propertyValue.HasValue)
            {
                material.SetColor(propertyName, propertyValue.Value);
            }
        }
    }
}
