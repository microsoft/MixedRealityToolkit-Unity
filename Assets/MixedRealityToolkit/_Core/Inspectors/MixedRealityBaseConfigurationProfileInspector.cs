// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Internal.Managers;
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

        protected bool CheckMixedRealityManager()
        {
            if (!MixedRealityManager.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityManager>();

                if (managerSearch.Length == 0)
                {
                    EditorGUILayout.HelpBox("No Mixed Reality Manager found in scene.", MessageType.Error);
                    return false;
                }

                MixedRealityManager.ConfirmInitialized();
            }

            if (!MixedRealityManager.HasActiveProfile)
            {
                EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Manager.", MessageType.Error);
                return false;
            }

            return true;
        }
    }
}