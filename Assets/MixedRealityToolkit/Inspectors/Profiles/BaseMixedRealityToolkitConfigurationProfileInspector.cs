// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Core.Definitions;
using Microsoft.MixedReality.Toolkit.Core.Services;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor.Setup;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    [Obsolete("Use BaseMixedRealityToolkitConfigurationProfileInspector instead")]
    public abstract class MixedRealityBaseConfigurationProfileInspector { }

    /// <summary>
    /// Base class for all Mixed Reality Toolkit specific <see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.BaseMixedRealityProfile"/> inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityProfileInspector
    {
        public bool RenderAsSubProfile
        {
            set { renderAsSubProfile = value; }
        }

        [SerializeField]
        private Texture2D logoLightTheme = null;

        [SerializeField]
        private Texture2D logoDarkTheme = null;

        private bool renderAsSubProfile = false;

        protected virtual void Awake()
        {
            string assetPath = $"{MixedRealityEditorSettings.MixedRealityToolkit_RelativeFolderPath}/StandardAssets/Textures";

            if (logoLightTheme == null)
            {
                logoLightTheme = (Texture2D)AssetDatabase.LoadAssetAtPath($"{assetPath}/MRTK_Logo_Black.png", typeof(Texture2D));
            }

            if (logoDarkTheme == null)
            {
                logoDarkTheme = (Texture2D)AssetDatabase.LoadAssetAtPath($"{assetPath}/MRTK_Logo_White.png", typeof(Texture2D));
            }
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        protected void RenderMixedRealityToolkitLogo()
        {
            // If we're being rendered as a sub profile, don't show the logo
            if (renderAsSubProfile)
                return;

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? logoDarkTheme : logoLightTheme, GUILayout.MaxHeight(128f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(12f);
        }
        
        /// <summary>
        /// Renders a button that will take user back to a specified profile object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="activeObject"></param>
        /// <returns>True if button was clicked</returns>
        protected bool DrawBacktrackProfileButton(string message, UnityEngine.Object activeObject)
        {
            // If we're being rendered as a sub profile, don't show the button
            if (renderAsSubProfile)
                return false;

            if (GUILayout.Button(message))
            {
                Selection.activeObject = activeObject;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the profile is locked and displays a warning.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="lockProfile"></param>
        protected static void CheckProfileLock(Object target, bool lockProfile = true)
        {
            if (MixedRealityPreferences.LockProfiles && !((BaseMixedRealityProfile)target).IsCustomProfile)
            {
                EditorGUILayout.HelpBox("This profile is part of the default set from the Mixed Reality Toolkit SDK. You can make a copy of this profile, and customize it if needed.", MessageType.Warning);
                GUI.enabled = !lockProfile;
            }
        }
    }
}