// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Base class for all Mixed Reality Toolkit specific <see cref="Microsoft.MixedReality.Toolkit.BaseMixedRealityProfile"/> inspectors to inherit from.
    /// </summary>
    public abstract class BaseMixedRealityToolkitConfigurationProfileInspector : BaseMixedRealityProfileInspector
    {
        public bool RenderAsSubProfile { get; set; }

        [SerializeField]
        private Texture2D logoLightTheme = null;

        [SerializeField]
        private Texture2D logoDarkTheme = null;

        [SerializeField]
        private static Texture helpIcon = null;

        private static GUIContent WarningIconContent = null;

        /// <summary>
        /// Internal enum used for back navigation along profile hiearchy. 
        /// Indicates what type of parent profile the current profile will return to for going back
        /// </summary>
        protected enum BackProfileType
        {
            Configuration,
            Input,
            RegisteredServices
        };

        protected virtual void Awake()
        {
            string assetPath = "StandardAssets/Textures";

            if (logoLightTheme == null)
            {
                logoLightTheme = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_Logo_Black.png"), typeof(Texture2D));
            }

            if (logoDarkTheme == null)
            {
                logoDarkTheme = (Texture2D)AssetDatabase.LoadAssetAtPath(MixedRealityToolkitFiles.MapRelativeFilePath($"{assetPath}/MRTK_Logo_White.png"), typeof(Texture2D));
            }

            if (helpIcon == null)
            {
                helpIcon = EditorGUIUtility.IconContent("_Help").image;
            }

            if (WarningIconContent == null)
            {
                WarningIconContent = new GUIContent(EditorGUIUtility.IconContent("console.warnicon").image,
                    "This profile is part of the default set from the Mixed Reality Toolkit SDK. You can make a copy of this profile, and customize it if needed.");
            }
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        protected void RenderMixedRealityToolkitLogo()
        {
            // If we're being rendered as a sub profile, don't show the logo
            if (RenderAsSubProfile)
            {
                return;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? logoDarkTheme : logoLightTheme, GUILayout.MaxHeight(96f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }


        protected bool DrawBacktrackProfileButton(BackProfileType returnProfileTarget = BackProfileType.Configuration)
        {
            string backText = string.Empty;
            BaseMixedRealityProfile backProfile = null;
            switch (returnProfileTarget)
            {
                case BackProfileType.Configuration:
                    backText = "Back to Configuration Profile";
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile;
                    break;
                case BackProfileType.Input:
                    backText = "Back to Input Profile";
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile;
                    break;
                case BackProfileType.RegisteredServices:
                    backText = "Back to Registered Service Providers Profile";
                    backProfile = MixedRealityToolkit.Instance.ActiveProfile.RegisteredServiceProvidersProfile;
                    break;
            }

            return DrawBacktrackProfileButton(backText, backProfile);
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
            if (RenderAsSubProfile)
            {
                return false;
            }

            if (GUILayout.Button(message))
            {
                Selection.activeObject = activeObject;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Helper function to render buttons correctly indented according to EditorGUI.indentLevel since GUILayout component don't respond naturally
        /// </summary>
        /// <param name="buttonText">text to place in button</param>
        /// <param name="options">layout options</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        protected static bool RenderIndentedButton(string buttonText, params GUILayoutOption[] options)
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
        protected static bool RenderIndentedButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            return RenderIndentedButton(() => { return GUILayout.Button(content, style, options); });
        }

        /// <summary>
        /// Helper function to support primary overloaded version of this functionality
        /// </summary>
        /// <param name="renderButton">The code to render button correctly based on parameter types passed</param>
        /// <returns>true if button clicked, false if otherwise</returns>
        private static bool RenderIndentedButton(Func<bool> renderButton)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15);
                result = renderButton();
            GUILayout.EndHorizontal();
            return result;
        }

        /// <summary>
        /// Helper function to render header correctly for all profiles
        /// </summary>
        /// <param name="title">Title of profile</param>
        /// <param name="description">profile tooltip describing purpose</param>
        /// <param name="backText">Text for back button if not rendering as sub-profile</param>
        /// <param name="backProfile">Target profile to return to if not rendering as sub-profile</param>
        /// <returns>true to render rest of profile as-is or false if profile/MRTK is in a state to not render rest of profile contents</returns>
        protected bool RenderProfileHeader(string title, string description, BackProfileType returnProfileTarget = BackProfileType.Configuration)
        {
            RenderMixedRealityToolkitLogo();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return false;
            }

            if (DrawBacktrackProfileButton(returnProfileTarget))
            {
                return false;
            }

            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent(title, description), EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);

            return true;
        }
    }
}