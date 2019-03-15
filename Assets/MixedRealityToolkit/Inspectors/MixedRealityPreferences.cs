// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    internal static class MixedRealityPreferences
    {
        #region Lock Profile Preferences

        private static readonly GUIContent LockContent = new GUIContent("Lock SDK profiles", "Locks the SDK profiles from being edited.\n\nThis setting only applies to the currently running project.");
        private const string LOCK_KEY = "LockProfiles";
        private static bool lockPrefLoaded;
        private static bool lockProfiles;

        /// <summary>
        /// Should the default profile inspectors be disabled to prevent editing?
        /// </summary>
        public static bool LockProfiles
        {
            get
            {
                if (!lockPrefLoaded)
                {
                    lockProfiles = EditorPreferences.Get(LOCK_KEY, true);
                    lockPrefLoaded = true;
                }

                return lockProfiles;
            }
            set => EditorPreferences.Set(LOCK_KEY, lockProfiles = value);
        }

        #endregion Lock Profile Preferences

        #region Ignore startup settings prompt

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore settings prompt on startup", "Prevents settings dialog popup from showing on startup.\n\nThis setting applies to all projects using MRTK.");
        private const string IGNORE_KEY = "MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
        private static bool ignorePrefLoaded;
        private static bool ignoreSettingsPrompt;

        /// <summary>
        /// Should the settings prompt show on startup?
        /// </summary>
        public static bool IgnoreSettingsPrompt
        {
            get
            {
                if (!ignorePrefLoaded)
                {
                    ignoreSettingsPrompt = EditorPrefs.GetBool(IGNORE_KEY, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set => EditorPrefs.SetBool(IGNORE_KEY, ignoreSettingsPrompt = value);
        }

        #endregion Ignore startup settings prompt

        #region Show Canvas Utility Prompt

        private static readonly GUIContent CanvasUtilityContent = new GUIContent("Canvas world space utility dialogs", "Enable or disable the dialog popups for the world space canvas settings.\n\nThis setting only applies to the currently running project.");
        private const string CANVAS_KEY = "EnableCanvasUtilityDialog";
        private static bool isCanvasUtilityPrefLoaded;
        private static bool showCanvasUtilityPrompt;

        /// <summary>
        /// Should the <see cref="Canvas"/> utility dialog show when updating the <see cref="RenderMode"/> settings on that component?
        /// </summary>
        public static bool ShowCanvasUtilityPrompt
        {
            get
            {
                if (!isCanvasUtilityPrefLoaded)
                {
                    showCanvasUtilityPrompt = EditorPreferences.Get(CANVAS_KEY, true);
                    isCanvasUtilityPrefLoaded = true;
                }

                return showCanvasUtilityPrompt;
            }
            set => EditorPreferences.Set(CANVAS_KEY, showCanvasUtilityPrompt = value);
        }

        #endregion Show Canvas Utility Prompt

        [SettingsProvider]
        private static SettingsProvider Preferences()
        {
            var provider = new SettingsProvider("Project/MRTK")
            {
                label = "MRTK",

                guiHandler = GUIHandler,

                keywords = new HashSet<string>(new[] { "Mixed", "Reality", "Toolkit" })
            };

            void GUIHandler(string searchContext)
            {
                var prevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 200f;

                EditorGUI.BeginChangeCheck();
                lockProfiles = EditorGUILayout.Toggle(LockContent, LockProfiles);

                // Save the preference
                if (EditorGUI.EndChangeCheck())
                {
                    LockProfiles = lockProfiles;
                }

                if (!LockProfiles)
                {
                    EditorGUILayout.HelpBox("This is only to be used to update the default SDK profiles. If any edits are made, and not checked into the MRTK's Github, the changes may be lost next time you update your local copy.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                ignoreSettingsPrompt = EditorGUILayout.Toggle(IgnoreContent, IgnoreSettingsPrompt);

                // Save the preference
                if (EditorGUI.EndChangeCheck())
                {
                    IgnoreSettingsPrompt = ignoreSettingsPrompt;
                }

                EditorGUI.BeginChangeCheck();
                showCanvasUtilityPrompt = EditorGUILayout.Toggle(CanvasUtilityContent, ShowCanvasUtilityPrompt);

                if (EditorGUI.EndChangeCheck())
                {
                    ShowCanvasUtilityPrompt = showCanvasUtilityPrompt;
                }

                if (!ShowCanvasUtilityPrompt)
                {
                    EditorGUILayout.HelpBox("Be aware that if a Canvas needs to receive input events it is required to have the CanvasUtility attached or the Focus Provider's UIRaycast Camera assigned to the canvas' camera reference.", MessageType.Warning);
                }

                EditorGUI.BeginChangeCheck();
                var scriptLock = EditorGUILayout.Toggle("Is Script Reloading locked?", EditorAssemblyReloadManager.LockReloadAssemblies);

                if (EditorGUI.EndChangeCheck())
                {
                    EditorAssemblyReloadManager.LockReloadAssemblies = scriptLock;
                }

                EditorGUIUtility.labelWidth = prevLabelWidth;
            }

            return provider;
        }
    }
}
