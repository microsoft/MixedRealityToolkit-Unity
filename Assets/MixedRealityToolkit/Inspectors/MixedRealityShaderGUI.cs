// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom shader inspector for the "Mixed Reality Toolkit/Wireframe" shader.
    /// </summary>
    public abstract class MixedRealityShaderGUI : ShaderGUI
    {
        protected enum RenderingMode
        {
            Opaque,
            TransparentCutout,
            Transparent,
            PremultipliedTransparent,
            Additive,
            Custom
        }

        protected enum CustomRenderingMode
        {
            Opaque,
            TransparentCutout,
            Transparent
        }

        protected enum DepthWrite
        {
            Off,
            On
        }

        protected static class BaseStyles
        {
            public static string renderingOptionsTitle = "Rendering Options";
            public static string advancedOptionsTitle = "Advanced Options";
            public static string renderTypeName = "RenderType";
            public static string renderingModeName = "_Mode";
            public static string customRenderingModeName = "_CustomMode";
            public static string sourceBlendName = "_SrcBlend";
            public static string destinationBlendName = "_DstBlend";
            public static string blendOperationName = "_BlendOp";
            public static string depthTestName = "_ZTest";
            public static string depthWriteName = "_ZWrite";
            public static string depthOffsetFactorName = "_ZOffsetFactor";
            public static string depthOffsetUnitsName = "_ZOffsetUnits";
            public static string colorWriteMaskName = "_ColorWriteMask";

            public static string alphaTestOnName = "_ALPHATEST_ON";
            public static string alphaBlendOnName = "_ALPHABLEND_ON";

            public static readonly string[] renderingModeNames = Enum.GetNames(typeof(RenderingMode));
            public static readonly string[] customRenderingModeNames = Enum.GetNames(typeof(CustomRenderingMode));
            public static readonly string[] depthWriteNames = Enum.GetNames(typeof(DepthWrite));
            public static GUIContent sourceBlend = new GUIContent("Source Blend", "Blend Mode of Newly Calculated Color");
            public static GUIContent destinationBlend = new GUIContent("Destination Blend", "Blend Mode of Existing Color");
            public static GUIContent blendOperation = new GUIContent("Blend Operation", "Operation for Blending New Color With Existing Color");
            public static GUIContent depthTest = new GUIContent("Depth Test", "How Should Depth Testing Be Performed.");
            public static GUIContent depthWrite = new GUIContent("Depth Write", "Controls Whether Pixels From This Material Are Written to the Depth Buffer");
            public static GUIContent depthOffsetFactor = new GUIContent("Depth Offset Factor", "Scales the Maximum Z Slope, with Respect to X or Y of the Polygon");
            public static GUIContent depthOffsetUnits = new GUIContent("Depth Offset Units", "Scales the Minimum Resolvable Depth Buffer Value");
            public static GUIContent colorWriteMask = new GUIContent("Color Write Mask", "Color Channel Writing Mask");
            public static GUIContent cullMode = new GUIContent("Cull Mode", "Triangle Culling Mode");
            public static GUIContent renderQueueOverride = new GUIContent("Render Queue Override", "Manually Override the Render Queue");
        }

        protected static void SetupMaterialWithRenderingMode(Material material, RenderingMode mode, CustomRenderingMode customMode, int renderQueueOverride)
        {
            switch (mode)
            {
                case RenderingMode.Opaque:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Opaque]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)CustomRenderingMode.Opaque);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.Zero);
                        material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                        material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.On);
                        material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                        material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                        material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Geometry;
                    }
                    break;

                case RenderingMode.TransparentCutout:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.TransparentCutout]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)CustomRenderingMode.TransparentCutout);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.Zero);
                        material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                        material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.On);
                        material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                        material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                        material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                        material.EnableKeyword(BaseStyles.alphaTestOnName);
                        material.DisableKeyword(BaseStyles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.AlphaTest;
                    }
                    break;

                case RenderingMode.Transparent:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.SrcAlpha);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.OneMinusSrcAlpha);
                        material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                        material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                        material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                        material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.EnableKeyword(BaseStyles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.PremultipliedTransparent:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.OneMinusSrcAlpha);
                        material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                        material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                        material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                        material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.EnableKeyword(BaseStyles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Additive:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.renderingModeNames[(int)RenderingMode.Transparent]);
                        material.SetInt(BaseStyles.customRenderingModeName, (int)CustomRenderingMode.Transparent);
                        material.SetInt(BaseStyles.sourceBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.destinationBlendName, (int)BlendMode.One);
                        material.SetInt(BaseStyles.blendOperationName, (int)BlendOp.Add);
                        material.SetInt(BaseStyles.depthTestName, (int)CompareFunction.LessEqual);
                        material.SetInt(BaseStyles.depthWriteName, (int)DepthWrite.Off);
                        material.SetFloat(BaseStyles.depthOffsetFactorName, 0.0f);
                        material.SetFloat(BaseStyles.depthOffsetUnitsName, 0.0f);
                        material.SetInt(BaseStyles.colorWriteMaskName, (int)ColorWriteMask.All);
                        material.DisableKeyword(BaseStyles.alphaTestOnName);
                        material.EnableKeyword(BaseStyles.alphaBlendOnName);
                        material.renderQueue = (renderQueueOverride >= 0) ? renderQueueOverride : (int)RenderQueue.Transparent;
                    }
                    break;

                case RenderingMode.Custom:
                    {
                        material.SetOverrideTag(BaseStyles.renderTypeName, BaseStyles.customRenderingModeNames[(int)customMode]);
                        // _SrcBlend, _DstBlend, _BlendOp, _ZTest, _ZWrite, _ColorWriteMask are controlled by UI.

                        switch (customMode)
                        {
                            case CustomRenderingMode.Opaque:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendOnName);
                                }
                                break;

                            case CustomRenderingMode.TransparentCutout:
                                {
                                    material.EnableKeyword(BaseStyles.alphaTestOnName);
                                    material.DisableKeyword(BaseStyles.alphaBlendOnName);
                                }
                                break;

                            case CustomRenderingMode.Transparent:
                                {
                                    material.DisableKeyword(BaseStyles.alphaTestOnName);
                                    material.EnableKeyword(BaseStyles.alphaBlendOnName);
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
            return !property.floatValue.Equals(0.0f);
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
                    if (!propertyValue.Value.Equals(0.0f))
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
