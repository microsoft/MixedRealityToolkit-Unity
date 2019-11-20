// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// A custom shader inspector for the "Mixed Reality Toolkit/TextShader3D".
    /// </summary>
    public class Text3DShaderGUI : ShaderGUI
    {
        protected bool firstTimeApply = true;
        protected MaterialProperty cullMode;


        public override void OnGUI(MaterialEditor matEditor, MaterialProperty[] props)
        {
            // Make sure that needed setup (i.e. keywords/renderqueue) are set up if we're switching from an existing material.
            // Do this before any GUI code has been issued to prevent layout issues in subsequent GUILayout statements (case 780071)
            if (firstTimeApply)
            {
                firstTimeApply = false;
            }

            EditorGUIUtility.labelWidth = 0f;

            EditorGUI.BeginChangeCheck();
            {
                base.OnGUI(matEditor, props);
            }
        }

        protected static class Styles
        {
            public static GUIContent cullMode = new GUIContent("Culling Mode", "Type of culling to apply to polygons - typically this is set to backfacing");
        }
    }
}
