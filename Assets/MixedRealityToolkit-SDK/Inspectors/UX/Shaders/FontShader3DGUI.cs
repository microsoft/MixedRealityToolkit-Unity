// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    /// <summary>
    /// Editor for FastConfigurable shader
    /// </summary>
    public class FontShader3DGUI : ShaderGUI
    {
        protected bool firstTimeApply = true;
        protected MaterialProperty cullMode;


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
        }
            
        protected virtual void ShowMainGUI(MaterialEditor matEditor)
        {
            //matEditor.ShaderProperty(cullMode, Styles.cullMode);
            // add special property
        }
        
        protected virtual void CacheOutputConfigurationProperties(MaterialProperty[] props)
        {
            //cullMode = FindProperty("_Cull", props);
            // cache property values
        }

        protected static class Styles
        {
            public static GUIContent cullMode = new GUIContent("Culling Mode", "Type of culling to apply to polygons - typically this is set to backfacing");
        }
    }
}