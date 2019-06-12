// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public static class MixedRealityEditorUtility
    {
        public static readonly Texture2D LogoLightTheme = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath("StandardAssets/Textures/MRTK_Logo_Black.png"), typeof(Texture2D));

        public static readonly Texture2D LogoDarkTheme = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath("StandardAssets/Textures/MRTK_Logo_White.png"), typeof(Texture2D));

        private static readonly Texture HelpIcon = EditorGUIUtility.IconContent("_Help").image;

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        public static void RenderMixedRealityToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? LogoDarkTheme : LogoLightTheme, GUILayout.MaxHeight(96f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        /// <summary>
        /// Helper function to render buttons correctly indented according to EditorGUI.indentLevel since GUILayout component don't respond naturally
        /// </summary>
        /// <param name="buttonText">text to place in button</param>
        /// <param name="options">layout options</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(string buttonText, params GUILayoutOption[] options)
        {
            return RenderIndentedButton(() => { return GUILayout.Button(buttonText, options); });
        }

        /// <summary>
        /// Helper function to render buttons correctly indented according to EditorGUI.indentLevel since GUILayout component don't respond naturally
        /// </summary>
        /// <param name="content">What to draw in button</param>
        /// <param name="style">Style configuration for button</param>
        /// <param name="options">layout options</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return RenderIndentedButton(() => { return GUILayout.Button(content, style, options); });
        }

        /// <summary>
        /// Helper function to support primary overloaded version of this functionality
        /// </summary>
        /// <param name="renderButton">The code to render button correctly based on parameter types passed</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        public static bool RenderIndentedButton(Func<bool> renderButton)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
            result = renderButton();
            GUILayout.EndHorizontal();
            return result;
        }
    }
}
