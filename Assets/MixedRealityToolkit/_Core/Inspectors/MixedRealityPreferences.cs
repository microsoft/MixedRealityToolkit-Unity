// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Core.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors
{
    internal static class MixedRealityPreferences
    {
        private static readonly GUIContent LockContent = new GUIContent("Lock SDK Profiles", "Locks the SDK profiles from being edited.");

        private static bool prefsLoaded;

        private static bool lockProfiles = true;

        /// <summary>
        /// Should the default profile inspectors be disabled to prevent editing?
        /// </summary>
        public static bool LockProfiles
        {
            get
            {
                // Load the preferences
                if (!prefsLoaded)
                {
                    lockProfiles = EditorPrefsUtility.GetEditorPref("_LockProfiles", true);
                    prefsLoaded = true;
                }

                return lockProfiles;
            }
            private set
            {
                lockProfiles = value;
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
                EditorPrefsUtility.SetEditorPref("_LockProfiles", LockProfiles);
            }

            if (!LockProfiles)
            {
                EditorGUILayout.HelpBox("This is only to be used to update the default SDK profiles. If any edits are made, and not checked into the MRTK's Github, the changes may be lost next time you update your local copy.", MessageType.Warning);
            }
        }
    }
}
