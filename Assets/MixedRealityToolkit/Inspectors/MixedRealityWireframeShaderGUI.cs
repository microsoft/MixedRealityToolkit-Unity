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

        protected MaterialProperty baseColor;
        protected MaterialProperty wireColor;
        protected MaterialProperty wireThickness;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            Material material = (Material)materialEditor.target;

            base.OnGUI(materialEditor, props);

            GUILayout.Label(Styles.mainPropertiesTitle, EditorStyles.boldLabel);
            materialEditor.ShaderProperty(baseColor, Styles.baseColor);
            materialEditor.ShaderProperty(wireColor, Styles.wireColor);
            materialEditor.ShaderProperty(wireThickness, Styles.wireThickness);

            AdvancedOptions(materialEditor, material);
        }

        protected override void FindProperties(MaterialProperty[] props)
        {
            base.FindProperties(props);

            baseColor = FindProperty("_BaseColor", props);
            wireColor = FindProperty("_WireColor", props);
            wireThickness = FindProperty("_WireThickness", props);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            float? cullMode = GetFloatProperty(material, "_Cull");

            base.AssignNewShaderToMaterial(material, oldShader, newShader);
   
            SetShaderFeatureActive(material, null, BaseStyles.cullModeName, cullMode);

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
