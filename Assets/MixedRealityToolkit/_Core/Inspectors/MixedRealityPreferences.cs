// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    internal static class MixedRealityPreferences
    {
        private static readonly GUIContent LockContent = new GUIContent("Lock SDK Profiles", "Locks the SDK profiles from being edited.\n\nThis setting only applies to the currently running project.");
        private const string LockKey = "_LockProfiles";
        private static bool lockPrefLoaded;
        private static bool lockProfiles = true;

        private static readonly GUIContent IgnoreContent = new GUIContent("Ignore Settings Prompt on Startup", "Prevents settings dialog popup from showing on startup.\n\nThis setting applies to all projects using MRTK.");
        private const string IgnoreKey = "_MixedRealityToolkit_Editor_IgnoreSettingsPrompts";
        private static bool ignorePrefLoaded;
        private static bool ignoreSettingsPrompt = false;

        /// <summary>
        /// Should the default profile inspectors be disabled to prevent editing?
        /// </summary>
        public static bool LockProfiles
        {
            get
            {
                // Load the preferences
                if (!lockPrefLoaded)
                {
                    lockProfiles = EditorPrefsUtility.GetEditorPref(LockKey, true);
                    lockPrefLoaded = true;
                }

                return lockProfiles;
            }
            private set
            {
                lockProfiles = value;
            }
        }

        public static bool IgnoreSettingsPrompt
        {
            get
            {
                // Load the preferences
                if (!ignorePrefLoaded)
                {
                    ignoreSettingsPrompt = EditorPrefs.GetBool(IgnoreKey, false);
                    ignorePrefLoaded = true;
                }

                return ignoreSettingsPrompt;
            }
            set
            {
                ignoreSettingsPrompt = value;
            }
        }

        [PreferenceItem("Mixed Reality Toolkit")]
        private static void Preferences()
        {
            // Preferences GUI
            EditorGUI.BeginChangeCheck();
            LockProfiles = EditorGUILayout.Toggle(LockContent, LockProfiles);

            // Save the preferences
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefsUtility.SetEditorPref(LockKey, LockProfiles);
            }

            if (!LockProfiles)
            {
                EditorGUILayout.HelpBox("This is only to be used to update the default SDK profiles. If any edits are made, and not checked into the MRTK's Github, the changes may be lost next time you update your local copy.", MessageType.Warning);
            }

            EditorGUI.BeginChangeCheck();
            IgnoreSettingsPrompt = EditorGUILayout.Toggle(IgnoreContent, IgnoreSettingsPrompt);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(IgnoreKey, IgnoreSettingsPrompt);
            }
        }
    }
}
