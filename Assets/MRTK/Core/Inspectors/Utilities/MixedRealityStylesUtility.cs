// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    public static class MixedRealityStylesUtility
    {
        /// <summary>
        /// Default style for foldouts with bold title
        /// </summary>
        public static readonly GUIStyle BoldFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold
          };

        /// <summary>
        /// Default style for foldouts with bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldTitleFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold,
              fontSize = InspectorUIUtility.TitleFontSize,
          };

        /// <summary>
        /// Default style for foldouts with large font size title
        /// </summary>
        public static readonly GUIStyle TitleFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontSize = InspectorUIUtility.TitleFontSize,
          };

        /// <summary>
        /// Default style for large button
        /// </summary>
        public static readonly GUIStyle ControllerButtonStyle = new GUIStyle("LargeButton")
        {
            imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold,
                    stretchHeight = true,
                    stretchWidth = true,
                    wordWrap = true,
                    fontSize = 10,
        };

        /// <summary>
        /// Default style for bold large font size title
        /// </summary>
        public static readonly GUIStyle BoldLargeTitleStyle = new GUIStyle()
        {
            fontSize = InspectorUIUtility.TitleFontSize,
            fontStyle = FontStyle.Bold,
        };
    }
}