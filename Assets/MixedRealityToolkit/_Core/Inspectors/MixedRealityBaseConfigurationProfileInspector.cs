// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Inspectors
{
    public abstract class MixedRealityBaseConfigurationProfileInspector : Editor
    {
        [SerializeField]
        private Texture2D logo = null;

        protected void Awake()
        {
            if (logo == null)
            {
                logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/MixedRealityToolkit/_Core/Resources/Textures/MRTK_Logo.png", typeof(Texture2D));
            }
        }

        protected void RenderMixedRealityToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(logo, GUILayout.MaxHeight(128f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }
    }
}