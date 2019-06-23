// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom shader inspector for the "Mixed Reality Toolkit/Wireframe" shader.
    /// </summary>
    public class MixedRealityWireframeShaderGUI : MixedRealityShaderGUI
    {
        protected static class Styles
        {
            public static string mainPropertiesTitle = "Main Properties";
            public static string advancedOptionsTitle = "Advanced Options";
            public static GUIContent baseColor = new GUIContent("Base Color", "Color of faces");
            public static GUIContent wireColor = new GUIContent("Wire Color", "Color of wires");
            public static GUIContent wireThickness = new GUIContent("Wire Thickness", "Thickness of wires");
        }

        protected bool initialized;

        protected MaterialProperty renderingMode;
        protected MaterialProperty customRenderingMode;
        protected MaterialProperty sourceBlend;
        protected MaterialProperty destinationBlend;
        protected MaterialProperty blendOperation;
        protected MaterialProperty depthTest;
        protected MaterialProperty depthWrite;
        protected MaterialProperty depthOffsetFactor;
        protected MaterialProperty depthOffsetUnits;
        protected MaterialProperty colorWriteMask;
        protected MaterialProperty cullMode;
        protected MaterialProperty renderQueueOverride;

        protected MaterialProperty baseColor;
        protected MaterialProperty wireColor;
        protected MaterialProperty wireThickness;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            FindProperties(props);
            Initialize(material);

            RenderingModeOptions(materialEditor);

            GUILayout.Label(Styles.mainPropertiesTitle, EditorStyles.boldLabel);
            materialEditor.ShaderProperty(baseColor, Styles.baseColor);
            materialEditor.ShaderProperty(wireColor, Styles.wireColor);
            materialEditor.ShaderProperty(wireThickness, Styles.wireThickness);

            AdvancedOptions(materialEditor, material);
        }

        protected void FindProperties(MaterialProperty[] props)
        {
            renderingMode = FindProperty(BaseStyles.renderingModeName, props);
            customRenderingMode = FindProperty(BaseStyles.customRenderingModeName, props);
            sourceBlend = FindProperty(BaseStyles.sourceBlendName, props);
            destinationBlend = FindProperty(BaseStyles.destinationBlendName, props);
            blendOperation = FindProperty(BaseStyles.blendOperationName, props);
            depthTest = FindProperty(BaseStyles.depthTestName, props);
            depthWrite = FindProperty(BaseStyles.depthWriteName, props);
            depthOffsetFactor = FindProperty(BaseStyles.depthOffsetFactorName, props);
            depthOffsetUnits = FindProperty(BaseStyles.depthOffsetUnitsName, props);
            colorWriteMask = FindProperty(BaseStyles.colorWriteMaskName, props);

            cullMode = FindProperty("_CullMode", props);
            renderQueueOverride = FindProperty("_RenderQueueOverride", props);

            baseColor = FindProperty("_BaseColor", props);
            wireColor = FindProperty("_WireColor", props);
            wireThickness = FindProperty("_WireThickness", props);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            float? cullMode = GetFloatProperty(material, "_Cull");

            base.AssignNewShaderToMaterial(material, oldShader, newShader);
   
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
                    mode = RenderingMode.TransparentCutout;
                }
                else if (oldShader.name.Contains(TransparentShadersPath))
                {
                    mode = RenderingMode.Transparent;
                }

                material.SetFloat(BaseStyles.renderingModeName, (float)mode);

                MaterialChanged(material);
            }
        }

        protected void Initialize(Material material)
        {
            if (!initialized)
            {
                MaterialChanged(material);
                initialized = true;
            }
        }

        protected void MaterialChanged(Material material)
        {
            SetupMaterialWithRenderingMode(material, 
                (RenderingMode)renderingMode.floatValue, 
                (CustomRenderingMode)customRenderingMode.floatValue, 
                (int)renderQueueOverride.floatValue);
        }

        protected void RenderingModeOptions(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = renderingMode.hasMixedValue;
            RenderingMode mode = (RenderingMode)renderingMode.floatValue;
            EditorGUI.BeginChangeCheck();
            mode = (RenderingMode)EditorGUILayout.Popup(renderingMode.displayName, (int)mode, BaseStyles.renderingModeNames);

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(renderingMode.displayName);
                renderingMode.floatValue = (float)mode;
            }

            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                Object[] targets = renderingMode.targets;

                foreach (Object target in targets)
                {
                    MaterialChanged((Material)target);
                }
            }

            if ((RenderingMode)renderingMode.floatValue == RenderingMode.Custom)
            {
                EditorGUI.indentLevel += 2;
                customRenderingMode.floatValue = EditorGUILayout.Popup(customRenderingMode.displayName, (int)customRenderingMode.floatValue, BaseStyles.customRenderingModeNames);
                materialEditor.ShaderProperty(sourceBlend, BaseStyles.sourceBlend);
                materialEditor.ShaderProperty(destinationBlend, BaseStyles.destinationBlend);
                materialEditor.ShaderProperty(blendOperation, BaseStyles.blendOperation);
                materialEditor.ShaderProperty(depthTest, BaseStyles.depthTest);
                depthWrite.floatValue = EditorGUILayout.Popup(depthWrite.displayName, (int)depthWrite.floatValue, BaseStyles.depthWriteNames);
                materialEditor.ShaderProperty(depthOffsetFactor, BaseStyles.depthOffsetFactor);
                materialEditor.ShaderProperty(depthOffsetUnits, BaseStyles.depthOffsetUnits);
                materialEditor.ShaderProperty(colorWriteMask, BaseStyles.colorWriteMask);
                EditorGUI.indentLevel -= 2;
            }

            if (!PropertyEnabled(depthWrite))
            {
                if (MixedRealityToolkitShaderGUIUtilities.DisplayDepthWriteWarning(materialEditor))
                {
                    renderingMode.floatValue = (float)RenderingMode.Custom;
                    depthWrite.floatValue = (float)DepthWrite.On;
                }
            }

            materialEditor.ShaderProperty(cullMode, BaseStyles.cullMode);
        }

        protected void AdvancedOptions(MaterialEditor materialEditor, Material material)
        {
            GUILayout.Label(Styles.advancedOptionsTitle, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            materialEditor.ShaderProperty(renderQueueOverride, BaseStyles.renderQueueOverride);

            if (EditorGUI.EndChangeCheck())
            {
                MaterialChanged(material);
            }

            // Show the RenderQueueField but do not allow users to directly manipulate it. That is done via the renderQueueOverride.
            GUI.enabled = false;
            materialEditor.RenderQueueField();
            GUI.enabled = true;

            materialEditor.EnableInstancingField();
        }
    }
}
