// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.﻿

using Microsoft.MixedReality.Toolkit.Utilities.Editor;
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

        // TODO: Troy add comments
        protected static bool RenderIndentedButton(string buttonText, params GUILayoutOption[] options)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
                result = GUILayout.Button(buttonText, options);
            GUILayout.EndHorizontal();
            return result;
        }

        protected static bool RenderIndentedButton(GUIContent content, GUIStyle style, params GUILayoutOption[] options)
        {
            bool result = false;
            GUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 15);
                result = GUILayout.Button(content, style, options);
            GUILayout.EndHorizontal();
            return result;
        }

        // TODO: Troy add comments
        protected bool RenderProfileHeader(string title, string description, string backText, BaseMixedRealityProfile backProfile)
        {
            RenderMixedRealityToolkitLogo();

            if (!MixedRealityInspectorUtility.CheckMixedRealityConfigured())
            {
                return false;
            }

            if (DrawBacktrackProfileButton(backText, backProfile))
            {
                return false;
            }
            
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                //console.infoicon
                EditorGUILayout.LabelField(new GUIContent(helpIcon, description), GUILayout.Width(48));

                CheckProfileLock(target);

            EditorGUILayout.EndHorizontal();
            
            //EditorGUILayout.HelpBox(description, MessageType.Info);
            return true;
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
                EditorGUILayout.LabelField(WarningIconContent, GUILayout.Width(48));
                //EditorGUILayout.HelpBox("This profile is part of the default set from the Mixed Reality Toolkit SDK. You can make a copy of this profile, and customize it if needed.", MessageType.Warning);
                GUI.enabled = !lockProfile;
            }
        }
    }
}