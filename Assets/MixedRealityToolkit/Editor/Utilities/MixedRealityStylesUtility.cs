// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public static class MixedRealityStylesUtility
    {
        public static readonly GUIStyle BoldFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold
          };

        public static readonly GUIStyle ControllerButtonStyle = new GUIStyle("LargeButton")
        {
            imagePosition = ImagePosition.ImageAbove,
                    fontStyle = FontStyle.Bold,
                    stretchHeight = true,
                    stretchWidth = true,
                    wordWrap = true,
                    fontSize = 10,
        };
    }
}