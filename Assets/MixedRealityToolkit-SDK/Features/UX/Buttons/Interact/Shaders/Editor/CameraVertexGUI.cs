// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace Interact
{
    /// <summary>
    /// Editor for FastConfigurable shader
    /// </summary>
    public class CameraVertexGUI : ShaderGUI
    {
        protected bool firstTimeApply = true;
        protected MaterialProperty opacity;

        public enum BlendMode
        {
            Opaque,
            Transparent
        }

        public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props)
        {
            // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly
            CacheOutputConfigurationProperties(props);
            
            // Make sure that needed setup (ie keywords/renderqueue) are set up if we're switching from an existing material.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (firstTimeApply)
            {
                firstTimeApply = false;
            }

            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                base.OnGUI(matEditor, props);
                ShowMainGUI(matEditor);
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in opacity.targets)
                {
                    var mat = obj as Material;
                    SetMaterialBlendMode(mat, (BlendMode)opacity.floatValue);
                }
            }
        }

        protected virtual void SetMaterialBlendMode(Material mat, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    Debug.Log("A: " + blendMode);
                    mat.SetOverrideTag("RenderType", "");
                    mat.SetOverrideTag("Queue", "Geometry");
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    mat.renderQueue = -1;
                    break;
                case BlendMode.Transparent:
                    Debug.Log("B: " + blendMode);
                    mat.SetOverrideTag("RenderType", "Transparent");
                    mat.SetOverrideTag("Queue", "Transparent");
                    //non pre-multiplied alpha
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
        }

        protected virtual void ShowMainGUI(MaterialEditor matEditor)
        {
            //matEditor.ShaderProperty(opacity, Styles.blendMode);
        }
        
        protected virtual void CacheOutputConfigurationProperties(MaterialProperty[] props)
        {
            opacity = FindProperty("_Opacity", props);
        }

        protected static class Styles
        {
            public static GUIContent blendMode = new GUIContent("Blending Mode", "Type of blending to apply to polygons or opacity");
        }
    }
}