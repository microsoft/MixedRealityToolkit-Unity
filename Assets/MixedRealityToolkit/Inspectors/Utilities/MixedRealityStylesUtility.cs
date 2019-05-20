// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    public class MixedRealityStylesUtility
    {
        public static readonly GUIStyle BoldFoldoutStyle =
          new GUIStyle(EditorStyles.foldout)
          {
              fontStyle = FontStyle.Bold
          };
    }
}