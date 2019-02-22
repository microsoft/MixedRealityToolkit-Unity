// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    internal static class MixedRealityPreferences
    {
        #region Lock Profile Preferences

        private static readonly GUIContent LockContent = new GUIContent("Lock SDK Profiles", "Locks the SDK profiles from being edited.\n\nThis setting only applies to the currently running project.");
        private const string LockKey = "_LockProfiles";
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
                    lockProfiles = EditorPrefsUtility.GetEditorPref(LockKey, true);
                    lockPrefLoaded = true;
                }

                return lockProfiles;
            }
            set
            {
                EditorPrefsUtility.SetEditorPref(LockKey, lockProfiles = value);
            }
        }

        #endregion Lock Profile Preferences

        #region Ignore startup settings prompt

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore Settings Prompt on Startup", "Prevents settings dialog pop-up from showing on startup.\n\nThis setting applies to all projects using MRTK.");
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
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
                    ignoreSettingsPrompt = EditorPrefs.GetBool(IgnoreKey, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set
            {
                EditorPrefs.SetBool(IgnoreKey, ignoreSettingsPrompt = value);
            }
        }

        #endregion Ignore startup settings prompt

        #region Show Canvas Utility Prompt

        private static readonly GUIContent CanvasUtilityContent = new GUIContent("Canvas World Space utility dialogs", "Enable or disable the dialog pop-ups for the world space canvas settings.\n\nThis setting only applies to the currently running project.");
        private const string CanvasKey = "_EnableCanvasUtilityDialog";
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
                    showCanvasUtilityPrompt = EditorPrefsUtility.GetEditorPref("_EnableCanvasUtilityDialog", true);
                    isCanvasUtilityPrefLoaded = true;
                }

                return showCanvasUtilityPrompt;
            }
            set
            {
                EditorPrefsUtility.SetEditorPref(CanvasKey, showCanvasUtilityPrompt = value);
            }
        }

        #endregion Show Canvas Utility Prompt

        [PreferenceItem("Mixed Reality Toolkit")]
        private static void Preferences()
        {
            EditorGUI.BeginChangeCheck();
            lockProfiles = EditorGUILayout.Toggle(LockContent, LockProfiles);

            // Save the preference
            if (EditorGUI.EndChangeCheck())
            {
                LockProfiles = lockProfiles;
            }

            if (!LockProfiles)
            {
                EditorGUILayout.HelpBox("This is only to be used to update the default SDK profiles. If any edits are made, and not checked into the MRTK's GitHub, the changes may be lost next time you update your local copy.", MessageType.Warning);
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
        }
    }
}
