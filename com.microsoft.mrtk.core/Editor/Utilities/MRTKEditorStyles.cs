// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class MRTKEditorStyles
    {
        /// <summary>
        /// Default style for foldouts with bold title
        /// </summary>
        public static readonly GUIStyle BoldFoldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };

        /// <summary>
        /// Default style for foldouts with bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldTitleFoldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = InspectorUIUtility.TitleFontSize,
        };

        /// <summary>
        /// Default style for foldouts with large font size title
        /// </summary>
        public static readonly GUIStyle TitleFoldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontSize = InspectorUIUtility.TitleFontSize,
        };

        /// <summary>
        /// Default style for controller mapping buttons
        /// </summary>
        public static readonly GUIStyle ControllerButtonStyle = new GUIStyle("iconButton")
        {
            imagePosition = ImagePosition.ImageAbove,
            fixedHeight = 128,
            fontStyle = FontStyle.Bold,
            stretchHeight = true,
            stretchWidth = true,
            wordWrap = true,
            fontSize = 10,
            alignment = TextAnchor.UpperCenter,
            fixedWidth = 0,
            margin = new RectOffset(0, 0, 0, 0)
        };

        /// <summary>
        /// Default style for bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldLargeTitleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = InspectorUIUtility.TitleFontSize,
        };

        internal static readonly GUIStyle LicenseStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };
        
        /// <summary>
        /// Reusable GUIStyle for drawing the MRTK product name label.
        /// </summary>
        public static readonly GUIStyle ProductNameStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 26,
            alignment = TextAnchor.UpperCenter,
            fixedHeight = 32
        };

        /// <summary>
        /// Box style with left margin
        /// </summary>
        public static GUIStyle BoxStyle(int margin)
        {
            GUIStyle box = new GUIStyle(GUI.skin.box);
            box.margin.left = margin;
            return box;
        }

        /// <summary>
        /// Help box style with left margin
        /// </summary>
        /// <param name="margin">amount of left margin</param>
        /// <returns>Configured helpbox GUIStyle</returns>
        public static GUIStyle HelpBoxStyle(int margin)
        {
            GUIStyle box = new GUIStyle(EditorStyles.helpBox);
            box.margin.left = margin;
            return box;
        }

        /// <summary>
        /// Create a custom label style based on color and size
        /// </summary>
        public static GUIStyle LabelStyle(int size, Color color)
        {
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = size;
            labelStyle.fixedHeight = size * 2;
            labelStyle.normal.textColor = color;
            return labelStyle;
        }
    }
}
